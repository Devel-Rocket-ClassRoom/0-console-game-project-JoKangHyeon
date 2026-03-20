using Framework.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Tetris
{
    internal class StartScene : Scene
    {
        public event GameAction<int> GameStartRequested;

        public override void Draw(ScreenBuffer buffer)
        {
            buffer.DrawBox(0, 0, 82, 22);
            buffer.WriteTextCentered(5, "테트리스");
            DrawGameObjects(buffer);
        }

        public override void Load()
        {
            //throw new NotImplementedException();
        }

        public override void Unload()
        {
            //throw new NotImplementedException();
        }

        public override void Update(float deltaTime)
        {
            if (Input.IsKeyDown(ConsoleKey.Enter))
            {
                GameStartRequested?.Invoke(0);
            }

            //throw new NotImplementedException();
            UpdateGameObjects(deltaTime);
        }
    }
}
