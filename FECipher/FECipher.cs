using IGamePlugInBase;
using IGamePlugInBase.IO;
using System.Reflection;

namespace FECipher
{
    public class FECipher : IGamePlugIn
    {
        IFormat[] formatList;
        Version? currentVersion;

        public FECipher()
        {
            currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

            formatList = new IFormat[2]
            {
                new FEFormats("unlimited", "Unlimited", Properties.Resources.UnlimitedIcon, "All cards are allowed in this format from Series 1 to Series 22.", fecard => true),
                new FEFormats("standard", "Standard", Properties.Resources.StandardIcon,
                "The last Official Format of Fire Emblem Cipher, cards from Series 1 to Series 4 are not allowed in this format.",
                (fecard) => fecard.seriesNo > 4),
            };

            // Create Menu Items
            this.ImportMenus = new IImportMenuItem[1]
            {
                new LackeyCCGImport()
            };

            this.ExportMenus = new IExportMenuItem[1]
            {
                new LackeyCCGExport()
            };
        }

        public string Name { get => "FECipher"; }

        public string LongName { get => "Fire Emblem Cipher"; }

        public byte[] IconImage { get => Properties.Resources.Icon; }

        public string AboutInformation
        {
            get
            {
                string text = "Fire Emblem Cipher Multi-TCG Deck Builder Plug-In\n" +
                    (this.currentVersion != null ? string.Format("Version {0}\n", this.currentVersion.ToString()) : "") +
                    "Developed by Eronan\n" +
                    "-\n" +
                    "The Program is free and released under the \"GNU General Public License v3.0\". Any iterations on the program must be open-sourced.\n" +
                    "https://github.com/Eronan/Multi-TCG-Deck-Builder-FECipher-Plug-In/blob/master/LICENSE.md\n" +
                    "-\n" +
                    "Check for New Releases on GitHub:\n" +
                    "https://github.com/Eronan/Multi-TCG-Deck-Builder-FECipher-Plug-In/releases\n" +
                    "-\n" +
                    "To find Verified Plug-Ins that work with the latest versions of the Application, please visit: \n" +
                    "https://github.com/Eronan/Multi-TCG-Deck-Builder-FECipher-Plug-In/releases";

                return text;
            }
        }

        /// <summary>
        /// A List of Fire Emblem Formats
        /// </summary>
        public IEnumerable<IFormat> Formats { get => this.formatList; }

        /// <summary>
        /// A Service used to Download Necessary Files
        /// </summary>
        public IDownloader? Downloader { get => new FEDownloader(); }

        /// <summary>
        /// All of the Import Menu Items that should be defined for the Plug-In.
        /// </summary>
        public IEnumerable<IImportMenuItem>? ImportMenus { get; }

        /// <summary>
        /// All of the Export Menu Items that should be defined for the Plug-In.
        /// </summary>
        public IEnumerable<IExportMenuItem>? ExportMenus { get; }
    }
}