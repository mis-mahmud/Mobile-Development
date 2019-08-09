using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using BitopiApprovalSystemWebApiNew.Models;
using BitopiApprovalSystemWebApiNew.Providers;
using BitopiApprovalSystemWebApiNew.Results;
using BitopiApprovalSystemWebApiModels;
using BitopiDBContext;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace BitopiApprovalSystemWebApiNew.Controllers
{
    public class AccountController : ApiController
    {
        DBUser userdb;
        public AccountController()
        {
            userdb = new DBUser();
        }
        public UserModel GetUserInfo(string userName, string Password, string DeviceID, string DeviceToken, string
            DeviceName, string Platform, int QryOption, string UserCode,string VersionCode)
        {
            Encryption encObj = new Encryption();

            string decryptUser1 = "";

            string decryptPwd1 = "";
            if (!String.IsNullOrEmpty(userName) && !String.IsNullOrEmpty(Password))
            {
                //decryptUser1 = encObj.EncryptWord(Cipher.Decrypt(userName));
                //decryptPwd1 = encObj.EncryptWord(Cipher.Decrypt(Password));

                decryptUser1 = encObj.EncryptWord(userName);
                decryptPwd1 = encObj.EncryptWord(Password);
            }

            UserModel model = userdb.GetUser(decryptUser1, decryptPwd1, DeviceID, DeviceToken,
             DeviceName, Platform, QryOption, UserCode, VersionCode);

            return model;
        }
        public MyTaskCountDBModel Get(string userid)
        {
            DBTNA _dbTna = new DBTNA();
            userid = Cipher.Decrypt(userid);
            userid = Cipher.Decrypt(userid);
            MyTaskCountDBModel myTaskList = _dbTna.GetTaskCount(userid);
            return myTaskList;
        }
    }
}
