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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Services.Tools
{

	[MenuAction("cancel", "dicom-file-import-contextmenu/MenuCancel", "Cancel")]
	[Tooltip("cancel", "TooltipCancel")]
	[EnabledStateObserver("cancel", "CancelEnabled", "CancelEnabledChanged")]

	[MenuAction("clear", "dicom-file-import-contextmenu/MenuClear", "Clear")]
	[Tooltip("clear", "TooltipClear")]
	[EnabledStateObserver("clear", "ClearEnabled", "ClearEnabledChanged")]

	[ButtonAction("clearInactive", "dicom-file-import-toolbar/ClearInactive", "ClearInactive")]
	[Tooltip("clearInactive", "TooltipClearInactive")]
	[IconSet("clearInactive", IconScheme.Colour, "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png")]
	[EnabledStateObserver("clearInactive", "ClearInactiveEnabled", "ClearInactiveEnabledChanged")]

	[ButtonAction("showBackground", "dicom-file-import-toolbar/ShowBackgroundImports", "ToggleShowBackground", Flags = ClickActionFlags.CheckAction)]
	[Tooltip("showBackground", "TooltipShowBackgroundImports")]
	[IconSet("showBackground", IconScheme.Colour, "Icons.ShowBackgroundImportsToolSmall.png", "Icons.ShowBackgroundImportsToolSmall.png", "Icons.ShowBackgroundImportsToolSmall.png")]
	[CheckedStateObserver("showBackground", "ShowBackground", "ShowBackgroundChanged")]

	[ExtensionOf(typeof(DicomFileImportComponentToolExtensionPoint))]
	public class DicomFileImportTools : Tool<IDicomFileImportComponentToolContext>
	{
		private bool _cancelEnabled;
		private bool _clearEnabled;
		private bool _clearInactiveEnabled;
		private event EventHandler _cancelEnabledChanged;
		private event EventHandler _clearEnabledChanged;
		private event EventHandler _clearInactiveEnabledChanged;
		private event EventHandler _showBackgroundChanged;

		public DicomFileImportTools()
		{
			_clearEnabled = false;
			_cancelEnabled = false;
			_clearInactiveEnabled = false;
		}

		public override void Initialize()
		{
			base.Initialize();
			this.Context.SelectionUpdated += new EventHandler(OnSelectionUpdated);
		}

		private void OnSelectionUpdated(object sender, EventArgs e)
		{
			this.CancelEnabled = this.Context.CanCancelSelected();
			this.ClearEnabled = this.Context.CanClearSelected();
			this.ClearInactiveEnabled = this.Context.CanClearInactive();
		}

		public bool CancelEnabled
		{
			get { return _cancelEnabled; }
			protected set
			{
				if (_cancelEnabled != value)
				{
					_cancelEnabled = value;
					EventsHelper.Fire(_cancelEnabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public bool ClearEnabled
		{
			get { return _clearEnabled; }
			protected set
			{
				if (_clearEnabled != value)
				{
					_clearEnabled = value;
					EventsHelper.Fire(_clearEnabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public bool ClearInactiveEnabled
		{
			get { return _clearInactiveEnabled; }
			protected set
			{
				if (_clearInactiveEnabled != value)
				{
					_clearInactiveEnabled = value;
					EventsHelper.Fire(_clearInactiveEnabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public bool ShowBackground
		{
			get { return base.Context.ShowBackgroundImports; }
			set
			{
				if (base.Context.ShowBackgroundImports != value)
				{
					base.Context.ShowBackgroundImports = value;
					EventsHelper.Fire(_showBackgroundChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler CancelEnabledChanged
		{
			add { _cancelEnabledChanged += value; }
			remove { _cancelEnabledChanged -= value; }
		}

		public event EventHandler ClearEnabledChanged
		{
			add { _clearEnabledChanged += value; }
			remove { _clearEnabledChanged -= value; }
		}

		public event EventHandler ClearInactiveEnabledChanged
		{
			add { _clearInactiveEnabledChanged += value; }
			remove { _clearInactiveEnabledChanged -= value; }
		}

		public event EventHandler ShowBackgroundChanged
		{
			add { _showBackgroundChanged += value; }
			remove { _showBackgroundChanged -= value; }
		}

		private void Cancel()
		{
			try
			{
				this.Context.CancelSelected();

			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageCancelFailed, this.Context.DesktopWindow);
			}
		}

		private void Clear()
		{
			try
			{
				this.Context.ClearSelected();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageClearFailed, this.Context.DesktopWindow);
			}
		}

		private void ClearInactive()
		{
			try
			{
				this.Context.ClearInactive();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageClearInactiveFailed, this.Context.DesktopWindow);
			}
		}

		private void ToggleShowBackground()
		{
			ShowBackground = !ShowBackground;
		}
	}
}
