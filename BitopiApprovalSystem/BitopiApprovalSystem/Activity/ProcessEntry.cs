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
using ApiRepository;
using Android.Graphics.Drawables;
using BitopiApprovalSystem.Model;
using Android.Util;
using Android.Support.V4.View;
using PullToRefresharp.Android.Views;
using Android.Support.V4.Widget;
using System.Threading;

namespace BitopiApprovalSystem
{
    [Activity(Label = "ProcessEntry",WindowSoftInputMode = SoftInput.StateHidden| SoftInput.AdjustResize)]
    public class ProcessEntry : BaseActivity
    {
        PullToRefresharp.Android.Widget.ScrollView ptr;
        ProductionRepository repo = new ProductionRepository();
        ListView lvProduct;
        TextView tvLocation;
        TextView tvRef, tvOrderQty, tvBalanceQty, tvProducedQty, txtWIPQty;
        EditText etQty;
        Button btnSave, btnPlus, btnMinus;
        public ProductionAccountigListAdapter adapter;
        public List<ProdcutionAccountingDBModel> list;
        RelativeLayout gifView;
        private IPullToRefresharpView ptr_view;
        RelativeLayout rlTop;
        RelativeLayout rltitle;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SupportActionBar.SetDisplayShowCustomEnabled(true);
            SupportActionBar.SetCustomView(Resource.Layout.custom_actionbar);
            SetContentView(Resource.Layout.ProcessEntry);
            InitializeControl();

            base.LoadDrawerView();
        }
        protected  override void OnStart()
        {
            base.OnStart();
            InitializeEvent();
            gifView.Visibility = ViewStates.Visible;
            LoadList();
            gifView.Visibility = ViewStates.Gone;
        }
        void LoadList()
        {
            new Thread(new ThreadStart(() =>
            {
                list = repo.GetProductionList(bitopiApplication.User.UserCode, bitopiApplication.ProcessID,
                    bitopiApplication.LocationID, bitopiApplication.PRStatus);
                RunOnUiThread(() =>
                {
                    adapter.Items = list;
                    adapter.NotifyDataSetChanged();
                    //AndHUD.Shared.Dismiss();
                    Log.Debug("", list.Count().ToString());
                });
            })).Start();
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
                //searchBtn.LayoutParameters.Width = 80;
                //searchBtn.LayoutParameters.Height = 80;
                searchBtn.RequestLayout();
                searchField = mSearchView.Class.GetDeclaredField("mSearchPlate");
                searchField.Accessible = true;
                LinearLayout searchPlate = (LinearLayout)searchField.Get(mSearchView);
                ImageView closeBtn = ((ImageView)searchPlate.GetChildAt(1));
                closeBtn.SetImageResource(Resource.Drawable.closed);
                closeBtn.SetScaleType(ImageView.ScaleType.FitCenter);
                //closeBtn.LayoutParameters.Width = 80;
                //closeBtn.LayoutParameters.Height = 80;
                closeBtn.RequestLayout();
                //searchPlate.SetBackgroundResource(Resource.Drawable.SearchPlate);
                //searchPlate.LayoutParameters.Height = 1;

            }
            catch (System.Exception e)
            {
                string dst = "sdfsfs";
            }
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
                //_searchView = searchView.JavaCast<Android.Support.V7.Widget.SearchView>();
                var _searchView = searchView.JavaCast<SearchView>();
                setSearchIcons(_searchView);
                //_searchView.SetBackgroundColor(Color.White);
                //_searchView.SetOnClickListener(new SearchViewOnClickListener(_searchListView));
                //_searchView.Click += (s, e) =>
                //{

                //    _adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1,
                //        truckList.Select(t => t.TruckName).ToArray());
                //    _searchListView.Adapter = _adapter;
                //};

                _searchView.QueryTextChange += (s, e) =>
                {

                    adapter.Filter.InvokeFilter(e.NewText);
                    //_searchListView.Visibility = ViewStates.Visible;
                };

                _searchView.SetOnCloseListener(new TruckSearchViewOnCloseListenter(rltitle, this));
                _searchView.SetOnSearchClickListener(new TruckSearchviewclicklistener(rltitle));

