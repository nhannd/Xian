using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
	[ExtensionOf(typeof(FolderSystemExtensionPoint))]
	class TestFolderSystem : WorkflowFolderSystem
	{
		private IApplicationComponent _contentComponent;
		public TestFolderSystem()
			: base("Test")
		{
			_contentComponent = new TestComponent("Test");
		}

		public override IApplicationComponent GetContentComponent()
		{
			return _contentComponent;
		}

		public override string GetPreviewUrl(IFolder folder, object[] items)
		{
			return null;
		}

		public override SearchParams CreateSearchParams(string searchText)
		{
			return null;
		}

		public override void LaunchSearchComponent()
		{
		}

		public override Type SearchComponentType
		{
			get { return null; }
		}

		protected override IToolSet CreateItemToolSet()
		{
			return null;
		}

		protected override IToolSet CreateFolderToolSet()
		{
			return null;
		}

		protected override SearchResultsFolder CreateSearchResultsFolder()
		{
			return null;
		}

		protected override IDictionary<string, bool> QueryOperationEnablement(ISelection selection)
		{
			return new Dictionary<string, bool>();
		}

		protected override object SelectDropHandler(IList handlers, object[] items)
		{
			return null;
		}
	}
}
