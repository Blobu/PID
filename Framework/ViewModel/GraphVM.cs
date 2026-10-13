using OxyPlot;
using OxyPlot.Series;

using System.Windows.Media;

namespace Framework.ViewModel
{
    class GraphVM : ThemedVM
    {
        public void CreateGraph(PointCollection points)
        {
            var series = new LineSeries
            {
                MarkerType = MarkerType.None,
                MarkerSize = 1,
                MarkerStroke = OxyColors.Red,
                MarkerFill = OxyColors.Red,
                Color = OxyColors.Red
            };

            foreach (var point in points)
                series.Points.Add(new DataPoint(point.X, point.Y));

            Plot.Series.Add(series);
        }

        #region Properties
        public PlotModel Plot { get; set; } =
            new PlotModel();
        #endregion
    }
}