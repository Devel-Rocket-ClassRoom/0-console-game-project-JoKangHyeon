using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Framework.Engine
{
    public static class Input
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private static readonly HashSet<VirtualKey> s_currentKeys = new();
        private static readonly HashSet<VirtualKey> s_previousKeys = new();


        static GameAction<ConsoleKey> getOneConsoleKeyCallback;
        static bool getOneKeyWaiting;
        static ConsoleKey[] allKeys = Enum.GetValues<ConsoleKey>();

        public enum VirtualKey
        {
            NONE,
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
            SPECIAL_ESC,
        }

        private static readonly Dictionary<ConsoleKey, VirtualKey> s_keyMapping = new();

        public static bool HasInput => s_currentKeys.Count > 0;

        public static bool IsConflict(ConsoleKey key)
        {
            return s_keyMapping.ContainsKey(key);
        }

        public static void SetKey(ConsoleKey actualKey, VirtualKey virtualKey)
        {
            if (s_keyMapping.ContainsKey(actualKey))
            {
                s_keyMapping[actualKey] = virtualKey;
            }
            else
            {
                s_keyMapping.Add(actualKey, virtualKey);
            }
        }

        public static void ClearKey()
        {
            s_keyMapping.Clear();
        }

        public static IEnumerable<(ConsoleKey actualKey, VirtualKey virtualKey)> GetKeys()
        {
            foreach (var kvp in s_keyMapping)
            {
                yield return (kvp.Key, kvp.Value);
            }
        }

        public static void Poll()
        {
            if (getOneKeyWaiting)
            {
                GetOneKeyCheck();
            }

            s_previousKeys.Clear();
            foreach (var key in s_currentKeys)
            {
                s_previousKeys.Add(key);
            }

            s_currentKeys.Clear();
            if (!getOneKeyWaiting)
            {
                foreach (var key in s_keyMapping.Keys)
                {
                    short state = GetAsyncKeyState((int)key);
                    if ((state & 0x8000) != 0)
                    {
                        s_currentKeys.Add(s_keyMapping[key]);
                    }
                }
            }



            // Console 입력 버퍼 drain (잔여 키 방지)
            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
        }

        /// <summary>
        /// 이번 프레임에 눌려있는지 (held)
        /// </summary>
        public static bool IsKey(VirtualKey key)
        {
            return s_currentKeys.Contains(key);
        }

        /// <summary>
        /// 이전 프레임에 안 눌렸다가 이번 프레임에 눌린 순간 (edge-triggered)
        /// </summary>
        public static bool IsKeyDown(VirtualKey key)
        {
            return s_currentKeys.Contains(key) && !s_previousKeys.Contains(key);
        }

        /// <summary>
        /// 이전 프레임에 눌렸다가 이번 프레임에 뗀 순간
        /// </summary>
        public static bool IsKeyUp(VirtualKey key)
        {
            return !s_currentKeys.Contains(key) && s_previousKeys.Contains(key);
        }

        public static void GetOneConsoleKey(GameAction<ConsoleKey> callback)
        {
            getOneConsoleKeyCallback = callback;
            getOneKeyWaiting = true;

        }


        static void GetOneKeyCheck()
        {
            HashSet<ConsoleKey> pass = new();
            foreach (var key in s_keyMapping)
            {
                if(key.Value == VirtualKey.Select)
                {
                    pass.Add(key.Key);
                }
            }

            foreach (var key in allKeys)
            {
                if (pass.Contains(key))
                    continue;

                short state = GetAsyncKeyState((int)key);
                if ((state & 0x8000) != 0)
                {
                    getOneKeyWaiting = false;
                    getOneConsoleKeyCallback?.Invoke(key);
                    break;
                }
            }
        }

        
    }
}
