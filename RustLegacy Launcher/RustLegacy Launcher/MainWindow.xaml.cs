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
using System.Runtime.InteropServices;
using System.Linq;
using System.ComponentModel;

namespace RustLegacy_Launcher
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {

        public static string version = "1.0.1";

        public static string newVersion = "";

        public static string languageActive = "BR";

        public static string fileLumaEmu = string.Concat(Directory.GetCurrentDirectory(), "\\LumaEmu.ini");

        public static string urlLauncherCheckUpdate = "https://rustlegacy.github.io/";

        public static string urlLauncherCheckUpdateBigFiles = "https://bitbucket.org/RustLegacy/rust-legacy-game/raw/e3cc06f4c43b885c251fb341da954aca66cf2321/";

        public static string pathLauncherCheckUpdate = "launcher/";
        public static string pathClientCheckUpdate = "client/";

        public static string pathClientFiles = "\\rust_Data\\Managed\\";

        public static bool falhaAoVerificarArquivos = false;

        public Dictionary<string, string> listFilesToDownload = new Dictionary<string, string>();

        public static List<string> ignoreFilesCheck = new List<string>() { "LumaEmu.ini", "cfg/client.cfg" };
        public MainWindow()
        {
            InitializeComponent();
            AdminRelauncher();
            
            label_version.Content = "v" + version;

            infoProgress.Content = "Verificando versão...";
            btn_playGame.IsEnabled = false;

            Thread thread = new Thread(() =>
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    InitStartCheck();
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
        public void InitStartCheck()
        {
            if (verificarSeTemAttDoLauncher())
            {
                infoProgress.Content = "Baixando novo launcher...";
                requestServerDownload(String.Concat(urlLauncherCheckUpdate, pathLauncherCheckUpdate, "files/"), String.Concat("rust-launcher_v", newVersion, ".exe"), Directory.GetCurrentDirectory());
                Process.Start(String.Concat(Directory.GetCurrentDirectory(), "\\", String.Concat("rust-launcher_v", newVersion, ".exe")));
                uninstall();
            }
            else
            {
                infoProgress.Content = "Verificando arquivos...";

                verificarArquivosDoJogo();

            }
        }
        public void habilitarGame()
        {
            infoProgress.Content = "";
            infoProgressFiles.Content = "";
            textbox_nick.Text = getNameThePlayer();
            //verificar arquivos do jogo
            btn_playGame.IsEnabled = true;
        }
        public string requestServer(string url)
        {
            using (WebClient client = new WebClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                return client.DownloadString(url);
            }
        }
        public void requestServerDownload(string url, string fileName, string folder)
        {
            infoProgressFiles.Content = "Baixando " + fileName + "...";

            string fileNameLocal = fileName.Replace("/", "\\");

            if (File.Exists(fileNameLocal))
            {
                try
                {
                    closeProcessOpen(fileNameLocal);
                    Thread.Sleep(400);
                }
                catch (Exception)
                {
                    throw;
                }
                File.Delete(fileNameLocal);
            }

            try
            {
                createFolder(fileNameLocal);
            }
            catch (Exception)
            {
            }

            Uri urlFileDownload = new Uri(String.Concat(url, fileName));

            try
            {
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    client.Headers.Add("user-agent", string.Concat("rand", (new Random()).Next(0, 999999)));
                    //client.DownloadFile(String.Concat(url, fileName), fileNameLocal);
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                    client.DownloadFileAsync(urlFileDownload, fileNameLocal);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao baixar arquivo " + fileName + " \n" + urlFileDownload + "\n" + ex, "Error");
                falhaAoVerificarArquivos = true;
            }

        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // handle error scenario
              //  MessageBox.Show(translator("download_error_antivirus"), "Error");
                MessageBox.Show(e.Error.ToString(), "Error");
                throw e.Error;
            }
            if (e.Cancelled)
            {
                // handle cancelled scenario
            }

            downloadInProgress();
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
            InfoFilesClient info = JsonHelper.Deserialize<InfoFilesClient>(requestServer(String.Concat(urlLauncherCheckUpdate, pathClientCheckUpdate, "manifestFiles.json")));
            if (info != null)
            {
                if (info.files != null && info.files.Count > 0)
                {
                    try
                    {
                        listFilesToDownload = info.files;
                        downloadInProgress();
                        //foreach (KeyValuePair<string, string> file in info.files)
                        //{
                        //    if (falhaAoVerificarArquivos) break;
                        //    string filePath = String.Concat(Directory.GetCurrentDirectory(), file.Key.Replace("/", "\\"));
                        //    if (File.Exists(filePath))
                        //    {
                        //        string hashCheck = Hash(filePath);
                        //        if (hashCheck != file.Value)
                        //        {

                        //            requestServerDownload(String.Concat(urlLauncherCheckUpdate, pathClientCheckUpdate, "files/"), file.Key, Directory.GetCurrentDirectory());
                        //        }
                        //    } else
                        //    {
                        //        requestServerDownload(String.Concat(urlLauncherCheckUpdate, pathClientCheckUpdate, "files/"), file.Key, Directory.GetCurrentDirectory());
                        //    }
                        //}
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao Verificar arquivos do jogo. \n" + ex, "Error");
                    }
                }
            }
        }

        public void downloadInProgress()
        {
            if (listFilesToDownload != null && listFilesToDownload.Count > 0)
            {
                var file = listFilesToDownload.First();

                listFilesToDownload.Remove(file.Key);

                string fileName = file.Key.Replace("BIG/", "");
                string filePath = String.Concat(Directory.GetCurrentDirectory(), "\\", fileName.Replace("/", "\\"));
                string urlFileForDownload = urlLauncherCheckUpdate;

                infoProgressFiles.Content = "Verificando " + fileName + "...";

                if (file.Key.StartsWith("BIG"))
                {
                    urlFileForDownload = urlLauncherCheckUpdateBigFiles;
                }

                if (File.Exists(filePath))
                {
                    if (!ignoreFilesCheck.Contains(fileName))
                    {
                        string hashFile = Hash(filePath);
                        if (hashFile != file.Value)
                        {
                            requestServerDownload(String.Concat(urlFileForDownload, pathClientCheckUpdate, "files/"), fileName, Directory.GetCurrentDirectory());
                        }
                        else
                        {
                            downloadInProgress();
                        }
                    } else
                    {
                        downloadInProgress();
                    }
                }
                else
                {
                    requestServerDownload(String.Concat(urlFileForDownload, pathClientCheckUpdate, "files/"), fileName, Directory.GetCurrentDirectory());
                }

            } else
            {
                habilitarGame();
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

        private  static void createFolder(string path)
        {
            //fix path
            path = path.Replace("/", "\\");

            path = Path.GetDirectoryName(path);

            if (path != "")
            {
                try
                {
                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao criar Pasta. \n" + ex, "Error");
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
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string running);
        public static void Running()
        {
            while (true)
            {
                if (GetModuleHandle("SbieDll.dll").ToInt32() != 0 || GetModuleHandle("Snxhk.dll").ToInt32() != 0)
                {
                    System.Threading.Thread.Sleep(17000);
                }
                else
                {
                    break;
                }
            }
        }

    }
}
