using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using System;
using ApiRepository;
using Model;
using Android.Util;
using Android.Content;
using Android.Views;
using BitopiApprovalSystem.Model;
using System.Collections.Generic;

namespace pa
{
    [Activity(Label = "pa", Theme = "@style/NoActionBar")]
    public class LoginActivity : BaseActivity
    {
        TextInputEditText txtUserName;
        TextInputEditText txtPwd;
        Button btnLogin;
        AccountRepository Repo;
        RelativeLayout gifView;
        TextView tvMsg;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Repo = new AccountRepository();
            SetContentView(Resource.Layout.Main);
           
            base.OnCreate(savedInstanceState);
        }
        protected override void OnStart()
        {
            base.OnStart();
        }
        
        public override void InitializeControl()
        {
            tvMsg = FindViewById<TextView>(Resource.Id.tvMsg);
            tvMsg.Visibility = ViewStates.Gone;
            txtUserName = FindViewById<TextInputEditText>(Resource.Id.username_edittext);
            txtPwd = FindViewById<TextInputEditText>(Resource.Id.pwd_edittext);
            btnLogin = FindViewById<Button>(Resource.Id.btnLogin);
            gifView = FindViewById<RelativeLayout>(Resource.Id.gifview);
            gifView.Visibility = ViewStates.Gone;
        }

        public override void InitializeEvent()
        {
            btnLogin.Click += BtnLogin_Click;

            
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            gifView.Visibility = ViewStates.Visible;
            string encryptedUser = Cipher.Encrypt(txtUserName.Text);
            string encryptedPwd = Cipher.Encrypt(txtPwd.Text);

            ISharedPreferences prefToken = Application.Context.GetSharedPreferences("_bitopi_pa_DeviceToken", FileCreationMode.Private);
            string Token = prefToken.GetString("Token", String.Empty);
            if (String.IsNullOrEmpty(Token))
            {
                bitopiApplication.MacAddress = Guid.NewGuid().ToString();
                prefToken.Edit().PutString("Token", bitopiApplication.MacAddress).Commit();
            }
            else
                bitopiApplication.MacAddress = Token;

            UserModel user = await Repo.getUser(encryptedUser, encryptedPwd, bitopiApplication.MacAddress,
                bitopiApplication.MacAddress,
                bitopiApplication.DeviceName, "android", 1);
            gifView.Visibility = ViewStates.Gone;
            if (!String.IsNullOrEmpty(user.UserCode))
            {
				bitopiApplication.User = user;

                ISharedPreferences pref = Application.Context.GetSharedPreferences("_bitopi_pa_UserInfo", FileCreationMode.Private);
                List<DDL> ddl = await new ProductionRepository().GetProductionDDL(user.UserCode);
                bitopiApplication.DDLList = ddl;
                pref.Edit().PutString("UserName", encryptedUser).Commit();
                pref.Edit().PutString("Password", encryptedPwd).Commit();
                pref.Edit().PutString("UserCode", user.UserCode).Commit();

                Intent i = new Intent(this, typeof(ProcessListActivity));
                StartActivity(i);
            }
            else
            {
                
                tvMsg.Visibility = ViewStates.Visible;
                tvMsg.Text = "UserName or Passoword is incorrect";
            }
        }
    }
}

