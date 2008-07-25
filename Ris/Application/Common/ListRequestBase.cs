using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class ListRequestBase : PagedDataContractBase
	{
		public ListRequestBase()
		{
		}


		public ListRequestBase(SearchResultPage page)
			: base(page)
		{
		}

		/// <summary>
		/// Specifies whether to include de-activated items in the results.  This value is ignored
		/// if the entity does not support the notion of de-activation.
		/// </summary>
		[DataMember]
		public bool IncludeDeactivated;
	}
}
