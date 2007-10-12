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

using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Alert
{
    public abstract class OrderAlertBase : IOrderAlert
    {
        protected OrderAlertBase()
        {
        }

        #region IOrderAlert Members

        public string Name
        {
            get { return "OrderAlert"; }
        }

        public virtual IAlertNotification Test(Order order, IPersistenceContext context)
        {
            return null;
        }

        #endregion
    }

    public class OrderAlertHelper
    {
        private static OrderAlertHelper _instance;
        private readonly IList<IOrderAlert> _orderAlertTests;

        public static OrderAlertHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new OrderAlertHelper();

                return _instance;
            }
        }

        private OrderAlertHelper()
        {
            OrderAlertExtensionPoint xp = new OrderAlertExtensionPoint();
            object[] tests = xp.CreateExtensions();

            _orderAlertTests = new List<IOrderAlert>();
            foreach (object o in tests)
            {
                _orderAlertTests.Add((IOrderAlert)o);
            }
        }

        public IList<IOrderAlert> GetAlertTests()
        {
            return _orderAlertTests;
        }
    }
}
