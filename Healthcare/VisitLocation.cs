using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// VisitLocation entity
    /// </summary>
	public partial class VisitLocation : Entity
	{
		/// <summary>
		/// Factory method
		/// </summary>
		public static VisitLocation New()
		{
			// add any object initialization code here
			// the signature of the New() method may be freely changed as needed
			return new VisitLocation();
		}
	}
}