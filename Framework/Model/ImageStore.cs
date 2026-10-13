using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Windows;
using PointCollection = System.Windows.Media.PointCollection;

namespace Framework.Model
{
    public class ImageStore : IImageStore
    {
        private static readonly Lazy<ImageStore> _instance =
            new Lazy<ImageStore>(() => new ImageStore());

        public static ImageStore Instance => _instance.Value;

        private ImageStore() { }

        public event EventHandler<ImageCanvasType> ImageChanged;

        #region Images
        private Image<Gray, byte> _grayInitialImage;
        public Image<Gray, byte> GrayInitialImage
        {
            get => _grayInitialImage;
            set
            {
                EnsureNotEmpty(value);
                _grayInitialImage = value;
                if (value != null)
                {
                    _colorInitialImage = null;
                    ImageChanged?.Invoke(this, ImageCanvasType.Initial);
                }
            }
        }

        private Image<Gray, byte> _grayProcessedImage;
        public Image<Gray, byte> GrayProcessedImage
        {
            get => _grayProcessedImage;
            set
            {
                EnsureNotEmpty(value);
                _grayProcessedImage = value;
                if (value != null)
                {
                    _colorProcessedImage = null;
                    ImageChanged?.Invoke(this, ImageCanvasType.Processed);
                }
            }
        }

        private Image<Bgr, byte> _colorInitialImage;
        public Image<Bgr, byte> ColorInitialImage
        {
            get => _colorInitialImage;
            set
            {
                EnsureNotEmpty(value);
                _colorInitialImage = value;
                if (value != null)
                {
                    _grayInitialImage = null;
                    ImageChanged?.Invoke(this, ImageCanvasType.Initial);
                }
            }
        }

        private Image<Bgr, byte> _colorProcessedImage;
        public Image<Bgr, byte> ColorProcessedImage
        {
            get => _colorProcessedImage;
            set
            {
                EnsureNotEmpty(value);
                _colorProcessedImage = value;
                if (value != null)
                {
                    _grayProcessedImage = null;
                    ImageChanged?.Invoke(this, ImageCanvasType.Processed);
                }
            }
        }
        #endregion

        #region Querying and clearing
        public bool HasInitialImage => GrayInitialImage != null || ColorInitialImage != null;
        public bool HasProcessedImage => GrayProcessedImage != null || ColorProcessedImage != null;

        public IImage GetImage(ImageCanvasType canvas)
        {
            return canvas == ImageCanvasType.Initial
                ? (IImage)GrayInitialImage ?? ColorInitialImage
                : (IImage)GrayProcessedImage ?? ColorProcessedImage;
        }

        public void ClearInitial()
        {
            _grayInitialImage = null;
            _colorInitialImage = null;
            ImageChanged?.Invoke(this, ImageCanvasType.Initial);
        }

        public void ClearProcessed()
        {
            _grayProcessedImage = null;
            _colorProcessedImage = null;
            ImageChanged?.Invoke(this, ImageCanvasType.Processed);
        }
        #endregion

        #region Selected points
        public Point LastMouseClick { get; set; } = new Point(0, 0);
        public PointCollection MouseClickCollection { get; } = new PointCollection();
        #endregion

        #region Helpers
        private static void EnsureNotEmpty(IImage image)
        {
            if (image != null && (image.Size.Width == 0 || image.Size.Height == 0))
                throw new ArgumentException($"The image is empty ({image.Size.Width} x {image.Size.Height} pixels).");
        }
        #endregion
    }
}
