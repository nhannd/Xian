using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class CannedTextDetail : DataContractBase
	{
		public CannedTextDetail()
		{
			this.Id = new CannedTextIdentifierDetail();
		}

		public CannedTextDetail(CannedTextIdentifierDetail id, string text)
		{
			this.Id = id;
			this.Text = text;
		}

		[DataMember]
		public CannedTextIdentifierDetail Id;

		[DataMember]
		public string Text;

		public bool IsPersonal
		{
			get { return this.Id.Staff != null; }
		}

		public bool IsGroup
		{
			get { return this.Id.StaffGroup != null; }
		}
	}
}
