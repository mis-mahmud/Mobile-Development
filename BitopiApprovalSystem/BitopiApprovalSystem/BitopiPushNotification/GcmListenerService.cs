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
using Android.Gms.Gcm;
using Android.Util;
using Android.Graphics;
using System.IO;

using System.Threading;

using Android.Preferences;
using Android.Support.V4.App;
using Newtonsoft.Json;
using Android;
using BitopiApprovalSystem;
using BitopiApprovalSystem.Model;
using ApiRepository;
using BitopiApprovalSystem.Library;

namespace BitopiApprovalSystem.PushNotification
{
    [Service(Exported = false), IntentFilter(new[] { "com.google.android.c2dm.intent.RECEIVE" })]
    public class BitopiGcmListenerService : GcmListenerService
    {

        public override void OnMessageReceived(string from, Bundle data)
        {
            var messageStr = data.GetString("message");
            BitopiGcmMessage gcmMsg = JsonConvert.DeserializeObject<BitopiGcmMessage>(messageStr);
                SendNotification(gcmMsg);
        }
        void SendNotification(BitopiGcmMessage gcmMsg)
        {
            try
            {
                string title = "", msgBody = "";

                // TytMoblieApplication application = (TytMoblieApplication)this.ApplicationContext;
                //BitopiGcmMessage gcmMsg = JsonConvert.DeserializeObject<BitopiGcmMessage>(message);
                string approvalName = gcmMsg.Approval == "1" ? "PO" :
                    gcmMsg.Approval == "4" ? "Cash Requisition" : "";
                int requestID = Convert.ToInt16(gcmMsg.Approval);
                if (BitopiSingelton.Instance.ReceivingMessages.ContainsKey(gcmMsg.Approval))
                {
                    BitopiSingelton.Instance.ReceivingMessages[gcmMsg.Approval].Add(gcmMsg.POID);
                    msgBody = "You have " + BitopiSingelton.Instance.ReceivingMessages[gcmMsg.Approval].Count() + " " + approvalName + " to " + gcmMsg.ApprovalType;
                }
                else
                {
                    BitopiSingelton.Instance.ReceivingMessages.Add(gcmMsg.Approval, new List<string> { gcmMsg.POID });
                    msgBody = "You have 1 " + approvalName + " to " + gcmMsg.ApprovalType;
                }
                ApprovalRepository repo = new ApprovalRepository();
                int result = repo.ReceiveNotification(BitopiSingelton.Instance.User.UserCode,
                    BitopiSingelton.Instance.MacAddress, (ApprovalType)Convert.ToInt32(gcmMsg.Approval),gcmMsg.ApprovalName, gcmMsg.POID).Result;
                gcmMsg = new BitopiGcmMessage();
                //ISharedPreferences sp = PreferenceManager.GetDefaultSharedPreferences(this);
                //string employeeid = sp.GetString("employeeid_id", "");

                gcmMsg.DateCreated = DateTime.Now;
                Intent intent;


                intent = new Intent(this, typeof(StartupActivity));


                intent.AddFlags(ActivityFlags.ClearTop);

                var pendingIntent = PendingIntent.GetActivity(ApplicationContext, requestID, intent, 0);



                //requestID = DateTime.Now.Millisecond;
                var notificationBuilder = new NotificationCompat.Builder(this)
                .SetSmallIcon(BitopiApprovalSystem.Resource.Drawable.bitopiLogo)
                .SetContentTitle("Bitopi Approval System")
                .SetContentText(msgBody)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent)
                .SetDefaults(NotificationCompat.DefaultSound);


                var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);

                //new UserRepository().GetNotificationCacheAsCompleted(gcmMsg.NotificationExecId);
                notificationManager.Notify(requestID, notificationBuilder.Build());

            }
            catch (Exception ex)
            {
                CustomLogger.CustomLog("From Activity: " + BitopiSingelton.Instance.CurrentActivity + "\nMessage: " + ex.Message + "\nStack Trace: " + ex.StackTrace + "\n\n", "", BitopiSingelton.Instance.User != null ?
                             BitopiSingelton.Instance.User.UserName : "");
            }
        }


        
        public override void OnDestroy()
        {
            base.OnDestroy();

        }
    }
}