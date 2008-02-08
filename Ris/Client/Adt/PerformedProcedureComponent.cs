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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="PerformedProcedureComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PerformedProcedureComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// PerformedProcedureComponent class
    /// </summary>
    [AssociateView(typeof(PerformedProcedureComponentViewExtensionPoint))]
    public class PerformedProcedureComponent : ApplicationComponent, IDocumentationPage
    {
        #region MPPS Details Component

        class MppsDetailsComponent : DHtmlComponent
        {
            private ModalityPerformedProcedureStepSummary _performedProcedureStep;

            public MppsDetailsComponent()
            {
            }

            protected override DataContractBase GetHealthcareContext()
            {
                return _performedProcedureStep;
            }

            protected override IDictionary<string, string> TagData
            {
                get
                {
                    return _performedProcedureStep.ExtendedProperties;
                }
            }

            public ModalityPerformedProcedureStepSummary PerformedProcedureStep
            {
                get { return _performedProcedureStep; }
                set
                {
                    if(!Equals(value, _performedProcedureStep))
                    {
                        if (_performedProcedureStep != null)
                        {
                            // store data for current step
                            SaveData();
                        }
                        _performedProcedureStep = value;

                        SetUrl(PerformedProcedureComponentSettings.Default.DetailsPageUrl);
                    }
                }
            }
        }

        #endregion

        private readonly TechnologistDocumentationMppsSummaryTable _mppsTable = new TechnologistDocumentationMppsSummaryTable();
        private ModalityPerformedProcedureStepSummary _selectedMpps;
        private EntityRef _orderRef;

        private SimpleActionModel _mppsActionHandler;
        private ClickAction _stopAction;
        private ClickAction _discontinueAction;

        private ChildComponentHost _mppsDetailsComponentHost;
        private MppsDetailsComponent _detailsComponent;
        private readonly string _title;
        private TechnologistDocumentationComponent _owner;

        private event EventHandler<ProcedurePlanChangedEventArgs> _procedurePlanChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        public PerformedProcedureComponent(string title, EntityRef orderRef, TechnologistDocumentationComponent owner)
        {
            _title = title;
            _orderRef = orderRef;
            _owner = owner;
        }

        internal void AddPerformedProcedureStep(ModalityPerformedProcedureStepSummary mpps)
        {
            _mppsTable.Items.Add(mpps);
            _mppsTable.Sort();
        }

        internal event EventHandler<ProcedurePlanChangedEventArgs> ProcedurePlanChanged
        {
            add { _procedurePlanChanged += value; }
            remove { _procedurePlanChanged -= value; }
        }

        internal void SaveData()
        {
            _detailsComponent.SaveData();
        }

        internal IList<ModalityPerformedProcedureStepSummary> PerformedProcedureSteps
        {
            get { return _mppsTable.Items; }   
        }

        #region ApplicationComponent overrides

        public override void Start()
        {
            _mppsDetailsComponentHost = new ChildComponentHost(this.Host, _detailsComponent = new MppsDetailsComponent());
            //_mppsDetailsComponentHost.Title = SR.TitlePerformedProcedureComponent;
            _mppsDetailsComponentHost.StartComponent();

            ResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);

            _mppsActionHandler = new SimpleActionModel(resolver);

            _stopAction = _mppsActionHandler.AddAction("stop", SR.TitleStopMpps, "Icons.CompleteToolSmall.png", SR.TitleStopMpps, StopPerformedProcedureStep);
            _discontinueAction = _mppsActionHandler.AddAction("discontinue", SR.TitleDiscontinueMpps, "Icons.DeleteToolSmall.png", SR.TitleDiscontinueMpps, DiscontinuePerformedProcedureStep);
            UpdateActionEnablement();

            if (_orderRef != null)
            {
                Platform.GetService<IModalityWorkflowService>(
                    delegate(IModalityWorkflowService service)
                    {
                        ListPerformedProcedureStepsRequest mppsRequest = new ListPerformedProcedureStepsRequest(_orderRef);
                        ListPerformedProcedureStepsResponse mppsResponse = service.ListPerformedProcedureSteps(mppsRequest);

                        _mppsTable.Items.AddRange(mppsResponse.PerformedProcedureSteps);
                    });
            }

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #endregion

        #region IDocumentationPage Members

        string IDocumentationPage.Title
        {
            get { return _title; }
        }

        IApplicationComponent IDocumentationPage.Component
        {
            get { return this; }   
        }

        #endregion

        #region Presentation Model

        public ITable ProcedurePlanSummaryTable
        {
            get { return _owner.ProcedurePlanSummaryTable; }
        }

        public ActionModelNode ProcedurePlanTreeActionModel
        {
            get { return _owner.ProcedurePlanTreeActionModel; }
        }

        public ITable MppsTable
        {
            get { return _mppsTable; }
        }

        public ISelection SelectedMpps
        {
            get { return new Selection(_selectedMpps); }
            set
            {
                ModalityPerformedProcedureStepSummary selectedMpps = (ModalityPerformedProcedureStepSummary)value.Item;
                if (selectedMpps != _selectedMpps)
                {
                    _selectedMpps = selectedMpps;
                    UpdateActionEnablement();
                    _detailsComponent.PerformedProcedureStep = _selectedMpps;
                }
            }
        }

        public ActionModelNode MppsTableActionModel
        {
            get { return _mppsActionHandler; }
        }

        public ApplicationComponentHost DetailsComponentHost
        {
            get { return _mppsDetailsComponentHost; }
        }

        private void RefreshProcedurePlanTree(ProcedurePlanDetail procedurePlanDetail)
        {
            _orderRef = procedurePlanDetail.OrderRef;
            EventsHelper.Fire(_procedurePlanChanged, this, new ProcedurePlanChangedEventArgs(procedurePlanDetail));
        }
        
        #endregion

        #region Tool Click Handlers

        private void StopPerformedProcedureStep()
        {
            if (this.HasValidationErrors || _detailsComponent.HasValidationErrors)
            {
                ShowValidation(true);
                _detailsComponent.ShowValidation(true);
                return;
            }

            //this.Host.DesktopWindow.ShowMessageBox("Valid", MessageBoxActions.Ok);
            //return;

            if(_selectedMpps == null)
            {
                return;
            }

            try
            {
                _detailsComponent.SaveData();

                Platform.GetService<IModalityWorkflowService>(
                    delegate(IModalityWorkflowService service)
                    {
                        CompleteModalityPerformedProcedureStepRequest request = new CompleteModalityPerformedProcedureStepRequest(
                                _selectedMpps.ModalityPerformendProcedureStepRef,
                                _selectedMpps.ExtendedProperties);
                        CompleteModalityPerformedProcedureStepResponse response = service.CompleteModalityPerformedProcedureStep(request);

                        RefreshProcedurePlanTree(response.ProcedurePlan);

                        _mppsTable.Items.Replace(
                            delegate(ModalityPerformedProcedureStepSummary mppsSummary)
                            {
                                return mppsSummary.ModalityPerformendProcedureStepRef.Equals(_selectedMpps.ModalityPerformendProcedureStepRef, true);
                            },
                            response.StoppedMpps);

                        // Refresh selection
                        _selectedMpps = response.StoppedMpps;
                        UpdateActionEnablement();
                        _mppsTable.Sort();
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        private void DiscontinuePerformedProcedureStep()
        {
            try
            {
                ModalityPerformedProcedureStepSummary selectedMpps = _selectedMpps;

                if (selectedMpps != null)
                {
                    Platform.GetService<IModalityWorkflowService>(
                        delegate(IModalityWorkflowService service)
                        {
                            //TODO should save details here too
                            DiscontinueModalityPerformedProcedureStepRequest request = new DiscontinueModalityPerformedProcedureStepRequest(selectedMpps.ModalityPerformendProcedureStepRef);
                            DiscontinueModalityPerformedProcedureStepResponse response = service.DiscontinueModalityPerformedProcedureStep(request);

                            RefreshProcedurePlanTree(response.ProcedurePlan);

                            _mppsTable.Items.Replace(
                                delegate(ModalityPerformedProcedureStepSummary mppsSummary)
                                {
                                    return mppsSummary.ModalityPerformendProcedureStepRef.Equals(_selectedMpps.ModalityPerformendProcedureStepRef, true);
                                },
                                response.DiscontinuedMpps);

                            _selectedMpps = response.DiscontinuedMpps;
                            UpdateActionEnablement();
                            _mppsTable.Sort();
                        });
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        #endregion

        #region Private Methods

        private void UpdateActionEnablement()
        {
            if(_selectedMpps != null)
            {
                // TOOD:  replace with server side logic
                _stopAction.Enabled = _discontinueAction.Enabled = _selectedMpps.State.Code == "IP";
            }
            else
            {
                _stopAction.Enabled = _discontinueAction.Enabled = false;
            }
        }

        #endregion
    }
}
