using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Framework.Engine
{
    public static class Input
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private static readonly HashSet<ConsoleKey> s_currentKeys = new();
        private static readonly HashSet<ConsoleKey> s_previousKeys = new();

        private static readonly ConsoleKey[] s_trackedKeys =
        {
            // 방향키
            ConsoleKey.UpArrow, ConsoleKey.DownArrow,
            ConsoleKey.LeftArrow, ConsoleKey.RightArrow,

            // 숫자키 (일반)
            ConsoleKey.D0, ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3, ConsoleKey.D4,
            ConsoleKey.D5, ConsoleKey.D6, ConsoleKey.D7, ConsoleKey.D8, ConsoleKey.D9,

            // 숫자키 (넘패드)
            ConsoleKey.NumPad0, ConsoleKey.NumPad1, ConsoleKey.NumPad2, ConsoleKey.NumPad3,
            ConsoleKey.NumPad4, ConsoleKey.NumPad5, ConsoleKey.NumPad6, ConsoleKey.NumPad7,
            ConsoleKey.NumPad8, ConsoleKey.NumPad9,

            // 특수키
            ConsoleKey.Enter, ConsoleKey.Escape, ConsoleKey.Spacebar,
            ConsoleKey.Tab, ConsoleKey.Backspace,

            // 영문자
            ConsoleKey.H, ConsoleKey.S, ConsoleKey.Y, ConsoleKey.N,
            ConsoleKey.W, ConsoleKey.A, ConsoleKey.D,
        };

        public static bool HasInput => s_currentKeys.Count > 0;

        public static bool TextInputMode { get; set; }

        const int k_LShift = 0xA0;
        const int k_RShift = 0xA1;

        public static bool ShiftHeld { get; private set; }
        public static char TextInputChar { get; private set; }
        public static bool TextInputed { get; private set; }

        public static readonly List<(ConsoleKey key, char input)> TextInputs = new List<(ConsoleKey, char)>()
        {
            // Numbers
            ( ConsoleKey.D0, '0' ), ( ConsoleKey.D1, '1' ), ( ConsoleKey.D2, '2' ),
            ( ConsoleKey.D3, '3' ), ( ConsoleKey.D4, '4' ), ( ConsoleKey.D5, '5' ),
            ( ConsoleKey.D6, '6' ), ( ConsoleKey.D7, '7' ), ( ConsoleKey.D8, '8' ),
            ( ConsoleKey.D9, '9' ),

            // Letters (Defaults to Uppercase)
            ( ConsoleKey.A, 'A' ), ( ConsoleKey.B, 'B' ), ( ConsoleKey.C, 'C' ),
            ( ConsoleKey.D, 'D' ), ( ConsoleKey.E, 'E' ), ( ConsoleKey.F, 'F' ),
            ( ConsoleKey.G, 'G' ), ( ConsoleKey.H, 'H' ), ( ConsoleKey.I, 'I' ),
            ( ConsoleKey.J, 'J' ), ( ConsoleKey.K, 'K' ), ( ConsoleKey.L, 'L' ),
            ( ConsoleKey.M, 'M' ), ( ConsoleKey.N, 'N' ), ( ConsoleKey.O, 'O' ),
            ( ConsoleKey.P, 'P' ), ( ConsoleKey.Q, 'Q' ), ( ConsoleKey.R, 'R' ),
            ( ConsoleKey.S, 'S' ), ( ConsoleKey.T, 'T' ), ( ConsoleKey.U, 'U' ),
            ( ConsoleKey.V, 'V' ), ( ConsoleKey.W, 'W' ), ( ConsoleKey.X, 'X' ),
            ( ConsoleKey.Y, 'Y' ), ( ConsoleKey.Z, 'Z' ),

            // Numpad
            ( ConsoleKey.NumPad0, '0' ), ( ConsoleKey.NumPad1, '1' ), ( ConsoleKey.NumPad2, '2' ),
            ( ConsoleKey.NumPad3, '3' ), ( ConsoleKey.NumPad4, '4' ), ( ConsoleKey.NumPad5, '5' ),
            ( ConsoleKey.NumPad6, '6' ), ( ConsoleKey.NumPad7, '7' ), ( ConsoleKey.NumPad8, '8' ),
            ( ConsoleKey.NumPad9, '9' ),
            ( ConsoleKey.Multiply, '*' ), ( ConsoleKey.Add, '+' ), ( ConsoleKey.Subtract, '-' ),
            ( ConsoleKey.Decimal, '.' ), ( ConsoleKey.Divide, '/' ),

            // Special Characters
            ( ConsoleKey.Spacebar, ' ' ),
        };


        public static void Poll()
        {
            s_previousKeys.Clear();
            foreach (var key in s_currentKeys)
            {
                s_previousKeys.Add(key);
            }

            s_currentKeys.Clear();

            if (TextInputMode)
            {
                TextInputed = false;

                foreach (var key in s_trackedKeys)
                {
                    short state = GetAsyncKeyState((int)key);
                    if ((state & 0x8000) != 0)
                    {
                        s_currentKeys.Add(key);
                    }
                }

                short lShiftState = GetAsyncKeyState(k_LShift);
                short rShiftState = GetAsyncKeyState(k_RShift);
                if ((lShiftState & 0x8000) != 0 || (rShiftState & 0x8000) != 0)
                {
                    ShiftHeld = true;
                }
                else
                {
                    ShiftHeld = false;
                }

                foreach (var kvp in TextInputs)
                {
                    short state = GetAsyncKeyState((int)kvp.key);
                    if ((state & 0x8000) != 0)
                    {
                        TextInputed = true;
                        if (ShiftHeld)
                        {
                            if (kvp.key == ConsoleKey.D9)
                            {
                                TextInputChar = '(';
                            }
                            else if (kvp.key == ConsoleKey.D0)
                            {
                                TextInputChar = ')';
                            }
                            else
                            {
                                TextInputChar = char.ToUpper(kvp.input);
                            }
                        }
                        else
                        {
                            TextInputChar = char.ToLower(kvp.input);
                        }

                    }
                }
            }
            else
            {
                foreach (var key in s_trackedKeys)
                {
                    short state = GetAsyncKeyState((int)key);
                    if ((state & 0x8000) != 0)
                    {
                        s_currentKeys.Add(key);
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
        public static bool IsKey(ConsoleKey key)
        {
            return s_currentKeys.Contains(key);
        }

        /// <summary>
        /// 이전 프레임에 안 눌렸다가 이번 프레임에 눌린 순간 (edge-triggered)
        /// </summary>
        public static bool IsKeyDown(ConsoleKey key)
        {
            return s_currentKeys.Contains(key) && !s_previousKeys.Contains(key);
        }

        /// <summary>
        /// 이전 프레임에 눌렸다가 이번 프레임에 뗀 순간
        /// </summary>
        public static bool IsKeyUp(ConsoleKey key)
        {
            return !s_currentKeys.Contains(key) && s_previousKeys.Contains(key);
        }
    }
}
