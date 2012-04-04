using System;
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Services.Tools
{
    [ExtensionPoint]
    public class TimeDialogViewExtensionPoint : ExtensionPoint<ITimeDialogView>
    {
    }

    public interface ITimeDialogView : IView
    {
        void SetDialog(TimedDialog dialog);
        void Open(TimeSpan lingerTime);
        void Close();
    }

    //TODO (Marmot): Make an app component.
    public class TimedDialog : INotifyPropertyChanged
    {
        private string _message;
        private string _linkText;
        private readonly Action _followLink;

        private ITimeDialogView _view;

        public TimedDialog(Action followLink)
        {
            Platform.CheckForNullReference(followLink, "followLink");
            _followLink = followLink;
            _view = (ITimeDialogView) new TimeDialogViewExtensionPoint().CreateExtension();
        }

        public string Message
        {
            get { return _message; }
            set
            {
                if (Equals(value, _message))
                    return;

                _message = value;
                NotifyPropertyChanged("Message");
            }
        }

        public string LinkText
        {
            get { return _linkText; }
            set
            {
                if (Equals(value, _linkText))
                    return;

                _linkText = value;
                NotifyPropertyChanged("LinkText");
            }
        }

        public void FollowLink()
        {
            _followLink();
            _view.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            EventsHelper.Fire(PropertyChanged, this, new PropertyChangedEventArgs(propertyName));
        }

        public void Show()
        {
            _view.SetDialog(this);
            _view.Open(TimeSpan.FromSeconds(3));
        }

        public void Close()
        {
            _view.Close();
        }
    }
}
