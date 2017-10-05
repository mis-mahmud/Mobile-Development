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
using Android.Support.V7.App;
using ApiRepository;
using System.Threading.Tasks;
using Model;
using BitopiApprovalSystem.BitopiIndicator;
using Android.Graphics.Drawables;
using BitopiApprovalSystem.Model;
using BitopiApprovalSystem.PushNotification;


namespace BitopiApprovalSystem
{
    [Activity(Label = "LoginActivity", Theme = "@style/MyTheme", NoHistory = true)]
    public class LoginActivity : AppCompatActivity
    {
        EditText etUserName, etPwd;
        TextView tvMsg;
        Button btnLogin;
        AVLoadingIndicatorView indRotator;
        BitopiApplication bitopiApplication;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //BitopiSingelton.Instance.CurrentActivity = "Login Activity";
            base.OnCreate(savedInstanceState);
            bitopiApplication = (BitopiApplication)this.ApplicationContext;
            bitopiApplication.CurrentActivity = "Login Activity";
            SetContentView(Resource.Layout.layout_login);
            SupportActionBar.SetDisplayShowCustomEnabled(true);
            SupportActionBar.SetCustomView(Resource.Layout.custom_actionbar);
            etUserName = FindViewById<EditText>(Resource.Id.etUserName);
            etPwd = FindViewById<EditText>(Resource.Id.etPassword);
            btnLogin = FindViewById<Button>(Resource.Id.ButLogInToApp);
            tvMsg = FindViewById<TextView>(Resource.Id.tvMsg);
            tvMsg.Visibility = ViewStates.Gone;
            FindViewById<ImageButton>(Resource.Id.btnDrawermenu).Visibility = ViewStates.Gone;
            //indRotator = FindViewById<AVLoadingIndicatorView>(Resource.Id.indLoader);
            FindViewById<TextView>(Resource.Id.tvPropiretory).Text = "© "+DateTime.Now.Year+" Bitopi Group. All Rights Reserved";    
        }
        protected override void OnStart()
        {
            base.OnStart();
            
            btnLogin.Click += onLoginClick;
            etPwd.TextChanged += (s, e) =>
            {
                etPwd.SetBackgroundResource(Resource.Drawable.rounded_textview);
            };
            etUserName.TextChanged += (s, e) =>
            {
                etUserName.SetBackgroundResource(Resource.Drawable.rounded_textview);
            };
        }
        public override void OnBackPressed()
        {
            //Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            this.FinishAffinity();
        }
        public async void onLoginClick(object sender, EventArgs er)
        {
            if(!bitopiApplication.IsInternetConnectionAvailable(this))
            {
                tvMsg.Visibility = ViewStates.Visible;
                tvMsg.Text = "No Internet Connection Available.";
                return;
            }
            var progressDialog = ProgressDialog.Show(this, null, "Please Wait.", true);
            AccountRepository repo = new AccountRepository();

        

            string encryptedUser = Cipher.Encrypt(etUserName.Text);
            string encryptedPwd = Cipher.Encrypt(etPwd.Text);
            string s=PushNotificationSingleton.Instance.TokenId;
            //UserModel user= await repo.getUser(encryptedUser, encryptedPwd,
            //    BitopiSingelton.Instance.MacAddress,
            //    PushNotificationSingleton.Instance.TokenId,
            //    BitopiSingelton.Instance.DeviceName,
            //    "android",1);
            ISharedPreferences prefToken = Application.Context.GetSharedPreferences("_bitopi_DeviceToken", FileCreationMode.Private);
            string Token = prefToken.GetString("Token", String.Empty);
            if (String.IsNullOrEmpty(Token))
            {
                bitopiApplication.MacAddress = Guid.NewGuid().ToString();
                prefToken.Edit().PutString("Token", bitopiApplication.MacAddress).Commit();
            }
            else
                bitopiApplication.MacAddress = Token;
            UserModel user = await repo.getUser(encryptedUser, encryptedPwd,
                bitopiApplication.MacAddress,
                bitopiApplication.MacAddress,
                bitopiApplication.DeviceName,
                "android", 1,bitopiApplication.CurrentVersion);
            user.PermittedApproval = new List<int> { 1, 3, 4 };
            if (user.VersionCode > bitopiApplication.CurrentVersion)
            {
                BitopiSingelton.Instance.ShowNewVersionDialog(this);
                return;
            }
            if (String.IsNullOrEmpty(user.UserCode))
            {
                progressDialog.Dismiss();
                etUserName.SetBackgroundResource(Resource.Drawable.rounded_textview_error);
                etPwd.SetBackgroundResource(Resource.Drawable.rounded_textview_error);
                tvMsg.Visibility = ViewStates.Visible;
                tvMsg.Text = "UserName or Passoword is incorrect";
            }
            else
            {
                progressDialog.Dismiss();
                tvMsg.Visibility = ViewStates.Gone;
                //BitopiSingelton.Instance.User = user;
                bitopiApplication.User = user;
                List<DDL> ddl = await new ProductionRepository().GetProductionDDL(user.UserCode);
                
                bitopiApplication.DDLList = ddl;
                ISharedPreferences pref = Application.Context.GetSharedPreferences ("_bitopi_UserInfo", FileCreationMode.Private);
                
                pref.Edit().PutString("UserName", encryptedUser).Commit();
                pref.Edit().PutString("Password", encryptedPwd).Commit();
                pref.Edit().PutString("UserCode", user.UserCode).Commit();
                

                Intent i = new Intent(this, typeof(BitopiActivity));
                StartActivity(i);
            }
        }
    }
}