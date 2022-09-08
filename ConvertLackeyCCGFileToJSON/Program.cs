using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using FECipher;

// See https://aka.ms/new-console-template for more information
const string lackeyCCGFile = @"D:\Ernest's Folder\Desktop Folders\Nothing Important\LackeyCCG\plugins\FECipher0\sets\official-cards.txt";
//const string outputFile = @"D:\Ernest's Folder\GitUpdate Repos\Multi-TCG-Deckbuilder\Multi-TCG-Deckbuilder\bin\Debug\net6.0-windows\plug-ins\fe-cipher\cardlist_New.json";

string[] setFileLines = File.ReadAllLines(lackeyCCGFile);

Dictionary<string, FECard> cardList = new Dictionary<string, FECard>();
//bool skipFirstLine = true;
/*
foreach (string line in setFileLines)
{
    if (skipFirstLine)
    {
        skipFirstLine = false;
        continue;
    }

    string[] items = line.Split('\t');
    string name = items[0].Split(',')[0];
    string title = items[0].Split(',')[1].Trim();

    title = title.Replace("+X", "");
    title = title.Replace("+", "");
    title = Regex.Replace(title, " \\([A-Z0-9]+\\)", "");

    string id = name.Replace(" ", "").Replace("(", "").Replace(")", "") + "." + title.Replace(" ", "");
    string[] colors = items[3].Split('/');
    string[] types = items[7].Split('/');
    string cost = items[5].Substring(0, 1);
    string? cccost = null;
    if (items[5].Length > 1)
    {
        cccost = items[5].Substring(2, 1);
    }

    int minRange = 0;
    int maxRange = 0;

    if (items[8].Trim() != "-")
    {
        string[] ranges = items[8].Split('-');
        minRange = int.Parse(ranges[0]);
        maxRange = int.Parse(ranges.Length > 1 ? ranges[1] : ranges[0]);
    }

    string skill = items[11];
    if (items[12].Trim() != "-")
    {
        skill += "\n" + items[12];
    }

    if (items[13].Trim() != "-")
    {
        skill += "\n" + items[13];
    }

    string supportSkill = items[14].Trim();

    FECard? card = cardList.GetValueOrDefault(id);

    if (card != null)
    {
        if (card.colors.SequenceEqual(colors) && card.types.SequenceEqual(types) && card.cost == cost.Trim() &&
            card.classChangeCost == cccost && card.cardClass.ToUpper() == items[6].ToUpper().Trim())
        {
            string artID = items[2].Split('_')[0].Replace("plus", "+");
            FEAlternateArts feArt = new FEAlternateArts(artID, items[1], @"\plug-ins\fe-cipher\images\" + items[1] + @"\" + items[2] + ".png", items[2], items[0], "");
            card.altArts.Add(feArt);
        }
        else
        {
            card = new FECard(id + ".2", name.Trim(), title.Trim(), colors, items[4].Trim(), cost.Trim(), cccost,
                items[6].Trim(), types, minRange, maxRange, items[9].Trim(), items[10].Trim(),
                skill.Trim(), supportSkill == "-" ? null : supportSkill.Trim(), int.Parse(items[1].Substring(1, 2)), new List<FEAlternateArts>());

            string artID = items[2].Split('_')[0].Replace("plus", "+");

            FEAlternateArts feArt = new FEAlternateArts(artID, items[1], @"\plug-ins\fe-cipher\images\" + items[1] + @"\" + items[2] + ".png", items[2], items[0], "");
            card.altArts.Add(feArt);

            cardList.Add(card.ID, card);
        }
    }
    else
    {
        card = new FECard(id, name.Trim(), title.Trim(), colors, items[4].Trim(), cost, cccost,
        items[6].Trim(), types, minRange, maxRange, items[9].Trim(), items[10].Trim(),
        skill.Trim(), supportSkill == "-" ? null : supportSkill.Trim(), int.Parse(items[1].Substring(1, 2)));

        string artID = items[2].Split('_')[0].Replace("plus", "+");

        FEAlternateArts feArt = new FEAlternateArts(artID, items[1], @"\plug-ins\fe-cipher\images\" + items[1] + @"\" + items[2] + ".png", items[2], items[0], "");
        card.altArts.Add(feArt);

        cardList.Add(card.ID, card);
    }
}

FECard[] cardListNoKey = cardList.Values.ToArray();

JsonSerializerOptions options = new JsonSerializerOptions();
options.WriteIndented = true;
string jsonText = JsonSerializer.Serialize<FECard[]>(cardListNoKey, options);
File.WriteAllText(outputFile, jsonText);
*/