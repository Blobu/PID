using Framework.Model;

namespace Framework.ViewModel.ColorLevels
{
    public static class ColorLevelsFactory
    {
        public static ColorLevelsVM Produce(ColorLevelsType type)
        {
            return Produce(type, ImageStore.Instance);
        }

        public static ColorLevelsVM Produce(ColorLevelsType type, IImageStore imageStore)
        {
            switch (type)
            {
                case ColorLevelsType.Row:
                    return new RowColorLevelsVM(imageStore);

                case ColorLevelsType.Column:
                    return new ColumnColorLevelsVM(imageStore);
            }

            return null;
        }
    }
}
