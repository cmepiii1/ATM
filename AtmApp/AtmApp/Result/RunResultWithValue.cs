namespace AtmApp.Result
{
    internal class RunResultWithValue<T> : RunResult
    {
        public T? Value { get; init; }
    }
}
