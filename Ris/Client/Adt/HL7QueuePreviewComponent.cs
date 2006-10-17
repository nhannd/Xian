using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Healthcare;

using ClearCanvas.HL7;

using Iesi.Collections;
using ClearCanvas.Ris.Services;
using ClearCanvas.Common.Utilities;


namespace ClearCanvas.Ris.Client.Adt
{
    [ExtensionPoint()]
    public class HL7QueuePreviewComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(HL7QueuePreviewComponentViewExtensionPoint))]
    public class HL7QueuePreviewComponent : ApplicationComponent
    {
        private HL7QueueItem _selectedItem;

        private HL7QueueItemTableData _queue;
        private IHL7QueueService _hl7QueueService;

        public override void Start()
        {
            base.Start();

            _hl7QueueService = ApplicationContext.GetService<IHL7QueueService>();
            _queue = new HL7QueueItemTableData(_hl7QueueService);
            

            ShowAllItems();
        }

        public HL7QueueItemTableData Queue
        {
            get { return _queue; }
            //set { _queue = value; }
        }

        public string Message
        {
            get { return _selectedItem.Message.Text; }
        }

        public void ShowAllItems()
        {
            IList<HL7QueueItem> items = _hl7QueueService.GetAllHL7QueueItems();

            _queue.Items.Clear();
            _queue.Items.AddRange(items);
        }

        public void ShowNextPendingBatchItems()
        {
            IList<HL7QueueItem> items = _hl7QueueService.GetNextInboundHL7QueueItemBatch();

            _queue.Items.Clear();
            _queue.Items.AddRange(items);
        }

        public void SyncQueues()
        {
            _hl7QueueService.SyncExternalQueue();
        }

        public void ProcessSelection()
        {
            try
            {
                _hl7QueueService.ProcessHL7QueueItem(_selectedItem);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to process queue item:  " + e.Message);
            }

            ShowAllItems();
        }

        public void SetSelectedItem(ISelection selection)
        {
            _selectedItem = selection.Item as HL7QueueItem;
            NotifyPropertyChanged("Message");
        }
    }
}
