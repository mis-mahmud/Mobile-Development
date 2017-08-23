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
    
    public class TNAController : ApiController
    {
        DBTNA _dbTna;
        public TNAController()
        {
            _dbTna = new DBTNA();
        }
        [HttpGet]
        public List<MyTaskDBModel> Get(string userid,string TaskType)
        {
            userid = Cipher.Decrypt(userid);
            List<MyTaskDBModel> myTaskList = _dbTna.GetUnSeenTask(userid, TaskType);
            return myTaskList;
        }
        [HttpGet]
        public MyTaskCountDBModel Get(string userid)
        {
            userid = Cipher.Decrypt(userid);
            MyTaskCountDBModel myTaskList = _dbTna.GetTaskCount(userid);
            return myTaskList;
        }
        [HttpPost]
        public int SetTaskUnSeentoSeen(int OrderRefTaskTemplateChildId, string CommittedDate,string UserCode,string Remarks)
        {
            UserCode = Cipher.Decrypt(UserCode);
            int result = _dbTna.SetTaskUnSeentoSeen(new MyTaskDBModel {
                OrderRefTaskTemplateChildId= OrderRefTaskTemplateChildId,
                CommittedDate= CommittedDate,
                Remarks= Remarks
            }, UserCode);
            //int result = 1;


            return result;
        }
        [HttpPost]
        public int SetTaskSeenToComplete(int OrderRefTaskTemplateChildId,string UserCode)
        {
            UserCode = Cipher.Decrypt(UserCode);
            int result = _dbTna.SetTaskSeenToComplete(new MyTaskDBModel
            {
                OrderRefTaskTemplateChildId = OrderRefTaskTemplateChildId,
               
            }, UserCode);
            //int result = 1;


            return result;
        }
    }
}
