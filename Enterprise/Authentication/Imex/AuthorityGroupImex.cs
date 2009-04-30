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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication.Brokers;

namespace ClearCanvas.Enterprise.Authentication.Imex
{
    [ExtensionOf(typeof(XmlDataImexExtensionPoint))]
    [ImexDataClass("AuthorityGroup")]
    public class AuthorityGroupImex : XmlEntityImex<AuthorityGroup, AuthorityGroupImex.AuthorityGroupData>
    {
        [DataContract]
        public class AuthorityGroupData
        {
            [DataMember]
            public string Name;

            [DataMember]
            public List<string> Tokens;
        }


        #region Overrides

        protected override IList<AuthorityGroup> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
        {
            AuthorityGroupSearchCriteria where = new AuthorityGroupSearchCriteria();
            where.Name.SortAsc(0);
            return context.GetBroker<IAuthorityGroupBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
        }

        protected override AuthorityGroupData Export(AuthorityGroup group, IReadContext context)
        {
            AuthorityGroupData data = new AuthorityGroupData();
            data.Name = group.Name;
            data.Tokens = CollectionUtils.Map<AuthorityToken, string>(
                group.AuthorityTokens,
                delegate(AuthorityToken token)
                {
                    return token.Name;
                });

            return data;
        }

        protected override void Import(AuthorityGroupData data, IUpdateContext context)
        {
            AuthorityGroup group = LoadOrCreateGroup(data.Name, context);
            if (data.Tokens != null)
            {
                foreach (string token in data.Tokens)
                {
                    AuthorityTokenSearchCriteria where = new AuthorityTokenSearchCriteria();
                    where.Name.EqualTo(token);

                    AuthorityToken authToken = CollectionUtils.FirstElement(context.GetBroker<IAuthorityTokenBroker>().Find(where));
                    if (authToken != null)
                        group.AuthorityTokens.Add(authToken);
                }
            }
        }

        #endregion


        private AuthorityGroup LoadOrCreateGroup(string name, IPersistenceContext context)
        {
            AuthorityGroup group = null;

            try
            {
                AuthorityGroupSearchCriteria criteria = new AuthorityGroupSearchCriteria();
                criteria.Name.EqualTo(name);

                IAuthorityGroupBroker broker = context.GetBroker<IAuthorityGroupBroker>();
                group = broker.FindOne(criteria);
            }
            catch (EntityNotFoundException)
            {
                group = new AuthorityGroup();
                group.Name = name;
                context.Lock(group, DirtyState.New);
            }
            return group;
        }
    }
}

