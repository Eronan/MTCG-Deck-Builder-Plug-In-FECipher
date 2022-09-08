using IGamePlugInBase;
using IGamePlugInBase.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FECipher
{
    internal class FEDeckBuildingMethods : IDeckBuilderService
    {
        Dictionary<string, int> SpecialCardMaximums;
        Func<FECard, bool> cardlistFilter;
        List<DeckBuilderCardArt> cardArtsList = new List<DeckBuilderCardArt>();

        /// <summary>
        /// Fields used by the Deck Builder to build the Advanced Search
        /// </summary>
        public IEnumerable<SearchField> SearchFields { get; }

        public IEnumerable<DeckBuilderCardArt> CardList { get => cardArtsList; }

        public FEDeckBuildingMethods(Func<FECard, bool> filter)
        {
            this.cardlistFilter = filter;

            // Create Search Fields
            string[] colours = new string[10] { "All", "Red", "Blue", "Yellow", "Purple", "Green", "Black", "White", "Brown", "Colorless" };
            this.SearchFields = new SearchField[12]
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

            SpecialCardMaximums = new Dictionary<string, int>();
            SpecialCardMaximums.Add("Anna.SecretSeller", -1);
            SpecialCardMaximums.Add("Anna.PeddlerofManyMysteries", -1);
            SpecialCardMaximums.Add("Necrodragon.ResurrectedWyrm", -1);
            SpecialCardMaximums.Add("Risen.GrotesqueSoldierA", -1);
            SpecialCardMaximums.Add("Risen.GrotesqueSoldierB", -1);
            SpecialCardMaximums.Add("Risen.GrotesqueSoldierC", -1);
            SpecialCardMaximums.Add("Legion.MaskedAssassin", -1);
            SpecialCardMaximums.Add("Faceless.HeartlessMonster", -1);
            SpecialCardMaximums.Add("Witch.SacrificeFatedforPuppetdom", -1);
            SpecialCardMaximums.Add("Risen.DefiledSoldierA", -1);
            SpecialCardMaximums.Add("Risen.DefiledSoldierB", -1);
            SpecialCardMaximums.Add("Risen.DefiledSoldierC", -1);
            SpecialCardMaximums.Add("Bael.GiantMan-EatingSpider", -1);
            SpecialCardMaximums.Add("Gorgon.Snake-HairedDemon", -1);
        }

        // Creates a Filter Function
        private Func<DeckBuilderCard, bool> GetMatchFunction(DeckBuilderCard matchCard, Func<FECard, FECard, bool> matchFunction)
        {
            FECard? feMatchCard = FECardList.GetCard(matchCard.CardID);

            bool IsCard(DeckBuilderCard item)
            {
                FECard? itemCardCheck = FECardList.GetCard(item.CardID);
                return feMatchCard != null && itemCardCheck != null &&
                    matchFunction(feMatchCard, itemCardCheck);
            }

            return IsCard;
        }

        // Determines whether a card satisfies the Search Fields
        private bool MatchFields(FECard card, IEnumerable<SearchField> searchFields)
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

        /// <summary>
        /// Function called before opening a Format in the Deck Builder Window.
        /// Any time-consuming actions should be run here.
        /// </summary>
        public void InitializeService()
        {
            try
            {
                FECardList.SetFilter(this.cardlistFilter);

                if (cardArtsList.Count == 0 || FECardList.CardListReloaded)
                {
                    foreach (var card in FECardList.Instance.filteredList)
                    {
                        foreach (var altArt in card.altArts)
                        {
                            cardArtsList.Add(new DeckBuilderCardArt(card.ID, altArt.Id, card.Name, altArt.ImageLocation, altArt.ImageDownloadURL, CardArtOrientation.Portrait, card.ViewDetails));
                        }
                    }

                    FECardList.CardListReloaded = false;
                }
            }
            catch (Exception e) when (e is FileNotFoundException || e is DirectoryNotFoundException || e.InnerException is FileNotFoundException || e.InnerException is DirectoryNotFoundException)
            {
                Console.WriteLine(e.Message);
                throw new PlugInFilesMissingException("Please download the necessary files for the Plug-In.", e);
            }
        }

        /// <summary>
        /// Determines whether a card has reached the Maximum Allowable number of copies in a Decklist.
        /// </summary>
        /// <param name="card">Card that is being added.</param>
        /// <param name="decks">All Cards in each Deck.</param>
        /// <returns>The card has reached the number of copies in the Deck.</returns>
        public bool ValidateMaximum(DeckBuilderCard card, Dictionary<string, IEnumerable<DeckBuilderCard>> decks)
        {
            int count = 0;

            // Max Copies of cards that don't have Special Maximums is 4
            int maxCopies = SpecialCardMaximums.GetValueOrDefault(card.CardID, 4);
            if (maxCopies == -1)
            {
                return false;
            }

            foreach (KeyValuePair<string, IEnumerable<DeckBuilderCard>> decklist in decks)
            {
                count += decklist.Value.Count(GetMatchFunction(card, (card1, card2) => card1.Name == card2.Name));
                if (count >= maxCopies) { return true; }
            }
            return false;
        }

        /// <summary>
        /// Returns Text detailing the types of cards in the Decks.
        /// </summary>
        /// <param name="decks">All the Cards in the Deck List separated by which Deck they are in.</param>
        /// <returns>The Text that is displayed with Labels and Counts. It should be smaller, and fit on the Deck Builder Window's Button.</returns>
        public string GetStats(Dictionary<string, IEnumerable<DeckBuilderCard>> decks)
        {
            // Get Main Character Card
            IEnumerable<DeckBuilderCard>? mainCharDeck = decks.GetValueOrDefault(FEMainCharacter.MCDeckName);
            DeckBuilderCard? mainCharacter = mainCharDeck != null && mainCharDeck.Count() == 1 ? mainCharDeck.FirstOrDefault() : null;

            // Get Counts
            int deckSize = 0;
            int mainCharacterNames = 0;

            // Count Cards
            foreach (KeyValuePair<string, IEnumerable<DeckBuilderCard>> keyValue in decks)
            {
                // Perform Counts
                deckSize += keyValue.Value.Count();
                mainCharacterNames += mainCharacter != null
                    ? keyValue.Value.Count(GetMatchFunction(mainCharacter, (card1, card2) => card1.characterName == card2.characterName))
                    : 0;
            }

            string textFormat = "Deck Size: {0}\tMain Characters: {1}";

            return string.Format(textFormat, deckSize, mainCharacterNames);
        }

        /// <summary>
        /// Returns Text detailing the types of cards in the Decks.
        /// </summary>
        /// <param name="decks">All the Cards in the Deck List separated by which Deck they are in.</param>
        /// <returns>The Text that is displayed with Labels and Counts. The text will show on a separate Window.</returns>
        public string GetDetailedStats(Dictionary<string, IEnumerable<DeckBuilderCard>> decks)
        {
            // Get Main Character Card
            IEnumerable<DeckBuilderCard>? mainCharDeck = decks.GetValueOrDefault(FEMainCharacter.MCDeckName);
            FECard? mainCharacter = mainCharDeck != null && mainCharDeck.Count() == 1 ? FECardList.GetCard(mainCharDeck.First().CardID) : null;

            // Get Counts
            int deckSize = 0;
            int mainCharacterNames = 0;
            int range0 = 0;
            int range1 = 0;
            int range2 = 0;
            int range3 = 0;
            int support0 = 0;
            int support10 = 0;
            int support20 = 0;
            int support30 = 0;

            // Count Cards
            foreach (KeyValuePair<string, IEnumerable<DeckBuilderCard>> keyValue in decks)
            {
                // Count Deck Size
                deckSize += keyValue.Value.Count();

                foreach (DeckBuilderCard card in keyValue.Value)
                {
                    FECard? feCard = FECardList.GetCard(card.CardID);

                    if (feCard == null)
                    {
                        continue;
                    }

                    // Main Character Counting
                    if (mainCharacter != null && feCard.characterName == mainCharacter.characterName)
                    {
                        mainCharacterNames++;
                    }

                    // Range Counting
                    if (feCard.minRange == 0 && feCard.maxRange == 0)
                    {
                        range0++;
                    }
                    if (feCard.minRange <= 1 && feCard.maxRange >= 1)
                    {
                        range1++;
                    }
                    if (feCard.minRange <= 2 && feCard.maxRange >= 2)
                    {
                        range2++;
                    }
                    if (feCard.minRange <= 3 && feCard.maxRange >= 3)
                    {
                        range3++;
                    }

                    // Support Counting
                    switch (feCard.support)
                    {
                        case "0":
                            support0++;
                            break;
                        case "10":
                            support10++;
                            break;
                        case "20":
                            support20++;
                            break;
                        case "30":
                            support30++;
                            break;
                    }
                }
            }

            string textFormat = "Deck Size: {0}\tMain Characters: {1} ({2}%)\n" +
                "0 Support: {3} ({4}%)\tNo Range: {11} ({12}%)\n" +
                "10 Support: {5} ({6}%)\tRange 1: {13} ({14}%)\n" +
                "20 Support: {7} ({8}%)\tRange 2: {15} ({16}%)\n" +
                "30 Support: {9} ({10}%)\tRange 3: {17} ({17}%)\n";

            return string.Format(textFormat, deckSize,
                mainCharacterNames, mainCharacterNames / deckSize * 100,
                support0, support0 / deckSize * 100,
                support10, support10 / deckSize * 100,
                support20, support20 / deckSize * 100,
                support30, support30 / deckSize * 100,
                range0, range0 / deckSize * 100,
                range1, range1 / deckSize * 100,
                range2, range2 / deckSize * 100,
                range3, range3 / deckSize * 100);
        }

        /// <summary>
        /// A Function that removes cards from the List based on searchFields.
        /// </summary>
        /// <param name="cards">The List of Cards to be Filtered.</param>
        /// <param name="searchFields">All the Search Fields and their Values.</param>
        /// <returns>List of Cards from cards that matching searchFields.</returns>
        public IEnumerable<DeckBuilderCardArt> AdvancedFilterSearchList(IEnumerable<DeckBuilderCardArt> cards, IEnumerable<SearchField> searchFields)
        {
            List<DeckBuilderCardArt> returnList = new List<DeckBuilderCardArt>();
            foreach (var card in cards)
            {
                FECard? feCard = FECardList.GetCard(card.CardID);

                if (feCard != null)
                {
                    if (this.MatchFields(feCard, searchFields)) returnList.Add(card);
                }
            }
            return returnList;
        }

        /// <summary>
        /// Used to Sort Card Lists
        /// </summary>
        /// <param name="x">First Card</param>
        /// <param name="y">Second Card</param>
        /// <returns>If x precedes y, it returns a number less than 0, if x and y are the same it returns 0, and if y precedes x it returns a number greater than 0.</returns>
        public int CompareCards(DeckBuilderCard x, DeckBuilderCard y)
        {
            if (x.CardID == y.CardID)
            {
                return string.Compare(x.ArtID, y.ArtID);
            }

            // Get FECards
            FECard? feCardX = FECardList.GetCard(x.CardID);
            FECard? feCardY = FECardList.GetCard(y.CardID);

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


        /// <summary>
        /// Gets the Deck that the Card should be added to by Default.
        /// </summary>
        /// <param name="card">Card which is being added.</param>
        /// <returns>The Index of the Deck that the card will be added to by Default.</returns>
        public string DefaultDeckName(DeckBuilderCard card)
        {
            return "main";
        }
    }
}

