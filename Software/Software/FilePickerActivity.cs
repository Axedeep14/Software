using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;

using System;
using System.IO;
using System.Linq;

namespace LNG.CMRI
{
    [Activity(ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    [Preserve(AllMembers = true)]
    public class FilePickerActivity : Activity
    {
        private bool IsFileSelected = false;

        private void StartPicker()
        {
            try
            {
                var intent = new Intent(Intent.ActionGetContent);

                intent.SetType("*/*");

                string[] allowedTypes = Intent.GetStringArrayExtra(ExtraAllowedTypes)?.Where(o => !string.IsNullOrEmpty(o) && o.Contains("/")).ToArray();

                if (allowedTypes != null && allowedTypes.Any())
                {
                    intent.PutExtra(Intent.ExtraMimeTypes, allowedTypes);
                }

                intent.AddCategory(Intent.CategoryOpenable);

                StartActivityForResult(Intent.CreateChooser(intent, "Select file"), 0);
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error("FilePickerActivity.StartPicker:", ex.Message);
                DoFinishActions();
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                if (resultCode == Result.Canceled)
                {
                    // Notify user file picking was cancelled.
                    Toast.MakeText(this, "file picking was cancelled", ToastLength.Short);
                }
                else
                {
                    try
                    {
                        if (data?.Data == null)
                        {
                            throw new Exception("File picking returned no valid data");
                        }

                        System.Diagnostics.Debug.Write(data.Data);

                        var uri = data.Data;

                        var filePath = FileUtils.GetPath(this, uri);

                        if (string.IsNullOrEmpty(filePath))
                        {
                            filePath = (uri.Scheme.StartsWith("content", StringComparison.OrdinalIgnoreCase)) ? uri.ToString() : uri.Path;
                        }

                        var fileName = FileUtils.GetFileName(this, uri);

                        FilePicked?.Invoke(this, (new FileDataReceivedArgs(filePath, uri, fileName)));
                        IsFileSelected = true;
                    }
                    catch (Exception ex)
                    {
                        Android.Util.Log.Error("FilePickerActivity.OnActivityResult", ex.Message);
                        Toast.MakeText(this, "file picking was cancelled", ToastLength.Short);
                    }
                }
            }
            finally
            {
                DoFinishActions();
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            StartPicker();
        }

        public const string ExtraAllowedTypes = "EXTRA_ALLOWED_TYPES";

        public const string ExtraSignal = "EXTRA_SIGNAL";

        public static event EventHandler<FileDataReceivedArgs> FilePicked;

        public void DoFinishActions()
        {
            if (!IsFileSelected)
            {
                FilePicked?.Invoke(this, (new FileDataReceivedArgs(string.Empty, null, string.Empty)));
            }

            Finish();
        }
    }
}