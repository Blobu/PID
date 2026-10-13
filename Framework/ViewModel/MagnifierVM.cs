using Emgu.CV;
using Emgu.CV.Structure;
using Framework.Messaging;
using Framework.Model;
using System;
using System.Drawing;
using System.Windows.Media;
using ImageConverter = Framework.Utilities.ImageConverter;
using Point = System.Windows.Point;

namespace Framework.ViewModel
{
    public class MagnifierVM : ThemedVM
    {
        private readonly IImageStore _imageStore;
        private Point _lastPoint;

        public MagnifierVM()
            : this(MessageBus.Instance, ImageStore.Instance)
        {
        }

        public MagnifierVM(IMessageBus messageBus, IImageStore imageStore)
            : base(messageBus)
        {
            _imageStore = imageStore;
        }

        #region Rendering
        public void RefreshMagnifierImages()
        {
            _lastPoint = _imageStore.LastMouseClick;

            if (_imageStore.GrayInitialImage != null)
                InitialMagnifierImage = RenderGrid(_imageStore.GrayInitialImage);
            else if (_imageStore.ColorInitialImage != null)
                InitialMagnifierImage = RenderGrid(_imageStore.ColorInitialImage);
            else
                InitialMagnifierImage = null;

            if (_imageStore.GrayProcessedImage != null)
                ProcessedMagnifierImage = RenderGrid(_imageStore.GrayProcessedImage);
            else if (_imageStore.ColorProcessedImage != null)
                ProcessedMagnifierImage = RenderGrid(_imageStore.ColorProcessedImage);
            else
                ProcessedMagnifierImage = null;
        }

        private ImageSource RenderGrid(Image<Bgr, byte> image)
        {
            return RenderGrid(image.Width, image.Height, (x, y) =>
            {
                int b = image.Data[y, x, 0];
                int g = image.Data[y, x, 1];
                int r = image.Data[y, x, 2];
                return (System.Drawing.Color.FromArgb(r, g, b), r + "\n" + g + "\n" + b, b <= 128 && g <= 128 && r <= 128);
            });
        }

        private ImageSource RenderGrid(Image<Gray, byte> image)
        {
            return RenderGrid(image.Width, image.Height, (x, y) =>
            {
                int gray = image.Data[y, x, 0];
                return (System.Drawing.Color.FromArgb(gray, gray, gray), "\n" + gray + "\n", gray <= 128);
            });
        }

        private ImageSource RenderGrid(int imageWidth, int imageHeight,
            Func<int, int, (System.Drawing.Color Fill, string Text, bool IsDark)> describePixel)
        {
            int step = GridSize / 9;

            using (Bitmap grid = new Bitmap(GridSize, GridSize))
            {
                using (Graphics g = Graphics.FromImage(grid))
                using (Font font = new Font("Arial", 7, FontStyle.Bold, GraphicsUnit.Point))
                {
                    for (int dx = -4; dx <= 4; ++dx)
                    {
                        for (int dy = -4; dy <= 4; ++dy)
                        {
                            Rectangle cell = new Rectangle((dx + 4) * step, (dy + 4) * step, step, step);

                            if (_lastPoint.Y + dy >= 0 && _lastPoint.X + dx >= 0 &&
                                _lastPoint.Y + dy < imageHeight && _lastPoint.X + dx < imageWidth)
                            {
                                var pixel = describePixel((int)_lastPoint.X + dx, (int)_lastPoint.Y + dy);

                                using (var brush = new SolidBrush(pixel.Fill))
                                    g.FillRectangle(brush, cell);
                                g.DrawString(pixel.Text, font, pixel.IsDark
                                    ? System.Drawing.Brushes.White : System.Drawing.Brushes.Black, cell);
                            }
                            else
                            {
                                g.FillRectangle(System.Drawing.Brushes.White, cell);
                            }
                        }
                    }
                }
                return ImageConverter.Convert(grid);
            }
        }
        #endregion

        #region Properties
        public int GridSize { get; set; } = 450;

        private ImageSource _initialMagnifierImage;
        public ImageSource InitialMagnifierImage
        {
            get => _initialMagnifierImage;
            set { _initialMagnifierImage = value; NotifyPropertyChanged(nameof(InitialMagnifierImage)); }
        }

        private ImageSource _processedMagnifierImage;
        public ImageSource ProcessedMagnifierImage
        {
            get => _processedMagnifierImage;
            set { _processedMagnifierImage = value; NotifyPropertyChanged(nameof(ProcessedMagnifierImage)); }
        }
        #endregion
    }
}
