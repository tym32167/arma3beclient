using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;

namespace Arma3BE.Client.Updater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var title = $"TEHGAM.COM - Arma 3 BattlEye Tool Updater v.{version.Major}.{version.Minor}.{version.Build}";
            Title = title;

            InitFromStartupArgs();

            try
            {
                RunUpdate();
            }
            catch (Exception e)
            {
                MessageBox.Show(this, "Update failed. " + e.Message, "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        private bool _needRestart = false;
        private string _originPath = string.Empty;

        private void InitFromStartupArgs()
        {
            var args = Environment.GetCommandLineArgs();

            if (args.Length == 1) throw new Exception("invalid startup args");
            _originPath = args[1];

            if (Directory.Exists(_originPath) == false) throw new Exception("invalid directory");
            _needRestart = args.Contains("RESTART");
        }

        private async void RunUpdate()
        {
            using (var client = new WebClient())
            {
                var str = await client.DownloadStringTaskAsync(new Uri("https://ci.appveyor.com/api/projects/ArtemMuradov/arma3beclient/"));

                var data = JsonConvert.DeserializeObject<RootObject>(str);

                if (data?.build == null)
                {
                    AppendLog("No build found");

                    MessageBox.Show(this, "Update failed", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }
                if (data.build.status == "success" && data.build.jobs?.Count == 1)
                {
                    var origDir = _originPath;
                    var tempDirName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    if (Directory.Exists(tempDirName) == false) Directory.CreateDirectory(tempDirName);
                    AppendLog($"Temporary directory: {tempDirName}");

                    var downloadUri = $"https://ci.appveyor.com/api/buildjobs/{data.build.jobs[0].jobId}/artifacts/src%2FArma3BE.Client%2Fbin%2FArma3BEClient.zip";
                    AppendLog($"downloadUri {downloadUri}");

                    var fileName = Path.Combine(tempDirName, "data.zip");
                    await client.DownloadFileTaskAsync(downloadUri, fileName);
                    AppendLog($"File {fileName} downloaded");

                    var directory = Path.Combine(tempDirName, "extracted");
                    ZipFile.ExtractToDirectory(fileName, directory);
                    AppendLog($"File {fileName} extracted");

                    foreach (var file in Directory.GetFiles(origDir).Where(x => new FileInfo(x).Name != "Database.sdf"))
                    {
                        AppendLog($"Deletinng file {file}");
                        File.Delete(file);
                    }

                    foreach (var dir in Directory.GetDirectories(origDir).Where(x => new DirectoryInfo(x).Name != "Logs"))
                    {
                        AppendLog($"Deletinng directory {dir}");
                        Directory.Delete(dir, true);
                    }

                    foreach (var file in Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories))
                    {
                        var dest = file.Replace(directory, origDir);
                        AppendLog($"Copiyng file {dest}");
                        var dir = Path.GetDirectoryName(dest);
                        if (Directory.Exists(dir) == false) Directory.CreateDirectory(dir);
                        File.Copy(file, dest);
                    }

                    AppendLog($"Deleting temp directory {tempDirName}");
                    Directory.Delete(tempDirName, true);

                    AppendLog("Done.");

                    MessageBox.Show(this, "Update finished", "Update finished successfully.", MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    var exeFile = Path.Combine(_originPath, "Arma3BEClient.exe");

                    Process.Start(exeFile);
                    Application.Current.Shutdown();
                }
            }
        }


        void AppendLog(string message)
        {
            tbOut.Text += $"{message}{Environment.NewLine}";
            sv.ScrollToEnd();
        }
    }


    #region DTOs



    public class NuGetFeed
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool publishingEnabled { get; set; }
        public string created { get; set; }
    }

    public class AccessRightDefinition
    {
        public string name { get; set; }
        public string description { get; set; }
    }

    public class AccessRight
    {
        public string name { get; set; }
        public bool allowed { get; set; }
    }

    public class RoleAce
    {
        public int roleId { get; set; }
        public string name { get; set; }
        public bool isAdmin { get; set; }
        public List<AccessRight> accessRights { get; set; }
    }

    public class SecurityDescriptor
    {
        public List<AccessRightDefinition> accessRightDefinitions { get; set; }
        public List<RoleAce> roleAces { get; set; }
    }

    public class Project
    {
        public int projectId { get; set; }
        public int accountId { get; set; }
        public string accountName { get; set; }
        public List<object> builds { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string repositoryType { get; set; }
        public string repositoryScm { get; set; }
        public string repositoryName { get; set; }
        public string repositoryBranch { get; set; }
        public bool isPrivate { get; set; }
        public bool skipBranchesWithoutAppveyorYml { get; set; }
        public bool enableSecureVariablesInPullRequests { get; set; }
        public bool enableSecureVariablesInPullRequestsFromSameRepo { get; set; }
        public bool enableDeploymentInPullRequests { get; set; }
        public bool rollingBuilds { get; set; }
        public bool alwaysBuildClosedPullRequests { get; set; }
        public string tags { get; set; }
        public NuGetFeed nuGetFeed { get; set; }
        public SecurityDescriptor securityDescriptor { get; set; }
        public string created { get; set; }
        public string updated { get; set; }
    }

    public class Job
    {
        public string jobId { get; set; }
        public string name { get; set; }
        public bool allowFailure { get; set; }
        public int messagesCount { get; set; }
        public int compilationMessagesCount { get; set; }
        public int compilationErrorsCount { get; set; }
        public int compilationWarningsCount { get; set; }
        public int testsCount { get; set; }
        public int passedTestsCount { get; set; }
        public int failedTestsCount { get; set; }
        public int artifactsCount { get; set; }
        public string status { get; set; }
        public string started { get; set; }
        public string finished { get; set; }
        public string created { get; set; }
        public string updated { get; set; }
    }

    public class Build
    {
        public int buildId { get; set; }
        public List<Job> jobs { get; set; }
        public int buildNumber { get; set; }
        public string version { get; set; }
        public string message { get; set; }
        public string branch { get; set; }
        public bool isTag { get; set; }
        public string commitId { get; set; }
        public string authorName { get; set; }
        public string authorUsername { get; set; }
        public string committerName { get; set; }
        public string committerUsername { get; set; }
        public string committed { get; set; }
        public List<object> messages { get; set; }
        public string status { get; set; }
        public string started { get; set; }
        public string finished { get; set; }
        public string created { get; set; }
        public string updated { get; set; }
    }

    public class RootObject
    {
        public Project project { get; set; }
        public Build build { get; set; }
    }

    #endregion
}