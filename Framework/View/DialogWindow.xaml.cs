using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;

using Framework.ViewModel;

namespace Framework.View
{
    public partial class DialogWindow : Window
    {
        private readonly DialogVM _dialogVM;
        private bool _isFirstBoxFocused;
        private bool _isAccepted;

        public DialogWindow(MainVM mainVM, List<string> labels)
        {
            InitializeComponent();
            UiHelper.SetMainWindowAsOwner(this);

            _dialogVM = new DialogVM();

            _dialogVM.Theme = mainVM.Theme;
            _dialogVM.CreateParameters(labels);

            DataContext = _dialogVM;
        }

        public List<double> GetValues()
        {
            return _isAccepted ? _dialogVM.GetValues() : null;
        }

        #region Event handlers
        private void ParameterBoxLoaded(object sender, RoutedEventArgs e)
        {
            if (_isFirstBoxFocused)
                return;

            _isFirstBoxFocused = true;
            ((TextBox)sender).Focus();
        }

        private void DoneClick(object sender, RoutedEventArgs e)
        {
            int invalidIndex = _dialogVM.FindInvalidParameter();
            if (invalidIndex >= 0)
            {
                MessageBox.Show(this, $"Please enter a number for \"{_dialogVM.Parameters[invalidIndex].ParamText.Trim()}\".");

                var parameterBox = FindParameterBox(invalidIndex);
                parameterBox?.Focus();
                parameterBox?.SelectAll();
                return;
            }

            _isAccepted = true;
            Close();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _dialogVM.Cleanup();
        }
        #endregion

        #region Helpers
        private TextBox FindParameterBox(int index)
        {
            var container = parameterList.ItemContainerGenerator.ContainerFromIndex(index);
            return container == null ? null : FindDescendant<TextBox>(container);
        }

        private static TElement FindDescendant<TElement>(DependencyObject parent)
            where TElement : DependencyObject
        {
            for (int index = 0; index < VisualTreeHelper.GetChildrenCount(parent); ++index)
            {
                var child = VisualTreeHelper.GetChild(parent, index);
                if (child is TElement element)
                    return element;

                var descendant = FindDescendant<TElement>(child);
                if (descendant != null)
                    return descendant;
            }

            return null;
        }
        #endregion
    }
}
