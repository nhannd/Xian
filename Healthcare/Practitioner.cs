using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Practitioner entity
    /// </summary>
	public partial class Practitioner : Staff
	{
		/// <summary>
		/// Factory method
		/// </summary>
		public static new Practitioner New()
		{
			// add any object initialization code here
			// the signature of the New() method may be freely changed as needed
			return new Practitioner();
		}
	}
}