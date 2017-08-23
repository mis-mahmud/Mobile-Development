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

using Android.Webkit;
using pa;
using pa.CustomControl;
using BitopiApprovalSystem.Model;

namespace BitopiApprovalSystem
{
    [Activity(MainLauncher = true, NoHistory = true, Theme = "@style/NoActionBar")]
    public class StartupActivity : BaseActivity
    {
        TextView tvMsg;
        RelativeLayout gifview;


        protected async override void OnCreate(Bundle savedInstanceState)
        {

            //BitopiSingelton.Instance.CurrentActivity = "Startup Activity";

           
            SetContentView(Resource.Layout.layout_startup);

            base.OnCreate(savedInstanceState);
            bitopiApplication.CurrentActivity = "Startup Activity";
            tvMsg = FindViewById<TextView>(Resource.Id.tvMsg);
            tvMsg.Visibility = ViewStates.Gone;
            gifview = FindViewById<RelativeLayout>(Resource.Id.gifview);
            gifview.Visibility = ViewStates.Gone;
            //WebView webView = (WebView)FindViewById(Resource.Id.wvLoader);
            //webView.SetInitialScale(1);
            //webView.Settings.JavaScriptEnabled = true;
            //webView.Settings.LoadWithOverviewMode = true;
            //webView.Settings.UseWideViewPort = true;
            //webView.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.SingleColumn);
            //webView.ScrollBarStyle = ScrollbarStyles.OutsideOverlay;
            //webView.ScrollbarFadingEnabled = false;
            //webView.LoadUrl("file:///android_asset/loading/loader.html");
            ////WifiManager wifiMan = (WifiManager)this.GetSystemService(Context.WifiService);
            ////WifiInfo wifiInf = wifiMan.ConnectionInfo;
            bitopiApplication.CurrentVersion = this.PackageManager.GetPackageInfo(this.PackageName,
                Android.Content.PM.PackageInfoFlags.MetaData).VersionName;
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
                    //CustomLogger.VersionName = bitopiApplication.CurrentVersion;
                    //CustomLogger.CustomLog("From Activity: " + bitopiApplication.CurrentActivity + "\nMessage: " + ex.Message + "\nStack Trace: " + ex.StackTrace + "\n\n", "", bitopiApplication.User != null ?
                    //     bitopiApplication.User.UserName : "");
                }
            };
            
        }
        protected async override void OnStart()
        {
            base.OnStart();
            try
            {
              
                ISharedPreferences pref = Application.Context.GetSharedPreferences("_bitopi_pa_UserInfo", FileCreationMode.Private);
                ISharedPreferences prefToken = Application.Context.GetSharedPreferences("_bitopi_pa_DeviceToken", FileCreationMode.Private);
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
                    gifview.Visibility = ViewStates.Visible;
                     UserModel user = await repo.getUser(UserName, Password, bitopiApplication.MacAddress,
                    bitopiApplication.MacAddress,
                   bitopiApplication.DeviceName,
                    "android", 1);
                    
                    if (!String.IsNullOrEmpty(user.UserCode))
                    {
                        List<DDL> ddl = await new ProductionRepository().GetProductionDDL(user.UserCode);
                        bitopiApplication.DDLList = ddl;
                        bitopiApplication.User = user;

                        pref.Edit().PutString("UserCode", user.UserCode).Commit();

                        i = new Intent(this, typeof(ProcessListActivity));
                    }
                    else
                    {
                        i = new Intent(this, typeof(LoginActivity));
                    }
                }
                gifview.Visibility = ViewStates.Gone;
                StartActivity(i);
            }
            catch (Exception ex)
            {
               
                tvMsg.Visibility = ViewStates.Visible;
                tvMsg.Text = "The application has encountered an unknown error.";
                
            }
        }
    }
}