                //ImageView searchIcon = (ImageView)_searchView.FindViewById();
                //searchIcon.setImageResource(R.drawable.abc_ic_search);

            }
            catch (System.Exception ex)
            {
                // CustomLogger.CustomLog("From Activity: " + application.CurrentActivity + "\nMessage: " + ex.Message + "Stack Trace: " + ex.StackTrace + "\n\n", "", application.CurrentUserName);
            }
            return base.OnCreateOptionsMenu(menu);
        }
        protected override void InitializeControl()
        {

            FindViewById<ImageButton>(Resource.Id.btnDrawermenu).Visibility = ViewStates.Visible;
            // ptr = FindViewById<PullToRefresharp.Android.Widget.ScrollView>(Resource.Id.ptr);
            rltitle = FindViewById<RelativeLayout>(Resource.Id.rltitle);
            rlTop = FindViewById<RelativeLayout>(Resource.Id.rlTop);
            lvProduct = FindViewById<ListView>(Resource.Id.lvProduct);
            adapter = new ProductionAccountigListAdapter(list, this);
            lvProduct.Adapter = (adapter);
            gifView = FindViewById<RelativeLayout>(Resource.Id.gifview);
            tvLocation = FindViewById<TextView>(Resource.Id.tvLocation);
            tvRef = FindViewById<TextView>(Resource.Id.tvRef);
            tvOrderQty = FindViewById<TextView>(Resource.Id.txtOrderQty);
            tvBalanceQty = FindViewById<TextView>(Resource.Id.txtBalanceQty);
            tvProducedQty = FindViewById<TextView>(Resource.Id.txtProduceQty);
            txtWIPQty = FindViewById<TextView>(Resource.Id.txtWIPQty);
            etQty = FindViewById<EditText>(Resource.Id.etQty);
            btnSave = FindViewById<Button>(Resource.Id.btnSubmit);

            btnPlus = FindViewById<Button>(Resource.Id.btnPlus);
            btnMinus = FindViewById<Button>(Resource.Id.btnMinus);

            FindViewById<TextView>(Resource.Id.tvHeaderName).Text = bitopiApplication.ProcessName;
            tvLocation.Text = bitopiApplication.LocationName;
            base.InitializeControl();
        }
        protected override void InitializeEvent()
        {
            lvProduct.ItemClick += LvProduct_ItemClick;
            btnSave.Click += BtnSave_Click;
            btnPlus.Click += BtnPlus_Click;
            btnMinus.Click += BtnMinus_Click;
            //if (ptr_view == null && ptr is IPullToRefresharpView)
            //{
            //    ptr_view = (IPullToRefresharpView)ptr;
            //    ptr_view.RefreshActivated += Ptr_view_RefreshActivated; ;
            //}
            base.InitializeEvent();
        }

        private void Ptr_view_RefreshActivated(object sender, EventArgs e)
        {
            LoadList();
            if (ptr_view != null)
            {
                ptr_view.OnRefreshCompleted();
            }
        }

        private void BtnMinus_Click(object sender, EventArgs e)
        {
            int qty = Convert.ToInt16(etQty.Text);
            qty -= 10;
            if (qty >= 0)
                etQty.Text = qty.ToString();
        }

        private void BtnPlus_Click(object sender, EventArgs e)
        {
            int qty = Convert.ToInt16(etQty.Text);
            qty += 10;
            etQty.Text = qty.ToString();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {

            if (tvRef.Text == "None")
            {
                Toast.MakeText(this, "Please Select an item first", ToastLength.Long).Show();
                return;
            }
            var model = list.Where(t => t.RefNo == tvRef.Text).First();
            gifView.Visibility = ViewStates.Visible;

            //new Thread(new ThreadStart(() =>
            //{

                var result = repo.SetProduction(tvRef.Text,
                    Convert.ToInt16(etQty.Text), model.LocationRef, bitopiApplication.User.UserCode);

                RunOnUiThread(() =>
                {
                    if (result > 0)
                    {
                        LoadList();

                        tvOrderQty.Text = model.OrderQty.ToString("N0");
                        tvBalanceQty.Text = model.BalanceQty.ToString("N0");
                        tvProducedQty.Text = model.ProducedQty.ToString("N0");
                        txtWIPQty.Text = model.WIP.ToString("N0");
                        Toast.MakeText(this, "Successfully Saved", ToastLength.Long).Show();
                    }
                    else
                    {
                        Toast.MakeText(this, "Unsuccessfull operation", ToastLength.Long).Show();
                    }
                    gifView.Visibility = ViewStates.Gone;
                });
            //})).Start();
        }

        private void LvProduct_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var model = list[e.Position];
            tvRef.Text = model.RefNo;

            tvOrderQty.Text = model.OrderQty.ToString("N0");
            tvBalanceQty.Text = model.BalanceQty.ToString("N0");
            tvProducedQty.Text = model.ProducedQty.ToString("N0");

            tvLocation.Text = model.LocationName;
        }
    }
    public class ProductionAccountigListAdapter : BaseAdapter, IFilterable
    {
        List<ProdcutionAccountingDBModel> _list;
        Context _context;
        Filter filter;
        public ProductionAccountigListAdapter(List<ProdcutionAccountingDBModel> list, Context context)
        {
            _list = list;
            _context = context;
            filter = new SuggestionsFilter(this);
        }
        public List<ProdcutionAccountingDBModel> Items
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
                view = LayoutInflater.From(_context).Inflate(Resource.Layout.EntryItem, parent, false);
            }
            view.FindViewById<TextView>(Resource.Id.tvRefNo).Text = model.RefNo;
            view.FindViewById<TextView>(Resource.Id.tvColor).Text = model.Color;
            view.FindViewById<TextView>(Resource.Id.tvEO).Text = model.EO;
            view.FindViewById<TextView>(Resource.Id.tvPR).Text = model.PR;
            view.FindViewById<TextView>(Resource.Id.tvSize).Text = model.Size;
            view.FindViewById<TextView>(Resource.Id.tvStyle).Text = model.Style;

            return view;
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
            ProductionAccountigListAdapter customAdapter;
            public SuggestionsFilter(ProductionAccountigListAdapter adapter)
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
                    var matches = from i in customAdapter._list where i.PR.ToLower().Contains(searchFor) select i;
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
                        matchObjects[i] = new Java.Lang.String(customAdapter._list[i].PR.ToLower());
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
    class TruckSearchviewclicklistener : Java.Lang.Object, Android.Views.View.IOnClickListener
    {
        private bool extended = false;
        RelativeLayout _rltitle;
        public TruckSearchviewclicklistener(RelativeLayout rltitle)
        {
            _rltitle = rltitle;
        }
        public void OnClick(View v)
        {
            _rltitle.Visibility = ViewStates.Gone;
        }
    }
    class TruckSearchViewOnCloseListenter : Java.Lang.Object, Android.Widget.SearchView.IOnCloseListener
    {
        SearchView _searchView;
        ListView _searchListView;
        RelativeLayout _rltitle;
        ProcessEntry _activity;
        public TruckSearchViewOnCloseListenter(RelativeLayout rltitle, ProcessEntry activity)
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
}