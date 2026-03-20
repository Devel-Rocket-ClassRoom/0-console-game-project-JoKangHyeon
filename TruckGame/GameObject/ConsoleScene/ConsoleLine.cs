using Framework.Engine;

public class ConsoleLine : GameObject
{
    string _text;
    int _posX;
    int _posY; 

    public ConsoleLine(Scene scene, string text, int posX, int posY) : base(scene)
    {
        _text = text;
        _posX = posX;
        _posY = posY;
    }

    public void SetPos(int x, int y)
    {
        _posX = x;
        _posY = y;
    }
    public void SetText(string text)
    {
        _text = text;  
    }


    public override void Draw(ScreenBuffer buffer)
    {
        buffer.WriteText(_posX, _posY, _text);
    }

    public override void Update(float deltaTime)
    {
    }
}
