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

        // source folders
        private static string _src = AppDomain.CurrentDomain.BaseDirectory;
        public string Src
        {
            get { return _src; }
        }
        private static string _srcScript = System.IO.Path.Combine(_src, "cfg");
        public string SrcScript
        {
            get { return _srcScript; }
        }
        private static string _srcScriptOptional = System.IO.Path.Combine(_srcScript, "optional files");
        public string SrcScriptOptional
        {
            get { return _srcScriptOptional; }
        }
        private static string _srcHUD = System.IO.Path.Combine(_src, "HUDs");
        public string SrcHUD
        {
            get { return _srcHUD; }
        }
        private static string _srcHUDNoid = System.IO.Path.Combine(_srcHUD, "noid hud");
        public string SrcHUDNoid
        {
            get { return _srcHUDNoid; }
        }
        
        // registry key, stores the install information for tf2
        private static string regKey = ReadKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 440", "InstallLocation");

        // destination folders
        private static string _dest = System.IO.Path.Combine(regKey, "tf");
        public string Dest
        {
            get { return _dest; }
            set 
            { 
                _dest = value;
                _destBackupPath = System.IO.Path.Combine(_dest, "backup");
                _destScript = System.IO.Path.Combine(_dest, "cfg");
            }
        }

        private string _destBackupPath = System.IO.Path.Combine(_dest, "backup");
        public string DestBackupPath
        {
            get { return _destBackupPath; }
        }

        private string _destScript = System.IO.Path.Combine(_dest, "cfg");
        public string DestScript
        {
            get { return _destScript; }
        }

        /// <summary>
        /// Handles all file copying, including back ups.
        /// </summary>  
        public IOHandler()
        {
        }
        /// <summary>
        /// Checks to see if a directory exists and if it doesn't, creates it.
        /// </summary>
        /// <param name="destination">Destination of new directory.</param>
        public void CreateDirectory(string destination)
        {
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }
        }
        /// <summary>
        /// Checks whether a directory exists.
        /// </summary>
        /// <returns>True if directory exists.</returns>
        public bool DirExists(string directory)
        {
            return File.Exists(directory);
        }
        /// <summary>
        /// Checks whether a file exists.
        /// </summary>
        /// <returns>True if directory exists.</returns>
        public bool FileExists(string file)
        {
            return File.Exists(file);
        }
        public void Delete(string directory)
        {
            Directory.Delete(directory);
        }
        public void Delete(string directory, bool recursive)
        {
            Directory.Delete(directory, recursive);
        }
        public string[] GetAsStrings(string textFileLoc)
        {
            string[] currentFile = File.ReadAllLines(textFileLoc);
            return currentFile;
        }
        public string[] GetSubDirsAsString(string source)
        {
            DirectoryInfo diSource = new DirectoryInfo(source);

            // Now find all the subdirectories under this directory.
            System.IO.DirectoryInfo[] subDirs = diSource.GetDirectories();
            string[] subDirString = new String[subDirs.Length];

            for (int x = 0; x < subDirs.Length; x++)
            {
                subDirString[x] = subDirs[x].FullName;
            }
            return subDirString;
        }
        public string CreationTimeDir(string dir)
        {
            System.IO.DirectoryInfo dirInfo = new DirectoryInfo(dir);
            return dirInfo.CreationTime.ToString();
        }
        /// <summary>
        /// Copies a directory recursively.
        /// </summary>
        /// <param name="diSource">source of the copy</param>
        /// <param name="diDestination">destination of the copy</param>
        public void CopyDirectoryTree(string source, string destination)
        {
            DirectoryInfo diSource = new DirectoryInfo(source);
            DirectoryInfo diDestination = new DirectoryInfo(destination);
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;
            // First, process all the files directly under this folder
            files = diSource.GetFiles("*.*");
                /*
            catch (UnauthorizedAccessException e)
            {
                // This code just writes out the message and continues to recurse.
                // You may decide to do something different here. For example, you
                // can try to elevate your privileges and access the file again.
                log.Add(e.Message);
            }
                 */

            if (files != null)
            {
                if (Directory.Exists(diDestination.FullName) == false)
                {
                    Directory.CreateDirectory(diDestination.FullName);
                }
                
                foreach (System.IO.FileInfo fi in files)
                {
                    //Console.WriteLine(@"Copying {0}\{1}\n", diDestination.FullName, fi.Name);
                    fi.CopyTo(System.IO.Path.Combine(diDestination.ToString(), fi.Name), true);
                }
                    
                // Now find all the subdirectories under this directory.
                subDirs = diSource.GetDirectories();

                foreach (DirectoryInfo dirInfo in subDirs)
                {
                    DirectoryInfo nextDestinationSubDir = diDestination.CreateSubdirectory(dirInfo.Name);
                    // Resursive call for each subdirectory.
                    CopyDirectoryTree(dirInfo.FullName, nextDestinationSubDir.FullName);
                }
            }
        }
        /// <summary>
        /// Reads a registry key and returns the value.
        /// </summary>
        /// <param name="Key">Key to be read.</param>
        /// <param name="SubKey">Subkey, or parameter, of a given key to be read.</param>
        /// <returns>Value contained by the key</returns>
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
