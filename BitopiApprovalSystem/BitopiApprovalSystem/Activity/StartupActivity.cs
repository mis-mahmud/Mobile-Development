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
using Android.Preferences;
using Android.Support.V7.App;
using ApiRepository;
using Model;
using System.Reflection;
using Android.Net.Wifi;
using BitopiApprovalSystem.PushNotification;
using BitopiApprovalSystem.BitopiPushNotification;
using Android.Webkit;
using Badge.Plugin;
using BitopiApprovalSystem.Library;
using BitopiApprovalSystem.Model;

namespace BitopiApprovalSystem
{
    [Activity(MainLauncher = true, NoHistory = true)]
    public class StartupActivity : AppCompatActivity
    {
        TextView tvMsg;
        BitopiApplication bitopiApplication;
        protected  override void OnCreate(Bundle savedInstanceState)
        {
            bitopiApplication = (BitopiApplication)this.ApplicationContext;
            //BitopiSingelton.Instance.CurrentActivity = "Startup Activity";
            bitopiApplication.CurrentActivity = "Startup Activity";
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layout_startup);
            tvMsg = FindViewById<TextView>(Resource.Id.tvMsg);
            tvMsg.Visibility = ViewStates.Gone;
            WebView webView = (WebView)FindViewById(Resource.Id.wvLoader);
            webView.SetInitialScale(1);
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.LoadWithOverviewMode = true;
            webView.Settings.UseWideViewPort = true;
            webView.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.SingleColumn);
            webView.ScrollBarStyle = ScrollbarStyles.OutsideOverlay;
            webView.ScrollbarFadingEnabled = false;
            webView.LoadUrl("file:///android_asset/loading/loader.html");
            WifiManager wifiMan = (WifiManager)this.GetSystemService(Context.WifiService);
            WifiInfo wifiInf = wifiMan.ConnectionInfo;
            //BitopiSingelton.Instance.MacAddress= UID.id(this);
            //bitopiApplication.MacAddress = UID.id(this);
            PushNotificationSingleton.Instance.SaveNotification("");
            //BitopiSingelton.Instance.CurrentVersion = this.PackageManager.GetPackageInfo(this.PackageName,
            //    Android.Content.PM.PackageInfoFlags.MetaData).VersionName;
            bitopiApplication.CurrentVersion = this.PackageManager.GetPackageInfo(this.PackageName,
                Android.Content.PM.PackageInfoFlags.MetaData).VersionCode;
            //StartService(new Intent(this, typeof(BitopiRegistrationIntentService)));
            //StartService(new Intent(this, typeof(BitopiInstanceIDListenerService)));
            //StartService(new Intent(this, typeof(BitopiGcmListenerService)));
            //StartService(new Intent(this, typeof(BitopiNotification)));
            StartService(new Intent(this, typeof(BitopiNotificationService)));
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                try
                {
                    typeof(System.Exception).GetField("stack_trace", BindingFlags.NonPublic | BindingFlags.Instance)
                        .SetValue(e.ExceptionObject, null);

                  
                    throw  new Exception("CurrentDomainOnUnhandledException", e.ExceptionObject as Exception);
                }
                catch (Exception ex)
                {
                    string str = ex.Message;
                    CustomLogger.VersionName = bitopiApplication.CurrentVersion;
                    CustomLogger.CustomLog("From Activity: " + bitopiApplication.CurrentActivity + "\nMessage: " + ex.Message + "\nStack Trace: " + ex.StackTrace + "\n\n", "", bitopiApplication.User != null ?
                         bitopiApplication.User.UserName : "");
                }
            };
            AndroidEnvironment.UnhandledExceptionRaiser += delegate (object sender, RaiseThrowableEventArgs args)
            {
                try
                {
                    typeof(System.Exception).GetField("stack_trace", BindingFlags.NonPublic | BindingFlags.Instance)
                        .SetValue(args.Exception, null);
                    args.Handled = true;

                    throw args.Exception;
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                    CustomLogger.VersionName = bitopiApplication.CurrentVersion;
                    CustomLogger.CustomLog("From Activity: " + bitopiApplication.CurrentActivity + "\nMessage: " + ex.Message + "\nStack Trace: " + ex.StackTrace + "\n\n", "", bitopiApplication.User != null ?
                         bitopiApplication.User.UserName : "");
                }
            };

        }
        protected async override void OnStart()
        {
            base.OnStart();
            try
            {
                try
                {
                    CrossBadge.Current.ClearBadge();
                }
                catch { }
                ISharedPreferences pref = Application.Context.GetSharedPreferences("_bitopi_UserInfo", FileCreationMode.Private);
                ISharedPreferences prefToken = Application.Context.GetSharedPreferences("_bitopi_DeviceToken", FileCreationMode.Private);
                string UserName = pref.GetString("UserName", String.Empty);
                string Password = pref.GetString("Password", String.Empty);
                string Token = prefToken.GetString("Token", String.Empty);
                AccountRepository repo = new AccountRepository();

                Intent i = new Intent(this, typeof(LoginActivity));


                if (String.IsNullOrEmpty(UserName))
                {
                    i = new Intent(this, typeof(LoginActivity));

                }
                else
                {

                    if (String.IsNullOrEmpty(Token))
                    {
                        bitopiApplication.MacAddress = Guid.NewGuid().ToString();
                        prefToken.Edit().PutString("Token", bitopiApplication.MacAddress).Commit();
                    }
                    else
                        bitopiApplication.MacAddress = Token;
                    UserModel user = await repo.getUser(UserName, Password, bitopiApplication.MacAddress,
                    bitopiApplication.MacAddress,
                   bitopiApplication.DeviceName,
                    "android", 1,bitopiApplication.CurrentVersion);
                    if(user.VersionCode>bitopiApplication.CurrentVersion)
                    {
                        BitopiSingelton.Instance.ShowNewVersionDialog(this);
                        return;
                    }
                    else if (!String.IsNullOrEmpty(user.UserCode))
                    {
                        bitopiApplication.User = user;
                        List<DDL> ddl = await new ProductionRepository().GetProductionDDL(user.UserCode);
                        bitopiApplication.DDLList = ddl;
                        pref.Edit().PutString("UserCode", user.UserCode).Commit();

                        i = new Intent(this, typeof(BitopiActivity));
                    }
                    else
                    {
                        i = new Intent(this, typeof(LoginActivity));
                    }
                }
                StartActivity(i);
            }
            catch (Exception ex)
            {
                tvMsg.Visibility = ViewStates.Visible;
                tvMsg.Text = "The application has encountered an unknown error.";
                CustomLogger.VersionName = bitopiApplication.CurrentVersion;
                CustomLogger.CustomLog("From Activity: " + BitopiSingelton.Instance.CurrentActivity + "\nMessage: " + ex.Message + "\nStack Trace: " + ex.StackTrace + "\n\n", "", BitopiSingelton.Instance.User != null ?
                             BitopiSingelton.Instance.User.UserName : "");
            }
        }
    }
}