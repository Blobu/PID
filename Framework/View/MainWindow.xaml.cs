using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using Framework.Messaging;
using Framework.Model;
using Framework.Themes;
using Framework.ViewModel;

using static Framework.Utilities.DrawingHelper;
using static Framework.View.UiHelper;

namespace Framework.View
{
    public partial class MainWindow : Window
    {
        private readonly MainVM _mainVM;
        private readonly IMessageBus _messageBus;

        public MainWindow()
        {
            InitializeComponent();

            _messageBus = MessageBus.Instance;
            _mainVM = (MainVM)DataContext;

            _messageBus.Subscribe<RefreshOverlaysMessage>(this, _ => Update());
            Closed += (s, e) => _messageBus.Unsubscribe<RefreshOverlaysMessage>(this);

            UpdateThemeMenu();
        }

        #region Selection markers
        public void Update()
        {
            double scaleValue = sliderZoom.Value;

            RemoveUiElements(initialImageCanvas, processedImageCanvas);
            DrawUiElements(initialImageCanvas, processedImageCanvas, scaleValue, _mainVM);
        }
        #endregion

        #region Mouse input
        private void ImageMouseMove(object sender, MouseEventArgs e)
        {
            var canvasType = sender == initialImageCanvas ? ImageCanvasType.Initial : ImageCanvasType.Processed;
            // Mouse position is in canvas coordinates which may be scaled by LayoutTransform; convert to image (unscaled) coords
            Point pos = e.GetPosition((IInputElement)sender);
            double scale = sliderZoom.Value;
            var unscaled = new Point(pos.X / scale, pos.Y / scale);
            _messageBus.Publish(new MouseMoveMessage(unscaled, canvasType));
        }

        private void ImageMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition((IInputElement)sender);
            double scale = sliderZoom.Value;
            var unscaled = new Point(pos.X / scale, pos.Y / scale);
            _mainVM.SelectPoint(unscaled);
        }

        private void ImageMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _mainVM.ClearSelectedPoints();
        }
        #endregion

        #region Zoom and scrolling
        private void SliderZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_mainVM == null) return;

            double scaleValue = sliderZoom.Value;

            UpdateShapesProperties(initialImageCanvas, scaleValue);
            UpdateShapesProperties(processedImageCanvas, scaleValue);

            _mainVM.InitialCanvasWidth = initialImage.ActualWidth * scaleValue;
            _mainVM.InitialCanvasHeight = initialImage.ActualHeight * scaleValue;
            _mainVM.ProcessedCanvasWidth = processedImage.ActualWidth * scaleValue;
            _mainVM.ProcessedCanvasHeight = processedImage.ActualHeight * scaleValue;
        }

        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender == initialImageCanvasScrollViewer)
            {
                processedImageCanvasScrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
                processedImageCanvasScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
            }
            else
            {
                initialImageCanvasScrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
                initialImageCanvasScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
            }
        }
        #endregion

        #region Theme
        private void UpdateThemeMenu()
        {
            string themeName = ThemeRegistry.GetName(_mainVM.Theme);
            foreach (MenuItem item in themeMenu.Items)
                item.IsChecked = item.Header.ToString() == themeName;
        }

        private void ThemeModeSelector(object sender, RoutedEventArgs e)
        {
            string selectedTheme = ((MenuItem)sender).Header.ToString();
            _mainVM.SetThemeMode(selectedTheme);
            UpdateThemeMenu();

            Properties.Settings.Default.Theme = selectedTheme;
            Properties.Settings.Default.Save();
        }
        #endregion
    }
}