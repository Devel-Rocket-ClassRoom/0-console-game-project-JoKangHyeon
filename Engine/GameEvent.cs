namespace Framework.Engine
{
    public delegate void GameAction();
    public delegate void GameAction<T>(T value);
    public delegate void GameAction<TInput1,TInput2>(TInput1 value1, TInput2 value2);
    public delegate void GameAction<TInput1, TInput2, TInput3>(TInput1 value1, TInput2 value2, TInput3 value3);
}
