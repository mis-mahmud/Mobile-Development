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
    public class AccountRepository
    {
        public async Task<UserModel> getUser(string userName, string password, string DeviceID,
            string DeviceToken,
            string DeviceName,
            string Platform,
            int QryOption,int VersionCode, string UserCode="")
        {
            string url = RepositorySettings.BaseURl + "Account?userName=" + userName + "&Password=" + password
                + "&DeviceID=" + DeviceID + " &DeviceToken=" + DeviceToken
                + "&DeviceName=" + DeviceName + "&Platform=" + Platform+ "&QryOption="+QryOption+ "&UserCode="
                + UserCode+ "&VersionCode="+ VersionCode;
                
            HttpClient client = new HttpClient();
            HttpResponseMessage result= await client.GetAsync(url);         
            return JsonConvert.DeserializeObject<UserModel>(result.Content.ReadAsStringAsync().Result);
        }
    }
}
