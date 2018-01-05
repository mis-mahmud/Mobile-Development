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
using Android.Graphics.Drawables;
using Android.Support.V4.View;

namespace BitopiApprovalSystem
{
    [Activity(Label = "Sample Process")]
    public class SampleProcessActivity : BaseActivity
    {
        public SampleProcessListAdapter adapter;
        public List<SampleProcessModel> list = new List<SampleProcessModel>();
        public ListView listView;
        RelativeLayout rltitle;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SupportActionBar.SetDisplayShowCustomEnabled(true);
            SupportActionBar.SetCustomView(Resource.Layout.custom_actionbar);
            SetContentView(Resource.Layout.SampleProcessLayout);
            listView = FindViewById<ListView>(Resource.Id.lvSampleProcess);
            rltitle = FindViewById<RelativeLayout>(Resource.Id.rltitle);
            rltitle.FindViewById<TextView>(Resource.Id.tvHeaderName).Text = "Sample Process";
            InitializeControl();
        }
        protected async override void OnStart()
        {
            base.OnStart();
            InitializeEvent();
            var SampleRepo = new SampleRepository();
            list = await SampleRepo.GetSampleRequisition();
            adapter = new SampleProcessListAdapter(list, this);
            listView.Adapter = (adapter);
            adapter.NotifyDataSetChanged();
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            try
            {
                var inflater = MenuInflater;
                inflater.Inflate(Resource.Menu.actions, menu);
                var arg1 = menu.FindItem(Resource.Id.action_search);
                var test = new Android.Widget.SearchView(this);
                arg1
                    .SetActionView(test)
                    .SetShowAsAction(ShowAsAction.Always);
                var searchView = MenuItemCompat.GetActionView(arg1);
                var _searchView = searchView.JavaCast<SearchView>();
                setSearchIcons(_searchView);
                _searchView.QueryTextChange += (s, e) =>
                {
                    adapter.Filter.InvokeFilter(e.NewText);
                };
                _searchView.SetOnCloseListener(new SampleProcessSearchViewOnCloseListenter(rltitle, this));
                _searchView.SetOnSearchClickListener(new SampleProcessSearchviewclicklistener(rltitle));
            }
            catch (System.Exception ex)
            {
                // CustomLogger.CustomLog("From Activity: " + application.CurrentActivity + "\nMessage: " + ex.Message + "Stack Trace: " + ex.StackTrace + "\n\n", "", application.CurrentUserName);
            }
            return base.OnCreateOptionsMenu(menu);
        }
        private void setSearchIcons(SearchView mSearchView)
        {
            try
            {
                Java.Lang.Reflect.Field searchField = mSearchView.Class.GetDeclaredField("mSearchButton");
                searchField.Accessible = true;
                ImageView searchBtn = (ImageView)searchField.Get(mSearchView);
                searchBtn.Background = null;
                searchBtn.SetImageResource(Resource.Drawable.search123);
                searchBtn.SetScaleType(ImageView.ScaleType.FitCenter);
                searchBtn.RequestLayout();
                searchField = mSearchView.Class.GetDeclaredField("mSearchPlate");
                searchField.Accessible = true;
                LinearLayout searchPlate = (LinearLayout)searchField.Get(mSearchView);
                ImageView closeBtn = ((ImageView)searchPlate.GetChildAt(1));
                closeBtn.SetImageResource(Resource.Drawable.closed);
                closeBtn.SetScaleType(ImageView.ScaleType.FitCenter);
                closeBtn.RequestLayout();
            }
            catch (System.Exception e)
            {
                string dst = "sdfsfs";
            }
        }
        public async void RefreshGrid()
        {
            var SampleRepo = new SampleRepository();
            list = await SampleRepo.GetSampleRequisition();
            adapter.Items = list;
            adapter.NotifyDataSetChanged();
        }
        protected override void InitializeEvent()
        {
           
            base.InitializeEvent();
        }
        public class SampleProcessListAdapter : BaseAdapter, IFilterable
        {
            List<SampleProcessModel> _list;
            SampleProcessActivity _context;
            Filter filter;

            public SampleProcessListAdapter(List<SampleProcessModel> list, SampleProcessActivity context)
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

                view = LayoutInflater.From(_context).Inflate(Resource.Layout.SampleProcessListItem, parent, false);
                Holder holder = new Holder();
                holder.SewingCompleteDate = (view.FindViewById<EditText>(Resource.Id.etSewingCompleteDate));
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
                model.ManualProcessEndDt = model.ProcessEndDt;
                //view.FindViewById<TextView>(Resource.Id.tvSample).SetTextColor(Color.ParseColor(model.StatusColor));
                if (model.StatusColor.ToLower() == "orange") model.StatusColor = "#FFA500";
                if (model.StatusColor.ToLower() == "green") model.StatusColor = "#23a25f";

