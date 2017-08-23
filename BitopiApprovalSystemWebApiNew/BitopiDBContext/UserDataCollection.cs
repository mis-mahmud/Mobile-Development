using BitopiApprovalSystemWebApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitopiDBContext
{
    public class UserDataCollection
    {
        public List<UserModel> UserList;
        static UserDataCollection instance;
        public Dictionary<int, ApprovalModel> ApprovalCollection;
        UserDataCollection()
        {
            UserList = new List<UserModel>();
            ApprovalCollection = new Dictionary<int, ApprovalModel>();
        }
        public static UserDataCollection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UserDataCollection();
                }
                return instance;
            }
        }
    }
}
