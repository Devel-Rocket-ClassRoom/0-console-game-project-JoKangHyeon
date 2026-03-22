using Framework.Engine;
using System;
using System.Collections.Generic;
using System.Text;

internal class ConfigScene : Scene
{
    string[] _keyConfigs = { };

    int _selectedMenu = 0;
    bool _keyEdit = false;

    Dictionary<Input.VirtualKey, ConsoleKey> defaultKeys = new()
    {
        {Input.VirtualKey.Up,ConsoleKey.UpArrow },
        {Input.VirtualKey.Down,ConsoleKey.DownArrow},
        {Input.VirtualKey.SoftDrop,ConsoleKey.S},
        {Input.VirtualKey.HardDrop,ConsoleKey.Spacebar},
        {Input.VirtualKey.Keep,ConsoleKey.C},
        {Input.VirtualKey.Left,ConsoleKey.LeftArrow},
        {Input.VirtualKey.Right,ConsoleKey.RightArrow},
        {Input.VirtualKey.Select,ConsoleKey.Enter},
        {Input.VirtualKey.SpinLeft,ConsoleKey.Z},
        {Input.VirtualKey.SpinRight ,ConsoleKey.X},
    };

    readonly int[] keyConfigY = { 7, 8, 9, 10, 13, 14, 15, 16, 17, 18, 19 };
    readonly int[] configY = { 6, 7, 9, 11 };

    bool _keyWaiting = false;
    Input.VirtualKey _keyEditingTarget;
    string Debug = "";

    public event GameAction RequestReturnMainScreen;
    public ConfigScene()
    {
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.DrawBox(0, 0, 82, 22);
        buffer.WriteText(4, 2, "설정");

        buffer.WriteText(4, 5, "음량설정");
        buffer.WriteText(4, 6, $"배경음악             {DataManager.CurrentGameData.BGMVolume * 10}%");
        for (int i = 0; i < DataManager.CurrentGameData.BGMVolume; i++)
        {
            buffer.SetCell(14 + i, 6, '\u2588');
        }


        buffer.WriteText(4, 7, $"효과음               {DataManager.CurrentGameData.SEVolume * 10}%");
        for (int i = 0; i < DataManager.CurrentGameData.SEVolume; i++)
        {
            buffer.SetCell(14 + i, 7, '\u2588');
        }


        buffer.WriteText(4, 9, "키설정");
        buffer.WriteText(4, 11, "저장하고 종료");


        buffer.DrawVLine(30, 1, 20);

        /*
            Select,
            Up,
            Down,
            Right,
            Left,
            HardDrop,
            SoftDrop,
            Keep,
            SpinRight,
            SpinLeft,
         */

        buffer.WriteText(34, 2, "키 설정 ");
        buffer.WriteText(34, 3, "* <Esc>를 눌러 초기화");
        buffer.WriteText(34, 4, "* <Enter>를 눌러 키 설정");

        buffer.WriteText(34, 6, "<방향키>");
        buffer.WriteText(34, 7, "위          : ");
        buffer.WriteText(34, 8, "아래        : ");
        buffer.WriteText(34, 9, "오른쪽      : ");
        buffer.WriteText(34, 10, "왼쪽        : ");
        buffer.WriteText(34, 12, "<특수 키>");
        buffer.WriteText(34, 13, "선택        : ");
        buffer.WriteText(34, 14, "하드 드랍   : ");
        buffer.WriteText(34, 15, "소프트 드랍 : ");
        buffer.WriteText(34, 16, "킵          : ");
        buffer.WriteText(34, 17, "우회전      : ");
        buffer.WriteText(34, 18, "좌회전      : ");
        buffer.WriteText(34, 19, "설정 완료");

        buffer.WriteLines(48, 7, _keyConfigs);


        if (_keyEdit)
        {
            buffer.SetCell(32, keyConfigY[_selectedMenu], '>');
        }
        else
        {
            buffer.SetCell(2, configY[_selectedMenu], '>');
        }

        buffer.WriteText(0, 0, Debug);
        DrawGameObjects(buffer);
    }

    public override void Load()
    {
        UpdateKeyConfigText();
    }

    public override void Unload()
    {

    }

    public void UpdateKeyConfigText()
    {
        _keyConfigs = new string[12];
        StringBuilder[] sb = new StringBuilder[12];
        for (int i = 0; i < 12; i++)
        {
            sb[i] = new();
        }

        foreach (var key in Input.GetKeys())
        {
            int line = 0;
            switch (key.virtualKey)
            {
                case Input.VirtualKey.Select:
                    line = 6;
                    break;
                case Input.VirtualKey.Up:
                    line = 0;
                    break;
                case Input.VirtualKey.Down:
                    line = 1;
                    break;
                case Input.VirtualKey.Right:
                    line = 2;
                    break;
                case Input.VirtualKey.Left:
                    line = 3;
                    break;
                case Input.VirtualKey.HardDrop:
                    line = 7;
                    break;
                case Input.VirtualKey.SoftDrop:
                    line = 8;
                    break;
                case Input.VirtualKey.Keep:
                    line = 9;
                    break;
                case Input.VirtualKey.SpinRight:
                    line = 10;
                    break;
                case Input.VirtualKey.SpinLeft:
                    line = 11;
                    break;
                default:
                    continue;
            }
            sb[line].Append(key.actualKey.ToString());
            sb[line].Append(", ");
        }
        foreach (var stringBuilder in sb)
        {
            if (stringBuilder.Length > 1)
            {
                stringBuilder.Remove(stringBuilder.Length - 2, 2);
            }
        }

        for (int i = 0; i < _keyConfigs.Length; i++)
        {
            _keyConfigs[i] = sb[i].ToString();
        }
    }

