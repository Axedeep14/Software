using System;
using System.IO;
using Xamarin.Forms;

namespace Software
{
    public partial class MainPage : ContentPage
    {
        string _fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "notes.txt");

        public MainPage()
        {
            InitializeComponent();

            if (File.Exists(_fileName))
            {
                editor.Text = File.ReadAllText(_fileName);
            }
        }

        protected void OnSaveButtonClicked(object sender, EventArgs e)
        {
            File.WriteAllText(_fileName, editor.Text);
        }

        protected void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (File.Exists(_fileName))
            {
                File.Delete(_fileName);
            }
            editor.Text = string.Empty;
        }

        protected async void OneButtonClicked(object sender, EventArgs e)
        {
            string fileName = string.Empty;
            string filePath = string.Empty;

            SemaphoreSlim signal = new SemaphoreSlim(0, 1);
            EventHandler<FileDataReceivedArgs> handler = null;
            handler = (s, e1) => { fileName = e1.FileName; filePath = e1.FilePath; FilePickerActivity.FilePicked -= handler; signal.Release(); };

            try
            {
                // Set Allowed FileTypes here (MIME types) //
                string[] allowedTypes = { "application/x-x509-ca-cert", "application/x-509-user-cert", "application/x-509-server-cert", "application/pkix-cert", "application/x-pkcs12" };
                Intent mIntent = new Intent(Android.App.Application.Context, typeof(FilePickerActivity));
                mIntent.PutExtra(FilePickerActivity.ExtraAllowedTypes, allowedTypes);

                FilePickerActivity.FilePicked += handler;
                StartActivity(mIntent);

                await signal.WaitAsync();

                if (string.IsNullOrWhiteSpace(filePath))
                {
                    return;
                }

                /// Do Something with FilePath /// 
            }
            finally
            {
                FilePickerActivity.FilePicked -= handler;
            }
        }
    }
}