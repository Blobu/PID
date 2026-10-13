using Framework.Messaging;
using Framework.Model;
using Framework.View;
using Framework.ViewModel;
using Framework.ViewModel.ColorLevels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Framework.Services
{
    public class WindowService : IWindowService
    {
        private readonly IMessageBus _messageBus;
        private readonly IImageStore _imageStore;
        private readonly MainVM _mainVM;

        private MagnifierWindow _magnifierWindow;
        private readonly Dictionary<ColorLevelsType, ColorLevelsWindow> _colorLevelsWindows =
            new Dictionary<ColorLevelsType, ColorLevelsWindow>();
        private readonly Dictionary<ImageCanvasType, HistogramWindow> _histogramWindows =
            new Dictionary<ImageCanvasType, HistogramWindow>();
        private bool _isRefreshPending;

        public WindowService(IMessageBus messageBus, IImageStore imageStore, MainVM mainVM)
        {
            _messageBus = messageBus;
            _imageStore = imageStore;
            _mainVM = mainVM;

            _imageStore.ImageChanged += OnImageChanged;
        }

        #region Magnifier
        public bool IsMagnifierOpen => _magnifierWindow != null;

        public void ShowMagnifier()
        {
            if (IsMagnifierOpen)
            {
                UiHelper.BringToFront(_magnifierWindow);
                return;
            }

            var window = new MagnifierWindow();
            window.SetTheme(_mainVM.Theme);
            window.Update();
            _magnifierWindow = window;
            window.Closed += (s, e) =>
            {
                _magnifierWindow = null;
                RequestMainOverlayRefresh();
            };
            window.Show();
            RequestMainOverlayRefresh();
        }
        #endregion

        #region Color levels
        public bool IsRowColorLevelsOpen => _colorLevelsWindows.ContainsKey(ColorLevelsType.Row);
        public bool IsColumnColorLevelsOpen => _colorLevelsWindows.ContainsKey(ColorLevelsType.Column);

        public void ShowColorLevels(ColorLevelsType type)
        {
            if (_colorLevelsWindows.TryGetValue(type, out var openWindow))
            {
                UiHelper.BringToFront(openWindow);
                return;
            }

            var window = new ColorLevelsWindow(_mainVM, type);
            _colorLevelsWindows[type] = window;
            window.Closed += (s, e) =>
            {
                _colorLevelsWindows.Remove(type);
                RequestMainOverlayRefresh();
            };
            window.Show();
            RequestMainOverlayRefresh();
        }
        #endregion

        #region Histograms
        public void ShowHistogram(ImageCanvasType canvas)
        {
            if (_histogramWindows.TryGetValue(canvas, out var openWindow))
            {
                openWindow.ShowHistogram(canvas);
                UiHelper.BringToFront(openWindow);
                return;
            }

            var window = new HistogramWindow(_mainVM, canvas);
            _histogramWindows[canvas] = window;
            window.Closed += (s, e) => RemoveHistogramWindow(window);
            window.Show();
        }

        private void RemoveHistogramWindow(HistogramWindow window)
        {
            foreach (var pair in _histogramWindows)
            {
                if (pair.Value == window)
                {
                    _histogramWindows.Remove(pair.Key);
                    return;
                }
            }
        }

        private void RefreshHistograms()
        {
            foreach (var pair in _histogramWindows.ToList())
            {
                if (_imageStore.GetImage(pair.Key) != null)
                    pair.Value.ShowHistogram(pair.Key);
                else
                    pair.Value.Close();
            }
        }
        #endregion

        #region Image changes
        private void OnImageChanged(object sender, ImageCanvasType canvas)
        {
            if (_imageStore.GetImage(canvas) == null)
            {
                foreach (var window in Application.Current.Windows.OfType<Window>().ToList())
                {
                    if (DependsOnClearedImage(window, canvas))
                        window.Close();
                }
            }

            if (_isRefreshPending)
                return;

            _isRefreshPending = true;
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(RefreshChildWindows));
        }

        private bool DependsOnClearedImage(Window window, ImageCanvasType clearedCanvas)
        {
            if (window is SliderWindow || window is GraphWindow)
                return clearedCanvas == ImageCanvasType.Initial;

            if (window is HistogramWindow histogramWindow && !_histogramWindows.ContainsValue(histogramWindow))
                return clearedCanvas == ImageCanvasType.Processed;

            return false;
        }

        private void RefreshChildWindows()
        {
            _isRefreshPending = false;

            _magnifierWindow?.Update();
            foreach (var window in _colorLevelsWindows.Values)
                window.RefreshImages();
            RefreshHistograms();
            RequestMainOverlayRefresh();
        }
        #endregion

        #region Child windows
        public void UpdateSelectedPoint()
        {
            _magnifierWindow?.Update();
            foreach (var window in _colorLevelsWindows.Values)
                window.Update();
        }

        public void CloseAllChildWindows()
        {
            foreach (var window in Application.Current.Windows.OfType<Window>().ToList())
            {
                if (window != Application.Current.MainWindow && !(window is DialogWindow))
                    window.Close();
            }
        }

        private void RequestMainOverlayRefresh()
        {
            _messageBus.Publish(new RefreshOverlaysMessage());
        }
        #endregion
    }
}
