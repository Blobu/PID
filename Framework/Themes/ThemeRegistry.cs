using System.Collections.Generic;
using System.Linq;

namespace Framework.Themes
{
    public static class ThemeRegistry
    {
        private static readonly Dictionary<string, ITheme> _themes =
            new Dictionary<string, ITheme>
            {
                { "Lime Green", LimeGreenTheme.Instance },
                { "Forest Green", ForestGreenTheme.Instance },
                { "Pale Pink", PalePinkTheme.Instance },
                { "Lavender Violet", LavenderVioletTheme.Instance },
                { "Cobalt Blue", CobaltBlueTheme.Instance }
            };

        public static ITheme GetTheme(string name)
        {
            return name != null && _themes.TryGetValue(name, out var theme) ? theme : null;
        }

        public static string GetName(ITheme theme)
        {
            return _themes.FirstOrDefault(pair => pair.Value == theme).Key;
        }
    }
}
