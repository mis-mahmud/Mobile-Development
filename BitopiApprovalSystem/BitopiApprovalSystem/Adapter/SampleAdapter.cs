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
using Model;
using ApiRepository;
using Android.Graphics.Drawables;
using Android.Graphics;

namespace BitopiApprovalSystem.Adapter
{
    public class SampleFollowupListAdapter : BaseAdapter, IFilterable
    {
        List<SampleFollowupModel> _list;
        SampleFragment _context;
        Filter filter;
        string _status;
        public string ProcessID { get; set; }
        public SampleFollowupListAdapter(List<SampleFollowupModel> list, SampleFragment context)
        {
            _list = list;
            _context = context;
            if (context is SampleNotDelivered)
                _status = "Delivered";
            if (context is SampleNotReceived)
                _status = "Received";

        }
        public List<SampleFollowupModel> Items
        {
            set { _list = value; }
            get { return _list; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            var model = Items[position];

            view = LayoutInflater.From(_context.Activity).Inflate(Resource.Layout.SampleFollowupListItem, parent, false);
            Holder holder = new Holder();

            CheckBox OrderPlanned;
            OrderPlanned = view.FindViewById<CheckBox>(Resource.Id.chckOrderPlanned);
            view.FindViewById<TextView>(Resource.Id.tvSample).Text = model.SampleID;
            view.FindViewById<TextView>(Resource.Id.tvBuyer).Text = model.Buyer;
            view.FindViewById<TextView>(Resource.Id.tvStyle).Text = model.ProductName;
            view.FindViewById<TextView>(Resource.Id.tvTotalQty).Text = model.ReqQty.ToString();
            view.FindViewById<TextView>(Resource.Id.ReqDate).Text = model.RequiredDays.ToString();
            view.FindViewById<TextView>(Resource.Id.tvBuyerDeliveryDt).Text = model.BuyerDeliveryDt;
            view.FindViewById<TextView>(Resource.Id.tvPlanningDate).Text = model.PlanningDate;
            view.FindViewById<TextView>(Resource.Id.lblSampleFollowup).Text = _status;
            if (model.StatusColor.ToLower() == "orange") model.StatusColor = "#FFA500";
            if (model.StatusColor.ToLower() == "green") model.StatusColor = "#23a25f";

            GradientDrawable shape = new GradientDrawable();
            shape.SetCornerRadius(8);
            shape.SetColor(Color.ParseColor(model.StatusColor));
            //view.FindViewById<RelativeLayout>(Resource.Id.rlPanel).SetBackgroundColor(Color.ParseColor(model.StatusColor));
            view.FindViewById<RelativeLayout>(Resource.Id.rlPanel).Background = shape;

            if ((position & 1) == 1)
            {
                view.SetBackgroundColor(Color.ParseColor("#E3F1F8"));
            }
            else
            {
                view.SetBackgroundColor(Color.ParseColor("#ffffff"));
            }



            OrderPlanned.CheckedChange -= OrderPlanned_CheckedChange;
            OrderPlanned.CheckedChange += OrderPlanned_CheckedChange;
            OrderPlanned.Tag = model.tblSampleRequestEntityId.ToString();


            holder.SampleID = model.tblSampleRequestEntityId.ToString();
            holder.GroupPostiion = position;
            view.Tag = holder;

            return view;
        }

        private async void OrderPlanned_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var progressDialog = ProgressDialog.Show(_context.Activity, null, "Please Wait.", true);
            CheckBox senderCheckBox = ((CheckBox)sender);
            var entityId = senderCheckBox.Tag.ToString();

            SampleRepository repo = new SampleRepository();

            var result = "";
            if (_status == "Delivered")
                result = await repo.MakeDelivered(entityId, ProcessID);
            else
                result = await repo.MakeReceived(entityId, ProcessID);
            if (result == "1")
            {
                _context.RefreshGrid();
            }
            progressDialog.Dismiss();

        }

        public override long GetItemId(int position)
        {
            return 1;
        }
        public override Java.Lang.Object GetItem(int position)
        {
            throw new NotImplementedException();
        }
        public override int Count
        {
            get
            {
                if (Items == null)
                    return 0;
                return Items.Count;
            }
        }

        public Filter Filter
        {
            get
            {
                return filter;
            }
        }
        class SuggestionsFilter : Filter
        {
            SampleFollowupListAdapter customAdapter;
            public SuggestionsFilter(SampleFollowupListAdapter adapter)
                : base()
            {
                customAdapter = adapter;
            }
            protected override Filter.FilterResults PerformFiltering(Java.Lang.ICharSequence constraint)
            {
                FilterResults results = new FilterResults();
                if (constraint != null)
                {
                    var searchFor = constraint.ToString().ToLower();

                    var matches = from i in customAdapter._list where i.SampleID.ToLower().Contains(searchFor) select i;
                    //foreach (var match in matches) {
                    //matchList.Add (match);
                    //}
                    customAdapter._list = matches.ToList();
                    // Console.System.Diagnostics.Debug.WriteLine("resultCount:" + customAdapter.MatchItems.Count);
                    // not sure if the Java array/FilterResults are used
                    Java.Lang.Object[] matchObjects;
                    matchObjects = new Java.Lang.Object[customAdapter._list.Count];
                    for (int i = 0; i < customAdapter._list.Count; i++)
                    {
                        matchObjects[i] = new Java.Lang.String(customAdapter._list[i].SampleID.ToLower());
                    }
                    results.Values = matchObjects;
                    results.Count = customAdapter._list.Count;
                }
                return results;
            }
            protected override void PublishResults(Java.Lang.ICharSequence constraint, Filter.FilterResults results)
            {
                customAdapter.NotifyDataSetChanged();
            }
        }
        class Holder : Java.Lang.Object
        {
            public string SampleID { get; set; }

