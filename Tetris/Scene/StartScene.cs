using Framework.Engine;

namespace Framework.Tetris
{
    internal class StartScene : Scene
    {
        public event GameAction<int> MenuSelected;

        int _selectedMenu;

        public override void Draw(ScreenBuffer buffer)
        {
            buffer.DrawBox(0, 0, 82, 22);
            buffer.WriteTextCentered(4,  ".__________. _______.  ___________. .______      __      ____.");
            buffer.WriteTextCentered(5,  " |          | |  ____|  |          | |   _  ＼\0   |  |    /    |");
            buffer.WriteTextCentered(6,  " `---|  |---` |  |__    `---|  |---` |  |_)  |   |  |   |  (--` ");
            buffer.WriteTextCentered(7,  "    |  |     |   __|       |  |     |       /   |  |   ＼\0 ＼\0  ");
            buffer.WriteTextCentered(8,  "   |  |     |  |____      |  |     |  |＼\0 ＼\0.  |  | .--)  | ");
            buffer.WriteTextCentered(9,  "  |__|     |_______|     |__|     |__| `.__|  |__| |____/ ");


            buffer.WriteTextCentered(12, "테트리스");

            if (_selectedMenu == 0)
                buffer.WriteTextCentered(13, "> 게임 시작  ");
            else
                buffer.WriteTextCentered(13, "게임 시작");

            if (_selectedMenu == 1)
                buffer.WriteTextCentered(14, "> 설정  ");
            else
                buffer.WriteTextCentered(14, "설정");

            if (_selectedMenu == 2)
                buffer.WriteTextCentered(15, "> 게임 종료  ");
            else
                buffer.WriteTextCentered(15, "게임 종료");

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
            if (Input.IsKeyDown(Input.VirtualKey.Select))
            {
                MenuSelected?.Invoke(_selectedMenu);
            }
            else if (Input.IsKeyDown(Input.VirtualKey.Down))
            {
                if (_selectedMenu < 2)
                    _selectedMenu++;
            }
            else if (Input.IsKeyDown(Input.VirtualKey.Up))
            {
                if (_selectedMenu > 0)
                    _selectedMenu--;
            }

            UpdateGameObjects(deltaTime);
        }
    }
}
