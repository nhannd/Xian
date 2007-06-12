using System;
using System.Collections.Generic;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client
{
    public class PagingActionModel<T> : SimpleActionModel
    {
        private delegate IList<T> PageResultsDelegate();

        private readonly IDesktopWindow _desktopWindow;
        private readonly IPagingController<T> _controller;
        private readonly Table<T> _table;

        public PagingActionModel(IPagingController<T> controller, Table<T> table, IDesktopWindow desktopWindow)
            : base(new ResourceResolver(typeof(PagingActionModel<T>).Assembly))
        {
            _controller = controller;
            _table = table;
            _desktopWindow = desktopWindow;

			AddAction("Previous", SR.TitlePrevious, "Icons.PreviousPageToolSmall.png");
            AddAction("Next", SR.TitleNext, "Icons.NextPageToolSmall.png");

            Next.SetClickHandler(OnNext);
            Previous.SetClickHandler(OnPrevious);

            Next.Enabled = _controller.HasNext;
            Previous.Enabled = _controller.HasPrev;  // can't go back from first

            _controller.OnInitialQueryEvent += this.UpdateActionAvailability;
        }

        private void OnNext()
        {
            PageChangeActionHandler(delegate {return _controller.GetNext(); });
        }

        private void OnPrevious()
        {
            PageChangeActionHandler(delegate { return _controller.GetPrev(); });
        }

        private void PageChangeActionHandler(PageResultsDelegate pageResults)
        {
            try
            {
                IList<T> results = pageResults();
                _table.Items.Clear();
                _table.Items.AddRange(results);
                UpdateActionAvailability();
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, _desktopWindow);
            }
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
