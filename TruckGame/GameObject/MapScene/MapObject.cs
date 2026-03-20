using Framework.Engine;
using System;

public class MapObject : GameObject
{
    WarObject _warObject;
    char _maker;
    ConsoleColor _color;

    public MapObject(Scene scene, WarObject warObject, char marker, ConsoleColor color) : base(scene)
    {
        _warObject = warObject;
        _maker = marker;
        _color = color;
        Name = warObject.Name;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        var pos = _warObject.Pos;
        buffer.SetCell(pos.x, pos.y, _maker, _color);

    }

    public override void Update(float deltaTime)
    {
        //
    }
}
