using extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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

        public static string languageActive = "BR";

        public static string fileLumaEmu = string.Concat(Directory.GetCurrentDirectory(), "\\LumaEmu.ini");

        

        public MainWindow()
        {
            InitializeComponent();
            textbox_nick.Text = getNameThePlayer();
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




        //=======================funcoes==========================
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
            } else
            {
                return "";
            }
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
