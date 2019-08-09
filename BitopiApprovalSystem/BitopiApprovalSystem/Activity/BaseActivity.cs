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
using Android.Support.V4.Widget;
using BitopiApprovalSystem.DAL;
using Android.Views.Animations;
using System.Threading;
using Android.Animation;
using System.Threading.Tasks;
using BitopiApprovalSystem.Model;

namespace BitopiApprovalSystem
{
    [Activity(Label = "BaseActivity")]
    public class BaseActivity : AppCompatActivity
    {
        protected BitopiApplication bitopiApplication;
        RelativeLayout RLleft_drawer;
        private DrawerLayout mDrawerLayout;
        bool isVersonOpen = false;
        RelativeLayout rlLoaderContainer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            bitopiApplication = (BitopiApplication)this.ApplicationContext;

            

        }
        protected override void OnStart()
        {

            //InitializeControl();
            if (FindViewById<RelativeLayout>(Resource.Id.rlLoading) == null)
                InitializeLoader();
            LoadDrawerView();
            //InitializeEvent();
            new Thread(new ThreadStart(() =>
            {
                if (new AccountRepository().GetVersion().Result > bitopiApplication.CurrentVersion)
                {
                    RunOnUiThread(() =>
                    {
                        LoadVersion();
                    });
                }
            })).Start();
            base.OnStart();
        }
        protected void InitializeLoader()
        {
            rlLoaderContainer = FindViewById<RelativeLayout>(Resource.Id.rlMain);
            if (rlLoaderContainer != null)
            {
                var layout = LayoutInflater.Inflate(Resource.Layout.layout_loader, null, true);
                rlLoaderContainer.AddView(layout);
            }
        }
        protected void LoadDrawerView()
        {
            Activity Context = this;
            var DrawerMenuParent = FindViewById<RelativeLayout>(Resource.Id.RLleft_drawer);
            if (DrawerMenuParent == null) return;
            DrawerMenuParent.RemoveAllViews();
            var layout = LayoutInflater.Inflate(Resource.Layout.DrawerMenu, null, true);
            DrawerMenuParent.AddView(layout);

            Context.FindViewById<TextView>(Resource.Id.tvUserName).Text = bitopiApplication.User.EmployeeName;
            if (bitopiApplication.User.EmpImage.Length > 0)
            {
                BitmapFactory.Options options = new BitmapFactory.Options();
                options.InSampleSize = 4;
                Bitmap bitmap = BitmapFactory.DecodeByteArray(bitopiApplication.User.EmpImage, 0,
                    bitopiApplication.User.EmpImage.Length, options);
                FindViewById<ImageView>(Resource.Id.ivUserImg).SetImageBitmap(bitmap);
            }
            Context.FindViewById<ImageButton>(Resource.Id.imgDownload).Click += (s, e) =>
            {
                FindViewById<DrawerLayout>(Resource.Id.drawer_layout).CloseDrawer(DrawerMenuParent);
                BitopiSingelton.Instance.DownloadFile(this);
            };
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
                     "android", 2, bitopiApplication.CurrentVersion, bitopiApplication.User.UserCode).Result;
                    ISharedPreferences pref = Application.Context.GetSharedPreferences("_bitopi_UserInfo", FileCreationMode.Private);
                    pref.Edit().Clear().Commit();
                    bitopiApplication.ClearData();
                    ProcessSingleton.Instance.RemoveProcess();
                    DBAccess.Database.DropAllTable();
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
                FindViewById<DrawerLayout>(Resource.Id.drawer_layout).CloseDrawer(DrawerMenuParent);
                Intent i = new Intent(Context, typeof(ApprovalActivity));

