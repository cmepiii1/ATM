namespace AtmApp.Result
{
    internal class RunResult
    {
        public bool Success { get; init; }
        public Action<bool>? Continuer { get; init; }
        public string? Message { get; init; }
    }
}
