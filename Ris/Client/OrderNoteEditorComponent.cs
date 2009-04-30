#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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

        public OrderNoteEditorComponent(OrderNoteDetail noteDetail)
        {
            _note = noteDetail;
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
            get { return _note.NoteBody; }
            set
            {
                _note.NoteBody = value;
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
