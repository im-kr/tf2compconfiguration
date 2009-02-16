using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptInstaller
{
    class Backup
    {
        private static IOHandler fileHandler = new IOHandler();
        private static string listLoc = System.IO.Path.Combine(fileHandler.DestBackupPath, "BackupList.txt");
        
        public Backup()
        {
        }
        public Backup(string backupName)
        {
        }
        public string[] BackupList()
        {
            if (fileHandler.FileExists(listLoc))
            {
                return fileHandler.getAsStrings(listLoc);
            }
            return null;
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
