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
using System.Text.RegularExpressions;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.ImageServer.Enterprise.Admin;

namespace ClearCanvas.ImageServer.Web.Common.Data.DataSource
{

	public class UserGroupDataSource : BaseDataSource
	{
        #region Private Members

		private string _groupName;

        private int _resultCount;

        #endregion Private Members

        #region Public Members
        public delegate void UserGroupFoundSetDelegate(IList<UserGroupRowData> list);
        public UserGroupFoundSetDelegate UserGroupFoundSet;
        #endregion Public Members

        #region Properties
        public int ResultCount
        {
            get { return _resultCount; }
            set { _resultCount = value; }
        }

	    public string GroupName
	    {
            get { return _groupName; }
            set { _groupName = value; }	        
	    }

        #endregion

        #region Private Methods

        private IList<UserGroupRowData> InternalSelect(int startRowIndex, int maximumRows, out int resultCount)
        {
            Array authorityRowData;
            Array authorityRowDataRange = Array.CreateInstance(typeof(UserGroupRowData), maximumRows);

            resultCount = 0;

            if (maximumRows == 0) return new List<UserGroupRowData>();

            using(AuthorityManagement service = new AuthorityManagement())
            {
                IList<AuthorityGroupSummary> list = service.ListAllAuthorityGroups();
                IList<AuthorityGroupSummary> filteredList = new List<AuthorityGroupSummary>();

                if(!string.IsNullOrEmpty(GroupName))
                {
                	string matchString = GroupName;

					while (matchString.StartsWith("*") || matchString.StartsWith("?"))
						matchString = matchString.Substring(1);
					while (matchString.EndsWith("*")||matchString.EndsWith("?"))
						matchString = matchString.Substring(0, matchString.Length - 1);

					matchString = matchString.Replace("*", "[A-Za-z0-9]*");
					matchString = matchString.Replace("?", ".");

                    foreach(AuthorityGroupSummary group in list)
                    {
						if (Regex.IsMatch(group.Name,matchString,RegexOptions.IgnoreCase))
							filteredList.Add(group);
                    }
                } 
				else
                {
                    filteredList = list;
                }

                List<UserGroupRowData> rows = CollectionUtils.Map<AuthorityGroupSummary, UserGroupRowData>(
                    filteredList, delegate(AuthorityGroupSummary group)
                              {
                                  UserGroupRowData row =
                                      new UserGroupRowData(service.LoadAuthorityGroupDetail(group));
                                  return row;
                              });

                authorityRowData = CollectionUtils.ToArray(rows);

                int copyLength = adjustCopyLength(startRowIndex, maximumRows, authorityRowData.Length);

                Array.Copy(authorityRowData, startRowIndex, authorityRowDataRange, 0, copyLength);

                if (copyLength < authorityRowDataRange.Length)
                {
                    authorityRowDataRange = resizeArray(authorityRowDataRange, copyLength);
                }
            };

            resultCount = authorityRowData.Length;

            return CollectionUtils.Cast<UserGroupRowData>(authorityRowDataRange);
        }

        #endregion Private Methods

        #region Public Methods
        public IEnumerable<UserGroupRowData> Select(int startRowIndex, int maximumRows)
        {
            IList<UserGroupRowData> _list = InternalSelect(startRowIndex, maximumRows, out _resultCount);

            if (UserGroupFoundSet != null)
                UserGroupFoundSet(_list);

            return _list;

        }

        public int SelectCount()
        {
            if (ResultCount != 0) return ResultCount;

            // Ignore the search results
            InternalSelect(0, 1, out _resultCount);

            return ResultCount;
        }

        #endregion Public Methods
    }

    [Serializable]
    public class UserGroupRowData
    {
        private string _name;
        private string _ref;
        private int _tokenCount;
        private List<TokenSummary> _tokens = new List<TokenSummary>();

        public UserGroupRowData() {}
        
        public UserGroupRowData(AuthorityGroupDetail group)
        {
            Ref = group.AuthorityGroupRef.Serialize();
            Name = group.Name;

            foreach(AuthorityTokenSummary token in group.AuthorityTokens)
            {
                Tokens.Add(new TokenSummary(token.Name, token.Description));
            }

            TokenCount = group.AuthorityTokens.Count;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Ref
        {
            get { return _ref; }
            set { _ref = value; }
        }

        public int TokenCount
        {
            get { return _tokenCount; }
            set { _tokenCount = value; }
        }

        public List<TokenSummary> Tokens
        {
            get { return _tokens; }
            set { _tokens = value; }
        }
    }

    [Serializable]
    public class TokenSummary
    {
        private string _name;
        private string _description;

        public TokenSummary(string name, string description)
        {
            _name = name;
            _description = description;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }     
    }
}





