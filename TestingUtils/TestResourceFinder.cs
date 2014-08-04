using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PubComp.Testing.TestingUtils
{
    public static class TestResourceFinder
    {
        public static string BuildTemplateSourcesDir { get; set; }
        public static string BuildTemplateBinariesDir { get; set; }

        static TestResourceFinder()
        {
            BuildTemplateSourcesDir = @"src";
            BuildTemplateBinariesDir = @"bin";
        }

        public static void FindResources(
            TestContext testContext,
            string rootDirName,
            string testDirName,
            bool isAnyCpu,
            bool isDebug,
            out string rootPath,
            out string testSrcDir,
            out string testBinDir,
            out string testRunDir,
            out bool isLocal)
        {
            FindResourcesInternal(testContext,
                rootDirName,
                testDirName,
                isAnyCpu,
                isDebug,
                false,
                out rootPath,
                out testSrcDir,
                out testBinDir,
                out testRunDir,
                out isLocal);
        }

        public static void FindResourcesForWebApplication(
            TestContext testContext,
            string rootDirName,
            string testDirName,
            bool isAnyCpu,
            bool isDebug,
            out string rootPath,
            out string testSrcDir,
            out string testBinDir,
            out string testRunDir,
            out bool isLocal)
        {
            FindResourcesInternal(testContext,
                rootDirName,
                testDirName,
                isAnyCpu,
                isDebug,
                true,
                out rootPath,
                out testSrcDir,
                out testBinDir,
                out testRunDir,
                out isLocal);
        }

        private static void FindResourcesInternal(
            TestContext testContext,
            string rootDirName,
            string testDirName,
            bool isAnyCpu,
            bool isDebug,
            bool isWebApplication,
            out string rootPath,
            out string testSrcDir,
            out string testBinDir,
            out string testRunDir,
            out bool isLocal)
        {
            testRunDir = testContext.DeploymentDirectory;
            isLocal = false;

            if (GetLocalRunRootPath(testContext, rootDirName, testDirName, isAnyCpu, isDebug, isWebApplication, out rootPath,
                out testSrcDir, out testBinDir))
            {
                isLocal = true;
                return;
            }

            if (GetBuildServerRootPath(testContext, rootDirName, testDirName, out rootPath, out testSrcDir, out testBinDir))
            {
                return;
            }

            testSrcDir = null;
            testBinDir = null;
            testRunDir = null;
            Assert.Fail("Could not find test resources directory. Search started at: " + testContext.DeploymentDirectory);
        }

        public static void CopyResources(
            string sourceDir, string destDir, string filter = "*.*")
        {
            foreach (var file in Directory.EnumerateFiles(sourceDir, filter))
            {
                var src = file;
                var dest = destDir + @"\" + Path.GetFileName(file);
                if (!File.Exists(dest))
                {
                    File.Copy(src, dest, false);
                    File.SetAttributes(dest, FileAttributes.Normal);
                }
            }

            foreach (var dir in Directory.EnumerateDirectories(sourceDir))
            {
                var src = dir;
                var dest = destDir + @"\" + Path.GetFileName(dir);
                CopyResources(src, dest, filter);
            }
        }

        private static bool GetLocalRunRootPath(
            TestContext testContext, String rootDirName, String testDirName, bool isAnyCpu, bool isDebug,
            bool isWebApplication, out String rootPath, out String testSrcDir, out String testBinDir)
        {
            var current = Path.GetDirectoryName(testContext.DeploymentDirectory);
            Assert.IsNotNull(current, "Could not located test run directory");

            while (!current.EndsWith(@"\" + rootDirName))
            {
                current = Path.GetFullPath(current + @"\..");

                if (current == Path.GetPathRoot(current))
                {
                    rootPath = null;
                    testSrcDir = null;
                    testBinDir = null;
                    return false;
                }
            }

            rootPath = current;
            testSrcDir = rootPath + @"\" + testDirName;
            testBinDir = rootPath + @"\" + testDirName + GetLocalBinOffset(isAnyCpu, isDebug, isWebApplication);
            return true;
        }

        private static bool GetBuildServerRootPath(
            TestContext testContext, String rootDirName, String testDirName,
            out String rootPath, out String testSrcDir, out String testBinDir)
        {
            var current = Path.GetDirectoryName(testContext.DeploymentDirectory);

            while (current != Path.GetPathRoot(current))
            {
                var sourcesPath = Path.GetFullPath(current + @"\" + BuildTemplateSourcesDir);
                var binariesPath = Path.GetFullPath(current + @"\" + BuildTemplateBinariesDir);
                var root = sourcesPath + @"\" + rootDirName;

                if (Directory.Exists(sourcesPath) && Directory.Exists(binariesPath) && Directory.Exists(root))
                {
                    rootPath = root;
                    testSrcDir = root + @"\" + testDirName;
                    testBinDir = binariesPath;
                    return true;
                }

                current = Path.GetFullPath(current + @"\..");
            }

            rootPath = null;
            testSrcDir = null;
            testBinDir = null;
            return false;
        }

        private static string GetLocalBinOffset(bool isAnyCpu, bool isDebug, bool isWebApplication)
        {
            var binOffSet = @"\bin";
            if (!isAnyCpu)
            {
                if (IntPtr.Size == 8)
                    binOffSet += @"\x64";
                else
                    binOffSet += @"\x86";
            }

            if (!isWebApplication)
            {
                if (isDebug)
                    binOffSet += @"\Debug";
                else
                    binOffSet += @"\Release";
            }

            return binOffSet;
        }
    }
}
