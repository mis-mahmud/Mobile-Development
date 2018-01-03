using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using System.Net;

using Newtonsoft.Json;

namespace BitopiApprovalSystem.Library
{
    public class CustomLogger
    {

        public const string APP_TAG = "BITOPIApproval";
        public static int VersionName;
        public static void CustomLog(string msg, string tag, string employeeName)
        {
            Log.Info(tag, System.DateTime.Now.ToString() + " - " + msg);
            CustomLogger.CustomWriteToLog(msg, tag, employeeName + ".txt");
        }

        public static void CustomActivityLog(string msg, string tag)
        {
            Log.Info(tag, System.DateTime.Now.ToString() + " - " + msg);
            CustomLogger.CustomWriteToLog(msg, tag, "activity.txt");
        }

        public static void CustomDBSyncLog(string msg, string tag)
        {
            Log.Info(tag, System.DateTime.Now.ToString() + " - " + msg);
            CustomLogger.CustomWriteToLog(msg, tag, "dbSync.txt");
        }

        private static void CustomWriteToLog(string msg, string tag, string logFileName)
        {
            try
            {
                //string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); //Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                string path = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "BITOPERROR");

                if (!System.IO.File.Exists(path))
                {
                    Java.IO.File dir = new Java.IO.File(path);
                    bool test = dir.Mkdir();
                }
                string filename = Path.Combine(path, logFileName);

                using (var streamWriter = new StreamWriter(filename, true))
                {
                    streamWriter.WriteLine(System.DateTime.Now.ToString() + " (" + VersionName + ") - " + msg + System.Environment.NewLine);
                }
            }
            catch
            {

            }
        }
        private static void CustomWriteToLogToPersonal(string msg, string logFileName)
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); //Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            string filename = Path.Combine(path, DateTime.Now.ToString("MM-dd-yyyy") + "_" + logFileName);

            using (var streamWriter = new StreamWriter(filename, true))
            {
                streamWriter.WriteLine(msg);
            }
        }

        public static void DeleteOldLogs()
        {
            //string directory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string directory = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "BITOPERROR");
            var dir = new DirectoryInfo(directory);
            try
            {
                foreach (var item in dir.GetFileSystemInfos().Where(item => item.FullName.Contains(".txt")))
                {
                    string[] fileName = item.FullName.Split('/');

                    if (Convert.ToDateTime(fileName[fileName.Length - 1].Split('_')[0]) < DateTime.Now.AddDays(-5))
                    {
                        File.Delete(item.FullName);
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }

        }


    }
}