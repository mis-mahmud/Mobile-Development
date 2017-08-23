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
using System.IO;
using Java.Util;
using Java.IO;
using BitopiApprovalSystem.Library;

namespace BitopiApprovalSystem
{
    public class UID
    {
        private static String sID = null;
        private static String INSTALLATION = "bitopiAppUid";

        public static String id(Context context)
        {
            if (sID == null || sID == String.Empty)
            {
                //Java.IO.File installation = new Java.IO.File(context.FilesDir, INSTALLATION);
                //var documents = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                //string documents = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "BITOPIUID");
                string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                var documents = System.IO.Path.Combine(folder, "");
                if (!System.IO.File.Exists(documents))
                {
                    Java.IO.File dir = new Java.IO.File(documents);
                    //bool test = dir.Mkdir();
                }
                var filename = System.IO.Path.Combine(documents, INSTALLATION);
                try
                {
                    //var documents = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                    //var filepath = System.IO.Path.Combine(documents, INSTALLATION);
                    //if (!installation.Exists())
                    if (!System.IO.File.Exists(filename))
                    {
                        //writeInstallationFile(installation);
                        writeInstallationFile();
                    }
                    //sID = readInstallationFile(installation);
                    sID = readInstallationFile();
                }
                catch (Exception ex)
                {
                    //string s = e.Message;
                    CustomLogger.CustomLog("From Activity: " + BitopiSingelton.Instance.CurrentActivity + "\nMessage: " + ex.Message + "\nStack Trace: " + ex.StackTrace + "\n\n", "", BitopiSingelton.Instance.User != null ?
                             BitopiSingelton.Instance.User.UserName : "");
                }
            }

            sID = sID.Replace(System.Environment.NewLine, "").Trim();
            return sID;
        }

        //private static String readInstallationFile(File installation)
        //{
        //    RandomAccessFile f = new RandomAccessFile(installation, "r");
        //    byte[] bytes = new byte[(int)f.Length()];
        //    f.ReadFully(bytes);
        //    f.Close();
        //    return new Java.Lang.String(bytes).ToString();
        //}
        private static String readInstallationFile(Java.IO.File installation)
        {
            RandomAccessFile f = new RandomAccessFile(installation, "r");
            byte[] bytes = new byte[(int)f.Length()];
            f.ReadFully(bytes);
            f.Close();
            return new Java.Lang.String(bytes).ToString();

        }
        private static String readInstallationFile()
        {
            var text = "";
            //var documents = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            string documents = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            string filename = Path.Combine(documents, "BITOPIUID");
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            if (System.IO.File.Exists(filename))
            {
                text = System.IO.File.ReadAllText(filename);
            }

            return text;

        }

        private static void writeInstallationFile(Java.IO.File installation)
        {
            FileOutputStream outFile = new FileOutputStream(installation);
            Java.Lang.String id = new Java.Lang.String(UUID.RandomUUID().ToString());
            outFile.Write(id.GetBytes());
            outFile.Close();
            //string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments); //Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            //string filename = Path.Combine(path, fileName);

            //using (var streamWriter = new StreamWriter(filename, true))
            //{
            //    streamWriter.WriteLine(UUID.RandomUUID().ToString());
            //}
        }
        private static void writeInstallationFile()
        {
            try
            {
                //string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments); //Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                //string documents = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "BITOPIUID");
                string documents = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                string filename = Path.Combine(documents, "BITOPIUID");
                string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                
                using (var streamWriter = new StreamWriter(filename, false))
                {

                    streamWriter.WriteLine(UUID.RandomUUID().ToString().Replace(System.Environment.NewLine, "").Trim());
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                CustomLogger.CustomLog("From Activity: " + BitopiSingelton.Instance.CurrentActivity + "\nMessage: " + ex.Message + "\nStack Trace: " + ex.StackTrace + "\n\n", "", BitopiSingelton.Instance.User != null ?
                             BitopiSingelton.Instance.User.UserName : "");
            }
        }
    }
}