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
using Android.Support.V4.App;
using Java.Util;
using ApiRepository;

namespace BitopiApprovalSystem
{
    [Activity(Label = "Sample Planning")]
    public class SamplePlanningActivity : BitopiActivity, TabHost.IOnTabChangeListener
    {
        RelativeLayout rltitle;
        private TabHost mTabHost;
        private Dictionary<string, TabInfo> mapTabInfo = new Dictionary<string, TabInfo>();
        private TabInfo mLastTab = null;
        Spinner spProcess;
        SampleRepository repo;
        List<DDLList> ddlList;
        string[] proceeArray;
        string ProcessID= "12";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            repo = new SampleRepository();
            base.OnCreate(savedInstanceState);
            SupportActionBar.SetDisplayShowCustomEnabled(true);
            SupportActionBar.SetCustomView(Resource.Layout.custom_actionbar);
            SetContentView(Resource.Layout.SamplePlanningLayout);
            initialiseTabHost(savedInstanceState);
            if (savedInstanceState != null)
            {
                mTabHost.SetCurrentTabByTag(savedInstanceState.GetString("tab")); //set the tab as per the saved state
            }
            rltitle = FindViewById<RelativeLayout>(Resource.Id.rltitle);
            rltitle.FindViewById<TextView>(Resource.Id.tvHeaderName).Text = "Sample Planning";
            InitializeControl();
        }
        protected override void InitializeControl()
        {
            base.InitializeControl();
            spProcess = FindViewById<Spinner>(Resource.Id.spProcess);
        }
        protected override void InitializeEvent()
        {
            spProcess.ItemSelected += SpProcess_ItemSelected; ;
            base.InitializeEvent();
        }

        private void SpProcess_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            string processName = proceeArray[e.Position];
            ProcessID = ddlList.Where(t => t.DrpText == processName).First().DrpValue;
        }

        protected async override void OnStart()
        {
            base.OnStart();
            InitializeEvent();
            ddlList = await repo.LoadddlProductionProcess();
            proceeArray = ddlList.Select(t => t.DrpText).Distinct().ToArray();
            spProcess.Adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, proceeArray);

        }
        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutString("tab", mTabHost.CurrentTabTag);
            base.OnSaveInstanceState(outState);
        }
        private void initialiseTabHost(Bundle args)
        {
            mTabHost = (TabHost)FindViewById(Android.Resource.Id.TabHost);
            mTabHost.Setup();
            TabInfo tabInfo = null;
            SamplePlanningActivity.addTab(this, this.mTabHost, this.mTabHost.NewTabSpec("Tab1").SetIndicator("Not Received"),
                (tabInfo = new TabInfo("Tab1", typeof(SampleNotReceived), args!=null?args:new Bundle())));

            this.mapTabInfo.Add(tabInfo.tag, tabInfo);

            SamplePlanningActivity.addTab(this, this.mTabHost, this.mTabHost.NewTabSpec("Tab2").SetIndicator("Not Delivered"),
                (tabInfo = new TabInfo("Tab2", typeof(SampleNotDelivered), args != null ? args : new Bundle())));
            this.mapTabInfo.Add(tabInfo.tag, tabInfo);
            SamplePlanningActivity.addTab(this, this.mTabHost, this.mTabHost.NewTabSpec("Tab3").SetIndicator("Upcoming"),
                (tabInfo = new TabInfo("Tab3", typeof(SampleUpcomoing), args != null ? args : new Bundle())));
            this.mapTabInfo.Add(tabInfo.tag, tabInfo);
            // Default to first tab
            this.OnTabChanged("Tab1");
            //
            mTabHost.SetOnTabChangedListener(this);
        }
        internal static void addTab(SamplePlanningActivity activity, TabHost tabHost, TabHost.TabSpec tabSpec, TabInfo tabInfo)
        {
            // Attach a Tab view factory to the spec
            tabSpec.SetContent(new TabFactory(activity));
            String tag = tabSpec.Tag;

            // Check to see if we already have a fragment for this tab, probably
            // from a previously saved state.  If so, deactivate it, because our
            // initial state is that a tab isn't shown.
            tabInfo.fragment = activity.SupportFragmentManager.FindFragmentByTag(tag);
            if (tabInfo.fragment != null && !tabInfo.fragment.IsDetached)
            {
                Android.Support.V4.App.FragmentTransaction ft = activity.SupportFragmentManager.BeginTransaction();
                ft.Detach(tabInfo.fragment);
                ft.Commit();
                activity.SupportFragmentManager.ExecutePendingTransactions();
            }

            tabHost.AddTab(tabSpec);
        }
        public void OnTabChanged(string tabId)
        {
            TabInfo newTab = this.mapTabInfo[tabId];
            
                newTab.args.PutString("ProcessID", ProcessID);
            if (mLastTab != newTab)
            {
                Android.Support.V4.App.FragmentTransaction ft = this.SupportFragmentManager.BeginTransaction();
                if (mLastTab != null)
                {
                    if (mLastTab.fragment != null)
                    {
                        ft.Detach(mLastTab.fragment);
                    }
                }
                if (newTab != null)
                {
                    if (newTab.fragment == null)
                    {
                        newTab.fragment = Android.Support.V4.App.Fragment.Instantiate(this, Java.Lang.Class.FromType(newTab.clss).Name
                                , newTab.args);

                        ft.Add(Resource.Id.realtabcontent, newTab.fragment, newTab.tag);

                    }
                    else
                    {
                        ft.Attach(newTab.fragment);
                    }
                }

                mLastTab = newTab;
                ft.Commit();
                this.SupportFragmentManager.ExecutePendingTransactions();
            }
        }

        internal class TabInfo
        {
            internal string tag;
            internal Type clss;
            internal Bundle args;
            internal Android.Support.V4.App.Fragment fragment;
            public TabInfo(String tag, Type clazz, Bundle args)
            {
                this.tag = tag;
                this.clss = clazz;
                this.args = args;
            }

        }
        class TabFactory : Java.Lang.Object, TabHost.ITabContentFactory
        {
            private static Context mContext;
            public TabFactory(Context context)
            {
                mContext = context;
            }
            public View CreateTabContent(string tag)
            {
                View v = new View(mContext);
                v.SetMinimumWidth(0);
                v.SetMinimumHeight(0);
                return v;
            }
        }
    }
}