using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common.CannedTextService;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="CannedTextEditorComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class CannedTextEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// CannedTextEditorComponent class
	/// </summary>
	[AssociateView(typeof(CannedTextEditorComponentViewExtensionPoint))]
	public class CannedTextEditorComponent : ApplicationComponent
	{
		private readonly CannedTextDetail _cannedTextDetail;

		/// <summary>
		/// Constructor
		/// </summary>
		public CannedTextEditorComponent(CannedTextDetail detail)
		{
			_cannedTextDetail = detail;
		}

		#region Presentation Model

		public bool AcceptEnabled
		{
			get { return this.Modified; }
		}

		public event EventHandler AcceptEnabledChanged
		{
			add { this.ModifiedChanged += value; }
			remove { this.ModifiedChanged -= value; }
		}

		[ValidateNotNull]
		public string Name
		{
			get { return _cannedTextDetail.Name; }
			set
			{
				_cannedTextDetail.Name = value;
				this.Modified = true;
			}
		}

		public string Path
		{
			get { return _cannedTextDetail.Path; }
			set
			{
				_cannedTextDetail.Path = value;
				this.Modified = true;
			}
		}

		[ValidateNotNull]
		public string Text
		{
			get { return _cannedTextDetail.Text; }
			set
			{
				_cannedTextDetail.Text = value;
				this.Modified = true;
			}
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

		#endregion
	}
}
