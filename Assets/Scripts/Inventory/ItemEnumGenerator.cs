using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

public static class ItemEnumGenerator
{
    private const string EnumPath = "Assets/Generated/ItemIDEnum.cs";

    public static void WriteEnum(IReadOnlyList<ItemData> items)
    {
        // ── build the new file text in memory ───────────────────────────────
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("// AUTO-GENERATED – do not edit by hand");
        sb.AppendLine("public enum ItemType");
        sb.AppendLine("{");
        sb.AppendLine("None,");

        foreach (ItemData item in items)
        {
            string safe = MakeSafeEnumName(item.name);
            sb.AppendLine($"    {safe},");
        }
        sb.AppendLine("}");

        string newText = sb.ToString();

        System.IO.Directory.CreateDirectory("Assets/Generated");

        if (!System.IO.File.Exists(EnumPath) ||
            System.IO.File.ReadAllText(EnumPath) != newText)
        {
            System.IO.File.WriteAllText(EnumPath, newText);
            Debug.Log("ItemIDEnum.cs re-generated.");
        }
        else
        {
            Debug.Log("ItemIDEnum.cs already up-to-date; no rewrite.");
        }

        AssetDatabase.Refresh();
    }

    private static string MakeSafeEnumName(string raw)
    {
        string cleaned = System.Text.RegularExpressions.Regex
            .Replace(raw, @"[^a-zA-Z0-9_]", "");   // remove spaces/ symbols
        return char.IsDigit(cleaned[0]) ? "_" + cleaned : cleaned;
    }
}
#endif
