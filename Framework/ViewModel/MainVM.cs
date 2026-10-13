using Emgu.CV;
using Emgu.CV.Structure;
using Framework.Messaging;
using Framework.Model;
using Framework.Services;
using Framework.Themes;
using Framework.Utilities;
using Framework.ViewModel.Commands;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Framework.ViewModel
{
    public class MainVM : BaseVM
    {
        private readonly IMessageBus _messageBus;
        private readonly IImageStore _imageStore;
        private readonly IWindowService _windowService;
        private bool _isStartOverPending;

        public MenuCommands MenuCommands { get; }

        public MainVM()
            : this(MessageBus.Instance, ImageStore.Instance, null)
        {
        }

        public MainVM(IMessageBus messageBus, IImageStore imageStore, IWindowService windowService)
        {
            _messageBus = messageBus;
            _imageStore = imageStore;
            _windowService = windowService ?? new WindowService(messageBus, imageStore, this);

            MenuCommands = new MenuCommands(this, _imageStore, _windowService);
            ResetZoomCommand = new RelayCommand(_ => ScaleValue = 1);
            ScaleValue = 1;

            string theme = Properties.Settings.Default.Theme;
            SetThemeMode(theme);

            _messageBus.Subscribe<MouseMoveMessage>(this, OnMouseMove);
            _imageStore.ImageChanged += OnImageChanged;
        }

        #region Mouse input
        private void OnMouseMove(MouseMoveMessage message)
        {
            var pos = message.Position;
            int x = (int)pos.X;
            int y = (int)pos.Y;

            XPos = x >= 0 ? "X: " + x : "";
            YPos = y >= 0 ? "Y: " + y : "";

            var image = _imageStore.GetImage(message.CanvasType);
            var grayImage = image as Image<Gray, byte>;
            var colorImage = image as Image<Bgr, byte>;

            bool onGray = grayImage != null && x >= 0 && y >= 0 && x < grayImage.Width && y < grayImage.Height;
            bool onColor = colorImage != null && x >= 0 && y >= 0 && x < colorImage.Width && y < colorImage.Height;

            GrayValue = onGray ? "Gray: " + grayImage.Data[y, x, 0] : "";
            BlueValue = onColor ? "B: " + colorImage.Data[y, x, 0] : "";
            GreenValue = onColor ? "G: " + colorImage.Data[y, x, 1] : "";
            RedValue = onColor ? "R: " + colorImage.Data[y, x, 2] : "";
        }

        public void SelectPoint(Point position)
        {
            var clicks = _imageStore.MouseClickCollection;
            if (clicks.Count == 0 || clicks[clicks.Count - 1] != position)
                clicks.Add(position);
            _imageStore.LastMouseClick = position;

            _windowService.UpdateSelectedPoint();
            _messageBus.Publish(new RefreshOverlaysMessage());
        }

        public void ClearSelectedPoints()
        {
            _imageStore.MouseClickCollection.Clear();
        }
        #endregion

        #region Selection markers
        public Point SelectedPoint => _imageStore.LastMouseClick;
        public bool IsSelectedAreaMarked => _windowService.IsMagnifierOpen;
        public bool IsSelectedRowMarked => _windowService.IsMagnifierOpen || _windowService.IsRowColorLevelsOpen;
        public bool IsSelectedColumnMarked => _windowService.IsMagnifierOpen || _windowService.IsColumnColorLevelsOpen;

        public Size? GetImageSize(ImageCanvasType canvas)
        {
            var image = _imageStore.GetImage(canvas);
            return image != null ? new Size(image.Size.Width, image.Size.Height) : (Size?)null;
        }
        #endregion

        #region Image changes
        private void OnImageChanged(object sender, ImageCanvasType canvas)
        {
            ImageSource image = ToImageSource(_imageStore.GetImage(canvas));
            if (canvas == ImageCanvasType.Initial)
                InitialImage = image;
            else
                ProcessedImage = image;

            if (!_imageStore.HasInitialImage && !_imageStore.HasProcessedImage && !_isStartOverPending)
            {
                _isStartOverPending = true;
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(StartOverIfEmpty));
            }
        }

        private void StartOverIfEmpty()
        {
            _isStartOverPending = false;
            if (_imageStore.HasInitialImage || _imageStore.HasProcessedImage)
                return;

            _windowService.CloseAllChildWindows();
            _imageStore.MouseClickCollection.Clear();
            XPos = "";
            YPos = "";
            GrayValue = "";
            RedValue = "";
            GreenValue = "";
            BlueValue = "";
            ScaleValue = 1;
        }

        private static ImageSource ToImageSource(IImage image)
        {
            if (image is Image<Gray, byte> grayImage)
                return ImageConverter.Convert(grayImage);
            if (image is Image<Bgr, byte> colorImage)
                return ImageConverter.Convert(colorImage);
            return null;
        }
        #endregion

        #region Theme
        public void SetThemeMode(string theme)
        {
            var resolved = ThemeRegistry.GetTheme(theme);
            if (resolved != null)
                Theme = resolved;
            else if (Theme == null)
                Theme = LimeGreenTheme.Instance;

            _messageBus.Publish(new ThemeChangedMessage(Theme));
        }

        private ITheme _theme;
        public ITheme Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                NotifyPropertyChanged(nameof(Theme));
            }
        }
        #endregion

        #region Image properties
        private ImageSource _initialImage;
        public ImageSource InitialImage
        {
            get => _initialImage;
            set
            {
                _initialImage = value;
                InitialCanvasWidth = (value?.Width ?? 0) * ScaleValue;
                InitialCanvasHeight = (value?.Height ?? 0) * ScaleValue;
                NotifyPropertyChanged(nameof(InitialImage));
            }
        }

        private ImageSource _processedImage;
        public ImageSource ProcessedImage
        {
            get => _processedImage;
            set
            {
                _processedImage = value;
                ProcessedCanvasWidth = (value?.Width ?? 0) * ScaleValue;
                ProcessedCanvasHeight = (value?.Height ?? 0) * ScaleValue;
                NotifyPropertyChanged(nameof(ProcessedImage));
            }
        }
        #endregion

        #region Canvases properties
        private double _initialCanvasWidth;
        public double InitialCanvasWidth
        {
            get => _initialCanvasWidth;
            set
            {
                _initialCanvasWidth = value;
                NotifyPropertyChanged(nameof(InitialCanvasWidth));
            }
        }

        private double _initialCanvasHeight;
        public double InitialCanvasHeight
        {
            get => _initialCanvasHeight;
            set
            {
                _initialCanvasHeight = value;
                NotifyPropertyChanged(nameof(InitialCanvasHeight));
            }
        }

        private double _processedCanvasWidth;
        public double ProcessedCanvasWidth
        {
            get => _processedCanvasWidth;
            set
            {
                _processedCanvasWidth = value;
                NotifyPropertyChanged(nameof(ProcessedCanvasWidth));
            }
        }

        private double _processedCanvasHeight;
        public double ProcessedCanvasHeight
        {
            get => _processedCanvasHeight;
            set
            {
                _processedCanvasHeight = value;
                NotifyPropertyChanged(nameof(ProcessedCanvasHeight));
            }
        }
        #endregion

        #region Pixel properties
        private string _xPos;
        public string XPos
        {
            get => _xPos;
            set { _xPos = value; NotifyPropertyChanged(nameof(XPos)); }
        }

        private string _yPos;
        public string YPos
        {
            get => _yPos;
            set { _yPos = value; NotifyPropertyChanged(nameof(YPos)); }
        }

        private string _grayValue;
        public string GrayValue
        {
            get => _grayValue;
            set { _grayValue = value; NotifyPropertyChanged(nameof(GrayValue)); }
        }

        private string _redValue;
        public string RedValue
        {
            get => _redValue;
            set { _redValue = value; NotifyPropertyChanged(nameof(RedValue)); }
        }

        private string _greenValue;
        public string GreenValue
        {
            get => _greenValue;
            set { _greenValue = value; NotifyPropertyChanged(nameof(GreenValue)); }
        }

        private string _blueValue;
        public string BlueValue
        {
            get => _blueValue;
            set { _blueValue = value; NotifyPropertyChanged(nameof(BlueValue)); }
        }
        #endregion

        #region Scale property and reset command
        private double _scaleValue;
        public double ScaleValue
        {
            get => _scaleValue;
            set
            {
                _scaleValue = value;
                NotifyPropertyChanged(nameof(ScaleValue));
            }
        }

        public ICommand ResetZoomCommand { get; }
        #endregion
    }
}