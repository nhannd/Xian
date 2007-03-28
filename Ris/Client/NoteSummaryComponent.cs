using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="NoteSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class NoteSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// NoteSummaryComponent class
    /// </summary>
    [AssociateView(typeof(NoteSummaryComponentViewExtensionPoint))]
    public class NoteSummaryComponent : ApplicationComponent
    {
        private List<NoteDetail> _noteList;
        private NoteTable _noteTable;

        /// <summary>
        /// Constructor
        /// </summary>
        public NoteSummaryComponent()
        {
            _noteTable = new NoteTable();
        }

        public List<NoteDetail> Subject
        {
            get { return _noteList; }
            set { _noteList = value; }
        }

        public override void Start()
        {
            _noteTable.Items.AddRange(_noteList);

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #region Presentation Model

        public ITable Notes
        {
            get { return _noteTable; }
        }

        #endregion

    }
}
