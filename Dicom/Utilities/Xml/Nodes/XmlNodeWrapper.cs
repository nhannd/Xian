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
    class XmlNodeWrapper : INode
    {
        private readonly XmlNode _node;
        public XmlNodeWrapper(XmlNode node)
        {
            _node = node;
        }

        public INode FirstChild
        {
            get
            {
                return _node.HasChildNodes ? new XmlNodeWrapper(_node.FirstChild) : null;
            }
        }

        public bool HasChildNodes
        {
            get { return _node.HasChildNodes; }
        }


        public string Name
        {
            get { return _node.Name; }
        }

        public bool HasAttribute(string name)
        {
            return _node.Attributes != null && _node.Attributes[name] != null;
        }

        public string Attribute(string name)
        {
            return HasAttribute(name) ? _node.Attributes[name].Value : null;
        }

        public string Content
        {
            get { return _node.InnerXml; }
        }

        public string InnerXml
        {
            get { return _node.InnerXml; }
        }

        public IEnumerator<INode> GetChildEnumerator()
        {
            return new XmlChildNodeEnumerator(_node);
        }

    }
}