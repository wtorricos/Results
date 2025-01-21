using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.ReportGenerator;
using Serilog;

/*
Generate the configuration with: nuke --generate-configuration GitHubActions_ci --host GitHubActions

Note: `chmod +x build.cmd` needs to be manually added before running nuke when using ubuntu-latest
      - name: Run './build.cmd CiReportCoverage'
        run: |
          chmod +x build.sh
          ./build.sh CiReportCoverage
*/
[GitHubActions(
    name: "ci",
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = false,
    FetchDepth = 0,
    On = new[]
    {
        GitHubActionsTrigger.Push,
        GitHubActionsTrigger.PullRequest
    },
    CacheKeyFiles = new[]
    {
        "**/global.json",
        "**/*.csproj",
        "Directory.Packages.props",
        ".config/dotnet-tools.json"
    },
    InvokedTargets = new[]
    {
        nameof(CiBuildAndTest)
    })]
sealed class Build : NukeBuild
{

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    static Build() =>
        Environment.SetEnvironmentVariable(variable: "NO_LOGO", value: "true");

    [Parameter(description: "Coverage test threshold, defaults to 85%")]
    double CoverageThreshold { get; set; } = 0.85;

    [Solution(GenerateProjects = true)]
    Solution Solution { get; set; } = null!;

    static AbsolutePath CoverageDirectory => RootDirectory / "coverage";

    Target CleanGenerated => _ => _
        .Executes(() =>
        {
            CoverageDirectory.CreateOrCleanDirectory();
        });

    Target Clean => _ => _
        .DependsOn(CleanGenerated)
        .Before(Restore)
        .Executes(() => DotNetTasks.DotNetClean());

    Target Restore => _ => _
        .Executes(() => DotNetTasks.DotNetRestore());

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() => DotNetTasks.DotNetBuild(_ => _
            .SetConfiguration(configuration)));

    Target Test => target => target
        .DependsOn(CleanGenerated)
        .ProceedAfterFailure()
        .Executes(
            () =>
            {
                // Sample command: dotnet test --configuration Debug --collect "XPlat Code Coverage" --no-build --no-restore /property:CollectCoverage=True
                return DotNetTasks.DotNetTest(
                    config => config
                        .EnableNoBuild()
                        .EnableNoRestore()
                        .SetProjectFile(Solution)
                        .SetConfiguration(configuration)
                        .EnableCollectCoverage()
                        .SetDataCollector("XPlat Code Coverage")
                        .SetSettingsFile($"{Solution.ResultsBuild.Directory / "CodeCoverage.runsettings"}"));
            });

    Target TestCoverage => target => target
        .DependsOn(Test)
        .AssuredAfterFailure()
        .Produces(CoverageDirectory)
        .Executes(
            () =>
            {
                // Sample command: dotnet ReportGenerator -targetdir:C:\...\Results\coverage -reporttypes:Html;Cobertura -reports:C:\...\Results\**\coverage.cobertura.xml
                _ = ReportGeneratorTasks.ReportGenerator(
                    s => s
                        .SetTargetDirectory(CoverageDirectory)
                        .SetReportTypes(ReportTypes.Html, ReportTypes.Cobertura)
                        .SetReports(Solution.Directory / "**" / "coverage.cobertura.xml"));

                // Path to the coverage file
                AbsolutePath coverageFile = CoverageDirectory / "Cobertura.xml";

                // Parse the coverage file to get the coverage percentage
                XDocument doc = XDocument.Load(coverageFile);
                double coverage = doc
                    .Descendants("coverage")
                    .Where(x => x.Attribute("line-rate")?.Value.Length > 0)
                    .Select(
                        x => double.Parse(
                            s: x.Attribute("line-rate")?.Value ?? "0",
                            NumberStyles.Number,
                            CultureInfo.InvariantCulture))
                    .First();

                if (coverage < CoverageThreshold)
                {
                    Log.Error($"Code coverage ({coverage:P}) is below the threshold ({CoverageThreshold:P}).");
                    throw new InvalidOperationException("Build failed due to insufficient code coverage.");
                }

                Log.Information($"Code coverage ({coverage:P}) meets the threshold ({CoverageThreshold:P}).");
            });

    // Run with: dotnet nuke cibuildandtest
    // You will find the nuget package under MaybeResults/bin/Release/WTorricos.Results.x.x.x.nupkg
    Target CiBuildAndTest => _ => _
        .DependsOn(TestCoverage)
        .Executes(() => DotNetTasks.DotNetBuild(
            _ => _.SetConfiguration(Configuration.Release)));

    public static int Main() => Execute<Build>(x => x.Compile);
}
