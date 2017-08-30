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

namespace BitopiApprovalSystem
{
    [Activity(Label = "ProcessListActivity")]
    public class ProcessListActivity : BaseActivity
    {
        CircleImageView ivUser;
        Button btnAll, btnRunning;
        Spinner spProcess, spLocation, spPr;
        Switch swLoadLastLocation;
        bool LastLocation;
        
        DDL[] ProcessName;
        DDL[] LocationName;
        string SelectedProcess;
        string SelectedLocation;
        RelativeLayout RLleft_drawer;
        private DrawerLayout mDrawerLayout;
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
            swLoadLastLocation = FindViewById<Switch>(Resource.Id.swLoadLastLocation);
            spProcess = FindViewById<Spinner>(Resource.Id.spProcess);
            spLocation = FindViewById<Spinner>(Resource.Id.spLocation);
            //spPr = FindViewById<Spinner>(Resource.Id.spPr);
            btnAll = FindViewById<Button>(Resource.Id.btnAll);
            btnRunning = FindViewById<Button>(Resource.Id.btnRunning);
            FindViewById<TextView>(Resource.Id.tvHeaderName).Text = "Production Accounting";
            swLoadLastLocation.Checked = LastLocation;
            base.InitializeControl();
        }
        protected override void InitializeEvent()
        {
            btnAll.Click += btnProcess_Click;
            btnRunning.Click += btnProcess_Click;
            swLoadLastLocation.CheckedChange += SwLoadLastLocation_CheckedChange;
            spProcess.ItemSelected += SpProcess_ItemSelected;
            spLocation.ItemSelected += SpLocation_ItemSelected;
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
            spLocation.Adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, LocationName
                .Where(t => t.ProcessName == SelectedProcess).Select(t => t.LocationName).Distinct().ToArray());
        }
        private void SwLoadLastLocation_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            ISharedPreferences pref= Application.Context.GetSharedPreferences
                ("_bitopi_UserInfo", FileCreationMode.Private); ;
            if (e.IsChecked)
            {
                LastLocation = true;
                pref.Edit().PutBoolean("IsLastLocation", true).Commit();
            }
            else
            {
                LastLocation = false;
                pref.Edit().PutBoolean("IsLastLocation", false).Commit();
            }
            if (ProcessSingleton.Instance.LocationList[0] != null)
                LoadCombo();
            else
            {
                Toast.MakeText(this, "You don't have any Previous Location", ToastLength.Long).Show();
                swLoadLastLocation.Checked = false;
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(ProcessEntry));
            DDL location = new DDL { };
            location.ProcessName = bitopiApplication.ProcessName = SelectedProcess;
            location.LocationName = bitopiApplication.LocationName = SelectedLocation;
            location.ProcessCode = bitopiApplication.ProcessID = ProcessName.Where(t => t.ProcessName == SelectedProcess).First().ProcessCode;
            location.LocationRef = bitopiApplication.LocationID = LocationName.Where(t => t.LocationName == SelectedLocation).First().LocationRef;
            bitopiApplication.PRStatus = ((Button)sender).Text;
            ProcessSingleton.Instance.Location = location;
            StartActivity(i);
        }
        void LoadCombo()
        {
            ISharedPreferences pref = Application.Context.GetSharedPreferences
                ("_bitopi_UserInfo", FileCreationMode.Private); ;
            if (ProcessSingleton.Instance.LocationList[2] == null)
            {
                LastLocation = false;
                swLoadLastLocation.Checked = false;
                pref.Edit().PutBoolean("IsLastLocation", false).Commit();
            }
            DDL[] ddl = ProcessSingleton.Instance.LocationList;
            LocationName = !LastLocation ? bitopiApplication.DDLList.ToArray()
                : ddl;
            ProcessName = !LastLocation ? bitopiApplication.DDLList.ToArray()
                : ddl;
            spProcess.Adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, ProcessName.Select(t => t.ProcessName).Distinct().ToArray());
            if (LastLocation)
            {
                spLocation.Adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, LocationName.Select(t => t.LocationName).Distinct().ToArray());
                spProcess.SetSelection(Array.IndexOf(ProcessName.Select(t => t.ProcessName).Distinct().ToArray(), ddl[2].ProcessName));
                spLocation.SetSelection(Array.IndexOf(LocationName.Select(t => t.LocationName).Distinct().ToArray(), ddl[2].LocationName));
            }
            else
            {
                //spLocation.Adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, new string[] { "== Please Select a Process First ==" });
            }
        }
    }
}