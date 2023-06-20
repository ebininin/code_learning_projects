using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Reg_Gmail_Android_Mi_A1
{
    class ADBHi
    {
        public static void ADBProcess (string cmd)
        {
            Process process = new Process();
            process.StartInfo.FileName = "adb.exe";
            process.StartInfo.Arguments = cmd;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start(); ;
            process.WaitForExit();
        }

        public static void ADBProcessWithoutWait (string cmd)
        {
            Process process = new Process();
            process.StartInfo.FileName = "adb.exe";
            process.StartInfo.Arguments = cmd;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start(); ;
        }

        public static string ADBProcessGetResult(string cmd)
        {
            Process process = new Process();
            process.StartInfo.FileName = "adb.exe";
            process.StartInfo.Arguments = cmd;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            Thread.Sleep(1000);
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            string list = output;
            return list;
        }

        public static string[] ADBGetConnectDevices()
        {
            Process process = new Process();
            process.StartInfo.FileName = "adb.exe";
            process.StartInfo.Arguments = "devices";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            Thread.Sleep(1000);
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            string[] list = output.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].Contains("\toffline"))
                {
                    list[i] = list[i].Replace("\toffline", "");
                }
                else if (list[i].Contains("\tdevice"))
                {
                    list[i] = list[i].Replace("\tdevice", "");
                }
                else;
            }
            list = list.Skip(1).ToArray();
            return list;
        }

        public static void ADBClearPackageGoogle (string deviceID)
        {
            string[] googlePackages = {
                                        "com.google.android.youtube",
                                        "com.google.android.ext.services",
                                        "com.google.android.googlequicksearchbox",
                                        "com.google.android.onetimeinitializer",
                                        "com.google.android.ext.shared",
                                        "com.google.android.configupdater",
                                        "com.google.android.ar.lens",
                                        "com.google.android.marvin.talkback",
                                        "com.google.android.apps.work.oobconfig",
                                        "com.google.android.gm",
                                        "com.google.android.apps.tachyon",
                                        "com.google.android.apps.wellbeing",
                                        "com.google.android.dialer",
                                        "com.google.android.apps.nbu.files",
                                        "com.google.android.contacts",
                                        "com.google.android.syncadapters.contacts",
                                        "com.google.android.calculator",
                                        "com.google.android.chrome",
                                        "com.google.android.packageinstaller",
                                        "com.google.android.gms",
                                        "com.google.android.gsf",
                                        "com.google.android.ims",
                                        "com.google.android.tts",
                                        "com.google.android.partnersetup",
                                        "com.google.android.videos",
                                        "com.google.android.apps.photos",
                                        "com.google.android.calendar",
                                        "com.android.vending"
                                    };
            foreach (var package in googlePackages)
            {
                string cmd = $"-s {deviceID} shell pm clear {package}";
                ADBProcess(cmd);
                Thread.Sleep(100);
            }
        }

        public static void ADBTap (string deviceID , long x , long y)
        {
            string cmd = $"-s {deviceID} shell input tap {x} {y}";
            ADBProcess(cmd);
        }

        public static void ADBSwipe (string deviceID, long x, long y , long x1 , long y1)
        {
            string cmd = $"-s {deviceID} shell input swipe {x} {y} {x1} {y1}";
            ADBProcess(cmd);
        }

        public static void ADBSendText(string deviceID, string text)
        {
            text = Regex.Replace(text, @"[\u0300-\u036f]", "");
            text = RemoveDiacritics(text);
            text = text.Replace(" ", "\\ ");
            if(text.Contains("đ"))
            {
                text = text.Replace("đ", "d");
            }
            if (text.Contains("Đ"))
            {
                text = text.Replace("Đ", "D");
            }
            string cmd = $"-s {deviceID} shell input keyboard text {text}";
            ADBProcess(cmd);
        }

        static string RemoveDiacritics(string str)
        {
            str = str.Normalize(System.Text.NormalizationForm.FormD);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < str.Length; i++)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(str[i]) != System.Globalization.UnicodeCategory.NonSpacingMark)
                    sb.Append(str[i]);
            }          
            return sb.ToString();
        }

        public static string ADBCheckAirplane (string deviceID)
        {
            try
            {
                string cmd = $"-s {deviceID} shell cmd connectivity airplane-mode";
                var result = ADBProcessGetResult(cmd);
                return result;
            }
            catch
            {               
            }
            return null;
        }

        public static void ADBTurnOnAirplane (string deviceID)
        {
            string cmd = $"-s {deviceID} shell cmd connectivity airplane-mode enable";
            ADBProcess(cmd);
        }

        public static void ADBTurnOffAirplane(string deviceID)
        {
            string cmd = $"-s {deviceID} shell cmd connectivity airplane-mode disable";
            ADBProcess(cmd);
        }

        public static void ADBTurnOnMobileData(string deviceID)
        {
            string cmd = $"-s {deviceID} shell svc data enable";
            ADBProcess(cmd);
        }

        public static void ADBTurnOffMobileData(string deviceID)
        {
            string cmd = $"-s {deviceID} shell svc data disable";
            ADBProcess(cmd);
        }
    }
}
