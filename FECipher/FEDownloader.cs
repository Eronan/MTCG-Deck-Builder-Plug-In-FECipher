using IGamePlugInBase.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FECipher
{
    internal class FEDownloader : IDownloader
    {
        private HttpClient? httpClient { get; set; }
        public string DownloadLink { get => "https://github.com/Eronan/Multi-TCG-Deck-Builder-FECipher-Plug-In/releases"; }

        public async Task DownloadFiles()
        {
            // https://raw.githubusercontent.com/Eronan/Multi-TCG-Deck-Builder-FECipher-Plug-In/master/FECipher/cardlist.json

            // Download Card List File
            if (!Directory.Exists("./plug-ins/fe-cipher"))
            {
                Directory.CreateDirectory("./plug-ins/fe-cipher");
            }

            if (httpClient == null)
            {
                this.httpClient = new HttpClient();
            }
            
            var jsonFile = await httpClient.GetStringAsync("https://raw.githubusercontent.com/Eronan/Multi-TCG-Deck-Builder-FECipher-Plug-In/master/FECipher/cardlist.json").ConfigureAwait(false);
            File.WriteAllText("./plug-ins/fe-cipher/cardlist.json", jsonFile);

            FECardList.ReloadCardList(this);

            httpClient.Dispose();
            httpClient = null;

            Console.WriteLine("Completed");
        }

        public async Task DownloadImage(string downloadURL, string imageLocation)
        {
            if (httpClient == null) { httpClient = new HttpClient(); }
            var response = await httpClient.GetAsync(downloadURL).ConfigureAwait(false);
            var fileInfo = new FileInfo(imageLocation);
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var fileStream = fileInfo.OpenWrite())
            {
                stream.CopyTo(fileStream);
            }
        }
    }
}
