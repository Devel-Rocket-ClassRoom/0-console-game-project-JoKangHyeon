namespace Framework.Engine
{
    public delegate void GameAction();
    public delegate void GameAction<T>(T value);
    public delegate void GameAction<T1,T2>(T1 value1, T2 value2);
}
