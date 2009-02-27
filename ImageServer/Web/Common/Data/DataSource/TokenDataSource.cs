#region License

// Copyright (c) 2006-2009, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.ImageServer.Common.Services.Admin;

namespace ClearCanvas.ImageServer.Web.Common.Data.DataSource
{
	public class TokenDataSource : BaseDataSource
	{
        #region Private Members
        private UserManagementController _controller = new UserManagementController();
        private IList<TokenRowData> _list = null;
        private int _resultCount;
        #endregion Private Members

        #region Public Members
        public delegate void TokenFoundSetDelegate(IList<TokenRowData> list);
        public TokenFoundSetDelegate TokenFoundSet;
        #endregion Public Members

        #region Properties
        public int ResultCount
        {
            get { return _resultCount; }
            set { _resultCount = value; }
        }
        #endregion

        #region Private Methods

        private IList<TokenRowData> InternalSelect(int startRowIndex, int maximumRows, out int resultCount)
        {
            Array tokenRowData = null;
            Array tokenRowDataRange = Array.CreateInstance(typeof(TokenRowData), maximumRows);

            resultCount = 0;

            if (maximumRows == 0) return new List<TokenRowData>();

            Platform.GetService<IAuthorityAdminService>(
                delegate(IAuthorityAdminService services)
                    {
                        IList<AuthorityTokenSummary> tokens = services.ListAuthorityTokens();
                        List<TokenRowData> tokenRows = CollectionUtils.Map<AuthorityTokenSummary, TokenRowData>(
                            tokens, delegate(AuthorityTokenSummary token)
                                   {
                                       TokenRowData row = new TokenRowData(token);
                                       return row;
                                   });

                        tokenRowData = CollectionUtils.ToArray(tokenRows);

                        int copyLength = adjustCopyLength(startRowIndex, maximumRows, tokenRowData.Length);

                        Array.Copy(tokenRowData, startRowIndex, tokenRowDataRange, 0, copyLength);

                        if(copyLength < tokenRowDataRange.Length)
                        {
                            tokenRowDataRange = resizeArray(tokenRowDataRange, copyLength);
                        }
            });

            if (tokenRowData != null)
            {
                resultCount = tokenRowData.Length;
            }

            return CollectionUtils.Cast<TokenRowData>(tokenRowDataRange);
        }

        #endregion Private Methods

        #region Public Methods
        public IEnumerable<TokenRowData> Select(int startRowIndex, int maximumRows)
        {
            IList<TokenRowData> _list = InternalSelect(startRowIndex, maximumRows, out _resultCount);

            if (TokenFoundSet != null)
                TokenFoundSet(_list);

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
    public class TokenRowData
    {
        private string _name;
        private string _description;

        public TokenRowData(AuthorityTokenSummary token)
        {
            Name = token.Name;
            Description = token.Description;
        }

        public TokenRowData()
        {
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