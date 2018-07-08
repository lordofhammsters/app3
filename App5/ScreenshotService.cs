using System.Collections.Generic;
using Java.IO;
using Java.Lang;
using Java.Net;

namespace App5
{
    internal class ScreenshotService
    {
        private const string UpLoadServerUri = "http://37.18.74.149/test/home/test"; // "http://api.vegate.net/file";
        private List<string> _filesList = new List<string>();
        private bool _firstTime = true;

        public string Log;

        private File GetFolder()
        {
            // https://stackoverflow.com/questions/9667297/path-to-screenshots-in-android

            var screenshotsDir = new File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim), "Screenshots");

            if (screenshotsDir.Exists())
                return screenshotsDir;

            screenshotsDir = new File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "Screenshots");

            if (screenshotsDir.Exists())
                return screenshotsDir;

            return null;
        }

        private File TryGetNewScreenshot()
        {
            Log += "GetFolder\r\n";

            var folder = GetFolder();

            Log += "folder = " + (folder != null ? folder.AbsolutePath : "null") + "\r\n";

            if (folder == null)
                return null;

            var files = folder.ListFiles();

            if (files == null || files.Length == 0)
                return null;

            foreach (var file in files)
            {
                if (!_filesList.Contains(file.AbsolutePath))
                {
                    _filesList.Add(file.AbsolutePath);
                    if (!_firstTime)
                        return file;
                }
            }

            return null;
        }

        private bool SendFile(File file, out string response)
        {
            var result = false;
            response = "";

            try
            {
                // https://stackoverflow.com/questions/25398200/uploading-file-in-php-server-from-android-device/37953351

                HttpURLConnection conn = null;
                DataOutputStream dos = null;
                string lineEnd = "\r\n";
                string twoHyphens = "--";
                string boundary = "*****";
                int bytesRead, bytesAvailable, bufferSize;
                byte[] buffer;
                int maxBufferSize = 1 * 1024 * 1024;
                
                try
                {
                    // open a URL connection to the Servlet
                    FileInputStream fileInputStream = new FileInputStream(file);
                    URL url = new URL(UpLoadServerUri);

                    // Open a HTTP connection to the URL
                    conn = (HttpURLConnection)url.OpenConnection();

                    conn.DoInput = true; // Allow Inputs
                    conn.DoOutput = true; // Allow Outputs
                    conn.UseCaches = false; // Don't use a Cached Copy
                    conn.RequestMethod = "POST";
                    
                    conn.AddRequestProperty("Connection", "Keep-Alive");
                    conn.AddRequestProperty("ENCTYPE","multipart/form-data");
                    conn.AddRequestProperty("Content-Type","multipart/form-data;boundary=" + boundary);

                    conn.AddRequestProperty("file", file.AbsolutePath);

                    dos = new DataOutputStream(conn.OutputStream); //getOutputStream());

                    dos.WriteBytes(twoHyphens + boundary + lineEnd);
                    dos.WriteBytes("Content-Disposition: form-data; name=\"file\";filename=\""+ file.AbsolutePath + "\"" + lineEnd);

                    dos.WriteBytes(lineEnd);

                    // create a buffer of maximum size
                    bytesAvailable = fileInputStream.Available();

                    bufferSize = Math.Min(bytesAvailable, maxBufferSize);
                    buffer = new byte[bufferSize];

                    // read file and write it into form...
                    bytesRead = fileInputStream.Read(buffer, 0, bufferSize);

                    while (bytesRead > 0)
                    {
                        dos.Write(buffer, 0, bufferSize);
                        bytesAvailable = fileInputStream.Available();
                        bufferSize = Math.Min(bytesAvailable, maxBufferSize);
                        bytesRead = fileInputStream.Read(buffer, 0, bufferSize);
                    }

                    // send multipart form data necesssary after file
                    // data...
                    dos.WriteBytes(lineEnd);
                    dos.WriteBytes(twoHyphens + boundary + twoHyphens + lineEnd);

                    // Responses from the server (code and message)
                    var serverResponseCode = conn.ResponseCode;
                    

                    if (serverResponseCode == HttpStatus.Ok)
                    {
                        Log += "serverResponseCode = OK\r\n";

                        result = true;

                        var rd = new BufferedReader(new InputStreamReader(conn.InputStream));
                        var sb = new StringBuilder();
                        string line;

                        while ((line = rd.ReadLine()) != null)
                            sb.Append(line + '\r');
                        
                        rd.Close();

                        response = sb.ToString();
                    }

                    // close the streams
                    fileInputStream.Close();
                    dos.Flush();
                    dos.Close();
                }
                catch (Exception e)
                {
                    Log += "1 error " + e.StackTrace + "\r\n";

                    e.PrintStackTrace();
                }
            }
            catch (Exception ex)
            {
                Log += "2 error " + ex.StackTrace + "\r\n";

                ex.PrintStackTrace();
            }

            return result;
        }

        public string TryGetNewScreenshotAndSendToServer()
        {
            var screenshot = TryGetNewScreenshot();

            _firstTime = false;

            if (screenshot == null)
                return null;

            Log += "screenshot = " + screenshot.AbsolutePath + "\r\n";

            string response = null;

            return SendFile(screenshot, out response) ? response : null;

        }
    }
}