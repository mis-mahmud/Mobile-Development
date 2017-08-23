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
}