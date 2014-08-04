using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PubComp.Testing.TestingUtils
{
    public static class DbDeployer
    {
#if DEBUG
        private static readonly bool IsDebug = true;
#else
        private static readonly bool IsDebug = false;
#endif

        public static void StartLocalDb(string instanceName, TestContext testContext)
        {
            var localDbExePath = @"C:\Program Files\Microsoft SQL Server\110\Tools\Binn\SqlLocalDB.exe";

            using (var process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = localDbExePath,
                    Arguments = @"stop " + instanceName + @" -k",
                    WindowStyle = IsDebug ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden,
                    CreateNoWindow = !IsDebug,
                }))
            {
                process.WaitForExit();
            }

            using (var process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = localDbExePath,
                    Arguments = @"create " + instanceName + @" -s",
                    WindowStyle = IsDebug ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden,
                    CreateNoWindow = !IsDebug,
                }))
            {
                process.WaitForExit();
            }
        }

        public static void DeployDacPac(string sqlPackagePath, string dacPacPath, string connectionString,
            TestContext testContext, bool recreateDatabase = true)
        {
            using (var process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = sqlPackagePath,
                    Arguments = @"/Action:Publish /SourceFile:" + dacPacPath
                        + @" /TargetConnectionString:" + '"' + connectionString + '"'
                        + @" /Properties:CreateNewDatabase=" + recreateDatabase
                        + @" /Properties:BlockOnPossibleDataLoss=False"
                        + @" /Quiet:True",
                    WindowStyle = IsDebug ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden,
                    CreateNoWindow = !IsDebug,
                }))
            {
                process.WaitForExit();
            }
        }
    }
}
