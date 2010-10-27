#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
