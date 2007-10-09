using System;
using System.IO;
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
    public class PerformedProcedureComponent : ApplicationComponent, IDocumentationPage
    {
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
                string data = null;
                if (string.Equals(tag, "Documentation"))
                {
                    try
                    {
                        StreamReader sr = File.OpenText(@"C:\tempdocumentation.txt");
                        data = sr.ReadToEnd();
                        sr.Close();
                    }
                    catch (FileNotFoundException)
                    {
                        data = null;
                    }
                }
                else if (string.Equals(tag, "StartTime"))
                {
                    data = _owner._selectedMpps.StartTime.ToString();
                }
                else if (string.Equals(tag, "StopTime"))
                {
                    data = _owner._selectedMpps.EndTime.HasValue
                               ? _owner._selectedMpps.EndTime.Value.ToString()
                               : null;
                }
                return data;
            }

            protected override void SetTagData(string tag, string data)
            {
                if(string.Equals(tag, "Documentation"))
                {
                    StreamWriter sw = File.CreateText(@"C:\tempdocumentation.txt");
                    sw.Write(data);
                    sw.Close();
                }
                //_owner._selectedMpps.Documtation = data;
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

            public new void SaveData()
            {
                base.SaveData();
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

        private event EventHandler<ProcedurePlanChangedEventArgs> _procedurePlanChanged;

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
            _mppsDetailsComponentHost = new ChildComponentHost(this.Host, _detailsComponent = new MppsDetailsComponent(this));
            //_mppsDetailsComponentHost.Title = SR.TitlePerformedProcedureComponent;
            _mppsDetailsComponentHost.StartComponent();

            ResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);

            _mppsActionHandler = new SimpleActionModel(resolver);

            _stopAction = _mppsActionHandler.AddAction("stop", SR.TitleStopMpps, "Icons.CompleteToolSmall.png", SR.TitleStopMpps, StopPerformedProcedureStep);
            _discontinueAction = _mppsActionHandler.AddAction("discontinue", SR.TitleDiscontinueMpps, "Icons.DeleteToolSmall.png", SR.TitleDiscontinueMpps, DiscontinuePerformedProcedureStep);
            UpdateActionEnablement();

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

        #region IDocumentationPage Members

        public void Save()
        {
            _detailsComponent.SaveData();
        }

        public void Validate()
        {
            throw new NotImplementedException();
        }

        public event EventHandler<ProcedurePlanChangedEventArgs> ProcedurePlanChanged
        {
            add { _procedurePlanChanged += value; }
            remove { _procedurePlanChanged -= value; }
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
                UpdateActionEnablement();
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

        private void RefreshProcedurePlanTree(ProcedurePlanSummary procedurePlanSummary)
        {
            _orderRef = procedurePlanSummary.OrderRef;
            EventsHelper.Fire(_procedurePlanChanged, this, new ProcedurePlanChangedEventArgs(procedurePlanSummary));
        }

        #endregion

        #region Tool Click Handlers

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

                            // Refresh selection
                            _selectedMpps = response.StoppedMpps;
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
