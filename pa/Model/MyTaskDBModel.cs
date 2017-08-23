using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitopiApprovalSystem.Model
{
    public class MyTaskDBModel
    {
        public string OrderRefNo { get; set; }
        public string Buyer { get; set; }
        public string FileRefNo { get; set; }
        public string BuyerStyleRef { get; set; }
        public string PoNo { get; set; }
        public string ShipmentDate { get; set; }
        public string LotNo { get; set; }
        public string Task { get; set; }
        public string PlannedDate { get; set; }
        public string CommittedDate { get; set; }
        public string Asignee { get; set; }
        public string Seen { get; set; }
        public string Remarks { get; set; }
        public string ApprovalNature { get; set; }
        public string CheckList { get; set; }
        public string ActualDate { get; set; }
        public string Status { get; set; }
        public string Complete { get; set; }
        public string SeenDate { get; set; }
        public string ChekList { get; set; }
        public int OrderRefTaskTemplateChildId { get; set; }
        public bool IsDisabled { get; set; } = false;
    }
    public class MyTaskCountDBModel
    {
        public string TotalUnSeenTask { get; set; }
        public string TotalSeenTask { get; set; }
        public string TotalCompleteTask { get; set; }
    }
}
