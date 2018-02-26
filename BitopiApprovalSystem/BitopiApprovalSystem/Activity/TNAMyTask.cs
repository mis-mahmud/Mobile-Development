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
using BitopiApprovalSystem.Widget;
using static BitopiApprovalSystem.ApprovalDetailActivity;
using BitopiApprovalSystem.Model;
using ApiRepository;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Graphics;
using System.Threading;
using Android.Views.InputMethods;
using static Android.Views.View;
using Android.Support.V4.Widget;
using System.Globalization;
using Android.Support.V4.Content;
using BitopiApprovalSystem.DAL;

namespace BitopiApprovalSystem
{
    [Activity(Label = "TNAMyTask", WindowSoftInputMode = SoftInput.AdjustNothing)]
    public class TNAMyTaskActivity : BaseActivity
    {
        AnimatedExpandableListView lvMyTask;
        TNAMyTaskListAdapter _TNAMyTaskListAdapter;
        ApprovalType _approvalType;
        ApprovalRoleType _approvalRoleType;
        CheckBox _chkApproveAll;
        MyTaskList _MyTaskList;
        int lastExpandedGroupPosition = -1;
        Android.App.AlertDialog.Builder builder;
        TNARepository repo;
        public RelativeLayout rlMsg;
        public RelativeLayout rlapprovalDetail;
        RelativeLayout rltitle;
        //BitopiApplication bitopiApplication;
        TextView tvMsg;
        TextView tvHeaderName;
        RelativeLayout RLleft_drawer;
        private DrawerLayout mDrawerLayout;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            //bitopiApplication = (BitopiApplication)this.ApplicationContext;
            builder = new Android.App.AlertDialog.Builder(this);
            builder.SetMessage("Hello, World!");

            builder.SetNegativeButton("Cancel", (s, e) => { /* do something on Cancel click */ });


            base.OnCreate(savedInstanceState);
            _MyTaskList = new MyTaskList(this);
            _approvalType = (ApprovalType)(Convert.ToInt16(Intent.GetStringExtra("ApprovalType")));
            _approvalRoleType = (ApprovalRoleType)(Convert.ToInt16(Intent.GetStringExtra("ApprovalRoleType")));

            SupportRequestWindowFeature(WindowCompat.FeatureActionBar);
            SupportActionBar.SetDisplayShowCustomEnabled(true);
            SupportActionBar.SetCustomView(Resource.Layout.custom_actionbar);
            rltitle = FindViewById<RelativeLayout>(Resource.Id.rltitle);
            tvHeaderName= FindViewById<TextView>(Resource.Id.tvHeaderName);

            SetContentView(Resource.Layout.TNAMyTaskList);

            lvMyTask = FindViewById<AnimatedExpandableListView>(Resource.Id.lvMyTask);
            tvMsg = FindViewById<TextView>(Resource.Id.tvMsg);
            tvMsg.Visibility = ViewStates.Gone;
            _chkApproveAll = FindViewById<CheckBox>(Resource.Id.chkSelectAll);
            rlMsg = FindViewById<RelativeLayout>(Resource.Id.rlMsg);
            rlapprovalDetail = FindViewById<RelativeLayout>(Resource.Id.rlapprovalDetail);
            switch (bitopiApplication.MyTaskType)
            {
                case MyTaskType.UNSEEN:
                    tvMsg.Text = "You don't have any unseen Task";
                    tvHeaderName.Text = "My Unseen Task";


                    break;
                case MyTaskType.SEEN:
                    tvMsg.Text = "You don't have any seen Task";
                    tvHeaderName.Text = "My Seen Task";
                    break;
                case MyTaskType.COMPLETED:
                    tvMsg.Text = "You don't have any completed Task";
                    tvHeaderName.Text = "My Completed Task";
                    break;
            }
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

