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

using ClearCanvas.Common;
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
