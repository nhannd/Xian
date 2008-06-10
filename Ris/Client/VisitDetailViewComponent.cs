using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Client
{
	public class VisitDetailViewComponent : DHtmlComponent
	{
		// Internal data contract used for jscript deserialization
		[DataContract]
		public class VisitContext : DataContractBase
		{
			public VisitContext(EntityRef visitRef)
			{
				this.VisitRef = visitRef;
			}

			[DataMember]
			public EntityRef VisitRef;
		}

		private VisitContext _context;

		public VisitDetailViewComponent(EntityRef visitRef)
		{
			_context = visitRef == null ? null : new VisitContext(visitRef);
		}

		public override void Start()
		{
			SetUrl(WebResourcesSettings.Default.VisitDetailPageUrl);
			base.Start();
		}

		public void Refresh()
		{
			NotifyAllPropertiesChanged();
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _context;
		}

		public VisitContext Context
		{
			get { return _context; }
			set
			{
				_context = value;
				Refresh();
			}
		}
	}
}
