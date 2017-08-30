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
using BitopiApprovalSystem.Model;
//using Android.Gms.Common;


namespace BitopiApprovalSystem.PushNotification
{
   public  class PushNotificationSingleton
    {
       public static PushNotificationSingleton Instance = new PushNotificationSingleton();
       PushNotificationSingleton()
       {
           this.Messages = new List<BitopiGcmMessage>();
       }
       public List<BitopiGcmMessage> Messages { get; set; }
       public string SenderId { get { return "7095124832"; } }

        //public string SenderId { get; set; }
        public string TokenId { get; set; }
       public void SaveNotification(string msg)
       {
            try
            {
                var documents = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                var filename = System.IO.Path.Combine(documents, "myNotificaion.txt");
                System.IO.File.WriteAllText(filename, msg);
            }
            catch { }
       }
       public string ReadNotification()
       {
           var text = "";

           var documents = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
           var filename = System.IO.Path.Combine(documents, "myNotificaion.txt");
           if (System.IO.File.Exists(filename))
               text = System.IO.File.ReadAllText(filename);
           return text;
       }
        //public bool IsGooglePlayServicesAvailable(Context context)
        //{

        //    int resultCode = GooglePlayServicesUtil.IsGooglePlayServicesAvailable(context);
        //    if (resultCode != ConnectionResult.Success)
        //    {
        //        if (GooglePlayServicesUtil.IsUserRecoverableError(resultCode))
        //        {
        //            //Play Services is not installed/enabled 
        //            GooglePlayServicesUtil.ShowErrorNotification(resultCode, context);
        //        }
        //        else
        //        {
        //            //This device does not support Play Services
        //        }
        //        return false;
        //    }
        //    return true;
        //}

    }
}