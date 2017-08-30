using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class UserModel
    {
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string EmployeeName { get; set; }
        public string Email { get; set; }

        public string Unit { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public byte[] EmpImage { get; set; }

        public string LeaveApproval { get; set; }
        public string LeaveRecommend { get; set; }
        public List<int> PermittedApproval { get; set; }
        public int VersionCode { get; set; }
    }
}
