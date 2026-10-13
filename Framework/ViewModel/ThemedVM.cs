using Framework.Messaging;
using Framework.Themes;

namespace Framework.ViewModel
{
    public class ThemedVM : BaseVM
    {
        protected readonly IMessageBus _messageBus;

        public ThemedVM()
            : this(MessageBus.Instance)
        {
        }

        public ThemedVM(IMessageBus messageBus)
        {
            _messageBus = messageBus;
            _messageBus.Subscribe<ThemeChangedMessage>(this, message => Theme = message.Theme);
        }

        private ITheme _theme = LimeGreenTheme.Instance;
        public ITheme Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                NotifyPropertyChanged(nameof(Theme));
            }
        }

        public virtual void Cleanup()
        {
            _messageBus.Unsubscribe<ThemeChangedMessage>(this);
        }
    }
}
