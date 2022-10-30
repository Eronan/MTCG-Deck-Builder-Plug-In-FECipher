using System.Text.Json.Serialization;

namespace FECipher
{
    public class FEAlternateArts
    {
        public FEAlternateArts(string id, string setCode, string imageLocation, string lackeyCCGId, string lackeyCCGName, int cipherVitId, string imageDownloadURL)
        {
            Id = id;
            SetCode = setCode;
            ImageLocation = imageLocation;
            LackeyCCGId = lackeyCCGId;
            LackeyCCGName = lackeyCCGName;
            CipherVitId = cipherVitId;
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

        [JsonPropertyName("CipherVitId")]
        [JsonPropertyOrder(5)]
        public int CipherVitId { get; set; }

        [JsonPropertyName("DownloadURL")]
        [JsonPropertyOrder(6)]
        public string ImageDownloadURL { get; set; }
    }
}
