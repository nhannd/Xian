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
    [ImexDataClass("Modality")]
    public class ModalityImex : XmlEntityImex<Modality, ModalityImex.ModalityData>
    {
        [DataContract]
		public class ModalityData : ReferenceEntityDataBase
        {
            [DataMember]
            public string Id;

            [DataMember]
            public string Name;
		}

        #region Overrides

        protected override IList<Modality> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
        {
            ModalitySearchCriteria where = new ModalitySearchCriteria();
            where.Id.SortAsc(0);

            return context.GetBroker<IModalityBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
        }

        protected override ModalityData Export(Modality entity, IReadContext context)
        {
            ModalityData data = new ModalityData();
			data.Deactivated = entity.Deactivated;
			data.Id = entity.Id;
            data.Name = entity.Name;

            return data;
        }

        protected override void Import(ModalityData data, IUpdateContext context)
        {
            Modality m = LoadOrCreateModality(data.Id, data.Name, context);
        	m.Deactivated = data.Deactivated;
			m.Name = data.Name;
        }

        #endregion

        private Modality LoadOrCreateModality(string id, string name, IPersistenceContext context)
        {
            Modality pt;
            try
            {
                // see if already exists in db
                ModalitySearchCriteria where = new ModalitySearchCriteria();
                where.Id.EqualTo(id);
                pt = context.GetBroker<IModalityBroker>().FindOne(where);
            }
            catch (EntityNotFoundException)
            {
                // create it
                pt = new Modality(id, name);
                context.Lock(pt, DirtyState.New);
            }

            return pt;
        }
    }
}
