using Framework.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;



public class GameData
{

    public Dictionary<ConsoleKey, Input.VirtualKey> VirtualKeyMapping { get; set; }
    public int BGMVolume { get; set; } = 5;
    public int SEVolume { get; set; } = 5;

    public int SprintScore { get; set; }
    public int BlitzScore { get; set; }
    public int InfinityScore { get; set; }

    public static GameData GetDefaultConfig()
    {
        return new GameData()
        {
            VirtualKeyMapping = new()
            {
                { ConsoleKey.UpArrow, Input.VirtualKey.Up },
                { ConsoleKey.DownArrow, Input.VirtualKey.Down },
                { ConsoleKey.S, Input.VirtualKey.SoftDrop },
                { ConsoleKey.Spacebar, Input.VirtualKey.HardDrop },
                { ConsoleKey.C, Input.VirtualKey.Keep },
                { ConsoleKey.LeftArrow, Input.VirtualKey.Left },
                { ConsoleKey.RightArrow, Input.VirtualKey.Right },
                { ConsoleKey.Enter, Input.VirtualKey.Select },
                { ConsoleKey.Z, Input.VirtualKey.SpinLeft },
                { ConsoleKey.X, Input.VirtualKey.SpinRight },
                { ConsoleKey.Escape,Input.VirtualKey.SPECIAL_ESC },
            }
        };
    }


}

public static class DataManager
{
    readonly static string directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    const string folder = "\\ConsoleTetris";
    const string file = "\\GameData.json";

    static GameData _currentGameData;
    public static GameData CurrentGameData
    {
        get
        {
            if (_currentGameData == null)
            {
                LoadData();
            }
            return _currentGameData;
        }
    }

    public static void LoadData()
    {
        string fileDirectory = directory + folder + file;

        if (File.Exists(fileDirectory))
        {
            try
            {
                string json = File.ReadAllText(fileDirectory);
                _currentGameData = JsonSerializer.Deserialize<GameData>(json);
            }
            catch (Exception)
            {
                _currentGameData = GameData.GetDefaultConfig();
            }
        }
        else
        {
            _currentGameData = GameData.GetDefaultConfig();
        }
    }

    public static void SaveConfig() 
    {
        UpdateKeyConfig();

        string json = JsonSerializer.Serialize(_currentGameData);
        string folderDirectory = directory + folder;

        if (!Directory.Exists(folderDirectory))
        {
            Directory.CreateDirectory(folderDirectory);
        }

        string fileDirectory = folderDirectory + file;

        using StreamWriter sw = new StreamWriter(fileDirectory);
        sw.WriteLine(json);
        Debug.WriteLine(fileDirectory);
    }

    public static void SetKey(Input.VirtualKey virtualKey, ConsoleKey actualKey)
    {
        Input.SetKey(actualKey, virtualKey);
    }

    public static bool IsKeyConflict(ConsoleKey key)
    {
        return Input.IsConflict(key);
    }

    public static void ApplyConfig()
    {
        ClearKey();
        foreach(var key in _currentGameData.VirtualKeyMapping)
        {
            SetKey(key.Value, key.Key);
        }
    }

    static void ClearKey()
    {
        Input.ClearKey();
    }

    public static void UpdateKeyConfig()
    {
        _currentGameData.VirtualKeyMapping.Clear();
        foreach (var key in Input.GetKeys())
        {
            _currentGameData.VirtualKeyMapping.Add(key.actualKey,key.virtualKey);
        }
    }
}
