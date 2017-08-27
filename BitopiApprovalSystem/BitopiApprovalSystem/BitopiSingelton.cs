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
            get;set;
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
            builder.SetMessage("Version:" + version.VersionName+"\nVersionCode: "+ version.VersionCode + "\n© " + DateTime.Now.Year + " Bitopi Group.\nAll Rights Reserved.");

            builder.SetCancelable(false);
            builder.SetPositiveButton("OK", delegate { builder.Dispose(); });
            

            builder.Show();

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
    [Application(Debuggable =false)]
    public class BitopiApplication : Application
    {
        
        public UserModel User;
        public int ApprovalId;
        public ApprovalType ApprovalType;
        public ApprovalRoleType ApprovalRoleType;
        public string CurrentVersion;
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
                _locList[0] = _locList[1];
                _locList[1] = _locList[2];
                _locList[2] = value;
                this.LocationList = _locList;
            }
        }
        public DDL[] LocationList
        {
            get
            {
                var text = "";

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
                var text = Newtonsoft.Json.JsonConvert.SerializeObject(value);
                var documents = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                var filename = System.IO.Path.Combine(documents, "mfc.txt");
                System.IO.File.WriteAllText(filename, text);
                ;
            }
        }

    }
}