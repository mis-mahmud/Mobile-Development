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
    [Activity(Label = "Insert Supplier Info")]
    public class FabricSupplierRollinfoInsert : BaseActivity
    {

        Button btnSave;
        Spinner spGrnID, spColor, spWUOM, spLUOM;
        Switch swHeadCut;
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
        List<string> ddlUOM;
        List<UOM> uomList;
        List<string> ddlColor;
        FabricSupplierRollAdapter adapter;
        string selectedGRN, selectedColor, selectedUOM;
        List<RollSettingsDBModel> rollList;
        TextView txtRollNo, tcSupWidth;
        EditText etSupRollNo, etOwnWidth, etWidthBW, etLengthBW;
        bool isHeadCut = false;
        RollSettingsDBModel roll;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            ISharedPreferences pref = Application.Context.GetSharedPreferences
                 ("_bitopi_UserInfo", FileCreationMode.Private);

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.FabricSupplierRollInfoInsertLayout);
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
            uomList = await repo.GetUOM();

            ddlUOM = uomList.Select(t => t.Unitname).ToList();
            spGrnID.Adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, ddlGRN.ToArray());
            spWUOM.Adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, ddlUOM.ToArray());
            spWUOM.SetSelection(ddlUOM.FindIndex(t => t == "Inch"));

            spLUOM.Adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, ddlUOM.ToArray());
            spLUOM.SetSelection(ddlUOM.FindIndex(t => t == "Yard"));

        }
        protected override void InitializeControl()
        {
            RLleft_drawer = FindViewById<RelativeLayout>(Resource.Id.RLleft_drawer);
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            FindViewById<ImageButton>(Resource.Id.btnDrawermenu).Visibility = ViewStates.Visible;
            btnSave = FindViewById<Button>(Resource.Id.btnSave);
            spGrnID = FindViewById<Spinner>(Resource.Id.spGRN);
            spColor = FindViewById<Spinner>(Resource.Id.spColor);
            spWUOM = FindViewById<Spinner>(Resource.Id.spWidthUOMValue);
            spLUOM = FindViewById<Spinner>(Resource.Id.spLengthUOMValue);
            txtRollNo = FindViewById<TextView>(Resource.Id.txtRollNoValue);
            tcSupWidth = FindViewById<TextView>(Resource.Id.tcSupWidth);
            swHeadCut = FindViewById<Switch>(Resource.Id.swHeadCut);
            etSupRollNo = FindViewById<EditText>(Resource.Id.etSupRollNo);
            etOwnWidth = FindViewById<EditText>(Resource.Id.etOwnWidth);
            etWidthBW = FindViewById<EditText>(Resource.Id.etWidthBW);
            etLengthBW = FindViewById<EditText>(Resource.Id.etLengthBW);
            etOwnWidth.TransformationMethod = null;
            etWidthBW.TransformationMethod = null;
            etLengthBW.TransformationMethod = null;
            etSupRollNo.TransformationMethod = null;
            base.InitializeControl();
        }
        protected override void InitializeEvent()
        {

            spGrnID.ItemSelected += SpGrnID_ItemSelected; ;
            spColor.ItemSelected += SpColor_ItemSelected;
            spWUOM.ItemSelected += SpUOM_ItemSelected;
            spLUOM.ItemSelected += SpUOM_ItemSelected;
            btnSave.Click += BtnSave_Click;
            swHeadCut.CheckedChange += SwHeadCut_CheckedChange;
            base.InitializeEvent();
        }

        private void SwHeadCut_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if(e.IsChecked)
            {
                etOwnWidth.Enabled = (true);
                etOwnWidth.Focusable = (true);
                //etOwnWidth.InputType = Android.Text.InputTypes.ClassNumber;
                isHeadCut = true;
                etOwnWidth.SetBackgroundResource(Resource.Drawable.rounded_textview);

                etOwnWidth.Enabled = (true);
                etOwnWidth.Focusable = (true);
                //etOwnWidth.InputType = Android.Text.InputTypes.ClassNumber;
                isHeadCut = true;
                etOwnWidth.SetBackgroundResource(Resource.Drawable.rounded_textview);

                etLengthBW.Enabled = (true);
                etLengthBW.Focusable = (true);
                
                etLengthBW.SetBackgroundResource(Resource.Drawable.rounded_textview);

                etWidthBW.Enabled = (true);
                etWidthBW.Focusable = (true);
                 
                etWidthBW.SetBackgroundResource(Resource.Drawable.rounded_textview);

                etWidthBW.Text = roll.SWidthBW;
                etLengthBW.Text = roll.SLengthBW;
            }
            else
            {
                etOwnWidth.Enabled = (false);
                etOwnWidth.Focusable = (false);
                etOwnWidth.FocusableInTouchMode = true;
                 
                isHeadCut = false;
                etOwnWidth.SetBackgroundResource(Resource.Drawable.rounded_textview_disabled);

                etLengthBW.Enabled = (false);
                etLengthBW.Focusable = (false);
                etLengthBW.FocusableInTouchMode = true;
                etLengthBW.Text = "";
                etLengthBW.SetBackgroundResource(Resource.Drawable.rounded_textview_disabled);

                etWidthBW.Enabled = (false);
                etWidthBW.Focusable = (false);
                etWidthBW.FocusableInTouchMode = true;
                etWidthBW.Text = "";
                etWidthBW.SetBackgroundResource(Resource.Drawable.rounded_textview_disabled);
            }
        }

        private void SpUOM_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            selectedUOM = ddlUOM[e.Position];
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            RollSettingsDBModel model = new RollSettingsDBModel()
            {
                SupplierRollNo = etSupRollNo.Text,
                OwnWidth = etOwnWidth.Text,
                WidthBeforeWash = etWidthBW.Text,
                SLengthBW = etLengthBW.Text,
                WidthUOM = selectedUOM,
                RollID = txtRollNo.Tag.ToString(),
                HeadCutting= isHeadCut
            };

            int result = await repo.SetRoll(model);
            if (result > 0)
            {
                roll = await repo.GetRollID(selectedGRN, selectedColor);
                txtRollNo.Text = roll.RollSerial.ToString();
                txtRollNo.Tag = roll.RollID;

                etSupRollNo.Text = "";
                etOwnWidth.Text = "";
                etWidthBW.Text = "";
                etLengthBW.Text = "";

            }
            else
            {
                Toast.MakeText(this, "SAVE FAILED!!!! Please Try Again", ToastLength.Long);
            }
        }

        private async void SpColor_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            //throw new NotImplementedException();
            selectedColor = ddlColor[e.Position];


            roll = await repo.GetRollID(selectedGRN, selectedColor);
            txtRollNo.Text = roll.RollSerial.ToString();
            txtRollNo.Tag = roll.RollID;
            etOwnWidth.Text = roll.OwnWidth;
            tcSupWidth.Text = roll.SupplierWidth.ToString();
            if (swHeadCut.Checked)
            {
                etWidthBW.Text = roll.SWidthBW;
                etLengthBW.Text = roll.SLengthBW;
            }
            else
            {

                etWidthBW.Text = "";
                etLengthBW.Text = "";
            }
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