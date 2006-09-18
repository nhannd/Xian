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

            _parcels = new TableData<Parcel>();
 
            _parcels.Columns.Add(new TableColumn<Parcel, string>("Description",
                delegate(Parcel aParcel) { return aParcel.Description; }
                ));
            _parcels.Columns.Add(new TableColumn<Parcel, string>("Status",
                delegate(Parcel aParcel) { return ParcelTransferStateHelper.GetString(aParcel.ParcelTransferState); }
                ));
            _parcels.Columns.Add(new TableColumn<Parcel, string>("Source",
                delegate(Parcel aParcel) { return aParcel.SourceAE.AE; }
                ));
            _parcels.Columns.Add(new TableColumn<Parcel, string>("Destination",
                delegate(Parcel aParcel) { return aParcel.DestinationAE.AE; }
                ));
            _parcels.Columns.Add(new TableColumn<Parcel, int>("Current",
                delegate(Parcel aParcel) { return aParcel.CurrentProgressStep; }
                ));
            _parcels.Columns.Add(new TableColumn<Parcel, int>("Total",
                delegate(Parcel aParcel) { return aParcel.TotalProgressSteps; }
                ));
           
            PopulateListFromDataStore();
        }

        private void PopulateListFromDataStore()
        {
            IEnumerable<IParcel> listOfParcels = DicomServicesLayer.GetISendQueue().GetParcels();
            if (null != listOfParcels)
            {
                foreach (IParcel aParcel in listOfParcels)
                {
                    _parcels.Add(aParcel as Parcel);
                }
            }
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        private TableData<Parcel> _parcels;
        private event EventHandler<QueueStatusChangedEventArgs> _queueStatusChanged;

        public TableData<Parcel> Parcels
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
            IParcel aParcel = _currentSelection.Item as IParcel;
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

    [MenuAction("launch", "global-menus/Services/Queue")]
    [ClickHandler("launch", "Launch")]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
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
