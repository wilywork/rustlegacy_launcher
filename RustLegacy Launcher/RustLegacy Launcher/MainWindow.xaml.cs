using extensions;
using RustLegacy_Launcher.extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.Security.Principal;


namespace RustLegacy_Launcher
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {

        public static string version = "1.0.0";

        public static string newVersion = "";

        public static string languageActive = "BR";

        public static string fileLumaEmu = string.Concat(Directory.GetCurrentDirectory(), "\\LumaEmu.ini");

        public static string urlLauncherCheckUpdate = "https://rustlegacy.github.io/";

        public static string pathLauncherCheckUpdate = "launcher/";
        public static string pathClientCheckUpdate = "client/";

        public static string pathClientFiles = "\\rust_Data\\Managed\\";


        public MainWindow()
        {
            InitializeComponent();
            AdminRelauncher();
            textbox_nick.Text = getNameThePlayer();
            label_version.Content = "v" + version;

            infoProgress.Content = "Verificando versão...";
            btn_playGame.IsEnabled = false;

            Thread thread = new Thread(() =>
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    if (verificarSeTemAttDoLauncher())
                    {
                        infoProgress.Content = "Baixando novo launcher...";
                        requestServerDownload(String.Concat(urlLauncherCheckUpdate, pathLauncherCheckUpdate, "files/"), String.Concat("rust-launcher_v", newVersion), "exe", Directory.GetCurrentDirectory());
                        Process.Start(String.Concat(Directory.GetCurrentDirectory(), "\\", String.Concat("rust-launcher_v", newVersion, ".exe")));
                        uninstall();
                    }
                    else
                    {
                        infoProgress.Content = "Verificando arquivos...";
                        verificarArquivosDoJogo();
                        infoProgress.Content = "";
                        //verificar arquivos do jogo
                        btn_playGame.IsEnabled = true;
                    }
                }));
            });

            thread.Start();


        }

        //======================components========================
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem curItem = ((ComboBoxItem)(sender as ComboBox).SelectedValue);
            languageActive = curItem.Name;
            //    UpdateNamesComponents();
        }

        private void btn_playGame_Click(object sender, RoutedEventArgs e)
        {
            ExecuteAsAdmin("rust.exe");
            if (checkRustGameOpen(false) != null)
            {
                Process.GetCurrentProcess().Kill();
            }
        }

        // change nick
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (File.Exists(fileLumaEmu))
            {
                IniParser lumaEmuReady = new IniParser(fileLumaEmu);
                lumaEmuReady.SetSetting("Player", "PlayerName ", textbox_nick.Text);
                lumaEmuReady.SaveSettings(fileLumaEmu);
            }
        }


        //=======================classes==========================
        public class InfoLauncher
        {
            public string version;
        }
        public class InfoFilesClient
        {
            public string version;

            public Dictionary<string, string> files;

            public InfoFilesClient(string _version, Dictionary<string, string> _files)
            {
                this.version = _version;
                this.files = _files;
            }

        }

        //=======================funcoes==========================
        public string requestServer(string url)
        {
            using (WebClient client = new WebClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                return client.DownloadString(url);
            }
        }
        public static void requestServerDownload(string url, string fileName, string extension, string folder)
        {
            string filePath;

            if (extension != null)
            {
                filePath = String.Concat(folder, "\\", fileName, ".", extension);
                fileName = String.Format(fileName, ".", extension);
            }
            else
            {
                filePath = String.Concat(folder, fileName);
            }

            if (File.Exists(filePath))
            {
                closeProcessOpen(fileName);
                Thread.Sleep(400);
                File.Delete(filePath);
            }

            using (WebClient client = new WebClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                client.DownloadFile(String.Concat(url, fileName), filePath);
            }
        }
        public bool verificarSeTemAttDoLauncher()
        {
            InfoLauncher info = JsonHelper.Deserialize<InfoLauncher>(requestServer(String.Concat(urlLauncherCheckUpdate, pathLauncherCheckUpdate, "version.json")));
            if (info != null)
            {
                newVersion = info.version;

                if (newVersion != version)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }
        public void verificarArquivosDoJogo()
        {
            //string request = requestServer(String.Concat(urlLauncherCheckUpdate, pathClientCheckUpdate, "manifest.json"));
            InfoFilesClient info = JsonHelper.Deserialize<InfoFilesClient>(requestServer(String.Concat(urlLauncherCheckUpdate, pathClientCheckUpdate, "manifest.json")));
            if (info != null)
            {
                if (info.files != null && info.files.Count > 0)
                {
                    try
                    {
                        foreach (KeyValuePair<string, string> file in info.files)
                        {
                            string filePath = String.Concat(Directory.GetCurrentDirectory(), pathClientFiles, file.Key);
                            if (File.Exists(filePath))
                            {
                                string hashCheck = Hash(filePath);
                                if (hashCheck != file.Value)
                                {
                                    infoProgress.Content = "Baixando " + file.Key + "...";
                                    requestServerDownload(String.Concat(urlLauncherCheckUpdate, pathClientCheckUpdate, "files/"), file.Key, null, String.Concat(Directory.GetCurrentDirectory(), pathClientFiles));
                                }
                            } else
                            {
                                infoProgress.Content = "Baixando " + file.Key + "...";
                                requestServerDownload(String.Concat(urlLauncherCheckUpdate, pathClientCheckUpdate, "files/"), file.Key, null, String.Concat(Directory.GetCurrentDirectory(), pathClientFiles));
                            }
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Erro ao Verificar arquivos do jogo.", "Error");
                    }
                }
            }
        }
        public Process[] checkRustGameOpen(bool closeGame)
        {
            Process[] processesByName = Process.GetProcessesByName("rust");
            if (processesByName.Length != 0)
            {
                if (closeGame)
                {
                    for (int i = 0; i < processesByName.Length; i++)
                    {
                        processesByName[i].Kill();
                    }
                    return null;
                }
                else
                {
                    return processesByName;
                }
            }
            else
            {
                return null;
            }
        }
        public void ExecuteAsAdmin(string fileName)
        {
            try
            {
                Process proc = new Process();
                proc.StartInfo.FileName = fileName;
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.Verb = "runas";
                proc.Start();
            }
            catch (Exception)
            {
                // MessageBox.Show(string.Format(Translator.UpdateLanguage(languageActive, "error_notFileStart"), fileName), "Error");
            }

        }
        public string getNameThePlayer()
        {
            if (File.Exists(fileLumaEmu))
            {

                IniParser lumaEmuReady = new IniParser(fileLumaEmu);
                return lumaEmuReady.GetSetting("Player", "PlayerName ");
            }
            else
            {
                return "";
            }
        }
        public void uninstall()
        {
            string app_name = Process.GetCurrentProcess().MainModule.FileName;
            string bat_name = app_name + ".bat";

            string bat = "@echo off\n"
                + ":loop\n"
                + "del \"" + app_name + "\"\n"
                + "if Exist \"" + app_name + "\" GOTO loop\n"
                + "del %0";

            StreamWriter file = new StreamWriter(bat_name);
            file.Write(bat);
            file.Close();

            Process bat_call = new Process();
            bat_call.StartInfo.FileName = bat_name;
            bat_call.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            bat_call.StartInfo.UseShellExecute = true;
            bat_call.Start();

            Process.GetCurrentProcess().Kill();
        }
        public static void closeProcessOpen(string process)
        {
            Process[] processesByName = Process.GetProcessesByName(process);
            if (processesByName.Length != 0)
            {
                for (int i = 0; i < processesByName.Length; i++)
                {
                    processesByName[i].Kill();
                }
            }
        }

        public static string Hash(string stringToHash)
        {
            using (var stream = new BufferedStream(File.OpenRead(stringToHash), 100000))
            {
                using (var sha1 = new SHA1Managed())
                {
                    return BitConverter.ToString(sha1.ComputeHash(stream)).Replace("-", string.Empty);
                }
            }

        }

        private void AdminRelauncher()
        {
            if (!IsRunAsAdmin())
            {
                ProcessStartInfo proc = new ProcessStartInfo();
                proc.UseShellExecute = true;
                proc.WorkingDirectory = Environment.CurrentDirectory;
                proc.FileName = Assembly.GetEntryAssembly().CodeBase;

                proc.Verb = "runas";

                try
                {
                    Process.Start(proc);
                    Application.Current.Shutdown();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("This program must be run as an administrator! \n\n" + ex.ToString());
                }
            }
        }

        private bool IsRunAsAdmin()
        {
            try
            {
                WindowsIdentity id = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(id);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (Exception)
            {
                return false;
            }
        }
        ////bypass
        //[DllImport("kernel32.dll")]
        //public static extern IntPtr GetModuleHandle(string running);
        //public static void Running()
        //{
        //    while (true)
        //    {
        //        if (GetModuleHandle("SbieDll.dll").ToInt32() != 0 || GetModuleHandle("Snxhk.dll").ToInt32() != 0)
        //        {
        //            System.Threading.Thread.Sleep(17000);
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }
        //}

    }
}
