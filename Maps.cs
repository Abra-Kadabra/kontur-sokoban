using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;

namespace sokoban;

public class Maps
{
    public static string active = "map0";
    public static IImmutableList<string> Atlas { get { return catalog.Keys.ToImmutableList(); } }
    public static Dictionary<string, Records> RecordsMap = new();
    
    private static readonly Dictionary<string, string[]> catalog = new();
    private static readonly HashSet<char> validChars = new HashSet<char> { '.', '#', 'X', '@', 'O', '~' };
    private static readonly string fileRecords = "records.csv";

    static Maps()
    {
        RegisterMap("map0", map0);
        RegisterMap("map1", map1);

        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var filesInDirectory = Directory.GetFiles(currentDirectory);

        foreach (var file in filesInDirectory)
        { 
            if (Path.GetExtension(file) == ".lvl")
            {
                var name = Path.GetFileNameWithoutExtension(file);
                var content = File.ReadAllLines(file);
                if (Validation(content))
                {
                    RegisterMap(name, content);
                    Sokoban.Logger.Info($"Map {name} loaded from file.");
                }
                else
                    Sokoban.Logger.Error($"Map {name} doesn't loaded due to error(s).");
            }
        }
        LoadRecords();
    }

    private static void RegisterMap(string name, string[] map)
    {
        catalog[name] = map;
        RecordsMap[name] = new Records(name);
    }

    public static string[] Get()
    {
        return catalog[active];
    }

    public static Records GetRecords()
    {
        return RecordsMap[active];
    }

    public static void SaveRecords()
    {
        var content = new StringBuilder();
        foreach (var map in RecordsMap.Values)
            content.AppendLine($"{map.Name}, {map.Steps}, {map.Time}");
        File.WriteAllText(fileRecords, content.ToString());
    }

    private static void LoadRecords()
    {
        if (!File.Exists(fileRecords))
            SaveRecords();

        var content = File.ReadAllLines(fileRecords);
        foreach(var line in content)
        {
            var values = line.Split(',');
            var name = values[0];
            var records = new Records(name, int.Parse(values[1]), TimeSpan.Parse(values[2]));
            RecordsMap[name] = records;
        }
    }

    private static bool Validation(string[] isCorrectMap)
    {
        var height = isCorrectMap.Length;
        var width = isCorrectMap[0].Length;
        var playerSet = false;
        var targets = 0;
        var crates = 0;
        
        for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var ch = isCorrectMap[y][x];
                if (!validChars.Contains(ch))
                    return false;

                if (ch == 'X')
                    targets++;
                else if (ch == 'O')
                    crates++;
                else if (ch == '@')
                { 
                    if (playerSet)
                        return false;
                    playerSet = true;
                }
            }

        return playerSet && targets > 0 && crates >= targets;
    }

    private static string[] map0 = new string[]
    {
        "#######",
        "#.....#",
        "#.....#",
        "#.O@X.#",
        "#.....#",
        "#.....#",
        "#######"
    };
    private static string[] map1 = new string[] // #1 https://github.com/begoon/sokoban-maps
    {
        "~~~~#####~~~~~~~~~~~~~",
        "~~~~#...#~~~~~~~~~~~~~",
        "~~~~#O..#~~~~~~~~~~~~~",
        "~~###..O###~~~~~~~~~~~",
        "~~#..O..O.#~~~~~~~~~~~",
        "###.#.###.#~~~~~######",
        "#...#.###.#######..XX#",
        "#.O..O.............XX#",
        "#####.####.#@####..XX#",
        "~~~~#......###~~######",
        "~~~~#######~~~~~~~~~~~"
    };
}
