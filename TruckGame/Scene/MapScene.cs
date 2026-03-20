using Framework.Engine;
using System;
using System.Collections.Generic;


public class MapScene : Scene
{
    public event GameAction ConsoleShowRequest;

    public override void Load()
    {
        
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.DrawBox(0, 0, 110, 30, ConsoleColor.White);
        buffer.WriteText(2, 0, "전\0술\0지\0도\0 1.0", ConsoleColor.White);
        DrawGameObjects(buffer);
    }

    public override void Unload()
    {
        
    }

    public override void Update(float deltaTime)
    {
        if (Input.IsKeyDown(ConsoleKey.Tab))
        {
            ConsoleShowRequest?.Invoke();
            return;
        }
        UpdateGameObjects(deltaTime);
    }

    public void AddObject(WarObject obj)
    {
        switch (obj)
        {
            case Cannon cannon:
                MapObject mapObject = new MapObject(this, cannon, '★', ConsoleColor.DarkGreen);
                AddGameObject(mapObject);
            break;
        }
    }
}