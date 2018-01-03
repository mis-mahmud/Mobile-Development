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
using BitopiApprovalSystem.Model;
using Model;
using ApiRepository;
using Android.Graphics;

namespace BitopiApprovalSystem
{
    [Activity(Label = "Sample Process")]
    public class SampleProcessActivity : BaseActivity
    {
        public SampleProcessListAdapter adapter;
        public List<SampleProcessModel> list = new List<SampleProcessModel>();
        public ListView listView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SampleProcessLayout);
            listView = FindViewById<ListView>(Resource.Id.lvSampleProcess);

        }
        protected async override void OnStart()
        {
            base.OnStart();
            var SampleRepo = new SampleRepository();
            list = await SampleRepo.GetSampleRequisition();
            adapter = new SampleProcessListAdapter(list, this);
            listView.Adapter = (adapter);
            adapter.NotifyDataSetChanged();
        }
        public class SampleProcessListAdapter : BaseAdapter, IFilterable
        {
            List<SampleProcessModel> _list;
            Context _context;
            Filter filter;

            public SampleProcessListAdapter(List<SampleProcessModel> list, Context context)
            {
                _list = list;
                _context = context;
                filter = new SuggestionsFilter(this);
            }
            public List<SampleProcessModel> Items
            {
                set { _list = value; }
                get { return _list; }
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                View view = convertView;
                var model = Items[position];
                if (view == null)
                {
                    view = LayoutInflater.From(_context).Inflate(Resource.Layout.SampleProcessListItem, parent, false);
                }
                CheckBox OrderPlanned, chckMaterialReceived;
                OrderPlanned = view.FindViewById<CheckBox>(Resource.Id.chckOrderPlanned);
                chckMaterialReceived = view.FindViewById<CheckBox>(Resource.Id.chckMaterialReceived);
                view.FindViewById<TextView>(Resource.Id.tvSample).Text = model.SampleID;
                view.FindViewById<TextView>(Resource.Id.tvBuyer).Text = model.BuyerName;
                view.FindViewById<TextView>(Resource.Id.tvStyle).Text = model.StyleName;
                view.FindViewById<TextView>(Resource.Id.tvReqType).Text = model.SampleType;
                view.FindViewById<TextView>(Resource.Id.DeliveryDate).Text = model.DeliveryTentativeDate;
                view.FindViewById<TextView>(Resource.Id.tvMerReqDate).Text = model.MerchRequestDt;
                view.FindViewById<TextView>(Resource.Id.tvSewingCompleteDate).Text = model.ProcessEndDt;
                model.ManualProcessEndDt= model.ProcessEndDt;
                //view.FindViewById<TextView>(Resource.Id.tvSample).SetTextColor(Color.ParseColor(model.StatusColor));
                if (model.StatusColor.ToLower() == "orange") model.StatusColor = "#FFA500";
                view.FindViewById<View>(Resource.Id.vwStatus).SetBackgroundColor(Color.ParseColor(model.StatusColor));
                OrderPlanned.Checked = model.OrderPlanned;
                OrderPlanned.Enabled = !model.OrderPlanned;
                chckMaterialReceived.Enabled = !model.MaterialReceived;
                chckMaterialReceived.Checked = model.MaterialReceived;
                if (model.OrderPlanned)
                {
                    
                    view.FindViewById<TextView>(Resource.Id.tvSewingCompleteDate).Visibility = ViewStates.Visible;
                    view.FindViewById<TextView>(Resource.Id.etSewingCompleteDate).Visibility = ViewStates.Gone;
                }
                else
                {
                    view.FindViewById<TextView>(Resource.Id.tvSewingCompleteDate).Visibility = ViewStates.Gone;
                    view.FindViewById<TextView>(Resource.Id.etSewingCompleteDate).Visibility = ViewStates.Visible;

                    view.FindViewById<TextView>(Resource.Id.etSewingCompleteDate).Click += (s, e) =>
                    {
                        DateTime SetDate = Convert.ToDateTime(model.ProcessEndDt);
                        DatePickerDialog dialog = new DatePickerDialog(_context, (sender, evnt) =>
                        {
                            model.ManualProcessEndDt = view.FindViewById<TextView>(Resource.Id.etSewingCompleteDate).Text = evnt.Date.ToString("dd-MMM-yyyy");
                        }, SetDate.Year, SetDate.Month - 1, SetDate.Day);
                        dialog.DatePicker.MinDate = SetDate.Millisecond;
                        dialog.Show();
                    };
                }
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
                OrderPlanned.Tag = model.SampleID;

                chckMaterialReceived.CheckedChange -= ChckMaterialReceived_CheckedChange;
                chckMaterialReceived.CheckedChange += ChckMaterialReceived_CheckedChange;
                chckMaterialReceived.Tag = model.SampleID;
                view.Tag = model.SampleID;
                return view;
            }

            private async void ChckMaterialReceived_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                CheckBox senderCheckBox = ((CheckBox)sender);
                var sampleid = senderCheckBox.Tag.ToString();
                var model = this.Items.Where(t => t.SampleID == sampleid).First();

                SampleRepository repo = new SampleRepository();
                var result = await repo.UpdateMaterialRcvd(sampleid);
                if (result > 0) model.MaterialReceived = true;
                this.NotifyDataSetChanged();
            }

            private async void OrderPlanned_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                CheckBox senderCheckBox = ((CheckBox)sender);
                var sampleid = senderCheckBox.Tag.ToString();
                var model = this.Items.Where(t => t.SampleID == sampleid).First();
                model.OrderPlanned = true;

                SampleRepository repo = new SampleRepository();
                var result = await repo.UpdateOrderRcvd(sampleid, model.ProcessEndDt, model.ManualProcessEndDt);
                if (result > 0) model.OrderPlanned = true;
                this.NotifyDataSetChanged();
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
                SampleProcessListAdapter customAdapter;
                public SuggestionsFilter(SampleProcessListAdapter adapter)
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
                        //Console.System.Diagnostics.Debug.WriteLine("searchFor:" + searchFor);
                        // var matchList = new List<TruckModel>();
                        // find matches, IndexOf means look for the input anywhere in the items
                        // but it isn't case-sensitive by default!
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
        }
    }
}