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
using Android.Support.V7.App;
using BitopiApprovalSystem.Model;
using ApiRepository;
using BitopiApprovalSystem.Widget;
using Android.Text;
using System.Threading.Tasks;
using Android.Graphics;
using System.Threading;
using Android.Support.V4.View;
using SearchView = Android.Support.V7.Widget.SearchView;
using Android.Views.InputMethods;
using BitopiApprovalSystem.BitopiPushNotification;
using BitopiApprovalSystem.Library;
using Android.Support.V4.Widget;
using BitopiApprovalSystem.DAL;

namespace BitopiApprovalSystem
{
    [Activity(Label = "ApprovalDetailActivity")]
    public class ApprovalDetailActivity : BaseActivity
    {
        AnimatedExpandableListView lvApprovalDetailList;
        ApprovalDetalisListAdapter _approvalListAdapter;
        ApprovalType _approvalType;
        ApprovalRoleType _approvalRoleType;
        CheckBox _chkApproveAll;
        ApprovalDetailList _approvalList;
        int lastExpandedGroupPosition = -1;
        Android.App.AlertDialog.Builder builder;
        ApprovalRepository repo;
        public RelativeLayout rlMsg;
        public RelativeLayout rlapprovalDetail;
        RelativeLayout rltitle;
        
        RelativeLayout RLleft_drawer;
        private DrawerLayout mDrawerLayout;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            
            builder = new Android.App.AlertDialog.Builder(this);
            builder.SetMessage("Hello, World!");

            builder.SetNegativeButton("Cancel", (s, e) => { /* do something on Cancel click */ });


            base.OnCreate(savedInstanceState);
            _approvalList = new ApprovalDetailList(this);
            _approvalType = (ApprovalType)(Convert.ToInt16(Intent.GetStringExtra("ApprovalType")));
            _approvalRoleType = (ApprovalRoleType)(Convert.ToInt16(Intent.GetStringExtra("ApprovalRoleType")));

            SupportRequestWindowFeature(WindowCompat.FeatureActionBar);
            SupportActionBar.SetDisplayShowCustomEnabled(true);
            SupportActionBar.SetCustomView(Resource.Layout.custom_actionbar);
            rltitle = FindViewById<RelativeLayout>(Resource.Id.rltitle);


            SetContentView(Resource.Layout.ApprovalDetailList);

            lvApprovalDetailList = FindViewById<AnimatedExpandableListView>(Resource.Id.lvApprovalsDetails);
            _chkApproveAll = FindViewById<CheckBox>(Resource.Id.chkSelectAll);
            rlMsg = FindViewById<RelativeLayout>(Resource.Id.rlMsg);
            rlapprovalDetail = FindViewById<RelativeLayout>(Resource.Id.rlapprovalDetail);

