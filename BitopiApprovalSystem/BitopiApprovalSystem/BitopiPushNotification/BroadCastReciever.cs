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
using Android.Util;
using System.Net.Http;
using BitopiApprovalSystem.BitopiPushNotification;

namespace BitopiApprovalSystem.PushNotification
{

    [BroadcastReceiver]
    class BitopiBroadCastReciever : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            ////TytMoblieApplication applicaiton = (TytMoblieApplication)context.ApplicationContext;
            ////var currentActivity = applicaiton.ActiveActivity;
            //if(applicaiton.ActiveActivity is TruckListActivity)
            //{
            //    currentActivity=(TruckListActivity)applicaiton.ActiveActivity;
            //}
            //else if(applicaiton.ActiveActivity is LastFiveLocationActivity)
            //{
            //    currentActivity=(LastFiveLocationActivity)applicaiton.ActiveActivity;
            //}
            //else if(applicaiton.ActiveActivity is ReportListActivity)
            //{
            //    currentActivity=(ReportListActivity)applicaiton.ActiveActivity;
            //}
            //if(applicaiton.ActiveActivity is MapReplayActivity)
            //{
            //    currentActivity=(MapReplayActivity)applicaiton.ActiveActivity;
            //}
            //if (currentActivity != null)
            //    currentActivity.ApplyRoles();
            context.StartService(new Intent(context, typeof(BitopiNotificationService)));
        }
    }
    [BroadcastReceiver]
    class TYTOnUnInstallReciever : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            //TytMoblieApplication applicaiton = (TytMoblieApplication)context.ApplicationContext;
            //UserRepository repo = new UserRepository();
            //repo.ActiveDeactiveNotificaion(0, applicaiton.MacAddress, "D");

        }
    }

}