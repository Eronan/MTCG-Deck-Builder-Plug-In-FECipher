using System.Text.Json.Serialization;

namespace FECipher
{
    public class FECard
    {
        public FECard(string iD, string characterName, string characterTitle, string[] colors, string cost, string? classChangeCost, string cardClass, string[] types, int minRange, int maxRange, string attack, string support, string skill, string? supportSkill, string rarity, int seriesNo, List<FEAlternateArts> altArts)
        {
            ID = iD;
            this.characterName = characterName;
            this.characterTitle = characterTitle;
            this.colors = colors;
            this.cost = cost;
            this.classChangeCost = classChangeCost;
            this.cardClass = cardClass;
            this.types = types;
            this.minRange = minRange;
            this.maxRange = maxRange;
            this.attack = attack;
            this.support = support;
            this.skill = skill;
            this.supportSkill = supportSkill;
            this.rarity = rarity;
            this.seriesNo = seriesNo;
            this.altArts = altArts;
        }

        [JsonPropertyName("CardID")]
        [JsonPropertyOrder(0)]
        public string ID { get; set; }

        [JsonPropertyName("Character")]
        [JsonPropertyOrder(1)]
        public string characterName { get; set; }

        [JsonPropertyName("Title")]
        [JsonPropertyOrder(2)]
        public string characterTitle { get; set; }

        [JsonPropertyName("Color")]
        [JsonPropertyOrder(3)]
        public string[] colors { get; set; }

        [JsonPropertyName("Cost")]
        [JsonPropertyOrder(4)]
        public string cost { get; set; }

        [JsonPropertyName("ClassChangeCost")]
        [JsonPropertyOrder(5)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? classChangeCost { get; set; }

        [JsonPropertyName("Class")]
        [JsonPropertyOrder(6)]
        public string cardClass { get; set; }

        [JsonPropertyName("Type")]
        [JsonPropertyOrder(7)]
        public string[] types { get; set; }

        [JsonPropertyName("MinRange")]
        [JsonPropertyOrder(8)]
        public int minRange { get; set; }

        [JsonPropertyName("MaxRange")]
        [JsonPropertyOrder(9)]
        public int maxRange { get; set; }

        [JsonPropertyName("Attack")]
        [JsonPropertyOrder(10)]
        public string attack { get; set; }

        [JsonPropertyName("Support")]
        [JsonPropertyOrder(11)]
        public string support { get; set; }

        [JsonPropertyName("Skill")]
        [JsonPropertyOrder(12)]
        public string skill { get; set; }

        [JsonPropertyName("SupportSkill")]
        [JsonPropertyOrder(13)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? supportSkill { get; set; }

        [JsonPropertyName("Rarity")]
        [JsonPropertyOrder(14)]
        public string rarity { get; set; }

        [JsonPropertyName("SeriesNumber")]
        [JsonPropertyOrder(15)]
        public int seriesNo { get; set; }

        [JsonPropertyName("AlternateArts")]
        [JsonPropertyOrder(16)]
        public List<FEAlternateArts> altArts { get; set; }

        [JsonIgnore]
        public string Name
        {
            get { return this.characterName + ": " + this.characterTitle; }
        }

        [JsonIgnore]
        public string ViewDetails
        {
            get
            {
                string fullDetails = this.Name;
                fullDetails += string.Format("\nClass: {0}/Cost: {1}", this.cardClass, this.cost);
                if (classChangeCost != null)
                {
                    fullDetails += "(" + this.classChangeCost + ")";
                }

                fullDetails += string.Format("\nColors: {0}\nTypes: {1}\nAttack: {2}/Support: {3}/Range: {4}-{5}", string.Join('/', this.colors), string.Join('/', this.types), this.attack, this.support, this.minRange, this.maxRange);
                fullDetails += "\n---\nSkills:\n" + skill;

                if (supportSkill != null)
                {
                    fullDetails += "\n---\nSupport:\n" + supportSkill;
                }
                return fullDetails;
            }
        }
    }
}
