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

        private string _contentTypeValue;
        public string ContentTypeValue
        {
            get 
            {
                if (x == "image")   { return "application/octet-stream"; }
                else { return "application/json"; }
            }
            private set { _contentTypeValue = value; }
        }

        private string _predictionURL;
        public string PredictionURL
        {
            get
            {
                if (x == "image")   {return _predictionURL + "image"; }
                else { return _predictionURL + "url"; }
            }
            private set {  _predictionURL = "https://eastus.api.cognitive.microsoft.com/customvision/v3.0/Prediction/2a12297c-2b81-4b75-ab12-38bebfbbea6e/classify/iterations/NovaLunaIdentifier%20Iteration1/"; }
        }

        public string ImageURL { get; set; }

        private string _body;
        public string Body
        {
            get 
            {
                return "{\"Url\": \"" + ImageURL + "\"}"; 
            }
            private set { _body = value; }
        }


        public async Task<string> UrlPredictionAsync(string imageURL)
        {
            using (HttpClient client = new HttpClient())
            {
                //add the client headers
                client.DefaultRequestHeaders.Add(PredictionKeyName, PredictionKeyValue);

                HttpContent content = new StringContent(Body);

                content.Headers.ContentType = new MediaTypeHeaderValue(ContentTypeValue);
                Task<HttpResponseMessage> response = client.PostAsync(PredictionURL, content);

                string responseString = await response.Result.Content.ReadAsStringAsync();
                return responseString;
            }
        }

        //I wonder if this should just be an overload of a single method that takes either a URL or an image file
        //Then passes that off to another method that is the difference between this one and the other.

        //Take image and use an http: request to send to cognitive services
        public async Task<string> LocalfilePredictionAsync(byte[] imageFile)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add(PredictionKeyName, PredictionKeyValue);

                HttpContent content = new ByteArrayContent(imageFile);

                content.Headers.ContentType = new MediaTypeHeaderValue(ContentTypeValue);
                Task<HttpResponseMessage> response = client.PostAsync(PredictionURL, content);

                string responseString = await response.Result.Content.ReadAsStringAsync();
                return responseString;
            }
        }

        public bool UrlChecker(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                Task<HttpResponseMessage> response = client.GetAsync(url);
                HttpContent content = response.Result.Content;
                string contentType = (content.Headers.GetValues("Content-Type")).First();

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
    }
}
