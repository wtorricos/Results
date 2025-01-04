using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Tools.DotNet;

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
        GitHubActionsTrigger.Push, GitHubActionsTrigger.PullRequest
    },
    CacheKeyFiles = new[]
    {
        "**/global.json", "**/*.csproj", "Directory.Packages.props", ".config/dotnet-tools.json"
    },
    InvokedTargets = new[]
    {
        nameof(CiBuildAndTest)
    })]
sealed class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
        });

    Target Restore => _ => _
        .Executes(() => DotNetTasks.DotNetRestore());

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() => DotNetTasks.DotNetBuild(_ => _
            .SetConfiguration(configuration)));

    // Run with: dotnet nuke cibuildandtest --configuration Release
    // You will find the nuget package under MaybeResults/bin/Release/WTorricos.Results.x.x.x.nupkg
    Target CiBuildAndTest => _ => _
        .DependsOn(Compile)
        .Executes(() => DotNetTasks.DotNetTest(
            _ => _
                .EnableNoBuild()
                .EnableNoRestore()
                .SetConfiguration(configuration)));
}
