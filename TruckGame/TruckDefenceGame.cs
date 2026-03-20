using Framework.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace TruckGame.TruckGame
{
    internal class TruckDefenceGame : GameApp
    {
        #region Singleton
        static TruckDefenceGame _instance;

        public static TruckDefenceGame Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new TruckDefenceGame();
                return _instance;
            }
        }
        #endregion


        private SceneManager<Scene> _sceneManager = new();
        
        
        SaveData _saveData;
        GameData _gameData;
        public ConsoleScene consoleScene = new ConsoleScene();
        public MapScene mapScene = new MapScene();

        public TruckDefenceGame():this(110,30)
        {

        }

        public TruckDefenceGame(int width, int height) : base(width, height)
        {
            _instance = this;
        }
        protected override void Initialize()
        {
            TitleScene titleScene = new TitleScene();
            titleScene.SceneRequested += ShowScene;
            _sceneManager.ChangeScene(titleScene);
        }

        protected override void Draw()
        {
            _sceneManager.CurrentScene?.Draw(Buffer);
        }


        protected override void Update(float deltaTime)
        {
            _sceneManager.CurrentScene.Update(deltaTime);
        }

        private void ShowScene(int sceneId)
        {
            switch (sceneId)
            {
                case 0://To Title
                    _sceneManager.ChangeScene(new TitleScene()); 
                    break;
                case 1://New Game Start
                    consoleScene.MapShowRequest += () => ShowScene(5);
                    _sceneManager.ChangeScene(consoleScene);
                    StartBattle(-1);
                    break;
                case 2:
                    //TODO : 로드
                    break;
                case 3:
                    Quit();
                    break;
                case 4:
                    _sceneManager.ChangeScene(consoleScene);
                    break;
                case 5:
                    _sceneManager.ChangeScene(mapScene);
                    break;
            }
        }

        private void StartBattle(int r)
        {
            _gameData = new();
            switch (r)
            {
                case -1://TUTORIAL
                    
                    break;
            }
        }
    }
}
