using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Model;
using BitopiApprovalSystem.Model;
using Newtonsoft.Json;
using Android.Text;
using Android.Text.Util;
using System.Net;
using System.IO;

namespace BitopiApprovalSystem
{
    public class BitopiSingelton
    {
        BitopiSingelton()
        {
            ReceivingMessages = new Dictionary<string, List<string>>();
        }
        static BitopiSingelton _instance;
        public UserModel User;
        public int ApprovalId;
        public ApprovalType ApprovalType;
        public ApprovalRoleType ApprovalRoleType;
        public string CurrentVersion;
        public string CurrentActivity;
        public MyTaskType MyTaskType;
        public string MacAddress
        {
            get; set;
            //get
            //{
            //    //WifiManager wifiMan = (WifiManager)this.GetSystemService(Context.WifiService);
            //    //WifiInfo wifiInf = wifiMan.ConnectionInfo;

            //    //return UID.id(this);
            //    //// return wifiInf.MacAddress;
            //    ////return "";
            //}
            //set { }
        }

        public void ShowAboutDialog(Activity activity)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(activity);
            builder.SetTitle(" About ");
            var version = activity.PackageManager.GetPackageInfo(activity.PackageName,
                Android.Content.PM.PackageInfoFlags.MetaData);
            builder.SetMessage("Version:" + version.VersionName + "\nVersionCode: " + version.VersionCode + "\n© " + DateTime.Now.Year + " Bitopi Group.\nAll Rights Reserved.");

            builder.SetCancelable(false);
            builder.SetPositiveButton("OK", delegate { builder.Dispose(); });


            builder.Show();

        }
        public void ShowNewVersionDialog(Activity activity)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(activity);
            builder.SetTitle(" New Version Available ");
            var version = activity.PackageManager.GetPackageInfo(activity.PackageName,
                Android.Content.PM.PackageInfoFlags.MetaData);
            SpannableString s = new SpannableString(String.Format("Please download the new version of this app from"));
            Linkify.AddLinks(s, MatchOptions.WebUrls);
            //builder.SetMessage(s);

            builder.SetCancelable(false);
            builder.SetPositiveButton("Download", delegate
            {
                builder.Dispose();

                DownloadFile(activity);

            });



