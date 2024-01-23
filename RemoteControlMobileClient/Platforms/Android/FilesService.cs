using Java.IO;

namespace RemoteControlMobileClient.BusinessLogic.Services.Partial
{
    internal partial class FilesService
    {
        public partial async Task<bool> SaveFileAsync(byte[] fileBytes, string fileName)
        {
            if (Android.OS.Environment.IsExternalStorageManager)
            {
                Java.IO.File downloadsDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
                Java.IO.File file = new Java.IO.File(downloadsDir, fileName);
                try
                {
                    FileOutputStream outputStream = new FileOutputStream(file);
                    await outputStream.WriteAsync(fileBytes);
                    outputStream.Close();
                    return true;
                }
                catch (Java.IO.IOException e)
                {
                    e.PrintStackTrace();
                    return false;
                }
            }

            return false;
        }
    }
}
