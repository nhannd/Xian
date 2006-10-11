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


namespace ClearCanvas.Ris.Client.Adt
{
    [ExtensionPoint()]
    public class HL7QueuePreviewComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(HL7QueuePreviewComponentViewExtensionPoint))]
    public class HL7QueuePreviewComponent : ApplicationComponent
    {
        private HL7QueueItemTableData _queue;
        private IHL7QueueService _hl7QueueService;

        public override void Start()
        {
            base.Start();

            _hl7QueueService = ApplicationContext.GetService<IHL7QueueService>();
            _queue = new HL7QueueItemTableData(_hl7QueueService);

            RefreshItems();
        }

        public HL7QueueItemTableData Queue
        {
            get { return _queue; }
            set { _queue = value; }
        }

        public void RefreshItems()
        {
            IList<HL7QueueItem> items = _hl7QueueService.GetNextInboundItemBatch();

            _queue.Items.Clear();
            _queue.Items.AddRange(items);
        }
    }
}
