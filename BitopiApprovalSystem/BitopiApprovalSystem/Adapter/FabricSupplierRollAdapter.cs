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

            holder.etSuppRollNo = view.FindViewById<EditText>(Resource.Id.etSuppRollNo);
            holder.etOwnWidth = view.FindViewById<EditText>(Resource.Id.etOwnWidth);
            holder.etWidthBW = view.FindViewById<EditText>(Resource.Id.etWidthBW);
            holder.etLengthBW = view.FindViewById<EditText>(Resource.Id.etLengthBW);
            view.FindViewById<TextView>(Resource.Id.txtRollId).Text = model.SerialNo.ToString();
            holder.etSuppRollNo.Text = model.SupplierRollNo;
            holder.etOwnWidth.Text = model.OwnWidth.ToString();
            holder.etWidthBW.Text = model.WidthBeforeWash.ToString();
            holder.etLengthBW.Text = model.SLengthBW.ToString();
            if ((position & 1) == 1)
            {
                view.SetBackgroundColor(Color.ParseColor("#E3F1F8"));
            }
            else
            {
                view.SetBackgroundColor(Color.ParseColor("#ffffff"));
            }
            holder.etWidthBW.TextChanged += (s, e) =>
            {
                model.WidthBeforeWash = holder.etWidthBW.Text;
            };
            holder.etOwnWidth.TextChanged += (s, e) =>
            {
                model.OwnWidth = holder.etOwnWidth.Text;
            };
            holder.etLengthBW.TextChanged += (s, e) =>
            {
                model.SLengthBW = holder.etLengthBW.Text;
            };
            //holder.etSuppUOM.TextChanged += (s, e) =>
            //{
            //    model.SupplierWidthUOM = holder.etSuppUOM.Text;
            //};
            if (currentlyFocusedRow == position)
            {
                switch (selectedInput)
                {
                    case Input.OwnWidth:
                        holder.etOwnWidth.RequestFocus();
                        break;
                    case Input.SupplierRollNo:
                        holder.etSuppRollNo.RequestFocus();
                        break;
                    case Input.WidthBW:
                        holder.etWidthBW.RequestFocus();
                        break;
                    case Input.LengthBw:
                        holder.etLengthBW.RequestFocus();
                        break;
                }

            }

            //(view.FindViewById<EditText>(Resource.Id.etRemarks)).FocusableInTouchMode = true;
            holder.etWidthBW.OnFocusChangeListener = new CustomOnFocusChangeListener(this, position, lvSupplierRollInfo, Input.WidthBW);
            holder.etSuppRollNo.OnFocusChangeListener = new CustomOnFocusChangeListener(this, position, lvSupplierRollInfo, Input.SupplierRollNo);
            holder.etLengthBW.OnFocusChangeListener = new CustomOnFocusChangeListener(this, position, lvSupplierRollInfo, Input.LengthBw);
            holder.etOwnWidth.OnFocusChangeListener = new CustomOnFocusChangeListener(this, position, lvSupplierRollInfo, Input.OwnWidth);

            holder.RollID = model.RollID.ToString();
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
            public EditText etSuppRollNo { get; set; }
            public EditText etOwnWidth { get; set; }
            public EditText etWidthBW { get; set; }
            public EditText etLengthBW { get; set; }

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