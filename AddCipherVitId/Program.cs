// See https://aka.ms/new-console-template for more information
using FECipher;
using Microsoft.VisualBasic.FileIO;
using System.Text.Json;

const string InputFile = @"D:\..\GitUpdate Repos\Multi-TCG Deck Builder Projects\FECipher\FECipher\cardlist.json";
const string CipherVitFileLocation = @"D:\..\Desktop Folders\Nothing Important\FECipherVit 5.9.0_en\res\cards\en";

var CipherVitIds = new Dictionary<string, string>(); 

var cipherVitFiles = Directory.GetFiles(CipherVitFileLocation, "*.fe0db");
foreach (var vitFile in cipherVitFiles)
{
    /*
    using (TextFieldParser parser = new TextFieldParser(vitFile))
    {
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(",");
        while (!parser.EndOfData)
        {
            var fields = parser.ReadFields();
            if (fields != null)
            {
                CipherVitIds.Add(fields[2], int.Parse(fields[0]));
            }
        }
    }
    */
    var fileLines = File.ReadAllLines(vitFile);
    foreach (var line in fileLines)
    {
        var fields = line.Split(',');
        string[] rarities;
        if ((rarities = fields[19].Split('/')).Count() == 2)
        {
            CipherVitIds.Add(fields[2].Replace("X", "").Replace("ZZ", "") + rarities[0], fields[0]+'+');
            CipherVitIds.Add(fields[2].Replace("X", "").Replace("ZZ", "") + rarities[1], fields[0]);
        }
        else
        {
            CipherVitIds.Add(fields[2].Replace("X", "").Replace("ZZ", "") + fields[19], fields[0]);
        }
    }
}

string jsonText = File.ReadAllText(InputFile);
JsonElement jsonDeserialize = JsonSerializer.Deserialize<dynamic>(jsonText);
var jsonEnumerator = jsonDeserialize.EnumerateArray();

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
        string? cipherVitID = code != null ? CipherVitIds.GetValueOrDefault(code) : null;
        string? imageURL = altArt.GetProperty("DownloadURL").GetString();

        //Cannot be Null
        if (code == null || setNo == null || image == null || lackeyID == null || lackeyName == null || cipherVitID == null || imageURL == null)
        {
            throw new ArgumentException("JSON Field AlternateArts is missing a Non-Nullable Property.");
        }

        FEAlternateArts alt = new FEAlternateArts(code, setNo, image, lackeyID, lackeyName, cipherVitID, imageURL.Trim());
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
File.WriteAllText(InputFile, newJsonText);