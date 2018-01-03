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
    public class TNARepository
    {
        public async Task<List<MyTaskDBModel>> GetMyTask(string userid, string type = "UNSEEN")
        {
            userid = Cipher.Encrypt(userid);
            string url = RepositorySettings.BaseURl + "TNA?userid=" + userid + "&TaskType=" + type;

            HttpClient client = new HttpClient();
            HttpResponseMessage result = await client.GetAsync(url);
            var aproves = JsonConvert.DeserializeObject<List<MyTaskDBModel>>(result.Content.ReadAsStringAsync().Result);
            if (aproves == null)
                aproves = new List<MyTaskDBModel>();
            return aproves;
        }
        public async Task<MyTaskCountDBModel> GetTaskCount(string userid)
        {
            userid = Cipher.Encrypt(userid);
            string url = RepositorySettings.BaseURl + "TNA?userid=" + userid;

            HttpClient client = new HttpClient();
            HttpResponseMessage result = await client.GetAsync(url);
            var aproves = JsonConvert.DeserializeObject<MyTaskCountDBModel>(result.Content.ReadAsStringAsync().Result);
            if (aproves == null)
                aproves = new MyTaskCountDBModel();
            return aproves;
        }
        public async Task<int> SetTaskUnSeentoSeen(MyTaskDBModel _dbModel, string UserCode)
        {
            UserCode = Cipher.Encrypt(UserCode);
            string url = RepositorySettings.BaseURl + "TNA?OrderRefTaskTemplateChildId=" + _dbModel.OrderRefTaskTemplateChildId 
                + "&CommittedDate=" + _dbModel.CommittedDate
       + "&UserCode=" + UserCode + "&Remarks=" + _dbModel.Remarks;

            HttpClient client = new HttpClient();

            HttpResponseMessage result = await client.PostAsync(url, null);
            return JsonConvert.DeserializeObject<int>(result.Content.ReadAsStringAsync().Result);
        }
        public async Task<int> SetTaskSeenToComplete(MyTaskDBModel _dbModel, string UserCode)
        {
            UserCode = Cipher.Encrypt(UserCode);
            string url = RepositorySettings.BaseURl + "TNA?OrderRefTaskTemplateChildId=" + _dbModel.OrderRefTaskTemplateChildId           
       + "&UserCode=" + UserCode ;

            HttpClient client = new HttpClient();

            HttpResponseMessage result = await client.PostAsync(url, null);
            return JsonConvert.DeserializeObject<int>(result.Content.ReadAsStringAsync().Result);
        }
    }
}
