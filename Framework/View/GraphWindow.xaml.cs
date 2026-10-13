using System.Windows;
using System.Windows.Media;

using Framework.ViewModel;

namespace Framework.View
{
    public partial class GraphWindow : Window
    {
        private readonly GraphVM _graphVM;

        public GraphWindow(MainVM mainVM, PointCollection points)
        {
            InitializeComponent();
            UiHelper.SetMainWindowAsOwner(this);

            _graphVM = new GraphVM();

            _graphVM.Theme = mainVM.Theme;
            _graphVM.CreateGraph(points.Clone());

            DataContext = _graphVM;
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _graphVM.Cleanup();
        }
    }
}