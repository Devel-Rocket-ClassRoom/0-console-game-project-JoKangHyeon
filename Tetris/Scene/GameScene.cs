using Framework.Engine;
using NAudio.Wave;
using System;
using TruckGame.Properties;

namespace Framework.Tetris
{
    internal class GameScene : Scene
    {
        public event GameAction RestartRequest;

        bool _multiplay = false;

        BoardObject _boardP1;
        BoardObject _boardP2;
        TetrisObject _tetrisP1;
        TetrisObject _tetrisP2;

        float _moveCooltime = 0.3f;
        float _moveTimer = 0;

        float _scoreCooltime = 0.5f;
        float _scoreTimer = 1;
        bool _tetris = false;
        int _lastScore = 0;


        int _scoreP1 = 0;

        bool _quick = false;
        bool _gameOver = false;

        WaveOutEvent _bgmPlayer;
        WaveOutEvent _buttonSound;

        public override void Draw(ScreenBuffer buffer)
        {
            buffer.DrawBox(21, 0, 10, 6);
            buffer.DrawBox(21, 5, 10, 6);
            buffer.DrawBox(21, 12, 3, 10);
            buffer.DrawBox(0, 0, 22, 22);
            buffer.WriteText(24, 0, "NEXT");
            buffer.WriteText(24, 5, "KEEP");
            buffer.WriteText(11, 22, _scoreP1.ToString());

            if (_multiplay)
            {
                buffer.DrawBox(33, 0, 10, 6);
                buffer.DrawBox(33, 5, 10, 6);
                buffer.DrawBox(40, 12, 3, 10);
                buffer.DrawBox(42, 0, 22, 22);
                buffer.WriteText(36, 0, "NEXT");
                buffer.WriteText(36, 5, "KEEP");
                buffer.WriteText(40, 22, "0000000000");
            }

            DrawGameObjects(buffer);

            if (_scoreTimer < _scoreCooltime)
            {

                if (_tetris)
                {
                    buffer.WriteText(8, 11, "TETRIS!", Enum.GetValues<ConsoleColor>()[((int)(_scoreTimer * 12)) % 12]);
                }
                buffer.WriteText(9, 12, _lastScore.ToString());
            }

            if (_gameOver)
            {
                buffer.WriteText(11-5, 9, "Game Over!");

                buffer.WriteText(11 - 3, 10, "Score");
                string score = _scoreP1.ToString();
                buffer.WriteText(11 - score.Length/2, 11, score);
                buffer.WriteText(5, 13, "Press <select>");
                buffer.WriteText(6, 14, "to restart");
            }

        }

        public void SetMultiplay(bool on)
        {
            _multiplay = on;
        }

        void GameOver()
        {
            _gameOver = true;
        }

        void AddScore(int amount, bool tetris)
        {
            _scoreTimer = 0;
            _lastScore = amount;
            _tetris = tetris;
        }

        public override void Load()
        {
            if (_boardP1 == null)
            {
                _boardP1 ??= new BoardObject(this, 0, 1);
                _tetrisP1 ??= new TetrisObject(this, 1, 1, _boardP1, 22, 1);
                _boardP2 ??= new BoardObject(this, 43, 1);
                _tetrisP2 ??= new TetrisObject(this, 43, 1, _boardP2, 43, 2);

                AddGameObject(_boardP1);
                AddGameObject(_boardP2);
                AddGameObject(_tetrisP1);
                AddGameObject(_tetrisP2);

                _boardP1.GameOver += GameOver;
                _boardP1.GetScore += AddScore;
            }



            _boardP1.IsActive = true;
            _boardP1.Clear();
            _tetrisP1.IsActive = true;
            _tetrisP1.Clear();


            if (_multiplay)
            {
                _boardP2.IsActive = true;
                _boardP2.Clear();
                _tetrisP2.IsActive = true;
                _tetrisP2.Clear();
            }
            else
            {
                _boardP2.IsActive = false;
                _tetrisP2.IsActive = false;
            }


            _tetrisP1.Clear();
            _boardP1.Clear();

            _scoreP1 = 0;
            _gameOver = false;

            var resourceStream = Resources.GameMusic;
            var waveReader = new WaveFileReader(resourceStream);
            var wavStream = new RawSourceWaveStream(waveReader, new WaveFormat(44100, 24, 2));
            _bgmPlayer = new WaveOutEvent();
            _bgmPlayer.Init(wavStream);
            _bgmPlayer.Volume = DataManager.CurrentGameData.BGMVolume / 10f;
            _bgmPlayer.Play();


            wavStream = new RawSourceWaveStream(waveReader, new WaveFormat(44100, 16, 1));
            _buttonSound = new WaveOutEvent();
            _buttonSound.Init(wavStream);
            _buttonSound.Volume = DataManager.CurrentGameData.SEVolume / 10f;
        }

        public override void Unload()
        {
            _buttonSound?.Dispose();
            _bgmPlayer?.Dispose();
        }

        public override void Update(float deltaTime)
        {

            if (_gameOver)
            {
                if (Input.IsKeyDown(Input.VirtualKey.Select))
                {
                    RestartRequest?.Invoke();
                    PlayButtonSound();
                }
                return;
            }

            if (Input.IsKeyDown(Input.VirtualKey.Left))
            {
                _tetrisP1.Move(-1, 0);
            }
            else if (Input.IsKeyDown(Input.VirtualKey.Right))
            {
                _tetrisP1.Move(1, 0);
            }

            if (Input.IsKeyDown(Input.VirtualKey.Up) || Input.IsKeyDown(Input.VirtualKey.SpinLeft))
            {
                _tetrisP1.Spin(false);
            }
            else if (Input.IsKeyDown(Input.VirtualKey.SpinRight))
            {
                _tetrisP1.Spin(true);
            }

            if (Input.IsKeyDown(Input.VirtualKey.Down) || Input.IsKeyDown(Input.VirtualKey.SoftDrop))
            {
                _quick = true;
            }
            if (Input.IsKeyUp(Input.VirtualKey.Down) || Input.IsKeyUp(Input.VirtualKey.SoftDrop))
            {
                _quick = false;
            }

            if (Input.IsKeyDown(Input.VirtualKey.HardDrop))
            {
                _tetrisP1.Drop();
                return;
            }

            if (Input.IsKeyDown(Input.VirtualKey.Keep))
            {
                _tetrisP1.Keep();
                PlayButtonSound();
            }

            if (_gameOver)
                return;
            _moveTimer += deltaTime;
            if (_quick || _moveTimer > _moveCooltime)
            {
                _tetrisP1.Move(0, 1);
                //tetrisP2.Move(0,1);
                _moveTimer = 0;
            }

            if (_scoreTimer < _scoreCooltime)
            {
                _scoreTimer += deltaTime;
            }
        }
        void PlayButtonSound()
        {
            var resourceStream = Resources.button_1;
            var waveReader = new WaveFileReader(resourceStream);

            _buttonSound.Stop();
            _buttonSound.Init(waveReader);
            _buttonSound.Play();
        }
    }

}
