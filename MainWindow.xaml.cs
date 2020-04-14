﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;

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

            const string profileFileDiretory = "C:\\Users\\EvanPavan\\Documents\\Documents\\2 - Personal\\Programming\\2 - Misc\\Udemy WPF course\\Cognitive Services\\NovaLunaProfile_Photos\\";
            const string NovaFile = "NovaProfile.jpg";
            const string LunaFile = "LunaProfile.jpg";
            Nova.Source = new BitmapImage(new Uri(profileFileDiretory + NovaFile));
            Luna.Source = new BitmapImage(new Uri(profileFileDiretory + LunaFile));
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

            if (fileExtension == ".jpeg" || fileExtension == ".jpg" || fileExtension == ".png")
            {
                NovaResult.Text = "Calculating...";
                LunaResult.Text = "Calculating...";
                SelectedImage.Source = new BitmapImage(new Uri(filePath));
                MakePredictionLocalImageAsync(filePath);
            }
            else
            {
                MessageBox.Show("Selected file is not a supported image file type. Please upload a JPEG or PNG image",
                                "Unable to upload image", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        //TODO: add text "Calculating" while async is waiting the response from cognitive services.
        //TODO: Refactor shared code

        //TODO: Add functionality to add image via URL
        private void ImageURL_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(ImageURL.Text != null)
            {
                NovaResult.Text = "Calculating...";
                LunaResult.Text = "Calculating...";
                SelectedImage.Source = new BitmapImage(new Uri(ImageURL.Text));
                MakePredictionURLAsync(ImageURL.Text);
            }
        }
        
        private async void MakePredictionURLAsync(string imageURL)
        {
            const string predictionURL = "https://eastus.api.cognitive.microsoft.com/customvision/v3.0/Prediction/2a12297c-2b81-4b75-ab12-38bebfbbea6e/classify/iterations/NovaLunaIdentifier%20Iteration1/url";

            const string predictionKeyName = "Prediction-Key";
            const string predictionKeyValue = "a0998615d63b423387cfbb26be1de9d4";
            const string contentTypeValue = "application/json";

            const string bodyPrefix = "{\"Url\": \"";
            const string bodySuffix = "\"}";

            //TODO: create a new HTTP post to the prediction API
            using (HttpClient client = new HttpClient())
            {
                //add the client headers
                client.DefaultRequestHeaders.Add(predictionKeyName, predictionKeyValue);

                HttpContent content = new StringContent(bodyPrefix + imageURL + bodySuffix);

                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentTypeValue);
                HttpResponseMessage response = await client.PostAsync(predictionURL, content);

                string responseString = await response.Content.ReadAsStringAsync();

                //Deserialize the response
                //When I deserialize it I'll end up with a Collection<Predictions>
                PredictionHeader predictionhead = new PredictionHeader();
                predictionhead = (JsonConvert.DeserializeObject<PredictionHeader>(responseString));

                Prediction[] predictions = predictionhead.Predictions;
                foreach (Prediction prediction in predictions)
                {
                    if (prediction.Tag == "Luna")
                    {
                        LunaResult.Text = String.Format("{0:P2} chance", prediction.Probability);
                    }
                    else if (prediction.Tag == "Nova")
                    {
                        NovaResult.Text = String.Format("{0:P2} chance", prediction.Probability);
                    }
                }
            }
        }

        //Take image and use an http: request to send to cognitive services
        private async void MakePredictionLocalImageAsync(string filePath)
        {
            const string predictionURL = "https://eastus.api.cognitive.microsoft.com/customvision/v3.0/Prediction/2a12297c-2b81-4b75-ab12-38bebfbbea6e/classify/iterations/NovaLunaIdentifier%20Iteration1/image";
            
            const string predictionKeyName = "Prediction-Key";
            const string predictionKeyValue = "a0998615d63b423387cfbb26be1de9d4";
            const string contentTypeValue = "application/octet-stream";

            //convert the file into a byte array because the content type that the Cogservices API expects is a byte stream (octet-stream)
            byte[] file = File.ReadAllBytes(filePath);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add(predictionKeyName, predictionKeyValue);

                using (HttpContent content = new ByteArrayContent(file))
                {
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentTypeValue);
                    HttpResponseMessage response = await client.PostAsync(predictionURL, content);

                    string responseString = await response.Content.ReadAsStringAsync();

                    //Deserialize the response
                    //When I deserialize it I'll end up with a Collection<Predictions>
                    PredictionHeader predictionhead = new PredictionHeader();
                    predictionhead = (JsonConvert.DeserializeObject<PredictionHeader>(responseString));

                    Prediction[] predictions = predictionhead.Predictions;
                    foreach(Prediction prediction in predictions)
                    {
                        if(prediction.Tag == "Luna")
                        {
                            LunaResult.Text = String.Format("{0:P2} chance", prediction.Probability);
                        }
                        else if(prediction.Tag == "Nova")
                        {
                            NovaResult.Text = String.Format("{0:P2} chance", prediction.Probability);
                        }
                    }
                }
            }
        }
    }
}