            builder.Show();

        }
        public void DownloadFile(Activity activity, string m_uri = "http://apps.bitopibd.com/BitopiApps/bitopiOTG.apk")
        {
            var webClient = new WebClient();
            ProgressDialog pbProgress;
            pbProgress = new ProgressDialog(activity);
            pbProgress.Indeterminate = false;
            pbProgress.SetProgressNumberFormat("%1d MB / %2d MB");
            if (System.IO.File.Exists(Android.OS.Environment.ExternalStorageDirectory + "/download/bitopiOTG.apk"))
             System.IO.File.Delete(Android.OS.Environment.ExternalStorageDirectory + "/download/bitopiOTG.apk");
            
            webClient.DownloadFileCompleted += (s, e) =>
            {

                Intent promptInstall = new Intent(Intent.ActionView).SetDataAndType(Android.Net.Uri.FromFile(new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory + "/download/" + "bitopiOTG.apk")), "application/vnd.android.package-archive");
                promptInstall.AddFlags(ActivityFlags.NewTask);
                activity.StartActivity(promptInstall);
                pbProgress.Dismiss();
                //Android.OS.Process.KillProcess(Android.OS.Process.MyPid());

            };

            var url = new System.Uri(m_uri);

            string directory = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads);
            string file = Path.Combine(directory, "/bitopiOTG.apk");
            WebClient client = new WebClient();

            webClient.DownloadProgressChanged += (sender, e) =>
            {
                int length = (Convert.ToInt32(e.TotalBytesToReceive.ToString()) / 1024) / 1024;
                int prog = (Convert.ToInt32(e.BytesReceived.ToString()) / 1024) / 1024;
                int perc = Convert.ToInt32(e.ProgressPercentage.ToString());
                pbProgress.Max = length;
                pbProgress.Progress = prog;
            };
            webClient.DownloadFileAsync(url, Android.OS.Environment.ExternalStorageDirectory + "/download/bitopiOTG.apk");
            pbProgress.SetProgressStyle(ProgressDialogStyle.Horizontal);
            pbProgress.Progress = 0;
            pbProgress.Show();
        }
        public void ClearData()
        {
            _instance.User = null;

        }
        public string DeviceName
        {
            get
            {
                return Android.OS.Build.Manufacturer + " " + Android.OS.Build.Model;
            }
        }
        public static BitopiSingelton Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BitopiSingelton();
                return _instance;
            }
        }

        public Dictionary<string, List<string>> ReceivingMessages { get; set; }
    }
    [Application(Debuggable = false)]
    public class BitopiApplication : Application
    {

        public UserModel User;
        public int ApprovalId;
        public ApprovalType ApprovalType;
        public ApprovalRoleType ApprovalRoleType;
        public int CurrentVersion;
        public string CurrentActivity;
        public MyTaskType MyTaskType;

        public string ProcessID;
        public string ProcessName;
        public string LocationID;
        public string LocationName;
        public List<DDL> DDLList;
        public string PRStatus;
        public BitopiApplication(IntPtr handle, JniHandleOwnership transfer)
            : base(handle, transfer)
        {
            //ReceivingMessages = new Dictionary<string, List<string>>();
        }
        public string MacAddress
        {
            get; set;
            //get
            //{
            //    //WifiManager wifiMan = (WifiManager)this.GetSystemService(Context.WifiService);
            //    //WifiInfo wifiInf = wifiMan.ConnectionInfo;

            //    //return UID.id(this);
            //    //// return wifiInf.MacAddress;
            //    ////return "";
            //}
            //set { }
        }

        public void ShowAboutDialog(Activity activity)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(activity);
            builder.SetTitle(" About ");
            var version = activity.PackageManager.GetPackageInfo(activity.PackageName,
                Android.Content.PM.PackageInfoFlags.MetaData);
            builder.SetMessage("Version:" + version.VersionName + "\nVersionCode: " + version.VersionCode + "\n© " + DateTime.Now.Year + " Bitopi Group.\nAll Rights Reserved.");

            builder.SetCancelable(false);
            builder.SetPositiveButton("OK", delegate { builder.Dispose(); });


            builder.Show();

        }
        public void ClearData()
        {
            // _instance.User = null;
            User = null;

        }
        public string DeviceName
        {
            get
            {
                return Android.OS.Build.Manufacturer + " " + Android.OS.Build.Model;
            }
        }
        public Dictionary<string, List<string>> ReceivingMessages { get; set; }
        public delegate void OkButtonClickedOfNetErrorDialog();
        public OkButtonClickedOfNetErrorDialog okButtonClickedOfNetErrorDialog;
        public bool IsInternetConnectionAvailable(Activity activity)
        {
            var connectivityManager = (Android.Net.ConnectivityManager)GetSystemService(ConnectivityService);
            var activeConnection = connectivityManager.ActiveNetworkInfo;
            if ((activeConnection != null) && activeConnection.IsConnected)
            {

                return true;
            }

            //Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(activity);
            //builder.SetTitle("Internet Connection Error");
            //builder.SetMessage("No internet connection available. Please check your device.");
            //builder.SetCancelable(false);
            //builder.SetPositiveButton("OK", delegate { if (okButtonClickedOfNetErrorDialog != null) okButtonClickedOfNetErrorDialog(); builder.Dispose(); });
            //builder.Show();

            return false;
        }
    }
    public class ProcessSingleton
    {
        public static ProcessSingleton Instance = new ProcessSingleton();

        ProcessSingleton()
        {
        }
        public DDL Location
        {
            set
            {
                DDL[] _locList = this.LocationList;
                _locList[0] = _locList[1] != null ? _locList[1] : new DDL { LocationName = "", ProcessName = "" };
                _locList[1] = _locList[2] != null ? _locList[2] : new DDL { LocationName = "", ProcessName = "" };
                _locList[2] = value;
                this.LocationList = _locList;
            }
        }
        public DDL[] LocationList
        {
            get
            {
                var text = "";
                //string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                //var filename= System.IO.Path.Combine(path, "mfc.txt");

                var documents = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                var filename = System.IO.Path.Combine(documents, "mfc.txt");
                if (System.IO.File.Exists(filename))
                    text = System.IO.File.ReadAllText(filename);
                if (text != "")
                    return JsonConvert.DeserializeObject<DDL[]>(text);
                else
                    return new DDL[3];
            }
            set
            {
                try
                {
                    var text = Newtonsoft.Json.JsonConvert.SerializeObject(value);
                    //string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                    //var filename = System.IO.Path.Combine(path, "mfc.txt");
                    var documents = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                    var filename = System.IO.Path.Combine(documents, "mfc.txt");
                    System.IO.File.WriteAllText(filename, text);
                    ;
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }
            }
        }
        public void RemoveProcess()
        {
            var documents = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var filename = System.IO.Path.Combine(documents, "mfc.txt");

            //string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            //var filename = System.IO.Path.Combine(path, "mfc.txt");
            if (System.IO.File.Exists(filename))
                System.IO.File.Delete(filename);
        }

    }
}