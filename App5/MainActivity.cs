using System;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace App5
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
	    private TextView label;
	    private Handler handler;

        protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_main);

			Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

			FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            // 1 way to detect volume button click
            //var receiver = new MessageReceiver();
            //RegisterReceiver(receiver, new IntentFilter("android.media.VOLUME_CHANGED_ACTION"));  //"android.media.VOLUME_CHANGED_ACTION"));

		    // 2 way to detect volume button click
            var obs = new SettingsContentObserver(this, new Handler());
            ApplicationContext.ContentResolver.RegisterContentObserver(Android.Provider.Settings.System.ContentUri, true, obs);


            label = FindViewById<TextView>(Resource.Id.myTextView);
            label.Text = "hi!";

		    handler = new Handler(msg =>
		    {
		        label.Text = (string) msg.Obj;

                // add push notification
		        var n = new NotificationCompat.Builder(this);
		        n.SetSmallIcon(Resource.Drawable.navigation_empty_icon);
		        n.SetContentTitle((string)msg.Obj);
		        n.SetContentText("");
		        n.SetPriority(NotificationCompat.PriorityMax);

		        var notificationManager = NotificationManagerCompat.From(this);
		        notificationManager.Notify(1, n.Build());
		    });

		    var thread = new Thread(GetScreenshots);
            thread.Start();
		}

	    private void GetScreenshots()
	    {
	        var i = 0;
	        var service = new ScreenshotService();

	        while (true)
	        {
	            service.Log += "Trying " + i + "\r\n";

	            var response = service.TryGetNewScreenshotAndSendToServer();
	            if (!string.IsNullOrEmpty(response))
	            {
	                handler.SendMessage(new Message() {Obj = response});  // service.Log
	            }

	            Thread.Sleep(500);
	            i++;
	        }
	    }



	    public override bool OnCreateOptionsMenu(IMenu menu)
	    {
	        MenuInflater.Inflate(Resource.Menu.menu_main, menu);
	        return true;
	    }

	    public override bool OnOptionsItemSelected(IMenuItem item)
	    {
	        int id = item.ItemId;
	        if (id == Resource.Id.action_settings)
	        {
	            return true;
	        }

	        return base.OnOptionsItemSelected(item);
	    }

	    private void FabOnClick(object sender, EventArgs eventArgs)
	    {
	        //View view = (View) sender;
	        //Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
	        //    .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
	    }

    }
}

