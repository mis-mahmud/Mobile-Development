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
        
        Button btnSave;
        Spinner spGrnID, spColor;
        Switch swLoadLastLocation;
        bool LastLocation;
        FabricRepository repo=new FabricRepository();
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
            spGrnID.Adapter = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, ddlGRN.ToArray() );      
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
            
           
            base.InitializeControl();
        }
        protected override void InitializeEvent()
        {

            spGrnID.ItemSelected += SpGrnID_ItemSelected; ;
            spColor.ItemSelected += SpColor_ItemSelected; ;
            btnSave.Click += BtnSave_Click;
            base.InitializeEvent();
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
        int result= await  repo.SetRoll(rollList);
            if(result>0)
            {
                rollList = await repo.GetRollList(selectedGRN, selectedColor);
                adapter.Items = rollList;
                adapter.NotifyDataSetChanged();
            }
        }

        private async void SpColor_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            //throw new NotImplementedException();
            selectedColor = ddlColor[e.Position];
            rollList = await repo.GetRollList(selectedGRN, selectedColor);
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