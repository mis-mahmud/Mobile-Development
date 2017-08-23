using BitopiApprovalSystemWebApiModels;
using BitopiDBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace BitopiApprovalSystemWebApiNew.Controllers
{
    public class ApprovalController : ApiController
    {
        DBApproval dbApproval;
        public ApprovalController()
        {

            dbApproval = new DBApproval();
        }

        public List<ApprovalModel> GetApprovals(string userid)
        {
            //userid = Cipher.Decrypt(userid);
            List<ApprovalModel> approvalList = dbApproval.GetApprovalList(userid);


            return approvalList;
        }
        public List<ApprovalDetailsModel> GetPOApprovalDetails(string userid, ApprovalRoleType roleType, ApprovalType approvalType)
        {
            userid = Cipher.Decrypt(userid);
            List<ApprovalDetailsModel> approvalList = dbApproval.POApprovalDetails(userid, roleType, approvalType);


            return approvalList;
        }
        public int SavePOApprovalDetails(string ApprovalID, string ApproveByID, string ApprovedBy, string ApprovaStatus, ApprovalType approvalType,
           ApprovalRoleType approvalRoleType, string Remarks)
        {
            ApproveByID = Cipher.Decrypt(ApproveByID);
            int result = dbApproval.ProcessApproval(ApprovalID, ApproveByID, ApprovedBy, ApprovaStatus, approvalType,
            approvalRoleType, Remarks);
            //int result = 1;


            return result;
        }

    }
}
