using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;

namespace BitopiApprovalSystem.DAL
{
    public class RecentPR
    {
        //[PrimaryKey, AutoIncrement]
        public int ID { get; set; }  
        public int EntryType { get; set; }
        public string RefID { get; set; }
        public string LocationRef { get; set; }
        
    }
    public class RecentHistory
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Process { get; set; }
        public string ProcessID { get; set; }
        public string Location { get; set; }
        public string LocationID { get; set; }
        public string EntryType { get; set; }
    }
    public enum EntryType
    {
        Production,Quality,Rejection
    }
    public class Version
    {
        [PrimaryKey]
        public int VersionCode { get; set; }
    }
}