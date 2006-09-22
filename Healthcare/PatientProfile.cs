using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// PatientProfile entity
    /// </summary>
	public partial class PatientProfile : Entity
	{
		/// <summary>
		/// Factory method
		/// </summary>
		public static PatientProfile New()
		{
			// add any object initialization code here
			// the signature of the New() method may be freely changed as needed
			PatientProfile patientProfile = new PatientProfile();
            Patient.New().AddProfile(patientProfile);
            patientProfile._sex = Sex.U;   // default to Unknown
            return patientProfile;
		}

        public virtual DateTime DateOfBirth
        {
            get { return _dateOfBirth.Date; }
            set { _dateOfBirth = value.Date; }
        }
    }
}