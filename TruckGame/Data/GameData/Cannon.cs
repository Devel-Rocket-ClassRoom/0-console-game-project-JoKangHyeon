using System;
using System.Collections.Generic;
using System.ComponentModel;

public abstract class WarObject
{
    protected int _posX, _posY;
    protected int _moveTargetX, _moveTargetY;
    protected int _speed;

    public (int x, int y) Pos => (_posX, _posY);

    public string Name { get; protected set; }

    public abstract void Turn();

    public virtual void Move(int x, int y)
    {
        _moveTargetX = x;
        _moveTargetY = y;
    }

    public virtual void MoveTurn()
    {
        for(int i=0; i< _speed; i++)
        {
            if (_moveTargetX != _posX || _moveTargetY != _posY)
            {
                int rangeX = Math.Abs(_moveTargetX - _posX);
                int rangeY = Math.Abs(_moveTargetY - _posY);

                if(rangeX > rangeY * 1.5)
                {
                    _posX += _moveTargetX - _posX > 0 ? 1 : -1;
                }
                else
                {
                    _posY += _moveTargetY - _posY > 0 ? 1 : -1;
                }
            }
            else
            {
                break;
            }
        }
        
    }
}

public class Cannon  : WarObject{
    private int _angle, _traverse;

    public int Angle => _angle;
    public int Traverse => _traverse;



    public Cannon()
    {
        Name = "Cannon";
    }

    public override void Turn()
    {
        
    }

    public override void Move(int x, int y)
    {
        
    }
}


public class GameData
{
    Dictionary<(int, int), WarObject> warMap;


    public void Turn()
    {
        foreach (var kvp in warMap)
        {
            kvp.Value.Turn();
        }
    }
}