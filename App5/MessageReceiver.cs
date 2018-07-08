using System;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Runtime;
using Android.Widget;

namespace App5
{
    internal class MessageReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Toast.MakeText(context, "Обнаружено сообщение: " + intent.Action, ToastLength.Long).Show();
        }
    }


    public class SettingsContentObserver : ContentObserver
    {
        private Context _context;

        public override bool DeliverSelfNotifications()
        {
            return false;
        }


        public override void OnChange(bool selfChange)
        {

            Toast.MakeText(_context, "Обнаружено сообщение: ", ToastLength.Long).Show();
        }

        public SettingsContentObserver(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public SettingsContentObserver(Context context, Handler handler) : base(handler)
        {
            _context = context;
        }
    }


}