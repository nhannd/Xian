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
using System.Xml;
using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare.Brokers;


namespace ClearCanvas.Healthcare {

    [ExtensionOf(typeof(ProcedureStepBuilderExtensionPoint))]
    public class ModalityProcedureStepBuilder : ProcedureStepBuilderBase
    {

        public override Type ProcedureStepClass
        {
            get { return typeof(ModalityProcedureStep); }
        }

        public override ProcedureStep CreateInstance(XmlElement xmlNode, Procedure procedure)
        {
            ModalityProcedureStep step = new ModalityProcedureStep();

            // set description
            step.Description = GetAttribute(xmlNode, "description", true);

            // set modality - need to look up by ID
            try
            {
                string modalityId = GetAttribute(xmlNode, "modality", true);
                ModalitySearchCriteria where = new ModalitySearchCriteria();
                where.Id.EqualTo(modalityId);

                // TODO might as well cache this query
                step.Modality = PersistenceScope.CurrentContext.GetBroker<IModalityBroker>().FindOne(where);
            }
            catch (EntityNotFoundException e)
            {
                throw new ProcedureBuilderException("Modality ID {0} is not valid.", e);
            }

            return step;
        }

        public override void SaveInstance(ProcedureStep prototype, XmlElement xmlNode)
        {
            ModalityProcedureStep step = (ModalityProcedureStep) prototype;
            xmlNode.SetAttribute("description", step.Description);
            xmlNode.SetAttribute("modality", step.Modality.Id);
        }
    }



    /// <summary>
    /// ModalityProcedureStep entity
    /// </summary>
	public class ModalityProcedureStep : ProcedureStep
	{
        private string _description;
        private Modality _modality;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="procedure"></param>
        /// <param name="description"></param>
        /// <param name="modality"></param>
        public ModalityProcedureStep(Procedure procedure, string description, Modality modality)
            :base(procedure)
        {
            _description = description;
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
            get { return _description; }
        }

        public override bool IsPreStep
        {
            get { return false; }
        }

		protected override ProcedureStep CreateScheduledCopy()
		{
			return new ModalityProcedureStep(this.Procedure, _description, _modality);
		}

        /// <summary>
        /// Gets or sets the description of this step (e.g. CT Chest w/o contrast).
        /// </summary>
        public virtual string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Gets or sets the modality on which this step is to be performed.
        /// </summary>
        public virtual Modality Modality
        {
            get { return _modality; }
            set { _modality = value; }
        }
	}
}