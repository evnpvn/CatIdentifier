using System;
using Newtonsoft.Json;

namespace NovaLunaIdentifier
{
    /// <summary>
    /// Functionality for parsing MS cognitive services results into a type 
    /// and determining results for Nova and Luna 
    /// </summary>
    /// 
    public class Prediction
    {
        [JsonProperty("probability")]
        public double Probability { get; set; }

        [JsonProperty("tagId")]
        public Guid TagId { get; set; }

        [JsonProperty("tagName")]
        public string Tag { get; set; }
    }
    public class CustomVision
    {
        public string LunaPrediction { get; set; }

        public string NovaPrediction { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("project")]
        public Guid Project { get; set; }

        [JsonProperty("iteration")]
        public Guid Iteration { get; set; }

        [JsonProperty("created")]
        public DateTimeOffset Created { get; set; }

        [JsonProperty("predictions")]
        public Prediction[] PredictionContents { get; set; }


        public void Deserialize(string jsonContent, out CustomVision prediction)
        {
            prediction = JsonConvert.DeserializeObject<CustomVision>(jsonContent);
        }

        public void DetermineResults()
        {
            foreach (Prediction prediction in PredictionContents)
            {
                if (prediction.Tag == "Luna")
                {
                    LunaPrediction = String.Format("{0:P2} chance", prediction.Probability);
                }
                else if (prediction.Tag == "Nova")
                {
                    NovaPrediction = String.Format("{0:P2} chance", prediction.Probability);
                }
            }
        }
    }
}