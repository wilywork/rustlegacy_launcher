using extensions;
using RustLegacy_Launcher.extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
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

namespace RustLegacy_Launcher
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {

        public static string version = "0.0.0";

        public static string newVersion = "";

        public static string languageActive = "BR";

        public static string fileLumaEmu = string.Concat(Directory.GetCurrentDirectory(), "\\LumaEmu.ini");

        public string urlLauncherCheckUpdate = "https://rustlegacy.github.io/launcher/";



        public MainWindow()
        {
            InitializeComponent();
            textbox_nick.Text = getNameThePlayer();
            label_version.Content = "v" + version;

            infoProgress.Content = "Verificando versão...";
            btn_playGame.IsEnabled = false;

            if (verificarSeTemAttDoLauncher())
            {
                infoProgress.Content = "Baixando novo launcher...";
                baixarNovoLauncher();
            }
            else
            {
                infoProgress.Content = "";
                //verificar arquivos do jogo
                btn_playGame.IsEnabled = true;
            }

        }

        //======================components========================
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.ComboBoxItem curItem = ((System.Windows.Controls.ComboBoxItem)(sender as ComboBox).SelectedValue);
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
        public class infoLauncher
        {
            public string version;
        }

        //=======================funcoes==========================
        public string requestServer(string url)
        {
            using (WebClient client = new WebClient())
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                return client.DownloadString(url);
            }
        }
        public void requestServerDownload(string url, string fileName, string extension, string folder)
        {
            string filePath = String.Concat(folder, "\\", fileName, ".", extension);

            if (File.Exists(filePath))
            {
                closeProcessOpen(fileName);
                System.Threading.Thread.Sleep(400);
                File.Delete(filePath);
            }

            using (WebClient client = new WebClient())
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                client.DownloadFile(String.Concat(url, fileName, ".", extension), String.Concat(folder, "\\", fileName, ".", extension));
            }
        }
        public bool verificarSeTemAttDoLauncher()
        {
            infoLauncher info = JsonHelper.Deserialize<infoLauncher>(requestServer(String.Concat(urlLauncherCheckUpdate, "version.json")));
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

            // GitClone(ICakeContext, string, DirectoryPath)
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
        public void closeProcessOpen(string process)
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
        public void baixarNovoLauncher()
        {
            //Thread threadCheckUpdate = new Thread(() =>
            //{
                string newLauncher = String.Concat(Directory.GetCurrentDirectory(), "\\", String.Concat("rust-launcher_v", newVersion, ".exe"));
                requestServerDownload(String.Concat(urlLauncherCheckUpdate, "files/"), String.Concat("rust-launcher_v", newVersion), "exe", Directory.GetCurrentDirectory());
                Process.Start(newLauncher);
                uninstall();
            //});
            //threadCheckUpdate.Start();
        }
        //bypass
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
