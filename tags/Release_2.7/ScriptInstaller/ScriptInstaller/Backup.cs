using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptInstaller
{
    class Backup
    {
        private static IOHandler fileHandler = new IOHandler();
        // private static string listLoc = System.IO.Path.Combine(fileHandler.DestBackupPath, "BackupList.txt");
        
        public Backup()
        {
        }
        public Backup(string backupName)
        {
        }
        public string[] BackupList()
        {
            string[] backupDirList = fileHandler.GetSubDirsAsString(fileHandler.DestBackupPath);

            // the number of fields
            int multiplier = 2;
            // total number of stored variables in abab format
            int numFields = backupDirList.Length * multiplier;

            string[] backupList = new string[numFields];
            int temp;
            for (int x = 0; x < numFields; x+=2)
            {
                temp = x / multiplier;
                backupList[x] = backupDirList[temp].Remove(0, fileHandler.DestBackupPath.Length + 1) + " (created " + 
                    fileHandler.CreationTimeDir(backupDirList[temp]) + ")";
                backupList[x+1] = backupDirList[temp];
            }
            return backupList;
        }
        /// <summary>
        /// Moves a folder to tf/backup/backup#/ and adds it to the list of backups.
        /// </summary>
        /// <param name="srcPath">Path to directory to back up.</param>
        /// <param name="folderNames">Names of the folder to be backed up.</param>
        /// <param name="sourcePath">Path to folders.</param>
        public void BackupFolders(string[] folderNames, string sourcePath)
        {
            foreach (string folderName in folderNames)
            {
                if (fileHandler.DirExists(System.IO.Path.Combine(fileHandler.Dest, folderName)))
                {
                    // creates the backup folder. This function automatically
                    // checks to see if it exists already.
                    fileHandler.CreateDirectory(fileHandler.DestBackupPath);
                    string source = System.IO.Path.Combine(sourcePath, folderName);
                    string destination = System.IO.Path.Combine(fileHandler.DestBackupPath, folderName);

                    // for now we need to delete the destination path until we get a system in place for multiple backups
                    if (fileHandler.DirExists(destination))
                    {
                        fileHandler.Delete(destination, true);
                    }
                    fileHandler.CopyDirectoryTree(source, destination);
                }
            }
        }
    }
}
