using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitopiApprovalSystemWebApiModels
{
    public class RollSettingsDBModel
    {
        public string RollMasterID { get; set; }
        public string Company { get; set; }
        public string Buyer { get; set; }
        public string Supplier { get; set; }
        public string GRNDate { get; set; }
        public int SerialNo { get; set; }
        public string RollSerial { get; set; }
        public string ColorID { get; set; }
        public string ColorName { get; set; }
        public string RollID { get; set; }
        public string Shade { get; set; }
        public string ShadeName { get; set; }
        public int RackID { get; set; }
        public string RackName { get; set; }
        public int RowNo { get; set; }
        public int ColNo { get; set; }
        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string LocationId { get; set; }
        public string LocationName { get; set; }
        public string RowColumn { get; set; }
        public string Type { get; set; }
        public string WidthBeforeWash { get; set; }
        public string WidthAfterWash { get; set; }
        public string WidthAfterWashParcent { get; set; }
        public string SLengthBW { get; set; }
        public string SWidthBW { get; set; }
        public string SLengthAW { get; set; }
        public string SWidthAW { get; set; }
        public string SLengthAWPercent { get; set; }
        public string SWidthAWParcent { get; set; }
        public decimal SupplierWidth { get; set; }
        public string SupplierWidthUOM { get; set; }
        public string SupplierLotNo { get; set; }
        public string SupplierRollNo { get; set; }
        public string OwnWidth { get; set; }
        public string WidthUOM { get; set; }
        public string LengthUOM { get; set; }
        public string UOM { get; set; }
        public bool HeadCutting { get; set; }
        public bool? QCPass { get; set; }
        public decimal OwnLength { get; set; }
    }
    public class UOM
    {
        public string Unitid { get; set; }
        public string Unitname { get; set; }
    }
}
