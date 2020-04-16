using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace NovaLunaIdentifier
{
    /// <summary>
    /// Functionality for using http GET and POST requests with the MS cognitive services API
    /// </summary>
    public class WebServices
    {
        private const string PredictionKeyName = "Prediction-Key";
        private const string PredictionKeyValue = "a0998615d63b423387cfbb26be1de9d4";
        private const string BaseURL = "https://eastus.api.cognitive.microsoft.com/customvision/v3.0/Prediction/2a12297c-2b81-4b75-ab12-38bebfbbea6e/classify/iterations/NovaLunaIdentifier%20Iteration1/";

        public bool ImageIsLocal { get; set; }
        public string CatImageUrl { get; set; }

        private string ContentTypeValue
        {
            get 
            {   
                if (ImageIsLocal == true)   
                { 
                    return "application/octet-stream"; 
                }
                else 
                { 
                    return "application/json"; 
                }
            }
        }

        public string PredictionURL
        {
            get
            {   
                if (ImageIsLocal == true)
                {
                    return BaseURL + "image"; 
                }
                else 
                {
                    return BaseURL + "url";
                }
            }
        }

        private string Body
        {
            get 
            {
                return "{\"Url\": \"" + CatImageUrl + "\"}"; 
            }
        }

        public async Task<string> PredictionAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpContent content = new StringContent(Body);

                client.DefaultRequestHeaders.Add(PredictionKeyName, PredictionKeyValue);

                content.Headers.ContentType = new MediaTypeHeaderValue(ContentTypeValue);

                HttpResponseMessage response = client.PostAsync(PredictionURL, content).Result;

                string responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
        }

        public async Task<string> PredictionAsync(byte[] imageFile)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpContent content = new ByteArrayContent(imageFile);

                client.DefaultRequestHeaders.Add(PredictionKeyName, PredictionKeyValue);

                content.Headers.ContentType = new MediaTypeHeaderValue(ContentTypeValue);

                HttpResponseMessage response = client.PostAsync(PredictionURL, content).Result;

                string responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
        }

        public bool UrlChecker(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                Task<HttpResponseMessage> response = client.GetAsync(url);
                HttpContent content = response.Result.Content;
                string contentType = (content.Headers.GetValues("Content-Type")).First();

                if (contentType.StartsWith("image"))
                {
                    return true;
                }
                else return false;
            }
        }
    }
}
