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

namespace ClearCanvas.Dicom.Utilities.Xml.Nodes
{
    public class ReaderChildNodeEnumerator : IEnumerator<INode>
    {
        private readonly ReaderNode _parent;
        private readonly ReaderWrapper _readerWrapper;
        private ReaderNode _current;
        private bool _firstTime = true;
        private bool _valid;

        public ReaderChildNodeEnumerator(ReaderNode parent, ReaderWrapper readerWrapper)
        {
            _readerWrapper = readerWrapper;
            _parent = parent;
        }

        #region IEnumerator<INode> Members

        public bool MoveNext()
        {
            if (_firstTime)
            {
                //no children
                if (_readerWrapper.GetXmlReader.IsEmptyElement)
                    return false;
                _valid = _readerWrapper.MoveFirstChild();
                _firstTime = false;
            }
            else
            {
                _valid = _readerWrapper.MoveNextSibling(_parent.Depth + 1, _current.NodeCount);
            }

            //advance current
            if (_valid)
                _current = new ReaderNode(_readerWrapper);
            return _valid;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public INode Current
        {
            get { return _valid ? _current : null; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}