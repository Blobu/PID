using Emgu.CV;

using System.Collections.Generic;

using Framework.Model;

namespace Framework.ViewModel.ColorLevels
{
    internal class RowColorLevelsVM : ColorLevelsVM
    {
        public RowColorLevelsVM(IImageStore imageStore)
            : base(imageStore)
        {
        }

        protected override int GetLineLength(IImage image) => image.Size.Width;

        protected override List<int> GetLevels<TColor>(Image<TColor, byte> image, int channel)
        {
            List<int> values = new List<int>();

            var lastClick = _imageStore.LastMouseClick;
            if (lastClick.Y < image.Height)
            {
                for (int x = 0; x < image.Width; x++)
                    values.Add(image.Data[(int)lastClick.Y, x, channel]);
            }

            return values;
        }

        public override string Title { get; set; } = "Color Levels on row";
    }
}
