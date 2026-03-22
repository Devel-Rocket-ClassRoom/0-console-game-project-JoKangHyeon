using Framework.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Tetris
{
    public class TetrisGame : GameApp
    {
        public const int k_BoardSizeX = 10;
        public const int k_BoardSizeY = 20;
        public const int k_YBuffer = 6;

        List<Scene> sceneList = new List<Scene>()
        {
            new StartScene(),
            new GameScene(),
            new ConfigScene(),
        };

        SceneManager<Scene> _scene = new();

        public TetrisGame() : base(84, 24)
        {
            Console.OutputEncoding = Encoding.UTF8;
        }

        protected override void Draw()
        {
            _scene.CurrentScene?.Draw(Buffer);
        }

        protected override void Initialize()
        {
            ((StartScene)sceneList[0]).MenuSelected += GameStart;
            ((GameScene)sceneList[1]).RestartRequest += () => _scene.ChangeScene(sceneList[0]);
            ((ConfigScene)sceneList[2]).RequestReturnMainScreen += () => _scene.ChangeScene(sceneList[0]);
            DataManager.LoadData();
            DataManager.ApplyConfig();
            _scene.ChangeScene(sceneList[0]);
        }

        protected override void Update(float deltaTime)
        {
            _scene.CurrentScene?.Update(deltaTime);
        }

        public void GameStart(int gameType)
        {
            switch (gameType)
            {
                case 0:
                    _scene.ChangeScene(sceneList[1]);
                    break;
                case 1:
                    _scene.ChangeScene(sceneList[2]);
                    break;
                case 2:
                    Quit();
                    break;
            }
        }
    }
}
