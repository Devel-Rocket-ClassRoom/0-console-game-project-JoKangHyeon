using Framework.Engine;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;

public class ConsoleScene : Scene
{
    public event GameAction<int> CmdTraverse;
    public event GameAction<int> CmdElevation;
    public event GameAction<string> CmdLoad;
    public event GameAction CmdShoot;
    public event GameAction<string, (int, int)> CmdMove;

    const float K_InputInterval = 0.07f;
    const string K_CommandPattern = @"(?<command>[a-zA-Z]+)\((?<args>[a-zA-Z0-9]*)\)";

    LinkedList<ConsoleLine> consoleLines = new LinkedList<ConsoleLine>();
    LinkedList<ConsoleLine> chatLines = new LinkedList<ConsoleLine>();
    Queue<ConsoleLine> consolelinePool = new();

    StringBuilder sb = new StringBuilder();

    float _inputTimer = 0;
    char _lastInput;

    public event GameAction MapShowRequest;


    public override void Load(){
        Input.TextInputMode = true;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.DrawBox(0, 0, 110, 30, ConsoleColor.Green);
        buffer.DrawVLine(55, 0, 30, bgColor: ConsoleColor.Green);
        buffer.WriteText(5, 27, sb.ToString(), ConsoleColor.Green);
        buffer.SetCell(3, 27, '>', ConsoleColor.Green);

        DrawGameObjects(buffer);
    }

    public override void Unload()
    {
        Input.TextInputMode = false;
    }

    public override void Update(float deltaTime)
    {
        _inputTimer += deltaTime;

        if (Input.IsKeyDown(ConsoleKey.Tab))
        {
            MapShowRequest?.Invoke();
            return;
        }

        if (Input.TextInputed)
        {
            if (Input.TextInputChar!= _lastInput || _inputTimer > K_InputInterval)
            {
                sb.Append(Input.TextInputChar);
                _inputTimer = 0;
                _lastInput = Input.TextInputChar;
            }
        }

        if (Input.IsKey(ConsoleKey.Backspace))
        {
            if (_inputTimer > K_InputInterval && sb.Length>0)
            {
                sb.Remove(sb.Length - 1, 1);
                _inputTimer = 0;
            }
        }
        
        if(!Input.TextInputed && !Input.IsKey(ConsoleKey.Backspace))
        {
            _inputTimer = K_InputInterval + 1;
        }

        if (Input.IsKeyDown(ConsoleKey.Enter))
        {
            string input = sb.ToString();
            sb.Clear();

            if(input.ToLower() == "help")
            {
                AddCommandLine("CalcElevation(Range) 거\0리\0에\0 맞\0는\0 부\0각\0을\0 계\0산\0합\0니\0다\0.");
                AddCommandLine("Range : 거\0리\0(숫\0자\0)");
                AddCommandLine();
                AddCommandLine("Traverse(Angle) 대\0포\0를\0 회\0전\0합\0니\0다\0. 시\0간\0이\0 필\0요\0해\0요\0.");
                AddCommandLine("Angle : 각\0도\0(숫\0자\0)");
                AddCommandLine();
                AddCommandLine("Elevation(Angle) 부\0각\0을\0 수\0정\0합\0니\0다\0. 시\0간\0이\0 필\0요\0해\0요\0.");
                AddCommandLine("장\0전\0이\0 되\0어\0있\0어\0야\0 포\0를\0 들\0어\0올\0릴\0 수\0 있\0어\0요\0.");
                AddCommandLine("Angle : 각\0도\0(숫\0자\0)");
                AddCommandLine();
                AddCommandLine("Load(Shell) 장\0전\0을\0 명\0령\0합\0니\0다\0. 시\0간\0이\0 필\0요\0해\0요\0.");
                AddCommandLine("Angle : 각\0도\0(숫\0자\0)");
                AddCommandLine();
                AddCommandLine("Shoot() 발\0사\0!");
                AddCommandLine();
                AddCommandLine("Move(name,posX,posY) 아\0군\0에\0게\0 이\0동\0을\0 명\0령\0합\0니\0다\0.");
                AddCommandLine("name : 우\0리\0 이\0름\0 외\0우\0고\0 계\0시\0죠\0?");
                AddCommandLine("posX : 좌\0표\0X(숫\0자\0)");
                AddCommandLine("posY : 좌\0표\0Y(숫\0자\0)");
                return;
            }


            AddCommandLine();
            AddCommandLine(input);
            var match = Regex.Match(input.ToLower(), K_CommandPattern);
            if (match.Success)
            {
                //AddLine(match.Groups["command"].Value);
                //AddLine(match.Groups["args"].Value);
                string command = match.Groups["command"].Value;
                string[] args = match.Groups["args"].Value.Split(',',StringSplitOptions.RemoveEmptyEntries);

                switch (command)
                {
                    case "calcelevation":
                        int range;
                        int angle;
                        if (args.Length == 1 && int.TryParse(args[0], out range))
                        {
                            angle = 100;
                            AddCommandLine($"range {range} : angle({angle})");
                        }
                        else
                        {
                            AddCommandLine("CalcElevation(range) 형\0태\0로\0 사\0용\0하\0세\0요\0.");
                        }
                        break;

                    case "traverse":
                        if (args.Length == 1 && int.TryParse(args[0], out angle))
                        {
                            CmdTraverse?.Invoke(angle);
                        }
                        else
                        {
                            AddCommandLine("Traverse(Angle) 형\0태\0로\0 사\0용\0하\0세\0요\0.");
                        }
                        break;

                    case "elevation":
                        if (args.Length == 1 && int.TryParse(args[0], out angle))
                        {
                            CmdElevation?.Invoke(angle);
                        }
                        else
                        {
                            AddCommandLine("Elevation(Angle) 형\0태\0로\0 사\0용\0하\0세\0요\0.");
                        }
                        break;

                    case "load":
                        if (args.Length == 1)
                        {
                            CmdLoad?.Invoke(args[0]);
                        }
                        else
                        {
                            AddCommandLine("Load(Shell) 형\0태\0로\0 사\0용\0하\0세\0요\0.");
                        }
                        break;

                    case "shoot":
                        if (args.Length == 1)
                        {
                            CmdShoot?.Invoke();
                        }
                        else
                        {
                            AddCommandLine("Shoot() 형\0태\0로\0 사\0용\0하\0세\0요\0.");
                        }
                        break;

                    case "move":
                        int posX;
                        int posY;
                        if (args.Length == 3 && int.TryParse(args[1], out posX) && int.TryParse(args[2], out posY))
                        {
                            CmdMove?.Invoke(args[0], (posX, posY));
                        }
                        else
                        {
                            AddCommandLine("Move(name,posX,posY) 형\0태\0로\0 사\0용\0하\0세\0요\0.");
                        }
                        break;

                    default:
                        AddCommandLine($"{input}는\0(은\0) 실\0행\0할\0 수\0 없\0는\0 명\0령\0어\0입\0니\0다\0.");
                        break;
                }
                
            }
            else
            {
                AddCommandLine($"{input}는\0(은\0) 실\0행\0할\0 수\0 없\0는\0 명\0령\0어\0입\0니\0다\0.");
            }
        }

        UpdateGameObjects(deltaTime);
    }

