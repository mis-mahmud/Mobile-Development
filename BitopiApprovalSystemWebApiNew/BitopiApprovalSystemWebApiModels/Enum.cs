using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitopiApprovalSystemWebApiModels
{
    public enum ApprovalType
    {
        PurchaseOrderApproval = 1, LeaveApplication = 2, ChequeRequisitionInformation = 3, CashRequisition = 4,
        ExpressCashRequisition = 5, Precosting = 6,
        APInvoice = 7,
        DCN = 8,
        FileRef = 9,
        FreightBill = 10,
        ItemRequisition = 11,
        OrderCancel = 12,
        Procurement = 13,
        PerformaInvoice = 14,
        SampleApproval = 15,
        ShortShipmentApproval = 16,
        SPOReq = 17,
        SupplierApproval = 18,
        WorkOrder = 19,
        UnSeenTask = 20,
            SeenTask = 21,
            CompletedTask = 22,
            PORequisitionApproval=23

    }
    public enum ApprovalRoleType
    {
        Recommend = 1, Approve = 2
    }

}
