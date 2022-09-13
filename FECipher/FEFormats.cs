using IGamePlugInBase;

namespace FECipher
{
    internal class FEFormats : IFormat
    {
        public FEFormats(string name, string longname, byte[] icon, string description, Func<FECard, bool> filter)
        {
            this.Name = name;
            this.LongName = longname;
            this.Icon = icon;
            this.Description = description;

            this.Decks = new IDeck[2]
            {
                new FEMainCharacter(),
                new FEMainDeck(),
            };

            this.DeckBuilderService = new FEDeckBuildingMethods(filter);
        }

        public string Name { get; }

        public string LongName { get; }

        public byte[] Icon { get; }

        public string Description { get; }

        public IEnumerable<IDeck> Decks { get; }

        public IDeckBuilderService DeckBuilderService { get; }
    }
}