    public void AddCommandLine()
    {
        AddCommandLine("");
    }
    public void AddCommandLine(string line)
    {
        ConsoleLine consoleLine;
        if (consolelinePool.Count > 0)
        {
            consoleLine = consolelinePool.Dequeue();
            consoleLine.IsActive = true;
        }
        else
        {
            consoleLine = new ConsoleLine(this, line, 0, 0);
            AddGameObject(consoleLine);
        }

        consoleLines.AddFirst(consoleLine);

        if(consoleLines.Count > 20)
        {
            ConsoleLine removed = consoleLines.Last.Value;
            consoleLines.RemoveLast();
            consolelinePool.Enqueue(consoleLine);
            removed.IsActive = false;
        }

        int lineCount = 0;
        var node = consoleLines.First;
        while (node != null)
        {
            node.Value.SetPos(3, 26 - (lineCount++));
            node= node.Next;
        }
    }

    public void AddChatLine(string name, string line)
    {
        ConsoleLine consoleLine;
        if (consolelinePool.Count > 0)
        {
            consoleLine = consolelinePool.Dequeue();
            consoleLine.IsActive = true;
        }
        else
        {
            consoleLine = new ConsoleLine(this, line, 0, 0);
            AddGameObject(consoleLine);
        }

         chatLines.AddFirst(consoleLine);

        if (chatLines.Count > 20)
        {
            ConsoleLine removed = consoleLines.Last.Value;
            chatLines.RemoveLast();
            consolelinePool.Enqueue(consoleLine);
            removed.IsActive = false;
        }

        int lineCount = 0;
        var node = chatLines.First;
        while (node != null)
        {
            node.Value.SetPos(106, 26 - (lineCount++));
            node = node.Next;
        }
    }
}
