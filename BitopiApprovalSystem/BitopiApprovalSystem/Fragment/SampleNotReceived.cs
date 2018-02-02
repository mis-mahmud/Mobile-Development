using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Model;
using ApiRepository;
using BitopiApprovalSystem.Adapter;

namespace BitopiApprovalSystem
{
    public class SampleNotReceived : SampleFragment
    {
        string stringData;
        ListView lvSampleNotReceived;
        SampleFollowupListAdapter adapter;
        List<SampleFollowupModel> _list;
        TextView tvMsg;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            stringData = Arguments.GetString("ProcessID", string.Empty);
            View view = inflater.Inflate(Resource.Layout.tab_frag_notreceived, null);
            lvSampleNotReceived = view.FindViewById<ListView>(Resource.Id.lvSampleNotReceived);
            tvMsg = view.FindViewById<TextView>(Resource.Id.tvMsg);
            _list = new List<SampleFollowupModel>();
            
            adapter = new Adapter.SampleFollowupListAdapter(_list, this);
            lvSampleNotReceived.Adapter = adapter;
            return view;
        }
        public async override void OnStart()
        {
            base.OnStart();
            SampleRepository repo = new SampleRepository();
            var progressDialog = ProgressDialog.Show(this.Activity, null, "Please Wait.", true);
            _list = await repo.GetProcessFollowUpList(stringData, "Not Received");
            
            adapter.ProcessID = stringData;
            adapter.Items = _list;
            adapter.NotifyDataSetChanged();
            progressDialog.Dismiss();
            if (_list.Count == 0) tvMsg.Visibility = ViewStates.Visible;
            else tvMsg.Visibility = ViewStates.Gone;
        }

        public async override void RefreshGrid()
        {
            SampleRepository repo = new SampleRepository();
            var progressDialog = ProgressDialog.Show(this.Activity, null, "Please Wait.", true);
            _list = await repo.GetProcessFollowUpList(stringData, "Not Received");
            adapter.ProcessID = stringData;
            adapter.Items = _list;
            adapter.NotifyDataSetChanged();
            progressDialog.Dismiss();
            if (_list.Count == 0) tvMsg.Visibility = ViewStates.Visible;
            else tvMsg.Visibility = ViewStates.Gone;
        }
    }
    public abstract class SampleFragment : Android.Support.V4.App.Fragment
    {
        public abstract void RefreshGrid();
    }
}