using Emgu.CV;

using System;

namespace Framework.ViewModel
{
    class SliderVM : SliderBaseVM
    {
        public SliderVM(MainVM mainVM)
            : base(mainVM)
        {
        }

        #region Properties
        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                NotifyPropertyChanged(nameof(Description));
            }
        }

        private double _minimumValue;
        public double MinimumValue
        {
            get => _minimumValue;
            set
            {
                _minimumValue = value;
                NotifyPropertyChanged(nameof(MinimumValue));
            }
        }

        private double _maximumValue;
        public double MaximumValue
        {
            get => _maximumValue;
            set
            {
                _maximumValue = value;
                NotifyPropertyChanged(nameof(MaximumValue));
            }
        }

        private double _value;
        public double Value
        {
            get => _value;
            set
            {
                _value = value;
                NotifyPropertyChanged(nameof(Value));

                ModifyProcessedImage();
            }
        }

        private double _frequency;
        public double Frequency
        {
            get => _frequency;
            set
            {
                _frequency = value;
                NotifyPropertyChanged(nameof(Frequency));
            }
        }
        #endregion

        protected override Func<IImage> CreateComputation()
        {
            var algorithm = Algorithm;
            var image = Image;
            double value = Value;

            return () => algorithm(image, value);
        }
    }
}
