#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
	public class ResultRecipientAssembler
	{
		public ResultRecipientDetail CreateResultRecipientDetail(ResultRecipient r, IPersistenceContext context)
		{
			ExternalPractitionerAssembler pracAssembler = new ExternalPractitionerAssembler();

			return new ResultRecipientDetail(
				pracAssembler.CreateExternalPractitionerSummary(r.PractitionerContactPoint.Practitioner, context),
				pracAssembler.CreateExternalPractitionerContactPointDetail(r.PractitionerContactPoint, context),
				EnumUtils.GetEnumValueInfo(r.PreferredCommunicationMode, context));
		}
	}
}