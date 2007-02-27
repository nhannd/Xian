using System;
using System.Collections.Generic;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Common
{
    public class PagingActionModel<T> : SimpleActionModel
    {
        IPagingController<T> _controller;
        Table<T> _table;

        public PagingActionModel(IPagingController<T> controller, Table<T> table)
            : base(new ResourceResolver(typeof(PagingActionModel<T>).Assembly))
        {
            _controller = controller;
            _table = table;

            this.AddAction("Previous", SR.TitlePrevious, "Icons.Previous.png");
            this.AddAction("Next", SR.TitleNext, "Icons.Next.png");

            Next.SetClickHandler(delegate { PageChangeActionHandler(_controller.GetNext()); });
            Previous.SetClickHandler(delegate { PageChangeActionHandler(_controller.GetPrev()); });

            Next.Enabled = _controller.HasNext;
            Previous.Enabled = _controller.HasPrev;  // can't go back from first

            _controller.OnInitialQueryEvent += this.UpdateActionAvailability;
        }

        private void PageChangeActionHandler(IList<T> results)
        {
            _table.Items.Clear();
            _table.Items.AddRange(results);
            UpdateActionAvailability();
        }

        private void UpdateActionAvailability()
        {
            Next.Enabled = _controller.HasNext;
            Previous.Enabled = _controller.HasPrev;
        }

        private ClickAction Next
        {
            get { return this["Next"]; }
        }

        private ClickAction Previous
        {
            get { return this["Previous"]; }
        }
    }
}
