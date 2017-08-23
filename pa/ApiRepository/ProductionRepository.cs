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
    public class ProductionRepository
    {
        public async Task<List<ProdcutionAccountingDBModel>> GetProductionList(string userid, string ProcessID)
        {
            userid = Cipher.Encrypt(userid);
            string url = RepositorySettings.BaseURl + "ProdcutionAccounting?UserID=" + userid + "&ProcessID=" + ProcessID;

            HttpClient client = new HttpClient();
            HttpResponseMessage result = await client.GetAsync(url);
            var aproves = JsonConvert.DeserializeObject<List<ProdcutionAccountingDBModel>>(result.Content.ReadAsStringAsync().Result);
            if (aproves == null)
                aproves = new List<ProdcutionAccountingDBModel>();
            return aproves;
        }
        public async Task<List<DDL>> GetProductionDDL(string userid)
        {
            userid = Cipher.Encrypt(userid);
            string url = RepositorySettings.BaseURl + "ProdcutionAccounting?UserID=" + userid;

            HttpClient client = new HttpClient();
            HttpResponseMessage result = await client.GetAsync(url);
            var aproves = JsonConvert.DeserializeObject<List<DDL>>(result.Content.ReadAsStringAsync().Result);
            if (aproves == null)
                aproves = new List<DDL>();
            return aproves;
        }
        public async Task<int> SetProduction(string RefNO, int Qty, string AddedBy)
        {
            
            string url = RepositorySettings.BaseURl + "ProdcutionAccounting?RefNO=" + RefNO + "&Qty=" + Qty + "&ProdDateTime=" + DateTime.Now + "&AddedBy=" + AddedBy;

            HttpClient client = new HttpClient();
            HttpResponseMessage result = await client.GetAsync(url);
            var aproves = JsonConvert.DeserializeObject<int>(result.Content.ReadAsStringAsync().Result);
            
            return aproves;
        }


    }
}
