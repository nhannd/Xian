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
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core
{
    public class EntityValidationException : Exception
    {
    	private readonly string _message;
        private readonly TestResultReason[] _reasons;

        public EntityValidationException(string message, TestResultReason[] reasons)
        {
        	_message = message;
            _reasons = reasons;
        }

        public EntityValidationException(string message)
            :this(message, new TestResultReason[]{})
        {
        }

		public override string Message
		{
			get
			{
				List<string> messages = new List<string>();
				foreach (TestResultReason reason in _reasons)
					messages.AddRange(BuildMessageStrings(reason));

				if (messages.Count > 0)
				{
					return _message + "\n" + StringUtilities.Combine(messages, "\n");
				}
				else
					return _message;
			}
		}

        public TestResultReason[] Reasons
        {
            get { return _reasons; }
        }

        private static List<string> BuildMessageStrings(TestResultReason reason)
        {
            List<string> messages = new List<string>();
            if (reason.Reasons.Length == 0)
                messages.Add(reason.Message);
            else
            {
                foreach (TestResultReason subReason in reason.Reasons)
                {
                    List<string> subMessages = BuildMessageStrings(subReason);
                    foreach (string subMessage in subMessages)
                    {
                        messages.Add(string.Format("{0} {1}", reason.Message, subMessage));
                    }
                }
            }
            return messages;
        }
    }
}
