namespace Framework.Themes
{
    internal class LavenderVioletTheme : ITheme
    {
        private LavenderVioletTheme() { }

        public static LavenderVioletTheme Instance { get; } = new LavenderVioletTheme();

        public string MenuBackgroundColor => "#5E548E";

        public string MenuItemBackgroundColor => "#BE95C4";

        public string MenuItemForegroundColor => "#1E1E1E";

        public string BackgroundColor => "#E0B1CB";

        public string CanvasBackgroundColor => "#BE95C4";

        public string CanvasBorderBrush => "#231942";

        public string TextForeground => "#1E1E1E";

        public string ButtonBackgroundColor => "#BE95C4";

        public string ButtonForegroundColor => "#1E1E1E";

        public string ButtonBorderBrush => "#9F86C0";
    }
}