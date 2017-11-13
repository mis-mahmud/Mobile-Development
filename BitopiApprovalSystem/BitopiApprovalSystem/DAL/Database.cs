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
using System.Threading.Tasks;


namespace BitopiApprovalSystem.DAL
{
    public interface IFileHelper
    {
        string GetLocalFilePath(string filename);
    }
    public static class FileHelper
    {
        public static string GetLocalFilePath(string filename)
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            return System.IO.Path.Combine(path, filename);
        }
    }
    public class BitopiDatabase
    {
        readonly SQLiteAsyncConnection db;

        public BitopiDatabase(string Path)
        {
            db = new SQLiteAsyncConnection(Path);
            db.CreateTableAsync<RecentPR>().Wait();
            db.CreateTableAsync<RecentHistory>().Wait();
            db.CreateTableAsync<Version>().Wait();
        }
        public Task<List<RecentPR>> RecentPRs
        {
            get
            {
                return db.Table<RecentPR>().ToListAsync();
            }
        }
        public Task<RecentHistory> RecentHistory
        {
            get
            {
                return db.Table<RecentHistory>().FirstOrDefaultAsync();
            }
        }
        public Task<int> SaveRecentHistory(RecentHistory item)

        {
            if (item.ID == 0)
            {
                return db.InsertAsync(item);
            }
            else
            {
                return db.UpdateAsync(item);
            }
        }
        public int LastVersion()
        {
            var result=db.Table<Version>().FirstOrDefaultAsync().Result;
            if (result == null)
                return 0;
            return result.VersionCode;
        }
        public int InsertVersion(int version)
        {
            return db.InsertAsync(new Version { VersionCode= version }).Result;
        }
        public int DeleteVersion(int Version)
        {
            return db.DeleteAsync(new Version { VersionCode = Version }).Result;
        }
        public void DropAllTable()
        {
            db.DropTableAsync<RecentHistory>();
            db.DropTableAsync<RecentPR>();
        }
        public async Task<int> SaveRecentPR(RecentPR item)
        {
            if (item.ID == 0)
            {
                item.ID = this.RecentPRs.Result.Where(t=>t.EntryType==item.EntryType).Count()+1;
                return await db.InsertAsync(item);
            }
            else
            {
                return await db.UpdateAsync(item);
            }
        }
        public async Task<int> DeleteItemAsync(RecentPR item)
        {
            return await db.DeleteAsync(item);
        }
        
    }
    public class DBAccess
    {
        static BitopiDatabase database;

        public static BitopiDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new BitopiDatabase(FileHelper.GetLocalFilePath("bitopiDB.db3"));
                }
                return database;
            }


        }
    }
}