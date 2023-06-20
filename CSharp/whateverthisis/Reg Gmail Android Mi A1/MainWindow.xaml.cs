using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;
using RingoLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using xNet;
using Path = System.IO.Path;

namespace Reg_Gmail_Android_Mi_A1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            lstView.ItemsSource = profileList;
        }

        ObservableCollection<Profile> profileList = new ObservableCollection<Profile>();
        private string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        class Profile : INotifyPropertyChanged
        {
            private bool _checkStatus;
            public bool CheckStatus
            {
                get
                {
                    return _checkStatus;
                }
                set
                {
                    _checkStatus = value;
                    OnPropertyChanged();

                }
            }

            private string _STT;
            public string STT
            {
                get
                {
                    return _STT;
                }
                set
                {
                    _STT = value;
                    OnPropertyChanged();

                }
            }

            private string _ID;
            public string ID
            {
                get
                {
                    return _ID;
                }
                set
                {
                    _ID = value;
                    OnPropertyChanged();

                }
            }

            private string _pass;
            public string Pass
            {
                get
                {
                    return _pass;
                }
                set
                {
                    _pass = value;
                    OnPropertyChanged();

                }
            }

            private string _recovery;
            public string Recovery
            {
                get
                {
                    return _recovery;
                }
                set
                {
                    _recovery = value;
                    OnPropertyChanged();

                }
            }

            private string _log;
            public string Log
            {
                get
                {
                    return _log;
                }
                set
                {
                    _log = value;
                    OnPropertyChanged();

                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged(string newName = null)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(newName));
                }
            }

        }

        private void NumberPhone_Change(object sender, TextChangedEventArgs e)
        {
            if (!Regex.IsMatch(NumberPhone_TextBox.Text, "^[0-9]*$"))
            {
                NumberPhone_TextBox.Text = "";
            }
        }

        private void Loop_Change(object sender, TextChangedEventArgs e)
        {
            if (!Regex.IsMatch(Loop_TextBox.Text, "^[0-9]*$"))
            {
                Loop_TextBox.Text = "";
            }
        }

        private void HeaderCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (Profile item in profileList)
            {
                item.CheckStatus = true;
            }
            lstView.UnselectAll();
        }

        private void HeaderCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (Profile item in profileList)
            {
                item.CheckStatus = false;
            }
            lstView.UnselectAll();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = lstView.SelectedItems;
            if (selectedItem.Count > 1)
            {
                foreach (Profile item in selectedItem)
                {
                    item.CheckStatus = true;
                }
            }
        }
        private void TextBlockTemplate_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var tb = sender as TextBlock;
            Clipboard.SetText(tb.Text);
        }


        private Semaphore semaphore = new Semaphore(1, 1);
        private void Run_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread MainThread = new Thread(() =>
                {
                    try
                    {
                        Dispatcher.Invoke(() =>
                        {
                            RunButton.IsEnabled = false;
                        });
                        int loop = Dispatcher.Invoke(() => { return int.Parse(Loop_TextBox.Text); });
                        int phone = Dispatcher.Invoke(() => { return int.Parse(NumberPhone_TextBox.Text); });
                        for (var forLoop = 0; forLoop < loop; forLoop++)
                        {
                            
                            int numFalse = 0;
                            int startIndex = 0;
                            string[] deviceList = null;
                            for (var getDevice = 0; getDevice < 10; getDevice++)
                            {
                                deviceList = ADBHi.ADBGetConnectDevices();
                                if (deviceList.Length != 0)
                                {
                                    break;
                                }
                            }
                            if (deviceList.Length == 0)
                            {
                                MessageBox.Show("Lỗi ADB");
                                Dispatcher.Invoke(() =>
                                {
                                    RunButton.IsEnabled = true;
                                });
                                return;
                            }
                            int threadsCompleted = 0;
                            for (var forPhone = 0; forPhone < phone; forPhone++)
                            {
                                int port = 4723;
                                port = port + forPhone;
                                string deviceID = deviceList[forPhone];
                                Profile profile = null;
                                for (int p = startIndex; p < profileList.Count; p++)
                                {
                                    if (profileList[p].CheckStatus == true && (string.IsNullOrEmpty(profileList[p].Log)))
                                    {
                                        profile = profileList[p];
                                        startIndex = p + 1;
                                        break;
                                    }
                                }
                                if (profile != null)
                                {
                                    Thread phoneThread = new Thread(() =>
                                    {
                                        try
                                        {
                                            profile.Log = "Running";
                                            ADBHi.ADBTurnOnAirplane(deviceID);
                                            Thread.Sleep(3000);
                                            ADBHi.ADBTurnOffAirplane(deviceID);
                                            Thread.Sleep(3000);
                                            ADBHi.ADBTurnOnMobileData(deviceID);
                                            Thread.Sleep(3000);                                          
                                            AppiumOptions options = new AppiumOptions();
                                            options.AddAdditionalCapability(MobileCapabilityType.PlatformName, "Android");
                                            options.AddAdditionalCapability(MobileCapabilityType.Udid, deviceID);
                                            AndroidDriver<AndroidElement> driver = new AndroidDriver<AndroidElement>(new Uri($"http://127.0.0.1:{port}/wd/hub"), options);
                                            CreateUser(driver, deviceID);
                                            //ClearAccountGoogle(driver);
                                            //ADBHi.ADBClearPackageGoogle(deviceID);
                                            //ADBHi.ADBTurnOnAirplane(deviceID);
                                            //Thread.Sleep(3000);
                                            //ADBHi.ADBTurnOffAirplane(deviceID);
                                            //Thread.Sleep(3000);
                                            //ADBHi.ADBTurnOnMobileData(deviceID);
                                            //Thread.Sleep(3000);

                                            if (CreateAccountGoogleByUser(driver, deviceID, profile))
                                            {
                                                profile.Log = "Done";
                                                UpdateProfileFile();
                                                Interlocked.Increment(ref threadsCompleted);
                                                return;
                                            }
                                            else
                                            {
                                                UpdateProfileFile();
                                                Interlocked.Increment(ref threadsCompleted);
                                                return;
                                            }

                                        }

                                        catch
                                        {
                                            profile.Log = "Error Thread";
                                            UpdateProfileFile();
                                            Interlocked.Increment(ref threadsCompleted);
                                            return;
                                        }
                                    });
                                    phoneThread.IsBackground = true;
                                    phoneThread.Start();

                                }
                            }
                            while (Interlocked.CompareExchange(ref threadsCompleted, 0, 0) != phone)
                            {
                                Thread.Sleep(100);
                            }
                            Dispatcher.Invoke(() =>
                            {
                                RunButton.IsEnabled = true;
                            });
                        }                        
                    }
                    catch 
                    {
                        MessageBox.Show("Lỗi Luồng");
                    }
                    
                });
                MainThread.IsBackground = true;
                MainThread.Start();
            }
            catch 
            {
                MessageBox.Show("Lỗi Luồng");
            }
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV Files | *.csv";
                saveFileDialog.DefaultExt = "csv";
                if (saveFileDialog.ShowDialog() == true)
                {
                    var newRecords = new List<Profile>();

                    foreach (Profile item in profileList)
                    {
                        newRecords.Add(new Profile
                        {
                            CheckStatus = item.CheckStatus,
                            STT = item.STT,
                            ID = item.ID,
                            Pass = item.Pass,
                            Recovery = item.Recovery,
                            Log = item.Log
                        });
                    }

                    using (var writer = new StreamWriter(saveFileDialog.FileName))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.WriteRecords(newRecords);
                    }

                }

            }
            catch (Exception exception)
            {
                CustomHelper.SaveLog("Export", exception);

            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            profileList.Clear();
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "CSV Files | *.csv";
                openFileDialog.DefaultExt = "csv";
                if (openFileDialog.ShowDialog() == true)
                {
                    using (var reader = new StreamReader(openFileDialog.FileName))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        var records = csv.GetRecords<Profile>().ToList();
                        foreach (Profile item in records)
                        {
                            if (string.IsNullOrEmpty(item.STT))
                            {
                                item.STT = null;
                            }
                            if (string.IsNullOrEmpty(item.ID))
                            {
                                item.ID = null;
                            }
                            if (string.IsNullOrEmpty(item.Pass))
                            {
                                item.Pass = null;
                            }
                            if (string.IsNullOrEmpty(item.Recovery))
                            {
                                item.Recovery = null;
                            }
                            if (string.IsNullOrEmpty(item.Log))
                            {
                                item.Log = null;
                            }
                            profileList.Add(new Profile
                            {
                                CheckStatus = item.CheckStatus,
                                STT = item.STT,
                                ID = item.ID,
                                Pass = item.Pass,
                                Recovery = item.Recovery,
                                Log = item.Log
                            });
                        }

                    }
                    UpdateProfileFile();
                }
            }
            catch (Exception exception)
            {
                CustomHelper.SaveLog("Import", exception);
            }
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in profileList)
            {
                item.Log = "";
            }
            UpdateProfileFile();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitProfileCSV();
            LoadProfile();
            LoadPath();
        }

        private void LoadPath()
        {
            if (File.Exists(Path.Combine(localAppDataPath, "Reg Mail", "Api.txt")))
            {
                string text = File.ReadAllText(Path.Combine(localAppDataPath, "Reg Mail", "Api.txt"));
                Api_TextBox.Text = text;
            }
        }

        private void Path_Change(object sender, TextChangedEventArgs e)
        {
            if (!Directory.Exists(Path.Combine(localAppDataPath, "Reg Mail")))
            {
                Directory.CreateDirectory(Path.Combine(localAppDataPath, "Reg Mail"));
            }
            string text = Api_TextBox.Text;
            string path = Path.Combine(localAppDataPath, "Reg Mail", "Api.txt");
            File.WriteAllText(path, text);
        }

        private void UpdateProfileFile()
        {
            var newRecords = new List<Profile>();

            foreach (Profile item in profileList)
            {
                newRecords.Add(new Profile
                {
                    CheckStatus = item.CheckStatus,
                    STT = item.STT,
                    ID = item.ID,
                    Pass = item.Pass,
                    Recovery = item.Recovery,
                    Log = item.Log
                });
            }
            try
            {
                using (var writer = new StreamWriter(localAppDataPath + @"\Reg Mail\profile.csv"))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(newRecords);
                }
                try
                {
                    File.Copy(localAppDataPath + @"\Reg Mail\profile.csv", localAppDataPath + @"\Reg Mail\profile.csv", true);
                }
                catch { }
            }
            catch
            {
                UpdateProfileFile();
            }

        }

        private void CheckUptoDateProfileCSV()
        {

            var newProfileInfo = new List<Profile>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
            };
            using (var reader = new StreamReader(localAppDataPath + @"\Reg Mail\profile.csv"))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<Profile>();
                foreach (var item in records)
                {
                    var newProfile = new Profile();
                    foreach (var oldProfileInfoProperty in item.GetType().GetProperties())
                    {
                        foreach (var newProfileInfoProperty in newProfile.GetType().GetProperties())
                        {
                            if (newProfileInfoProperty.Name == oldProfileInfoProperty.Name)
                            {
                                newProfileInfoProperty.SetValue(newProfile, oldProfileInfoProperty.GetValue(item));
                            }
                        }
                    }
                    newProfileInfo.Add(newProfile);
                }
            }

            using (var writer = new StreamWriter(localAppDataPath + @"\Reg Mail\profile.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(newProfileInfo);
            }
        }

        private void InitProfileCSV()
        {
            if (!Directory.Exists(localAppDataPath + @"\Reg Mail"))
            {
                Directory.CreateDirectory(localAppDataPath + @"\Reg Mail");
            }

            if (File.Exists(localAppDataPath + @"\Reg Mail\profile.csv"))
            {
                CheckUptoDateProfileCSV();
            }
            else
            {
                var records = Enumerable.Empty<Profile>();
                using (var writer = new StreamWriter(localAppDataPath + @"\Reg Mail\profile.csv"))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(records);
                }
            }
        }

        private void LoadProfile()
        {
            string filePath = Path.Combine(localAppDataPath, "Reg Mail", "profile.csv");
            if (File.Exists(filePath))
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<Profile>().ToList();
                    foreach (Profile item in records)
                    {
                        if (string.IsNullOrEmpty(item.STT))
                        {
                            item.STT = null;
                        }
                        if (string.IsNullOrEmpty(item.ID))
                        {
                            item.ID = null;
                        }
                        if (string.IsNullOrEmpty(item.Pass))
                        {
                            item.Pass = null;
                        }
                        if (string.IsNullOrEmpty(item.Recovery))
                        {
                            item.Recovery = null;
                        }
                        if (string.IsNullOrEmpty(item.Log))
                        {
                            item.Log = null;
                        }
                        profileList.Add(new Profile
                        {
                            CheckStatus = item.CheckStatus,
                            STT = item.STT,
                            ID = item.ID,
                            Pass = item.Pass,
                            Recovery = item.Recovery,
                            Log = item.Log
                        });
                    }
                }
            }
        }


        static void ClearAccountGoogle(AndroidDriver<AndroidElement> driver)
        {
            try
            {
                startClear:
                driver.StartActivity("com.android.settings", "com.android.settings.Settings$AccountDashboardActivity");
                Random random = new Random();
                Thread.Sleep(random.Next(5000, 7000));
                var googleAccount = AppiumHi.WaitXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.support.v7.widget.RecyclerView/android.widget.LinearLayout[1]/android.widget.LinearLayout[2]/android.widget.LinearLayout/android.widget.TextView[1]");
                if(googleAccount != null)
                {
                    if (googleAccount.Text == "Google")
                    {
                        googleAccount.Click();
                        Thread.Sleep(random.Next(1000, 2000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.support.v7.widget.RecyclerView/android.widget.FrameLayout[2]/android.widget.LinearLayout/android.widget.Button");
                        Thread.Sleep(random.Next(1000, 2000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.ScrollView/android.widget.LinearLayout/android.widget.Button[2]");
                        Thread.Sleep(random.Next(1000, 2000));
                        goto startClear;
                    }

                }
            }
            catch { }
        }

        static void CreateUser (AndroidDriver<AndroidElement> driver,string deviceID)
        {
            string AlwayScreen = $"-s {deviceID} shell svc power stayon true";
            ADBHi.ADBProcess(AlwayScreen);
            string CheckList = $"-s {deviceID} shell pm list users";
            var ListUser = ADBHi.ADBProcessGetResult(CheckList);
            ListUser = ListUser.Replace("Users:\r\n","");
            ListUser = ListUser.Replace("\t", "");
            string[] User = ListUser.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if(User.Length > 1)
            {
                string switchMainUser = $"-s {deviceID} shell am switch-user 0";
                ADBHi.ADBProcess(switchMainUser);
                Thread.Sleep(3000);
                for (int i = 1; i < User.Length; i++)
                {                  
                    string idRemove = User[i].Split(new char[] { '{', ':' })[1];
                    string removeUser = $"-s {deviceID} shell pm remove-user {idRemove}";
                    ADBHi.ADBProcess(removeUser);
                }
            }
            Thread.Sleep(500);
            string CreateUser = $"-s {deviceID} shell pm create-user Google";
            ADBHi.ADBProcess(CreateUser);
            Thread.Sleep(2000);
            ListUser = null;
            ListUser = ADBHi.ADBProcessGetResult(CheckList);
            ListUser = ListUser.Replace("Users:\r\n", "");
            ListUser = ListUser.Replace("\t", "");
            User = null;
            User = ListUser.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            string id = User[1].Split(new char[] { '{', ':' })[1];
            string switchUser = $"-s {deviceID} shell am switch-user {id}";
            ADBHi.ADBProcess(switchUser);
            Thread.Sleep(15000);
            ADBHi.ADBProcess(AlwayScreen);
            string openLock = $"-s {deviceID} shell input keyevent KEYCODE_MENU";
            ADBHi.ADBProcess(openLock);
            Thread.Sleep(1000);
            AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.Button[2]");
            Thread.Sleep(1000);
            AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.Button");
            Thread.Sleep(1000);
            AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.ScrollView/android.widget.LinearLayout/android.widget.Button[2]");
            Thread.Sleep(1000);
            AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.support.v7.widget.RecyclerView/android.widget.LinearLayout[3]/android.widget.Switch");           
            Thread.Sleep(2000);
            AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.Button");
            Thread.Sleep(1000);
            AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.Button");
            Thread.Sleep(1000);
            AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.Button");
            Thread.Sleep(1000);
            AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.Button[1]");
            Thread.Sleep(1000);
            AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.support.v7.widget.RecyclerView/android.widget.LinearLayout[4]/android.widget.LinearLayout/android.widget.RelativeLayout/android.widget.TextView");
            Thread.Sleep(1000);
            AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.ScrollView/android.widget.LinearLayout/android.widget.Button[2]");
            Thread.Sleep(1000);
            AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.ScrollView/android.widget.LinearLayout/android.support.v7.widget.RecyclerView/android.widget.FrameLayout[3]/android.widget.LinearLayout/android.widget.TextView");
            Thread.Sleep(1000);
            AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.ScrollView/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.LinearLayout[1]/android.widget.LinearLayout/android.widget.TextView");
            Thread.Sleep(1000);
        }

        private bool CreateAccountGoogleByUser(AndroidDriver<AndroidElement> driver, string deviceID, Profile profile)
        {
            try
            {

                //driver.StartActivity("com.android.settings", "com.android.settings.Settings$AccountDashboardActivity");
                Random random = new Random();
                //Thread.Sleep(random.Next(5000, 7000));
                //AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.support.v7.widget.RecyclerView/android.widget.LinearLayout[1]/android.widget.LinearLayout/android.widget.RelativeLayout/android.widget.TextView");
                //Thread.Sleep(random.Next(1000, 2000));
                //AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.support.v7.widget.RecyclerView/android.widget.LinearLayout[5]/android.widget.LinearLayout/android.widget.RelativeLayout/android.widget.TextView");
                Thread.Sleep(random.Next(8000, 10000));
                AppiumHi.WaitXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[5]/android.view.View/android.widget.Spinner");
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[5]/android.view.View/android.widget.Spinner");
                Thread.Sleep(random.Next(1000, 2000));
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View/android.view.View[2]/android.view.View[5]/android.view.View[2]/android.view.View/android.view.View/android.view.MenuItem[1]");
                Thread.Sleep(random.Next(3000, 5000));
                string[] info = GeneratorInfoVietnam();
                string name = info[0];
                string[] nameParts = name.Split(' ');
                string firstName = nameParts[0];
                string lastName = string.Join(" ", nameParts.Skip(1));
                string gender = info[1];              
                    AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[1]/android.view.View/android.view.View[2]/android.view.View");
                    Thread.Sleep(random.Next(1000, 2000));                   
                    ADBHi.ADBSendText(deviceID, firstName);
                    Thread.Sleep(random.Next(1000, 2000));                
                //AppiumHi.SendTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[1]/android.view.View/android.view.View[2]/android.view.View", firstName);               

                    AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[2]/android.view.View/android.view.View[2]/android.view.View");
                    Thread.Sleep(random.Next(1000, 2000));
                    //AppiumHi.SendTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[2]/android.view.View/android.view.View[2]/android.view.View", lastName);
                    ADBHi.ADBSendText(deviceID, lastName);
                    Thread.Sleep(random.Next(1000, 2000));
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[1]");
                Thread.Sleep(random.Next(1000, 2000));
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[4]/android.view.View/android.widget.Button");
                Thread.Sleep(random.Next(3000, 5000));
                string birthdayday = random.Next(1, 28).ToString();
                string birthdaymonth = random.Next(1, 10).ToString();
                string birthdayyear = random.Next(1970, 2000).ToString();
                    AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[1]/android.view.View[2]/android.view.View");
                    Thread.Sleep(random.Next(500, 1000));
                    //AppiumHi.SendTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[1]/android.view.View[2]/android.view.View", birthdayday);
                    ADBHi.ADBSendText(deviceID, birthdayday);
                    Thread.Sleep(random.Next(1000, 2000));
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[2]/android.view.View[2]/android.widget.Spinner");
                Thread.Sleep(random.Next(500, 1000));
                AppiumHi.ClickXpath(driver, $"/hierarchy/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.ListView/android.widget.CheckedTextView[{birthdaymonth}]");
                Thread.Sleep(random.Next(1000, 2000));
                    AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[3]/android.view.View[2]/android.view.View");
                    AppiumHi.ClearText(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[3]/android.view.View[2]/android.view.View");
                    Thread.Sleep(random.Next(500, 1000));
                    //AppiumHi.SendTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[3]/android.view.View[2]/android.view.View", birthdayyear);
                    ADBHi.ADBSendText(deviceID, birthdayyear);
                    Thread.Sleep(random.Next(1000, 2000));
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[4]/android.view.View[2]/android.widget.Spinner");
                Thread.Sleep(random.Next(500, 1000));
                if (gender.Contains("Male"))
                {
                    AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.ListView/android.widget.CheckedTextView[2]");
                }
                else
                {
                    AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.ListView/android.widget.CheckedTextView[1]");
                }
                Thread.Sleep(random.Next(1000, 2000));
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[4]/android.view.View/android.widget.Button");
                Thread.Sleep(random.Next(3000, 5000));
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View/android.view.View[3]/android.view.View");
                Thread.Sleep(random.Next(1000, 2000));
                    AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[2]/android.view.View[3]/android.view.View");
                    Thread.Sleep(random.Next(500, 1000));
                    //AppiumHi.SendTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[2]/android.view.View[3]/android.view.View", profile.ID);
                    ADBHi.ADBSendText(deviceID, profile.ID);
                    Thread.Sleep(random.Next(500, 1000));
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[1]");
                Thread.Sleep(random.Next(500, 1000));
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[4]/android.view.View/android.widget.Button");
                Thread.Sleep(random.Next(3000, 5000));
                    AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[1]/android.view.View/android.view.View/android.view.View[2]/android.view.View");
                    AppiumHi.ClearText(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[1]/android.view.View/android.view.View/android.view.View[2]/android.view.View");
                    Thread.Sleep(random.Next(500, 1000));
                    //AppiumHi.SendTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[1]/android.view.View/android.view.View/android.view.View[2]/android.view.View", profile.Pass);
                    ADBHi.ADBSendText(deviceID, profile.Pass);
                    Thread.Sleep(random.Next(500, 1000));
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[1]");
                Thread.Sleep(random.Next(500, 1000));
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[4]/android.view.View/android.widget.Button");
                Thread.Sleep(random.Next(3000, 5000));
                var Ver = AppiumHi.WaitXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[1]", 15);
                if (Ver != null)
                {
                    if (Ver.Text == "Xác nhận bạn không phải là rô-bốt")
                    {
                    VerPhone:
                        string[] phone = GetPhoneNumber2ndLine();
                        if (phone[0].Contains("Fail"))
                        {
                            profile.Log = phone[0];
                            return false;
                        }
                            AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[2]/android.view.View/android.view.View[3]/android.view.View/android.widget.EditText");
                            Thread.Sleep(random.Next(500, 1000));
                            //AppiumHi.SendTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[2]/android.view.View/android.view.View[3]/android.view.View/android.widget.EditText", phone[0]);
                            ADBHi.ADBSendText(deviceID, phone[0]);
                            Thread.Sleep(random.Next(500, 1000));
                        //AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[1]");
                        driver.PressKeyCode(AndroidKeyCode.Enter);
                        Thread.Sleep(random.Next(500, 1000));
                        var checkErrorPhone = AppiumHi.WaitXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[2]/android.view.View/android.view.View[4]");
                        if (checkErrorPhone != null)
                        {
                            if (checkErrorPhone.Text == "Không thể sử dụng số điện thoại này để xác minh.")
                            {
                                goto VerPhone;
                            }
                        }
                        //AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[4]/android.view.View/android.widget.Button");
                        Thread.Sleep(random.Next(3000, 5000));
                        string code = GetCode2ndline(phone[1]);
                        if (code.Contains("not"))
                        {
                            profile.Log = code;
                            return false;
                        }
                            AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[2]/android.view.View/android.view.View[2]/android.view.View[3]/android.view.View");
                            Thread.Sleep(random.Next(500, 1000));
                            //AppiumHi.SendTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[2]/android.view.View/android.view.View[2]/android.view.View[3]/android.view.View", code);
                            ADBHi.ADBSendText(deviceID, code);
                            Thread.Sleep(random.Next(500, 1000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[1]");
                        Thread.Sleep(random.Next(500, 1000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.widget.Button");
                        Thread.Sleep(random.Next(3000, 5000));
                    }
                }
                var addPhone = AppiumHi.WaitXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[1]");
                if (addPhone.Text == "Thêm số điện thoại?")
                {
                    driver.PressKeyCode(new KeyEvent(AndroidKeyCode.Keycode_MOVE_END));
                    driver.PressKeyCode(new KeyEvent(AndroidKeyCode.Keycode_MOVE_END));
                    Thread.Sleep(random.Next(500, 1000));
                    AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[4]/android.view.View/android.widget.Button");
                    Thread.Sleep(random.Next(3000, 5000));
                }
                var checkInfo = AppiumHi.WaitXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[1]");
                if (checkInfo != null)
                {
                    if (checkInfo.Text == "Xem lại thông tin tài khoản của bạn")
                    {
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[4]/android.view.View/android.widget.Button");
                        Thread.Sleep(random.Next(3000, 5000));
                    }
                }
                var checkRules = AppiumHi.WaitXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[1]");
                if (checkRules != null)
                {
                    if (checkRules.Text == "Quyền riêng tư và Điều khoản")
                    {
                        driver.PressKeyCode(new KeyEvent(AndroidKeyCode.Keycode_MOVE_END));
                        driver.PressKeyCode(new KeyEvent(AndroidKeyCode.Keycode_MOVE_END));
                        Thread.Sleep(random.Next(500, 1000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.widget.Button");
                        Thread.Sleep(random.Next(5000, 8000));
                    }
                }
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.ScrollView/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout[2]/android.widget.LinearLayout[6]/android.widget.LinearLayout/android.widget.TextView");
                Thread.Sleep(1000);
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.Button");
                Thread.Sleep(2000);
                var WaitHome = AppiumHi.WaitXpath(driver, "//android.appwidget.AppWidgetHostView[@content-desc=\"Tìm kiếm\"]");
                //var serviceGoogle = AppiumHi.WaitXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.support.v7.widget.RecyclerView/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.TextView", 15);
                //if (serviceGoogle != null)
                //{
                //    if (serviceGoogle.Text == "Các dịch vụ của Google")
                //    {
                //        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.Button");
                //        Thread.Sleep(random.Next(1500, 2500));
                //        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.Button");
                //        Thread.Sleep(random.Next(5000, 8000));
                //    }
                //}
                driver.StartActivity("com.android.settings", "com.android.settings.Settings$AccountDashboardActivity");
                Thread.Sleep(3000);
                var checkAccount = AppiumHi.WaitXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.support.v7.widget.RecyclerView/android.widget.LinearLayout[1]/android.widget.LinearLayout[2]/android.widget.LinearLayout/android.widget.TextView[1]");
                if (checkAccount != null)
                {
                    if (checkAccount.Text == "Google")
                    {
                        //done
                        checkAccount.Click();
                        Thread.Sleep(random.Next(1000, 2000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.support.v7.widget.RecyclerView/android.widget.LinearLayout[1]/android.widget.RelativeLayout/android.widget.TextView[1]");
                        Thread.Sleep(random.Next(3000, 5000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.Button");
                        Thread.Sleep(random.Next(1000, 2000));
                        AppiumHi.ClickXpath(driver, "//android.widget.LinearLayout[@content-desc=\"Dữ liệu và quyền riêng tư\"]");
                        Thread.Sleep(random.Next(1000, 2000));
                        AppiumHi.ClickXpath(driver, "//android.widget.LinearLayout[@content-desc=\"Bảo mật\"]");
                        Thread.Sleep(random.Next(1000, 2000));
                        ADBHi.ADBSwipe(deviceID, 530, 1819, 530, 400);
                        Thread.Sleep(random.Next(1000, 2000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.ScrollView/android.widget.FrameLayout[2]/android.view.ViewGroup/androidx.viewpager.widget.ViewPager/android.view.ViewGroup/android.support.v7.widget.RecyclerView/android.view.ViewGroup[1]/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout[3]/android.widget.LinearLayout");
                        Thread.Sleep(random.Next(2000, 4000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[2]/android.view.View/android.view.View/android.view.View[2]/android.view.View");
                        Thread.Sleep(random.Next(500, 1000));
                        //AppiumHi.SendTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[2]/android.view.View/android.view.View/android.view.View[2]/android.view.View", profile.Pass);
                        ADBHi.ADBSendText(deviceID, profile.Pass);
                        Thread.Sleep(random.Next(500, 1000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[1]");
                        Thread.Sleep(random.Next(500, 1000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[4]/android.view.View/android.widget.Button");
                        Thread.Sleep(random.Next(3000, 5000));
                        var savePass = AppiumHi.WaitXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.LinearLayout[1]/android.widget.LinearLayout/android.widget.TextView");
                        if (savePass != null)
                        {
                            if (savePass.Text == "Lưu mật khẩu vào Google?")
                            {
                                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.LinearLayout[2]/android.widget.Button[1]");
                                Thread.Sleep(1000);
                            }
                        }
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View/android.view.View/android.view.View/android.view.View/android.view.View[2]/android.view.View[3]/android.view.View[1]/android.view.View[2]/android.view.View/android.view.View/android.view.View/android.widget.EditText");
                        Thread.Sleep(random.Next(500, 1000));
                        AppiumHi.SendTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View/android.view.View/android.view.View/android.view.View/android.view.View[2]/android.view.View[3]/android.view.View[1]/android.view.View[2]/android.view.View/android.view.View/android.view.View/android.widget.EditText", profile.Recovery);
                        Thread.Sleep(random.Next(500, 1000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View/android.view.View/android.view.View/android.view.View/android.view.View[1]");
                        Thread.Sleep(random.Next(500, 1000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View/android.view.View/android.view.View/android.view.View/android.view.View[2]/android.view.View[3]/android.view.View[1]/android.view.View[4]/android.widget.Button");
                        Thread.Sleep(random.Next(500, 1000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View/android.view.View/android.app.Dialog/android.view.View[4]/android.widget.Button[1]");
                        Thread.Sleep(random.Next(5000, 8000));
                        return true;
                    }
                }


            }
            catch
            {
                return false;
            }
            return false;

        }

        private bool CreateAccountGoogle(AndroidDriver<AndroidElement> driver,string deviceID,Profile profile)
        {
            try
            {

                driver.StartActivity("com.android.settings", "com.android.settings.Settings$AccountDashboardActivity");
                Random random = new Random();
                Thread.Sleep(random.Next(5000, 7000));
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.support.v7.widget.RecyclerView/android.widget.LinearLayout[1]/android.widget.LinearLayout/android.widget.RelativeLayout/android.widget.TextView");
                Thread.Sleep(random.Next(1000, 2000));
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.support.v7.widget.RecyclerView/android.widget.LinearLayout[5]/android.widget.LinearLayout/android.widget.RelativeLayout/android.widget.TextView");
                Thread.Sleep(random.Next(8000, 10000));
                AppiumHi.WaitXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[5]/android.view.View/android.widget.Spinner");
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[5]/android.view.View/android.widget.Spinner");
                Thread.Sleep(random.Next(1000, 2000));
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View/android.view.View[2]/android.view.View[5]/android.view.View[2]/android.view.View/android.view.View/android.view.MenuItem[1]");
                Thread.Sleep(random.Next(3000, 5000));
                string[] info = GeneratorInfoVietnam();
                string name = info[0];
                string[] nameParts = name.Split(' ');
                string firstName = nameParts[0];
                string lastName = string.Join(" ", nameParts.Skip(1));
                string gender = info[1];
                while (true)
                {
                    AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[1]/android.view.View/android.view.View[2]/android.view.View");
                    AppiumHi.ClearText(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[1]/android.view.View/android.view.View[2]/android.view.View");
                    Thread.Sleep(random.Next(1000, 2000));
                    ADBHi.ADBSendText(deviceID, firstName);
                    Thread.Sleep(random.Next(1000, 2000));
                    if(AppiumHi.CheckTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[1]/android.view.View/android.view.View[2]/android.view.View", firstName))
                    {
                        break;
                    }
                }
                //AppiumHi.SendTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[1]/android.view.View/android.view.View[2]/android.view.View", firstName);               
                while (true)
                {
                    AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[2]/android.view.View/android.view.View[2]/android.view.View");
                    AppiumHi.ClearText(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[2]/android.view.View/android.view.View[2]/android.view.View");
                    Thread.Sleep(random.Next(1000, 2000));
                    //AppiumHi.SendTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[2]/android.view.View/android.view.View[2]/android.view.View", lastName);
                    ADBHi.ADBSendText(deviceID, lastName);
                    Thread.Sleep(random.Next(1000, 2000));
                    if(AppiumHi.CheckTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[2]/android.view.View/android.view.View[2]/android.view.View", lastName))
                    {
                        break;
                    }
                    //driver.PressKeyCode(AndroidKeyCode.Enter);

                }
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[1]");
                Thread.Sleep(random.Next(1000, 2000));
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[4]/android.view.View/android.widget.Button");
                Thread.Sleep(random.Next(3000, 5000));
                string birthdayday = random.Next(1, 28).ToString();
                string birthdaymonth = random.Next(1, 10).ToString();
                string birthdayyear = random.Next(1970, 2000).ToString();
                while (true)
                {
                    AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[1]/android.view.View[2]/android.view.View");
                    AppiumHi.ClearText(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[1]/android.view.View[2]/android.view.View");
                    Thread.Sleep(random.Next(500, 1000));
                    //AppiumHi.SendTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[1]/android.view.View[2]/android.view.View", birthdayday);
                    ADBHi.ADBSendText(deviceID, birthdayday);
                    Thread.Sleep(random.Next(1000, 2000));
                    if (AppiumHi.CheckTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[1]/android.view.View[2]/android.view.View", birthdayday))
                    {
                        break;
                    }

                }
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[2]/android.view.View[2]/android.widget.Spinner");
                Thread.Sleep(random.Next(500, 1000));
                AppiumHi.ClickXpath(driver, $"/hierarchy/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.ListView/android.widget.CheckedTextView[{birthdaymonth}]");
                Thread.Sleep(random.Next(1000, 2000));
                while (true)
                {
                    AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[3]/android.view.View[2]/android.view.View");
                    AppiumHi.ClearText(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[3]/android.view.View[2]/android.view.View");
                    Thread.Sleep(random.Next(500, 1000));
                    //AppiumHi.SendTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[3]/android.view.View[2]/android.view.View", birthdayyear);
                    ADBHi.ADBSendText(deviceID, birthdayyear);
                    Thread.Sleep(random.Next(1000, 2000));
                    if (AppiumHi.CheckTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[3]/android.view.View[2]/android.view.View", birthdayyear))
                    {
                        break;
                    }
                }
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[4]/android.view.View[2]/android.widget.Spinner");
                Thread.Sleep(random.Next(500, 1000));
                if (gender.Contains("Male"))
                {
                    AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.ListView/android.widget.CheckedTextView[2]");
                }
                else
                {
                    AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.ListView/android.widget.CheckedTextView[1]");
                }
                Thread.Sleep(random.Next(1000, 2000));
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[4]/android.view.View/android.widget.Button");
                Thread.Sleep(random.Next(3000, 5000));
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View/android.view.View[3]/android.view.View");
                Thread.Sleep(random.Next(1000, 2000));
                while (true)
                {
                    AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[2]/android.view.View[3]/android.view.View");
                    AppiumHi.ClearText(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[2]/android.view.View[3]/android.view.View");
                    Thread.Sleep(random.Next(500, 1000));
                    //AppiumHi.SendTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[2]/android.view.View[3]/android.view.View", profile.ID);
                    ADBHi.ADBSendText(deviceID, profile.ID);
                    Thread.Sleep(random.Next(500, 1000));
                    if (AppiumHi.CheckTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[2]/android.view.View[3]/android.view.View", profile.ID))
                    {
                        break;
                    }
                }
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[1]");
                Thread.Sleep(random.Next(500, 1000));
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[4]/android.view.View/android.widget.Button");
                Thread.Sleep(random.Next(3000, 5000));
                while (true)
                {
                    AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[1]/android.view.View/android.view.View/android.view.View[2]/android.view.View");
                    AppiumHi.ClearText(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[1]/android.view.View/android.view.View/android.view.View[2]/android.view.View");
                    Thread.Sleep(random.Next(500, 1000));
                    //AppiumHi.SendTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[1]/android.view.View/android.view.View/android.view.View[2]/android.view.View", profile.Pass);
                    ADBHi.ADBSendText(deviceID, profile.Pass);
                    Thread.Sleep(random.Next(500, 1000));
                    if (AppiumHi.CheckTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[1]/android.view.View/android.view.View/android.view.View[2]/android.view.View", profile.Pass))
                    {
                        break;
                    }
                }
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[1]");
                Thread.Sleep(random.Next(500, 1000));
                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[4]/android.view.View/android.widget.Button");
                Thread.Sleep(random.Next(3000, 5000));
                var Ver = AppiumHi.WaitXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[1]", 15);
                if (Ver != null)
                {
                    if (Ver.Text == "Xác nhận bạn không phải là rô-bốt")
                    {                        
                        VerPhone:
                        string[] phone = GetPhoneNumber2ndLine();
                        if (phone[0].Contains("Fail"))
                        {
                            profile.Log = phone[0];
                            return false;
                        }
                        while (true)
                        {
                            AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[2]/android.view.View/android.view.View[3]/android.view.View/android.widget.EditText");
                            Thread.Sleep(random.Next(500, 1000));
                            AppiumHi.ClearText(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[2]/android.view.View/android.view.View[3]/android.view.View/android.widget.EditText");
                            //AppiumHi.SendTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[2]/android.view.View/android.view.View[3]/android.view.View/android.widget.EditText", phone[0]);
                            ADBHi.ADBSendText(deviceID, phone[0]);
                            Thread.Sleep(random.Next(500, 1000));
                            if (AppiumHi.CheckTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[2]/android.view.View/android.view.View[3]/android.view.View/android.widget.EditText", phone[0]))
                            {
                                break;
                            }
                        }
                        //AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[1]");
                        driver.PressKeyCode(AndroidKeyCode.Enter);
                        Thread.Sleep(random.Next(500, 1000));
                        var checkErrorPhone = AppiumHi.WaitXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[2]/android.view.View/android.view.View[4]");
                        if (checkErrorPhone != null)
                        {
                            if(checkErrorPhone.Text == "Không thể sử dụng số điện thoại này để xác minh.")
                            {
                                goto VerPhone;
                            }
                        }
                        //AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[4]/android.view.View/android.widget.Button");
                        Thread.Sleep(random.Next(3000, 5000));
                        string code = GetCode2ndline(phone[1]);
                        if (code.Contains("not"))
                        {
                            profile.Log = code;
                            return false;
                        }
                        while (true)
                        {
                            AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[2]/android.view.View/android.view.View[2]/android.view.View[3]/android.view.View");
                            AppiumHi.ClearText(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[2]/android.view.View/android.view.View[2]/android.view.View[3]/android.view.View");
                            Thread.Sleep(random.Next(500, 1000));
                            //AppiumHi.SendTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[2]/android.view.View/android.view.View[2]/android.view.View[3]/android.view.View", code);
                            ADBHi.ADBSendText(deviceID, code);
                            Thread.Sleep(random.Next(500, 1000));
                            if (AppiumHi.CheckTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[2]/android.view.View/android.view.View[2]/android.view.View[3]/android.view.View", code))
                            {
                                break;
                            }
                        }
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[1]");
                        Thread.Sleep(random.Next(500, 1000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.widget.Button");
                        Thread.Sleep(random.Next(3000, 5000));
                    }
                }
                var addPhone = AppiumHi.WaitXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[1]");
                if (addPhone.Text == "Thêm số điện thoại?")
                {
                    driver.PressKeyCode(new KeyEvent(AndroidKeyCode.Keycode_MOVE_END));
                    driver.PressKeyCode(new KeyEvent(AndroidKeyCode.Keycode_MOVE_END));
                    Thread.Sleep(random.Next(500, 1000));
                    AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[4]/android.view.View/android.widget.Button");
                    Thread.Sleep(random.Next(3000, 5000));
                }
                var checkInfo = AppiumHi.WaitXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[1]");
                if (checkInfo != null)
                {
                    if (checkInfo.Text == "Xem lại thông tin tài khoản của bạn")
                    {
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[4]/android.view.View/android.widget.Button");
                        Thread.Sleep(random.Next(3000, 5000));
                    }
                }
                var checkRules = AppiumHi.WaitXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[1]");
                if (checkRules != null)
                {
                    if (checkRules.Text == "Quyền riêng tư và Điều khoản")
                    {
                        driver.PressKeyCode(new KeyEvent(AndroidKeyCode.Keycode_MOVE_END));
                        driver.PressKeyCode(new KeyEvent(AndroidKeyCode.Keycode_MOVE_END));
                        Thread.Sleep(random.Next(500, 1000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.widget.Button");
                        Thread.Sleep(random.Next(5000, 8000));
                    }
                }
                var serviceGoogle = AppiumHi.WaitXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.support.v7.widget.RecyclerView/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.TextView", 15);
                if (serviceGoogle != null)
                {
                    if (serviceGoogle.Text == "Các dịch vụ của Google")
                    {
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.Button");
                        Thread.Sleep(random.Next(1500, 2500));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.Button");
                        Thread.Sleep(random.Next(5000, 8000));
                    }
                }
                var checkAccount = AppiumHi.WaitXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.support.v7.widget.RecyclerView/android.widget.LinearLayout[1]/android.widget.LinearLayout[2]/android.widget.LinearLayout/android.widget.TextView[1]");
                if(checkAccount != null)
                {
                    if(checkAccount.Text == "Google")
                    {
                        //done
                        checkAccount.Click();
                        Thread.Sleep(random.Next(1000, 2000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.support.v7.widget.RecyclerView/android.widget.LinearLayout[1]/android.widget.RelativeLayout/android.widget.TextView[1]");
                        Thread.Sleep(random.Next(3000, 5000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.Button");
                        Thread.Sleep(random.Next(1000, 2000));
                        AppiumHi.ClickXpath(driver, "//android.widget.LinearLayout[@content-desc=\"Dữ liệu và quyền riêng tư\"]");
                        Thread.Sleep(random.Next(1000, 2000));
                        AppiumHi.ClickXpath(driver, "//android.widget.LinearLayout[@content-desc=\"Bảo mật\"]");
                        Thread.Sleep(random.Next(1000, 2000));
                        ADBHi.ADBSwipe(deviceID, 530, 1819, 530, 400);
                        Thread.Sleep(random.Next(1000, 2000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.ScrollView/android.widget.FrameLayout[2]/android.view.ViewGroup/androidx.viewpager.widget.ViewPager/android.view.ViewGroup/android.support.v7.widget.RecyclerView/android.view.ViewGroup[1]/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout[3]/android.widget.LinearLayout");
                        Thread.Sleep(random.Next(2000, 4000));                       
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[2]/android.view.View/android.view.View/android.view.View[2]/android.view.View");
                        Thread.Sleep(random.Next(500, 1000));
                        //AppiumHi.SendTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[2]/android.view.View/android.view.View/android.view.View[2]/android.view.View", profile.Pass);
                        ADBHi.ADBSendText(deviceID, profile.Pass);
                        Thread.Sleep(random.Next(500, 1000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[3]/android.view.View/android.view.View[1]");
                        Thread.Sleep(random.Next(500, 1000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View[2]/android.view.View[4]/android.view.View/android.widget.Button");
                        Thread.Sleep(random.Next(3000, 5000));
                        var savePass = AppiumHi.WaitXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.LinearLayout[1]/android.widget.LinearLayout/android.widget.TextView");
                        if(savePass != null)
                        {
                            if(savePass.Text == "Lưu mật khẩu vào Google?")
                            {
                                AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.LinearLayout/android.widget.LinearLayout[2]/android.widget.Button[1]");
                                Thread.Sleep(1000);
                            }
                        }
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View/android.view.View/android.view.View/android.view.View/android.view.View[2]/android.view.View[3]/android.view.View[1]/android.view.View[2]/android.view.View/android.view.View/android.view.View/android.widget.EditText");
                        Thread.Sleep(random.Next(500, 1000));
                        AppiumHi.SendTextXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View/android.view.View/android.view.View/android.view.View/android.view.View[2]/android.view.View[3]/android.view.View[1]/android.view.View[2]/android.view.View/android.view.View/android.view.View/android.widget.EditText", profile.Recovery);
                        Thread.Sleep(random.Next(500, 1000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View/android.view.View/android.view.View/android.view.View/android.view.View[1]");
                        Thread.Sleep(random.Next(500, 1000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View/android.view.View/android.view.View/android.view.View/android.view.View[2]/android.view.View[3]/android.view.View[1]/android.view.View[4]/android.widget.Button");
                        Thread.Sleep(random.Next(500, 1000));
                        AppiumHi.ClickXpath(driver, "/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.FrameLayout/android.view.ViewGroup/android.widget.FrameLayout/android.webkit.WebView/android.webkit.WebView/android.view.View/android.view.View/android.app.Dialog/android.view.View[4]/android.widget.Button[1]");
                        Thread.Sleep(random.Next(5000, 8000));
                        return true;
                    }
                }


            }
            catch 
            {
                return false;
            }
            return false;
            
        }

        static string[] GeneratorInfoVietnam()
        {
            HttpRequest httpRequest = new HttpRequest();
            httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36";
            string html = httpRequest.Get("https://dichthuatphuongdong.com/tienich/random-vietnamese-profile.html").ToString();
            string regex = "(?<=<td><strong>)[^<]+(?=</strong></td>)";
            MatchCollection match = Regex.Matches(html, regex);
            string[] result = match.Cast<Match>().Select(m => m.Value).ToArray();
            return result;
        }
        
        private string[] GetPhoneNumber2ndLine()
        {
            string api = Dispatcher.Invoke(() => { return Api_TextBox.Text; });
            HttpRequest httpRequest = new HttpRequest();
            httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36";
            string html = httpRequest.Get($"https://2ndline.io/apiv1/getbalance?apikey={api}").ToString();
            JObject obj = JObject.Parse(html);
            string status = obj["status"].ToString();
            string balance = obj["balance"].ToString();
            if(status == "0")
            {
                string[] Fail = { "Fail Check 2ndline.io" };
                return Fail;
            }
            if(balance == "0")
            {
                string[] Fail = { "Fail insufficient balance 2ndline.io" };
                return Fail;
            }
            GetPhone:
            string html1 = httpRequest.Get($"https://2ndline.io/apiv1/order?apikey={api}&serviceId=292&allowVoiceSms=false").ToString();
            JObject obj1 = JObject.Parse(html1);
            string status1 = obj1["status"].ToString();
            string id = obj1["id"].ToString();
            string phone = obj1["phone"].ToString();
            if (status1 == "0")
            {
                goto GetPhone;
            }
            if(phone == "")
            {
                while (true)
                {
                    string html2 = httpRequest.Get($"https://2ndline.io/apiv1/ordercheck?apikey={api}&id={id}").ToString();
                    JObject obj2 = JObject.Parse(html2);
                    phone = obj2["data"]["phone"].ToString();
                    var status2 = obj2["data"]["statusOrder"].ToString();
                    if (phone != "")
                    {
                        break;
                    }
                    if(status2 == "-1")
                    {
                        string[] Fail = { "Fail Get PhoneNumber" };
                        return Fail;
                    }
                    Thread.Sleep(1000);
                }                
            }
            string[] Done = { phone , id };
            return Done;
        }
        private string GetCode2ndline (string id)
        {
            string api = Dispatcher.Invoke(() => { return Api_TextBox.Text; });
            HttpRequest httpRequest = new HttpRequest();
            httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36";
            for (int i = 0; i< 60; i++)
            {
                string html = httpRequest.Get($"https://2ndline.io/apiv1/ordercheck?apikey={api}&id={id}").ToString();
                JObject obj = JObject.Parse(html);
                string code = obj["data"]["code"].ToString();
                if(code != "")
                {
                    return code;
                    
                }
                Thread.Sleep(1000);
                
            }
            string Fail = "Code not received";
            return Fail;
        }

    }
}
