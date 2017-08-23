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
using Refractored.Controls;
using Android.Graphics;
using Android.Support.V4.View;
using Android.Graphics.Drawables;

namespace pa
{
    [Activity(Label = "ProcessListActivity")]
    public class ProcessListActivity : BaseActivity
    {
        CircleImageView ivUser;
        Button btnAll, btnRunning;
        Spinner spProcess, spLocation, spPr;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            SupportRequestWindowFeature(WindowCompat.FeatureActionBar);
            SupportActionBar.SetDisplayShowCustomEnabled(true);
            SupportActionBar.SetCustomView(Resource.Layout.custom_actionbar);
            SupportActionBar.SetBackgroundDrawable(
    new ColorDrawable(Color.ParseColor("#0C2E78")));
            SetContentView(Resource.Layout.ProcessList);
            base.OnCreate(savedInstanceState);
        }
        protected override void OnStart()
        {
            base.OnStart();
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InSampleSize = 4;
            Bitmap bitmap = BitmapFactory.DecodeByteArray(bitopiApplication.User.EmpImage, 0,
                bitopiApplication.User.EmpImage.Length, options);
            ivUser.SetImageBitmap(bitmap);
            spProcess.Adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, bitopiApplication.DDLList.Select(t => t.ProcessName).Distinct().ToArray());
            spLocation.Adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, bitopiApplication.DDLList.Select(t => t.LocationName).Distinct().ToArray());

        }
        public override void InitializeControl()
        {
            ivUser = FindViewById<CircleImageView>(Resource.Id.ivUserImg);

            spProcess = FindViewById<Spinner>(Resource.Id.spProcess);
            spLocation = FindViewById<Spinner>(Resource.Id.spLocation);
            //spPr = FindViewById<Spinner>(Resource.Id.spPr);

            FindViewById<TextView>(Resource.Id.tvHeaderName).Text = "Production Accounting";
            base.InitializeControl();
        }
        public override void InitializeEvent()
        {
            
            base.InitializeEvent();
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(ProcessEntry));
            bitopiApplication.ProcessID = ((Button)(sender)).Tag.ToString();
            bitopiApplication.ProcessName = ((Button)(sender)).Text;
            StartActivity(i);
        }
    }
}