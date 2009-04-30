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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core.Imex;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Imex
{
    [ExtensionOf(typeof(XmlDataImexExtensionPoint))]
    [ImexDataClass("Facility")]
    public class FacilityImex : XmlEntityImex<Facility, FacilityImex.FacilityData>
    {
        [DataContract]
		public class FacilityData : ReferenceEntityDataBase
        {
            [DataMember]
            public string Code;

            [DataMember]
            public string Name;

            [DataMember]
            public string InformationAuthority;
        }

        #region Overrides

        protected override IList<Facility> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
        {
            FacilitySearchCriteria where = new FacilitySearchCriteria();
            where.Code.SortAsc(0);
            return context.GetBroker<IFacilityBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
        }

        protected override FacilityData Export(Facility entity, IReadContext context)
        {
            FacilityData data = new FacilityData();
			data.Deactivated = entity.Deactivated;
			data.Code = entity.Code;
            data.Name = entity.Name;
            data.InformationAuthority = entity.InformationAuthority.Code;

            return data;
        }

        protected override void Import(FacilityData data, IUpdateContext context)
        {
            InformationAuthorityEnum ia =
                context.GetBroker<IEnumBroker>().Find<InformationAuthorityEnum>(data.InformationAuthority);

            Facility f = LoadOrCreateFacility(data.Code, data.Name, ia, context);
        	f.Deactivated = data.Deactivated;
            f.Name = data.Name;
            f.InformationAuthority = ia;
        }

        #endregion

        private Facility LoadOrCreateFacility(string code, string name, InformationAuthorityEnum ia, IPersistenceContext context)
        {
            Facility pt;
            try
            {
                // see if already exists in db
                FacilitySearchCriteria where = new FacilitySearchCriteria();
                where.Code.EqualTo(code);
                pt = context.GetBroker<IFacilityBroker>().FindOne(where);
            }
            catch (EntityNotFoundException)
            {
                // create it
                pt = new Facility(code, name, ia);
                context.Lock(pt, DirtyState.New);
            }

            return pt;
        }
    }
}
