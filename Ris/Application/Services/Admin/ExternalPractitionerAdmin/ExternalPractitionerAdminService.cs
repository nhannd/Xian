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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common;
using System.Security.Permissions;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;

namespace ClearCanvas.Ris.Application.Services.Admin.ExternalPractitionerAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IExternalPractitionerAdminService))]
    public class ExternalPractitionerAdminService : ApplicationServiceBase, IExternalPractitionerAdminService
    {
        #region IExternalPractitionerAdminService Members

        [ReadOperation]
        // note: this operation is not protected with ClearCanvas.Ris.Application.Common.AuthorityTokens.ExternalPractitionerAdmin
        // because it is used in non-admin situations - perhaps we need to create a separate operation???
        public ListExternalPractitionersResponse ListExternalPractitioners(ListExternalPractitionersRequest request)
        {
            ExternalPractitionerAssembler assembler = new ExternalPractitionerAssembler();

            ExternalPractitionerSearchCriteria criteria = new ExternalPractitionerSearchCriteria();
            if (!string.IsNullOrEmpty(request.FirstName))
                criteria.Name.GivenName.StartsWith(request.FirstName);
            if (!string.IsNullOrEmpty(request.LastName))
                criteria.Name.FamilyName.StartsWith(request.LastName);

            return new ListExternalPractitionersResponse(
                CollectionUtils.Map<ExternalPractitioner, ExternalPractitionerSummary, List<ExternalPractitionerSummary>>(
                    PersistenceContext.GetBroker<IExternalPractitionerBroker>().Find(criteria, request.Page),
                    delegate(ExternalPractitioner s)
                    {
                        return assembler.CreateExternalPractitionerSummary(s, PersistenceContext);
                    }));
        }

        [ReadOperation]
        //[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.ExternalPractitionerAdmin)]
        public LoadExternalPractitionerForEditResponse LoadExternalPractitionerForEdit(LoadExternalPractitionerForEditRequest request)
        {
            // note that the version of the ExternalPractitionerRef is intentionally ignored here (default behaviour of ReadOperation)
            ExternalPractitioner s = PersistenceContext.Load<ExternalPractitioner>(request.PractitionerRef);
            ExternalPractitionerAssembler assembler = new ExternalPractitionerAssembler();

            return new LoadExternalPractitionerForEditResponse(s.GetRef(), assembler.CreateExternalPractitionerDetail(s, this.PersistenceContext));
        }

        [ReadOperation]
        public LoadExternalPractitionerEditorFormDataResponse LoadExternalPractitionerEditorFormData(LoadExternalPractitionerEditorFormDataRequest request)
        {
            //TODO:  replace "dummy" lists
            List<string> dummyCountries = new List<string>();
            dummyCountries.Add("Canada");

            List<string> dummyProvinces = new List<string>();
            dummyProvinces.Add("Ontario");

            return new LoadExternalPractitionerEditorFormDataResponse(
                EnumUtils.GetEnumValueList<AddressTypeEnum>(PersistenceContext),
                dummyProvinces,
                dummyCountries,
                (new SimplifiedPhoneTypeAssembler()).GetSimplifiedPhoneTypeChoices(false),
                EnumUtils.GetEnumValueList<PractitionerLicenseAuthorityEnum>(PersistenceContext)
                );

        }

        [UpdateOperation]
        //[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.ExternalPractitionerAdmin)]
        public AddExternalPractitionerResponse AddExternalPractitioner(AddExternalPractitionerRequest request)
        {
            ExternalPractitioner prac = new ExternalPractitioner();

            ExternalPractitionerAssembler assembler = new ExternalPractitionerAssembler();
            assembler.UpdateExternalPractitioner(request.PractitionerDetail, prac, PersistenceContext);

            PersistenceContext.Lock(prac, DirtyState.New);

            // ensure the new prac is assigned an OID before using it in the return value
            PersistenceContext.SynchState();

            return new AddExternalPractitionerResponse(assembler.CreateExternalPractitionerSummary(prac, PersistenceContext));
        }

        [UpdateOperation]
        //[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.ExternalPractitionerAdmin)]
        public UpdateExternalPractitionerResponse UpdateExternalPractitioner(UpdateExternalPractitionerRequest request)
        {
            ExternalPractitioner prac = PersistenceContext.Load<ExternalPractitioner>(request.PractitionerRef, EntityLoadFlags.CheckVersion);

            ExternalPractitionerAssembler assembler = new ExternalPractitionerAssembler();
            assembler.UpdateExternalPractitioner(request.PractitionerDetail, prac, PersistenceContext);

            return new UpdateExternalPractitionerResponse(assembler.CreateExternalPractitionerSummary(prac, PersistenceContext));
        }

        [ReadOperation]
        public TextQueryResponse<ExternalPractitionerSummary> TextQuery(TextQueryRequest request)
        {
            IExternalPractitionerBroker broker = PersistenceContext.GetBroker<IExternalPractitionerBroker>();
            ExternalPractitionerAssembler assembler = new ExternalPractitionerAssembler();

            TextQueryHelper<ExternalPractitioner, ExternalPractitionerSearchCriteria, ExternalPractitionerSummary> helper
                = new TextQueryHelper<ExternalPractitioner, ExternalPractitionerSearchCriteria, ExternalPractitionerSummary>(
                    delegate(string rawQuery)
                    {
                        List<ExternalPractitionerSearchCriteria> criteria = new List<ExternalPractitionerSearchCriteria>();

                        // build criteria against names
                        PersonName[] names = TextQueryHelper.ParsePersonNames(rawQuery);
                        criteria.AddRange(CollectionUtils.Map<PersonName, ExternalPractitionerSearchCriteria>(names,
                            delegate(PersonName n)
                            {
                                ExternalPractitionerSearchCriteria sc = new ExternalPractitionerSearchCriteria();
                                sc.Name.FamilyName.StartsWith(n.FamilyName);
                                if (n.GivenName != null)
                                    sc.Name.GivenName.StartsWith(n.GivenName);
                                return sc;
                            }));

                        // build criteria against identifiers
                        string[] ids = TextQueryHelper.ParseIdentifiers(rawQuery);
                        criteria.AddRange(CollectionUtils.Map<string, ExternalPractitionerSearchCriteria>(ids,
                                     delegate(string word)
                                     {
                                         ExternalPractitionerSearchCriteria c = new ExternalPractitionerSearchCriteria();
                                         c.LicenseNumber.Id.StartsWith(word);
                                         return c;
                                     }));

                        return criteria.ToArray();
                    },
                    delegate(ExternalPractitioner prac)
                    {
                        return assembler.CreateExternalPractitionerSummary(prac, PersistenceContext);
                    },
                    delegate(ExternalPractitionerSearchCriteria[] criteria)
                    {
                        return broker.Count(criteria);
                    },
                    delegate(ExternalPractitionerSearchCriteria[] criteria, SearchResultPage page)
                    {
                        return broker.Find(criteria, page);
                    });

            return helper.Query(request);
        }

        #endregion

    }
}
