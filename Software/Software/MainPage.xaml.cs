using System;
using System.IO;
using Xamarin.Forms;

namespace Software
{
    public partial class MainPage : ContentPage
    {
        string _filePath;
        public MainPage()
        {
            InitializeComponent();
        }
        protected async void OnFileSelect(object sender, EventArgs e)
        {
            try
            {
                var crossFilePicker = Plugin.FilePicker.CrossFilePicker.Current;
                var myResult = await crossFilePicker.PickFile();
                if (!string.IsNullOrEmpty(myResult.FileName)) //Just the file name, it doesn't has the path
                {
                    _filePath = myResult.FilePath;
                    if (File.Exists(_filePath))
                    {
                        editor.Text = File.ReadAllText(_filePath);
                    }
                }
           }
            catch (InvalidOperationException ex)
            {
                ex.ToString(); 
            }
        }

        private void OnEditFile(object sender, EventArgs e)
        {
            File.WriteAllText(_filePath, editor.Text);
            editor.Text = string.Empty;
            result.Text = File.ReadAllText(_filePath);
        }
    }

}