using Framework.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Tetris
{
    internal class TetrisObject : GameObject
    {
        readonly int _screenPosX, _screenPosY;
        readonly int _nextPosX, _nextPosY;
        int _posX, _posY;
        BoardObject _board;

        SpinType _spin = 0;
        ShapeType _shapeType;
        ShapeType _keep = (ShapeType)(-1);

        bool _keepUsed = false;
        bool _dropFinished = false;

        List<ShapeType> _bag = new() { ShapeType.I, ShapeType.O, ShapeType.Z, ShapeType.S, ShapeType.J, ShapeType.L, ShapeType.T };
        int _bagCursor = 0;


        StringBuilder debug = new StringBuilder();

        private readonly List<ConsoleColor> _minoColors = new() { ConsoleColor.Blue, ConsoleColor.Yellow, ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.DarkBlue, ConsoleColor.DarkYellow, ConsoleColor.Magenta };

        private readonly Dictionary<ShapeType, List<bool[,]>> _minoShapes = new()
        {
            {
                ShapeType.I, new(){
                new[,]{{false, false, false, false},
                       {true,  true,  true,  true },
                       {false, false, false, false},
                       {false, false, false, false}},
                new[,]{{false, false, true, false},
                       {false, false, true, false},
                       {false, false, true, false},
                       {false, false, true, false}},
                new[,]{{false, false, false, false},
                       {false, false, false, false},
                       {true,  true,  true,  true },
                       {false, false, false, false}},
                new[,]{{false, true,  false, false},
                       {false, true,  false, false},
                       {false, true,  false, false},
                       {false, true,  false, false}},

                }
            },
            {
                ShapeType.O,new(){
                new[,]{{false, false, false, false},
                       {false, true,  true,  false},
                       {false, true,  true,  false},
                       {false, false, false, false}},
                new[,]{{false, false, false, false},
                       {false, true,  true,  false},
                       {false, true,  true,  false},
                       {false, false, false, false}},
                new[,]{{false, false, false, false},
                       {false, true,  true,  false},
                       {false, true,  true,  false},
                       {false, false, false, false}},
                new[,]{{false, false, false, false},
                       {false, true,  true,  false},
                       {false, true,  true,  false},
                       {false, false, false, false}},
                }
            },
            {
                ShapeType.Z,new(){
                new[,]{{true, true, false},
                       {false,true, true },
                       {false,false,false}},
                new[,]{{false,false,true },
                       {false,true, true },
                       {false,true, false}},
                new[,]{{false,false,false},
                       {true, true, false},
                       {false,true, true }},
                new[,]{{false,true ,false},
                       {true, true ,false},
                       {true, false,false}},
                }
            },
            {
                ShapeType.S,new(){
                new[,]{{false,true, true },
                       {true, true, false},
                       {false,false,false}},
                new[,]{{false,true, false },
                       {false,true, true },
                       {false,false,true}},
                new[,]{{false,false,false},
                       {false,true, true },
                       {true, true, false}},
                new[,]{{true, false,false},
                       {true, true ,false},
                       {false,true ,false}},
                }
            },
            {
                ShapeType.J,new(){
                new[,]{{true ,false,false},
                       {true, true, true },
                       {false,false,false}},
                new[,]{{false,true, true },
                       {false,true, false},
                       {false,true, false}},
                new[,]{{false,false,false},
                       {true, true, true },
                       {false,false,true }},
                new[,]{{false,true, false},
                       {false,true, false},
                       {true, true, false}},
                }
            },
            {
                ShapeType.L,new(){
                new[,]{{false,false,true },
                       {true, true, true },
                       {false,false,false}},
                new[,]{{false,true, false },
                       {false,true, false},
                       {false,true, true}},
                new[,]{{false,false,false},
                       {true, true, true },
                       {true, false,false}},
                new[,]{{true, true, false},
                       {false,true, false},
                       {false,true, false}},
                }
            },
            {
                ShapeType.T,new(){
                new[,]{{false,true, false},
                       {true, true, true },
                       {false,false,false}},
                new[,]{{false,true, false },
                       {false,true, true },
                       {false,true, false}},
                new[,]{{false,false,false},
                       {true, true, true },
                       {false,true, false}},
                new[,]{{false,true, false},
                       {true, true, false},
                       {false,true, false}},
                }
            },
        };

        private readonly Dictionary<SpinMoveType, List<(int X, int Y)>> wallKickLookupNormal = new()
        {
            { SpinMoveType.SR, new() { (0,0), (-1,0), (-1, 1), (0,-2), (-1,-2) } },
            { SpinMoveType.RS, new() { (0,0), ( 1,0), ( 1,-1), (0, 2), ( 1, 2) } },
            { SpinMoveType.RT, new() { (0,0), ( 1,0), ( 1,-1), (0, 2), ( 1, 2) } },
            { SpinMoveType.TR, new() { (0,0), (-1,0), (-1, 1), (0,-2), (-1,-2) } },
            { SpinMoveType.TL, new() { (0,0), ( 1,0), ( 1, 1), (0,-2), ( 1,-2) } },
            { SpinMoveType.LT, new() { (0,0), (-1,0), (-1,-1), (0, 2), (-1, 2) } },
            { SpinMoveType.LS, new() { (0,0), (-1,0), (-1,-1), (0, 2), (-1, 2) } },
            { SpinMoveType.SL, new() { (0,0), ( 1,0), ( 1, 1), (0,-2), ( 1,-2) } },
        };

        private readonly Dictionary<SpinMoveType, List<(int X, int Y)>> wallKickLookupI = new()
        {
            { SpinMoveType.SR, new() { (0,0), (-2,0), ( 1, 0), (-2,-1), ( 1, 2) } },
            { SpinMoveType.RS, new() { (0,0), ( 2,0), (-1, 0), ( 2, 1), (-1,-2) } },
            { SpinMoveType.RT, new() { (0,0), (-1,0), ( 2, 0), (-1, 2), ( 2,-1) } },
            { SpinMoveType.TR, new() { (0,0), ( 1,0), (-2, 0), ( 1,-2), (-2, 1) } },
            { SpinMoveType.TL, new() { (0,0), ( 2,0), (-1, 0), ( 2, 1), (-1,-2) } },
            { SpinMoveType.LT, new() { (0,0), (-2,0), ( 1, 0), (-2,-1), ( 1, 2) } },
            { SpinMoveType.LS, new() { (0,0), ( 1,0), (-2, 0), ( 1,-2), (-2, 1) } },
            { SpinMoveType.SL, new() { (0,0), (-1,0), ( 2, 0), (-1, 2), ( 2,-1) } },
        };

        public enum ShapeType
        {
            I, O, Z, S, J, L, T
        }
        public enum SpinType
        {
            S = 0,
            R = 1,
            T = 2,
            L = 3
        }
        enum SpinMoveType
        {
            SR,
            RS,
            RT,
            TR,
            TL,
            LT,
            LS,
            SL
        }

        public TetrisObject(Scene scene, int screenPosX, int screenPosY, BoardObject board, int nextPosX, int nextPosY) : base(scene)
        {
            _screenPosX = screenPosX;
            _screenPosY = screenPosY;
            _nextPosX = nextPosX;
            _nextPosY = nextPosY;
            _board = board;
        }

        public override void Draw(ScreenBuffer buffer)
        {
            //buffer.WriteText(0, 0, $"{_posX},{_posY}");
            //buffer.WriteText(0, 1, debug.ToString());
            var drawTarget = _minoShapes[_shapeType][(int)_spin];
            var nextMino = _minoShapes[_bag[_bagCursor]][(int)SpinType.S];
            for (int i = 0; i < drawTarget.GetLength(0); i++)
            {
                for (int j = 0; j < drawTarget.GetLength(1); j++)
                {
                    if (drawTarget[i, j])
                    {
                        int realX = _posX + i;
                        int realY = _posY + j;

                        if (realX < 0 || realY < 0 || realY - TetrisGame.k_YBuffer < 0)
                        {
                            continue;
                        }
                        else
                        {
                            buffer.SetCell(realX * 2 + _screenPosX, realY + _screenPosY - TetrisGame.k_YBuffer, '\u2588', _minoColors[(int)_shapeType]);
                            buffer.SetCell(realX * 2 + _screenPosX + 1, realY + _screenPosY - TetrisGame.k_YBuffer, '\u2588', _minoColors[(int)_shapeType]);
                        }
                    }
                }
            }

            int nextXOffset = _bag[_bagCursor] switch
            {
                ShapeType.I => 1,
                ShapeType.O => 0,
                ShapeType.Z => 2,
                ShapeType.S => 2,
                ShapeType.J => 2,
                ShapeType.L => 2,
                ShapeType.T => 2,
                _ => 0
            };
            for (int i = 0; i < nextMino.GetLength(0); i++)
            {
                for (int j = 0; j < nextMino.GetLength(1); j++)
                {
                    if (nextMino[i, j])
                    {
                        buffer.SetCell(_nextPosX + i * 2 + nextXOffset, _nextPosY + j, '\u2588', _minoColors[(int)_bag[_bagCursor]]);
                        buffer.SetCell(_nextPosX + i * 2 + 1 + nextXOffset, _nextPosY + j, '\u2588', _minoColors[(int)_bag[_bagCursor]]);
                    }
                }
            }

            int keepXOffset = _keep switch
            {
                ShapeType.I => 1,
                ShapeType.O => 0,
                ShapeType.Z => 2,
                ShapeType.S => 2,
                ShapeType.J => 2,
                ShapeType.L => 2,
                ShapeType.T => 2,
                _ => 0
            };

            if ((int)_keep != -1)
            {
                bool[,] keepMino = _minoShapes[_keep][0];

                for (int i = 0; i < keepMino.GetLength(0); i++)
                {
                    for (int j = 0; j < keepMino.GetLength(1); j++)
                    {
                        if (keepMino[i, j])
                        {
                            buffer.SetCell(_nextPosX + i * 2 + keepXOffset, _nextPosY + j+5, '\u2588', _minoColors[(int)_keep]);
                            buffer.SetCell(_nextPosX + i * 2 + 1 + keepXOffset, _nextPosY + j+5, '\u2588', _minoColors[(int)_keep]);
                        }
                    }
                }
            }
        }

        public override void Update(float deltaTime)
        {

        }

        public void Clear()
        {
            _bag = _bag.Shuffle().ToList();
            _bagCursor = 0;
            _keep = (ShapeType)(-1);
            SpawnNext();
        }

        public void Move(int xDelta, int yDelta)
        {
            var currentShape = _minoShapes[_shapeType][(int)_spin];
            for (int i = 0; i < currentShape.GetLength(0); i++)
            {
                for (int j = 0; j < currentShape.GetLength(1); j++)
                {
                    if (currentShape[i, j])
                    {
                        int checkX = _posX + i + xDelta;
                        int checkY = _posY + j + yDelta;

                        if (_board.CheckCollide(checkX, checkY))
                        {
                            if (yDelta == 1)
                            {
                                for (int i2 = 0; i2 < currentShape.GetLength(0); i2++)
                                {
                                    for (int j2 = 0; j2 < currentShape.GetLength(1); j2++)
                                    {
                                        if (currentShape[i2, j2])
                                            _board.AddBlock(_posX + i2, _posY + j2, _minoColors[(int)_shapeType]);
                                    }
                                }

                                SpawnNext();
                                _board.CheckBoard();
                                _dropFinished = true;
                                _keepUsed = false;
                                return;
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                }
            }

            _posX += xDelta;
            _posY += yDelta;
        }

        void SpawnNext()
        {
            _posX = TetrisGame.k_BoardSizeX / 2;
            _posY = 0;
            _shapeType = _bag[_bagCursor++];
            _spin = SpinType.S;

            if (_bagCursor == _bag.Count)
            {
                _bagCursor = 0;
            }
        }

        public void Spin(bool right)
        {
            SpinMoveType spinType = default;
            SpinType newSpin = default;
            switch (_spin)
            {
                case SpinType.S:
                    if (right)
                    {
                        spinType = SpinMoveType.SR;
                        newSpin = SpinType.R;
                    }
                    else
                    {
                        spinType = SpinMoveType.SL;
                        newSpin = SpinType.L;
                    }
                    break;
                case SpinType.R:
                    if (right)
                    {
                        spinType = SpinMoveType.RT;
                        newSpin = SpinType.T;
                    }
                    else
                    {
                        spinType = SpinMoveType.RS;
                        newSpin = SpinType.S;
                    }
                    break;
                case SpinType.T:
                    if (right)
                    {
                        spinType = SpinMoveType.TL;
                        newSpin = SpinType.L;
                    }
                    else
                    {
                        spinType = SpinMoveType.TR;
                        newSpin = SpinType.R;
                    }
                    break;
                case SpinType.L:
                    if (right)
                    {
                        spinType = SpinMoveType.LS;
                        newSpin = SpinType.S;
                    }
                    else
                    {
                        spinType = SpinMoveType.LT;
                        newSpin = SpinType.T;
                    }
                    break;
            }
            int newX, newY;
            for (int i = 0; i < 5; i++)
            {
                if (_shapeType == ShapeType.I)
                {
                    newX = wallKickLookupI[spinType][i].X + _posX;
                    newY = wallKickLookupI[spinType][i].Y + _posY;
                }
                else
                {
                    newX = wallKickLookupNormal[spinType][i].X + _posX;
                    newY = wallKickLookupNormal[spinType][i].Y + _posY;
                }

                bool matchFlag = true;

                for (int x = 0; matchFlag && x < _minoShapes[_shapeType][(int)newSpin].GetLength(0); x++)
                {
                    for (int y = 0; y < _minoShapes[_shapeType][(int)newSpin].GetLength(1); y++)
                    {
                        if (_minoShapes[_shapeType][(int)newSpin][x, y])
                        {
                            if (_board.CheckCollide(newX + x, newY + y))
                            {
                                debug.Append($"{newX + x},{newY + y} : {i} Failed");
                                matchFlag = false;
                                break;
                            }
                        }
                    }
                }

                if (matchFlag)
                {
                    _spin = newSpin;
                    _posX = newX;
                    _posY = newY;
                    break;
                }
            }
        }

        public void Drop()
        {
            _dropFinished = false;
            while (!_dropFinished)
            {
                Move(0, 1);
            }
        }

        public void Keep()
        {
            if (!_keepUsed)
            {
                ShapeType lastKeep = _keep;
                _keep = _shapeType;

                if(lastKeep == (ShapeType)(-1))
                {
                    SpawnNext();
                }
                else
                {
                    _shapeType = lastKeep;

                    _posX = TetrisGame.k_BoardSizeX / 2;
                    _posY = 0;                    
                    _spin = SpinType.S;
                }

                _keepUsed = true;
            }
        }
    }
}
