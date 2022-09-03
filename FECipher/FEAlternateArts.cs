using System.Text.Json.Serialization;
using IGamePlugInBase;

namespace FECipher
{
    public class FEAlternateArts
    {
        public FEAlternateArts(string id, string setCode, string imageLocation, string lackeyCCGId, string lackeyCCGName, string imageDownloadURL)
        {
            Id = id;
            SetCode = setCode;
            ImageLocation = imageLocation;
            LackeyCCGId = lackeyCCGId;
            LackeyCCGName = lackeyCCGName;
            ImageDownloadURL = imageDownloadURL;
        }

        [JsonPropertyName("CardCode")]
        [JsonPropertyOrder(0)]
        public string Id { get; set; }

        [JsonPropertyName("SetCode")]
        [JsonPropertyOrder(1)]
        public string SetCode { get; set; }

        [JsonPropertyName("ImageFile")]
        [JsonPropertyOrder(2)]
        public string ImageLocation { get; set; }

        [JsonPropertyName("LackeyCCGID")]
        [JsonPropertyOrder(3)]
        public string LackeyCCGId { get; set; }

        [JsonPropertyName("LackeyCCGName")]
        [JsonPropertyOrder(4)]
        public string LackeyCCGName { get; set; }

        [JsonPropertyName("DownloadURL")]
        [JsonPropertyOrder(5)]
        public string ImageDownloadURL { get; set; }
    }
}
