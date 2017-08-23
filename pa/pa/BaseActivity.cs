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
using System.Threading;
using ApiRepository;

namespace pa
{
    [Activity(Label = "BaseActivity")]
    public class BaseActivity : AppCompatActivity
    {
        public static BitopiApplication bitopiApplication;
        public ImageButton btnLogout;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (bitopiApplication == null)
                bitopiApplication = new pa.BitopiApplication();
            base.OnCreate(savedInstanceState);
            InitializeControl();
            InitializeEvent();
            // Create your application here
        }
        protected override void OnStart()
        {
            ISharedPreferences prefToken = Application.Context.GetSharedPreferences("_bitopi_pa_DeviceToken", FileCreationMode.Private);
            string Token = prefToken.GetString("Token", String.Empty);
            if (String.IsNullOrEmpty(Token))
            {
                bitopiApplication.MacAddress = Guid.NewGuid().ToString();
                prefToken.Edit().PutString("Token", bitopiApplication.MacAddress).Commit();
            }
            else
                bitopiApplication.MacAddress = Token;
            base.OnStart();
        }
        public virtual void InitializeControl()
        {

            btnLogout = FindViewById<ImageButton>(Resource.Id.btnLogout);
        }
        public virtual void InitializeEvent()
        {
            if(btnLogout!=null)
            btnLogout.Click += (s, e) => {
                var progressDialog = ProgressDialog.Show(this, null, "Please wait...", true);
                new System.Threading.Thread(new ThreadStart(() =>
                {
                    AccountRepository repo = new AccountRepository();

                    var resutl = repo.getUser("", "",
                     bitopiApplication.MacAddress,
                     "",
                     "",
                     "android", 2, bitopiApplication.User.UserCode).Result;

                    ISharedPreferences pref =
                    Application.Context.GetSharedPreferences("_bitopi_pa_UserInfo", FileCreationMode.Private);
                    pref.Edit().Clear().Commit();
                    bitopiApplication.ClearData();
                    RunOnUiThread(() =>
                    {
                        progressDialog.Dismiss();
                        Intent i = new Intent(this, typeof(LoginActivity));
                        i.SetFlags(ActivityFlags.ClearTask);
                        StartActivity(i);
                        Finish();
                    });
                })).Start();
            };
        }
    }
}