            if (bitopiApplication.ApprovalRoleType == ApprovalRoleType.Recommend)
            {
                (FindViewById<Button>(Resource.Id.btnApproveAll)).Text = "RECOMMEND SELECTED";
                (FindViewById<Button>(Resource.Id.btnNotApproveAll)).Text = "NOT RECOMMEND SELECTED";
            }
            else
            {
                (FindViewById<Button>(Resource.Id.btnApproveAll)).Text = "APPROVE SELECTED";
                (FindViewById<Button>(Resource.Id.btnNotApproveAll)).Text = "NOT APPROVE SELECTED";
            }
            (FindViewById<Button>(Resource.Id.btnApproveAll)).Click += (s, e) =>
            {
                builder.SetMessage("Do you want to " + (bitopiApplication.ApprovalRoleType == ApprovalRoleType.Approve ? "Approve" : "Recommend ") + _approvalList.Where(t => t.isApproved == true).Count() + " Approval");

                builder.SetPositiveButton("OK", (sender, evnt) =>
                {
                    var progressDialog = ProgressDialog.Show(this, null, "", true);
                    _approvalList.SaveSelected((bitopiApplication.ApprovalRoleType == ApprovalRoleType.Approve ? "Approved" : "Recommended"), (numberOfSuccessFullOperation) =>
                    {
                        progressDialog.Dismiss();
                        var tempApprovalList = _approvalList.Where(t => t.isDeleted == false).ToList();
                        _approvalList = new ApprovalDetailList(this);
                        tempApprovalList.ForEach(st => _approvalList.Add(st));
                        _approvalListAdapter.SetData(_approvalList);
                        _approvalListAdapter.NotifyDataSetChanged();
                        Toast.MakeText(this, "Total " + numberOfSuccessFullOperation + " Approval has been " + ((bitopiApplication.ApprovalRoleType == ApprovalRoleType.Approve) ? "Approved" : "Recommended"), ToastLength.Long).Show();
                        if (numberOfSuccessFullOperation > 0 && _approvalList.Count == 0)
                        {
                            rlMsg.Visibility = ViewStates.Visible;
                            rlapprovalDetail.Visibility = ViewStates.Gone;
                        }

                    });

                    /* do something on OK click */
                });
                builder.Create().Show();
            };
            (FindViewById<Button>(Resource.Id.btnNotApproveAll)).Click += (s, e) =>
            {
                builder.SetMessage("Do you want to " + ((bitopiApplication.ApprovalRoleType == ApprovalRoleType.Approve) ? "Reject" : "Not Recommend ") + _approvalList.Where(t => t.isApproved == true).Count() + " Approval");

                builder.SetPositiveButton("OK", (sender, evnt) =>
                {
                    var progressDialog = ProgressDialog.Show(this, null, "", true);
                    _approvalList.SaveSelected((bitopiApplication.ApprovalRoleType == ApprovalRoleType.Approve) ? "Rejected" : "NotRecommend", (numberOfSuccessFullOperation) =>
                    {
                        progressDialog.Dismiss();
                        var tempApprovalList = _approvalList.Where(t => t.isDeleted == false).ToList();
                        _approvalList = new ApprovalDetailList(this);
                        tempApprovalList.ForEach(st => _approvalList.Add(st));
                        _approvalListAdapter.SetData(_approvalList);
                        _approvalListAdapter.NotifyDataSetChanged();
                        Toast.MakeText(this, "Total " + numberOfSuccessFullOperation + " Approval has been " + ((bitopiApplication.ApprovalRoleType == ApprovalRoleType.Approve) ? "Rejected" : "Not Recommended"), ToastLength.Long).Show();
                        if (numberOfSuccessFullOperation > 0 && _approvalList.Count == 0)
                        {
                            rlMsg.Visibility = ViewStates.Visible;
                            rlapprovalDetail.Visibility = ViewStates.Gone;
                        }
                    });
                });
                builder.Create().Show();
            };
            RLleft_drawer = FindViewById<RelativeLayout>(Resource.Id.RLleft_drawer);
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            FindViewById<ImageButton>(Resource.Id.btnDrawermenu).Visibility = ViewStates.Visible;
            FindViewById<ImageButton>(Resource.Id.btnDrawermenu).Click += (s, e) =>
            {
                if (mDrawerLayout.IsDrawerOpen(RLleft_drawer))
                {
                    mDrawerLayout.CloseDrawer(RLleft_drawer);
                }
                else
                {
                    mDrawerLayout.OpenDrawer(RLleft_drawer);
                }
            };

