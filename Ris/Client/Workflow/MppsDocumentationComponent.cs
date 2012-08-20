#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="MppsDocumentationComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class MppsDocumentationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// MppsDocumentationComponent class
	/// </summary>
	[AssociateView(typeof(MppsDocumentationComponentViewExtensionPoint))]
	public class MppsDocumentationComponent : ApplicationComponent, IPerformedStepEditorPage
	{
		private readonly IPerformedStepEditorContext _context;
		private ICannedTextLookupHandler _cannedTextLookupHandler;

		public MppsDocumentationComponent(IPerformedStepEditorContext context)
		{
			_context = context;
		}

		public override void Start()
		{
			_cannedTextLookupHandler = new CannedTextLookupHandler(this.Host.DesktopWindow);

			// when the selected step changes, refresh the browser
			_context.SelectedPerformedStepChanged += delegate
			{
				NotifyPropertyChanged("Comments");
				NotifyPropertyChanged("CommentsEnabled");
			};

			base.Start();
		}

		#region Presentation Model

		public string CommentsLabel
		{
			get
			{ 
				return _context.SelectedPerformedStep == null ? "" 
				: string.Format("Comments for {0}", 
					string.Join("/",_context.SelectedPerformedStep.ModalityProcedureSteps.Select(mps => mps.Description).ToArray()));
			}
		}

		public bool CommentsEnabled
		{
			get { return _context.SelectedPerformedStep != null; }
		}

		public string Comments
		{
			get
			{
				return _context.SelectedPerformedStepExtendedProperties == null ? null : _context.SelectedPerformedStepExtendedProperties["Comments"];
			}
			set
			{
				if (_context.SelectedPerformedStepExtendedProperties != null)
				{
					_context.SelectedPerformedStepExtendedProperties["Comments"] = value;
				}
			}
		}

		public ICannedTextLookupHandler CannedTextLookupHandler
		{
			get { return _cannedTextLookupHandler; }
		}

		#endregion

		#region IPerformedStepEditorPage Members

		Path IExtensionPage.Path
		{
			//todo: loc
			get { return new Path("MPPS Documentation", new ResourceResolver(this.GetType().Assembly)); }
		}

		IApplicationComponent IExtensionPage.GetComponent()
		{
			return this;
		}

		void IPerformedStepEditorPage.Save()
		{
		}

		#endregion
	}
}
