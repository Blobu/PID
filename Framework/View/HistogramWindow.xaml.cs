using System.Windows;

using Framework.Model;
using Framework.ViewModel;

namespace Framework.View
{
    public partial class HistogramWindow : Window
    {
        private readonly HistogramVM _histogramVM;

        public HistogramWindow(MainVM mainVM, ImageCanvasType canvas)
            : this(mainVM)
        {
            ShowHistogram(canvas);
        }

        public HistogramWindow(MainVM mainVM, string title, dynamic values)
            : this(mainVM)
        {
            _histogramVM.Title = title;

            if (values != null)
                _histogramVM.CreateHistogram(values);
        }

        private HistogramWindow(MainVM mainVM)
        {
            InitializeComponent();
            UiHelper.SetMainWindowAsOwner(this);

            _histogramVM = new HistogramVM();
            _histogramVM.Theme = mainVM.Theme;

            DataContext = _histogramVM;
        }

        public void ShowHistogram(ImageCanvasType canvas)
        {
            _histogramVM.CreateHistogram(canvas);
        }

        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Width == 0 || e.PreviousSize == e.NewSize)
                return;

            Left += (e.PreviousSize.Width - e.NewSize.Width) / 2;
            Top += (e.PreviousSize.Height - e.NewSize.Height) / 2;
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _histogramVM.Cleanup();
        }
    }
}