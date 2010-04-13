using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class WorklistPrintViewComponent : DHtmlComponent
	{
		// Internal data contract used for jscript deserialization
		[DataContract]
		public class PrintContext : DataContractBase
		{
			public PrintContext(string folderSystemName, string folderName, string folderDescription, int totalItemCount, List<object> items)
			{
				this.PrintedBy = LoginSession.Current.FullName;
				this.PrintedTime = Platform.Time;

				this.FolderSystemName = folderSystemName;
				this.FolderName = folderName;
				this.FolderDescription = folderDescription;
				this.TotalItemCount = totalItemCount;
				this.Items = items;
			}

			[DataMember]
			public PersonNameDetail PrintedBy;

			[DataMember]
			public DateTime PrintedTime;

			[DataMember]
			public string FolderSystemName;

			[DataMember]
			public string FolderName;

			[DataMember]
			public string FolderDescription;

			[DataMember]
			public int TotalItemCount;

			[DataMember]
			public List<object> Items;
		}

		private readonly PrintContext _context;

		public WorklistPrintViewComponent(PrintContext context)
		{
			_context = context;
		}

		public override void Start()
		{
			SetUrl(this.PageUrl);
			base.Start();
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _context;
		}

		protected string PageUrl
		{
			get { return WebResourcesSettings.Default.WorklistPrintPreviewPageUrl; }
		}
	}
}
