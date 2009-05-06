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
    [ImexDataClass("User")]
    public class UserImex : XmlEntityImex<User, UserImex.UserData>
    {
        [DataContract]
        public class UserData
        {
            [DataMember]
            public string UserName;

            [DataMember]
            public string DisplayName;

            [DataMember]
            public DateTime? ValidFrom;

            [DataMember]
            public DateTime? ValidUntil;

            [DataMember]
            public bool Enabled;

            [DataMember]
            public List<string> AuthorityGroups;
        }

		private readonly AuthenticationSettings _settings = new AuthenticationSettings();


        #region Overrides

        protected override IList<User> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
        {
            UserSearchCriteria where = new UserSearchCriteria();
            where.UserName.SortAsc(0);
            return context.GetBroker<IUserBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
        }

        protected override UserData Export(User user, IReadContext context)
        {
            UserData data = new UserData();
            data.UserName = user.UserName;
            data.DisplayName = user.DisplayName;
            data.ValidFrom = user.ValidFrom;
            data.ValidUntil = user.ValidUntil;
            data.Enabled = user.Enabled;
            data.AuthorityGroups = CollectionUtils.Map<AuthorityGroup, string>(
                user.AuthorityGroups,
                delegate(AuthorityGroup group)
                {
                    return group.Name;
                });

            return data;
        }

        protected override void Import(UserData data, IUpdateContext context)
        {
            UserInfo info = new UserInfo(data.UserName, data.DisplayName, data.ValidFrom, data.ValidUntil);
            User user = LoadOrCreateUser(info, context);
            user.Enabled = data.Enabled;

            if (data.AuthorityGroups != null)
            {
                foreach (string group in data.AuthorityGroups)
                {
                    AuthorityGroupSearchCriteria where = new AuthorityGroupSearchCriteria();
                    where.Name.EqualTo(group);

                    AuthorityGroup authGroup = CollectionUtils.FirstElement(context.GetBroker<IAuthorityGroupBroker>().Find(where));
                    if (authGroup != null)
                        user.AuthorityGroups.Add(authGroup);
                }
            }
        }

        #endregion


        private User LoadOrCreateUser(UserInfo info, IPersistenceContext context)
        {
            User user = null;

            try
            {
                UserSearchCriteria criteria = new UserSearchCriteria();
                criteria.UserName.EqualTo(info.UserName);

                IUserBroker broker = context.GetBroker<IUserBroker>();
                user = broker.FindOne(criteria);
            }
            catch (EntityNotFoundException)
            {
				user = User.CreateNewUser(info, _settings.DefaultTemporaryPassword);
                context.Lock(user, DirtyState.New);
            }
            return user;
        }
    }
}

