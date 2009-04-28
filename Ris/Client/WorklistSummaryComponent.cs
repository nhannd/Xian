using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Client
{
	public class WorklistSummaryComponent : DHtmlComponent
	{
		[DataContract]
		internal class WorklistSummaryContext : DataContractBase
		{
			private readonly WorklistAdminDetail _worklist;
			private readonly bool _isAdmin;

			private bool _hasMultipleWorklists;
			private List<string> _worklistNames;
			private List<string> _worklistDescriptions;
			private List<WorklistClassSummary> _worklistClasses;

			public WorklistSummaryContext(WorklistAdminDetail worklist, bool isAdmin)
			{
				_worklist = worklist;
				_isAdmin = isAdmin;
			}

			[DataMember]
			public bool IsAdmin
			{
				get { return _isAdmin; }
			}

			[DataMember]
			public WorklistAdminDetail Worklist
			{
				get { return _worklist; }
			}

			[DataMember]
			public bool HasMultipleWorklists
			{
				get { return _hasMultipleWorklists; }
				set { _hasMultipleWorklists = value; }
			}

			[DataMember]
			public List<string> WorklistNames
			{
				get { return _worklistNames; }
				set { _worklistNames = value; }
			}

			[DataMember]
			public List<string> WorklistDescriptions
			{
				get { return _worklistDescriptions; }
				set { _worklistDescriptions = value; }
			}

			[DataMember]
			public List<WorklistClassSummary> WorklistClasses
			{
				get { return _worklistClasses; }
				set { _worklistClasses = value; }
			}
		}

		private readonly WorklistSummaryContext _context;

		public WorklistSummaryComponent(WorklistAdminDetail worklist, bool isAdmin)
		{
			_context = new WorklistSummaryContext(worklist, isAdmin);
		}

		public override void Start()
		{
			this.SetUrl(WebResourcesSettings.Default.WorklistSummaryPageUrl);
			base.Start();
		}

		public void SetMultipleWorklistInfo(List<string> names, List<string> descriptions, List<WorklistClassSummary> classes)
		{
			_context.HasMultipleWorklists = true;
			_context.WorklistNames = names;
			_context.WorklistDescriptions = descriptions;
			_context.WorklistClasses = classes;
		}

		public void Refresh()
		{
			NotifyAllPropertiesChanged();
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _context;
		}
	}
}
