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
using Android.Graphics;
using ApiRepository;

namespace BitopiApprovalSystem
{
    [Activity(Label = "BaseActivity")]
    public class BaseActivity : AppCompatActivity
    {
        protected BitopiApplication bitopiApplication;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            bitopiApplication = (BitopiApplication)this.ApplicationContext;
            // Create your application here
        }
        protected void LoadDrawerView(Activity Context)
        {
            Context.FindViewById<TextView>(Resource.Id.tvUserName).Text = bitopiApplication.User.EmployeeName;
            if (bitopiApplication.User.EmpImage.Length > 0)
            {
                BitmapFactory.Options options = new BitmapFactory.Options();
                options.InSampleSize = 4;
                Bitmap bitmap = BitmapFactory.DecodeByteArray(bitopiApplication.User.EmpImage, 0,
                    bitopiApplication.User.EmpImage.Length, options);
                FindViewById<ImageView>(Resource.Id.ivUserImg).SetImageBitmap(bitmap);
            }
            Context.FindViewById<Button>(Resource.Id.btnLogout).Click += (s, e) =>
            {


                var progressDialog = ProgressDialog.Show(Context, null, "", true);
                new System.Threading.Thread(new System.Threading.ThreadStart(() =>
                {
                    AccountRepository repo = new AccountRepository();
                    var resutl = repo.getUser("", "",
                     bitopiApplication.MacAddress,
                     "",
                     "",
                     "android", 2, bitopiApplication.User.UserCode).Result;
                    ISharedPreferences pref = Application.Context.GetSharedPreferences("_bitopi_UserInfo", FileCreationMode.Private);
                    pref.Edit().Clear().Commit();
                    bitopiApplication.ClearData();
                    RunOnUiThread(() =>
                    {
                        progressDialog.Dismiss();
                        Intent i = new Intent(Context, typeof(LoginActivity));
                        i.SetFlags(ActivityFlags.ClearTask);
                        StartActivity(i);
                        Finish();
                    });
                })).Start();
            };
            Context.FindViewById<RelativeLayout>(Resource.Id.rlmenuapproval).Click += (s, e) =>
            {
                Intent i = new Intent(Context, typeof(ApprovalActivity));

                StartActivity(i);
            };
            Context.FindViewById<RelativeLayout>(Resource.Id.rlmenumytask).Click += (s, e) =>
            {
                Intent i = new Intent(Context, typeof(MyTaskMenu));

                StartActivity(i);
            };
        }
    }
}