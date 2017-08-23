using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitopiDBContext
{
    public class NotificationSingleton
    {
        static NotificationSingleton _instance;
        public Dictionary<string, List<string>> RecieveMessage;
        NotificationSingleton()
        {
            RecieveMessage = new Dictionary<string, List<string>>();
        }
        public static NotificationSingleton Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NotificationSingleton();
                return _instance;
            }
        }
    }
}
