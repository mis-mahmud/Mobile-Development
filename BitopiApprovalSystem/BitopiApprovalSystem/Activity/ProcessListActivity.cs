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
using Android.Graphics;
using Android.Support.V4.View;
using Android.Graphics.Drawables;
using Android.Support.V7.App;
using Refractored.Controls;
using BitopiApprovalSystem.Model;
using Android.Support.V4.Widget;
using BitopiApprovalSystem.DAL;

namespace BitopiApprovalSystem
{
    [Activity(Label = "ProcessListActivity")]
    public class ProcessListActivity : BaseActivity
    {
        CircleImageView ivUser;
        // Button btnAll, btnRunning;
        Button btnNext;
        Spinner spProcess, spLocation, spPr,spEntryType;
        Switch swLoadLastLocation;
        bool LastLocation;
        
        DDL[] ProcessName;
        DDL[] LocationName;
        string SelectedProcess;
        string SelectedLocation;
        string SelectedEntryType;
        RelativeLayout RLleft_drawer;
        private DrawerLayout mDrawerLayout;
        string RecentProces, RecentLocation, RecentEntryType;
        public static string[] EntryTypeArray = { "Production","Quality","Rejection" };
        RecentHistory recentHistory;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            ISharedPreferences pref = Application.Context.GetSharedPreferences
                ("_bitopi_UserInfo", FileCreationMode.Private);
            LastLocation = pref.GetBoolean("IsLastLocation", false);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ProcessList);
            SupportActionBar.SetDisplayShowCustomEnabled(true);
            SupportActionBar.SetCustomView(Resource.Layout.custom_actionbar);
            InitializeControl();
            InitializeEvent();
            base.LoadDrawerView();
        }
        protected override void OnStart()
        {
            base.OnStart();
            recentHistory = DBAccess.Database.RecentHistory.Result;
            if(recentHistory!=null)
            {
                RecentProces = recentHistory.Process;
                RecentLocation = recentHistory.Location;
                RecentEntryType = recentHistory.EntryType;
            }
            else
            {
                recentHistory = new RecentHistory();
            }
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InSampleSize = 4;
            Bitmap bitmap = BitmapFactory.DecodeByteArray(bitopiApplication.User.EmpImage, 0,
                bitopiApplication.User.EmpImage.Length, options);
            ivUser.SetImageBitmap(bitmap);
            LoadCombo();
        }
        protected override  void InitializeControl()
        {
            RLleft_drawer = FindViewById<RelativeLayout>(Resource.Id.RLleft_drawer);
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            FindViewById<ImageButton>(Resource.Id.btnDrawermenu).Visibility = ViewStates.Visible;
            ivUser = FindViewById<CircleImageView>(Resource.Id.ivPAUserImg);
            //swLoadLastLocation = FindViewById<Switch>(Resource.Id.swLoadLastLocation);
            spProcess = FindViewById<Spinner>(Resource.Id.spProcess);
            spLocation = FindViewById<Spinner>(Resource.Id.spLocation);
            spEntryType = FindViewById<Spinner>(Resource.Id.spEntryType);
            //spPr = FindViewById<Spinner>(Resource.Id.spPr);
            // btnAll = FindViewById<Button>(Resource.Id.btnAll);
            //  btnRunning = FindViewById<Button>(Resource.Id.btnRunning);
            btnNext = FindViewById<Button>(Resource.Id.btnNext);
            FindViewById<TextView>(Resource.Id.tvHeaderName).Text = "Production Accounting";
            //swLoadLastLocation.Checked = LastLocation;
            base.InitializeControl();
        }
        protected override void InitializeEvent()
        {
            btnNext.Click += btnProcess_Click;
            //btnAll.Click += btnProcess_Click;
            //btnRunning.Click += btnProcess_Click;
            //swLoadLastLocation.CheckedChange += SwLoadLastLocation_CheckedChange;
            spProcess.ItemSelected += SpProcess_ItemSelected;
            spLocation.ItemSelected += SpLocation_ItemSelected;
            spEntryType.ItemSelected += SpEntryType_ItemSelected;
            //FindViewById<ImageButton>(Resource.Id.btnDrawermenu).Click += (s, e) =>
            //{
            //    if (mDrawerLayout.IsDrawerOpen(RLleft_drawer))
            //    {
            //        mDrawerLayout.CloseDrawer(RLleft_drawer);
            //    }
            //    else
            //    {
            //        mDrawerLayout.OpenDrawer(RLleft_drawer);
            //    }
            //};
            base.InitializeEvent();
        }

        private void SpEntryType_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            SelectedEntryType = EntryTypeArray[e.Position];
        }

        private void SpLocation_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            SelectedLocation = ProcessName[e.Position].LocationName;

        }

        private void SpProcess_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            //if(e.Position==0)
            //{
            //    spLocation.Adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, new string[] { "== Please Select a Process First ==" });
            //    return;
            //}

            SelectedProcess = ProcessName[e.Position].ProcessName;
            string[] locationArray = LocationName
                .Where(t => t.ProcessName == SelectedProcess).Select(t => t.LocationName).Distinct().ToArray();
            spLocation.Adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, locationArray);
            spLocation.SetSelection(Array.IndexOf(locationArray, RecentLocation));
        }
        private void btnProcess_Click(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(ProcessEntry));
            recentHistory.Process = SelectedProcess;
            recentHistory.Location = SelectedLocation;
            recentHistory.EntryType = SelectedEntryType;
            recentHistory.ProcessID = ProcessName.Where(t => t.ProcessName == SelectedProcess).FirstOrDefault().ProcessCode;
            recentHistory.LocationID = LocationName.Where(t => t.LocationName == SelectedLocation).FirstOrDefault().LocationRef;
            DBAccess.Database.SaveRecentHistory(recentHistory);
            bitopiApplication.PRStatus = ((Button)sender).Text;
            StartActivity(i);
        }
        void LoadCombo()
        {

            LocationName = bitopiApplication.DDLList.ToArray();


            ProcessName = bitopiApplication.DDLList.ToArray();
              
            spProcess.Adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, ProcessName.Select(t => t.ProcessName).Distinct().ToArray());

            
            spEntryType.Adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, EntryTypeArray); ;
            spLocation.Adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, LocationName.Select(t => t.LocationName).Distinct().ToArray());
            spProcess.SetSelection(Array.IndexOf(ProcessName.Select(t => t.ProcessName).Distinct().ToArray(), RecentProces));
            
            spEntryType.SetSelection(Array.IndexOf(EntryTypeArray, RecentEntryType));
            
        }
    }
}