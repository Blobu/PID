using Emgu.CV;
using Emgu.CV.Structure;

using System.Collections.Generic;
using System.Windows;

using OxyPlot;
using OxyPlot.Axes;
using LineSeries = OxyPlot.Series.LineSeries;

using Framework.Model;

namespace Framework.ViewModel.ColorLevels
{
    public abstract class ColorLevelsVM : ThemedVM
    {
        protected readonly IImageStore _imageStore;
        private Point? _lastPoint;

        public ColorLevelsVM(IImageStore imageStore)
        {
            _imageStore = imageStore;
        }

        #region Updating the plots
        public void UpdateSelectedPoint()
        {
            var point = _imageStore.LastMouseClick;
            if (point == _lastPoint)
                return;

            _lastPoint = point;
            XPos = "X: " + (int)point.X;
            YPos = "Y: " + (int)point.Y;
            RefreshPlots();
        }

        public void RefreshPlots()
        {
            InitialPlot = CreatePlot(_imageStore.GetImage(ImageCanvasType.Initial));
            ProcessedPlot = CreatePlot(_imageStore.GetImage(ImageCanvasType.Processed));
            HasColorImage = _imageStore.ColorInitialImage != null || _imageStore.ColorProcessedImage != null;
        }

        private PlotModel CreatePlot(IImage image)
        {
            if (image == null)
                return new PlotModel();

            var plot = PlotImage(image);
            ApplyChannelVisibility(plot);
            return plot;
        }
        #endregion

        #region Plot creation
        private PlotModel PlotImage(IImage image)
        {
            var plot = new PlotModel();

            plot.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Maximum = GetLineLength(image) + 30,
                Minimum = -1,
            });

            plot.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Maximum = 300,
                Minimum = -1,
            });

            if (image is Image<Gray, byte> grayImage)
            {
                plot.Series.Add(CreateSeries(GetLevels(grayImage, 0), OxyColors.Blue));
            }
            else if (image is Image<Bgr, byte> colorImage)
            {
                plot.Series.Add(CreateSeries(GetLevels(colorImage, 0), OxyColors.Blue));
                plot.Series.Add(CreateSeries(GetLevels(colorImage, 1), OxyColors.Green));
                plot.Series.Add(CreateSeries(GetLevels(colorImage, 2), OxyColors.Red));
            }

            return plot;
        }

        protected abstract int GetLineLength(IImage image);

        protected abstract List<int> GetLevels<TColor>(Image<TColor, byte> image, int channel)
            where TColor : struct, IColor;

        private LineSeries CreateSeries(List<int> values, OxyColor color)
        {
            var series = new LineSeries
            {
                MarkerType = MarkerType.None,
                MarkerSize = 1,
                MarkerStroke = color,
                MarkerFill = color,
                Color = color
            };

            for (int index = 0; index < values.Count; ++index)
                series.Points.Add(new DataPoint(index, values[index]));

            return series;
        }
        #endregion

        #region Channel visibility
        private bool _isBlueChannelVisible = true;
        public bool IsBlueChannelVisible
        {
            get => _isBlueChannelVisible;
            set
            {
                _isBlueChannelVisible = value;
                NotifyPropertyChanged(nameof(IsBlueChannelVisible));
                ApplyChannelVisibility();
            }
        }

        private bool _isGreenChannelVisible = true;
        public bool IsGreenChannelVisible
        {
            get => _isGreenChannelVisible;
            set
            {
                _isGreenChannelVisible = value;
                NotifyPropertyChanged(nameof(IsGreenChannelVisible));
                ApplyChannelVisibility();
            }
        }

        private bool _isRedChannelVisible = true;
        public bool IsRedChannelVisible
        {
            get => _isRedChannelVisible;
            set
            {
                _isRedChannelVisible = value;
                NotifyPropertyChanged(nameof(IsRedChannelVisible));
                ApplyChannelVisibility();
            }
        }

        private void ApplyChannelVisibility()
        {
            ApplyChannelVisibility(InitialPlot);
            ApplyChannelVisibility(ProcessedPlot);
        }

        private void ApplyChannelVisibility(PlotModel plot)
        {
            // Only the plot of a color image has one series per channel (blue, green, red).
            if (plot == null || plot.Series.Count != 3)
                return;

            plot.Series[0].IsVisible = IsBlueChannelVisible;
            plot.Series[1].IsVisible = IsGreenChannelVisible;
            plot.Series[2].IsVisible = IsRedChannelVisible;
            plot.InvalidatePlot(true);
        }
        #endregion

        #region Properties
        public virtual string Title { get; set; } = "Color Levels";

        private string _xPos;
        public string XPos
        {
            get => _xPos;
            set
            {
                _xPos = value;
                NotifyPropertyChanged(nameof(XPos));
            }
        }

        private string _yPos;
        public string YPos
        {
            get => _yPos;
            set
            {
                _yPos = value;
                NotifyPropertyChanged(nameof(YPos));
            }
        }

        private PlotModel _initialPlot;
        public PlotModel InitialPlot
        {
            get => _initialPlot;
            private set
            {
                _initialPlot = value;
                NotifyPropertyChanged(nameof(InitialPlot));
            }
        }

        private PlotModel _processedPlot;
        public PlotModel ProcessedPlot
        {
            get => _processedPlot;
            private set
            {
                _processedPlot = value;
                NotifyPropertyChanged(nameof(ProcessedPlot));
            }
        }

        private bool _hasColorImage;
        public bool HasColorImage
        {
            get => _hasColorImage;
            private set
            {
                _hasColorImage = value;
                NotifyPropertyChanged(nameof(HasColorImage));
            }
        }
        #endregion
    }
}
