using IGamePlugInBase;
using Octokit;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;

namespace FECipher
{
    public class FECipher : IGamePlugIn
    {
        //Variables
        IFormat[] formatList;
        SearchField[] searchFieldList;
        Version? currentVersion;

        // Constructor
        public FECipher()
        {
            CardListInitialized = false;

            // Create Valid Formats
            formatList = new IFormat[2]
            {
                new FEFormats("unlimited", "Unlimited", Properties.Resources.UnlimitedIcon, "All cards are allowed in this format from Series 1 to Series 22.", fecard => true),
                new FEFormats("standard", "Standard", Properties.Resources.StandardIcon,
                "The last Official Format of Fire Emblem Cipher, cards from Series 1 to Series 4 are not allowed in this format.",
                (fecard) => fecard.seriesNo > 4),
            };

            // Create Search Fields
            string[] colours = new string[10] { "All", "Red", "Blue", "Yellow", "Purple", "Green", "Black", "White", "Brown", "Colorless" };
            this.searchFieldList = new SearchField[12]
            {
                new SearchField("character", "Character"),
                new SearchField("title", "Title"),
                new SearchField("color1", "Color", colours, "All"),
                new SearchField("color2", "Color", colours, "All"),
                new SearchField("cost", "Cost", 1),
                new SearchField("cccost", "Class Change Cost", 1),
                new SearchField("class", "Class"),
                new SearchField("type", "Type"),
                new SearchField("range", "Range", 0, 3),
                new SearchField("attack", "Attack", 3),
                new SearchField("support", "Support", 3),
                new SearchField("series", "Series", 0, 12),
            };


            // Create Menu Items
            this.ImportFunctions = new IImportMenuItem[1]
            {
                new LackeyCCGImport()
            };

            this.ExportFunctions = new IExportMenuItem[1]
            {
                new LackeyCCGExport()
            };

            currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
        }

        // Private Functions
        private bool MatchFields(FECard card, SearchField[] searchFields)
        {
            foreach (SearchField field in searchFields)
            {
                if (string.IsNullOrEmpty(field.Value) || (field.Value == "All" && (field.Id == "color1" || field.Id == "color2"))) continue;

                switch (field.Id)
                {
                    case "character":
                        // Comparison is Equals or Name Contains Value
                        if (field.Comparison == SearchFieldComparison.Equals ^ card.characterName.Contains(field.Value)) { return false; }
                        break;
                    case "title":
                        if (field.Comparison == SearchFieldComparison.Equals ^ card.characterTitle.Contains(field.Value)) return false;
                        break;
                    case "color1":
                    case "color2":
                        if (field.Comparison == SearchFieldComparison.Equals ^ Array.IndexOf(card.colors, field.Value) != -1) return false;
                        break;
                    case "cost":
                        if (field.Comparison == SearchFieldComparison.Equals ^ card.cost == field.Value) return false;
                        break;
                    case "cccost":
                        if (field.Comparison == SearchFieldComparison.Equals ^ card.classChangeCost == field.Value) return false;
                        break;
                    case "class":
                        if (field.Comparison == SearchFieldComparison.Equals ^ card.cardClass.Contains(field.Value)) return false;
                        break;
                    case "type":
                        if (field.Comparison == SearchFieldComparison.Equals ^ card.types.Any(item => item.Contains(field.Value))) return false;
                        break;
                    case "range":
                        int fieldValue = int.Parse(field.Value);
                        switch (field.Comparison)
                        {
                            case SearchFieldComparison.Equals:
                                if (card.minRange < fieldValue || card.maxRange > fieldValue) return false;
                                break;
                            case SearchFieldComparison.NotEquals:
                                if (card.minRange > fieldValue || card.maxRange < fieldValue) return false;
                                break;
                            case SearchFieldComparison.LessThan:
                                if (card.minRange > fieldValue) return false;
                                break;
                            case SearchFieldComparison.GreaterThan:
                                if (card.maxRange < fieldValue) return false;
                                break;
                        }
                        break;
                    case "attack":
                        if (field.Comparison == SearchFieldComparison.Equals ^ card.attack == field.Value) return false;
                        break;
                    case "support":
                        if (field.Comparison == SearchFieldComparison.Equals ^ card.support == field.Value) return false;
                        break;
                    case "series":
                        if (field.Comparison == SearchFieldComparison.Equals ^ card.seriesNo == int.Parse(field.Value)) return false;
                        break;
                }
            }
            return true;
        }