    void ResetKey(Input.VirtualKey target)
    {
        HashSet<ConsoleKey> keys = new();

        foreach (var key in Input.GetKeys())
        {
            if (key.virtualKey == target)
            {
                keys.Add(key.actualKey);
            }
        }

        foreach (var key in keys)
        {
            Input.SetKey(key, Input.VirtualKey.NONE);
        }

        Input.SetKey(defaultKeys[target], target);
        UpdateKeyConfigText();
    }

    public override void Update(float deltaTime)
    {
        if (_keyWaiting)
        {
            if (Input.IsKey(Input.VirtualKey.SPECIAL_ESC))
            {
                _keyWaiting = false;
            }
            else
            {
                return;
            }
        }


        if (Input.IsKeyDown(Input.VirtualKey.SPECIAL_ESC))
        {
            Debug = "ESC";
            if (_keyEdit)
            {
                switch (_selectedMenu)
                {
                    case 0:
                        ResetKey(Input.VirtualKey.Up);
                        break;
                    case 1:
                        ResetKey(Input.VirtualKey.Down);
                        break;
                    case 2:
                        ResetKey(Input.VirtualKey.Right);
                        break;
                    case 3:
                        ResetKey(Input.VirtualKey.Left);
                        break;
                    case 4:
                        ResetKey(Input.VirtualKey.Select);
                        break;
                    case 5:
                        ResetKey(Input.VirtualKey.HardDrop);
                        break;
                    case 6:
                        ResetKey(Input.VirtualKey.SoftDrop);
                        break;
                    case 7:
                        ResetKey(Input.VirtualKey.Keep);
                        break;
                    case 8:
                        ResetKey(Input.VirtualKey.SpinRight);
                        break;
                    case 9:
                        ResetKey(Input.VirtualKey.SpinLeft);
                        break;
                }

            }
        }

        if (Input.IsKeyDown(Input.VirtualKey.Down))
        {
            if (_keyEdit)
            {
                if (_selectedMenu < keyConfigY.Length - 1)
                {
                    _selectedMenu++;
                }
            }
            else
            {
                if (_selectedMenu < configY.Length - 1)
                {
                    _selectedMenu++;
                }
            }
        }

        if (Input.IsKeyDown(Input.VirtualKey.Up))
        {
            if (_selectedMenu > 0)
            {
                _selectedMenu--;
            }
        }

        if (Input.IsKeyDown(Input.VirtualKey.Select))
        {
            if (_keyEdit)
            {
                switch (_selectedMenu)
                {
                    case 0:
                        _keyEditingTarget = Input.VirtualKey.Up;
                        break;
                    case 1:
                        _keyEditingTarget = Input.VirtualKey.Down;
                        break;
                    case 2:
                        _keyEditingTarget = Input.VirtualKey.Right;
                        break;
                    case 3:
                        _keyEditingTarget = Input.VirtualKey.Left;
                        break;
                    case 4:
                        _keyEditingTarget = Input.VirtualKey.Select;
                        break;
                    case 5:
                        _keyEditingTarget = Input.VirtualKey.HardDrop;
                        break;
                    case 6:
                        _keyEditingTarget = Input.VirtualKey.SoftDrop;
                        break;
                    case 7:
                        _keyEditingTarget = Input.VirtualKey.Keep;
                        break;
                    case 8:
                        _keyEditingTarget = Input.VirtualKey.SpinRight;
                        break;
                    case 9:
                        _keyEditingTarget = Input.VirtualKey.SpinLeft;
                        break;
                    case 10:
                        _keyEdit = false;
                        _selectedMenu = 0;
                        break;
                }

                if (_selectedMenu < 10)
                {
                    _keyWaiting = true;
                    Input.GetOneConsoleKey((key) =>
                    {
                        if (!Input.IsConflict(key))
                        {
                            _keyWaiting = false;
                            Input.SetKey(key, _keyEditingTarget);
                            UpdateKeyConfigText();
                        }
                        else
                        {
                            Debug = key.ToString();
                            _keyWaiting = false;
                        }
                    });
                }
            }
            else
            {
                switch (_selectedMenu)
                {
                    case 2:
                        _keyEdit = true;
                        _selectedMenu = 0;
                        break;
                    case 3:
                        DataManager.ApplyConfig();
                        DataManager.SaveConfig();
                        RequestReturnMainScreen?.Invoke();
                        break;
                }
            }
        }

        if (Input.IsKeyDown(Input.VirtualKey.Left))
        {
            if (!_keyEdit)
            {
                switch (_selectedMenu)
                {
                    case 0:
                        if (DataManager.CurrentGameData.BGMVolume > 0)
                        {
                            DataManager.CurrentGameData.BGMVolume--;
                        }
                        break;
                    case 1:
                        if (DataManager.CurrentGameData.SEVolume > 0)
                        {
                            DataManager.CurrentGameData.SEVolume--;
                        }
                        break;
                }
            }
        }

        if (Input.IsKeyDown(Input.VirtualKey.Right))
        {
            if (!_keyEdit)
            {
                switch (_selectedMenu)
                {
                    case 0:
                        if (DataManager.CurrentGameData.BGMVolume < 10)
                        {
                            DataManager.CurrentGameData.BGMVolume++;
                        }
                        break;
                    case 1:
                        if (DataManager.CurrentGameData.SEVolume < 10)
                        {
                            DataManager.CurrentGameData.SEVolume++;
                        }
                        break;
                }
            }
        }
    }
}

