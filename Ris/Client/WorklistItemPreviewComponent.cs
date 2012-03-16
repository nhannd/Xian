#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	[ExtensionPoint]
	public class PreviewToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public interface IPreviewToolContext : IToolContext
	{
		WorklistItemSummaryBase WorklistItem { get; }
		IDesktopWindow DesktopWindow { get; }
	}

	public class WorklistItemPreviewComponent : DHtmlComponent, IPreviewComponent
	{
		class PreviewToolContext : ToolContext, IPreviewToolContext
		{
			private readonly WorklistItemPreviewComponent _component;

			public PreviewToolContext(WorklistItemPreviewComponent component)
			{
				_component = component;
			}

			#region IPreviewToolContext Members

			public WorklistItemSummaryBase WorklistItem
			{
				get { return _component._folderSystemItem as WorklistItemSummaryBase; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			#endregion
		}

		private DataContractBase _folderSystemItem;
		private ToolSet _toolSet;

		public override void Start()
		{
			_toolSet = new ToolSet(new PreviewToolExtensionPoint(), new PreviewToolContext(this));
			base.Start();
		}

		public override void Stop()
		{
			_toolSet.Dispose();
			base.Stop();
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _folderSystemItem;
		}

		#region IPreviewComponent Members

		void IPreviewComponent.SetPreviewItems(string url, object[] items)
		{
			_folderSystemItem = CollectionUtils.FirstElement<DataContractBase>(items);
			SetUrl(url);
		}

		#endregion
	}
}
