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

using System.Collections.Generic;
using System.Xml;

namespace ClearCanvas.Dicom.Utilities.Xml.Nodes
{
    /// <summary>
    /// Wrapper around xml text reader element.
    /// </summary>
    public class ReaderNode : INode
    {
        private readonly IDictionary<string, string> _attributes;
        private readonly int _depth;
        private readonly bool _hasChildNodes;
        private readonly string _name;
        private readonly int _nodeCount;
        private readonly ReaderWrapper _readerWrapper;
        private ReaderChildNodeEnumerator _childEnumerator;
        private string _content;
        private string _innerXml;
        private ReaderNode _firstChild;

        public ReaderNode(ReaderWrapper readerWrapper)
        {
            _readerWrapper = readerWrapper;
            var xmlReader = readerWrapper.GetXmlReader;
            _depth = xmlReader.Depth;
            _nodeCount = readerWrapper.NodeCount;
            _name = xmlReader.LocalName;
            _hasChildNodes = !xmlReader.IsEmptyElement;

            _attributes = readerWrapper.GetAttributes();
        }

        public int Depth
        {
            get { return _depth; }
        }

        public int NodeCount
        {
            get { return _nodeCount; }
        }

        #region INode Members

        public INode FirstChild
        {
            get
            {
                if (!HasChildNodes)
                    return null;
                if (_firstChild == null)
                {
                    _readerWrapper.MoveFirstChild();
                    _firstChild = new ReaderNode(_readerWrapper);
                }
                return _firstChild;
            }
        }

        public bool HasChildNodes
        {
            get { return _hasChildNodes; }
        }

        public string Name
        {
            get { return _name; }
        }

        public bool HasAttribute(string attrName)
        {
            return _attributes != null && _attributes.ContainsKey(attrName);
        }

        public string Attribute(string attrName)
        {
            return HasAttribute(attrName)  ? _attributes[attrName] : null ;
        } 

        public string Content
        {
            get { return _content ?? (_content = _readerWrapper.ReadString()); }
        }

        public string InnerXml
        {
            get { return _innerXml ?? (_innerXml = _readerWrapper.ReadInnerXml()); }
        }

        public IEnumerator<INode> GetChildEnumerator()
        {
            if (!HasChildNodes)
                return null;
            return _childEnumerator ?? (_childEnumerator = new ReaderChildNodeEnumerator(this, _readerWrapper));
        }

        #endregion
    }
}