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
using BitopiApprovalSystem;
using Android.Graphics;
using System.Threading;
using ApiRepository;
using Android.Support.V7.App;
using BitopiApprovalSystem.Model;
using Android.Support.V4.Widget;
using BitopiApprovalSystem.PushNotification;
using BitopiApprovalSystem.DAL;

namespace BitopiApprovalSystem
{
    [Activity(Label = "MyTaskMenu",WindowSoftInputMode =SoftInput.AdjustPan)]
    public class MyTaskMenu : BaseActivity
    {
        //BitopiApplication bitopiApplication;
        RelativeLayout RLleft_drawer;
        TextView numberSeen;
        TextView numberUnseen;
        TextView numberCompleted;
        private DrawerLayout mDrawerLayout;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //bitopiApplication = (BitopiApplication)this.ApplicationContext;
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MyTaskMenuLayout);
            SupportActionBar.SetDisplayShowCustomEnabled(true);
            SupportActionBar.SetCustomView(Resource.Layout.custom_actionbar);
            //LoadDrawerView();
            PushNotificationSingleton.Instance.SaveNotification("");
            FindViewById<RelativeLayout>(Resource.Id.rlSeenTask).Click += (s, e) =>
            {
                Intent i = new Intent(this, typeof(TNAMyTaskActivity));
                bitopiApplication.MyTaskType = MyTaskType.SEEN;
                StartActivity(i);
            };
            FindViewById<RelativeLayout>(Resource.Id.rlUnSeenTask).Click += (s, e) =>
            {
                Intent i = new Intent(this, typeof(TNAMyTaskActivity));
                bitopiApplication.MyTaskType = MyTaskType.UNSEEN;
                StartActivity(i);
            };
            FindViewById<RelativeLayout>(Resource.Id.rlCompleteTask).Click += (s, e) =>
            {
                Intent i = new Intent(this, typeof(TNAMyTaskActivity));
                bitopiApplication.MyTaskType = MyTaskType.COMPLETED;
                StartActivity(i);
            };
            RLleft_drawer = FindViewById<RelativeLayout>(Resource.Id.RLleft_drawer);
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            FindViewById<ImageButton>(Resource.Id.btnDrawermenu).Visibility = ViewStates.Visible;
            FindViewById<ImageButton>(Resource.Id.btnDrawermenu).Click += (s, e) =>
            {
                if (mDrawerLayout.IsDrawerOpen(RLleft_drawer))
                {
                    mDrawerLayout.CloseDrawer(RLleft_drawer);
                }
                else
                {
                    mDrawerLayout.OpenDrawer(RLleft_drawer);
                }
            };
           var tvHeaderName = FindViewById<TextView>(Resource.Id.tvHeaderName);
            tvHeaderName.Text = "My Task";
            numberSeen = FindViewById<TextView>(Resource.Id.numberImageSeen) ;
             numberUnseen= FindViewById<TextView>(Resource.Id.numberImageUnseen);
             numberCompleted= FindViewById<TextView>(Resource.Id.numberImageCompleted);
        }
        protected async override void OnStart()
        {
            base.OnStart();
            TNARepository repo = new TNARepository();

            MyTaskCountDBModel mytaskList = null;
            mytaskList = await repo.GetTaskCount(bitopiApplication.User.UserCode);
            if (mytaskList != null)
            {
                if (mytaskList.TotalUnSeenTask != "0")
                {
                    numberUnseen.Text = mytaskList.TotalUnSeenTask;
                    numberUnseen.Visibility = ViewStates.Visible;
                }
                else
                {
                    numberUnseen.Visibility = ViewStates.Gone;
                }
                if (mytaskList.TotalSeenTask != "0")
                {
                    numberSeen.Text = mytaskList.TotalSeenTask;
                    numberSeen.Visibility = ViewStates.Visible;
                }
                else
                {
                    numberSeen.Visibility = ViewStates.Gone;
                }
                if (mytaskList.TotalCompleteTask != "0")
                {
                    numberCompleted.Text = mytaskList.TotalCompleteTask;
                    numberCompleted.Visibility = ViewStates.Visible;
                }
                else
                {
                    numberCompleted.Visibility = ViewStates.Gone;
                }


            }

        }
        void LoadDrawerView()
        {
            //FindViewById<TextView>(Resource.Id.tvUserName).Text = BitopiSingelton.Instance.User.EmployeeName;
            FindViewById<TextView>(Resource.Id.tvUserName).Text = bitopiApplication.User.EmployeeName;
            if (bitopiApplication.User.EmpImage.Length > 0)
            {
                BitmapFactory.Options options = new BitmapFactory.Options();
                options.InSampleSize = 4;
                Bitmap bitmap = BitmapFactory.DecodeByteArray(bitopiApplication.User.EmpImage, 0,
                    bitopiApplication.User.EmpImage.Length, options);
                FindViewById<ImageView>(Resource.Id.ivUserImg).SetImageBitmap(bitmap);
            }
            FindViewById<Button>(Resource.Id.btnLogout).Click += (s, e) =>
            {
                var progressDialog = ProgressDialog.Show(this, null, "", true);
                new System.Threading.Thread(new ThreadStart(() =>
                {
                    AccountRepository repo = new AccountRepository();

                    var resutl = repo.getUser("", "",
                     bitopiApplication.MacAddress,
                     "",
                     "",
                     "android", 2, bitopiApplication.CurrentVersion,bitopiApplication.User.UserCode).Result;

                    ISharedPreferences pref =
                    Application.Context.GetSharedPreferences("_bitopi_UserInfo", FileCreationMode.Private);
                    pref.Edit().Clear().Commit();
                    bitopiApplication.ClearData();
                    DBAccess.Database.DropAllTable();
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
            FindViewById<RelativeLayout>(Resource.Id.rlmenuapproval).Click += (s, e) =>
            {
                Intent i = new Intent(this, typeof(ApprovalActivity));

                StartActivity(i);
            };
            FindViewById<RelativeLayout>(Resource.Id.rlmenumytask).Click += (s, e) =>
            {
                Intent i = new Intent(this, typeof(MyTaskMenu));

                StartActivity(i);
            };
        }
    }
}