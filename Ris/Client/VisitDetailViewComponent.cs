using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Client
{
	public abstract class VisitDetailViewComponent : DHtmlComponent
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

		public VisitDetailViewComponent()
			: this(null)
		{
		}

		public VisitDetailViewComponent(EntityRef visitRef)
		{
			_context = visitRef == null ? null : new VisitContext(visitRef);
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

		protected abstract string PageUrl { get; }

		public VisitContext Context
		{
			get { return _context; }
			set
			{
				_context = value;
				NotifyAllPropertiesChanged();
			}
		}
	}
}
