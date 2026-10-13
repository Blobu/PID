using System.Globalization;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Framework.ViewModel
{
    class DialogVM : ThemedVM
    {
        public double Height { get; set; }

        public ObservableCollection<DialogParameter> Parameters { get; } =
            new ObservableCollection<DialogParameter>();

        public void CreateParameters(List<string> labels)
        {
            Height = (labels.Count + 3) * 40;

            foreach (var label in labels)
            {
                Parameters.Add(new DialogParameter()
                {
                    ParamText = label,
                    Height = 20,
                });
            }
        }

        public List<double> GetValues()
        {
            var values = new List<double>();

            foreach (var parameter in Parameters)
                values.Add(TryParseNumber(parameter.InputText, out double value) ? value : 0);

            return values;
        }

        public int FindInvalidParameter()
        {
            for (int index = 0; index < Parameters.Count; ++index)
            {
                if (!TryParseNumber(Parameters[index].InputText, out _))
                    return index;
            }

            return -1;
        }

        private static bool TryParseNumber(string text, out double value)
        {
            return double.TryParse(text?.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        internal class DialogParameter
        {
            public string ParamText { get; set; }
            public string InputText { get; set; }
            public double Height { get; set; }
        }
    }
}