            public int GroupPostiion { get; set; }

        }
    }
    public class SampleUpCommingListAdapter : BaseAdapter, IFilterable
    {
        List<SampleUpcommingModel> _list;
        SampleFragment _context;
        Filter filter;
        string _status;

        public SampleUpCommingListAdapter(List<SampleUpcommingModel> list, SampleFragment context)
        {
            _list = list;
            _context = context;


        }
        public List<SampleUpcommingModel> Items
        {
            set { _list = value; }
            get { return _list; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            var model = Items[position];

            view = LayoutInflater.From(_context.Activity).Inflate(Resource.Layout.SampleUpComingListItem, parent, false);
            Holder holder = new Holder();

            CheckBox OrderPlanned;
            OrderPlanned = view.FindViewById<CheckBox>(Resource.Id.chckOrderPlanned);

            view.FindViewById<TextView>(Resource.Id.tvSample).Text = model.SampleID;
            view.FindViewById<TextView>(Resource.Id.tvBuyer).Text = model.Buyer;
            view.FindViewById<TextView>(Resource.Id.tvStyle).Text = model.ProductName;
            view.FindViewById<TextView>(Resource.Id.tvTotalQty).Text = model.ReqQty.ToString();
            view.FindViewById<TextView>(Resource.Id.etMerReqDt).Text = model.BuyerDeliveryDt;
            //view.FindViewById<TextView>(Resource.Id.tvBuyerDeliveryDt).Text = model.BuyerDeliveryDt;
            view.FindViewById<TextView>(Resource.Id.tvPlanningDate).Text = model.PlanningDate;
            view.FindViewById<TextView>(Resource.Id.tvCurrentStatus).Text = model.DevelopmentStatus;
            if (model.StatusColor == null)
            {

                view.FindViewById<RelativeLayout>(Resource.Id.rlPanel).Visibility = ViewStates.Gone;
            }
            else
            {
                view.FindViewById<RelativeLayout>(Resource.Id.rlPanel).Visibility = ViewStates.Visible;
                if (model.StatusColor.ToLower() == "orange") model.StatusColor = "#FFA500";
                if (model.StatusColor.ToLower() == "green") model.StatusColor = "#23a25f";

                GradientDrawable shape = new GradientDrawable();
                shape.SetCornerRadius(8);
                shape.SetColor(Color.ParseColor(model.StatusColor));
                //view.FindViewById<RelativeLayout>(Resource.Id.rlPanel).SetBackgroundColor(Color.ParseColor(model.StatusColor));
                view.FindViewById<RelativeLayout>(Resource.Id.rlPanel).Background = shape;
            }

            if ((position & 1) == 1)
            {
                view.SetBackgroundColor(Color.ParseColor("#E3F1F8"));
            }
            else
            {
                view.SetBackgroundColor(Color.ParseColor("#ffffff"));
            }


            holder.GroupPostiion = position;
            view.Tag = holder;

            return view;
        }

        private async void OrderPlanned_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {

        }

        public override long GetItemId(int position)
        {
            return 1;
        }
        public override Java.Lang.Object GetItem(int position)
        {
            throw new NotImplementedException();
        }
        public override int Count
        {
            get
            {
                if (Items == null)
                    return 0;
                return Items.Count;
            }
        }

        public Filter Filter
        {
            get
            {
                return filter;
            }
        }
        class SuggestionsFilter : Filter
        {
            SampleUpCommingListAdapter customAdapter;
            public SuggestionsFilter(SampleUpCommingListAdapter adapter)
                : base()
            {
                customAdapter = adapter;
            }
            protected override Filter.FilterResults PerformFiltering(Java.Lang.ICharSequence constraint)
            {
                FilterResults results = new FilterResults();
                if (constraint != null)
                {
                    var searchFor = constraint.ToString().ToLower();

                    var matches = from i in customAdapter._list where i.SampleID.ToLower().Contains(searchFor) select i;
                    //foreach (var match in matches) {
                    //matchList.Add (match);
                    //}
                    customAdapter._list = matches.ToList();
                    // Console.System.Diagnostics.Debug.WriteLine("resultCount:" + customAdapter.MatchItems.Count);
                    // not sure if the Java array/FilterResults are used
                    Java.Lang.Object[] matchObjects;
                    matchObjects = new Java.Lang.Object[customAdapter._list.Count];
                    for (int i = 0; i < customAdapter._list.Count; i++)
                    {
                        matchObjects[i] = new Java.Lang.String(customAdapter._list[i].SampleID.ToLower());
                    }
                    results.Values = matchObjects;
                    results.Count = customAdapter._list.Count;
                }
                return results;
            }
            protected override void PublishResults(Java.Lang.ICharSequence constraint, Filter.FilterResults results)
            {
                customAdapter.NotifyDataSetChanged();
            }
        }
        class Holder : Java.Lang.Object
        {
            public string SampleID { get; set; }

            public int GroupPostiion { get; set; }

        }
    }
}