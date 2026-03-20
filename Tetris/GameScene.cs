using Framework.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Tetris
{
    internal class GameScene : Scene
    {
        bool multiplay = false;

        BoardObject boardP1;
        BoardObject boardP2;
        TetrisObject tetrisP1;
        TetrisObject tetrisP2;

        float moveCooltime = 0.3f;
        float moveTimer = 0;

        bool quick = false;

        public override void Draw(ScreenBuffer buffer)
        {
            buffer.DrawBox(21, 0, 10, 5);
            buffer.DrawBox(21, 12, 3, 10);
            buffer.DrawBox(0, 0, 22, 22);
            buffer.WriteText(24, 0, "NEXT");
            buffer.WriteText(11, 22, "0000000000");

            if (multiplay)
            {
                buffer.DrawBox(33, 0, 10, 5);
                buffer.DrawBox(40, 12, 3, 10);
                buffer.DrawBox(42, 0, 22, 22);
                buffer.WriteText(36, 0, "NEXT");
                buffer.WriteText(40, 22, "0000000000");
            }

            buffer.SetCell(2, 2, '\u2588');
            buffer.SetCell(3, 2, '\u2588');

            DrawGameObjects(buffer);
        }

        public void SetMultiplay(bool on)
        {
            multiplay = on;
        }

        public override void Load()
        {
            if (boardP1 == null)
            {
                boardP1 ??= new BoardObject(this, 1, 1);
                tetrisP1 ??= new TetrisObject(this, 1, 1,boardP1);
                boardP2 ??= new BoardObject(this, 43, 1);
                tetrisP2 ??= new TetrisObject(this, 43, 1,boardP2);

                AddGameObject(boardP1);
                AddGameObject(boardP2);
                AddGameObject(tetrisP1);
                AddGameObject(tetrisP2);
            }



            boardP1.IsActive = true;
            boardP1.Clear();
            tetrisP1.IsActive = true;
            tetrisP1.Clear();


            if (multiplay)
            {
                boardP2.IsActive = true;
                boardP2.Clear();
                tetrisP2.IsActive = true;
                tetrisP2.Clear();
            }
            else
            {
                boardP2.IsActive = false;
                tetrisP2.IsActive = false;
            }


            tetrisP1.Clear();
        }

        public override void Unload()
        {
            
        }

        public override void Update(float deltaTime)
        {

            if (Input.IsKeyDown(ConsoleKey.LeftArrow))
            {
                tetrisP1.Move(-1, 0);
            }
            else if (Input.IsKeyDown(ConsoleKey.RightArrow))
            {
                tetrisP1.Move(1, 0);
            }
            
            if(Input.IsKeyDown(ConsoleKey.UpArrow) || Input.IsKeyDown(ConsoleKey.Z))
            {
                tetrisP1.Spin(false);
            }
            else if (Input.IsKeyDown(ConsoleKey.X))
            {
                tetrisP1.Spin(true);
            }

            if (Input.IsKeyDown(ConsoleKey.DownArrow))
            {
                quick = true;
            }
            if (Input.IsKeyUp(ConsoleKey.DownArrow))
            {
                quick = false;
            }

            moveTimer += deltaTime;
            if (quick|| moveTimer > moveCooltime)
            {
                tetrisP1.Move(0,1);
                //tetrisP2.Move(0,1);
                moveTimer = 0;
            }
        }
    }
}
