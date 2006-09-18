using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Location entity
    /// </summary>
	public partial class Location : Entity
	{
		/// <summary>
		/// Factory method
		/// </summary>
		public static Location New()
		{
			// add any object initialization code here
			// the signature of the New() method may be freely changed as needed
			return new Location();
		}
	}
}