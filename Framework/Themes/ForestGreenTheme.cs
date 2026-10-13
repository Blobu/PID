namespace Framework.Themes
{
    internal class ForestGreenTheme : ITheme
    {
        private ForestGreenTheme() { }

        public static ForestGreenTheme Instance { get; } = new ForestGreenTheme();

        public string MenuBackgroundColor => "#062923";

        public string MenuItemBackgroundColor => "#11423C";

        public string MenuItemForegroundColor => "#F0F8FF";

        public string BackgroundColor => "#011A17";

        public string CanvasBackgroundColor => "#062923";

        public string CanvasBorderBrush => "#4F8E85";

        public string TextForeground => "#F1F1F1";

        public string ButtonBackgroundColor => "#11423C";

        public string ButtonForegroundColor => "#F0F8FF";

        public string ButtonBorderBrush => "#4F8E85";
    }
}