        private async Task<bool> DownloadImage(HttpClient httpClient, string downloadURL, string imageLocation)
        {
            var response = await httpClient.GetAsync(downloadURL);
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var fileInfo = new FileInfo(imageLocation);
                using (var fileStream = fileInfo.OpenWrite())
                {
                    await stream.CopyToAsync(fileStream);
                }
            }

            return true;
        }

        private void LoadCardList(HttpClient? httpClient = null)
        {
            string jsonText = File.ReadAllText("./plug-ins/fe-cipher/cardlist.json");
            JsonElement jsonDeserialize = JsonSerializer.Deserialize<dynamic>(jsonText);
            var jsonEnumerator = jsonDeserialize.EnumerateArray();

            // Get Card Data
            List<FECard> feCards = new List<FECard>();
            List<Task<bool>> downloadList = new List<Task<bool>>();
            foreach (var jsonCard in jsonEnumerator)
            {
                string? id = jsonCard.GetProperty("CardID").GetString();
                string? character = jsonCard.GetProperty("Character").GetString();
                string? title = jsonCard.GetProperty("Title").GetString();
                string[]? colors = jsonCard.GetProperty("Color").Deserialize<string[]>();
                string? cost = jsonCard.GetProperty("Cost").GetString();
                JsonElement classChangeProperty;
                string? cccost = null;
                if (jsonCard.TryGetProperty("ClassChangeCost", out classChangeProperty))
                {
                    cccost = classChangeProperty.GetString();
                }
                string? cardClass = jsonCard.GetProperty("Class").GetString();
                string[]? types = jsonCard.GetProperty("Type").Deserialize<string[]>();
                int minRange = jsonCard.GetProperty("MinRange").GetInt32();
                int maxRange = jsonCard.GetProperty("MaxRange").GetInt32();
                string? attack = jsonCard.GetProperty("Attack").GetString();
                string? support = jsonCard.GetProperty("Support").GetString();
                string? skill = jsonCard.GetProperty("Skill").GetString();
                JsonElement supportSkillProperty;
                string? supportSkill = null;
                if (jsonCard.TryGetProperty("SupportSkill", out supportSkillProperty))
                {
                    supportSkill = supportSkillProperty.GetString();
                }

                string? rarity = jsonCard.GetProperty("Rarity").GetString();
                int seriesNo = jsonCard.GetProperty("SeriesNumber").GetInt32();
                var altArtEnumerator = jsonCard.GetProperty("AlternateArts").EnumerateArray();
                List<FEAlternateArts> altArts = new List<FEAlternateArts>();

                foreach (var altArt in altArtEnumerator)
                {
                    string? code = altArt.GetProperty("CardCode").GetString();
                    string? setNo = altArt.GetProperty("SetCode").GetString();
                    string? image = altArt.GetProperty("ImageFile").GetString();
                    string? lackeyID = altArt.GetProperty("LackeyCCGID").GetString();
                    string? lackeyName = altArt.GetProperty("LackeyCCGName").GetString();
                    string? downloadURL = altArt.GetProperty("DownloadURL").GetString();

                    //Cannot be Null
                    if (code == null || setNo == null || image == null || lackeyID == null || lackeyName == null || downloadURL == null)
                    {
                        throw new ArgumentException("JSON Field AlternateArts is missing a Non-Nullable Property.");
                    }

                    FEAlternateArts alt = new FEAlternateArts(code, setNo, image, lackeyID, lackeyName, downloadURL);
                    altArts.Add(alt);

                    // Download File
                    if (httpClient != null)
                    {
                        if (!File.Exists(image))
                        {
                            var directoryPath = Path.GetDirectoryName(image);
                            if (directoryPath != null && !Directory.Exists(directoryPath))
                            {
                                Directory.CreateDirectory(directoryPath);
                            }

                            downloadList.Add(DownloadImage(httpClient, downloadURL, image));
                        }
                    }
                }

                if (id == null || character == null || title == null || colors == null || cost == null || cardClass == null || types == null ||
                    attack == null || support == null || skill == null || rarity == null)
                {
                    throw new ArgumentException(String.Format("JSON Object {0}: {1} is missing a Non-Nullabe Property.", character, title));
                }

                FECard card = new FECard(id, character, title, colors, cost, cccost, cardClass, types, minRange,
                    maxRange, attack, support, skill, supportSkill, rarity, seriesNo, altArts);

                feCards.Add(card);
            }

            if (httpClient != null)
            {
                while (downloadList.Exists(task => task.Status == TaskStatus.Running))
                {
                    continue;
                }
            }

            // Create Singleton Instance
            FECardList.SetCardlist(feCards);
        }

