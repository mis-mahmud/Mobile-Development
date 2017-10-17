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
using BitopiApprovalSystem.Model;
using System.Threading;
using Android.Support.V4.View;
using BitopiApprovalSystem.DAL;
using Android.Graphics;
using PullToRefresharp.Android.Views;
using Android.Util;
using Android.Views.Animations;
using Android.Graphics.Drawables;

namespace BitopiApprovalSystem
{
    [Activity(Label = "ProductionRejection")]
    public class ProductionRejection : BaseActivity
    {
        PullToRefresharp.Android.Widget.ScrollView ptr;
        ProductionRepository repo = new ProductionRepository();
        ListView lvProduct;
        TextView tvLocation;
        TextView tvRef, tvOrderQty, tvBalanceQty, tvProducedQty, txtWIPQty;
        EditText etQty, etGrade, etSKUCode;
        Button btnSave, btnPlus, btnMinus;
        AutoCompleteTextView atvReference;
        public ProductionAccountigListAdapter adapter;
        public List<ProdcutionAccountingDBModel> list;
        RelativeLayout gifView;
        private IPullToRefresharpView ptr_view;
        RelativeLayout rlTop;
        RelativeLayout rltitle, rlPRLV;
        Button recent1, recent2, recent3, recent4, recent5;
        Button btnAll, btnRunning;
        string SelectedPRStatus = "";
        bool isListShown = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SupportActionBar.SetDisplayShowCustomEnabled(true);
            SupportActionBar.SetCustomView(Resource.Layout.custom_actionbar);
            SetContentView(Resource.Layout.ProductionRejection);
            InitializeControl();
            PopulateRecentItem();
            if (recent1.Text != "")
            {
                LoadSelectedRef(recent1.Text);
            }
            //else
            //{
            //    LoadList("");
            //}
            base.LoadDrawerView();
        }
        protected override void OnStart()
        {
            base.OnStart();
            InitializeEvent();
            gifView.Visibility = ViewStates.Visible;

            gifView.Visibility = ViewStates.Gone;

        }
        void LoadList(string PRStatus, Action task = null)
        {
            var progressDialog = ProgressDialog.Show(this, null, "Please Wait.", true);
            new Thread(new ThreadStart(() =>
            {
                list = repo.GetProductionList(bitopiApplication.User.UserCode, DBAccess.Database.RecentHistory.Result.ProcessID,
                    DBAccess.Database.RecentHistory.Result.LocationID, PRStatus, 3);
                RunOnUiThread(() =>
                {

                    atvReference.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, list.Select(t => t.RefNo).ToArray());
                    adapter.Items = list;
                    adapter.NotifyDataSetChanged();
                    progressDialog.Dismiss();
                    if (task != null)
                    {
                        if (list.Count > 0) task();
                        else
                            Toast.MakeText(this, "No Data Found", ToastLength.Long).Show();
                    }
                    //AndHUD.Shared.Dismiss();
                    Log.Debug("", list.Count().ToString());
                });
            })).Start();
        }
        public override void OnBackPressed()
        {
            if (isListShown)
            {
                Animation bottomUp = Android.Views.Animations.AnimationUtils.LoadAnimation(this,
                Resource.Animation.bottom_down);

                lvProduct.StartAnimation(bottomUp);
                rlPRLV.Visibility = (ViewStates.Gone);
                isListShown = false;
            }
            else
            {
                base.OnBackPressed();
            }
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

                // _searchView.SetOnCloseListener(new TruckSearchViewOnCloseListenter(rltitle, this));
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
            atvReference = FindViewById<AutoCompleteTextView>(Resource.Id.atvReference);
            FindViewById<ImageButton>(Resource.Id.btnDrawermenu).Visibility = ViewStates.Visible;
            // ptr = FindViewById<PullToRefresharp.Android.Widget.ScrollView>(Resource.Id.ptr);
            rltitle = FindViewById<RelativeLayout>(Resource.Id.rltitle);
            rlPRLV = FindViewById<RelativeLayout>(Resource.Id.rlPRLV);
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
            etGrade = FindViewById<EditText>(Resource.Id.etGrade);
            etSKUCode = FindViewById<EditText>(Resource.Id.etSKUCode);
            btnSave = FindViewById<Button>(Resource.Id.btnSubmit);

            btnPlus = FindViewById<Button>(Resource.Id.btnPlus);
            btnMinus = FindViewById<Button>(Resource.Id.btnMinus);
            btnAll = FindViewById<Button>(Resource.Id.btnAll);
            btnRunning = FindViewById<Button>(Resource.Id.btnRunning);
            FindViewById<TextView>(Resource.Id.tvHeaderName).Text = DBAccess.Database.RecentHistory.Result.Process + @"\Rejection";
            tvLocation.Text = DBAccess.Database.RecentHistory.Result.Location;

            recent1 = FindViewById<Button>(Resource.Id.recent1);
            recent2 = FindViewById<Button>(Resource.Id.recent2);
            recent3 = FindViewById<Button>(Resource.Id.recent3);
            recent4 = FindViewById<Button>(Resource.Id.recent4);
            recent5 = FindViewById<Button>(Resource.Id.recent5);


            base.InitializeControl();
        }
        public void PopulateRecentItem()
        {
            GradientDrawable shape = new GradientDrawable();
            shape.SetCornerRadius(6);
            List<RecentPR> prs = DBAccess.Database.RecentPRs.Result.Where(t => t.EntryType == (int)EntryType.Rejection &&
            t.LocationRef == DBAccess.Database.RecentHistory.Result.Location
            ).ToList();
            if (prs.Count == 0)
            {
                return;
            }
            for (int i = prs.Count - 1; i >= 0; i--)
            {
                if (i == prs.Count - 1)
                {
                    recent1.Text = prs[i].RefID;
                    shape.SetColor(Color.ParseColor("#ff5722"));
                    recent1.Background = (shape);

                }
                if (i == prs.Count - 2)
                {
                    recent2.Text = prs[i].RefID;
                    //recent2.SetBackgroundColor(Color.ParseColor("#ff7043"));
                    shape.SetColor(Color.ParseColor("#ff7043"));
                    recent2.Background = (shape);
                }
                if (i == prs.Count - 3)
                {
                    recent3.Text = prs[i].RefID;
                    //recent3.SetBackgroundColor(Color.ParseColor("#ff8a65"));
                    shape.SetColor(Color.ParseColor("#ff8a65"));
                    recent3.Background = (shape);
                }
                if (i == prs.Count - 4)
                {
                    recent4.Text = prs[i].RefID;
                    // recent4.SetBackgroundColor(Color.ParseColor("#ffab91"));
                    shape.SetColor(Color.ParseColor("#ffab91"));
                    recent4.Background = (shape);
                }
                if (i == prs.Count - 5)
                {
                    recent5.Text = prs[i].RefID;
                    //recent5.SetBackgroundColor(Color.ParseColor("#ffccbc"));
                    shape.SetColor(Color.ParseColor("#ffccbc"));
                    recent5.Background = (shape);
                }
            }
        }
        protected override void InitializeEvent()
        {
            lvProduct.ItemClick += LvProduct_ItemClick;
            btnSave.Click += BtnSave_Click;
            //btnPlus.Click += BtnPlus_Click;
            btnPlus.SetOnTouchListener(new LongPressClickListener(() =>
            {
                int qty = Convert.ToInt16(etQty.Text == "" ? "0" : etQty.Text);
                qty += 10;
                etQty.Text = qty.ToString();
            }));
            btnMinus.SetOnTouchListener(new LongPressClickListener(() =>
            {
                int qty = Convert.ToInt16(etQty.Text == "" ? "0" : etQty.Text);
                qty -= 10;
                if (qty >= 0)
                    etQty.Text = qty.ToString();
            }));

            //btnMinus.Click += BtnMinus_Click;
            btnAll.Click += LoadPR_Click;
            btnRunning.Click += LoadPR_Click;
            recent1.Click += Recent_Click;
            recent2.Click += Recent_Click;
            recent3.Click += Recent_Click;
            recent4.Click += Recent_Click;
            recent5.Click += Recent_Click;
            //if (ptr_view == null && ptr is IPullToRefresharpView)
            //{
            //    ptr_view = (IPullToRefresharpView)ptr;
            //    ptr_view.RefreshActivated += Ptr_view_RefreshActivated; ;
            //}
            rlPRLV.Click += RlPRLV_Click; ;
            base.InitializeEvent();
        }

        private void RlPRLV_Click(object sender, EventArgs e)
        {
            Animation bottomUp = Android.Views.Animations.AnimationUtils.LoadAnimation(this,
            Resource.Animation.bottom_down);

            lvProduct.StartAnimation(bottomUp);
            rlPRLV.Visibility = (ViewStates.Gone);
            isListShown = false;
        }

        private void Recent_Click(object sender, EventArgs e)
        {
            string Ref = ((Button)sender).Text;
            LoadSelectedRef(Ref);
        }
        void LoadSelectedRef(string Ref)
        {
            //var progressDialog = ProgressDialog.Show(this, null, "Please Wait.", true);
            new Thread(new ThreadStart(() =>
            {
                var list = repo.GetProductionList(bitopiApplication.User.UserCode, DBAccess.Database.RecentHistory.Result.ProcessID,
                      DBAccess.Database.RecentHistory.Result.LocationID, SelectedPRStatus, 3, Ref);
                if (list.Count == 0) return;
                ProdcutionAccountingDBModel model = list.First();
                RunOnUiThread(() =>
                {
                    tvOrderQty.Text = model.OrderQty.ToString("N0");
                    tvBalanceQty.Text = model.BalanceQty.ToString("N0");
                    tvProducedQty.Text = model.ProducedQty.ToString("N0");
                    txtWIPQty.Text = model.WIP.ToString("N0");

                    atvReference.Text = tvRef.Text = Ref;
                    //progressDialog.Dismiss();
                });
            })).Start();
        }
        private void LoadPR_Click(object sender, EventArgs e)
        {
            SelectedPRStatus = ((Button)sender).Text;
            LoadList(SelectedPRStatus, () =>
            {
                isListShown = true;
                Animation bottomUp = Android.Views.Animations.AnimationUtils.LoadAnimation(this,
            Resource.Animation.bottom_up);
                rlPRLV.Visibility = (ViewStates.Visible);
                lvProduct.StartAnimation(bottomUp);
            });

        }

        //private void Ptr_view_RefreshActivated(object sender, EventArgs e)
        //{
        //    LoadList();
        //    if (ptr_view != null)
        //    {
        //        ptr_view.OnRefreshCompleted();
        //    }
        //}

        private void BtnMinus_Click(object sender, EventArgs e)
        {
            int qty = Convert.ToInt16(etQty.Text == "" ? "0" : etQty.Text);
            qty -= 10;
            if (qty >= 0)
                etQty.Text = qty.ToString();
        }

        private void BtnPlus_Click(object sender, EventArgs e)
        {
            int qty = Convert.ToInt16(etQty.Text == "" ? "0" : etQty.Text);
            qty += 10;
            etQty.Text = qty.ToString();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {

            if (atvReference.Text == "")
            {
                Toast.MakeText(this, "Please Select an item first", ToastLength.Long).Show();
                return;
            }

            List<RecentPR> prs = DBAccess.Database.RecentPRs.Result.Where(t => t.EntryType == (int)EntryType.Rejection).ToList();
            if (prs.Where(t => t.RefID == atvReference.Text).Count() == 0)
            {
                DBAccess.Database.SaveRecentPR(new DAL.RecentPR { RefID = atvReference.Text, LocationRef = tvLocation.Text, EntryType = (int)EntryType.Rejection });
                prs = DBAccess.Database.RecentPRs.Result.Where(t => t.EntryType == (int)EntryType.Rejection).ToList();
                if (prs.Count == 6)
                {
                    DBAccess.Database.DeleteItemAsync(prs.Where(t => t.ID == 1).FirstOrDefault());
                    foreach (var pr in prs)
                    {
                        pr.ID = pr.ID - 1;
                        DBAccess.Database.SaveRecentPR(pr);
                    }
                }
                PopulateRecentItem();
            }
            ProdcutionAccountingDBModel model;
            if (list != null)
                model = list.Where(t => t.RefNo == atvReference.Text).First();
            else
                model = new ProdcutionAccountingDBModel { LocationRef = DBAccess.Database.RecentPRs.Result.Where(t => t.RefID == atvReference.Text).First().LocationRef };
            //gifView.Visibility = ViewStates.Visible;

            var progressDialog = ProgressDialog.Show(this, null, "Please Wait.", true);
            var refid = atvReference.Text;
            var qty = Convert.ToInt16(etQty.Text);
            var userCode = bitopiApplication.User.UserCode;
            //new Thread(new ThreadStart(() =>
            //{

            ProductionRejectionDBModel dbmodel = new ProductionRejectionDBModel();
            dbmodel.RefNo = atvReference.Text;
            dbmodel.LocationRef = DBAccess.Database.RecentHistory.Result.LocationID;
            dbmodel.Grade = etGrade.Text;
            //dbmodel.SKUCode = Convert.ToInt16(etSKUCode.Text);
            dbmodel.ProducedQty = Convert.ToInt16(etQty.Text);
            dbmodel.AddedBy = bitopiApplication.User.UserCode;
            var result = repo.SetRejection(dbmodel);

            RunOnUiThread(() =>
            {
                if (result > 0)
                {
                    if (SelectedPRStatus != "")
                    {
                        LoadList(SelectedPRStatus);
                    }
                    LoadSelectedList(atvReference.Text);
                    progressDialog.Dismiss();
                    Toast.MakeText(this, "Successfully Saved", ToastLength.Long).Show();

                }
                else
                {
                    progressDialog.Dismiss();
                    Toast.MakeText(this, "Unsuccessfull operation", ToastLength.Long).Show();
                }
                //gifView.Visibility = ViewStates.Gone;
            });
            //})).Start();
        }
        public void LoadSelectedList(string Ref)
        {
            new Thread(new ThreadStart(() =>
            {
                var prodList = repo.GetProductionList(bitopiApplication.User.UserCode, DBAccess.Database.RecentHistory.Result.ProcessID,
                     DBAccess.Database.RecentHistory.Result.LocationID, "", 3, Ref);
                RunOnUiThread(() =>
                {
                    var m = prodList.First();
                    tvOrderQty.Text = m.OrderQty.ToString("N0");
                    tvBalanceQty.Text = m.BalanceQty.ToString("N0");
                    tvProducedQty.Text = m.ProducedQty.ToString("N0");
                    txtWIPQty.Text = m.WIP.ToString("N0");
                });
            })).Start();

        }
        private void LvProduct_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Animation bottomUp = Android.Views.Animations.AnimationUtils.LoadAnimation(this,
            Resource.Animation.bottom_down);
            lvProduct.StartAnimation(bottomUp);
            rlPRLV.Visibility = (ViewStates.Gone);
            var model = list[e.Position];
            tvRef.Text = atvReference.Text = model.RefNo;
            tvOrderQty.Text = model.OrderQty.ToString("N0");
            tvBalanceQty.Text = model.BalanceQty.ToString("N0");
            tvProducedQty.Text = model.ProducedQty.ToString("N0");
            //tvLocation.Text = model.LocationName;
            isListShown = false;
        }
    }
}