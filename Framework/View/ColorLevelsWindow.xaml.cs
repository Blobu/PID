using System.Windows;

using Framework.ViewModel;
using Framework.ViewModel.ColorLevels;

namespace Framework.View
{
    public partial class ColorLevelsWindow : Window
    {
        private readonly ColorLevelsVM _colorLevelsVM;

        public ColorLevelsWindow(MainVM mainVM, ColorLevelsType type)
        {
            InitializeComponent();
            UiHelper.SetMainWindowAsOwner(this);

            _colorLevelsVM = ColorLevelsFactory.Produce(type);
            _colorLevelsVM.Theme = mainVM.Theme;
            DataContext = _colorLevelsVM;

            Update();
        }

        public void Update()
        {
            _colorLevelsVM.UpdateSelectedPoint();
        }

        public void RefreshImages()
        {
            _colorLevelsVM.RefreshPlots();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _colorLevelsVM.Cleanup();
        }
    }
}
