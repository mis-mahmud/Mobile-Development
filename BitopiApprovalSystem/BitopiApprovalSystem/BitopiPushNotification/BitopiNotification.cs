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
using ApiRepository;
using BitopiApprovalSystem.Model;
using Newtonsoft.Json;
using System.Threading;
using Android.Support.V4.App;
using System.Threading.Tasks;
using System.Net.Http;
using Badge.Plugin;
using BitopiApprovalSystem.Library;
using BitopiApprovalSystem;
using Model;
using BitopiApprovalSystem.PushNotification;
using BitopiApprovalSystem.DAL;

namespace BitopiApprovalSystem.BitopiPushNotification
{
    [Service(Exported = false, Enabled = true,
    Name = "com.bitopi.approvalsystem.BitopiNotificationService")]
    public class BitopiNotificationService : StickyIntentService
    {

        public BitopiNotificationService() : base("BitopiNotificationService") { }
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {

            new Task(() =>
            {
                System.Timers.Timer timer = new System.Timers.Timer();
                //ApprovalRepository repo = new ApprovalRepository();
                timer.Interval = 10000;
                timer.Elapsed += (s, e) =>
                {
                    Intent notiIntent=null;
                    
                    BitopiApplication bitopiApplication = (BitopiApplication)this.ApplicationContext;
                    try
                    {
                        if (bitopiApplication.User == null || String.IsNullOrEmpty(bitopiApplication.User.UserCode) == true)
                        {
                            ISharedPreferences pref = Application.Context.GetSharedPreferences("_bitopi_UserInfo", FileCreationMode.Private);
                            string _userCode = pref.GetString("UserCode", String.Empty);
                            ISharedPreferences prefToken = Application.Context.GetSharedPreferences("_bitopi_DeviceToken", FileCreationMode.Private);
                            string Token = prefToken.GetString("Token", String.Empty);
                            if (String.IsNullOrEmpty(Token))
                            {
                                bitopiApplication.MacAddress = Guid.NewGuid().ToString();
                                prefToken.Edit().PutString("Token", bitopiApplication.MacAddress).Commit();
                            }
                            else
                                bitopiApplication.MacAddress = Token;
                            if (_userCode == "")
                            {
                                notiIntent = new Intent(this, typeof(LoginActivity));
                            }
                            else
                            {
                                notiIntent = new Intent(this, typeof(BitopiActivity));
                                bitopiApplication.User = new UserModel() { UserCode = _userCode };
                                string url = RepositorySettings.BaseURl + "Notification?UserID=" + Cipher.Encrypt(bitopiApplication.User.UserCode)
                         + "&DeviceID=" + bitopiApplication.MacAddress;

                                HttpClient client = new HttpClient();
                                HttpResponseMessage result = client.GetAsync(url).Result;
                                var messages = JsonConvert.DeserializeObject<List<BitopiGcmMessage>>(result.Content.ReadAsStringAsync().Result);
                                if (messages != null && messages.Count > 0)
                                    SendNotification(messages.First(), bitopiApplication.MacAddress);
                            }
                            
                        }
                        else
                        {
                            notiIntent = new Intent(this, typeof(BitopiActivity));
                        }

                        int UpdateVersion = new AccountRepository().GetVersion();
                        int lastUpdateVersion = DBAccess.Database.LastVersion();
                        if(lastUpdateVersion==0)
                        {
                            lastUpdateVersion = this.PackageManager.GetPackageInfo(this.PackageName,
                Android.Content.PM.PackageInfoFlags.MetaData).VersionCode;
                            DBAccess.Database.InsertVersion(lastUpdateVersion);
                        }
                        if (UpdateVersion > lastUpdateVersion)
                        {

                            int requestID = DateTime.Now.Millisecond;
                            DBAccess.Database.DeleteVersion(lastUpdateVersion);
                            DBAccess.Database.InsertVersion(UpdateVersion);
                            notiIntent= new Intent(this, typeof(DownloadNewVersionActivity));
                            notiIntent.AddFlags(ActivityFlags.ClearTop);
                            var pendingIntent = PendingIntent.GetActivity(ApplicationContext, requestID, notiIntent, 0);//requestID = DateTime.Now.Millisecond;
                            var notificationBuilder = new NotificationCompat.Builder(this)
                                 .SetSmallIcon(BitopiApprovalSystem.Resource.Drawable.bitopiLogo)
                                 .SetContentTitle("Bitopi Approval System")
                                 .SetContentText("New Version Arrived")
                                 .SetAutoCancel(true)
                                 .SetContentIntent(pendingIntent)
                                 .SetDefaults(NotificationCompat.DefaultSound);
                            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);

                            //new UserRepository().GetNotificationCacheAsCompleted(gcmMsg.NotificationExecId);
                            notificationManager.Notify(requestID, notificationBuilder.Build());
                        }

                    }
                    catch (Exception ex)
                    {

                        //CustomLogger.CustomLog("From Activity: " + Line + "\nMessage: " + ex.Message + "\nStack Trace: " + ex.StackTrace + "\n\n", "", bitopiApplication.User != null ?
                        //   bitopiApplication.User.UserName : "");
                    }
                   
                };
                timer.Enabled = true;
            }).Start();
            return base.OnStartCommand(intent, flags, startId);
        }
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
        protected override void OnHandleIntent(Intent intent)
        {
            //throw new NotImplementedException();
        }
        void SendNotification(BitopiGcmMessage gcmMsg, string DeviceToken)
        {
            Dictionary<string, List<string>> ReceivingMessages;
            string s = "";
            BitopiApplication bitopiApplication = (BitopiApplication)this.ApplicationContext;
            try
            {

                s = Newtonsoft.Json.JsonConvert.SerializeObject(gcmMsg);
                if (PushNotificationSingleton.Instance.ReadNotification() == "")
                {
                    ReceivingMessages = new Dictionary<string, List<string>>();
                }
                else
                {
                    string notiMsg = PushNotificationSingleton.Instance.ReadNotification();
                    ReceivingMessages = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(notiMsg);
                }
                string title = "", msgBody = "";

                // TytMoblieApplication application = (TytMoblieApplication)this.ApplicationContext;
                //BitopiGcmMessage gcmMsg = JsonConvert.DeserializeObject<BitopiGcmMessage>(message);
                string approvalName = gcmMsg.Approval == "1" ? "PO" :
                      gcmMsg.Approval == "4" ? "Cash Requisition" : gcmMsg.Approval == "5" ? "Express Cash Requisition" : "";
                int requestID = Convert.ToInt16(gcmMsg.Approval);
                if (ReceivingMessages.Count > 0 && ReceivingMessages.ContainsKey(gcmMsg.Approval))
                {
                    ReceivingMessages[gcmMsg.Approval].Add(gcmMsg.POID);
                    msgBody = "You have " + ReceivingMessages[gcmMsg.Approval].Count() + " " + gcmMsg.ApprovalName + " to " + gcmMsg.ApprovalType;
                }
                else
                {
                    ReceivingMessages.Add(gcmMsg.Approval, new List<string> { gcmMsg.POID });
                    msgBody = "You have 1 " + gcmMsg.ApprovalName + " to " + gcmMsg.ApprovalType;
                }
                var totalNotification = 0;
                foreach (var msg in ReceivingMessages)
                {
                    totalNotification += msg.Value.Count;
                }


                try
                {
                    CrossBadge.Current.SetBadge(totalNotification);
                }
                catch { }

                //int result = repo.ReceiveNotification(bitopiApplication.User.UserCode,
                //    bitopiApplication.MacAddress,(ApprovalType)Convert.ToInt32(gcmMsg.Approval), gcmMsg.POID).Result;

                string approval = ((ApprovalType)Convert.ToInt32(gcmMsg.Approval) == ApprovalType.PurchaseOrderApproval) ? "PO" :
                   ((ApprovalType)Convert.ToInt32(gcmMsg.Approval) == ApprovalType.CashRequisition) ? "Cash Requisition" :
                   ((ApprovalType)Convert.ToInt32(gcmMsg.Approval) == ApprovalType.LeaveApplication) ? "Leave Application" :
                   ((ApprovalType)Convert.ToInt32(gcmMsg.Approval) == ApprovalType.ChequeRequisitionInformation) ? "ChequeRequisition" :
                   ((ApprovalType)Convert.ToInt32(gcmMsg.Approval) == ApprovalType.ExpressCashRequisition) ? "ExpressCashRequisition" :
                   ((ApprovalType)Convert.ToInt32(gcmMsg.Approval) == ApprovalType.PORequisitionApproval) ? "PO Requisition" :
                   ((ApprovalType)Convert.ToInt32(gcmMsg.Approval) == ApprovalType.UnSeenTask) ? "UnSeenTask" : "";
                ApprovalRepository repo = new ApprovalRepository();
                string url = RepositorySettings.BaseURl + "Notification?userId=" +
                     Cipher.Encrypt(bitopiApplication.User.UserCode) + "&deviceId=" +
                     DeviceToken
                     + "&approval=" + gcmMsg.ApprovalName + "&requisitioId="
                     + gcmMsg.POID;

                HttpClient client = new HttpClient();
                HttpResponseMessage result = client.GetAsync(url).Result;
                int count = JsonConvert.DeserializeObject<int>(result.Content.ReadAsStringAsync().Result);

                gcmMsg = new BitopiGcmMessage();
                //ISharedPreferences sp = PreferenceManager.GetDefaultSharedPreferences(this);
                //string employeeid = sp.GetString("employeeid_id", "");

                gcmMsg.DateCreated = DateTime.Now;
                Intent intent; intent = new Intent(this, typeof(StartupActivity));
                if ((ApprovalType)Convert.ToInt32(gcmMsg.Approval) == ApprovalType.UnSeenTask)
                {
                    intent = new Intent(this, typeof(MyTaskMenu));
                }
                intent.AddFlags(ActivityFlags.ClearTop); var pendingIntent = PendingIntent.GetActivity(ApplicationContext, requestID, intent, 0);//requestID = DateTime.Now.Millisecond;
                var notificationBuilder = new NotificationCompat.Builder(this)
                     .SetSmallIcon(BitopiApprovalSystem.Resource.Drawable.bitopiLogo)
                     .SetContentTitle("Bitopi Approval System")
                     .SetContentText(msgBody)
                     .SetAutoCancel(true)
                     .SetContentIntent(pendingIntent)
                     .SetDefaults(NotificationCompat.DefaultSound);

                string toSaveNotiMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ReceivingMessages);
                PushNotificationSingleton.Instance.SaveNotification(toSaveNotiMsg);
                var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);

