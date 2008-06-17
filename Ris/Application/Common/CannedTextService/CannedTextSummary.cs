using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class CannedTextSummary : DataContractBase
	{
		/// <summary>
		/// Define the maximum length of the TextSnippet
		/// </summary>
		public const int MaxTextLength = 128;

		public CannedTextSummary(EntityRef cannedTextRef
			, CannedTextIdentifierDetail id
			, string textSnippet)
		{
			this.CannedTextRef = cannedTextRef;
			this.Id = id;
			this.TextSnippet = textSnippet.Substring(0, textSnippet.Length < MaxTextLength ? textSnippet.Length : MaxTextLength);
		}

		[DataMember]
		public EntityRef CannedTextRef;

		[DataMember]
		public CannedTextIdentifierDetail Id;

		[DataMember]
		public string TextSnippet;

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
