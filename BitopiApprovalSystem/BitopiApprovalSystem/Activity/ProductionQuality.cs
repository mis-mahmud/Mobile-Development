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
using PullToRefresharp.Android.Views;
using System.Threading;
using BitopiApprovalSystem.DAL;
using Android.Support.V4.View;
using Android.Graphics;
using Android.Util;
using Android.Views.Animations;
using Android.Text;
using System.Threading.Tasks;
using Android.Views.InputMethods;
using Android.Graphics.Drawables;

namespace BitopiApprovalSystem
{
    [Activity(Label = "ProductionQuality", WindowSoftInputMode = SoftInput.AdjustPan)]
    public class ProductionQuality : BaseActivity
    {
        PullToRefresharp.Android.Widget.ScrollView ptr;
        ProductionRepository repo ;
        ListView lvProduct, lvDefect, lvOperation;
        TextView tvLocation, etSample;
        TextView tvRef, tvOrderQty, tvBalanceQty, tvProducedQty, txtWIPQty;
        EditText etQty, etLotQ, etCheck, etDefectiveUnit;
        Button btnSave, btnPlus, btnMinus;
        AutoCompleteTextView atvReference;
        public ProductionAccountigListAdapter adapter;
        public List<ProductionAccountingDBModel> list;
        public OperationAdapter operationAdapter;
        public List<Operation> OperationList;
        RelativeLayout gifView;
        private IPullToRefresharpView ptr_view;
        RelativeLayout rlTop;
        RelativeLayout rltitle, rlPRLV, rlPOPUPOperation;
        Button recent1, recent2, recent3, recent4, recent5;
        Button btnAll, btnRunning;
        string SelectedPRStatus = "";
        DefectMastAdapter defectAdapter;
        TextView spStatus;
        List<DefectMaster> defectList;
        public static string[] EntryTypeArray = { "Pass", "Fail" };
        bool isListShown, isPopupShown;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            repo = new ProductionRepository(ShowLoader,HideLoader);
            SupportActionBar.SetDisplayShowCustomEnabled(true);
            SupportActionBar.SetCustomView(Resource.Layout.custom_actionbar);
            SetContentView(Resource.Layout.ProductionQuality);
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
        protected async override void OnStart()
        {
            base.OnStart();
            InitializeEvent();
            defectList =await repo.GetGetDefectList();
            defectAdapter.Items = defectList;
            defectAdapter.NotifyDataSetChanged();
                
                //gifView.Visibility = ViewStates.Visible;

                //gifView.Visibility = ViewStates.Gone;
            
        }
       async void LoadList(string PRStatus, Action task = null)
        {
           // var progressDialog = ProgressDialog.Show(this, null, "Please Wait.", true);
            //new Thread(new ThreadStart(() =>
            //{
                list =await repo.GetProductionList(bitopiApplication.User.UserCode, DBAccess.Database.RecentHistory.Result.ProcessID,
                    DBAccess.Database.RecentHistory.Result.LocationID, PRStatus, 2);
                RunOnUiThread(() =>
                {

                    atvReference.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, list.Select(t => t.RefNo).ToArray());
                    adapter.Items = list;
                    adapter.NotifyDataSetChanged();
                    //progressDialog.Dismiss();
                    //AndHUD.Shared.Dismiss();
                    if (task != null)
                    {
                        if (list.Count > 0) task();
                        else
                            Toast.MakeText(this, "No Data Found", ToastLength.Long).Show();
                    }
                    Log.Debug("", list.Count().ToString());
                });
           // })).Start();
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
                 
                _searchView.QueryTextChange += (s, e) =>
                {

                    adapter.Filter.InvokeFilter(e.NewText);
                    
                };

                 
                _searchView.SetOnSearchClickListener(new ProductionSearchviewclicklistener(rltitle));
 
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
            rlPOPUPOperation = FindViewById<RelativeLayout>(Resource.Id.rlPOPUpOperation);
            rlTop = FindViewById<RelativeLayout>(Resource.Id.rlTop);
            lvProduct = FindViewById<ListView>(Resource.Id.lvProduct);
            lvOperation = FindViewById<ListView>(Resource.Id.lvOperation);
            lvDefect = FindViewById<ListView>(Resource.Id.lvDefect);
            adapter = new ProductionAccountigListAdapter(list, this);
            lvProduct.Adapter = (adapter);

            operationAdapter = new BitopiApprovalSystem.OperationAdapter(OperationList, this);
            lvOperation.Adapter = operationAdapter;

