using Emgu.CV;
using Emgu.CV.Structure;

using System;
using System.Windows;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using PointCollection = System.Windows.Media.PointCollection;

using Framework.View;
using Framework.Model;
using Framework.Services;
using Framework.ViewModel.ColorLevels;
using static Framework.Utilities.DrawingHelper;
using static Framework.Utilities.FileHelper;

using Algorithms.Sections;
using Algorithms.Tools;
using Algorithms.Utilities;

namespace Framework.ViewModel.Commands
{
    public class MenuCommands : BaseVM
    {
        private readonly MainVM _mainVM;
        private readonly IImageStore _imageStore;
        private readonly IWindowService _windowService;

        public MenuCommands(MainVM mainVM, IImageStore imageStore, IWindowService windowService)
        {
            _mainVM = mainVM;
            _imageStore = imageStore;
            _windowService = windowService;
        }

        #region Properties
        private Image<Gray, byte> GrayInitialImage
        {
            get => _imageStore.GrayInitialImage;
            set => _imageStore.GrayInitialImage = value;
        }

        private Image<Gray, byte> GrayProcessedImage
        {
            get => _imageStore.GrayProcessedImage;
            set => _imageStore.GrayProcessedImage = value;
        }

        private Image<Bgr, byte> ColorInitialImage
        {
            get => _imageStore.ColorInitialImage;
            set => _imageStore.ColorInitialImage = value;
        }

        private Image<Bgr, byte> ColorProcessedImage
        {
            get => _imageStore.ColorProcessedImage;
            set => _imageStore.ColorProcessedImage = value;
        }

        private PointCollection MouseClickCollection
        {
            get => _imageStore.MouseClickCollection;
        }

        private Point LastMouseClick
        {
            get => _imageStore.LastMouseClick;
        }

        private double ScaleValue
        {
            get => _mainVM.ScaleValue;
            set => _mainVM.ScaleValue = value;
        }
        #endregion

        #region File

        #region Load grayscale image
        private RelayCommand _loadGrayscaleImageCommand;
        public RelayCommand LoadGrayscaleImageCommand
        {
            get
            {
                if (_loadGrayscaleImageCommand == null)
                    _loadGrayscaleImageCommand = new RelayCommand(LoadGrayscaleImage);
                return _loadGrayscaleImageCommand;
            }
        }

        private void LoadGrayscaleImage(object parameter)
        {
            var image = AskForImage<Gray>("Select a grayscale picture");
            if (image != null)
            {
                Clear(parameter);
                GrayInitialImage = image;
            }
        }
        #endregion

        #region Load color image
        private ICommand _loadColorImageCommand;
        public ICommand LoadColorImageCommand
        {
            get
            {
                if (_loadColorImageCommand == null)
                    _loadColorImageCommand = new RelayCommand(LoadColorImage);
                return _loadColorImageCommand;
            }
        }

        private void LoadColorImage(object parameter)
        {
            var image = AskForImage<Bgr>("Select a color picture");
            if (image != null)
            {
                Clear(parameter);
                ColorInitialImage = image;
            }
        }
        #endregion

        #region Save processed image
        private ICommand _saveProcessedImageCommand;
        public ICommand SaveProcessedImageCommand
        {
            get
            {
                if (_saveProcessedImageCommand == null)
                    _saveProcessedImageCommand = new RelayCommand(SaveProcessedImage);
                return _saveProcessedImageCommand;
            }
        }

        private void SaveProcessedImage(object parameter)
        {
            if (GrayProcessedImage == null && ColorProcessedImage == null)
            {
                MessageBox.Show("If you want to save your processed image, " +
                    "please load and process an image first!");
                return;
            }

            string imagePath = SaveFileDialog("image.jpg");
            if (imagePath != null)
            {
                if (GrayProcessedImage != null)
                    SaveImage(GrayProcessedImage, imagePath);
                else
                    SaveImage(ColorProcessedImage, imagePath);
                Process.Start(imagePath);
            }
        }
        #endregion

