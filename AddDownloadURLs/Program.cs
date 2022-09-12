// See https://aka.ms/new-console-template for more information
using FECipher;
using System.Text.Json;

const string jsonCardList = @"D:\Ernest's Folder\GitUpdate Repos\Multi-TCG Deck Builder Projects\Multi-TCG-Deckbuilder\Multi-TCG-Deckbuilder\bin\Debug\net6.0-windows\plug-ins\fe-cipher\cardlist.json";
const string imageDownloadURLs = @"D:\Ernest's Folder\Desktop Folders\Nothing Important\LackeyCCG\plugins\FECipher0\CardImageURLs3.txt";

Dictionary<string, string> downloadURLs = new Dictionary<string, string>();
foreach (string line in File.ReadAllLines(imageDownloadURLs))
{
    string[] items = line.Split('\t');
    string id = Path.GetFileNameWithoutExtension(items[0]);
    downloadURLs.Add(id, items[1].Trim());
}

string jsonText = File.ReadAllText(jsonCardList);
JsonElement jsonDeserialize = JsonSerializer.Deserialize<dynamic>(jsonText);
var jsonEnumerator = jsonDeserialize.EnumerateArray();

// Get Card Data
List<FECard> feCards = new List<FECard>();
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
        string? imageURL = lackeyID != null ? downloadURLs.GetValueOrDefault(Path.GetFileNameWithoutExtension(lackeyID)) : null;

        //Cannot be Null
        if (code == null || setNo == null || image == null || lackeyID == null || lackeyName == null || imageURL == null)
        {
            throw new ArgumentException("JSON Field AlternateArts is missing a Non-Nullable Property.");
        }

        FEAlternateArts alt = new FEAlternateArts(code, setNo, image, lackeyID, lackeyName, imageURL.Trim());
        altArts.Add(alt);
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


FECard[] cardListNoKey = feCards.ToArray();

JsonSerializerOptions options = new JsonSerializerOptions();
options.WriteIndented = true;
string newJsonText = JsonSerializer.Serialize<FECard[]>(cardListNoKey, options);
File.WriteAllText(jsonCardList, newJsonText);