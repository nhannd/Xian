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
    public class HqlFrom : HqlElement
    {
        private readonly string _alias;
        private readonly string _entityClass;
        private readonly List<HqlJoin> _joins;


        public HqlFrom(string entityClass, string alias)
            :this(entityClass, alias, new HqlJoin[]{})
        {
        }

        public HqlFrom(string entityClass, string alias, IEnumerable<HqlJoin> joins)
        {
            _alias = alias;
            _entityClass = entityClass;
            _joins = new List<HqlJoin>(joins);
        }

        public List<HqlJoin> Joins
        {
            get { return _joins; }
        }

        public override string Hql
        {
            get
            {
                // build the joins clause
                StringBuilder joins = new StringBuilder();
                foreach (HqlJoin j in _joins)
                {
                    joins.Append(" ");
                    joins.AppendLine(j.Hql);
                }

                return string.Format("{0} {1}\n{2}", _entityClass, _alias, joins);
            }
        }
    }
}
