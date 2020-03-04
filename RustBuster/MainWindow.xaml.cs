using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RustBuster.resources;
using System.Threading;
using System.Net;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Input;
using System.Reflection;
using System.Windows.Threading;

namespace RustBuster
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool loadComponents = false;

        public const string urlServer = "http://fougerite.com/RustBuster/";

        public double numberOfFiles = 0;

        public int numberOfFilesProgress = 0;

        public int percentageProgressBar = 0;

        public string progressLabelTranslator = "";
        public string progressLabelTranslatorContent = "";

        public List<string> listFilesToDownload;

        IniParser filesDownload;

        public static string fileVersion = string.Concat(Directory.GetCurrentDirectory(), "\\version.txt");

        public static string fileLumaEmu = string.Concat(Directory.GetCurrentDirectory(), "\\LumaEmu.ini");

        public static string versionRB = "0.0.0";

        public static string serverVersion;

        public static string languageActive = "US";

        public static BrushConverter BrushConvert = new BrushConverter();
        
        [Flags]
        private enum MySecurityProtocolType
        {
            //
            // Summary:
            //     Specifies the Secure Socket Layer (SSL) 3.0 security protocol.
            Ssl3 = 48,
            //
            // Summary:
            //     Specifies the Transport Layer Security (TLS) 1.0 security protocol.
            Tls = 192,
            //
            // Summary:
            //     Specifies the Transport Layer Security (TLS) 1.1 security protocol.
            Tls11 = 768,
            //
            // Summary:
            //     Specifies the Transport Layer Security (TLS) 1.2 security protocol.
            Tls12 = 3072
        }

        public MainWindow()
        {
            System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)(MySecurityProtocolType.Tls12 | MySecurityProtocolType.Tls11 | MySecurityProtocolType.Tls);
            var dllDirectory = Directory.GetCurrentDirectory() + "\\rust_Data\\Managed";
            if (!Directory.Exists(dllDirectory + "\\RefDLL"))
            {
                Directory.CreateDirectory(dllDirectory + "\\RefDLL");
            }
            if (File.Exists(dllDirectory + "\\RefDLL\\RustBuster2016.dll"))
            {
                File.Delete(dllDirectory + "\\RefDLL\\RustBuster2016.dll");
            }
            try
            {
                File.Copy(dllDirectory + "\\RustBuster2016.dll", dllDirectory + "\\RefDLL\\RustBuster2016.dll");
            }
            catch
            {
                
            }
            Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + dllDirectory + "\\RefDLL");
            Translator.Init();
            InitializeComponent();
            loadComponents = true;
            btn_playGame.IsEnabled = false;
            label_version.Content = string.Concat("v", GetVersionRB());

            setDefaultLanguage();
            UpdateNamesComponents();

            //att status = check version
            label_status.Content = translator("label_status");

            label_nickName.Content = translator("label_nickName") + ":";

            textbox_nick.Tag = translator("label_nickName");

            //START CHECK
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead(urlServer + "Verify.ini"))
                    {
                        //yes
                        //create Thread open fast
                        Thread thread = new Thread(() =>
                        {
                            Dispatcher.BeginInvoke((Action)(() =>
                            {
                                VerifyUpdates();
                            }));
                        });
                        thread.Start();
                    }
                }

            }
            catch(Exception ex)
            {
                Debug.Print(ex.ToString());
                //no
                //att status = check version
                label_status.Content = translator("label_status_error");
                progress_status.Value = 100;
                label_speedNet.Content = "100%";
                btn_playGame.IsEnabled = true;
            }
            //END


            // LOAD NICK PLAYER
            if (File.Exists(fileLumaEmu))
            {
                IniParser lumaEmuReady = new IniParser(fileLumaEmu);
                textbox_nick.Text = lumaEmuReady.GetSetting("Player", "PlayerName ");
                //File.Move(Path.Combine(Environment.CurrentDirectory, @"files/LumaEmu.ini"), Directory.GetCurrentDirectory());
            } else
            {
                MessageBox.Show(string.Format("LumaEmu.ini, file not found!", "LumaEmu.ini"), "Error");
            }
                
        


        }
        
        public bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        //functions
        public void setDefaultLanguage() {
            string nameCulture = Thread.CurrentThread.CurrentCulture.Name;
            string[] names = nameCulture.Split(new[] { "-" }, StringSplitOptions.None);
            if (names != null && names.Length > 0 && Translator.CultureList(names[1])) {
                languageActive = names[1];
                foreach (System.Windows.Controls.ComboBoxItem item in comboBox_translator.Items) {
                    if (item.Name == names[1]) {
                        item.IsSelected = true;
                    }
                }
            } else {
                languageActive = "US";
            }
        }

        public static List<string> GetTests(Type testClass)
        {
            List<string> result = new List<string>();
            MethodInfo[] methodInfos = testClass.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
            Array.Sort(methodInfos,
                  delegate (MethodInfo methodInfo1, MethodInfo methodInfo2)
                  { return methodInfo1.Name.CompareTo(methodInfo2.Name); });

            foreach (MethodInfo mi in methodInfos)
            {
                foreach (var item in mi.GetCustomAttributes(false))
                {
                    if
                   (item.ToString().CompareTo("Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute") == 0)
                        result.Add(mi.Name);
                }
            }

            return result;
        }

        public static string GetVersionRB() {

            string dllfiledir = string.Concat(Directory.GetCurrentDirectory(), "\\rust_Data\\Managed\\RustBuster2016.dll");

            if (File.Exists(dllfiledir))
            {
                Assembly assembly = Assembly.Load(File.ReadAllBytes(dllfiledir));

                try
                {
                    if (assembly != null)
                    {
                        Type typeInstance = assembly.GetType("RustBuster2016.API.RustBusterPlugin");
                        if (typeInstance != null)
                        {
                            MethodInfo methodInfo = typeInstance.GetMethod("get_RustBusterVersion");
                            versionRB = methodInfo.Invoke(Activator.CreateInstance(typeInstance), null) as string;
                        }
                    }
                }
                catch (Exception)
                {
                    if (File.Exists(fileVersion))
                    {
                        versionRB = File.ReadAllText(fileVersion);
                    }
                }
            }
            else
            {
                if (File.Exists(fileVersion))
                {
                    versionRB = File.ReadAllText(fileVersion);
                }
            }

            return versionRB;

        }

        public void VerifyFiles()
        {
            bool missingFiles = false;

            string[] strArrays = filesDownload.EnumSection("Verify");
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string str = strArrays[i];

                if (!string.IsNullOrEmpty(filesDownload.GetSetting("Verify", str)))
                {
                    if (!File.Exists(System.IO.Path.Combine(string.Concat(Directory.GetCurrentDirectory(), "\\"), System.IO.Path.Combine(string.Concat(filesDownload.GetSetting("Verify", str), "\\"), str))))
                    {
                        missingFiles = true;
                    }
                }
                else
                {
                    if (!File.Exists(string.Concat(Directory.GetCurrentDirectory(), "\\", str))) {
                        missingFiles = true;
                    }
                }
            }

            // Missing files ? updated
            if (missingFiles)
            {
                DownloadUpdate();
            }
            else {
                finalyDownload();
            }

        }

        public void VerifyUpdates()
        {
            string[] serverVersionVerify;

            //att status = downloading file...
            label_status.Content = string.Format(translator("downloading_file"), "Verify.ini");
            progressLabelTranslator = "downloading_file";
            progressLabelTranslatorContent = "Verify.ini";

            //download new verify
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
            using (System.Net.WebClient webClient = new System.Net.WebClient())
            {
                webClient.Headers.Add("user-agent", string.Concat("RustBuster", (new Random()).Next(0, 999999)));
                webClient.DownloadFile(urlServer + "Verify.ini",
                    string.Concat(Directory.GetCurrentDirectory(), "\\Verify.ini"));
            }

            //ready file verify
            filesDownload = new IniParser(string.Concat(Directory.GetCurrentDirectory(), "\\Verify.ini"));

            //att status = check version
            label_status.Content = translator("label_status");
            progressLabelTranslator = "";
            progressLabelTranslatorContent = "";

            serverVersionVerify = filesDownload.EnumSection("Version");
            if (serverVersionVerify != null && serverVersionVerify.Length > 0)
            {
                serverVersion = filesDownload.GetSetting("Version", serverVersionVerify[0]);

                if (!string.IsNullOrEmpty(serverVersion))
                {
                    if (serverVersion != versionRB)
                    {
                        //if version != ? updated !
                        DownloadUpdate();
                    }
                    else
                    {
                        //if else check if you have all files
                        VerifyFiles();
                    }
                }
                else
                {
                    DownloadUpdate();
                }
            } else
            {
                DownloadUpdate();
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
                MessageBox.Show( string.Format(Translator.UpdateLanguage(languageActive, "error_notFileStart"), fileName), "Error");
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

        public void UpdateNamesComponents() {
            if (loadComponents) {
                if (btn_playGame != null) {
                    btn_playGame.Content = translator("btn_playGame");
                }
                if (label_status != null) {
                    label_status.Content = string.Format(translator(progressLabelTranslator), progressLabelTranslatorContent);
                }
                if (textBlock_terms != null) {
                    textBlock_terms.Text = translator("textBlock_terms");
                }
                if (label_nickName != null) {
                    label_nickName.Content = translator("label_nickName") + ":";
                }
                if (textbox_nick != null) {
                    textbox_nick.Tag = translator("label_nickName");
                }

                
            }
        }

        public string translator(string component) {
            return Translator.UpdateLanguage(languageActive, component);
        }

        //downloads functions
        public void DownloadUpdate()
        {
            //att status = Starting downloads...
            label_status.Content = translator("start_download");
            progressLabelTranslator = "start_download";
            progressLabelTranslatorContent = "";

            try
            {
                //clear old verify
                if (File.Exists(string.Concat(Directory.GetCurrentDirectory(), "\\Verify.ini")))
                {
                    File.Delete(string.Concat(Directory.GetCurrentDirectory(), "\\Verify.ini"));
                }

                //zero progress
                progress_status.Value = 0;

                //create list files downloads
                listFilesToDownload = filesDownload.EnumSection("Verify").ToList();

                //used in progress
                numberOfFiles = 100/(int)listFilesToDownload.Count;

                //active download
                label_speedNet.Content = "0%";
                DownloadProgressFiles();

            }
            catch (Exception exception)
            {
                Console.WriteLine(string.Concat("ERROR: ", exception));
            }
        }

        public void DownloadProgressFiles() {

            if(listFilesToDownload != null && listFilesToDownload.Count > 0)
            {


                string str = listFilesToDownload[0];

                string str1 = "";

                listFilesToDownload.Remove(str);

                percentageProgressBar = int.Parse(progress_status.Value.ToString());

                if (!string.IsNullOrEmpty(filesDownload.GetSetting("Verify", str)))
                {
                    try {
                        str1 = System.IO.Path.Combine(string.Concat(Directory.GetCurrentDirectory(), "\\"), System.IO.Path.Combine(string.Concat(filesDownload.GetSetting("Verify", str), "\\"), str));
                        if (File.Exists(str1))
                        {
                            File.Delete(str1);
                        }
                        //att status = downloading file...
                        label_status.Content = string.Format(translator("downloading_file"), str);
                        progressLabelTranslator = "downloading_file";
                        progressLabelTranslatorContent = str;

                        startDownload(str, str1);
                    } catch (Exception e) {
                        // handle error scenario
                        MessageBox.Show(translator("download_error_access"), "Error");
                        MessageBox.Show(e.ToString(), "Error");
                        throw e;
                    }

                }
                else
                {
                    str1 = string.Concat(Directory.GetCurrentDirectory(), "\\");
                    if (File.Exists(str1))
                    {
                        File.Delete(str1);
                    }
                    //att status = downloading file...
                    label_status.Content = string.Format(translator("downloading_file"), str);
                    progressLabelTranslator = "downloading_file";
                    progressLabelTranslatorContent = str;

                    startDownload(str, string.Concat(str1, str));
                }
            } else
            {
                finalyDownload();
            }

        }

        private void startDownload(string fileDownload, string pathSave)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                client.Headers.Add("user-agent", string.Concat("RustBuster", (new Random()).Next(0, 999999)));
                client.DownloadProgressChanged +=
                    new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri(urlServer + fileDownload), pathSave);
            }
        }

        public void finalyDownload() {

            string[] strArrays = filesDownload.EnumSection("Delete");
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string str2 = strArrays[i];
                string str3 = System.IO.Path.Combine(string.Concat(Directory.GetCurrentDirectory(), "\\"), System.IO.Path.Combine(string.Concat(filesDownload.GetSetting("Delete", str2), "\\"), str2));
                if (File.Exists(str3))
                {
                    File.Delete(str3);
                }
            }

            //att status = updated!
            progress_status.Value = 100;
            label_speedNet.Content = "100%";
            label_status.Content = translator("finish_update");
            progressLabelTranslator = "finish_update";
            progressLabelTranslatorContent = "";

            btn_playGame.IsEnabled = true;

            label_version.Content = string.Concat("v", serverVersion);

            //save new version in file txt
            if (File.Exists(fileVersion))
            {
                File.WriteAllText(fileVersion, serverVersion);
            }
            else
            {
                File.AppendAllText(fileVersion, serverVersion);
            }
        }



        // components
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.ComboBoxItem curItem = ((System.Windows.Controls.ComboBoxItem)(sender as ComboBox).SelectedValue);
            languageActive = curItem.Name;
            UpdateNamesComponents();
        }

        private void btn_playGame_Click(object sender, RoutedEventArgs e)
        {
            ExecuteAsAdmin("rust.exe");
            if (checkRustGameOpen(false) != null) {
                Process.GetCurrentProcess().Kill();
            }
        }

        private void btn_webSite_Click(object sender, RoutedEventArgs e)
        {
        
            Process.Start("https://fougerite.com/ewr-porta/");
        }

        private void btn_forum_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://fougerite.com/");
        }

        private void btn_servers_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://fougerite.com/RustBuster/ServerList.ini");
        }

        private void btn_checkFiles_Click(object sender, RoutedEventArgs e)
        {
            //att status = downloading file...
            label_status.Content = string.Format(translator("label_status"), "Verify.ini");
            progressLabelTranslator = "label_status";
            progressLabelTranslatorContent = "Verify.ini";
            progress_status.Value = 0;
            label_speedNet.Content = "0%";
            versionRB = "0.0.0";

            if (File.Exists(fileVersion))
            {
                File.WriteAllText(fileVersion, versionRB);
            }

            label_version.Content = string.Concat("v", versionRB);

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += new EventHandler((sendere, args) =>
            {
                timer.Stop();
                VerifyUpdates();
            });
            timer.Start();
          //  timer.Interval = new TimeSpan(0, 0, 1);

            
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {

            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * numberOfFiles;

            progress_status.Value = int.Parse(Math.Truncate( percentage + percentageProgressBar ).ToString());

            label_speedNet.Content = string.Concat(progress_status.Value.ToString(), "%");
        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // handle error scenario
                MessageBox.Show(translator("download_error_antivirus"), "Error");
                MessageBox.Show(e.Error.ToString(),"Error");
                throw e.Error;
            }
            if (e.Cancelled)
            {
                // handle cancelled scenario
            }

            DownloadProgressFiles();
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

        //bypass
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string running);

        public static void Running()
        {
            while (true)
            {
                if (GetModuleHandle("SbieDll.dll").ToInt32() != 0 || GetModuleHandle("Snxhk.dll").ToInt32() != 0){
                    System.Threading.Thread.Sleep(17000);
                } else {
                    break;
                }
            }
        }

        private void mouseEnter_terms(object sender, System.Windows.Input.MouseEventArgs e)
        {
            textBlock_terms.Foreground = (Brush)BrushConvert.ConvertFrom("#FF3630CB");
        }

        private void mouseLeave_terms(object sender, System.Windows.Input.MouseEventArgs e)
        {
            textBlock_terms.Foreground = (Brush)BrushConvert.ConvertFrom("#FF3666CB");
        }

        private void mouseDown_terms(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(translator("text_terms"), translator("textBlock_terms"));
        }
    }

}
