using System.Text.Json;

namespace FECipher
{
    /// <summary>
    /// A Singleton Class Instance used by the Plug-In to Read Card Data.
    /// </summary>
    internal class FECardList
    {
        private static FECardList? instance;
        internal IList<FECard> allCards;
        internal IList<FECard> filteredList;

        private FECardList()
        {
            if (instance == null)
            {
                allCards = LoadCardList();
                filteredList = allCards;
                instance = this;
            }
            else
            {
                allCards = instance.allCards;
                filteredList = instance.filteredList;
            }
        }

        private FECardList(IList<FECard> allCards)
        {
            this.allCards = allCards;
            this.filteredList = allCards;
        }

        private static IList<FECard> LoadCardList()
        {
            string jsonText = File.ReadAllText("./plug-ins/fe-cipher/cardlist.json");
            JsonElement jsonDeserialize = JsonSerializer.Deserialize<dynamic>(jsonText);
            var jsonEnumerator = jsonDeserialize.EnumerateArray();

            // Get Card Data
            List<FECard> feCards = new List<FECard>();
            List<Task> downloadList = new List<Task>();

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

                    // Make File Path a Relative Path
                    image = "." + image;
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

            // Create Singleton Instance
            return feCards.OrderBy(card => card.ID).ToList();
        }

        public static IList<FECard> ReloadCardList()
        {
            var cardlist = LoadCardList();
            if (instance == null)
            {
                instance = new FECardList(cardlist);
            }

            instance.allCards = cardlist;
            CardListReloaded = true;

            return instance.allCards;
        }

        public static FECardList Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FECardList();
                }

                return instance;
            }
        }

        public static bool Initialized { get => instance != null; }

        public static bool CardListReloaded { get; set; } = false;

        public static void SetFilter(Func<FECard, bool> filter)
        {
            Instance.filteredList = Instance.allCards.Where(filter).ToList();
        }

        public static FECard? GetCard(string cardId)
        {
            // Perform Binary Search
            int minNum = 0;
            int maxNum = Instance.filteredList.Count - 1;

            while (minNum <= maxNum)
            {
                int mid = (minNum + maxNum) / 2;
                if (cardId == Instance.filteredList[mid].ID)
                {
                    return Instance.filteredList[mid];
                }
                else if (cardId.CompareTo(Instance.filteredList[mid].ID) < 0)
                {
                    maxNum = mid - 1;
                }
                else
                {
                    minNum = mid + 1;
                }
            }

            return null;
        }
    }
}