                GradientDrawable shape = new GradientDrawable();
                shape.SetCornerRadius(8);
                shape.SetColor(Color.ParseColor(model.StatusColor));
                //view.FindViewById<RelativeLayout>(Resource.Id.rlPanel).SetBackgroundColor(Color.ParseColor(model.StatusColor));
                view.FindViewById<RelativeLayout>(Resource.Id.rlPanel).Background = shape;
                OrderPlanned.Checked = model.OrderPlanned;
                OrderPlanned.Enabled = !model.OrderPlanned;
                chckMaterialReceived.Enabled = model.OrderPlanned == false ? false : !model.MaterialReceived;
                chckMaterialReceived.Checked = model.MaterialReceived;
                holder.SewingCompleteDate.FocusableInTouchMode = false;
                if (model.OrderPlanned)
                {

                    view.FindViewById<TextView>(Resource.Id.tvSewingCompleteDate).Visibility = ViewStates.Visible;
                    view.FindViewById<TextView>(Resource.Id.etSewingCompleteDate).Visibility = ViewStates.Gone;
                }
                else
                {
                    view.FindViewById<TextView>(Resource.Id.tvSewingCompleteDate).Visibility = ViewStates.Gone;
                    view.FindViewById<TextView>(Resource.Id.etSewingCompleteDate).Visibility = ViewStates.Visible;

                    holder.SewingCompleteDate.Click += (s, e) =>
                    {
                        DateTime SetDate = Convert.ToDateTime(model.ProcessEndDt);
                        DatePickerDialog dialog = new DatePickerDialog(_context, (sender, evnt) =>
                        {
                            model.ManualProcessEndDt = holder.SewingCompleteDate.Text = evnt.Date.ToString("dd-MMM-yy");
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

                holder.SampleID = model.SampleID;
                holder.GroupPostiion = position;
                view.Tag = holder;

                return view;
            }

            private async void ChckMaterialReceived_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                CheckBox senderCheckBox = ((CheckBox)sender);
                var sampleid = senderCheckBox.Tag.ToString();
                var model = this.Items.Where(t => t.SampleID == sampleid).First();

                SampleRepository repo = new SampleRepository();
                var result = await repo.UpdateMaterialRcvd(sampleid);
                if (result == "1")
                {
                    model.MaterialReceived = true;
                    _context.RefreshGrid();
                }
            }

            private async void OrderPlanned_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                CheckBox senderCheckBox = ((CheckBox)sender);
                var sampleid = senderCheckBox.Tag.ToString();
                var model = this.Items.Where(t => t.SampleID == sampleid).First();
                model.OrderPlanned = true;

                SampleRepository repo = new SampleRepository();
                var result = await repo.UpdateOrderRcvd(sampleid, model.ProcessEndDt, model.ManualProcessEndDt);
                if (result == "1")
                {
                    model.MaterialReceived = true;
                    _context.RefreshGrid();
                }
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
        class Holder : Java.Lang.Object
        {
            public string SampleID { get; set; }
            public EditText SewingCompleteDate { get; set; }
            public int GroupPostiion { get; set; }

        }
    }
    class SampleProcessSearchViewOnCloseListenter : Java.Lang.Object, Android.Widget.SearchView.IOnCloseListener
    {
        SearchView _searchView;
        ListView _searchListView;
        RelativeLayout _rltitle;
        SampleProcessActivity _activity;
        public SampleProcessSearchViewOnCloseListenter(RelativeLayout rltitle, SampleProcessActivity activity)
        {
            _rltitle = rltitle;
            this._activity = activity;

        }
        public bool OnClose()
        {
            _rltitle.Visibility = ViewStates.Visible;
            _activity.adapter.Items = _activity.list;
            _activity.adapter.NotifyDataSetChanged();
            return false;
        }

    }
    class SampleProcessSearchviewclicklistener : Java.Lang.Object, Android.Views.View.IOnClickListener
    {
        private bool extended = false;
        RelativeLayout _rltitle;
        public SampleProcessSearchviewclicklistener(RelativeLayout rltitle)
        {
            _rltitle = rltitle;
        }
        public void OnClick(View v)
        {
            _rltitle.Visibility = ViewStates.Gone;
        }
    }
}