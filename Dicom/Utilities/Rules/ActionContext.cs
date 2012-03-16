using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.Utilities.Command;

namespace ClearCanvas.Dicom.Utilities.Rules
{
    public abstract class ActionContext
    {

        #region Public Properties

        /// <summary>
        /// The message being worked against.
        /// </summary>
        public DicomMessageBase Message { get; set; }

        /// <summary>
        /// The command processor.
        /// </summary>
        public CommandProcessor CommandProcessor { get; set; }
        #endregion

    }
}
