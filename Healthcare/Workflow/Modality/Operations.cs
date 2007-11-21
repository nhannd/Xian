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
using System.Collections;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Workflow.Modality
{
    public abstract class ModalityOperation
    {
        public abstract void Execute(ModalityProcedureStep mps, IWorkflow workflow);
        public abstract bool CanExecute(ModalityProcedureStep mps);

        protected void UpdateCheckInStep(RequestedProcedure rp, bool procedureAborted, IWorkflow workflow)
        {
            bool allMpsScheduled = rp.ModalityProcedureSteps.TrueForAll(
                delegate(ModalityProcedureStep mps) { return mps.State == ActivityStatus.SC; });

            bool allMpsComplete = rp.ModalityProcedureSteps.TrueForAll(
                delegate(ModalityProcedureStep mps) { return mps.State == ActivityStatus.CM; });

            bool allMpsTerminated = rp.ModalityProcedureSteps.TrueForAll(
                delegate(ModalityProcedureStep mps) { return mps.IsTerminated; });


            if (allMpsComplete || (allMpsTerminated && !procedureAborted))
            {
                // complete the check-in
                rp.ProcedureCheckIn.CheckOut();

                // schedule an interpretation
                // Note: in reality, we want to create the Interpretation step earlier (perhaps when an MPS starts)
                // so the radiologist can start interpreting as soon as the technologist finishes some part of the scan
                workflow.AddActivity(new InterpretationStep(rp));
            }
            else if (allMpsTerminated && procedureAborted)
            {
                // discontinue check-in, since procedure was aborted
                rp.ProcedureCheckIn.CheckOut();
            }
            else if (!allMpsScheduled)
            {
                // check-in this procedure, since some mps has started
                rp.ProcedureCheckIn.CheckIn();
            }
        }
    }

    public class StartModalityProcedureStepOperation : ModalityOperation
    {
        private Staff _technologist;

        public StartModalityProcedureStepOperation(Staff technologist)
        {
            _technologist = technologist;
        }

        public override void Execute(ModalityProcedureStep mps, IWorkflow workflow)
        {
            mps.Start(_technologist);
            UpdateCheckInStep(mps.RequestedProcedure, false, workflow);
        }

        public override bool CanExecute(ModalityProcedureStep mps)
        {
            return mps.State == ActivityStatus.SC;
        }
    }

    public class CompleteModalityProcedureStepOperation : ModalityOperation
    {
        private bool _procedureAborted;

        public CompleteModalityProcedureStepOperation(bool procedureAborted)
        {
            _procedureAborted = procedureAborted;
        }

        public override void Execute(ModalityProcedureStep mps, IWorkflow workflow)
        {
            // complete the modality step
            mps.Complete();
            UpdateCheckInStep(mps.RequestedProcedure, _procedureAborted, workflow);
        }

        public override bool CanExecute(ModalityProcedureStep mps)
        {
            return mps.State == ActivityStatus.IP || mps.State == ActivityStatus.SC;
        }
    }

    public class CancelModalityProcedureStepOperation : ModalityOperation
    {
        private bool _procedureAborted;

        public CancelModalityProcedureStepOperation(bool procedureAborted)
        {
            _procedureAborted = procedureAborted;
        }

        public override void Execute(ModalityProcedureStep mps, IWorkflow workflow)
        {
            mps.Discontinue();
            UpdateCheckInStep(mps.RequestedProcedure, _procedureAborted, workflow);
        }

        public override bool CanExecute(ModalityProcedureStep mps)
        {
            return !mps.IsTerminated;
        }
    }
}
