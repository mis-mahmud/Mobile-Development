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
        Spinner spGrnID, spColor, spUOM;
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
        List<string> ddlUOM;
        List<UOM> uomList;
        List<string> ddlColor;
        FabricSupplierRollAdapter adapter;
        string selectedGRN, selectedColor, selectedUOM;
        List<RollSettingsDBModel> rollList;
        TextView txtRollNo;
        EditText etSupRollNo, etOwnWidth, etWidthBW, etLengthBW;
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
            spUOM.Adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, ddlUOM.ToArray());
            spUOM.SetSelection(ddlUOM.FindIndex(t => t == "Inch"));

        }
        protected override void InitializeControl()
        {
            RLleft_drawer = FindViewById<RelativeLayout>(Resource.Id.RLleft_drawer);
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            FindViewById<ImageButton>(Resource.Id.btnDrawermenu).Visibility = ViewStates.Visible;
            btnSave = FindViewById<Button>(Resource.Id.btnSave);
            spGrnID = FindViewById<Spinner>(Resource.Id.spGRN);
            spColor = FindViewById<Spinner>(Resource.Id.spColor);
            spUOM = FindViewById<Spinner>(Resource.Id.spUOMValue);
            txtRollNo = FindViewById<TextView>(Resource.Id.txtRollNoValue);

            etSupRollNo = FindViewById<EditText>(Resource.Id.etSupRollNo);
            etOwnWidth = FindViewById<EditText>(Resource.Id.etOwnWidth);
            etWidthBW = FindViewById<EditText>(Resource.Id.etWidthBW);
            etLengthBW = FindViewById<EditText>(Resource.Id.etLengthBW);
            base.InitializeControl();
        }
        protected override void InitializeEvent()
        {

            spGrnID.ItemSelected += SpGrnID_ItemSelected; ;
            spColor.ItemSelected += SpColor_ItemSelected;
            spUOM.ItemSelected += SpUOM_ItemSelected;
            btnSave.Click += BtnSave_Click;
            base.InitializeEvent();
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
                OwnWidthUOM = selectedUOM,
                RollID = txtRollNo.Tag.ToString()
            };

            int result = await repo.SetRoll(model);
            if (result > 0)
            {
                var roll = await repo.GetRollID(selectedGRN, selectedColor);
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


            var roll = await repo.GetRollID(selectedGRN, selectedColor);
            txtRollNo.Text = roll.RollSerial.ToString();
            txtRollNo.Tag = roll.RollID;
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