            //LoadDrawerView();
        }
        public override void OnBackPressed()
        {
            Finish();
        }
        protected async override void OnStart()
        {
            bitopiApplication.CurrentActivity = "Approval Detail Activity";
            base.OnStart();
            var progressDialog = ProgressDialog.Show(this, null, "Please wait...", true);
            repo = new TNARepository();

            List<MyTaskDBModel> mytaskList = null;
            if (bitopiApplication.MyTaskType == MyTaskType.UNSEEN)
                mytaskList = await repo.GetMyTask(bitopiApplication.User.UserCode);
            if (bitopiApplication.MyTaskType == MyTaskType.SEEN)
                mytaskList = await repo.GetMyTask(bitopiApplication.User.UserCode, "SEEN");
            if (bitopiApplication.MyTaskType == MyTaskType.COMPLETED)
                mytaskList = await repo.GetMyTask(bitopiApplication.User.UserCode, "COMPLETE");
            if (mytaskList.Count == 0)
                tvMsg.Visibility = ViewStates.Visible;
            mytaskList.ForEach(s => _MyTaskList.Add(s));
            _TNAMyTaskListAdapter = new TNAMyTaskListAdapter(_MyTaskList, lvMyTask, this,()=> { tvMsg.Visibility = ViewStates.Visible; });
            lvMyTask.SetAdapter(_TNAMyTaskListAdapter);
            _TNAMyTaskListAdapter.NotifyDataSetChanged();
            lvMyTask.SetGroupIndicator(null);
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
                //string dst = "sdfsfs";
                //CustomLogger.CustomLog("From Activity: " + bitopiApplication.CurrentActivity + "\nMessage: " + ex.Message + "\nStack Trace: " + ex.StackTrace + "\n\n", "", bitopiApplication.User != null ?
                //         bitopiApplication.User.UserName : "");
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
            v.Visibility = ViewStates.Gone;
            
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
                        repo = new TNARepository();

                        List<MyTaskDBModel> mytaskList = null;
                        if (bitopiApplication.MyTaskType == MyTaskType.UNSEEN)
                            mytaskList =  repo.GetMyTask(bitopiApplication.User.UserCode).Result;
                        if (bitopiApplication.MyTaskType == MyTaskType.SEEN)
                            mytaskList =  repo.GetMyTask(bitopiApplication.User.UserCode, "SEEN").Result;
                        if (bitopiApplication.MyTaskType == MyTaskType.COMPLETED)
                            mytaskList =  repo.GetMyTask(bitopiApplication.User.UserCode, "COMPLETE").Result;
                        if (mytaskList.Count == 0)
                            tvMsg.Visibility = ViewStates.Visible;

                        _MyTaskList.Clear();
                        mytaskList.ForEach(s => _MyTaskList.Add(s));


                        RunOnUiThread(() =>
                        {
                            TNAMyTaskListAdapter _adapter 
                            = new TNAMyTaskListAdapter(_MyTaskList, lvMyTask, this, () => { tvMsg.Visibility = ViewStates.Visible; });
                            lvMyTask.SetAdapter(_adapter);
                            _adapter.NotifyDataSetChanged();

                            
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
            //FindViewById<TextView>(Resource.Id.tvUserName).Text = BitopiSingelton.Instance.User.EmployeeName;
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
                var progressDialog = ProgressDialog.Show(this, null, "Please wait...", true);
                new System.Threading.Thread(new ThreadStart(() =>
                {
                    AccountRepository repo = new AccountRepository();

                    var resutl = repo.getUser("", "",
                     bitopiApplication.MacAddress,
                     "",
                     "",
                     "android", 2, bitopiApplication.CurrentVersion,bitopiApplication.User.UserCode).Result;

                    ISharedPreferences pref =
                    Application.Context.GetSharedPreferences("_bitopi_UserInfo", FileCreationMode.Private);
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
        //public override bool DispatchTouchEvent(MotionEvent ev)
        //{
        //    if (ev.Action == MotionEventActions.Down)
        //    {
        //        View v = CurrentFocus;
        //        if (v is EditText)
        //        {
        //            Rect outRect = new Rect();
        //            v.GetGlobalVisibleRect(outRect);
        //            if (!outRect.Contains((int)ev.RawX, (int)ev.RawY))
        //            {
        //                v.ClearFocus();
        //                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
        //                imm.HideSoftInputFromWindow(v.WindowToken, 0);
        //            }
        //        }
        //    }
        //    return base.DispatchTouchEvent(ev);
        //}

        public class TNAMyTaskListAdapter : AnimatedExpandableListView.AnimatedExpandableListAdapter
        {
            MyTaskList _MyTaskList;
            MyTaskList MatchItems;
            TNAMyTaskActivity _context;
            AnimatedExpandableListView lvMyTask;
            BitopiApplication bitopiApplication;
            TNAMyTaskListAdapter adapter;
            Action calback;
            int currentlyFocusedRow = -1;
            public TNAMyTaskListAdapter(MyTaskList myTaskList, AnimatedExpandableListView _lvMyTask,
                TNAMyTaskActivity _activity) : base(_activity)
            {
                MatchItems = _MyTaskList = myTaskList;
                _context = _activity;
                lvMyTask = _lvMyTask;
                bitopiApplication = (BitopiApplication)_context.ApplicationContext;
                adapter = this;


            }
            public TNAMyTaskListAdapter(MyTaskList myTaskList, AnimatedExpandableListView _lvMyTask,
               TNAMyTaskActivity _activity, Action calback) : base(_activity)
            {
                MatchItems = _MyTaskList = myTaskList;
                _context = _activity;
                lvMyTask = _lvMyTask;
                bitopiApplication = (BitopiApplication)_context.ApplicationContext;
                adapter = this;
                this.calback = calback;

            }
            public void SetData(MyTaskList myTaskList)
            {

                MatchItems = _MyTaskList = myTaskList;
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
                MyTaskDBModel model = MatchItems[groupPosition];
                View view = convertView;
                Holder holder = null;
                //if (convertView == null)
                // {
                if (bitopiApplication.MyTaskType == MyTaskType.UNSEEN)
                    view = LayoutInflater.From(_context).Inflate(Resource.Layout.MyTaskUnSeenRow, parent, false);
                if (bitopiApplication.MyTaskType == MyTaskType.SEEN)
                    view = LayoutInflater.From(_context).Inflate(Resource.Layout.MyTaskSeenRow, parent, false);
                if (bitopiApplication.MyTaskType == MyTaskType.COMPLETED)
                    view = LayoutInflater.From(_context).Inflate(Resource.Layout.MyTaskCompletedRow, parent, false);

                //  visiblePosArray[position%visiblePosArray.length]=position;
                if (bitopiApplication.MyTaskType == MyTaskType.UNSEEN)
                    holder = new Holder();
                else
                    holder = new Holder2();
                if (holder is Holder2)
                {
                    ((Holder2)holder).remarks = view.FindViewById<TextView>(Resource.Id.etRemarks);
                    ((Holder2)holder).commitedDate = view.FindViewById<TextView>(Resource.Id.etCommitedDate);

                }
                else
                {
                    holder.remarks = view.FindViewById<EditText>(Resource.Id.etRemarks);
                    holder.commitedDate = view.FindViewById<EditText>(Resource.Id.etCommitedDate);
                }
                if (bitopiApplication.MyTaskType == MyTaskType.UNSEEN || bitopiApplication.MyTaskType == MyTaskType.SEEN)
                {
                    holder.ckbox = view.FindViewById<CheckBox>(Resource.Id.chckApprove);
                }
                
                if (model.IsDisabled)
                {
                    holder.ckbox.Enabled = false;
                    if (bitopiApplication.MyTaskType == MyTaskType.UNSEEN)
                        holder.commitedDate.Enabled = holder.remarks.Enabled = false;
                }
                if (bitopiApplication.MyTaskType == MyTaskType.UNSEEN || bitopiApplication.MyTaskType == MyTaskType.SEEN)
                {
                    holder.ckbox.Click += (sender, e) =>
                {
                    var builder = new Android.App.AlertDialog.Builder(_context);
                    builder.SetMessage("Are you sure to seen this task?");
                    if (bitopiApplication.MyTaskType == MyTaskType.UNSEEN)
                    {
                        model.Remarks = holder.remarks.Text;//etUserName.SetBackgroundResource(Resource.Drawable.rounded_textview_error);
                        if (holder.commitedDate.Text == "")
                        {
                            holder.commitedDate.SetBackgroundResource(Resource.Drawable.rounded_textview_error);
                            ((CheckBox)sender).Checked = false;
                            return;
                        }
                        else
                        {
                            holder.commitedDate.SetBackgroundResource(Resource.Drawable.rounded_textview);
                        }
                    }
                    builder.SetPositiveButton("OK", (s, ev) =>
                    {
                        CheckBox senderCheckBox = ((CheckBox)sender);

                        View parentView = (View)senderCheckBox.Parent;
                        Holder holder1 = parentView.Tag as Holder;
                        ;
                        var progressDialog = ProgressDialog.Show(_context, null, "Please wait...", true);
                        new Thread(new ThreadStart(() =>
                        {
                            TNARepository repo = new TNARepository();
                            int result = 0;
                            if (bitopiApplication.MyTaskType == MyTaskType.UNSEEN)
                                result = repo.SetTaskUnSeentoSeen(_MyTaskList[holder.GroupPostiion], bitopiApplication.User.UserCode).Result;
                            if (bitopiApplication.MyTaskType == MyTaskType.SEEN)
                                result = repo.SetTaskSeenToComplete(_MyTaskList[holder.GroupPostiion], bitopiApplication.User.UserCode).Result;
                            if (result == 1)
                            {

                                _MyTaskList[holder1.GroupPostiion].IsDisabled = true;

                            }
                            _context.RunOnUiThread(() =>
                            {

                                if (result == 1)
                                {

                                    //ListViewAnimationHelper helper = new ListViewAnimationHelper(adapter, lvMyTask, MatchItems);
                                    //helper.animateRemoval(lvMyTask, parentView);
                                    int position = lvMyTask.GetPositionForView(parentView);
                                    MatchItems.RemoveAt(position);
                                    NotifyDataSetChanged();
                                    if (MatchItems.Count == 0 && calback != null)
                                        calback();
                                    progressDialog.Dismiss();
                                    Toast.MakeText(_context, "Has been seen", ToastLength.Short).Show();
                                }
                                else
                                {
                                    ((CheckBox)sender).Checked = false;
                                    Toast.MakeText(_context, "Data Save Failed. Please Try Later", ToastLength.Short).Show();
                                }



                            });

                        })).Start();
                    });
                    builder.SetNegativeButton("CANCEL", (s, ev) =>
                    {
                        ((CheckBox)sender).Checked = false;
                    });
                    builder.Create().Show();
                };
                }

                if (holder is Holder2)
                {
                    ((Holder2)holder).remarks.Text = model.Remarks;
                    ((Holder2)holder).commitedDate.Text = model.CommittedDate;
                }
                else
                {
                    (holder).remarks.Text = model.Remarks;
                    holder.commitedDate.Text = model.PlannedDate;
                }
                (view.FindViewById<TextView>(Resource.Id.tvTask)).Text = model.Task;
                (view.FindViewById<TextView>(Resource.Id.tvShimpentDate)).Text = model.ShipmentDate;

                if (bitopiApplication.MyTaskType == MyTaskType.UNSEEN)
                {
                    holder.commitedDate.Click += (s, e) =>
                    {
                        DateTime SetDate;
                        if (holder.commitedDate.Text == "")
                        {
                            SetDate = DateTime.Today;
                            //model.ActualDate= holder.commitedDate.Text = SetDate.Date.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            //DateTime.TryParseExact(holder.commitedDate.Text, "dd/MM/yyyy", new CultureInfo("en-US"),
                            //DateTimeStyles.None,
                            //out SetDate);
                            SetDate = Convert.ToDateTime(holder.commitedDate.Text);
                        }

                        DatePickerDialog dialog = new DatePickerDialog(_context, (sender, evnt) =>
                        {
                            model.PlannedDate = model.CommittedDate = holder.commitedDate.Text = evnt.Date.ToString("dd/MM/yyyy");
                            holder.commitedDate.SetBackgroundResource(Resource.Drawable.rounded_textview);
                        }, SetDate.Year, SetDate.Month - 1, SetDate.Day);
                        dialog.DatePicker.MinDate = SetDate.Millisecond;
                        dialog.Show();
                    };

                    holder.remarks.TextChanged += (s, e) =>
                    {
                        model.Remarks = holder.remarks.Text;
                    };
                    if (currentlyFocusedRow == groupPosition)
                    {
                        holder.remarks.RequestFocus();
                    }
                    (view.FindViewById<EditText>(Resource.Id.etCommitedDate)).FocusableInTouchMode = false;
                    //(view.FindViewById<EditText>(Resource.Id.etRemarks)).FocusableInTouchMode = true;
                    holder.remarks.OnFocusChangeListener = new CustomOnFocusChangeListener(this, groupPosition, lvMyTask);
                }

                holder.GroupPostiion = groupPosition;

                view.Tag = holder;
                return view;
            }
            void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
            {
                //editText.Text = e.Date.ToLongDateString();
            }
            class CustomOnFocusChangeListener : Java.Lang.Object, IOnFocusChangeListener
            {
                TNAMyTaskListAdapter adapter; int position;
                AnimatedExpandableListView lvMyTask;
                public CustomOnFocusChangeListener(TNAMyTaskListAdapter adapter, int position, AnimatedExpandableListView lvMyTask)
                {
                    this.adapter = adapter;
                    this.position = position;
                    this.lvMyTask = lvMyTask;
                }
                public void OnFocusChange(View v, bool hasFocus)
                {
                    if (hasFocus)
                    {
                        if (adapter.currentlyFocusedRow == -1)
                        {
                            adapter.currentlyFocusedRow = position;
                            lvMyTask.SmoothScrollToPositionFromTop(position, 0, 500);
                        }
                    }
                    else
                    {
                        adapter.currentlyFocusedRow = -1;
                    }
                }
            }
        }
        
        
    }
    public class MyTaskList : List<MyTaskDBModel>
    {
        BitopiApplication bitopiApplication;
        public MyTaskList(Activity activity)
        {
            bitopiApplication = (BitopiApplication)activity.ApplicationContext;
        }
        public async void SaveSelected(string POStatus, Action<int> Callback, string Remarks = "")
        {

        }
    }
    class Holder : Java.Lang.Object
    {

        public CheckBox ckbox { get; set; }
        public EditText remarks { get; set; }
        public EditText commitedDate { get; set; }
        public int GroupPostiion { get; set; }

    }
    class Holder2 : Holder
    {
        public new TextView remarks { get; set; }
        public new TextView commitedDate { get; set; }

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
}