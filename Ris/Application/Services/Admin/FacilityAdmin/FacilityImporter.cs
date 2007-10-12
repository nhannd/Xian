#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Application.Services.Admin.FacilityAdmin
{
    [ExtensionOf(typeof(DataImporterExtensionPoint), Name = "Facility Importer")]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class FacilityImporter : DataImporterBase
    {
        public FacilityImporter()
        {

        }

        public override bool SupportsCsv
        {
            get { return true; }
        }

        public override void ImportCsv(List<string> rows, IUpdateContext context)
        {
           List<Facility> facilities = new List<Facility>();

           foreach (string line in rows)
            {
                // expect 2 fields in the row
                string[] fields = ParseCsv(line, 2);

                string id = fields[0];
                string name = fields[1];

                // first check if we have it in memory
                Facility facility = CollectionUtils.SelectFirst<Facility>(facilities,
                    delegate(Facility f) { return f.Code == id && f.Name == name; });

                // if not, check the database
                if (facility == null)
                {
                    FacilitySearchCriteria where = new FacilitySearchCriteria();
                    where.Code.EqualTo(id);
                    where.Name.EqualTo(name);

                    IFacilityBroker broker = context.GetBroker<IFacilityBroker>();
                    facility = CollectionUtils.FirstElement<Facility>(broker.Find(where));

                    // if not, create a new instance
                    if (facility == null)
                    {
                        facility = new Facility(id, name);
                        context.Lock(facility, DirtyState.New);
                    }

                    facilities.Add(facility);
                }
            }
        }
    }
}
