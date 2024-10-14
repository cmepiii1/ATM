using System.Collections.Immutable;

namespace AtmApp.Settings
{
    internal class DefaultSettings:ISettings
    {
        public ImmutableArray<(int, int)> MoneyCases => [(10, 100), (50, 100), (100, 100), (500, 100), (1000, 100), (5000, 100)];
    }
}
