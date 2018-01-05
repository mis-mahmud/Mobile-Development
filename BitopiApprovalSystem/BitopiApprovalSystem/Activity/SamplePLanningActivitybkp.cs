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
using Android.Support.V7.App;

namespace BitopiApprovalSystem.bkp
{
    [Activity(Label = "Sample Planning")]
    public class SamplePlanningActivity : BitopiActivity, Android.Support.V7.App.ActionBar.ITabListener
    {
        static readonly string Tag = "ActionBarTabsSupport";
        RelativeLayout rltitle;
        Android.Support.V4.App.Fragment[] _fragments;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SupportActionBar.SetDisplayShowCustomEnabled(true);
            SupportActionBar.SetCustomView(Resource.Layout.custom_actionbar);
            SetContentView(Resource.Layout.SamplePlanningLayout);

            SupportActionBar.NavigationMode = Android.Support.V7.App.ActionBar.NavigationModeTabs;
            SetContentView(Resource.Layout.SamplePlanningLayout);
            rltitle = FindViewById<RelativeLayout>(Resource.Id.rltitle);
            rltitle.FindViewById<TextView>(Resource.Id.tvHeaderName).Text = "Sample Process";
            _fragments = new Android.Support.V4.App.Fragment[]
                         {
                             new SampleNotReceived(),
                             new SampleNotDelivered(),
                             new SampleUpcomoing()
                         };

            AddTabToActionBar(Resource.String.SampleNotReceived_label, Resource.Drawable.notreceived);
            AddTabToActionBar(Resource.String.SampleNotDelivered_label, Resource.Drawable.notdelivered);
            AddTabToActionBar(Resource.String.SampleUpcomoing_label, Resource.Drawable.upcoming);
            InitializeControl();
        }
        protected override void OnStart()
        {
            base.OnStart();
            InitializeEvent();
        }

        void AddTabToActionBar(int labelResourceId, int iconResourceId)
        {

            Android.Support.V7.App.ActionBar.Tab tab = SupportActionBar.NewTab()
                                          .SetText(labelResourceId)
                                         .SetIcon(iconResourceId)
                                         .SetTabListener(this);
            //tab.TabSelected += TabOnTabSelected;
            SupportActionBar.AddTab(tab);
        }
        public void OnTabReselected(Android.Support.V7.App.ActionBar.Tab tab, Android.Support.V4.App.FragmentTransaction ft)
        {

        }

        public void OnTabSelected(Android.Support.V7.App.ActionBar.Tab tab, Android.Support.V4.App.FragmentTransaction ft)
        {

            Android.Support.V4.App.Fragment frag = _fragments[tab.Position];
            ft.Replace(Resource.Id.frameLayout1, frag);
        }

        public void OnTabUnselected(Android.Support.V7.App.ActionBar.Tab tab, Android.Support.V4.App.FragmentTransaction ft)
        {

        }

    }
}