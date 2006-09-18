using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// CompositeIdentifier component
    /// </summary>
	public partial class CompositeIdentifier
	{
		/// <summary>
		/// Factory method
		/// </summary>
		public static CompositeIdentifier New()
		{
            // add any object initialization code here
            // the signature of the New() method may be freely changed as needed
            CompositeIdentifier patientIdentifier = new CompositeIdentifier();
            patientIdentifier.AssigningAuthority = "UHN";
            return patientIdentifier;
        }

        public void CopyFrom(CompositeIdentifier source)
        {
            _id = source.Id;
            _assigningAuthority = source.AssigningAuthority;
        }
	}
}