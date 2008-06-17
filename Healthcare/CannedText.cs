using System;
using System.Collections;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// CannedText entity
    /// </summary>
	[UniqueKey("CannedTextId", new string[] { "Name", "Category", "Staff", "StaffGroup" })]
	public partial class CannedText : ClearCanvas.Enterprise.Core.Entity
	{
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}