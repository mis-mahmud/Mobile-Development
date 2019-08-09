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
using Android.Support.V4.Widget;
using ApiRepository;
using BitopiApprovalSystem.Adapter;
using BitopiApprovalSystem.Model;

namespace BitopiApprovalSystem
{
    [Activity(Label = "FabricSupplierRollinfo")]
    public class FabricSupplierRollinfo : BaseActivity
    {

        Button btnSave, btnCancelPopupHeadCut;
        Spinner spGrnID, spColor;
        Switch swLoadLastLocation;
        bool LastLocation;
        FabricRepository repo = new FabricRepository();
        ListView lvSupplierRollInfo;
        string SelectedProcess;
        string SelectedLocation;
        string SelectedEntryType;
        RelativeLayout RLleft_drawer;
        private DrawerLayout mDrawerLayout;
        string RecentProces, RecentLocation, RecentEntryType;
        List<string> ddlGRN;
        List<string> ddlColor;
        FabricSupplierRollAdapter adapter;
        string selectedGRN, selectedColor;
        List<RollSettingsDBModel> rollList;
        RelativeLayout rlPopup;

        TextView txtRollNoValue, etSuppRollNo;
        EditText etOwnWidth, etWidthBW, etLengthBW,
            etOwnLength, etWidthAW, etLengthAW;
        TextView tvWidthPercent, tvLengthPercent;
        Switch swPassFail;
        public FabricRollUpdateItem FabricRollUpdateItem;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            ISharedPreferences pref = Application.Context.GetSharedPreferences
                 ("_bitopi_UserInfo", FileCreationMode.Private);

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.FabricSupplierRollInfoLayout);
            SupportActionBar.SetDisplayShowCustomEnabled(true);
            SupportActionBar.SetCustomView(Resource.Layout.custom_actionbar);
            InitializeControl();
            InitializeEvent();
            base.LoadDrawerView();
        }
        protected async override void OnStart()
        {
            base.OnStart();
            ddlGRN = await repo.GetGrnID();
            spGrnID.Adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, ddlGRN.ToArray());
        }
        protected override void InitializeControl()
        {

            lvSupplierRollInfo = FindViewById<ListView>(Resource.Id.lvSupplierRoll);
            adapter = new Adapter.FabricSupplierRollAdapter(new List<Model.RollSettingsDBModel>(), this, lvSupplierRollInfo);
            lvSupplierRollInfo.Adapter = adapter;
            RLleft_drawer = FindViewById<RelativeLayout>(Resource.Id.RLleft_drawer);
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            FindViewById<ImageButton>(Resource.Id.btnDrawermenu).Visibility = ViewStates.Visible;
            btnSave = FindViewById<Button>(Resource.Id.btnSave);
            spGrnID = FindViewById<Spinner>(Resource.Id.spGRN);
            spColor = FindViewById<Spinner>(Resource.Id.spColor);
            btnCancelPopupHeadCut = FindViewById<Button>(Resource.Id.btnCancelHeadPopup);
            rlPopup = FindViewById<RelativeLayout>(Resource.Id.rlPopup);
            etOwnWidth = rlPopup.FindViewById<EditText>(Resource.Id.etOwnWidth);
            etOwnLength = rlPopup.FindViewById<EditText>(Resource.Id.etOwnLenght);

            etWidthAW = rlPopup.FindViewById<EditText>(Resource.Id.etWidthAW);
            etLengthAW = rlPopup.FindViewById<EditText>(Resource.Id.etLengthAW);

            tvWidthPercent = rlPopup.FindViewById<TextView>(Resource.Id.tvWidthPercent);
            tvLengthPercent = rlPopup.FindViewById<TextView>(Resource.Id.tvLengthPercent);

            txtRollNoValue = rlPopup.FindViewById<TextView>(Resource.Id.txtRollNoValue);
            etSuppRollNo = rlPopup.FindViewById<TextView>(Resource.Id.etSupRollNo);
            swPassFail = FindViewById<Switch>(Resource.Id.swPassFail);
            base.InitializeControl();
        }
        protected override void InitializeEvent()
        {

            spGrnID.ItemSelected += SpGrnID_ItemSelected; ;
            spColor.ItemSelected += SpColor_ItemSelected; ;
            btnSave.Click += BtnSave_Click;
            btnCancelPopupHeadCut.Click += (s, e) =>
            {
                FindViewById<RelativeLayout>(Resource.Id.rlPopup).Visibility = ViewStates.Gone;
            };

            etWidthAW.TextChanged += (s, e) =>
            {
                CalcWidthPercent();
            };

            etLengthAW.TextChanged += (s, e) =>
            {
                CalcLengthPercent();
            };
            base.InitializeEvent();
        }
        void CalcLengthPercent()
        {
            decimal bw, aw;
            if (!String.IsNullOrEmpty(etLengthBW.Text) && !String.IsNullOrEmpty(etLengthAW.Text))
            {
                bw = Convert.ToDecimal(etLengthBW.Text);
                aw = Convert.ToDecimal(etLengthAW.Text);
                int per = (int)Math.Round(((bw - aw) / aw) * 100);
                tvLengthPercent.Text = $"Shrinkage Length After Wash Parcent is {per}%";
            }
            else
            {
                tvLengthPercent.Text = "";
            }
        }
        void CalcWidthPercent()
        {
            decimal bw, aw;
            if (!String.IsNullOrEmpty(etWidthBW.Text) && !String.IsNullOrEmpty(etWidthAW.Text))
            {
                bw = Convert.ToDecimal(etWidthBW.Text);
                aw = Convert.ToDecimal(etWidthAW.Text);
                int per = (int)Math.Round(((bw - aw) / aw) * 100);
                tvWidthPercent.Text = $"Shrinkage Width After Wash Parcent is {per}%";
            }
            else
            {
                tvWidthPercent.Text = "";
            }
        }
        public void LoadHeadCut(RollSettingsDBModel model)
        {
            rlPopup.Visibility = ViewStates.Visible;
            var rlHeadCut = FindViewById<RelativeLayout>(Resource.Id.rlHeadCut);
            rlHeadCut.Visibility = ViewStates.Visible;
            FindViewById<RelativeLayout>(Resource.Id.rlQuality).Visibility = ViewStates.Invisible;
            FindViewById<RelativeLayout>(Resource.Id.rlInspection).Visibility = ViewStates.Invisible;

            etWidthBW = rlHeadCut.FindViewById<EditText>(Resource.Id.etWidthBW);
            etLengthBW = rlHeadCut.FindViewById<EditText>(Resource.Id.etLengthBW);

            txtRollNoValue.Text = model.RollSerial.ToString();
            txtRollNoValue.Tag = model.RollID;
            etSuppRollNo.Text = model.SupplierRollNo;
            etOwnWidth.Text = model.OwnWidth;
            etWidthBW.Text = model.WidthBeforeWash;
            etLengthBW.Text = model.SLengthBW;
        }
        public void LoadQuality(RollSettingsDBModel model)
        {
            rlPopup.Visibility = ViewStates.Visible;
            var rlQuality = FindViewById<RelativeLayout>(Resource.Id.rlQuality);
            FindViewById<RelativeLayout>(Resource.Id.rlHeadCut).Visibility = ViewStates.Invisible;
            rlQuality.Visibility = ViewStates.Visible;
            FindViewById<RelativeLayout>(Resource.Id.rlInspection).Visibility = ViewStates.Invisible;

            etWidthBW = rlQuality.FindViewById<EditText>(Resource.Id.etWidthBW);
            etLengthBW = rlQuality.FindViewById<EditText>(Resource.Id.etLengthBW);

            txtRollNoValue.Text = model.RollSerial.ToString();
            txtRollNoValue.Tag = model.RollID;
            etSuppRollNo.Text = model.SupplierRollNo;

            etWidthBW.Text = model.WidthBeforeWash;
            etLengthBW.Text = model.SLengthBW;
            etWidthAW.Text = model.SWidthAW;
            etLengthAW.Text = model.SLengthAW;
            etWidthBW.TextChanged += (s, e) =>
            {
                CalcWidthPercent();
            };
            etLengthBW.TextChanged += (s, e) =>
            {
                CalcWidthPercent();
            };

            if (!String.IsNullOrEmpty(model.SLengthAWPercent))
                tvLengthPercent.Text = $"Shrinkage Length After Wash Parcent is {model.SLengthAWPercent}%";
            if (!String.IsNullOrEmpty(model.SWidthAWParcent))
                tvWidthPercent.Text = $"Shrinkage Width After Wash Parcent is {model.SWidthAWParcent}%";
        }


        public void LoadInspection(RollSettingsDBModel model)
        {
            rlPopup.Visibility = ViewStates.Visible;
            FindViewById<RelativeLayout>(Resource.Id.rlHeadCut).Visibility = ViewStates.Invisible;
            FindViewById<RelativeLayout>(Resource.Id.rlQuality).Visibility = ViewStates.Invisible;
            FindViewById<RelativeLayout>(Resource.Id.rlInspection).Visibility = ViewStates.Visible;
            txtRollNoValue.Text = model.RollSerial.ToString();
            txtRollNoValue.Tag = model.RollID;
            etSuppRollNo.Text = model.SupplierRollNo;
            etOwnLength.Text = model.OwnLength.ToString();
            swPassFail.Checked = model.QCPass == null ? true : (bool)model.QCPass;

        }
        private async void BtnSave_Click(object sender, EventArgs e)
        {
            RollSettingsDBModel roll = new RollSettingsDBModel() { RollID = txtRollNoValue.Tag.ToString(),
            RollSerial= txtRollNoValue.Text
            };
            if (this.FabricRollUpdateItem == FabricRollUpdateItem.HeadCut)
            {
                roll.OwnWidth = etOwnWidth.Text;
                roll.WidthBeforeWash = etWidthBW.Text;
                roll.SLengthBW = etLengthBW.Text;
            }
            if (this.FabricRollUpdateItem == FabricRollUpdateItem.Inspection)
            {
                roll.OwnLength = Convert.ToDecimal(etOwnLength.Text);
                roll.QCPass = swPassFail.Checked;

            }
            if (this.FabricRollUpdateItem == FabricRollUpdateItem.QC)
            {
                roll.WidthBeforeWash = etWidthBW.Text;
                roll.SWidthAW = etWidthAW.Text;
                roll.SLengthBW = etLengthBW.Text;
                roll.SLengthAW = etLengthAW.Text;
            }
            int result = await repo.SetRoll(roll);
            if (result > 0)
            {
                rollList = await repo.GetRollList(selectedGRN, selectedColor);
                adapter.Items = rollList;
                adapter.NotifyDataSetChanged();
                List<string> serial = rollList.Select(t => t.RollSerial).OrderBy(t=>Convert.ToInt16(t)).ToList();
                int srialMe = serial.IndexOf(roll.RollSerial);
                if ((srialMe + 1) <= serial.Count()-1)
                {
                    string srialNext = serial[srialMe + 1];

                    roll = rollList.Where(t => t.RollSerial == srialNext).FirstOrDefault();
                    if (this.FabricRollUpdateItem == FabricRollUpdateItem.HeadCut)
                        LoadHeadCut(roll);
                    if (this.FabricRollUpdateItem == FabricRollUpdateItem.Inspection)
                        LoadInspection(roll);
                    if (this.FabricRollUpdateItem == FabricRollUpdateItem.QC)
                        LoadQuality(roll);
                }
                Toast.MakeText(this, "Saved Successfully!!!", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(this, "Save not Successful!!!", ToastLength.Long).Show();
            }
        }

        private async void SpColor_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            //throw new NotImplementedException();
            selectedColor = ddlColor[e.Position];
            rollList = await repo.GetRollList(selectedGRN, selectedColor);
            if (rollList != null && rollList.Count > 0)
            {
                FindViewById<RelativeLayout>(Resource.Id.rlHeader).Visibility = ViewStates.Visible;
            }
            else
            {
                FindViewById<RelativeLayout>(Resource.Id.rlHeader).Visibility = ViewStates.Gone;
                Toast.MakeText(this, "No Data Found!!!!!!!!", ToastLength.Long);
            }
            adapter.Items = rollList;
            adapter.NotifyDataSetChanged();
        }

        private async void SpGrnID_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            string grnid = ddlGRN[e.Position];
            selectedGRN = grnid;
            ddlColor = await repo.GetColor(grnid);
            spColor.Adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, ddlColor.ToArray());
        }

    }
}