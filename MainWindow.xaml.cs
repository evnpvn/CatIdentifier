using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Win32;

namespace NovaLunaIdentifier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //load all constant variables and settings
            const string profileFileDiretory = "C:\\Users\\EvanPavan\\Documents\\Documents\\2 - Personal\\Programming\\2 - Misc\\Udemy WPF course\\Cognitive Services\\Photos\\NovaLunaProfile_Photos\\";
            const string NovaFile = "NovaProfile.jpg";
            const string LunaFile = "LunaProfile.jpg";
            Nova.Source = new BitmapImage(new Uri(profileFileDiretory + NovaFile));
            Luna.Source = new BitmapImage(new Uri(profileFileDiretory + LunaFile));
        }

        private void ImageURL_TextChanged(object sender, TextChangedEventArgs e)
        {
            //check if the URL is valid
            if (Uri.IsWellFormedUriString(ImageURL.Text, UriKind.Absolute) == true)
            {
                //call method to check if link is an image
                WebServices webServices = new WebServices();
                webServices.CatImageUrl = ImageURL.Text;

                if (webServices.UrlChecker(webServices.CatImageUrl) == true)
                {
                    NovaResult.Text = "Calculating...";
                    LunaResult.Text = "Calculating...";
                    SelectedImage.Source = new BitmapImage(new Uri(webServices.CatImageUrl));

                    string responseString = webServices.PredictionAsync().Result;

                    Prediction prediction = new Prediction();
                    prediction.Deserialize(responseString, out prediction);
                    prediction.DetermineResults();
                   
                    LunaResult.Text = prediction.LunaPrediction;
                    NovaResult.Text = prediction.NovaPrediction;
                }
            }
        }

        private void AddPicture_Click(object sender, RoutedEventArgs e)
        {
            //Creates a new instance of the file directory
            //By default it filters for jpeg and png image types
            //Also looks in the MyPictures folder by default
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Image files |*.png;*.jpg;*jpeg |All files |*.*";
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            //Actually opens the file directory at this point
            fileDialog.ShowDialog();

            //The selected image file is set to a variable
            string filePath = fileDialog.FileName;

            //Checks to ensure the file type is the desired image and file then
            //The selected image file is set to the Image object in the XAML
            string fileExtension = System.IO.Path.GetExtension(filePath);

            if (filePath != null && filePath != "")
            {
                if (fileExtension == ".jpeg" || fileExtension == ".jpg" || fileExtension == ".png")
                {
                    //convert the file into a byte array because the content type that the Cogservices API expects is a byte stream (octet-stream)
                    byte[] imageFile = File.ReadAllBytes(filePath);
                    if (imageFile.Length < 4000000)
                    {
                        NovaResult.Text = "Calculating...";
                        LunaResult.Text = "Calculating...";
                        SelectedImage.Source = new BitmapImage(new Uri(filePath));

                        WebServices webServices = new WebServices();
                        string responseString = webServices.PredictionAsync(imageFile).Result;

                        Prediction prediction = new Prediction();
                        prediction.Deserialize(responseString, out prediction);
                        prediction.DetermineResults();

                        LunaResult.Text = prediction.LunaPrediction;
                        NovaResult.Text = prediction.NovaPrediction;

                    }
                    else
                    {
                        MessageBox.Show("Selected file is more than 4mb",
                                    "Unable to upload image", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Selected file is not a supported image file type. Please upload a JPEG or PNG image",
                                    "Unable to upload image", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}