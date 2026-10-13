namespace Framework.Themes
{
    internal class CobaltBlueTheme : ITheme
    {
        private CobaltBlueTheme() { }

        public static CobaltBlueTheme Instance { get; } = new CobaltBlueTheme();

        public string MenuBackgroundColor => "#24272B";

        public string MenuItemBackgroundColor => "#0049A3";

        public string MenuItemForegroundColor => "#F0F8FF";

        public string BackgroundColor => "#4A525A";

        public string CanvasBackgroundColor => "#0049A3";

        public string CanvasBorderBrush => "#24272B";

        public string TextForeground => "#F1F1F1";

        public string ButtonBackgroundColor => "#0049A3";

        public string ButtonForegroundColor => "#F0F8FF";

        public string ButtonBorderBrush => "#07070A";
    }
}