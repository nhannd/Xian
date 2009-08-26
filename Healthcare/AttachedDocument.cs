using System;
using System.Collections;
using System.Text;

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// AttachedDocument entity
    /// </summary>
	public partial class AttachedDocument : ClearCanvas.Enterprise.Core.Entity
	{
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		/// <summary>
		/// Marks this document as having been attached.
		/// </summary>
		public virtual void Attach()
		{
			return;
		}

		/// <summary>
		/// Marks this document as being detached.
		/// </summary>
		public virtual void Detach()
		{
			
		}

		/// <summary>
		/// Summary of derived-class specific details of the attached document
		/// </summary>
		public virtual string MetaDataSummary
		{
			get { return string.Empty; }
		}
	}
}