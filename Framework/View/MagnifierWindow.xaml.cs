using System.Windows;

using Framework.Themes;
using Framework.ViewModel;

namespace Framework.View
{
    public partial class MagnifierWindow : Window
    {
        private readonly MagnifierVM _magnifierVM;

        public MagnifierWindow()
        {
            InitializeComponent();
            UiHelper.SetMainWindowAsOwner(this);

            _magnifierVM = (MagnifierVM)DataContext;
        }

        public void SetTheme(ITheme theme)
        {
            _magnifierVM.Theme = theme;
        }

        public void Update()
        {
            _magnifierVM.RefreshMagnifierImages();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _magnifierVM.Cleanup();
        }
    }
}