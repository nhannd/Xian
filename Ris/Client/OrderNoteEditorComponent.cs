using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    [ExtensionPoint]
    public class OrderNoteEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(OrderNoteEditorComponentViewExtensionPoint))]
    public class OrderNoteEditorComponent : ApplicationComponent
    {
        private readonly OrderNoteDetail _note;

        public OrderNoteEditorComponent(OrderNoteDetail noteDetail)
        {
            _note = noteDetail;
        }

        #region Presentation Model

        public string Comment
        {
            get { return _note.Comment; }
            set
            {
                _note.Comment = value;
                this.Modified = true;
            }
        }

        public bool IsNewItem
        {
            get { return _note.CreationTime == null; }
        }

        public void Accept()
        {
            this.ExitCode = ApplicationComponentExitCode.Accepted;
            Host.Exit();
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.None;
            Host.Exit();
        }

        public bool AcceptEnabled
        {
            get { return this.Modified && this.IsNewItem; }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }

        #endregion
    }
}
