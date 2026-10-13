using Emgu.CV;
using Emgu.CV.Structure;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using System;
using System.Linq;
using System.Windows.Media;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Framework.Model;

using static Algorithms.Utilities.Utils;

namespace Framework.ViewModel
{
    class HistogramVM : ThemedVM
    {
        public void CreateHistogram(ImageCanvasType canvas)
        {
            string imageName = canvas == ImageCanvasType.Initial ? "Initial" : "Processed";
            List<int[]> histograms = new List<int[]>();
            List<string> colors = new List<string>();

            switch (ImageStore.Instance.GetImage(canvas))
            {
                case Image<Gray, byte> grayImage:
                    Title = $"Grayscale - {imageName} Image Histogram";
                    InitHelper(grayImage, histograms, colors);
                    break;

                case Image<Bgr, byte> colorImage:
                    Title = $"Color - {imageName} Image Histogram";
                    InitHelper(colorImage, histograms, colors);
                    break;
            }

            InitializeHistogram(histograms, colors);
        }

        public void CreateHistogram<T>(List<(T[] Histogram, string PlotColor)> values)
        {
            List<T[]> histograms = values.Select(_ => _.Histogram).ToList();
            List<string> colors = values.Select(_ => _.PlotColor).ToList();

            InitializeHistogram(histograms, colors);
        }

        #region Properties
        private string _title = "Histogram";
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                NotifyPropertyChanged(nameof(Title));
            }
        }

        private double _width;
        public double Width
        {
            get => _width;
            set
            {
                _width = value;
                NotifyPropertyChanged(nameof(Width));
            }
        }

        public ObservableCollection<PlotModel> Plots { get; set; } =
            new ObservableCollection<PlotModel>();
        #endregion

        #region Helpers
        private void InitializeHistogram<T>(List<T[]> histograms, List<string> colors)
        {
            Plots.Clear();
            Width = 400 * histograms.Count;

            for (int index = 0; index < histograms.Count; ++index)
            {
                var plot = new PlotModel();

                plot.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    Maximum = 256,
                    Minimum = -1,
                });

                var series = new RectangleBarSeries
                {
                    StrokeThickness = 2.5,
                    StrokeColor = OxyColor.Parse(colors[index]),
                };

                for (int i = 0; i < histograms[index].Length; ++i)
                {
                    double barValue = Convert.ToDouble(histograms[index][i]);
                    if (barValue != 0)
                    {
                        var bar = new RectangleBarItem(i, 0, i + 0.01, barValue);
                        series.Items.Add(bar);
                    }
                }

                plot.Series.Add(series);
                Plots.Add(plot);
            }
        }

        private void InitHelper(Image<Gray, byte> image, List<int[]> histograms, List<string> colors)
        {
            histograms.Add(ComputeHistogram(image));
            colors.Add(Brushes.Black.ToString());
        }

        private void InitHelper(Image<Bgr, byte> image, List<int[]> histograms, List<string> colors)
        {
            for (int channel = 0; channel < 3; ++channel)
            {
                using (Image<Gray, byte> channelImage = image[channel])
                    histograms.Add(ComputeHistogram(channelImage));
            }

            colors.Add(Brushes.Blue.ToString());
            colors.Add(Brushes.Green.ToString());
            colors.Add(Brushes.Red.ToString());
        }
        #endregion
    }
}