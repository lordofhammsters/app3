using System;
using System.IO;
using Android.App;
using Android.Graphics;
using Android.Widget;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using File = Java.IO.File;
using IOException = Java.IO.IOException;

namespace App3
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
	    private File[] _screensList;


		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_main);

			Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

			FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

		    GetNewScreenshots();
		}

	    public void GetNewScreenshots()
	    {
            // https://stackoverflow.com/questions/9667297/path-to-screenshots-in-android

            var pix = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);
            var screenshotsDir = new File(pix, "Screenshots");

	        var files = screenshotsDir.ListFiles();

            var label = FindViewById<TextView>(Resource.Id.myTextView);

	        label.Text = "hi! pictures count = " + (files != null ? files.Length : 0);
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
            ShareScreen();
            GetNewScreenshots();
        }

	    private void ShareScreen()
	    {
	        try
	        {
	            var photoName = "/test_" + DateTime.Now.ToString("dd_hh_mm_ss") + ".png";


                var pix = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);
	            var filePath = new File(pix, "Screenshots" + photoName); 

	            //if (filePath.Exists())
	            //    filePath.Delete();

	            SavePic(ScreenShot(Window.DecorView), filePath.AbsolutePath);
	        }
	        catch (Java.Lang.NullPointerException ignored)
	        {
	            ignored.PrintStackTrace();
	        }
	    }

	    private static void SavePic(Bitmap b, string strFileName)
	    {
	        try
	        {
	            var stream = new FileStream(strFileName, FileMode.Create);
	            b.Compress(Bitmap.CompressFormat.Png, 100, stream);
	            stream.Flush();
	            stream.Close();
	        }
	        catch (IOException e)
	        {
	            e.PrintStackTrace();
	        }
	        catch (System.Exception ex)
	        {

	        }
	    }

        private Bitmap ScreenShot(View view)
	    {
	        var bitmap = Bitmap.CreateBitmap(view.Width, view.Height, Bitmap.Config.Argb8888);
            var canvas = new Canvas(bitmap);
	        view.Draw(canvas);
	        return bitmap;
	    }


        //public class PlayerReceiver : BroadcastReceiver
        //{
        //    private static string TYPE = "type";
        //    private const int ID_ACTION_PLAY = 0;
        //    private static int ID_ACTION_STOP = 1;


        //    public override void OnReceive(Context context, Intent intent)
        //    {
        //        int type = intent.GetIntExtra(TYPE, ID_ACTION_STOP);
        //        switch (type)
        //        {
        //            case ID_ACTION_PLAY:
        //                context.StartService(new Intent(context, PlayService.class));
        //                break;
        //        }
        //    }
        //}
    }
}

