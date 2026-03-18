using System;
using System.Collections.Generic;
using System.Text;
using Framework.Engine;

namespace TruckGame.TruckGame
{
    public class TitleScene : Scene
    {
        int selectedMenu = 0;

        public TitleScene()
        {
        }

        public override void Draw(ScreenBuffer buffer)
        {
            buffer.DrawBox(0, 0, 110, 30, ConsoleColor.White);
            buffer.WriteText(5, 5, "터\0미\0널\0을\0 향\0해\0서\0");

            buffer.WriteText(90, 25, "새\0 게\0임\0");
            buffer.WriteText(90, 26, "이\0어\0하\0기\0");
            buffer.WriteText(90, 27, "게\0임\0 종\0료\0");

            buffer.WriteText(88, 25 + selectedMenu, ">");
        }

        public override void Load()
        {
            
        }

        public override void Unload()
        {
            
        }

        public override void Update(float deltaTime)
        {
            if (Input.IsKeyDown(ConsoleKey.DownArrow))
            {
                if(selectedMenu < 2)
                {
                    selectedMenu++;
                }
            }

            if (Input.IsKeyDown(ConsoleKey.UpArrow))
            {
                if (selectedMenu > 0)
                {
                    selectedMenu--;
                }
            }
        }
    }
}
