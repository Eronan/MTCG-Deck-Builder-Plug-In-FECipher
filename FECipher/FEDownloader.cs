using IGamePlugInBase.IO;

namespace FECipher
{
    internal class FEDownloader : IDownloader
    {
        public string DownloadLink { get => "https://github.com/Eronan/Multi-TCG-Deck-Builder-FECipher-Plug-In/releases"; }

        public IEnumerable<UrlToFile> FileDownloads => new UrlToFile[1] { new UrlToFile("https://raw.githubusercontent.com/Eronan/Multi-TCG-Deck-Builder-FECipher-Plug-In/master/FECipher/cardlist.json", "./plug-ins/fe-cipher/cardlist.json") };
    }
}
