using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
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
using System.Windows.Threading;

namespace BMBF_Updater
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        int MajorV = 1;
        int MinorV = 0;
        int PatchV = 1;
        Boolean Preview = false;

        String exe = System.Reflection.Assembly.GetEntryAssembly().Location;
        String User = System.Environment.GetEnvironmentVariable("USERPROFILE");
        String BMBF = "https://bmbf.dev/stable/24064447";
        String IP;
        Boolean draggable = true;
        Boolean running = false;

        public MainWindow()
        {
            InitializeComponent();
            exe = exe.Replace("\\BMBF_Updater.exe", "");
            if (!Directory.Exists(exe + "\\tmp"))
            {
                Directory.CreateDirectory(exe + "\\tmp");
            }
            if (!Directory.Exists(exe + "\\tmp\\Backup"))
            {
                Directory.CreateDirectory(exe + "\\tmp\\Backup");
            }
            if (!Directory.Exists(exe + "\\Backup"))
            {
                Directory.CreateDirectory(exe + "\\Backup");
            }
            if (File.Exists(exe + "\\BMBF_Update.exe"))
            {
                File.Delete(exe + "\\BMBF_Update.exe");
            }
            Update();
            BMBF_Link();

        }

        private void BMBF_Link()
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile("https://bmbf.dev/stable/json", exe + "\\tmp\\BMBF.txt");
            }
            StreamReader VReader = new StreamReader(exe + "\\tmp\\BMBF.txt");

            String line;
            String f = "";
            while ((line = VReader.ReadLine()) != null)
            {
                f = f + line;
            }

            var json = SimpleJSON.JSON.Parse(f);
            String id = "";
            String name;
            for (int i = 0; i < 5; i++)
            {
                name = json[0]["assets"][i]["name"].ToString();
                if (name == "\"com.weloveoculus.BMBF.apk\"")
                {
                    id = json[0]["assets"][i]["id"].ToString();
                    return;
                }
            }
            BMBF = "https://bmbf.dev/stable/" + id;
        }


        private void Drag(object sender, RoutedEventArgs e)
        {
            bool mouseIsDown = System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed;


            if (mouseIsDown)
            {
                if (draggable)
                {
                    this.DragMove();
                }

            }

        }

        public void noDrag(object sender, MouseEventArgs e)
        {
            draggable = false;
        }

        public void doDrag(object sender, MouseEventArgs e)
        {
            draggable = true;
        }

        private void Mini(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(exe + "\\tmp"))
            {
                Directory.Delete(exe + "\\tmp", true);
            }
            this.Close();
        }


        private void Uninstall(object sender, RoutedEventArgs e)
        {
            if(running)
            {
                return;
            }
            running = true;
            Backup();
            
            txtbox.AppendText("\n\nBacking Up Beat Saber Game data");
            txtbox.ScrollToEnd();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

            //Back Up BS game Data
            if (Directory.Exists(User + "\\AppData\\Roaming\\SideQuest\\backups\\com.beatgames.beatsaber\\data\\Backup"))
            {
                Directory.Delete(User + "\\AppData\\Roaming\\SideQuest\\backups\\com.beatgames.beatsaber\\data\\Backup", true);
            }
            if (!Directory.Exists(User + "\\AppData\\Roaming\\SideQuest\\backups\\com.beatgames.beatsaber\\data\\Backup"))
            {
                Directory.CreateDirectory(User + "\\AppData\\Roaming\\SideQuest\\backups\\com.beatgames.beatsaber\\data\\Backup");
            }
            ProcessStartInfo s = new ProcessStartInfo();
            s.CreateNoWindow = false;
            s.UseShellExecute = false;
            s.FileName = "adb.exe";
            s.WindowStyle = ProcessWindowStyle.Minimized;
            s.Arguments = "pull /sdcard/Android/data/com.beatgames.beatsaber/files/ \"" + exe + "\\Backup\"";
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(s))
                {
                    exeProcess.WaitForExit();
                    txtbox.AppendText("\nBacked up Beat Saber Game Data.");
                    Directory.Delete(exe + "\\Backup\\files\\libs", true);
                    Directory.Delete(exe + "\\Backup\\files\\mods", true);
                    txtbox.ScrollToEnd();
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                }
            }
            catch
            {

                
                ProcessStartInfo se = new ProcessStartInfo();
                se.CreateNoWindow = false;
                se.UseShellExecute = false;
                se.FileName = User + "\\AppData\\Roaming\\SideQuest\\platform-tools\\adb.exe";
                se.WindowStyle = ProcessWindowStyle.Minimized;
                se.Arguments = "pull /sdcard/Android/data/com.beatgames.beatsaber/files/ \"" + exe + "\\Backup\"";
                try
                {
                    // Start the process with the info we specified.
                    // Call WaitForExit and then the using statement will close.
                    using (Process exeProcess = Process.Start(se))
                    {
                        exeProcess.WaitForExit();
                        txtbox.AppendText("\nBacked up Beat Saber Game Data.");
                        Directory.Delete(exe + "\\Backup\\files\\libs", true);
                        Directory.Delete(exe + "\\Backup\\files\\mods", true);
                        txtbox.ScrollToEnd();
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

                    }
                }
                catch
                {
                    // Log error.
                    txtbox.AppendText("\n\n\nAn Error Occured (Code: ADB100). Check following");
                    txtbox.AppendText("\n\n- Your Quest is connected and USB Debugging enabled.");
                    txtbox.AppendText("\n\n- You have adb installed.");
                }

            }
            




            //Uninstall BS
            txtbox.AppendText("\n\nUninstalling Beat Saber");
            txtbox.ScrollToEnd();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

            //Back Up BS game Data
            ProcessStartInfo Bs = new ProcessStartInfo();
            s.CreateNoWindow = false;
            s.UseShellExecute = false;
            s.FileName = "adb.exe";
            s.WindowStyle = ProcessWindowStyle.Minimized;
            s.Arguments = "uninstall com.beatgames.beatsaber";
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(s))
                {
                    exeProcess.WaitForExit();
                    txtbox.AppendText("\nUninstalled Beat Saber");
                    txtbox.ScrollToEnd();
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                }
            }
            catch
            {


                ProcessStartInfo Bse = new ProcessStartInfo();
                Bse.CreateNoWindow = false;
                Bse.UseShellExecute = false;
                Bse.FileName = User + "\\AppData\\Roaming\\SideQuest\\platform-tools\\adb.exe";
                Bse.WindowStyle = ProcessWindowStyle.Minimized;
                Bse.Arguments = "uninstall com.beatgames.beatsaber";
                try
                {
                    // Start the process with the info we specified.
                    // Call WaitForExit and then the using statement will close.
                    using (Process exeProcess = Process.Start(Bse))
                    {
                        exeProcess.WaitForExit();
                        txtbox.AppendText("\nUninstalled Beat Saber");
                        txtbox.ScrollToEnd();
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

                    }
                }
                catch
                {
                    // Log error.
                    txtbox.AppendText("\n\n\nAn Error Occured (Code: ADB100). Check following");
                    txtbox.AppendText("\n\n- Your Quest is connected and USB Debugging enabled.");
                    txtbox.AppendText("\n\n- You have adb installed.");
                }

            }


            //Uninstall BMBF
            txtbox.AppendText("\n\nUninstalling old BMBF");
            txtbox.ScrollToEnd();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));


            ProcessStartInfo Ms = new ProcessStartInfo();
            Ms.CreateNoWindow = false;
            Ms.UseShellExecute = false;
            Ms.FileName = "adb.exe";
            Ms.WindowStyle = ProcessWindowStyle.Minimized;
            Ms.Arguments = "uninstall com.weloveoculus.BMBF";
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(Ms))
                {
                    exeProcess.WaitForExit();
                    txtbox.AppendText("\nUninstalled BMBF");
                    txtbox.ScrollToEnd();
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                }
            }
            catch
            {


                ProcessStartInfo Mse = new ProcessStartInfo();
                Mse.CreateNoWindow = false;
                Mse.UseShellExecute = false;
                Mse.FileName = User + "\\AppData\\Roaming\\SideQuest\\platform-tools\\adb.exe";
                Mse.WindowStyle = ProcessWindowStyle.Minimized;
                Mse.Arguments = "uninstall com.weloveoculus.BMBF";
                try
                {
                    // Start the process with the info we specified.
                    // Call WaitForExit and then the using statement will close.
                    using (Process exeProcess = Process.Start(Mse))
                    {
                        exeProcess.WaitForExit();
                        txtbox.AppendText("\nUninstalled BMBF");
                        txtbox.ScrollToEnd();
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

                    }
                }
                catch
                {
                    // Log error.
                    txtbox.AppendText("\n\n\nAn Error Occured (Code: ADB100). Check following");
                    txtbox.AppendText("\n\n- Your Quest is connected and USB Debugging enabled.");
                    txtbox.AppendText("\n\n- You have adb installed.");
                }

            }
            

            txtbox.AppendText("\n\n\nnow download Beat Saber in the library of your Quest AND start it once. Then click \"Install BMBF\"");
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            txtbox.ScrollToEnd();
            running = false;
        }



        private void Install(object sender, RoutedEventArgs e)
        {
            if(running)
            {
                return;
            }
            running = true;
            txtbox.AppendText("\n\nDownloading BMBF");
            txtbox.ScrollToEnd();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(BMBF, exe + "\\tmp\\BMBF.apk");
            }
            txtbox.AppendText("\nDownload Complete");
            txtbox.ScrollToEnd();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));


            //Install BMBF
            txtbox.AppendText("\n\nInstalling new BMBF");
            txtbox.ScrollToEnd();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));


            ProcessStartInfo Ms = new ProcessStartInfo();
            Ms.CreateNoWindow = false;
            Ms.UseShellExecute = false;
            Ms.FileName = "adb.exe";
            Ms.WindowStyle = ProcessWindowStyle.Minimized;
            Ms.Arguments = "install \"" + exe + "\\tmp\\BMBF.apk\"";
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(Ms))
                {
                    exeProcess.WaitForExit();
                    txtbox.AppendText("\nInstalled BMBF");
                    txtbox.ScrollToEnd();
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                }
            }
            catch
            {


                ProcessStartInfo Mse = new ProcessStartInfo();
                Mse.CreateNoWindow = false;
                Mse.UseShellExecute = false;
                Mse.FileName = User + "\\AppData\\Roaming\\SideQuest\\platform-tools\\adb.exe";
                Mse.WindowStyle = ProcessWindowStyle.Minimized;
                Mse.Arguments = "install \"" + exe + "\\tmp\\BMBF.apk\"";
                try
                {
                    // Start the process with the info we specified.
                    // Call WaitForExit and then the using statement will close.
                    using (Process exeProcess = Process.Start(Mse))
                    {
                        exeProcess.WaitForExit();
                        txtbox.AppendText("\nInstalled BMBF");
                        txtbox.ScrollToEnd();
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

                    }
                }
                catch
                {
                    // Log error.
                    txtbox.AppendText("\n\n\nAn Error Occured (Code: ADB100). Check following");
                    txtbox.AppendText("\n\n- Your Quest is connected and USB Debugging enabled.");
                    txtbox.AppendText("\n\n- You have adb installed.");
                }

            }
            txtbox.AppendText("\n\n\nFinished Installing BMBF. Please continue modding in the headset. Start BMBF from unknown sources. After you finished click \"Reload Songs Folder\". After that start Beat Saber, Fail a Song and click \"Restore Playlists\" to restore your playlists and Scores.");
            txtbox.ScrollToEnd();
            running = false;
        }

        public void Backup()
        {
            
            try
            {
                getQuestIP();
                

                txtbox.AppendText("\n\nBacking up Playlist to " + exe + "\\Playlist.json");
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                


                if (!Directory.Exists(exe + "\\tmp"))
                {
                    Directory.CreateDirectory(exe + "\\tmp");
                }
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile("http://" + IP + ":50000/host/beatsaber/config", exe + "\\tmp\\Config.json");
                }

                txtbox.AppendText("\nDownloaded");
                String Config = exe + "\\tmp\\Config.json";
                


                StreamReader reader = new StreamReader(@Config);
                String line;
                while ((line = reader.ReadLine()) != null)
                {
                    int Index = line.IndexOf("\"Mods\":[{", 0, line.Length);
                    String Playlists = line.Substring(0, Index);
                    File.WriteAllText(exe + "\\Backup\\Playlist.json", Playlists);
                }
                txtbox.AppendText("\n\nBacked up Playlists to " + exe + "\\Playlist.json");
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            }
            catch
            {
                txtbox.AppendText("\n\n\nAn error occured (Code: BMBF100). Check following:");
                txtbox.AppendText("\n\n- You put in the Quests IP right.");
                txtbox.AppendText("\n\n- Your Quest is on.");

            }
            txtbox.ScrollToEnd();

        }

        public void Restore(object sender, RoutedEventArgs e)
        {
            if(running)
            {
                return;
            }
            running = true;
            try
            {


                getQuestIP();
                

                String Playlists;
                

                txtbox.AppendText("\n\nRestoring Playlist");
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));


                if (!Directory.Exists(exe + "\\tmp"))
                {
                    Directory.CreateDirectory(exe + "\\tmp");
                }
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile("http://" + IP + ":50000/host/beatsaber/config", exe + "\\tmp\\OConfig.json");
                }

                String Config = exe + "\\tmp\\OConfig.json";

                Playlists = exe + "\\Backup\\Playlist.json";

                txtbox.AppendText("\n\n" + Playlists);

                StreamReader reader = new StreamReader(@Config);
                String line;
                String CContent = "";
                int Index = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    Index = line.IndexOf("\"Mods\":", 0, line.Length);
                    CContent = line.Substring(Index, line.Length - Index);
                }

                StreamReader Preader = new StreamReader(@Playlists);
                String Pline;
                String Content = "";
                while ((Pline = Preader.ReadLine()) != null)
                {
                    Content = Pline;
                }

                String finished = Content + CContent;

                JObject o = JObject.Parse(finished);
                o.Property("SyncConfig").Remove();
                o.Property("IsCommitted").Remove();
                o.Property("BeatSaberVersion").Remove();

                JProperty lrs = o.Property("Config");
                o.Add(lrs.Value.Children<JProperty>());
                lrs.Remove();

                String FConfig = o.ToString();
                File.WriteAllText(exe + "\\tmp\\config.json", FConfig);

                postChanges(exe + "\\tmp\\config.json");
                txtbox.AppendText("\n\nRestored old Playlists.");
                txtbox.ScrollToEnd();
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            }
            catch
            {
                txtbox.AppendText("\n\n\nAn error occured (Code: BMBF100). Check following:");
                txtbox.AppendText("\n\n- Your Quest is on and BMBF opened");
            }


            //Restore Game Data
            txtbox.AppendText("\n\nRestoring Game Data");
            txtbox.ScrollToEnd();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            


            ProcessStartInfo Ms = new ProcessStartInfo();
            Ms.CreateNoWindow = false;
            Ms.UseShellExecute = false;
            Ms.FileName = "adb.exe";
            Ms.WindowStyle = ProcessWindowStyle.Minimized;
            Ms.Arguments = "push \"" + exe + "\\Backup\\files\" /sdcard/Android/data/com.beatgames.beatsaber";
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(Ms))
                {
                    exeProcess.WaitForExit();
                    txtbox.AppendText("\n\n\nRestored Game Data.");
                    txtbox.ScrollToEnd();
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                }
            }
            catch
            {


                ProcessStartInfo Mse = new ProcessStartInfo();
                Mse.CreateNoWindow = false;
                Mse.UseShellExecute = false;
                Mse.FileName = User + "\\AppData\\Roaming\\SideQuest\\platform-tools\\adb.exe";
                Mse.WindowStyle = ProcessWindowStyle.Minimized;
                Mse.Arguments = "push \"" + exe + "\\Backup\\files\" /sdcard/Android/data/com.beatgames.beatsaber";
                try
                {
                    // Start the process with the info we specified.
                    // Call WaitForExit and then the using statement will close.
                    using (Process exeProcess = Process.Start(Mse))
                    {
                        exeProcess.WaitForExit();
                        txtbox.AppendText("\n\n\nMaybe restored Game Data. Please Restore your Game Data with Sidequest. A backup named \"Backup\" has been created.");
                        txtbox.ScrollToEnd();
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

                    }
                }
                catch
                {
                    // Log error.
                    txtbox.AppendText("\n\n\nAn Error Occured (Code: ADB100). Check following");
                    txtbox.AppendText("\n\n- Your Quest is connected and USB Debugging enabled.");
                    txtbox.AppendText("\n\n- You have adb installed.");
                }

            }
            running = false;
        }


        public void postChanges(String Config)
        {
            using (WebClient client = new WebClient())
            {
                client.QueryString.Add("foo", "foo");
                client.UploadFile("http://" + IP + ":50000/host/beatsaber/config", "PUT", Config);
                client.UploadValues("http://" + IP + ":50000/host/beatsaber/commitconfig", "POST", client.QueryString);
            }
        }

        private void QuestIPCheck(object sender, RoutedEventArgs e)
        {
            if (Quest.Text == "")
            {
                Quest.Text = "Quest IP";
            }
        }

        private void ClearText(object sender, RoutedEventArgs e)
        {
            Quest.Text = "";
        }

        public void getQuestIP()
        {
            IP = Quest.Text;
            return;
        }


        public void Update()
        {
            try
            {
                //Download Update.txt
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        client.DownloadFile("https://raw.githubusercontent.com/ComputerElite/BMBF-Updater/master/Update.txt", exe + "\\tmp\\Update.txt");

                    }
                    catch
                    {
                        txtbox.AppendText("\n\n\nAn error Occured (Code: UD100).");
                    }
                }
                StreamReader VReader = new StreamReader(exe + "\\tmp\\Update.txt");

                String line;
                int l = 0;

                int MajorU = 0;
                int MinorU = 0;
                int PatchU = 0;
                while ((line = VReader.ReadLine()) != null)
                {
                    if (l == 0)
                    {
                        String URL = line;
                    }
                    if (l == 1)
                    {
                        MajorU = Convert.ToInt32(line);
                    }
                    if (l == 2)
                    {
                        MinorU = Convert.ToInt32(line);
                    }
                    if (l == 3)
                    {
                        PatchU = Convert.ToInt32(line);
                    }
                    l++;
                }

                if (MajorU > MajorV || MinorU > MinorV || PatchU > PatchV)
                {
                    //Newer Version available
                    txtbox.AppendText("\n\nAaa");
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

                    Start_Update();
                }

                String MajorVS = Convert.ToString(MajorV);
                String MinorVS = Convert.ToString(MinorV);
                String PatchVS = Convert.ToString(PatchV);
                String MajorUS = Convert.ToString(MajorU);
                String MinorUS = Convert.ToString(MinorU);
                String PatchUS = Convert.ToString(PatchU);

                String VersionVS = MajorVS + MinorVS + PatchVS;
                int VersionV = Convert.ToInt32(VersionVS);
                String VersionUS = MajorUS + MinorUS + PatchUS + " ";
                int VersionU = Convert.ToInt32(VersionUS);
                if (VersionV > VersionU)
                {
                    //Newer Version that hasn't been released yet
                    txtbox.AppendText("\n\nLooks like you have a preview version.");
                    
                }
                if (VersionV == VersionU && Preview)
                {
                    txtbox.AppendText("\n\nLooks like you have a preview version. The release version has been released. You are beeing updated. ");
                    Start_Update();
                    
                }

            }
            catch
            {

            }
        }

        private void Start_Update()
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile("https://raw.githubusercontent.com/ComputerElite/BMBF-Updater/master/BMBF_Update.exe", exe + "\\BMBF_Update.exe");
            }
            //Process.Start(exe + "\\QSE_Update.exe");
            ProcessStartInfo s = new ProcessStartInfo();
            s.CreateNoWindow = false;
            s.UseShellExecute = false;
            s.FileName = exe + "\\BMBF_Update.exe";
            try
            {
                using (Process exeProcess = Process.Start(s))
                {
                }
                this.Close();
            }
            catch
            {
                // Log error.
                txtbox.AppendText("\nAn Error Occured (Code: UD200)");
            }
        }

    }
}
