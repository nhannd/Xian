#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	/// <summary>
	/// Implementation of <see cref="IPatientBroker"/>. See PatientBroker.hbm.xml for queries.
	/// </summary>
	public partial class PatientBroker : IPatientBroker
	{
		#region IPatientBroker Members

		public Patient FindDocumentOwner(AttachedDocument document)
		{
			var q = this.GetNamedHqlQuery("documentPatientOwner");
			q.SetParameter(0, document);
			return (Patient) q.UniqueResult();
		}

		#endregion
	}
}