        #region Exit
        private ICommand _exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                    _exitCommand = new RelayCommand(Exit);
                return _exitCommand;
            }
        }

        private void Exit(object parameter)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #endregion

        #region Edit

        #region Remove drawn shapes
        private ICommand _removeDrawnShapesCommand;
        public ICommand RemoveDrawnShapesCommand
        {
            get
            {
                if (_removeDrawnShapesCommand == null)
                    _removeDrawnShapesCommand = new RelayCommand(RemoveDrawnShapes);
                return _removeDrawnShapesCommand;
            }
        }

        private void RemoveDrawnShapes(object parameter)
        {
            foreach (var canvas in parameter as object[] ?? new[] { parameter })
                RemoveUiElements(canvas as Canvas);
        }
        #endregion

        #region Clear initial canvas
        private ICommand _clearInitialCanvasCommand;
        public ICommand ClearInitialCanvasCommand
        {
            get
            {
                if (_clearInitialCanvasCommand == null)
                    _clearInitialCanvasCommand = new RelayCommand(ClearInitialCanvas);
                return _clearInitialCanvasCommand;
            }
        }

        private void ClearInitialCanvas(object parameter)
        {
            RemoveUiElements(parameter as Canvas);

            _imageStore.ClearInitial();
        }
        #endregion

        #region Clear processed canvas
        private ICommand _clearProcessedCanvasCommand;
        public ICommand ClearProcessedCanvasCommand
        {
            get
            {
                if (_clearProcessedCanvasCommand == null)
                    _clearProcessedCanvasCommand = new RelayCommand(ClearProcessedCanvas);
                return _clearProcessedCanvasCommand;
            }
        }

        private void ClearProcessedCanvas(object parameter)
        {
            RemoveUiElements(parameter as Canvas);

            _imageStore.ClearProcessed();
        }
        #endregion

        #region Closing all open windows and clear both canvases
        private ICommand _clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                    _clearCommand = new RelayCommand(Clear);
                return _clearCommand;
            }
        }

        private void Clear(object parameter)
        {
            _windowService.CloseAllChildWindows();

            ScaleValue = 1;

            var canvases = (object[])parameter;
            ClearInitialCanvas(canvases[0]);
            ClearProcessedCanvas(canvases[1]);
        }
        #endregion

        #endregion

        #region Tools

        #region Magnifier
        private ICommand _magnifierCommand;
        public ICommand MagnifierCommand
        {
            get
            {
                if (_magnifierCommand == null)
                    _magnifierCommand = new RelayCommand(Magnifier);
                return _magnifierCommand;
            }
        }

        private void Magnifier(object parameter)
        {
            if (_windowService.IsMagnifierOpen || IsAreaSelected())
                _windowService.ShowMagnifier();
        }
        #endregion

        #region Visualize color levels

        #region Row color levels
        private ICommand _rowColorLevelsCommand;
        public ICommand RowColorLevelsCommand
        {
            get
            {
                if (_rowColorLevelsCommand == null)
                    _rowColorLevelsCommand = new RelayCommand(RowColorLevels);
                return _rowColorLevelsCommand;
            }
        }

        private void RowColorLevels(object parameter)
        {
            if (_windowService.IsRowColorLevelsOpen || IsAreaSelected())
                _windowService.ShowColorLevels(ColorLevelsType.Row);
        }
        #endregion

        #region Column color levels
        private ICommand _columnColorLevelsCommand;
        public ICommand ColumnColorLevelsCommand
        {
            get
            {
                if (_columnColorLevelsCommand == null)
                    _columnColorLevelsCommand = new RelayCommand(ColumnColorLevels);
                return _columnColorLevelsCommand;
            }
        }

        private void ColumnColorLevels(object parameter)
        {
            if (_windowService.IsColumnColorLevelsOpen || IsAreaSelected())
                _windowService.ShowColorLevels(ColorLevelsType.Column);
        }
        #endregion

        #endregion

        #region Visualize image histogram

        #region Initial image histogram
        private ICommand _histogramInitialImageCommand;
        public ICommand HistogramInitialImageCommand
        {
            get
            {
                if (_histogramInitialImageCommand == null)
                    _histogramInitialImageCommand = new RelayCommand(HistogramInitialImage);
                return _histogramInitialImageCommand;
            }
        }

        private void HistogramInitialImage(object parameter)
        {
            if (!IsImageLoaded())
                return;

            _windowService.ShowHistogram(ImageCanvasType.Initial);
        }
        #endregion

        #region Processed image histogram
        private ICommand _histogramProcessedImageCommand;
        public ICommand HistogramProcessedImageCommand
        {
            get
            {
                if (_histogramProcessedImageCommand == null)
                    _histogramProcessedImageCommand = new RelayCommand(HistogramProcessedImage);
                return _histogramProcessedImageCommand;
            }
        }

        private void HistogramProcessedImage(object parameter)
        {
            if (!IsImageProcessed())
                return;

            _windowService.ShowHistogram(ImageCanvasType.Processed);
        }
        #endregion

        #endregion

        #region Copy image
        private ICommand _copyImageCommand;
        public ICommand CopyImageCommand
        {
            get
            {
                if (_copyImageCommand == null)
                    _copyImageCommand = new RelayCommand(CopyImage);
                return _copyImageCommand;
            }
        }

        private void CopyImage(object parameter)
        {
            if (!IsImageLoaded())
                return;

            ClearProcessedCanvas(parameter);

            if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Copy(ColorInitialImage);
            }
            else if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Copy(GrayInitialImage);
            }
        }
        #endregion

        #region Invert image
        private ICommand _invertImageCommand;
        public ICommand InvertImageCommand
        {
            get
            {
                if (_invertImageCommand == null)
                    _invertImageCommand = new RelayCommand(InvertImage);
                return _invertImageCommand;
            }
        }

        private void InvertImage(object parameter)
        {
            if (!IsImageLoaded())
                return;

            ClearProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Invert(GrayInitialImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Invert(ColorInitialImage);
            }
        }
        #endregion

        #region Binary
        private ICommand _binaryImageCommand;
        public ICommand BinaryImageCommand
        {
            get
            {
                if (_binaryImageCommand == null)
                    _binaryImageCommand = new RelayCommand(BinaryImage);
                return _binaryImageCommand;
            }
        }
        private void BinaryImage(object parameter)
        {
            if (!IsImageLoaded())
                return;

            List<string> labels = new List<string>
            {
            "Threshold",
            };
            DialogWindow window = new DialogWindow(_mainVM, labels);
            window.ShowDialog();
            List<double> values = window.GetValues();
            if (values == null)
                return;
            byte threshold = (byte)values[0];

            ClearProcessedCanvas(parameter);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Binary(GrayInitialImage, threshold);
            }
            else if (ColorInitialImage != null)
            {
                var GrayImage = Tools.Convert(ColorInitialImage);
                GrayProcessedImage = Tools.Binary(GrayImage, threshold);
            }
        }
        #endregion

        #region Mirror
        private ICommand _mirrorImageCommand;
        public ICommand MirrorImageCommand
        {
            get
            {
                if (_mirrorImageCommand == null)
                    _mirrorImageCommand = new RelayCommand(MirrorImage);
                return _mirrorImageCommand;
            }
        }

        private void MirrorImage(object parameter)
        {
            if (!IsImageLoaded())
                return;
            ClearProcessedCanvas(parameter);
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Mirror(GrayInitialImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Mirror(ColorInitialImage);
            }
        }
        #endregion

        #region Rotate
        private ICommand _rotateImageCommand;
        public ICommand RotateImageCommand
        {
            get
            {
                if (_rotateImageCommand == null)
                    _rotateImageCommand = new RelayCommand(RotateImage);
                return _rotateImageCommand;
            }
        }

        private void RotateImage(object parameter)
        {
            if (!IsImageLoaded())
                return;
            List<string> labels = new List<string>
            {
                "Direction: 0 for clockwise, 1 for counter-clockwise",
            };
            DialogWindow window = new DialogWindow(_mainVM, labels);
            window.ShowDialog();
            byte direction = (byte)window.GetValues()[0];

            ClearProcessedCanvas(parameter);
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Rotate(GrayInitialImage, direction);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Rotate(ColorInitialImage, direction);
            }
        }


        #endregion


        #region Convert color image to grayscale image
        private ICommand _convertImageToGrayscaleCommand;
        public ICommand ConvertImageToGrayscaleCommand
        {
            get
            {
                if (_convertImageToGrayscaleCommand == null)
                    _convertImageToGrayscaleCommand = new RelayCommand(ConvertImageToGrayscale);
                return _convertImageToGrayscaleCommand;
            }
        }

        private void ConvertImageToGrayscale(object parameter)
        {
            if (!IsImageLoaded())
                return;

            if (GrayInitialImage != null)
            {
                MessageBox.Show("It is possible to convert only color images!");
                return;
            }

            ClearProcessedCanvas(parameter);

            GrayProcessedImage = Tools.Convert(ColorInitialImage);
        }
        #endregion

        #endregion

        #region Pointwise operations

        #region Contrast & Brightness
        private ICommand _contrastBrightnessImageCommand;
        public ICommand ContrastBrightnessImageCommand
        {
            get
            {
                if (_contrastBrightnessImageCommand == null)
                    _contrastBrightnessImageCommand = new RelayCommand(ContrastBrightnessImage);
                return _contrastBrightnessImageCommand;
            }
        }
        private void ContrastBrightnessImage(object parameter)
        {
            if (!IsImageLoaded())
                return;
            List<string> labels = new List<string>
            {
                "Contrast (a):",
                "Brightness (b):"
            };
            DialogWindow window = new DialogWindow(_mainVM, labels);
            window.ShowDialog();
            List<double> values = window.GetValues();
            if (values == null)
                return;
            double a = values[0];
            double b = values[1];
            ClearProcessedCanvas(parameter);
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = PointwiseOperations.ApplyLUT(GrayInitialImage,PointwiseOperations.ContrastAndBrightness(a, b));
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = PointwiseOperations.ApplyLUT(ColorInitialImage, PointwiseOperations.ContrastAndBrightness(a, b));
            }
        }



        private ICommand _contrastBrightnessGammaImageCommand;
        public ICommand ContrastBrightnessGammaImageCommand
        {
            get
            {
                if (_contrastBrightnessGammaImageCommand == null)
                    _contrastBrightnessGammaImageCommand = new RelayCommand(ContrastBrightnessGammaImage);
                return _contrastBrightnessGammaImageCommand;
            }
        }

        private void ContrastBrightnessGammaImage(object parameter)
        {
            if (!IsImageLoaded())
                return;
            List<string> labels = new List<string>
            {
                "Gamma:"
            };
            DialogWindow window = new DialogWindow(_mainVM, labels);
            window.ShowDialog();
            List<double> values = window.GetValues();
            if (values == null)
                return;
            double gamma = values[0];
            ClearProcessedCanvas(parameter);
            if (GrayInitialImage != null)
            {
                GrayProcessedImage = PointwiseOperations.ApplyLUT(GrayInitialImage, PointwiseOperations.ContrastAndBrightnessGammaOperator(gamma));
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = PointwiseOperations.ApplyLUT(ColorInitialImage, PointwiseOperations.ContrastAndBrightnessGammaOperator(gamma));
            }
        }


        #endregion

        #endregion

        #region Thresholding
        #endregion

        #region Filters
        #endregion

        #region Morphological operations
        #endregion

        #region Geometric transformations
        #endregion

        #region Segmentation
        #endregion

        #region Use processed image as initial image
        private ICommand _useProcessedImageAsInitialImageCommand;
        public ICommand UseProcessedImageAsInitialImageCommand
        {
            get
            {
                if (_useProcessedImageAsInitialImageCommand == null)
                    _useProcessedImageAsInitialImageCommand = new RelayCommand(UseProcessedImageAsInitialImage);
                return _useProcessedImageAsInitialImageCommand;
            }
        }

        private void UseProcessedImageAsInitialImage(object parameter)
        {
            if (!IsImageProcessed())
                return;

            var canvases = (object[])parameter;

            ClearInitialCanvas(canvases[0]);

            if (GrayProcessedImage != null)
            {
                GrayInitialImage = GrayProcessedImage;
            }
            else if (ColorProcessedImage != null)
            {
                ColorInitialImage = ColorProcessedImage;
            }

            ClearProcessedCanvas(canvases[1]);
        }
        #endregion

        #region Helpers
        private Image<TColor, byte> AskForImage<TColor>(string dialogTitle)
            where TColor : struct, IColor
        {
            _windowService.CloseAllChildWindows();

            string fileName = LoadFileDialog(dialogTitle);
            return fileName != null ? ReadImage<TColor>(fileName) : null;
        }

        private static Image<TColor, byte> ReadImage<TColor>(string fileName)
            where TColor : struct, IColor
        {
            try
            {
                return LoadImage<TColor>(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"The image could not be opened:\n{fileName}\n\n{ex.Message}");
                return null;
            }
        }

        private bool IsImageLoaded()
        {
            if (GrayInitialImage != null || ColorInitialImage != null)
                return true;

            MessageBox.Show("Please add an image!");
            return false;
        }

        private bool IsImageProcessed()
        {
            if (GrayProcessedImage != null || ColorProcessedImage != null)
                return true;

            MessageBox.Show("Please process an image first!");
            return false;
        }

        private bool IsAreaSelected()
        {
            if (!IsImageLoaded())
                return false;

            if (MouseClickCollection.Count > 0)
                return true;

            MessageBox.Show("Please select an area first!");
            return false;
        }
        #endregion
    }
}