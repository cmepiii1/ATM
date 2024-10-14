using System.Collections.Immutable;

namespace AtmApp.Settings
{
    internal interface ISettings
    {
        ImmutableArray<(int value, int ammount)> MoneyCases { get; }
        int ValuesCount => MoneyCases.Length;
    }
}
