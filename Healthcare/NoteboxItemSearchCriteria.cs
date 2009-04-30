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
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// 
    /// </summary>
    // Because the notebox system is so simple, there is no point in
    // using the standard ISearchCondition/SearchCriteria pattern here...
    // A few boolean flags will suffice for all current use cases.  As the
    // system evolves, it is expected that this may need to be refactored
    // to be more similar to the typical SearchCriteria classes.
    public class NoteboxItemSearchCriteria : SearchCriteria
    {
        private bool _sentToMe;
        private bool _sentToGroupIncludingMe;
        private bool _sentByMe;
        private bool _isAcknowledged;

        /// <summary>
        /// Constructor.
        /// </summary>
        public NoteboxItemSearchCriteria()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected NoteboxItemSearchCriteria(NoteboxItemSearchCriteria other)
            : base(other)
        {
        }

        public override object Clone()
        {
            return new NoteboxItemSearchCriteria(this);
        }

        public bool SentToMe
        {
            get { return _sentToMe; }
            set { _sentToMe = value; }
        }

        public bool SentToGroupIncludingMe
        {
            get { return _sentToGroupIncludingMe; }
            set { _sentToGroupIncludingMe = value; }
        }

        public bool SentByMe
        {
            get { return _sentByMe; }
            set { _sentByMe = value; }
        }

        public bool IsAcknowledged
        {
            get { return _isAcknowledged; }
            set { _isAcknowledged = value; }
        }
    }
}
