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
             
            context.StartService(new Intent(context, typeof(BitopiNotificationService)));
        }
    }
    [BroadcastReceiver]
    class TYTOnUnInstallReciever : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            

        }
    }

}