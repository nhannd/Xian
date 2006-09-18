using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Staff entity
    /// </summary>
	public partial class Staff : Entity
	{
		/// <summary>
		/// Factory method
		/// </summary>
		public static Staff New()
		{
			// add any object initialization code here
			// the signature of the New() method may be freely changed as needed
			return new Staff();
		}
	}
}