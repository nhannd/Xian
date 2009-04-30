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

namespace ClearCanvas.Enterprise.Hibernate.Hql
{
    public enum HqlJoinMode
    {
        Inner,
        Right,
        Left
    }

    public class HqlJoin : HqlElement
    {
        private readonly string _alias;
        private readonly string _source;
        private readonly HqlJoinMode _mode;
        private readonly bool _fetch;

        public HqlJoin(string source, string alias)
            :this(source, alias, HqlJoinMode.Inner, false)
        {
        }

        public HqlJoin(string source, string alias, HqlJoinMode mode)
            : this(source, alias, mode, false)
        {
        }

        public HqlJoin(string source, string alias, HqlJoinMode mode, bool fetch)
        {
            _source = source;
            _alias = alias;
            _mode = mode;
            _fetch = fetch;
        }

    	public string Alias
    	{
    		get { return _alias; }
    	}

    	public string Source
    	{
    		get { return _source; }
    	}

    	public HqlJoinMode Mode
    	{
    		get { return _mode; }
    	}

    	public bool Fetch
    	{
    		get { return _fetch; }
    	}

    	public override string Hql
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (_mode != HqlJoinMode.Inner)
                    sb.AppendFormat("{0} ", _mode.ToString().ToLower());

                sb.Append("join ");
                if (_fetch)
                    sb.Append("fetch ");
                sb.Append(_source);
                if (!string.IsNullOrEmpty(_alias))
                    sb.AppendFormat(" {0}", _alias);

                return sb.ToString();
            }
        }
    }
}
