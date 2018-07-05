using System;
using System.IO;
using Android;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Widget;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using File = Java.IO.File;
using IOException = Java.IO.IOException;

namespace App5
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_main);

			Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

			FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;
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

            GetNewScreenshots();
        }


	    public void GetNewScreenshots()
	    {
	        //check for permission
	        //if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) == Permission.Denied)
	        //{
	        //    //ask for permission
	        //    RequestPermissions(new String[] {Manifest.Permission.ReadExternalStorage}, REQUEST_CODE); //READ_EXTERNAL_STORAGE_PERMISSION_CODE);
	        //}

            // https://stackoverflow.com/questions/9667297/path-to-screenshots-in-android

            var pix = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim); // DirectoryPictures
            var screenshotsDir = new File(pix, "Screenshots");

	        var files = screenshotsDir.ListFiles();

	        var label = FindViewById<TextView>(Resource.Id.myTextView);

	        label.Text = screenshotsDir.AbsolutePath + (screenshotsDir.Exists() ? " exist! " : " not exist! ") + " pictures count = " + (files != null ? files.Length : 0);


            var test = new File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "Screenshots");
	        label.Text += "  " + test.AbsolutePath + (test.Exists() ? " exist! " : " not exist! ") + " count " + (test.ListFiles() != null ? test.ListFiles().Length : 0);
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

    }
}