        //Public Accessors
        public string Name { get => "FECipher"; }

        public string LongName { get => "Fire Emblem Cipher"; }

        public byte[] IconImage { get => Properties.Resources.Icon; }

        public IFormat[] Formats { get => this.formatList; }

        public SearchField[] SearchFields { get => this.searchFieldList; }

        public IImportMenuItem[] ImportFunctions { get; private set; }

        public IExportMenuItem[] ExportFunctions { get; private set; }

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

        public bool CardListInitialized { get; private set; }

        public string DownloadLink { get => "https://github.com/Eronan/Multi-TCG-Deck-Builder-FECipher-Plug-In/releases"; }

        // Public Functions
        public void InitializePlugIn()
        {
            try
            {
                LoadCardList();
            }
            catch (Exception e) when (e is FileNotFoundException || e is DirectoryNotFoundException)
            {
                Console.WriteLine(e.Message);
                throw new PlugInUpdateRequiredException("Update required to download Files.", e);
            }
        }

        public List<DeckBuilderCard> AdvancedFilterSearchList(IEnumerable<DeckBuilderCard> cards, SearchField[] searchFields)
        {
            List<DeckBuilderCard> returnList = new List<DeckBuilderCard>();
            foreach (DeckBuilderCard card in cards)
            {
                FECard? feCard = FECardList.Instance.GetCard(card.CardID);
                if (feCard != null)
                {
                    if (this.MatchFields(feCard, searchFields)) returnList.Add(card);
                }
            }
            return returnList;
        }

        public int CompareCards(DeckBuilderCard x, DeckBuilderCard y)
        {
            if (x.CardID == y.CardID)
            {
                return string.Compare(x.ArtID, y.ArtID);
            }

            // Get FECards
            FECard? feCardX = FECardList.Instance.GetCard(x.CardID);
            FECard? feCardY = FECardList.Instance.GetCard(y.CardID);

            if (feCardX == null || feCardY == null)
            {
                return 0;
            }

            if (feCardX.characterName != feCardY.characterName)
            {
                // First Comparison: Character Name
                return string.Compare(feCardX.characterName, feCardY.characterName);
            }
            else if (feCardX.cost != feCardY.cost)
            {
                // Third Comparison: Deployment Cost
                if (feCardX.cost == "X") { return 1; }
                if (feCardY.cost == "X") { return -1; }

                return string.Compare(feCardX.cost, feCardY.cost);
            }
            else if (feCardX.classChangeCost != feCardY.classChangeCost)
            {
                // Third Comparison: Class Change Cost
                return string.Compare(feCardX.classChangeCost, feCardY.classChangeCost);
            }
            else if (feCardX.attack != feCardY.attack)
            {
                // Fourth Comparison: Attack
                return string.Compare(feCardX.attack, feCardY.attack);
            }
            else if (feCardX.support != feCardY.support)
            {
                // Fourth Comparison: Attack
                return string.Compare(feCardX.support, feCardY.support);
            }
            else if (feCardX.characterTitle != feCardY.characterTitle)
            {
                // Fifth Comparison: Character Title
                return string.Compare(feCardX.characterTitle, feCardY.characterTitle);
            }
            else
            {
                // Sixth Comparison: Card ID
                return string.Compare(feCardX.ID, feCardY.ID);
            }
        }

        public async Task<bool> DownloadFiles()
        {
            // https://raw.githubusercontent.com/Eronan/Multi-TCG-Deck-Builder-FECipher-Plug-In/master/FECipher/cardlist.json

            // Download Card List File
            if (!Directory.Exists("./plug-ins/fe-cipher"))
            {
                Directory.CreateDirectory("./plug-ins/fe-cipher");
            }

            var client = new HttpClient();
            var jsonFile = await client.GetStringAsync(@"https://raw.githubusercontent.com/Eronan/Multi-TCG-Deck-Builder-FECipher-Plug-In/master/FECipher/cardlist.json");
            File.WriteAllText("./plug-ins/fe-cipher/cardlist.json", jsonFile);

            LoadCardList(client);

            return true;
        }
    }
}
