using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Dicom.Services
{
    /// <summary>
    /// Extension point for views onto <see cref="QueueDisplay"/>
    /// </summary>
    [ExtensionPoint]
    public class QueueDisplayComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// QueueDisplay class
    /// </summary>
    [AssociateView(typeof(QueueDisplayComponentViewExtensionPoint))]
    public class QueueDisplayComponent : ApplicationComponent
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public QueueDisplayComponent()
        {
        }

        public override void Start()
        {
            // TODO prepare the component for its live phase
            base.Start();

            _parcels = new TableData<SendParcel>();
 
            _parcels.Columns.Add(new TableColumn<SendParcel, string>("Description",
                delegate(SendParcel aParcel) { return aParcel.Description; }
                ));
            _parcels.Columns.Add(new TableColumn<SendParcel, string>("Status",
                delegate(SendParcel aParcel) { return ParcelTransferStateHelper.GetString(aParcel.ParcelTransferState); }
                ));
            _parcels.Columns.Add(new TableColumn<SendParcel, string>("Source",
                delegate(SendParcel aParcel) { return aParcel.SourceAE.AE; }
                ));
            _parcels.Columns.Add(new TableColumn<SendParcel, string>("Destination",
                delegate(SendParcel aParcel) { return aParcel.DestinationAE.AE; }
                ));
            _parcels.Columns.Add(new TableColumn<SendParcel, int>("Current",
                delegate(SendParcel aParcel) { return aParcel.CurrentProgressStep; }
                ));
            _parcels.Columns.Add(new TableColumn<SendParcel, int>("Total",
                delegate(SendParcel aParcel) { return aParcel.TotalProgressSteps; }
                ));
           
            PopulateListFromDataStore();
        }

        private void PopulateListFromDataStore()
        {
            IEnumerable<ISendParcel> listOfParcels = DicomServicesLayer.GetISendQueue().GetParcels();
            if (null != listOfParcels)
            {
                foreach (ISendParcel aParcel in listOfParcels)
                {
                    _parcels.Add(aParcel as SendParcel);
                }
            }
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        private TableData<SendParcel> _parcels;
        private event EventHandler<QueueStatusChangedEventArgs> _queueStatusChanged;

        public TableData<SendParcel> Parcels
        {
            get { return _parcels; }
        }

        public event EventHandler<QueueStatusChangedEventArgs> QueueStatusChanged
        {
            add { _queueStatusChanged += value; }
            remove { _queueStatusChanged -= value; }
        }

        public void SetSelection(ISelection selection)
        {
            if (_currentSelection != selection)
            {
                _currentSelection = selection;
            }
        }

        public void RemoveSelectedParcel()
        {
            ISendParcel aParcel = _currentSelection.Item as ISendParcel;
            DicomServicesLayer.GetISendQueue().Remove(aParcel);
            Refresh();
        }

        public void AbortSendSelectedParcel()
        {

        }

        public void PauseSendSelectedParcel()
        {

        }

        public void RestartSendSelectedParcel()
        {
        }

        public void Refresh()
        {
            _parcels.Clear();
            PopulateListFromDataStore();
        }

        #region Handcoded Members
        #region Private Members
        ISelection _currentSelection;
        #endregion
        #endregion
    }

	//[MenuAction("launch", "global-menus/Services/Queue")]
	//[ClickHandler("launch", "Launch")]
	//[ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class QueueDisplayLaunchTool : Tool<IDesktopToolContext>
    {
        public QueueDisplayLaunchTool()
        {

        }

        public void Launch()
        {
            ApplicationComponent.LaunchAsWorkspace(
                this.Context.DesktopWindow,
                new QueueDisplayComponent(),
                "Services Queue",
                delegate(IApplicationComponent component)
                { Console.WriteLine("Done!"); }
                );
        }
    }
}
