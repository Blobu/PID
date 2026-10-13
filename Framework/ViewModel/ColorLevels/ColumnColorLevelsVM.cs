using Emgu.CV;

using System.Collections.Generic;

using Framework.Model;

namespace Framework.ViewModel.ColorLevels
{
    internal class ColumnColorLevelsVM : ColorLevelsVM
    {
        public ColumnColorLevelsVM(IImageStore imageStore)
            : base(imageStore)
        {
        }

        protected override int GetLineLength(IImage image) => image.Size.Height;

        protected override List<int> GetLevels<TColor>(Image<TColor, byte> image, int channel)
        {
            List<int> values = new List<int>();

            var lastClick = _imageStore.LastMouseClick;
            if (lastClick.X < image.Width)
            {
                for (int y = 0; y < image.Height; y++)
                    values.Add(image.Data[y, (int)lastClick.X, channel]);
            }

            return values;
        }

        public override string Title { get; set; } = "Color Levels on column";
    }
}
