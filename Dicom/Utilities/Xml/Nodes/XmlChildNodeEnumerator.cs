﻿#region License

//Copyright (C)  2012 Aaron Boxer

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace ClearCanvas.Dicom.Utilities.Xml.Nodes
{
    public class XmlChildNodeEnumerator : IEnumerator<INode>
    {
        private readonly IEnumerator _nodeEnumerator;
        private bool _valid;

        public XmlChildNodeEnumerator(XmlNode parentNode)
        {
            if (parentNode.HasChildNodes)
                _nodeEnumerator = parentNode.ChildNodes.GetEnumerator();
        }


        public bool MoveNext()
        {
            if (_nodeEnumerator != null)
                _valid = _nodeEnumerator.MoveNext();
            return _valid;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public INode Current
        {
            get { return _valid ? new XmlNodeWrapper((XmlNode)_nodeEnumerator.Current) : null; }
        }
        object IEnumerator.Current
        {
            get { return Current; }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}