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

namespace pa
{
    public class BitopiApplication 
    {

        public UserModel User;
        public int ApprovalId;
        public string ProcessID;
        public string ProcessName;
        public string CurrentVersion;
        public string CurrentActivity;
        public List<DDL> DDLList;
        public BitopiApplication()
        {
            //ReceivingMessages = new Dictionary<string, List<string>>();
        }
        public string MacAddress
        {
            get; set;
           
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