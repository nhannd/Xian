#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// ModalityProcedureStep entity
    /// </summary>
	public class ModalityProcedureStep : ProcedureStep
	{
        private ModalityProcedureStepType _type;
        private Modality _modality;
        private bool _portable;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="procedure"></param>
        /// <param name="type"></param>
        /// <param name="modality"></param>
        public ModalityProcedureStep(Procedure procedure, ModalityProcedureStepType type, Modality modality)
            :base(procedure)
        {
            _type = type;
            _modality = modality;
        }

        /// <summary>
        /// Default no-args constructor required by NHibernate
        /// </summary>
        public ModalityProcedureStep()
        {
        }

        public override string Name
        {
            get { return _type.Name; }
        }

        public override bool IsPreStep
        {
            get { return false; }
        }

        public virtual ModalityProcedureStepType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// Gets or sets the modality on which this step is to be performed.
        /// </summary>
        public virtual Modality Modality
        {
            get { return _modality; }
            set { _modality = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this step is to be performed on a portable modality.
        /// </summary>
        public virtual bool Portable
        {
            get { return _portable; }
            set { _portable = value; }
        }

		#region Object overrides
		
		public override bool Equals(object that)
		{
			// TODO: implement a test for business-key equality
			return base.Equals(that);
		}
		
		public override int GetHashCode()
		{
			// TODO: implement a hash-code based on the business-key used in the Equals() method
			return base.GetHashCode();
		}
		
		#endregion

	}
}