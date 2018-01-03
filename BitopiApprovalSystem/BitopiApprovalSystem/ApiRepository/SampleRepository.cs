using BitopiApprovalSystem;
using BitopiApprovalSystem.Model;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiRepository
{
    public class SampleRepository
    {
        public async Task<List<SampleProcessModel>> GetSampleRequisition()
        {
            List<SampleProcessModel> aproves;
            try
            {
                //userid = Cipher.Encrypt(userid);
                string url = RepositorySettings.BimobAppsBaseURl + "SampleProcess/GetSampleRequisition";

                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                {
                    HttpClient client = new HttpClient();
                    cookieContainer.Add(new Uri(RepositorySettings.BimobAppsBaseURl), new Cookie("ASP.NET_SessionId", "123"));
                    HttpResponseMessage result = await client.GetAsync(url);
                    string strResult = result.Content.ReadAsStringAsync().Result;
                    aproves = JsonConvert.DeserializeObject<List<SampleProcessModel>>(strResult);
                    if (aproves == null)
                        aproves = new List<SampleProcessModel>();
                }

                ;
            }
            catch (Exception ex)
            {
                aproves = new List<SampleProcessModel>();
            }
            return aproves;
        }
        public async Task<int> UpdateOrderRcvd(string SampleID, string ActualSewingDt, string UserSewingDt)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
    new KeyValuePair<string, string>("SampleID", SampleID),
    new KeyValuePair<string, string>("ActualSewingDt", ActualSewingDt),
    new KeyValuePair<string, string>("UserSewingDt", UserSewingDt)
});
            string url = RepositorySettings.BimobAppsBaseURl + "SampleProcess/UpdateOrderRcvd";
            HttpClient client = new HttpClient();
            HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(""), Encoding.UTF8,
"application/json");

            var response = await client.PostAsync(url, formContent);

            var aproves = JsonConvert.DeserializeObject<int>(response.Content.ReadAsStringAsync().Result);
            return aproves;
        }
        public async Task<int> UpdateMaterialRcvd(string SampleID)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
    new KeyValuePair<string, string>("SampleID", SampleID)
});
            string url = RepositorySettings.BimobAppsBaseURl + "SampleProcess/UpdateMaterialRcvd";
            HttpClient client = new HttpClient();
            HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(""), Encoding.UTF8,
"application/json");

            var response = await client.PostAsync(url, formContent);

            var aproves = JsonConvert.DeserializeObject<int>(response.Content.ReadAsStringAsync().Result);
            return aproves;
        }
        public async Task<List<BitopiGcmMessage>> GetNotification(string UserID, string DeviceID)
        {
            UserID = Cipher.Encrypt(UserID);
            string url = RepositorySettings.BaseURl + "Notification?UserID=" + UserID + "&DeviceID=" + DeviceID;

            HttpClient client = new HttpClient();
            HttpResponseMessage result = await client.GetAsync(url);
            var aproves = JsonConvert.DeserializeObject<List<BitopiGcmMessage>>(result.Content.ReadAsStringAsync().Result);
            if (aproves == null)
                aproves = new List<BitopiGcmMessage>();
            return aproves;
        }

        public async Task<List<ApprovalDetailsModel>> GetPOApprovalDetails(string userid, ApprovalRoleType roleType, ApprovalType approvalType)
        {
            userid = Cipher.Encrypt(userid);
            string url = RepositorySettings.BaseURl + "Approval?userid=" + userid + "&roleType=" + roleType + "&approvalType=" + approvalType;

            HttpClient client = new HttpClient();
            HttpResponseMessage result = await client.GetAsync(url);
            return JsonConvert.DeserializeObject<List<ApprovalDetailsModel>>(result.Content.ReadAsStringAsync().Result);
        }
        public async Task<int> SavePOApprovalDetails(string ApprovalID, string ApproveByID, string ApprovedBy, string ApprovaStatus, ApprovalType approvalType,
           ApprovalRoleType approvalRoleType, string Remarks = "")
        {
            ApproveByID = Cipher.Encrypt(ApproveByID);
            string url = RepositorySettings.BaseURl + "Approval?ApprovalID=" + ApprovalID + "&ApproveByID=" + ApproveByID
                + "&ApprovedBy=" + ApprovedBy + "&ApprovaStatus="
                + ApprovaStatus + "&approvalType=" + approvalType + "&approvalRoleType=" + approvalRoleType + "&Remarks=" + Remarks;

            HttpClient client = new HttpClient();

            HttpResponseMessage result = await client.PostAsync(url, null);
            return JsonConvert.DeserializeObject<int>(result.Content.ReadAsStringAsync().Result);
        }
        public async Task<int> ReceiveNotification(string userId, string deviceId, ApprovalType approvalType, string ApprovalName, string requisitioId)
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
