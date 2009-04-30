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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Healthcare;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Imex
{
    [ExtensionOf(typeof(CsvDataImporterExtensionPoint), Name = "Modality Importer")]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class ModalityImporter : CsvDataImporterBase
    {
        public ModalityImporter()
        {

        }

        public override void Import(List<string> rows, IUpdateContext context)
        {
            List<Modality> modalities = new List<Modality>();
        
            foreach (string line in rows)
            {
                // expect 2 fields in the row
                string[] fields = ParseCsv(line, 2);

                string id = fields[0];
                string name = fields[1];

                // first check if we have it in memory
                Modality modality = CollectionUtils.SelectFirst<Modality>(modalities,
                    delegate(Modality sp) { return sp.Id == id && sp.Name == name; });

                // if not, check the database
                if (modality == null)
                {
                    ModalitySearchCriteria where = new ModalitySearchCriteria();
                    where.Id.EqualTo(id);
                    where.Name.EqualTo(name);

                    IModalityBroker broker = context.GetBroker<IModalityBroker>();
                    modality = CollectionUtils.FirstElement<Modality>(broker.Find(where));

                    // if not, create a new instance
                    if (modality == null)
                    {
                        modality = new Modality(id, name);
                        context.Lock(modality, DirtyState.New);
                    }

                    modalities.Add(modality);
                }
            }
        }
    }
}