                //new UserRepository().GetNotificationCacheAsCompleted(gcmMsg.NotificationExecId);
                notificationManager.Notify(requestID, notificationBuilder.Build());

            }
            catch (Exception ex)
            {

                //CustomLogger.CustomLog("Line:" + Line + " From Activity: " + bitopiApplication.CurrentActivity + "\nMessage: " + s + "\nStack Trace: " + ex.StackTrace + "\n\n", "", bitopiApplication.User != null ?
                //         bitopiApplication.User.UserName : "");
            }
        }
    }
}
public abstract class StickyIntentService : Service
{

    private volatile Looper mServiceLooper;
    private volatile ServiceHandler mServiceHandler;
    private String mName;
    private bool mRedelivery;

    private sealed class ServiceHandler : Handler
    {
        private StickyIntentService sis;
        public ServiceHandler(Looper looper, StickyIntentService sis) : base(looper)
        {
            this.sis = sis;
        }

        public override void HandleMessage(Message msg)
        {
            sis.OnHandleIntent((Intent)msg.Obj);
        }
    }

    /**
     * Creates an IntentService.  Invoked by your subclass's constructor.
     *
     * @param name Used to name the worker thread, important only for debugging.
     */
    public StickyIntentService(String name) : base()
    {
        mName = name;
    }

