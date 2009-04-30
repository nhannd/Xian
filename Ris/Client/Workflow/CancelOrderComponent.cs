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

using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;

namespace ClearCanvas.Ris.Client.Workflow
{
    /// <summary>
    /// Extension point for views onto <see cref="CancelOrderComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class CancelOrderComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// CancelOrderComponent class
    /// </summary>
    [AssociateView(typeof(CancelOrderComponentViewExtensionPoint))]
    public class CancelOrderComponent : ApplicationComponent
    {
        private EnumValueInfo _selectedCancelReason;
        private List<EnumValueInfo> _cancelReasonChoices;

        /// <summary>
        /// Constructor
        /// </summary>
        public CancelOrderComponent()
        {
        }

        public override void Start()
        {
            Platform.GetService<IOrderEntryService>(
                delegate(IOrderEntryService service)
                {
                    GetCancelOrderFormDataResponse response = service.GetCancelOrderFormData(new GetCancelOrderFormDataRequest());
                    _cancelReasonChoices = response.CancelReasonChoices;
                });

            base.Start();
        }

        #region Presentation Model

        public IList CancelReasonChoices
        {
            get { return _cancelReasonChoices; }
        }

        public EnumValueInfo SelectedCancelReason
        {
            get { return _selectedCancelReason; }
            set { _selectedCancelReason = value; }
        }

        #endregion

        public void Accept()
        {
            this.Exit(ApplicationComponentExitCode.Accepted);
        }

        public void Cancel()
        {
            this.Exit(ApplicationComponentExitCode.None);
        }

        public bool AcceptEnabled
        {
            get { return _selectedCancelReason != null; }
        }
    }
}
