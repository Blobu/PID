using Framework.Themes;

namespace Framework.Messaging
{
    public class ThemeChangedMessage
    {
        public ITheme Theme { get; }

        public ThemeChangedMessage(ITheme theme)
        {
            Theme = theme;
        }
    }
}