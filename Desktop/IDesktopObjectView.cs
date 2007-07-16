using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    public interface IDesktopObjectView : IView, IDisposable
    {
        event EventHandler VisibleChanged;
        event EventHandler ActiveChanged;
        event EventHandler CloseRequested;

        void SetTitle(string title);
        void Open();
        void Show();
        void Hide();
        void Activate();

        bool Visible { get; }
        bool Active { get; }
    }
}
