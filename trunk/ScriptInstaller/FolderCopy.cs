using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace ScriptInstaller
{
    class IOHandler
    {
        static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();

        private static string _src = AppDomain.CurrentDomain.BaseDirectory;
        private static string _srcScript = System.IO.Path.Combine(_src, "cfg");
        private static string _srcScriptOptional = System.IO.Path.Combine(_srcScript, "optional files");
        private static string _srcHUD = System.IO.Path.Combine(_src, "HUDs");
        private static string _srcHUDNoid = System.IO.Path.Combine(_srcHUD, "noid hud");

        private static string regKey = ReadKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 440", "InstallLocation");
        private static string _dest = System.IO.Path.Combine(regKey, "tf");
        private string _destBackupPath = System.IO.Path.Combine(_dest, "backup");
        private static string _destScript = System.IO.Path.Combine(_dest, "cfg");

        private int _totalInstalled = 0;
        private int _totalAttemptedInstalled = 0;
        private int _totalUpdated = 0;
        private int _totalAttemptedUpdated = 0;

        /// <summary>
        /// Handles all file copying, including back ups.
        /// </summary>
        public IOHandler()
        {
            DirectoryInfo diSource = new DirectoryInfo(_srcScript);

            if (!File.Exists(_destBackupPath))
            {
                Directory.CreateDirectory(_destBackupPath);
            }
        }
        /// <summary>
        /// Copies recursively from source folder to the target.
        /// </summary>
        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            ///  Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(System.IO.Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
        public void CreateDirectory(string destination)
        {
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }
        }
        /// <summary>
        /// Copies a directory hierarchy recursively.
        /// </summary>
        /// <param name="diSource">source of the copy</param>
        /// <param name="diDestination">destination of the copy</param>
        public void CopyDirectoryTree(DirectoryInfo diSource, DirectoryInfo diDestination)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            // First, process all the files directly under this folder
            try
            {
                files = diSource.GetFiles("*.*");
            }
            // This is thrown if even one of the files requires permissions greater
            // than the application provides.
            catch (UnauthorizedAccessException e)
            {
                // This code just writes out the message and continues to recurse.
                // You may decide to do something different here. For example, you
                // can try to elevate your privileges and access the file again.
                Console.WriteLine(e.Message);
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                foreach (System.IO.FileInfo fi in files)
                {
                    try
                    {
                        //Console.WriteLine(@"Copying {0}\{1}\n", diDestination.FullName, fi.Name);
                        fi.CopyTo(System.IO.Path.Combine(diDestination.ToString(), fi.Name), true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                // Now find all the subdirectories under this directory.
                subDirs = diSource.GetDirectories();

                foreach (DirectoryInfo dirInfo in subDirs)
                {
                    DirectoryInfo nextDestinationSubDir = diDestination.CreateSubdirectory(dirInfo.Name);
                    // Resursive call for each subdirectory.
                    CopyDirectoryTree(dirInfo, nextDestinationSubDir);
                }
            }
        }
        /// <summary>
        /// Moves a folder to tf/backup/backup#/ and adds it to the list of backups.
        /// </summary>
        /// <param name="srcPath">Path to directory to back up.</param>
        /// <param name="folderName">Name of the folder to be backed up.</param>
        public void BackupFolder(string folderName, string srcPath)
        {
            if (Directory.Exists(System.IO.Path.Combine(srcPath, folderName)))
            {
                if (!Directory.Exists(_destBackupPath))
                {
                    Directory.CreateDirectory(_destBackupPath);
                }
                DirectoryInfo diSource = new DirectoryInfo(System.IO.Path.Combine(srcPath, folderName));
                DirectoryInfo diDestination = new DirectoryInfo(System.IO.Path.Combine(_destBackupPath, folderName));

                // for now we need to delete the destination path until we get a system in place for multiple backups
                if (Directory.Exists(diDestination.FullName))
                {
                    Directory.Delete(diDestination.FullName, true);
                }
                CopyDirectoryTree(diSource, diDestination);
            }
        }
        /// <summary>
        /// Reads a registry key and returns the value.
        /// </summary>
        /// <param name="Key">Key to be read.</param>
        /// <param name="SubKey">Subkey, or parameter, of a given key to be read.</param>
        /// <returns>value contained by the key</returns>
        private static string ReadKey(string Key, string SubKey)
        {
            RegistryKey regKey;
            string ver = string.Empty;
            try
            {
                regKey = Registry.LocalMachine.OpenSubKey(Key);
                ver = (string)regKey.GetValue(SubKey);
                regKey.Close();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return ver;
        }

    }
}
