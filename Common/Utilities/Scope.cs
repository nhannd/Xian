#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Taken and modified from the following MSDN article by Stephen Toub:
    /// http://msdn.microsoft.com/msdnmag/issues/06/09/NETMatters/default.aspx
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Scope<T> : IDisposable
        where T : class
    {
        private bool _disposed, _ownsInstance;
        private T _instance;
        private Scope<T> _parent;

        [ThreadStatic]
        private static Scope<T> _head;

        public Scope(T instance) : this(instance, true) { }

        public Scope(T instance, bool ownsInstance)
        {
            Platform.CheckForNullReference(instance, "instance");

            _instance = instance;
            _ownsInstance = ownsInstance;

            _parent = _head;
            _head = this;
        }

        public static T Current
        {
            get { return _head != null ? _head._instance : null; }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                System.Diagnostics.Debug.Assert(this == _head, "Disposed out of order.");
                _head = _parent;

                if (_ownsInstance)
                {
                    IDisposable disposable = _instance as IDisposable;
                    if (disposable != null) disposable.Dispose();
                }
            }
        }
    }
}