    /**
     * Sets intent redelivery preferences.  Usually called from the constructor
     * with your preferred semantics.
     *
     * <p>If enabled is true,
     * {@link #onStartCommand(Intent, int, int)} will return
     * {@link Service#START_REDELIVER_INTENT}, so if this process dies before
     * {@link #onHandleIntent(Intent)} returns, the process will be restarted
     * and the intent redelivered.  If multiple Intents have been sent, only
     * the most recent one is guaranteed to be redelivered.
     *
     * <p>If enabled is false (the default),
     * {@link #onStartCommand(Intent, int, int)} will return
     * {@link Service#START_NOT_STICKY}, and if the process dies, the Intent
     * dies along with it.
     */
    public void setIntentRedelivery(bool enabled)
    {
        mRedelivery = enabled;
    }

    public override void OnCreate()
    {
        // TODO: It would be nice to have an option to hold a partial wakelock
        // during processing, and to have a static startService(Context, Intent)
        // method that would launch the service & hand off a wakelock.

        base.OnCreate();
        HandlerThread thread = new HandlerThread("IntentService[" + mName + "]");
        thread.Start();

        mServiceLooper = thread.Looper;
        mServiceHandler = new ServiceHandler(mServiceLooper, this);
    }

    public override void OnStart(Intent intent, int startId)
    {
        Message msg = mServiceHandler.ObtainMessage();
        msg.Arg1 = startId;
        msg.Obj = intent;
        mServiceHandler.SendMessage(msg);
    }

    /**
     * You should not override this method for your IntentService. Instead,
     * override {@link #onHandleIntent}, which the system calls when the IntentService
     * receives a start request.
     * @see android.app.Service#onStartCommand
     */

    public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
    {
        OnStart(intent, startId);
        return StartCommandResult.Sticky;
    }

    public override void OnDestroy()
    {
        mServiceLooper.Quit();
    }

    /**
     * Unless you provide binding for your service, you don't need to implement this
     * method, because the default implementation returns null. 
     * @see android.app.Service#onBind
     */
    public override IBinder OnBind(Intent intent)
    {
        return null;
    }

    /**
     * This method is invoked on the worker thread with a request to process.
     * Only one Intent is processed at a time, but the processing happens on a
     * worker thread that runs independently from other application logic.
     * So, if this code takes a long time, it will hold up other requests to
     * the same IntentService, but it will not hold up anything else.
     * When all requests have been handled, the IntentService stops itself,
     * so you should not call {@link #stopSelf}.
     *
     * @param intent The value passed to {@link
     *               android.content.Context#startService(Intent)}.
     */
    protected abstract void OnHandleIntent(Intent intent);


}
