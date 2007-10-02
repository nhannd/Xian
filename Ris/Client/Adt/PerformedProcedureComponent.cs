using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;

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
    public class PerformedProcedureComponent : ApplicationComponent
    {
        #region ChildComponentHost class

        class ChildComponentHost : ApplicationComponentHost
        {
            private readonly PerformedProcedureComponent _owner;
            private string _title;

            public ChildComponentHost(PerformedProcedureComponent owner, IApplicationComponent hostedComponent)
                : base(hostedComponent)
            {
                _owner = owner;
            }

            public override DesktopWindow DesktopWindow
            {
                get { return _owner.Host.DesktopWindow; }
            }

            public override string Title
            {
                get { return _title; }
                set { _title = value; }
            }
        }

        #endregion

        #region MPPS Details Component

        class MppsDetailsComponent : DHtmlComponent
        {
            private static readonly HtmlFormSelector _detailsFormSelector =
                new HtmlFormSelector(PerformedProcedureComponentSettings.Default.DetailsPageUrlSelectorScript, new string[] { "performedProcedureStep" });

            private readonly PerformedProcedureComponent _owner;

            public MppsDetailsComponent(PerformedProcedureComponent owner)
            {
                _owner = owner;
            }

            protected override string GetTagData(string tag)
            {
                return null;
            }

            public void SelectedMppsChanged()
            {
                if (_owner._selectedMpps == null)
                {
                    SetUrl(null);
                }
                else
                {
                    SetUrl(_detailsFormSelector.SelectForm(_owner._selectedMpps));
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


        /// <summary>
        /// Constructor
        /// </summary>
        public PerformedProcedureComponent(EntityRef orderRef)
        {
            _orderRef = orderRef;
        }

        internal void AddPerformedProcedureStep(ModalityPerformedProcedureStepSummary mpps)
        {
            _mppsTable.Items.Add(mpps);
            _mppsTable.Sort();
        }

        #region ApplicationComponent overrides

        public override void Start()
        {
            _mppsDetailsComponentHost = new ChildComponentHost(this, _detailsComponent = new MppsDetailsComponent(this));
            _mppsDetailsComponentHost.Title = "TODO";
            _mppsDetailsComponentHost.StartComponent();

            ResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);

            _mppsActionHandler = new SimpleActionModel(resolver);
            _stopAction = _mppsActionHandler.AddAction("stop", "STOP", "Icons.CompleteToolSmall.png", "STOP", StopPerformedProcedureStep);
            _discontinueAction = _mppsActionHandler.AddAction("discontinue", "DISCONTINUE", "Icons.DeleteToolSmall.png", "DISCONTINUE", DiscontinuePerformedProcedureStep);

            if (_orderRef != null)
            {
                Platform.GetService<ITechnologistDocumentationService>(
                    delegate(ITechnologistDocumentationService service)
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

        #region Presentation Model

        public ITable MppsTable
        {
            get { return _mppsTable; }
        }

        public ISelection SelectedMpps
        {
            get { return new Selection(_selectedMpps); }
            set
            {
                _selectedMpps = (ModalityPerformedProcedureStepSummary)value.Item;
                _detailsComponent.SelectedMppsChanged();
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

        public void OnComplete()
        {
            try
            {
                Platform.GetService<ITechnologistDocumentationService>(
                    delegate(ITechnologistDocumentationService service)
                    {
                        CompleteModalityProcedureStepsRequest request = new CompleteModalityProcedureStepsRequest(_orderRef);
                        CompleteModalityProcedureStepsResponse response = service.CompleteModalityProcedureSteps(request);

                        RefreshProcedurePlanTree(response.ProcedurePlanSummary);
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

        }


        private void RefreshProcedurePlanTree(ProcedurePlanSummary procedurePlanSummary)
        {
            _orderRef = procedurePlanSummary.OrderRef;
            throw new Exception("The method or operation is not implemented.");
        }
        
        #endregion

        #region Private helpers

        private void StopPerformedProcedureStep()
        {
            try
            {
                ModalityPerformedProcedureStepSummary selectedMpps = _selectedMpps;

                if (selectedMpps != null)
                {
                    Platform.GetService<ITechnologistDocumentationService>(
                        delegate(ITechnologistDocumentationService service)
                        {
                            StopModalityPerformedProcedureStepRequest request = new StopModalityPerformedProcedureStepRequest(selectedMpps.ModalityPerformendProcedureStepRef);
                            StopModalityPerformedProcedureStepResponse response = service.StopModalityPerformedProcedureStep(request);

                            RefreshProcedurePlanTree(response.ProcedurePlanSummary);

                            _mppsTable.Items.Replace(
                                delegate(ModalityPerformedProcedureStepSummary mppsSummary)
                                {
                                    return mppsSummary.ModalityPerformendProcedureStepRef == selectedMpps.ModalityPerformendProcedureStepRef;
                                },
                                response.StoppedMpps);
                            _mppsTable.Sort();
                        });
                }
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
                    Platform.GetService<ITechnologistDocumentationService>(
                        delegate(ITechnologistDocumentationService service)
                        {
                            DiscontinueModalityPerformedProcedureStepRequest request = new DiscontinueModalityPerformedProcedureStepRequest(selectedMpps.ModalityPerformendProcedureStepRef);
                            DiscontinueModalityPerformedProcedureStepResponse response = service.DiscontinueModalityPerformedProcedureStep(request);

                            RefreshProcedurePlanTree(response.ProcedurePlanSummary);

                            _mppsTable.Items.Replace(
                                delegate(ModalityPerformedProcedureStepSummary mppsSummary)
                                {
                                    return mppsSummary.ModalityPerformendProcedureStepRef == selectedMpps.ModalityPerformendProcedureStepRef;
                                },
                                response.DiscontinuedMpps);
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
    }
}
