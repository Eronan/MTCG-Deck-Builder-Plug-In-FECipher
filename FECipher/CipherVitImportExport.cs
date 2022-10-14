using IGamePlugInBase.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FECipher
{
    internal class CipherVitImport : IImportMenuItem
    {
        public string Header { get => "From FECipher Vit"; }

        public string DefaultExtension { get => ".fe0d"; }

        public string FileFilter { get => "FE Cipher Vit Deck (.fe0d)|*.fe0d"; }

        public DeckBuilderDeckFile Import(string filePath, string currentFormat)
        {
            throw new NotImplementedException();
        }
    }

    internal class CipherVitExport : IExportMenuItem
    {
        public string Header { get => "To FECipher Vit" }

        public string DefaultExtension { get => ".fe0d"; }

        public string FileFilter { get => "FE Cipher Vit Deck (.fe0d)|*.fe0d"; }

        public void Export(string filePath, DeckBuilderDeckFile decks)
        {
            throw new NotImplementedException();
        }
    }
}
