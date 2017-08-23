using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitopiApprovalSystem.Model
{
    public class ApprovalModel
    {
        public ApprovalType Approval { get; set; }
        public string ApprovalName { get; set; }
        public int Count { get; set; }
        public ApprovalRoleType RoleType { get; set; }
    }
    public class ApprovalDetailsModel
    {
        public string POID { get; set; }

        public List<ApprovalDataModel> ApprovalDataList { get; set; }
        public bool isApproved { get; set; }
        public bool isDeleted { get; set; }
        public async Task<int> Save(string POStatus, Func<string,int> Action)
        {
            int result=  Action(POStatus);
            return result;
        }
    }
    public class ApprovalDataModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public class BitopiGcmMessage
    {
        public string POID { get; set; }
        public string ApprovalType { get; set; }
        public string ApprovalName { get; set; }
        public string Approval { get; set; }
        public DateTime DateCreated { get; set; }
        public int Count { get; set; }
    }
}
