using Framework.Engine;
using System;

namespace Framework.Tetris
{
    internal class BoardObject : GameObject
    {
        int _posX, _posY;

        ConsoleColor[,] tetrisColors = new ConsoleColor[TetrisGame.k_BoardSizeX, TetrisGame.k_BoardSizeY + TetrisGame.k_YBuffer];
        bool[,] tetrisBlocks = new bool[TetrisGame.k_BoardSizeX, TetrisGame.k_BoardSizeY + TetrisGame.k_YBuffer];

        public BoardObject(Scene scene, int posX, int posY) : base(scene)
        {
            _posX = posX;
            _posY = posY;
        }

        public override void Draw(ScreenBuffer buffer)
        {
            for (int i = 0; i < tetrisColors.GetLength(0); i++)
            {
                for (int j = 0; j < tetrisColors.GetLength(1); j++)
                {
                    if (j - TetrisGame.k_YBuffer < 0)
                    {
                        continue;
                    }
                    if (tetrisBlocks[i, j])
                    {
                    buffer.SetCell(_posX + i * 2, _posY + j - TetrisGame.k_YBuffer, '\u2588', tetrisColors[i, j]);
                    buffer.SetCell(_posX + i * 2 + 1, _posY + j - TetrisGame.k_YBuffer, '\u2588', tetrisColors[i, j]);
                    }
                }
            }
        }

        public void Clear()
        {

        }

        public override void Update(float deltaTime)
        {

        }

        public bool CheckCollide(int x, int y)
        {
            if (x < 0 || y < 0 || x >= TetrisGame.k_BoardSizeX || y >= TetrisGame.k_BoardSizeY + TetrisGame.k_YBuffer)
                return true;
            return tetrisBlocks[x, y];
        }

        public void AddBlock(int x, int y, ConsoleColor color)
        {
            tetrisBlocks[x, y] = true;
            tetrisColors[x, y] = color;
        }

        public void CheckBoard()
        {
            int removeCount = 0;

            for (int y = 0; y < TetrisGame.k_BoardSizeY+TetrisGame.k_YBuffer; y++)
            {
                bool removeFlag = true;
                for (int x = 0; x < tetrisBlocks.GetLength(0); x++)
                {
                    if (!tetrisBlocks[x, y])
                    {
                        removeFlag = false;
                        break;
                    }
                }

                if (removeFlag)
                {
                    for (int x = 0; x < tetrisBlocks.GetLength(0); x++)
                    {
                        tetrisBlocks[x, y] = false;
                    }

                    for (int y2 = y; y2 < tetrisBlocks.GetLength(1)-1; y2++)
                    {
                        for (int x = 0; x < tetrisBlocks.GetLength(0); x++)
                        {
                            tetrisBlocks[x, y2] = tetrisBlocks[x, y2+1];
                        }
                    }
                    y++;
                }
            }


        }
    }
}
