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
