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
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Authentication.Hibernate.Brokers
{
    public partial class AuthorityTokenBroker
    {
        public string[] FindTokensByUserName(string userName)
        {
            UserSearchCriteria where = new UserSearchCriteria();
            where.UserName.EqualTo(userName);

            // want this to be as fast as possible - use joins and only select the AuthorityToken names
            HqlQuery query = new HqlQuery("select distinct t.Name from User u join u.AuthorityGroups g join g.AuthorityTokens t");
            query.Conditions.AddRange(HqlCondition.FromSearchCriteria("u", where));

            // take advantage of query caching if possible
            query.Cacheable = true;

            IList<string> tokens = this.ExecuteHql<string>(query);
            string[] result = new string[tokens.Count];
            tokens.CopyTo(result, 0);
            return result;
        }

        public bool AssertUserHasToken(string userName, string token)
        {
            UserSearchCriteria whereUser = new UserSearchCriteria();
            whereUser.UserName.EqualTo(userName);

            AuthorityTokenSearchCriteria whereToken = new AuthorityTokenSearchCriteria();
            whereToken.Name.EqualTo(token);

            // want this to be as fast as possible - use joins and only select the count
            HqlQuery query = new HqlQuery("select count(*) from User u join u.AuthorityGroups g join g.AuthorityTokens t");
            query.Conditions.AddRange(HqlCondition.FromSearchCriteria("u", whereUser));
            query.Conditions.AddRange(HqlCondition.FromSearchCriteria("t", whereToken));

            // expect exactly one integer result
            return ExecuteHqlUnique<long>(query) > 0;
        }

    }
}
