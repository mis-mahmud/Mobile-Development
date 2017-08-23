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
using Android.Util;
using Android.Gms.Gcm.Iid;
using Android.Gms.Gcm;


namespace BitopiApprovalSystem.PushNotification
{

    [Service(Exported=false)]
    class BitopiRegistrationIntentService:IntentService
    {
        static object locker = new object();
        public BitopiRegistrationIntentService() : base("BitopiRegistrationIntentService") { }
        protected override void OnHandleIntent(Intent intent)
        {
           
            try
            {
                //string serializedApplicationContext = intent.GetStringExtra("tytApplicationContext");
                //applicaion = Newtonsoft.Json.JsonConvert.DeserializeObject<TytMoblieApplication>(serializedApplicationContext);
                Log.Info("BitopiRegistrationIntentService", "Calling InstanceID.GetToken");
                lock (locker)
                {
                    var instanceID = InstanceID.GetInstance(this);
                    var token = instanceID.GetToken(PushNotificationSingleton.Instance.SenderId
                        , GoogleCloudMessaging.InstanceIdScope);
                    Log.Info("BitopiRegistrationIntentService", "GCM Registration Token: " + token);
                    //SendRegistrationToAppServer(token);
                    PushNotificationSingleton.Instance.TokenId = token;
                    Subscribe(token);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("BitopiRegistrationIntentService", "Failed to get a registration token");
                return;
            }
        }
        void SendRegistrationToAppServer(string token)
        {
            //TytMoblieApplication application = (TytMoblieApplication)this.ApplicationContext;
            //UserRepository repo = new UserRepository();
            //repo.SaveTokenId(application.EmployeeId, application.ClientId, token, application.MacAddress,application.IMEIId, application.DeviceName);
        }
        void Subscribe(string token)
        {
            var pubSub = GcmPubSub.GetInstance(this);
            pubSub.Subscribe(token, "/topics/bitopiAlert", null);
            PushNotificationSingleton.Instance.TokenId = token;
        }
    }
   public class ServiceData 
    {
      public int  EmployeeId {get;set;}
                public int   ClientId{get;set;} 
                public string     DeviceId {get;set;}
                public string    DeviceName {get;set;}
    }
}