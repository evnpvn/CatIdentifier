using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NovaLunaIdentifier
{
    //class Prediction
    //{
    //    public double Probability { get; set; }
    //    public string TagName { get; set; }
    //    public string TagId { get; set; }
    //}

    public class PredictionHeader
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("project")]
        public Guid Project { get; set; }

        [JsonProperty("iteration")]
        public Guid Iteration { get; set; }

        [JsonProperty("created")]
        public DateTimeOffset Created { get; set; }

        [JsonProperty("predictions")]
        public Prediction[] Predictions { get; set; }
    }

    public class Prediction
    {
        [JsonProperty("probability")]
        public double Probability { get; set; }

        [JsonProperty("tagId")]
        public Guid TagId { get; set; }

        [JsonProperty("tagName")]
        public string Tag { get; set; }
    }
}