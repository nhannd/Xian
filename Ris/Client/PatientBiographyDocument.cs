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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
    public class PatientBiographyDocument : Document
    {
		private readonly EntityRef _patientRef;
		private readonly EntityRef _profileRef;
    	private readonly EntityRef _orderRef;

		public PatientBiographyDocument(EntityRef patientRef, EntityRef profileRef, IDesktopWindow window)
			: base(patientRef, window)
		{
			Platform.CheckForNullReference(patientRef, "patientRef");
			Platform.CheckForNullReference(profileRef, "profileRef");

			_patientRef = patientRef;
			_profileRef = profileRef;
		}

		public PatientBiographyDocument(EntityRef patientRef, EntityRef profileRef, EntityRef orderRef, IDesktopWindow window)
			: this(patientRef, profileRef, window)
        {
			_orderRef = orderRef;
		}

        public override string GetTitle()
        {
            return "";  // not relevant - component will set title
        }

		public override bool SaveAndClose()
		{
			return base.Close();
		}

        public override IApplicationComponent GetComponent()
        {
            BiographyOverviewComponent component = new BiographyOverviewComponent(_patientRef, _profileRef);
        	component.SelectedOrderRef = _orderRef;
        	return component;
        }
    }    
}
