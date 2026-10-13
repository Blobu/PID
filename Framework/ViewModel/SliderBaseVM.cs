using Emgu.CV;
using Emgu.CV.Structure;

using System;
using System.Threading.Tasks;

using Framework.Model;

namespace Framework.ViewModel
{
    abstract class SliderBaseVM : ThemedVM
    {
        private readonly IImageStore _imageStore;
        private bool _isRecomputeNeeded;
        private bool _isClosed;

        protected SliderBaseVM(MainVM mainVM)
        {
            _imageStore = ImageStore.Instance;
            Theme = mainVM.Theme;
        }

        public dynamic Algorithm = null;
        public dynamic Image = null;

        #region Computation
        private bool _isComputing;
        public bool IsComputing
        {
            get => _isComputing;
            private set
            {
                _isComputing = value;
                NotifyPropertyChanged(nameof(IsComputing));
            }
        }

        protected abstract Func<IImage> CreateComputation();

        public async void ModifyProcessedImage()
        {
            if (Image == null || Algorithm == null)
                return;

            if (IsComputing)
            {
                _isRecomputeNeeded = true;
                return;
            }

            IsComputing = true;

            try
            {
                do
                {
                    _isRecomputeNeeded = false;

                    var imageBefore = GetProcessedImage();
                    var processedImage = await Task.Run(CreateComputation());

                    if (!_isClosed && ReferenceEquals(GetProcessedImage(), imageBefore))
                        SetProcessedImage(processedImage);
                }
                while (_isRecomputeNeeded && !_isClosed);
            }
            finally
            {
                IsComputing = false;
            }
        }

        public override void Cleanup()
        {
            _isClosed = true;
            base.Cleanup();
        }
        #endregion

        #region Helpers
        private IImage GetProcessedImage()
        {
            return _imageStore.GetImage(ImageCanvasType.Processed);
        }

        private void SetProcessedImage(IImage image)
        {
            if (image is Image<Gray, byte> grayImage)
                _imageStore.GrayProcessedImage = grayImage;
            else if (image is Image<Bgr, byte> colorImage)
                _imageStore.ColorProcessedImage = colorImage;
        }
        #endregion
    }
}
