using System;
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
    [ExtensionPoint]
    public class TimeDialogViewExtensionPoint : ExtensionPoint<ITimeDialogView>
    {
    }

    public interface ITimeDialogView : IView
    {
        void SetDialog(TimedDialog dialog);
        void Open();
        void Close();
    }

    //TODO (Marmot): Make a desktop object that hosts a component, if it proves useful.
    public class TimedDialog : INotifyPropertyChanged
    {
        private string _title;
        private string _message;
        private string _linkText;
        private readonly Action _followLink;

        private ITimeDialogView _view;

        internal TimedDialog(Action followLink)
            : this(TimeSpan.FromSeconds(3), followLink)
        {
        }

        internal TimedDialog(TimeSpan lingerTime, Action followLink)
        {
            Platform.CheckForNullReference(followLink, "followLink");
            _followLink = followLink;
            LingerTime = lingerTime;
            _view = (ITimeDialogView) new TimeDialogViewExtensionPoint().CreateExtension();
        }

        #region Presentation Model

        public string Title
        {
            get { return _title; }
            set
            {
                if (Equals(value, _title))
                    return;

                _title = value;
                NotifyPropertyChanged("Title");
            }
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

        public TimeSpan LingerTime { get; private set; }

        public void FollowLink()
        {
            _followLink();
            _view.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public void Show()
        {
            _view.SetDialog(this);
            _view.Open();
        }

        public void Close()
        {
            _view.Close();
        }

        public static void Show(string title, string message, string linkText, Action followLink)
        {
            var dialog = new TimedDialog(followLink) {Title = title, Message = message, LinkText = linkText};
            dialog.Show();
        }

        public static void Show(string title, string message, string linkText, Action followLink, TimeSpan lingerTime)
        {
            var dialog = new TimedDialog(lingerTime, followLink) { Title = title, Message = message, LinkText = linkText };
            dialog.Show();
        }

        public static void Show(string message, string linkText, Action followLink)
        {
            Show(null, message, linkText, followLink);
        }

        public static void Show(string message, string linkText, Action followLink, TimeSpan lingerTime)
        {
            Show(null, message, linkText, followLink, lingerTime);
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            EventsHelper.Fire(PropertyChanged, this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
