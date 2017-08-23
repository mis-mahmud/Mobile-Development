using BitopiApprovalSystem;
using BitopiApprovalSystem.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiRepository
{
   public class ApprovalRepository
    {
        public async Task<List<ApprovalModel>> GetApprovals(string userid)
        {
            //userid = Cipher.Encrypt(userid);
            string url = RepositorySettings.BaseURl + "Approval?userid=" + userid;
            
            HttpClient client = new HttpClient();
            HttpResponseMessage result = await client.GetAsync(url);
            var aproves= JsonConvert.DeserializeObject<List<ApprovalModel>>(result.Content.ReadAsStringAsync().Result);
            if (aproves == null)
                aproves = new List<ApprovalModel>();
            return aproves;
        }
        public async Task<List<BitopiGcmMessage>> GetNotification(string UserID,string DeviceID)
        {
            UserID = Cipher.Encrypt(UserID);
            string url = RepositorySettings.BaseURl + "Notification?UserID="+ UserID+ "&DeviceID="+DeviceID;

            HttpClient client = new HttpClient();
            HttpResponseMessage result = await client.GetAsync(url);
            var aproves = JsonConvert.DeserializeObject<List<BitopiGcmMessage>>(result.Content.ReadAsStringAsync().Result);
            if (aproves == null)
                aproves = new List<BitopiGcmMessage>();
            return aproves;
        }

        public async Task<List<ApprovalDetailsModel>> GetPOApprovalDetails(string userid, ApprovalRoleType roleType,ApprovalType approvalType)
        {
            userid = Cipher.Encrypt(userid);
            string url = RepositorySettings.BaseURl + "Approval?userid=" + userid+"&roleType="+ roleType+ "&approvalType="+ approvalType;
            
            HttpClient client = new HttpClient();
            HttpResponseMessage result = await client.GetAsync(url);
            return JsonConvert.DeserializeObject<List<ApprovalDetailsModel>>(result.Content.ReadAsStringAsync().Result);
        }
        public async Task<int> SavePOApprovalDetails(string ApprovalID, string ApproveByID, string ApprovedBy, string ApprovaStatus, ApprovalType approvalType,
           ApprovalRoleType approvalRoleType, string Remarks="")
        {
            ApproveByID = Cipher.Encrypt(ApproveByID);
            string url = RepositorySettings.BaseURl + "Approval?ApprovalID=" + ApprovalID + "&ApproveByID=" + ApproveByID
                + "&ApprovedBy=" + ApprovedBy + "&ApprovaStatus=" 
                + ApprovaStatus + "&approvalType=" + approvalType+ "&approvalRoleType="+ approvalRoleType+ "&Remarks="+ Remarks;

            HttpClient client = new HttpClient();

            HttpResponseMessage result = await client.PostAsync(url,null);
            return JsonConvert.DeserializeObject<int>(result.Content.ReadAsStringAsync().Result);
        }
        public async Task<int> ReceiveNotification(string userId, string deviceId, ApprovalType approvalType,string ApprovalName, string requisitioId)
        {
            userId = Cipher.Encrypt(userId);
            string approval = (approvalType == ApprovalType.PurchaseOrderApproval) ? "PO" :
                (approvalType == ApprovalType.CashRequisition) ? "Cash Requisition" :
                (approvalType == ApprovalType.LeaveApplication) ? "Leave Application" :
                (approvalType == ApprovalType.ChequeRequisitionInformation) ? "ChequeRequisition" : "";
            string url = RepositorySettings.BaseURl + "Notification?userId=" + userId + "&deviceId=" + deviceId
                + "&approval=" + ApprovalName + "&requisitioId="
                + requisitioId;

            HttpClient client = new HttpClient();

            HttpResponseMessage result = await client.GetAsync(url);
            return JsonConvert.DeserializeObject<int>(result.Content.ReadAsStringAsync().Result);
        }
    }
}
