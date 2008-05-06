using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.CannedTextService;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	[MenuAction("launch", "global-menus/Tools/Canned Text", "Launch")]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class CannedTextTool : Tool<IDesktopToolContext>
	{
		private IShelf _shelf;

		public void Launch()
		{
			try
			{
				if (_shelf == null)
				{
					CannedTextSummaryComponent component = new CannedTextSummaryComponent();

					_shelf = ApplicationComponent.LaunchAsShelf(
						this.Context.DesktopWindow,
						component,
						SR.TitleCannedText, ShelfDisplayHint.DockFloat);

					_shelf.Closed += delegate { _shelf = null; };
				}
				else
				{
					_shelf.Activate();
				}
			}
			catch (Exception e)
			{
				// could not launch component
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}

	/// <summary>
	/// Extension point for views onto <see cref="CannedTextSummaryComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class CannedTextSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// CannedTextSummaryComponent class
	/// </summary>
	[AssociateView(typeof(CannedTextSummaryComponentViewExtensionPoint))]
	public class CannedTextSummaryComponent : ApplicationComponent
	{
		private CannedTextTable _cannedTextTable;
		private SimpleActionModel _cannedTextActionHandler;
		private CannedTextSummary _selectedCannedText;

		private static readonly object _addKey = new object();
		private static readonly object _editKey = new object();
		private static readonly object _deleteKey = new object();
		private static readonly object _copyKey = new object();

		private EventHandler _copyCannedTextRequested;

		public override void Start()
		{
			_cannedTextTable = new CannedTextTable();
			_cannedTextActionHandler = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
			_cannedTextActionHandler.AddAction(_addKey, SR.TitleAdd, "Icons.AddToolSmall.png", AddCannedText);
			_cannedTextActionHandler.AddAction(_editKey, SR.TitleEdit, "Icons.EditToolSmall.png", EditCannedText);
			_cannedTextActionHandler.AddAction(_deleteKey, SR.TitleDelete, "Icons.DeleteToolSmall.png", DeleteCannedText);
			_cannedTextActionHandler.AddAction(_copyKey, SR.TitleCopy, "Icons.CopyToolSmall.png", CopyCannedText);

			_cannedTextActionHandler[_editKey].Enabled = false;
			_cannedTextActionHandler[_deleteKey].Enabled = false;


			List<CannedTextSummary> cannedTexts = new List<CannedTextSummary>();
			Platform.GetService<ICannedTextService>(
				delegate(ICannedTextService service)
					{
						ListCannedTextResponse response = service.ListCannedText(new ListCannedTextRequest());
						cannedTexts = response.CannedTexts;
					});

			_cannedTextTable.Items.Clear();
			_cannedTextTable.Items.AddRange(cannedTexts);

			base.Start();
		}

		#region Presentation Model

		public ITable CannedTextTable
		{
			get { return _cannedTextTable; }
		}

		public ActionModelNode CannedTextActionModel
		{
			get { return _cannedTextActionHandler; }
		}

		public ISelection SelectedCannedText
		{
			get { return new Selection(_selectedCannedText); }
			set
			{
				if (value != _selectedCannedText)
				{
					_selectedCannedText = (CannedTextSummary)value.Item;
					CannedTextSelectionChanged();
				}
			}
		}

		public string Text
		{
			get { return _selectedCannedText == null ? null : _selectedCannedText.Text; }
		}

		public event EventHandler CopyCannedTextRequested
		{
			add { _copyCannedTextRequested += value; }
			remove { _copyCannedTextRequested -= value; }
		}
		public void CopyCannedText()
		{
			EventsHelper.Fire(_copyCannedTextRequested, this, EventArgs.Empty);
		}

		public void AddCannedText()
		{
			try
			{
				CannedTextDetail detail = new CannedTextDetail();

				CannedTextEditorComponent editor = new CannedTextEditorComponent(detail);
				ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddCannedText);

				if (exitCode == ApplicationComponentExitCode.Accepted)
				{
					CannedTextSummary updatedSummary = null;

					Platform.GetService<ICannedTextService>(
						delegate(ICannedTextService service)
						{
							AddCannedTextRequest request = new AddCannedTextRequest(detail);
							AddCannedTextResponse response = service.AddCannedText(request);
							updatedSummary = response.CannedTextSummary;
						});

					_cannedTextTable.Items.Add(updatedSummary);
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public void EditCannedText()
		{
			try
			{
				CannedTextDetail detail = new CannedTextDetail(_selectedCannedText.Name, _selectedCannedText.Path, _selectedCannedText.Text);

				CannedTextEditorComponent editor = new CannedTextEditorComponent(detail);
				ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdateCannedText);

				if (exitCode == ApplicationComponentExitCode.Accepted)
				{
					CannedTextSummary updatedSummary = null;

					Platform.GetService<ICannedTextService>(
						delegate(ICannedTextService service)
						{
							UpdateCannedTextRequest request = new UpdateCannedTextRequest(_selectedCannedText.CannedTextRef, detail);
							UpdateCannedTextResponse response = service.UpdateCannedText(request);
							updatedSummary = response.CannedTextSummary;
						});

					_cannedTextTable.Items.Replace(
						delegate(CannedTextSummary c) { return c.CannedTextRef.Equals(updatedSummary.CannedTextRef, true); },
						updatedSummary);
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public void DeleteCannedText()
		{
			try 
			{
				Platform.GetService<ICannedTextService>(
					delegate(ICannedTextService service)
						{
							DeleteCannedTextRequest request = new DeleteCannedTextRequest(_selectedCannedText.CannedTextRef);
							service.DeleteCannedText(request);
						});

				_cannedTextTable.Items.Remove(_selectedCannedText);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		#endregion

		private void CannedTextSelectionChanged()
		{
			_cannedTextActionHandler[_editKey].Enabled = _cannedTextActionHandler[_deleteKey].Enabled = _selectedCannedText != null;
		}
	}
}
