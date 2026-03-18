using Framework.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace TruckGame.TruckGame
{
    internal class TruckDefenceGame : GameApp
    {
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

        SceneManager<Scene> _sceneManager = new();
        SaveData _saveData;

        public TruckDefenceGame():this(110,30)
        {

        }

        public TruckDefenceGame(int width, int height) : base(width, height)
        {
            _instance = this;
        }
        protected override void Initialize()
        {
            _sceneManager.ChangeScene(new TitleScene());

        }

        protected override void Draw()
        {
            _sceneManager.CurrentScene?.Draw(Buffer);
        }


        protected override void Update(float deltaTime)
        {
            _sceneManager.CurrentScene.Update(deltaTime);
        }
    }
}
