using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

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

    Target CiBuildAndTest => _ => _
        .DependsOn(Compile)
        .Executes(() => DotNetTasks.DotNetTest(
            _ => _
                .EnableNoBuild()
                .EnableNoRestore()
                .SetConfiguration(configuration)));
}
