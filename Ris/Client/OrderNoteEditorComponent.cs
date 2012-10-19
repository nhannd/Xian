#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
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
		private ICannedTextLookupHandler _cannedTextLookupHandler;
    	private string _noteBody;

        public OrderNoteEditorComponent(OrderNoteDetail noteDetail)
        {
            _note = noteDetail;
        	_noteBody = noteDetail.NoteBody;
        }

		public override void Start()
		{
			_cannedTextLookupHandler = new CannedTextLookupHandler(this.Host.DesktopWindow);

			base.Start();
		}

        #region Presentation Model

		public ICannedTextLookupHandler CannedTextLookupHandler
		{
			get { return _cannedTextLookupHandler; }
		}
		
		[ValidateNotNull]
		public string Comment
        {
			get { return _noteBody; }
            set
            {
				_noteBody = value;
                this.Modified = true;
            }
        }

        public bool IsNewItem
        {
            get { return _note.OrderNoteRef == null; }
        }

        public void Accept()
        {
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

        	_note.NoteBody = _noteBody;
            this.Exit(ApplicationComponentExitCode.Accepted);
        }

        public void Cancel()
        {
            this.Exit(ApplicationComponentExitCode.None);
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