            defectAdapter = new BitopiApprovalSystem.DefectMastAdapter(defectList, this, ShowOperationPopup);
            lvDefect.Adapter = defectAdapter;


            gifView = FindViewById<RelativeLayout>(Resource.Id.gifview);
            tvLocation = FindViewById<TextView>(Resource.Id.tvLocation);
            tvRef = FindViewById<TextView>(Resource.Id.tvRef);
            tvOrderQty = FindViewById<TextView>(Resource.Id.txtOrderQty);
            tvBalanceQty = FindViewById<TextView>(Resource.Id.txtBalanceQty);
            tvProducedQty = FindViewById<TextView>(Resource.Id.txtProduceQty);
            txtWIPQty = FindViewById<TextView>(Resource.Id.txtWIPQty);
            etQty = FindViewById<EditText>(Resource.Id.etQty);
            etLotQ = FindViewById<EditText>(Resource.Id.etLot);
            etSample = FindViewById<TextView>(Resource.Id.etSample);
            etCheck = FindViewById<EditText>(Resource.Id.etCheck);
            etDefectiveUnit = FindViewById<EditText>(Resource.Id.etDU);
            btnSave = FindViewById<Button>(Resource.Id.btnSubmit);

            btnPlus = FindViewById<Button>(Resource.Id.btnPlus);
            btnMinus = FindViewById<Button>(Resource.Id.btnMinus);
            btnAll = FindViewById<Button>(Resource.Id.btnAll);
            btnRunning = FindViewById<Button>(Resource.Id.btnRunning);
            FindViewById<TextView>(Resource.Id.tvHeaderName).Text =
             Html.FromHtml(DBAccess.Database.RecentHistory.Result.Process + @"\Quality").ToString();
            tvLocation.Text = DBAccess.Database.RecentHistory.Result.Location;

