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

namespace pa
{
    [Activity(Label = "ProcessManageActivity")]
    public class ProcessManageActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.ProcessManage);
            base.OnCreate(savedInstanceState);
        }
    }
}