                StartActivity(i);
            };
            Context.FindViewById<RelativeLayout>(Resource.Id.rlmenumytask).Click += (s, e) =>
            {
                FindViewById<DrawerLayout>(Resource.Id.drawer_layout).CloseDrawer(DrawerMenuParent);
                Intent i = new Intent(Context, typeof(MyTaskMenu));

                StartActivity(i);
            };
            Context.FindViewById<RelativeLayout>(Resource.Id.rlmenuPA).Click += async (s, e) =>
            {
                FindViewById<DrawerLayout>(Resource.Id.drawer_layout).CloseDrawer(DrawerMenuParent);
                ProductionRepository repo = new ProductionRepository(ShowLoader, HideLoader);
                List<DDL> ddl = await repo.GetProductionDDL(bitopiApplication.User.UserCode);
                if (ddl != null && ddl.Count > 0)
                {
                    bitopiApplication.DDLList = ddl;

                    Intent i = new Intent(this, typeof(ProcessListActivity));
                    StartActivity(i);
                }
                else
                {
                    Toast.MakeText(this, "You Do not have any Production Location Attached!!!!", ToastLength.Long).Show();
                }
            };
        }
        protected virtual void InitializeControl()
        {
            RLleft_drawer = FindViewById<RelativeLayout>(Resource.Id.RLleft_drawer);
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
        }
        protected virtual void InitializeEvent()
        {

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
        }
        protected void LoadVersion()
        {
            var rlMain = FindViewById<RelativeLayout>(Resource.Id.rlVersionMain);
            if (rlMain != null)
            {
                RelativeLayout rlVersionContainer = FindViewById<RelativeLayout>(Resource.Id.rlVersionContainer);
                rlMain.RemoveAllViews();
                var layout = LayoutInflater.Inflate(Resource.Layout.layout_version, null, true);
                rlMain.AddView(layout);
                Animation aniRight = Android.Views.Animations.AnimationUtils.LoadAnimation(this,
           Resource.Animation.anim_right);
                rlVersionContainer = FindViewById<RelativeLayout>(Resource.Id.rlVersionContainer);
                var rlVersion = FindViewById<RelativeLayout>(Resource.Id.rlVersion);
                var btnDownLoad = FindViewById<Button>(Resource.Id.btnDownLoad);
                btnDownLoad.Click += BtnDownLoad_Click;
                var btnCancel = FindViewById<Button>(Resource.Id.btnCancel);
                btnCancel.Click += BtnCancel_Click;
                rlVersionContainer.Click += BtnCancel_Click;
                rlVersionContainer.Visibility = (ViewStates.Visible);
                rlVersion.StartAnimation(aniRight);
                isVersonOpen = true;
            }
        }
        protected void ShowLoader(long? totalDownloadSize, long totalBytesRead, double? progressPercentage)
        {

            //rlLoading = FindViewById<RelativeLayout>(Resource.Id.rlLoading);


            //rlLoading.Visibility = ViewStates.Visible;

            //var width = WindowManager.DefaultDisplay.Width;
            ////vwLoading.Post(() =>
            ////{
            ////    var param = vwLoading.LayoutParameters;
            //    var nowWidth = width * progressPercentage / 100;
            ////    param.Width = (int)nowWidth;
            ////    vwLoading.LayoutParameters = param;
            ////    vwLoading.RequestLayout();
            //ValueAnimator anim = ValueAnimator.OfInt((int)nowWidth);
            //anim.AddUpdateListener(new ValueAnimatorUpdateListener(vwLoading));
            //anim.SetDuration(500);
            //anim.Start();

            //});
            if (rlLoaderContainer != null)
            {

                //rlLoaderContainer.Visibility = ViewStates.Visible;
                FindViewById<RelativeLayout>(Resource.Id.rlLoading).Visibility = ViewStates.Visible;
                ProgressBar vwLoading = FindViewById<ProgressBar>(Resource.Id.vwLoading);
                //vwLoading.pro=(ProgressDialogStyle.Horizontal);
                //vwLoading.Progress = 0;
                vwLoading.Max = (int)totalDownloadSize;
                vwLoading.Progress = (int)totalBytesRead;
            }
        }
        public void HideLoader()
        {
            if (rlLoaderContainer != null)
            {
                Task.Delay(500).ContinueWith(t =>
            {

                if (rlLoaderContainer != null)
                {
                    //rlLoaderContainer.Visibility = ViewStates.Gone;
                    FindViewById<RelativeLayout>(Resource.Id.rlLoading).Visibility = ViewStates.Gone;
                }
            });
            }
        }
        public class ValueAnimatorUpdateListener : Java.Lang.Object, ValueAnimator.IAnimatorUpdateListener
        {
            View viewToIncreaseHeight;
            public ValueAnimatorUpdateListener(View view)
            {
                viewToIncreaseHeight = view;
            }
            public void OnAnimationUpdate(ValueAnimator valueAnimator)
            {
                int val = (int)valueAnimator.AnimatedValue;
                ViewGroup.LayoutParams layoutParams = viewToIncreaseHeight.LayoutParameters;
                layoutParams.Width = val;
                viewToIncreaseHeight.LayoutParameters = (layoutParams);
            }
        }
        public override void OnBackPressed()
        {
            if (isVersonOpen)
                hideVersion();
            else
                base.OnBackPressed();
        }
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            hideVersion();
        }
        void hideVersion()
        {
            Animation aniRight = Android.Views.Animations.AnimationUtils.LoadAnimation(this,
          Resource.Animation.anim_left);
            var rlVersionContainer = FindViewById<RelativeLayout>(Resource.Id.rlVersionContainer);
            var rlVersion = FindViewById<RelativeLayout>(Resource.Id.rlVersion);
            var btnDownLoad = FindViewById<Button>(Resource.Id.btnDownLoad);
            btnDownLoad.Click += BtnDownLoad_Click;
            var btnCancel = FindViewById<Button>(Resource.Id.btnCancel);
            btnCancel.Click += BtnCancel_Click;

            rlVersion.StartAnimation(aniRight);
            aniRight.AnimationEnd += (s, er) =>
            {
                rlVersionContainer.Visibility = (ViewStates.Gone);
                isVersonOpen = false;
            };
        }

        private void BtnDownLoad_Click(object sender, EventArgs e)
        {
            BitopiSingelton.Instance.DownloadFile(this);
        }
    }
}

