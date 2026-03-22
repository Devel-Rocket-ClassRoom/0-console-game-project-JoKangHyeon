using Framework.Engine;
using NAudio.Wave;
using TruckGame.Properties;

namespace Framework.Tetris
{
    internal class StartScene : Scene
    {
        public event GameAction<int> MenuSelected;

        int _selectedMenu;

        WaveOutEvent _bgmPlayer = new();
        WaveOutEvent _buttonSound = new();

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
            var resourceStream = Resources.TitleMusic;
            var waveReader = new WaveFileReader(resourceStream);
            var wavStream = new RawSourceWaveStream(waveReader, new WaveFormat(44100, 24, 2));
            _bgmPlayer = new WaveOutEvent();
            _bgmPlayer.Init(wavStream);
            _bgmPlayer.Volume = DataManager.CurrentGameData.BGMVolume/10f;
            _bgmPlayer.Play();


            wavStream = new RawSourceWaveStream(waveReader, new WaveFormat(44100, 16, 1));
            _buttonSound = new WaveOutEvent();
            _buttonSound.Init(wavStream);
            _buttonSound.Volume = DataManager.CurrentGameData.SEVolume/10f;
        }

        public override void Unload()
        {
            _bgmPlayer.Dispose();
            _buttonSound.Dispose();
        }

        public override void Update(float deltaTime)
        {
            if (Input.IsKeyDown(Input.VirtualKey.Select))
            {
                MenuSelected?.Invoke(_selectedMenu);
            }
            else if (Input.IsKeyDown(Input.VirtualKey.Down))
            {
                var resourceStream = Resources.button_1;
                var waveReader = new WaveFileReader(resourceStream);

                _buttonSound.Stop();
                _buttonSound.Init(waveReader);
                _buttonSound.Play();
                if (_selectedMenu < 2)
                    _selectedMenu++;
            }
            else if (Input.IsKeyDown(Input.VirtualKey.Up))
            {
                var resourceStream = Resources.button_1;
                var waveReader = new WaveFileReader(resourceStream);

                _buttonSound.Stop();
                _buttonSound.Init(waveReader);
                _buttonSound.Play();
                if (_selectedMenu > 0)
                    _selectedMenu--;
            }

            UpdateGameObjects(deltaTime);
        }
    }
}
