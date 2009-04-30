#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Imex
{
	[ExtensionOf(typeof(XmlDataImexExtensionPoint))]
	[ImexDataClass("PatientNoteCategory")]
	public class PatientNoteCategoryImex : XmlEntityImex<PatientNoteCategory, PatientNoteCategoryImex.PatientNoteCategoryData>
	{
		[DataContract]
		public class PatientNoteCategoryData : ReferenceEntityDataBase
		{
			[DataMember]
			public string Name;

			[DataMember]
			public string Description;

			[DataMember]
			public string Severity;
		}

		#region Overrides

		protected override IList<PatientNoteCategory> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
		{
			PatientNoteCategorySearchCriteria where = new PatientNoteCategorySearchCriteria();
			where.Name.SortAsc(0);

			return context.GetBroker<IPatientNoteCategoryBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
		}

		protected override PatientNoteCategoryData Export(PatientNoteCategory entity, IReadContext context)
		{
			PatientNoteCategoryData data = new PatientNoteCategoryData();
			data.Deactivated = entity.Deactivated;
			data.Name = entity.Name;
			data.Description = entity.Description;
			data.Severity = entity.Severity.ToString();

			return data;
		}

		protected override void Import(PatientNoteCategoryData data, IUpdateContext context)
		{
			NoteSeverity severity = (NoteSeverity) Enum.Parse(typeof (NoteSeverity), data.Severity);
			PatientNoteCategory nc = LoadOrCreatePatientNoteCategory(data.Name, severity, context);
			nc.Deactivated = data.Deactivated;
			nc.Description = data.Description;
		}

		#endregion

		private PatientNoteCategory LoadOrCreatePatientNoteCategory(string name, NoteSeverity severity, IPersistenceContext context)
		{
			PatientNoteCategory nc;
			try
			{
				// see if already exists in db
				PatientNoteCategorySearchCriteria where = new PatientNoteCategorySearchCriteria();
				where.Name.EqualTo(name);
				nc = context.GetBroker<IPatientNoteCategoryBroker>().FindOne(where);
			}
			catch (EntityNotFoundException)
			{
				// create it
				nc = new PatientNoteCategory(name, null, severity);
				context.Lock(nc, DirtyState.New);
			}

			return nc;
		}
	}
}