            recent1 = FindViewById<Button>(Resource.Id.recent1);
            recent2 = FindViewById<Button>(Resource.Id.recent2);
            recent3 = FindViewById<Button>(Resource.Id.recent3);
            recent4 = FindViewById<Button>(Resource.Id.recent4);
            recent5 = FindViewById<Button>(Resource.Id.recent5);
            spStatus = FindViewById<TextView>(Resource.Id.spStatus);
            //spStatus.Adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, EntryTypeArray);
            base.InitializeControl();
        }
        public void PopulateRecentItem()
        {
            GradientDrawable shape = new GradientDrawable();
            shape.SetCornerRadius(6);
            List<RecentPR> prs = DBAccess.Database.RecentPRs.Result.Where(t => t.EntryType == (int)EntryType.Quality &&
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
            lvOperation.ItemClick += LvOperation_ItemClick;
            btnSave.Click -= BtnSave_Click;
            btnSave.Click += BtnSave_Click;

            btnAll.Click += LoadPR_Click;
            btnRunning.Click += LoadPR_Click;
            recent1.Click += Recent_Click;
            recent2.Click += Recent_Click;
            recent3.Click += Recent_Click;
            recent4.Click += Recent_Click;
            recent5.Click += Recent_Click;
            etLotQ.TextChanged += (e, r) =>
            {
                EditText etNO = ((EditText)e);
                if (etNO.Text != "")
                {
                    new Thread(new ThreadStart(() =>
                    {
                        string Sample = repo.GetAQL(atvReference.Text, Convert.ToInt32(etNO.Text));

                        RunOnUiThread(() =>
                        {
                            etSample.Text = Sample;
                        });
                    })).Start();
                }

            };
            etDefectiveUnit.TextChanged += (e, r) =>
            {
                EditText detDefect = ((EditText)e);
                if (detDefect.Text != "")
                {
                    new Thread(new ThreadStart(() =>
                {
                    string status = repo.GetAQL(atvReference.Text, Convert.ToInt32(etLotQ.Text), Convert.ToInt32(detDefect.Text));

                    RunOnUiThread(() =>
                    {
                        spStatus.Text = status;
                        if (status == "Fail")
                        {
                            spStatus.SetBackgroundResource(Resource.Drawable.rounded_textview_error);
                            spStatus.SetTextColor(Color.Red);
                        }
                        else
                        {
                            spStatus.SetBackgroundResource(Resource.Drawable.rounded_button_pass);
                            spStatus.SetTextColor(Color.Green);
                        }
                    });
                })).Start();
                }

            };
            //if (ptr_view == null && ptr is IPullToRefresharpView)
            //{
            //    ptr_view = (IPullToRefresharpView)ptr;
            //    ptr_view.RefreshActivated += Ptr_view_RefreshActivated; ;
            //}
            rlPRLV.Click += RlPRLV_Click;
            base.InitializeEvent();
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
                      DBAccess.Database.RecentHistory.Result.LocationID, SelectedPRStatus, 2, Ref).Result;
                if (list.Count == 0) return;
                ProductionAccountingDBModel model = list.First();
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
                Animation bottomUp = Android.Views.Animations.AnimationUtils.LoadAnimation(this,
            Resource.Animation.bottom_up);
                rlPRLV.Visibility = (ViewStates.Visible);
                lvProduct.StartAnimation(bottomUp);
                isListShown = true;
            });
        }
        private void RlPRLV_Click(object sender, EventArgs e)
        {
            Animation bottomUp = Android.Views.Animations.AnimationUtils.LoadAnimation(this,
             Resource.Animation.bottom_down);

            lvProduct.StartAnimation(bottomUp);
            bottomUp.AnimationEnd += (s, er) =>
            {
                rlPRLV.Visibility = (ViewStates.Gone);
            };
            isListShown = false;

        }

        private void LvProduct_AnimationEnd(object sender, Animation.AnimationEndEventArgs e)
        {

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

            List<RecentPR> prs = DBAccess.Database.RecentPRs.Result.Where(t => t.EntryType == (int)EntryType.Quality).ToList();
            if (prs.Where(t => t.RefID == atvReference.Text).Count() == 0)
            {
                DBAccess.Database.SaveRecentPR(new DAL.RecentPR { RefID = atvReference.Text, LocationRef = tvLocation.Text, EntryType = (int)EntryType.Quality });
                prs = DBAccess.Database.RecentPRs.Result.Where(t => t.EntryType == (int)EntryType.Quality).ToList();
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
            //ProdcutionAccountingDBModel model;
            //if (list != null)
            //    model = list.Where(t => t.RefNo == atvReference.Text).First();
            //else
            //    model = new ProdcutionAccountingDBModel { LocationRef = DBAccess.Database.RecentPRs.Result.Where(t => t.RefID == atvReference.Text).First().LocationRef };
            ////gifView.Visibility = ViewStates.Visible;

            var progressDialog = ProgressDialog.Show(this, null, "Please Wait.", true);

            //var qty = Convert.ToInt16(etQty.Text);
            var userCode = bitopiApplication.User.UserCode;
            ProductionQualityDBModel model = new ProductionQualityDBModel();
            model.LocationRef = DBAccess.Database.RecentHistory.Result.LocationID;
            model.RefNo = atvReference.Text;
            model.LotQ = Convert.ToInt16(etLotQ.Text);
            model.Sample = Convert.ToInt16(etSample.Text);
            model.Check = etSample.Text;
            model.DefectiveUnit = Convert.ToInt16(etDefectiveUnit.Text);
            model.QualityStatus = spStatus.Text.ToString();
            model.DefectList = defectList;
            model.AddedBy = bitopiApplication.User.UserCode;
            //new Thread(new ThreadStart(() =>
            //{

            //var result = repo.SetProduction(refid,
            //qty, model.LocationRef, userCode);
            var result = repo.SetQuality(model);

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
        string selectedDefectCode = "";
        public void ShowOperationPopup(string DefectCode)
        {
            if (atvReference.Text != "")
            {
                InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Context.InputMethodService);
                inputManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
                isPopupShown = true;
                selectedDefectCode = DefectCode;
                rlPOPUPOperation.Visibility = ViewStates.Visible;
                //var progressDialog = ProgressDialog.Show(this, null, "Please Wait.", true);
                new Thread(new ThreadStart(() =>
                {
                    OperationList = repo.GetOperationList(atvReference.Text).Result;
                    RunOnUiThread(() =>
                    {
                        operationAdapter.Items = OperationList;
                        operationAdapter.NotifyDataSetChanged();
                    //progressDialog.Dismiss();
                });
                })).Start();
            }
            else
            {
                Toast.MakeText(this, "Please Select a Reference First", ToastLength.Long).Show();
            }
        }
        public override void OnBackPressed()
        {
            if (isPopupShown)
            {
                rlPOPUPOperation.Visibility = ViewStates.Gone;
                isPopupShown = false;
            }
            else if (isListShown)
            {
                Animation bottomUp = Android.Views.Animations.AnimationUtils.LoadAnimation(this,
                Resource.Animation.bottom_down);

                lvProduct.StartAnimation(bottomUp);
                bottomUp.AnimationEnd += (s, er) =>
                {
                    rlPRLV.Visibility = (ViewStates.Gone);
                };
                isListShown = false;
            }
            else
            {
                base.OnBackPressed();
            }
        }
        public void LoadSelectedList(string Ref)
        {
            new Thread(new ThreadStart(() =>
            {
                var prodList = repo.GetProductionList(bitopiApplication.User.UserCode, DBAccess.Database.RecentHistory.Result.ProcessID,
                     DBAccess.Database.RecentHistory.Result.LocationID, "", 2, Ref).Result;
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
            bottomUp.AnimationEnd += (s, er) =>
            {
                rlPRLV.Visibility = (ViewStates.Gone);
            };
            var model = list[e.Position];
            tvRef.Text = atvReference.Text = model.RefNo;
            tvOrderQty.Text = model.OrderQty.ToString("N0");
            tvBalanceQty.Text = model.BalanceQty.ToString("N0");
            tvProducedQty.Text = model.ProducedQty.ToString("N0");

            //tvLocation.Text = model.LocationName;
            isListShown = false;

        }
        private void LvOperation_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                string operationCode = OperationList[e.Position].OperationCode;
                defectList.Where(t => t.DefectCode == selectedDefectCode).First().OperationCode = operationCode;
                defectAdapter.Items = defectList;
                defectAdapter.NotifyDataSetChanged();
                rlPOPUPOperation.Visibility = ViewStates.Gone;
                isPopupShown = false;
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

        }
    }

    public class DefectMastAdapter : BaseAdapter
    {
        List<DefectMaster> _list;
        Context _context;
        Filter filter;
        Action<string> _CallBack;
        public DefectMastAdapter(List<DefectMaster> list, Context context, Action<string> CallBack)
        {
            _list = list;
            _context = context;
            _CallBack = CallBack;

        }
        public List<DefectMaster> Items
        {
            set { _list = value; }
            get { return _list; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            var model = Items[position];
            // if (view == null)
            //{
            view = LayoutInflater.From(_context).Inflate(Resource.Layout.DefectItem, parent, false);
            // }
            view.FindViewById<TextView>(Resource.Id.tvDC).Text = model.DefectCode;
            view.FindViewById<TextView>(Resource.Id.tvDZ).Text = model.DefectName;
            //view.FindViewById<TextView>(Resource.Id.tvOC).Text = model.OperationCode;
            view.FindViewById<TextView>(Resource.Id.tvCategory).Text = model.Category;
            view.FindViewById<TextView>(Resource.Id.etNO).Text = model.No > 0 ? model.No.ToString() : "";
            view.FindViewById<TextView>(Resource.Id.etNO).TextChanged -= DefectMastAdapter_TextChanged;
            view.FindViewById<TextView>(Resource.Id.etNO).TextChanged += DefectMastAdapter_TextChanged;
            view.FindViewById<TextView>(Resource.Id.etNO).Tag = model.DefectCode;
            view.FindViewById<Button>(Resource.Id.tvOC).Text = model.OperationCode;
            view.FindViewById<Button>(Resource.Id.tvOC).Click += tvOC_Click;
            view.FindViewById<Button>(Resource.Id.tvOC).Tag = model.DefectCode;
            if (position % 2 == 0)
                view.SetBackgroundColor(Color.ParseColor("#aaaaaa"));
            else
                view.SetBackgroundColor(Color.ParseColor("#ffffff"));
            return view;
        }

        private void tvOC_Click(object sender, EventArgs e)
        {
            string tag = ((Button)(sender)).Tag.ToString();
            _CallBack(tag);
        }

        private void DefectMastAdapter_TextChanged(object sender, TextChangedEventArgs e)
        {
            EditText etNO = ((EditText)sender);
            var DefectCode = etNO.Tag.ToString();
            var item = _list.Where(t => t.DefectCode == DefectCode).FirstOrDefault();
            if (etNO.Text != "")
                item.No = Convert.ToInt32(etNO.Text);

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

    }
    public class OperationAdapter : BaseAdapter
    {
        List<Operation> _list;
        Context _context;
        Filter filter;
        public OperationAdapter(List<Operation> list, Context context)
        {
            _list = list;
            _context = context;

        }
        public List<Operation> Items
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
                view = LayoutInflater.From(_context).Inflate(Resource.Layout.OperationItem, parent, false);
            }
            TextView tvOperationCode = view.FindViewById<TextView>(Resource.Id.lblOperationCode);
            if (tvOperationCode != null)
                tvOperationCode.Text = model.OperationCode;
            TextView lblOperationName = view.FindViewById<TextView>(Resource.Id.lblOperationName);
            if (lblOperationName != null)
                lblOperationName.Text = model.OperationName; ;

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

    }
}