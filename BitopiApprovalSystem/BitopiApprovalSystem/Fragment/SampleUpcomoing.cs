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
using ApiRepository;
using Model;
using BitopiApprovalSystem.Adapter;

namespace BitopiApprovalSystem
{
    public class SampleUpcomoing : SampleFragment
    {
        string stringData;
        ListView lvSampleUpcoming;
        SampleUpCommingListAdapter adapter;
        List<SampleUpcommingModel> _list;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            stringData = Arguments.GetString("ProcessID", string.Empty);
            View view = inflater.Inflate(Resource.Layout.tab_frag_upcoming, null);
            lvSampleUpcoming = view.FindViewById<ListView>(Resource.Id.lvSampleUpcoming);
            _list = new List<SampleUpcommingModel>();
            adapter = new SampleUpCommingListAdapter(_list, this);
            lvSampleUpcoming.Adapter = adapter;
            return view;
        }
        public async override void OnStart()
        {
            base.OnStart();
            SampleRepository repo = new SampleRepository();
            _list = await repo.GetProcessFollowUp_Upcoming(Convert.ToInt16(stringData));
            adapter.Items = _list;
            adapter.NotifyDataSetChanged();
        }

        public override void RefreshGrid()
        {
            adapter.NotifyDataSetChanged();
        }
    }
}