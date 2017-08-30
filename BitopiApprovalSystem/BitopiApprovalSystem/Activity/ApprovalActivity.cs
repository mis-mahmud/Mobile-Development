using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ApiRepository;
using BitopiApprovalSystem.Model;
using Java.Lang;
using Android.Support.V7.App;
using Android.Graphics;
using BitopiApprovalSystem.PushNotification;
using System.Threading;
using Android.Text;
using BitopiApprovalSystem.BitopiPushNotification;
using Android.Content.Res;
using Android.Support.V4.Widget;

namespace BitopiApprovalSystem
{
    [Activity(Label = "ApprovalActivity")]
    public class ApprovalActivity : BaseActivity
    {
        TextView tvMsg;
        RelativeLayout rlApproval;
        RelativeLayout rlApprovalList;
        List<ApprovalModel> aprovalList;
       // BitopiApplication bitopiApplication;
        RelativeLayout RLleft_drawer;
        private DrawerLayout mDrawerLayout;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //BitopiSingelton.Instance.CurrentActivity = "Approval Activity";
            
            base.OnCreate(savedInstanceState);
            //bitopiApplication = (BitopiApplication)this.ApplicationContext;
            bitopiApplication.CurrentActivity = "Approval Activity";
            //bitopiApplication.ReceivingMessages= new Dictionary<string, List<string>>();
            //PushNotificationSingleton.Instance.SaveNotification("");
            //BitopiSingelton.Instance.ReceivingMessages = new Dictionary<string, List<string>>();
            SetContentView(Resource.Layout.ApprovalList);
            SupportActionBar.SetDisplayShowCustomEnabled(true);
            SupportActionBar.SetCustomView(Resource.Layout.custom_actionbar);

            rlApproval = FindViewById<RelativeLayout>(Resource.Id.rlApproval);
            rlApprovalList = FindViewById<RelativeLayout>(Resource.Id.rlApprovalList);
            tvMsg = FindViewById<TextView>(Resource.Id.tvMsg);


            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            FindViewById<ImageButton>(Resource.Id.btnDrawermenu).Visibility = ViewStates.Visible;
            RLleft_drawer = FindViewById<RelativeLayout>(Resource.Id.RLleft_drawer);
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
           // base.LoadDrawerView();

        }
        protected async override void OnStart()
        {
            var progressDialog = ProgressDialog.Show(this, null, "", true);
            //try
           // {
                base.OnStart();

                ApprovalRepository repo = new ApprovalRepository();
                aprovalList = await repo.GetApprovals(bitopiApplication.User.UserCode);
                RunOnUiThread(() =>
                {
                    LoadView(aprovalList, progressDialog);
                });
            ///}
            //catch (System.Exception ex)
            //{
            //    progressDialog.Dismiss();
            //    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            //}
        }
        public void LoadView(List<ApprovalModel> aprovalList, ProgressDialog progressDialog)
        {
            int id = 2;
            rlApprovalList.RemoveAllViews();
            foreach (var data in aprovalList)
            {
                if (data.Count > 0)
                {
                    var layout = LayoutInflater.Inflate(Resource.Layout.Approval_View, null, true);
                    var rlApprovalView = layout.FindViewById<RelativeLayout>(Resource.Id.rlApprovalView);

                    Button approvalButton = rlApprovalView.FindViewById<Button>(Resource.Id.tvMsg);
                    approvalButton.Text = data.ApprovalName;

                    rlApprovalView.FindViewById<TextView>(Resource.Id.numberImage).Text = data.Count.ToString();
                    RelativeLayout.LayoutParams params1 =
                        new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent,
                        RelativeLayout.LayoutParams.WrapContent);
                    approvalButton.Id = layout.Id = id;
                    approvalButton.Tag = layout.Tag = aprovalList.IndexOf(data);
                    approvalButton.Click += OnApprovalButtonClick;
                    params1.TopMargin = 30;
                    params1.LeftMargin = 20;
                    params1.RightMargin = 20;
                    params1.AddRule(LayoutRules.Below, (id - 1));
                    rlApprovalList.AddView(layout, params1);

                    id++;
                }
            }

            if (aprovalList.Count > 0 && aprovalList.Where(t => t.Count > 0).Count() > 0)
            {                
                tvMsg.Visibility = ViewStates.Gone;
                rlApprovalList.Visibility = ViewStates.Visible;                
                progressDialog.Dismiss();                
            }
            else
            {
                progressDialog.Dismiss();
                //lvApprovals.Visibility = ViewStates.Gone;
                rlApprovalList.Visibility = ViewStates.Gone;
                tvMsg.Visibility = ViewStates.Visible;
            }
        }
        public void OnApprovalButtonClick(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(ApprovalDetailActivity));
            Button btn = (Button)sender;
            int position = (int)btn.Tag;
            //BitopiSingelton.Instance.ApprovalType = aprovalList[position].Approval;
            //BitopiSingelton.Instance.ApprovalRoleType = aprovalList[position].RoleType;
            bitopiApplication.ApprovalType = aprovalList[position].Approval;
            bitopiApplication.ApprovalRoleType = aprovalList[position].RoleType;
            StartActivity(i);
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
                     "android", 2, bitopiApplication.CurrentVersion, bitopiApplication.User.UserCode).Result;

                    ISharedPreferences pref =
                    Application.Context.GetSharedPreferences("_bitopi_UserInfo", FileCreationMode.Private);
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
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.optionmenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.refresh:
                    var progressDialog = ProgressDialog.Show(this, null, "", true);
                    new System.Threading.Thread(new ThreadStart(() =>
                    {
                        ApprovalRepository repo = new ApprovalRepository();
                        //aprovalList = repo.GetApprovals(BitopiSingelton.Instance.User.UserCode).Result;
                        aprovalList = repo.GetApprovals(bitopiApplication.User.UserCode).Result;
                        RunOnUiThread(() =>
                        {
                            LoadView(aprovalList, progressDialog);
                        });
                    })).Start();
                    return true;

                case Resource.Id.about:
                    //BitopiSingelton.Instance.ShowAboutDialog(this);
                    bitopiApplication.ShowAboutDialog(this);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        public override void OnDetachedFromWindow()
        {
            StartService(new Intent(this, typeof(BitopiNotificationService)));
            base.OnDetachedFromWindow();
        }

    }
    public class ApprovalListAdapter : BaseAdapter
    {
        List<ApprovalModel> _approvalList;
        Context _context;
        public ApprovalListAdapter(List<ApprovalModel> approvalList, Context context)
        {
            _approvalList = approvalList;
            _context = context;
        }
        public override int Count
        {
            get
            {
                return _approvalList.Count;
            }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view;
            if (convertView == null)
                view = LayoutInflater.From(_context).Inflate(Resource.Layout.ApprovalListRow, parent, false);
            else
                view = convertView;
            ApprovalModel model = _approvalList[position];
            (view.FindViewById<TextView>(Resource.Id.tvApprovalName)).Text = model.ApprovalName;
            (view.FindViewById<TextView>(Resource.Id.tvApprovalCount)).Text = model.Count.ToString();

            return view;
        }
        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }
        public override long GetItemId(int position)
        {
            return 1;
        }

    }

}