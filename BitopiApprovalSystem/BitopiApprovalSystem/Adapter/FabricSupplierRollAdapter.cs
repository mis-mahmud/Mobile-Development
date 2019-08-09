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
using Model;
using ApiRepository;
using Android.Graphics.Drawables;
using Android.Graphics;
using BitopiApprovalSystem.Model;
using static Android.Views.View;

namespace BitopiApprovalSystem.Adapter
{
    public class FabricSupplierRollAdapter : BaseAdapter
    {
        List<RollSettingsDBModel> _list;
        FabricSupplierRollinfo _context;
        Filter filter;
        string _status;
        ListView lvSupplierRollInfo;
        public string ProcessID { get; set; }
        int currentlyFocusedRow = -1;
        Input selectedInput;
        public FabricSupplierRollAdapter(List<RollSettingsDBModel> list, FabricSupplierRollinfo context, ListView lvSupplierRollInfo)
        {
            _list = list;
            _context = context;
            this.lvSupplierRollInfo = lvSupplierRollInfo;
        }
        public List<RollSettingsDBModel> Items
        {
            set { _list = value; }
            get { return _list; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            var model = Items[position];
            Holder holder = null;

            
            view = LayoutInflater.From(_context).Inflate(Resource.Layout.SupplierRollListItem, parent, false);

            holder = new Holder();

            holder.etSuppRollNo = view.FindViewById<TextView>(Resource.Id.etSuppRollNo);
            holder.BtnHeadCut = view.FindViewById<Button>(Resource.Id.btnHeadCut);
            holder.BtnInspection = view.FindViewById<Button>(Resource.Id.btnInspection);
            holder.BtnQuality = view.FindViewById<Button>(Resource.Id.btnQuality);
            view.FindViewById<TextView>(Resource.Id.txtRollId).Text = model.RollSerial.ToString();
            holder.etSuppRollNo.Text = model.SupplierRollNo;
            
            if ((position & 1) == 1)
            {
                view.SetBackgroundColor(Color.ParseColor("#E3F1F8"));
            }
            else
            {
                view.SetBackgroundColor(Color.ParseColor("#ffffff"));
            }
            holder.BtnHeadCut.Click += (s, e) =>
            {
                _context.LoadHeadCut(model);
                _context.FabricRollUpdateItem = FabricRollUpdateItem.HeadCut;

            };
            holder.BtnInspection.Click += (s, e) =>
            {
                _context.LoadInspection(model);
                _context.FabricRollUpdateItem = FabricRollUpdateItem.Inspection;
            };
            holder.BtnQuality.Click += (s, e) =>
            {
                _context.LoadQuality(model);
                _context.FabricRollUpdateItem = FabricRollUpdateItem.QC;
            };
            //holder.etSuppUOM.TextChanged += (s, e) =>
            //{
            //    model.SupplierWidthUOM = holder.etSuppUOM.Text;
            //};
            
            holder.RollID = model.SerialNo.ToString();
            holder.GroupPostiion = position;
            view.Tag = holder;

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

        class Holder : Java.Lang.Object
        {
            public string RollID { get; set; }

            public int GroupPostiion { get; set; }
            public TextView etSuppRollNo { get; set; }
            public Button BtnHeadCut { get; set; }
            public Button BtnQuality { get; set; }
            public Button BtnInspection { get; set; }

        }
        class CustomOnFocusChangeListener : Java.Lang.Object, IOnFocusChangeListener
        {
            FabricSupplierRollAdapter adapter; int position;
            ListView lvMyTask;
            Input input;
            public CustomOnFocusChangeListener(FabricSupplierRollAdapter adapter, int position, ListView lvMyTask, Input input)
            {
                this.adapter = adapter;
                this.position = position;
                this.lvMyTask = lvMyTask;
                this.input = input;
            }
            public void OnFocusChange(View v, bool hasFocus)
            {
                adapter.selectedInput = this.input;
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
        enum Input
        {
            WidthBW, SupplierRollNo, LengthBw, OwnWidth
        }
    }

}