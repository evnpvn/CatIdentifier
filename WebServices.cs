using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace NovaLunaIdentifier
{
    //is this class static?
    public class WebServices
    {
        public bool UrlChecker(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                Task<HttpResponseMessage> response = client.GetAsync(url);
                HttpContent content = response.Result.Content;
                string contentType = (content.Headers.GetValues("Content-Type")).First<string>();

                //change to a ternary if once this is working again
                //Format below:
                //condition ? consequent : alternative
                if (contentType.StartsWith("image"))
                {
                    return true;
                }
                else return false;
            }
        }

        public async Task<string> UrlPredictionAsync(string imageURL)
        {
            const string predictionURL = "https://eastus.api.cognitive.microsoft.com/customvision/v3.0/Prediction/2a12297c-2b81-4b75-ab12-38bebfbbea6e/classify/iterations/NovaLunaIdentifier%20Iteration1/url";
            const string predictionKeyName = "Prediction-Key";
            const string predictionKeyValue = "a0998615d63b423387cfbb26be1de9d4";
            const string contentTypeValue = "application/json";
            const string bodyPrefix = "{\"Url\": \"";
            const string bodySuffix = "\"}";

            using (HttpClient client = new HttpClient())
            {
                //add the client headers
                client.DefaultRequestHeaders.Add(predictionKeyName, predictionKeyValue);

                HttpContent content = new StringContent(bodyPrefix + imageURL + bodySuffix);

                content.Headers.ContentType = new MediaTypeHeaderValue(contentTypeValue);
                Task<HttpResponseMessage> response = client.PostAsync(predictionURL, content);

                //this here may need to be further refactored. Not sure if that await below or the one above is ideal.
                string responseString = await response.Result.Content.ReadAsStringAsync();
                return responseString;
            }
        }

        //I wonder if this should just be an overload of a single method that takes either a URL or an image file
        //Then passes that off to another method that is the difference between this one and the other.

        //Take image and use an http: request to send to cognitive services
        public async Task<string> LocalfilePredictionAsync(byte[] imageFile)
        {
            const string predictionURL = "https://eastus.api.cognitive.microsoft.com/customvision/v3.0/Prediction/2a12297c-2b81-4b75-ab12-38bebfbbea6e/classify/iterations/NovaLunaIdentifier%20Iteration1/image";
            const string predictionKeyName = "Prediction-Key";
            const string predictionKeyValue = "a0998615d63b423387cfbb26be1de9d4";
            const string contentTypeValue = "application/octet-stream";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add(predictionKeyName, predictionKeyValue);

                HttpContent content = new ByteArrayContent(imageFile);
                
                content.Headers.ContentType = new MediaTypeHeaderValue(contentTypeValue);
                Task<HttpResponseMessage> response = client.PostAsync(predictionURL, content);

                string responseString = await response.Result.Content.ReadAsStringAsync();
                return responseString;
            }
        }
    }
}
