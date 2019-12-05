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

        protected async void OnButtonClicked (object sender, EventArgs e)
        {
            try
            {
                var crossFilePicker = Plugin.FilePicker.CrossFilePicker.Current;
                var myResult = await crossFilePicker.PickFile();
                if (!string.IsNullOrEmpty(myResult.FileName)) //Just the file name, it doesn't has the path
                {
                    string _fileName = myResult.FilePath;
                    Console.WriteLine("deepak     "+_fileName);
                    if (File.Exists(_fileName))
                    {
                        editor.Text = File.ReadAllText(_fileName);
                    }
                    /*foreach (byte b in myResult.DataArray) //Empty array
                        b.ToString();*/
                }
            }
            catch (InvalidOperationException ex)
            {
                ex.ToString(); //"Only one operation can be active at a time"
            }
        }

    }

}