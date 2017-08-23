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
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Graphics;
using ApiRepository;

namespace BitopiApprovalSystem
{
    [Activity(Label = "ActualProductionEntryActivity")]
    public class ActualProductionEntryActivity : BaseActivity
    {
     
        Android.App.AlertDialog.Builder builder;
        public RelativeLayout rlMsg;
        public RelativeLayout rlapprovalDetail;
        RelativeLayout rltitle;
       
        RelativeLayout RLleft_drawer;
        private DrawerLayout mDrawerLayout;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            builder = new Android.App.AlertDialog.Builder(this);
            builder.SetMessage("Hello, World!");

            builder.SetNegativeButton("Cancel", (s, e) => { /* do something on Cancel click */ });


            base.OnCreate(savedInstanceState);

            SupportRequestWindowFeature(WindowCompat.FeatureActionBar);
            SupportActionBar.SetDisplayShowCustomEnabled(true);
            SupportActionBar.SetCustomView(Resource.Layout.custom_actionbar);
            rltitle = FindViewById<RelativeLayout>(Resource.Id.rltitle);


            SetContentView(Resource.Layout.ApprovalDetailList);

            
            rlMsg = FindViewById<RelativeLayout>(Resource.Id.rlMsg);
            rlapprovalDetail = FindViewById<RelativeLayout>(Resource.Id.rlapprovalDetail);
            
            base.LoadDrawerView(this);
        }
       
    }
}