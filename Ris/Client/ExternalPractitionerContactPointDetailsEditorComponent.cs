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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using System.Collections;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="ExternalPractitionerContactPointDetailsEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ExternalPractitionerContactPointDetailsEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ExternalPractitionerContactPointDetailsEditorComponent class
    /// </summary>
    [AssociateView(typeof(ExternalPractitionerContactPointDetailsEditorComponentViewExtensionPoint))]
    public class ExternalPractitionerContactPointDetailsEditorComponent : ApplicationComponent
    {
        private ExternalPractitionerContactPointDetail _contactPointDetail;
        private IList<EnumValueInfo> _resultCommunicationModeChoices;

        /// <summary>
        /// Constructor
        /// </summary>
        public ExternalPractitionerContactPointDetailsEditorComponent(ExternalPractitionerContactPointDetail contactPointDetail, IList<EnumValueInfo> resultCommunicationModeChoices)
        {
            _contactPointDetail = contactPointDetail;
            _resultCommunicationModeChoices = resultCommunicationModeChoices;
        }

        #region Presentation Model

        [ValidateNotNull]
        public string ContactPointName
        {
            get { return _contactPointDetail.Name; }
            set
            {
                _contactPointDetail.Name = value;
                this.Modified = true;
            }
        }

        public string ContactPointDescription
        {
            get { return _contactPointDetail.Description; }
            set
            {
                _contactPointDetail.Description = value;
                this.Modified = true;
            }
        }

        public bool IsDefaultContactPoint
        {
            get { return _contactPointDetail.IsDefaultContactPoint; }
            set
            {
                _contactPointDetail.IsDefaultContactPoint = value;
                this.Modified = true;
            }
        }

        public IList ResultCommunicationModeChoices
        {
            get { return (IList)_resultCommunicationModeChoices; }
        }

        [ValidateNotNull]
        public EnumValueInfo SelectedResultCommunicationMode
        {
            get { return _contactPointDetail.PreferredResultCommunicationMode; }
            set
            {
                _contactPointDetail.PreferredResultCommunicationMode = value;
                this.Modified = true;
            }
        }

        #endregion
    }
}
