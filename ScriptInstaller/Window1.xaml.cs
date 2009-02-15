
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace ScriptInstaller
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {

        private static string _src = AppDomain.CurrentDomain.BaseDirectory;
        private static string _srcScript = System.IO.Path.Combine(_src, "cfg");
        private static string _srcScriptOptional = System.IO.Path.Combine(_srcScript, "optional files");
        private static string _srcHUD = System.IO.Path.Combine(_src, "HUDs");
        private static string _srcHUDNoid = System.IO.Path.Combine(_srcHUD, "noid hud");

        private static string regKey = ReadKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 440", "InstallLocation");
        private static string _dest = System.IO.Path.Combine(regKey, "tf");
        private static string _destBackupPath = _dest + "backup\\";
        private static string _destScript = System.IO.Path.Combine(_dest, "cfg");

        private int _totalInstalled = 0;
        private int _totalAttemptedInstalled = 0;
        private int _totalUpdated = 0;
        private int _totalAttemptedUpdated = 0;

        private IOHandler fileHandler = new IOHandler();
        public Window1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Get the server information from the text box.
        /// </summary>
        /// <param name="x">The number of the text box to retrieve.</param>
        /// <returns>User entered data from the textbox, either nothing or a connect line.</returns>
        private string getServerInfo (int num)
        {
            switch(num)
            {
                case 1: return server1info.Text; 
                case 2: return server2info.Text;
                case 3: return server3info.Text;
                case 4: return server4info.Text;
            }
            return "error";
        }
        /// <summary>
        /// Get the server RCON from the text box.
        /// </summary>
        /// <param name="x">The number of the text box to retrieve.</param>
        /// <returns>User entered data from the textbox, either nothing or a RCON.</returns>
        private string getServerRcon (int num)
        {
            switch(num)
            {
                case 1: return server1rcon.Text; 
                case 2: return server2rcon.Text;
                case 3: return server3rcon.Text;
                case 4: return server4rcon.Text;
            }
            return "error";
        }
        /// <summary>
        /// Executed when install button is pressed.
        /// </summary>
        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {            
            _totalInstalled = 0;
            _totalAttemptedInstalled = 0;
            _totalUpdated = 0;
            _totalAttemptedUpdated = 0;
            string script_path = "cfg\\";
            string newPath;

            MessageBoxResult reply = MessageBox.Show("The script will backup your files but your tf2 folders will be altered if you continue. Old backups will be overwritten.\n\n Are you sure you want to continue?", "Are you sure?",MessageBoxButton.YesNo);

            if (reply == MessageBoxResult.Yes)
            {
                if (noidHUDCheckBox.IsChecked == true)
                {
                    Console.WriteLine("HUD =======================================================");

                    fileHandler.BackupFolder("scripts", _dest);
                    fileHandler.BackupFolder("resource", _dest);

                    DirectoryInfo srcPath = new DirectoryInfo(_srcHUDNoid);
                    DirectoryInfo destPath = new DirectoryInfo(_dest);
                    fileHandler.CopyDirectoryTree(srcPath, destPath);
                }
                if (coreScript.IsChecked == true)
                {
                    Console.WriteLine("Script =======================================================");
                    fileHandler.BackupFolder("cfg", _dest);

                    //copy the files over
                    DirectoryInfo srcPath = new DirectoryInfo(_srcScript);
                    DirectoryInfo destPath = new DirectoryInfo(_destScript);
                    fileHandler.CopyDirectoryTree(srcPath, destPath);
                    /*
                    // lines to replace in the file if you want primary reload turned off
                    ReplacePair primaryReloadDisable = new ReplacePair();
                    primaryReloadDisable.ReplaceMe = "alias \"class_primary\" \"alias autoreload autoreload_on\"";
                    primaryReloadDisable.NewLine = "alias \"class_primary\" \"alias autoreload autoreload_off\"";
                    // lines to replace in the file if you want primary reload turned off
                    ReplacePair secondaryReloadDisable = new ReplacePair();
                    secondaryReloadDisable.ReplaceMe = "alias \"class_secondary\" \"alias autoreload autoreload_on\"";
                    secondaryReloadDisable.NewLine = "alias \"class_secondary\" \"alias autoreload autoreload_off\"";


                    // copy files
                    // autoexec.cfg
                    if (File.Exists(System.IO.Path.Combine(_dest, "autoexec.cfg       //where the magic happens")))
                        File.AppendAllText(_dest + "autoexec.cfg", "exec competitive.cfg           //where the magic happens");
                    else
                        copyFile("autoexec.cfg", _srcScript, script_path);
                    // competitive.cfg
                    if (minusfps.IsChecked == true)
                    {
                        ReplacePair lowerMaxFPS = new ReplacePair();
                        lowerMaxFPS.ReplaceMe = "fps_max \"0\"													// Set this to something maintainable for your system. 0 If you consistently keep 70+ fps";
                        lowerMaxFPS.NewLine = "fps_max \"67\"													// Set this to something maintainable for your system. 0 If you consistently keep 70+ fps";
                        copyFile("competitive.cfg", _srcScript, lowerMaxFPS);
                    }
                    else
                        copyFile("competitive.cfg", _srcScript, script_path);
                    // sniper.cfg
                    copyFile("sniper.cfg", _srcScript, script_path);
                    // spy.cfg
                    copyFile("spy.cfg", _srcScript, script_path);
                    // soldier.cfg
                    if (SoldierReloadCheckBox.IsChecked == true)
                    {
                        copyFile("soldier.cfg", _srcScript, script_path);
                    }
                    else
                    {
                        Collection<ReplacePair> soldierLines = new Collection<ReplacePair>();
                        soldierLines.Add(primaryReloadDisable);
                        soldierLines.Add(secondaryReloadDisable);
                        copyFile("soldier.cfg", soldierLines);
                        Console.WriteLine(" -- Primary and Secondary AutoReload removed from file.");
                    }
                    // engineer.cfg
                    if (AutoPistolCheckBox.IsChecked == false && EngineerReloadCheckBox.IsChecked == false)
                    {
                        Console.Write("Autopistol off version of ");
                        copyFile("engineer.cfg", _srcScriptOptional, primaryReloadDisable);
                        Console.WriteLine(" -- Primary AutoReload removed from file.");
                    }
                    else if (AutoPistolCheckBox.IsChecked == false && EngineerReloadCheckBox.IsChecked == true)
                    {
                        Console.Write("Autopistol off, reload version of ");
                        copyFile("engineer.cfg", _srcScriptOptional, script_path);
                    }
                    else if (AutoPistolCheckBox.IsChecked == true && EngineerReloadCheckBox.IsChecked == false)
                    {
                        Console.Write("Autopistol version of ");
                        copyFile("engineer.cfg", _srcScript, primaryReloadDisable);
                        Console.WriteLine(" -- Primary AutoReload removed from file.");
                    }
                    else if (AutoPistolCheckBox.IsChecked == true && EngineerReloadCheckBox.IsChecked == true)
                    {
                        Console.Write("Autopistol version of ");
                        copyFile("engineer.cfg", _srcScript, primaryReloadDisable);
                    }
                    // heavyweapons.cfg
                    if (HeavyReloadCheckBox.IsChecked == true)
                    {
                        copyFile("heavyweapons.cfg", _srcScript, script_path);
                    }
                    else
                    {
                        copyFile("heavyweapons.cfg", _srcScript, secondaryReloadDisable);
                        Console.WriteLine(" -- Secondary AutoReload removed from file.");
                    }
                    // pyro.cfg
                    if (PyroReloadCheckBox.IsChecked == true)
                    {
                        copyFile("pyro.cfg", _srcScript, script_path);
                    }
                    else
                    {
                        copyFile("pyro.cfg", _srcScript, secondaryReloadDisable);
                        Console.WriteLine(" -- Secondary AutoReload removed from file.");
                    }
                    // medic.cfg
                    if (AutohealCheckBox.IsChecked == true)
                    {
                        Console.Write("Autoheal version of ");
                        copyFile("medic.cfg", _srcScriptOptional, script_path);
                    }
                    else
                    {
                        Console.Write("Regular (non-Autoheal) version of ");
                        copyFile("medic.cfg", _srcScript, script_path);
                    }
                    // scout.cfg
                    if (AutoPistolCheckBox.IsChecked == false && ScoutReloadCheckBox.IsChecked == false)
                    {
                        Console.Write("Autopistol off, no reload version of ");
                        copyFile("scout.cfg", _srcScriptOptional, primaryReloadDisable);
                        Console.WriteLine(" -- Primary AutoReload removed from file.");
                    }
                    else if (AutoPistolCheckBox.IsChecked == false && ScoutReloadCheckBox.IsChecked == true)
                    {
                        Console.Write("Autopistol off, reload version of ");
                        copyFile("scout.cfg", _srcScriptOptional, script_path);
                    }
                    else if (AutoPistolCheckBox.IsChecked == true && ScoutReloadCheckBox.IsChecked == false)
                    {
                        Console.Write("Autopistol version of ");
                        copyFile("scout.cfg", _srcScript, primaryReloadDisable);
                        Console.WriteLine(" -- Primary AutoReload removed from file.");
                    }
                    else if (AutoPistolCheckBox.IsChecked == true && ScoutReloadCheckBox.IsChecked == true)
                    {
                        Console.Write("Autopistol version of ");
                        copyFile("scout.cfg", _srcScript, primaryReloadDisable);
                    }
                    // demoman.cfg
                    if (DemoReloadCheckBox.IsChecked == true)
                    {
                        copyFile("demoman.cfg", _srcScript, script_path);
                    }
                    else
                    {
                        copyFile("demoman.cfg", _srcScriptOptional, script_path);
                        Console.WriteLine(" -- Primary and Secondary AutoReload removed from file.");
                    }
                     */
                    // servers.cfg
                    Collection<ReplacePair> linesToReplace = new Collection<ReplacePair>();
                    for (int x = 0; x < 4; x++)
                    {
                        ReplacePair lineInfo = new ReplacePair();
                        lineInfo.ReplaceMe = String.Concat("alias server", x + 1, " \"\"");
                        lineInfo.NewLine = String.Concat("alias server", x + 1, " \"", getServerInfo(x + 1), "\"");
                        //MessageBox.Show("ReplaceMe: " + lineInfo.ReplaceMe + "\n Newline: " + lineInfo.NewLine);
                        linesToReplace.Add(lineInfo);
                        ReplacePair lineRcon = new ReplacePair();
                        lineRcon.ReplaceMe = String.Concat("alias server_rcon", x + 1, " \"\"");
                        lineRcon.NewLine = String.Concat("alias server_rcon", x + 1, " \"rcon_password ", getServerRcon(x + 1), "\"");
                        //MessageBox.Show("ReplaceMe RCON: " + lineRcon.ReplaceMe + "\n Newline RCON: " + lineRcon.NewLine);
                        linesToReplace.Add(lineRcon);
                    }
                    copyFile("servers.cfg", linesToReplace);

                    // default_class.cfg
                    copyFile("default_class.cfg", _srcScript, script_path);
                    
                    //hotkeyList();
                }
            }
            MessageBox.Show("Successfully installed " + _totalInstalled + " / " + _totalAttemptedInstalled + " files and updated " + _totalUpdated + " / " + _totalAttemptedUpdated,"Success!");
        }
        private void hotkeyList()
        {
            if(System.IO.File.Exists(System.IO.Path.Combine(_srcScript, "hotkeys.txt")))
            {
                Collection<ReplacePair> hotkeysToReplace = new Collection<ReplacePair>();
                
                string temp = "";
                string[] currentFile = File.ReadAllLines(System.IO.Path.Combine(_srcScript, "hotkeys.txt"));

                foreach (string line in currentFile)
                {
                    if (line.Contains("bind "))
                    {
                        temp = line.Replace("bind ", "");
                        string[] tempLines = temp.Split(new char[] { ' ' }, StringSplitOptions.None);
                        ReplacePair hotkey = new ReplacePair();
                        hotkey.ReplaceMe = tempLines[0];
                        for (int x = 1; x < tempLines.Length; x++)
                        {
                            hotkey.NewLine += tempLines[x] + " ";
                        }
                        hotkeysToReplace.Add(hotkey);
                        continue;
                    }
                }
                updateHotkeys("config.cfg", hotkeysToReplace);
            }
            else
            {
                MessageBox.Show("Could not find hotkey list.");
            }
        }
        //=====================================
        // Replace/Add Hotkeys
        //   -- Copies a file but updates hotkeys and adds hotkeys in if they don't exist already
        //=====================================
        private void updateHotkeys(string file, Collection<ReplacePair> hotkeysToReplace)
        {
            _totalAttemptedUpdated++;
            string targetFilename = _dest + file;

            StringBuilder newFile = new StringBuilder();
            string temp = "";
            string[] currentFile = File.ReadAllLines(_destScript + file);

            Collection<ReplacePair> addedHotkeys = new Collection<ReplacePair>();
            foreach (string line in currentFile)
            {
                bool hasHotkey = false;
                foreach (ReplacePair hotkey in hotkeysToReplace)
                {
                    if (line.Contains(hotkey.ReplaceMe))
                    {
                        temp = "bind " + hotkey.ReplaceMe + " " + hotkey.NewLine;
                        newFile.Append(temp + "\r\n");
                        addedHotkeys.Add(hotkey);
                        hasHotkey = true;
                        continue;
                    }
                }
                if (!hasHotkey)
                {
                    newFile.Append(line + "\r\n");
                }
            }
            foreach (ReplacePair hotkey in hotkeysToReplace)
            {
                if (!addedHotkeys.Contains(hotkey))
                {
                    temp = "bind " + hotkey.ReplaceMe + " " + hotkey.NewLine;
                    newFile.Append(temp + "\r\n");
                }
            }
            try
            {
                string temp2 = _dest + "cfg\\";
                File.WriteAllText(System.IO.Path.Combine(temp2, file), newFile.ToString());
                Console.WriteLine(file + " updated successfully.");
                _totalUpdated++;
            }
            catch
            {
                errorMessage(file);
            }
        }
        //=====================================
        // CopyFile Remove Collection of Lines
        //   -- Copies a file but removes a collection of paired lines (line to replace and new line)
        //=====================================
        private void copyFile(string file, Collection<ReplacePair> linesToReplace)
        {
            _totalAttemptedInstalled++;
            string targetFilename = _dest + file;

            StringBuilder newFile = new StringBuilder();
            string temp = "";
            string[] currentFile = File.ReadAllLines(System.IO.Path.Combine(_srcScript,file));

            foreach (string line in currentFile)
            {
                bool continueOuter = true;
                foreach (ReplacePair swap in linesToReplace)
                {
                    if (line.Contains(swap.ReplaceMe))
                    {
                        temp = line.Replace(swap.ReplaceMe, swap.NewLine);
                        newFile.Append(temp + "\r\n");
                        continueOuter = false;
                        continue;
                    }
                }
                if (continueOuter)
                    newFile.Append(line + "\r\n");
            }
            try
            {
                // Try to copy the same file again, which should succeed.
                string temp2 = System.IO.Path.Combine(_dest, "cfg");
                File.WriteAllText(System.IO.Path.Combine(temp2, file), newFile.ToString());
                Console.WriteLine(file + " copied successfully.");
                _totalInstalled++;
            }
            catch
            {
                errorMessage(file);
            }
        }
        //========================
        // CopyFile Remove Lines
        //   -- Copies a file but removes a line
        //========================
        private void copyFile(string file, string fileSource, ReplacePair swap)
        {
            _totalAttemptedInstalled++;
            string targetFilename = _dest + file;

            StringBuilder newFile = new StringBuilder();
            string temp = "";
            string[] currentFile = File.ReadAllLines(System.IO.Path.Combine(_srcScript, file));

            foreach (string line in currentFile)
            {
                if (line.Contains(swap.ReplaceMe))
                {
                    temp = line.Replace(swap.ReplaceMe, swap.NewLine);

                    newFile.Append(temp + "\r\n");

                    continue;
                }
                newFile.Append(line + "\r\n");
            }
            try
            {
                // Try to copy the same file again, which should succeed.
                string temp2 = System.IO.Path.Combine(_dest, "cfg");
                File.WriteAllText(System.IO.Path.Combine(temp2, file), newFile.ToString());
                Console.WriteLine(file + " copied successfully.");
                _totalInstalled++;
            }
            catch
            {
                errorMessage(file);
            }
        }
        //==========
        // CopyFile
        //   -- Copies a file
        //==========
        private void copyFile(string file, string fileSource, string fileDestination)
        {
            _totalAttemptedInstalled++;
            try
            {
                // Try to copy the same file again, which should succeed.
                string temp = System.IO.Path.Combine(_dest, fileDestination);
                File.Copy(System.IO.Path.Combine(fileSource, file), System.IO.Path.Combine(temp, file), true);
                Console.WriteLine(file + " copied successfully.");
                _totalInstalled++;
            }
            catch
            {
                errorMessage(file, fileSource);
            }
        }
        private void errorMessage (string file)
        {
            Console.WriteLine("\n\nError:\n" + file + " failed to copy.");
        }
        private void errorMessage(string file, string tfPath)
        {
            Console.WriteLine("\n\nError:\n" + file + " failed to copy from " + tfPath);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            installDirectory.Text = _dest;
        }

        private void installDirectory_TextChanged(object sender, TextChangedEventArgs e)
        {
            _dest = installDirectory.Text;
        }

        private void revertButton_Click_1(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Reverted to backups.");
        }

        private void editHotkey_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(System.IO.Path.Combine(_srcScript, "hotkeys.txt"));
        }
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
