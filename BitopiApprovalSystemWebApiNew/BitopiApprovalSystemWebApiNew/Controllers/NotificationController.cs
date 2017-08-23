using BitopiApprovalSystemWebApiModels;
using BitopiDBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BitopiApprovalSystemWebApiNew.Controllers
{
    public class NotificationController : ApiController
    {
        DBApproval dbApproval;
        public NotificationController()
        {
            dbApproval = new DBApproval();
        }
        public List<Message> GetNotification(string UserID, string DeviceID)
        {
            UserID = Cipher.Decrypt(UserID);
            List<Message> msg = dbApproval.GetNotification(UserID, DeviceID);
            return msg;
        }
        public int GetReceiveNotification(string userId, string deviceId, string approval, string requisitioId)
        {
            userId = Cipher.Decrypt(userId);
            int result = dbApproval.ReceiveNotification(userId, deviceId, approval, requisitioId);
            return result;
        }
    }
}