            //base.LoadDrawerView();
        }
        protected async override void OnStart()
        {
            bitopiApplication.CurrentActivity = "Approval Detail Activity";
            base.OnStart();
            var progressDialog = ProgressDialog.Show(this, null, "", true);
            repo = new ApprovalRepository();

            List<ApprovalDetailsModel> aprovalList = await repo.GetPOApprovalDetails(bitopiApplication.User.UserCode, bitopiApplication.ApprovalRoleType,
                bitopiApplication.ApprovalType);
            _approvalList.Clear();
            aprovalList.ForEach(s => _approvalList.Add(s));
            _approvalListAdapter = new ApprovalDetalisListAdapter(_approvalList, lvApprovalDetailList, this);
            lvApprovalDetailList.SetAdapter(_approvalListAdapter);
            _approvalListAdapter.NotifyDataSetChanged();
            _chkApproveAll.Click += (s, e) =>
            {
                _approvalList.ForEach(t => t.isApproved = _chkApproveAll.Checked);
                _approvalListAdapter.SetData(_approvalList);
                _approvalListAdapter.NotifyDataSetChanged();
            };
            lvApprovalDetailList.SetGroupIndicator(null);
            progressDialog.Dismiss();
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


                int searchPlateId = mSearchView.Context.Resources.GetIdentifier("android:id/search_plate", null, null);
                EditText plate = (EditText)mSearchView.FindViewById(searchPlateId);
                //plate.SetBackgroundColor(Color.White);
                plate.SetTextColor(Color.White);
                int voiceSearchPlateId = mSearchView.Context.Resources.GetIdentifier("android:id/submit_area", null, null);
                mSearchView.FindViewById(voiceSearchPlateId).SetBackgroundResource(Resource.Drawable.rounded_textview);

                // change hint color
                int searchTextViewId = mSearchView.Context.Resources.GetIdentifier("android:id/search_src_text", null, null);
                TextView searchTextView = (TextView)mSearchView.FindViewById(searchTextViewId);
                searchTextView.SetHintTextColor(Resources.GetColor(Color.LightGray));


            }
            catch (Exception ex)
            {
                string dst = "sdfsfs";
                CustomLogger.CustomLog("From Activity: " + bitopiApplication.CurrentActivity + "\nMessage: " + ex.Message + "\nStack Trace: " + ex.StackTrace + "\n\n", "", bitopiApplication.User != null ?
                         bitopiApplication.User.UserName : "");
            }
        }
        IMenuItem menuitem;
        SearchView _searchView;
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.optionmenu, menu);
            menuitem = menu.FindItem(Resource.Id.action_search);
            var test = new SearchView(this);

            menuitem.SetActionView(test);
            var searchView = MenuItemCompat.GetActionView(menuitem);

            int searchImgId = Resources.GetIdentifier("android:id/search_button", null, null);
            ImageView v = (ImageView)searchView.FindViewById(searchImgId);
            //v.SetImageResource(Resource.Drawable.abc_ic_search);

            _searchView = searchView.JavaCast<SearchView>();

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
                var searchFor = e.NewText.ToLower();
                if (!String.IsNullOrEmpty(searchFor))
                {
                    //Console.System.Diagnostics.Debug.WriteLine("searchFor:" + searchFor);
                    // var matchList = new List<TruckModel>();
                    // find matches, IndexOf means look for the input anywhere in the items
                    // but it isn't case-sensitive by default!
                    var matches = (from i in _approvalList where i.POID.ToLower().Contains(searchFor) select i).ToList();
                    //foreach (var match in matches) {
                    //matchList.Add (match);
                    //}
                    ApprovalDetailList list = new ApprovalDetailList(this);

                    matches.ForEach(model => list.Add(model));
                    _approvalListAdapter.SetData(list);
                    _approvalListAdapter.NotifyDataSetChanged();
                    //if (_searchListView.Visibility == ViewStates.Gone && _searchView.Iconified == false)
                    //{
                    //    //_searchListView.Visibility = ViewStates.Visible;
                    //    _searchView.Iconified = true;
                    //}
                }
                else
                {
                    _approvalListAdapter.SetData(_approvalList);
                    _approvalListAdapter.NotifyDataSetChanged();
                }

            };
            _searchView.SetOnQueryTextFocusChangeListener(new CustomSearchViewOnFocusChangeListenter(this));
            _searchView.SetOnSearchClickListener(new CustomSearchviewclicklistener(rltitle));
            _searchView.SetOnCloseListener(new CustomSearchViewOnCloseListenter(rltitle, _approvalListAdapter, _approvalList));
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.refresh:
                    var progressDialog = ProgressDialog.Show(this, null, "", true);
                    new Thread(new ThreadStart(() =>
                    {
                        repo = new ApprovalRepository();

                        List<ApprovalDetailsModel> aprovalList = repo.GetPOApprovalDetails(bitopiApplication.User.UserCode, bitopiApplication.ApprovalRoleType,
                            bitopiApplication.ApprovalType).Result;

                        _approvalList.Clear();
                        aprovalList.ForEach(s => _approvalList.Add(s));


                        RunOnUiThread(() =>
                        {
                            _approvalListAdapter.NotifyDataSetChanged();
                            progressDialog.Dismiss();
                        });
                    })).Start();
                    return true;
                case Resource.Id.about:
                    bitopiApplication.ShowAboutDialog(this);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        void LoadDrawerView()
        {
            FindViewById<TextView>(Resource.Id.tvUserName).Text = bitopiApplication.User.EmployeeName;
            if (bitopiApplication.User.EmpImage.Length > 0)
            {
                BitmapFactory.Options options = new BitmapFactory.Options();
                options.InSampleSize = 4;
                Bitmap bitmap = BitmapFactory.DecodeByteArray(bitopiApplication.User.EmpImage, 0,
                    bitopiApplication.User.EmpImage.Length, options);
                FindViewById<ImageView>(Resource.Id.ivUserImg).SetImageBitmap(bitmap);
            }
            FindViewById<Button>(Resource.Id.btnLogout).Click += (s, e) =>
            {


                var progressDialog = ProgressDialog.Show(this, null, "", true);
                new System.Threading.Thread(new System.Threading.ThreadStart(() =>
                {
                    AccountRepository repo = new AccountRepository();
                    var resutl = repo.getUser("", "",
                     bitopiApplication.MacAddress,
                     "",
                     "",
                     "android", 2, bitopiApplication.CurrentVersion, bitopiApplication.User.UserCode).Result;
                    ISharedPreferences pref = Application.Context.GetSharedPreferences("_bitopi_UserInfo", FileCreationMode.Private);
                    pref.Edit().Clear().Commit();
                    bitopiApplication.ClearData();
                    DBAccess.Database.DropAllTable();
                    RunOnUiThread(() =>
                    {
                        progressDialog.Dismiss();
                        Intent i = new Intent(this, typeof(LoginActivity));
                        i.SetFlags(ActivityFlags.ClearTask);
                        StartActivity(i);
                        Finish();
                    });
                })).Start();
            };
            FindViewById<RelativeLayout>(Resource.Id.rlmenuapproval).Click += (s, e) =>
            {
                Intent i = new Intent(this, typeof(ApprovalActivity));

                StartActivity(i);
            };
            FindViewById<RelativeLayout>(Resource.Id.rlmenumytask).Click += (s, e) =>
            {
                Intent i = new Intent(this, typeof(MyTaskMenu));

                StartActivity(i);
            };
        }
        public class ApprovalDetalisListAdapter : AnimatedExpandableListView.AnimatedExpandableListAdapter
        {
            ApprovalDetailList _approvalList;
            ApprovalDetailList MatchItems;
            ApprovalDetailActivity _context;
            AnimatedExpandableListView lvApprovalDetailList;
            BitopiApplication bitopiApplication;
            public ApprovalDetalisListAdapter(ApprovalDetailList approvalList, AnimatedExpandableListView _lvApprovalDetailList, ApprovalDetailActivity _activity) : base(_activity)
            {
                MatchItems = _approvalList = approvalList;
                _context = _activity;
                lvApprovalDetailList = _lvApprovalDetailList;
                bitopiApplication = (BitopiApplication)_context.ApplicationContext;


            }
            public void SetData(ApprovalDetailList approvalList)
            {

                MatchItems = _approvalList = approvalList;
            }
            public override int GroupCount
            {
                get
                {
                    return MatchItems != null ? MatchItems.Count : 0;
                }
            }

            int lastExpandedGroupPosition = -1;
            public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
            {
                ApprovalDetailsModel model = MatchItems[groupPosition];
                View view;
                if (convertView == null)
                {
                    view = LayoutInflater.From(_context).Inflate(Resource.Layout.ApprovalDetailsListRow, parent, false);



                }
                else
                {
                    view = convertView;
                }
                (view.FindViewById<CheckBox>(Resource.Id.chckApprove)).Click -= chkSelect;
                (view.FindViewById<CheckBox>(Resource.Id.chckApprove)).Click += chkSelect;
                view.Click -= RowClickListener;
                view.Click += RowClickListener;

                (view.FindViewById<ImageButton>(Resource.Id.btnNotApprove)).Tag =
                (view.FindViewById<ImageButton>(Resource.Id.btnApprove)).Tag = model.POID;
                (view.FindViewById<ImageButton>(Resource.Id.btnApprove)).Click -= BtnApprove_click;
                (view.FindViewById<ImageButton>(Resource.Id.btnApprove)).Click += BtnApprove_click;

                (view.FindViewById<ImageButton>(Resource.Id.btnNotApprove)).Click -= BtnNotApprove_click;
                (view.FindViewById<ImageButton>(Resource.Id.btnNotApprove)).Click += BtnNotApprove_click;

                (view.FindViewById<TextView>(Resource.Id.tvApprovaDetailslName)).Text = model.POID;
                (view.FindViewById<CheckBox>(Resource.Id.chckApprove)).Checked = model.isApproved;


                view.Tag = groupPosition;
                return view;
            }
            public void chkSelect(object sender, EventArgs e)
            {
                CheckBox senderCheckBox = ((CheckBox)sender);
                _context._chkApproveAll.Checked = false;
                View parentView = (View)senderCheckBox.Parent;
                int ItemPostition = (int)parentView.Tag;
                _approvalList[ItemPostition].isApproved = ((CheckBox)sender).Checked;
            }
            public void BtnNotApprove_click(object sender, EventArgs e)
            {
                ImageButton senderButton = ((ImageButton)sender);
                string PO = senderButton.Tag.ToString();
                View parentView = (View)senderButton.Parent;
                int ItemPostition = (int)parentView.Tag;
                var progressDialog = ProgressDialog.Show(_context, null, "", true);
                new Thread(new ThreadStart(() =>
                {
                    ApprovalRepository repo = new ApprovalRepository();
                    int result = 0;
                    result = repo.SavePOApprovalDetails(PO, bitopiApplication.User.UserCode,
                            bitopiApplication.User.EmployeeName,
                            bitopiApplication.ApprovalRoleType == ApprovalRoleType.Approve ? "Rejected" : "NotRecommend",
                            bitopiApplication.ApprovalType, bitopiApplication.ApprovalRoleType, "").Result;
                    if (result == 1)
                    {
                        ////model.isDeleted = true;
                        //if (_approvalList.Count > 1)
                        //{
                        //    var tempModel = _approvalList.Where(er => er.POID == PO).First();
                        //    if (tempModel != null)
                        //        _approvalList.Remove(tempModel);
                        //}
                        //else
                        //    _approvalList = new ApprovalDetailList();
                        _approvalList.RemoveAt(ItemPostition);

                    }
                    //List<ApprovalDetailsModel> aprovalList = repo.GetPOApprovalDetails(bitopiApplication.User.UserCode, bitopiApplication.ApprovalRoleType,
                    //bitopiApplication.ApprovalType).Result;
                    //_approvalList.Clear();
                    //aprovalList.ForEach(s => _approvalList.Add(s));


                    _context.RunOnUiThread(() =>
                    {

                        if (result == 1 && _approvalList.Count == 0)
                        {
                            ((ApprovalDetailActivity)_context).rlMsg.Visibility = ViewStates.Visible;
                            ((ApprovalDetailActivity)_context).rlapprovalDetail.Visibility = ViewStates.Gone;
                        }
                        NotifyDataSetChanged();
                        progressDialog.Dismiss();
                        Toast.MakeText(_context, PO + " has been " +
                            ((bitopiApplication.ApprovalRoleType == ApprovalRoleType.Approve) ? "Rejected" : "Not Recommended")
                            , ToastLength.Short).Show();
                    });

                })).Start();
            }
            public void BtnApprove_click(object sender, EventArgs e)
            {
                ImageButton senderButton = ((ImageButton)sender);
                string PO = senderButton.Tag.ToString();
                View parentView = (View)senderButton.Parent;
                int ItemPostition = (int)parentView.Tag;
                var progressDialog = ProgressDialog.Show(_context, null, "", true);
                ApprovalRepository repo = new ApprovalRepository();

                new Thread(new ThreadStart(() =>
                {
                    var result = 0;
                    result = repo.SavePOApprovalDetails(PO, bitopiApplication.User.UserCode,
                       bitopiApplication.User.EmployeeName, bitopiApplication.ApprovalRoleType == ApprovalRoleType.Approve ? "Approved" : "Recommended",
                       bitopiApplication.ApprovalType, bitopiApplication.ApprovalRoleType, "").Result;
                    if (result == 1)
                    {
                        //model.isDeleted = true;
                        //var tempModel = _approvalList.Where(er => er.POID == PO).First();
                        //if (tempModel != null)
                        //    _approvalList.Remove(tempModel);
                        //_approvalList = new ApprovalDetailList();
                        _approvalList.RemoveAt(ItemPostition);
                    }

                    //List<ApprovalDetailsModel> aprovalList =  repo.GetPOApprovalDetails(bitopiApplication.User.UserCode, bitopiApplication.ApprovalRoleType,
                    //bitopiApplication.ApprovalType).Result;
                    //_approvalList.Clear();
                    //aprovalList.ForEach(s => _approvalList.Add(s));


                    _context.RunOnUiThread(() =>
                    {

                        if (result == 1 && _approvalList.Count == 0)
                        {
                            ((ApprovalDetailActivity)_context).rlMsg.Visibility = ViewStates.Visible;
                            ((ApprovalDetailActivity)_context).rlapprovalDetail.Visibility = ViewStates.Gone;
                        }
                        NotifyDataSetChanged();
                        progressDialog.Dismiss();
                        Toast.MakeText(_context, PO + " has been " +
                               ((bitopiApplication.ApprovalRoleType == ApprovalRoleType.Approve) ? "Approved" : "Recommended")
                               , ToastLength.Short).Show();
                    });


                })).Start();
            }
            public override View getRealChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
            {
                View row = LayoutInflater.From(_context).Inflate(Resource.Layout.ApprovalDetailChildLayout, null);
                //TextView tvDataDetails = row.FindViewById<TextView>(Resource.Id.tvDataDetails);

                //string s = "";
                //foreach (var data in _approvalList[groupPosition].ApprovalDataList)
                //{
                //    s += "<b>" + data.Key + "</b>" + data.Value + "</br>";
                //}
                //tvDataDetails.SetText(Html.FromHtml(s), TextView.BufferType.Spannable);
                //row.Post(() => {
                //    row.SetMinimumHeight(500);
                //});
                int id = 2;
                foreach (var data in _approvalList[groupPosition].ApprovalDataList)
                {
                    var childView = LayoutInflater.From(_context).Inflate(Resource.Layout.ApprovalDetailRow, null);
                    var key = childView.FindViewById<TextView>(Resource.Id.tvKey);
                    var value = childView.FindViewById<TextView>(Resource.Id.tvValue);
                    key.Text = data.Key;
                    value.Text = data.Value;
                    key.SetTextColor(Color.ParseColor("#ff5722"));
                    value.SetTextColor(Color.ParseColor("#ff5722"));
                    RelativeLayout rlApprovalDataContainer = row.FindViewById<RelativeLayout>(Resource.Id.rlApprovalDataContainer);
                    RelativeLayout.LayoutParams params1 = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                    childView.Id = id;
                    params1.AddRule(LayoutRules.Below, (id - 1));

                    rlApprovalDataContainer.AddView(childView, params1);
                    id++;
                }
                //row.Post(() =>
                //{
                //    var param = row.LayoutParameters;
                //    param.Height = 300;
                //    //param.Height = lvReportSubMenu.Height;

                //    row.LayoutParameters = param;
                //    row.RequestLayout();
                //});
                Console.WriteLine(groupPosition);
                return row;
            }
            void RowClickListener(Object sender, EventArgs e)
            {
                int groupPosition = Convert.ToInt32(((View)sender).Tag.ToString());
                if (groupPosition != (int)lastExpandedGroupPosition)
                {

                    lvApprovalDetailList.collapseGroupWithAnimation((int)lastExpandedGroupPosition, null);
                }
                if (lvApprovalDetailList.IsGroupExpanded(groupPosition))
                {
                    lvApprovalDetailList.collapseGroupWithAnimation(groupPosition, null);
                }
                else
                {
                    lvApprovalDetailList.SmoothScrollToPositionFromTop(groupPosition,0,500);
                    lvApprovalDetailList.expandGroupWithAnimation(groupPosition);
                }

                lastExpandedGroupPosition = groupPosition;
                
            }

        }
        public class ApprovalDataDetailsAdapter : BaseAdapter
        {
            List<ApprovalDataModel> _approvalList;
            Context _context;
            public ApprovalDataDetailsAdapter(List<ApprovalDataModel> approvalList, Context context)
            {
                _approvalList = approvalList;
                _context = context;
            }
            public override int Count
            {
                get
                {
                    return _approvalList.Count;
                }
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                View view;
                if (convertView == null)
                    view = LayoutInflater.From(_context).Inflate(Resource.Layout.ApprovalDetailRow, parent, false);
                else
                    view = convertView;
                ApprovalDataModel model = _approvalList[position];
                (view.FindViewById<TextView>(Resource.Id.tvKey)).Text = model.Key;
                (view.FindViewById<TextView>(Resource.Id.tvValue)).Text = model.Value.ToString();

                return view;
            }
            public override Java.Lang.Object GetItem(int position)
            {
                return null;
            }
            public override long GetItemId(int position)
            {
                return position;
            }

        }
        public class ApprovalDetailList : List<ApprovalDetailsModel>
        {
            BitopiApplication bitopiApplication;
            public ApprovalDetailList(Activity activity)
            {
                bitopiApplication = (BitopiApplication)activity.ApplicationContext;
            }
            public async void SaveSelected(string POStatus, Action<int> Callback, string Remarks = "")
            {
                int NumberOfSelectedItem = this.Where(t => t.isApproved == true).Count();
                int numberOfSuccessFullOperation = 0;
                int numberOfSaveCompleted = 0;
                foreach (var model in this)
                {
                    if (model.isApproved == true)
                    {
                        ApprovalRepository repo = new ApprovalRepository();
                        int result = 0;

                        result = await repo.SavePOApprovalDetails(model.POID, bitopiApplication.User.UserCode,
                            bitopiApplication.User.EmployeeName, POStatus,
                            bitopiApplication.ApprovalType, bitopiApplication.ApprovalRoleType, Remarks);

                        numberOfSuccessFullOperation += result;
                        numberOfSaveCompleted++;
                        if (result == 1)
                        {
                            model.isDeleted = true;
                            //numberOfSuccessFullOperation++;
                        }

                        if (numberOfSaveCompleted == NumberOfSelectedItem)
                        {
                            Callback(numberOfSuccessFullOperation);
                        }
                    }
                }
            }
        }

        class CustomSearchviewclicklistener : Java.Lang.Object, Android.Views.View.IOnClickListener
        {
            private bool extended = false;
            RelativeLayout _rltitle;
            public CustomSearchviewclicklistener(RelativeLayout rltitle)
            {
                _rltitle = rltitle;

            }
            public void OnClick(View v)
            {
                _rltitle.Visibility = ViewStates.Gone;

                v.ClearFocus();
            }
        }
        class CustomSearchViewOnCloseListenter : Java.Lang.Object, Android.Support.V7.Widget.SearchView.IOnCloseListener
        {
            RelativeLayout _rltitle;
            ApprovalDetalisListAdapter _approvalListAdapter;
            ApprovalDetailList _list;
            public CustomSearchViewOnCloseListenter(RelativeLayout rltitle, ApprovalDetalisListAdapter adapter, ApprovalDetailList list)
            {
                _rltitle = rltitle;
                _approvalListAdapter = adapter;
                _list = list;



            }
            public bool OnClose()
            {
                _rltitle.Visibility = ViewStates.Visible;

                if (_approvalListAdapter != null)
                {
                    _approvalListAdapter.SetData(_list);
                    _approvalListAdapter.NotifyDataSetChanged();
                }

                return false;
            }

        }
        class CustomSearchViewOnFocusChangeListenter : Java.Lang.Object, View.IOnFocusChangeListener
        {

            Activity _context;
            public CustomSearchViewOnFocusChangeListenter(Activity context)
            {
                _context = context;
            }


            public void OnFocusChange(View v, bool hasFocus)
            {
                if (hasFocus)
                {
                    InputMethodManager imm = (InputMethodManager)_context.GetSystemService(Context.InputMethodService);
                    imm.ToggleSoftInput(InputMethodManager.ShowForced, 0);
                }
            }
        }
        public override void OnDetachedFromWindow()
        {
            StartService(new Intent(this, typeof(BitopiNotificationService)));
            base.OnDetachedFromWindow();
        }
    }
}