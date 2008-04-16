using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCanvas.ImageServer.Web.Application.WorkQueue.Edit
{
    /// <summary>
    /// Base class for controls that display the Work Queue Item details view.
    /// </summary>
    public class WorkQueueDetailsViewBase : UserControl
    {
        private Unit _width;
        private Model.WorkQueue _workQueue;

        /// <summary>
        /// Sets or gets the list of studies whose information are displayed
        /// </summary>
        public Model.WorkQueue WorkQueue
        {
            get { return _workQueue; }
            set { _workQueue = value; }
        }

        /// <summary>
        /// Sets or gets the width of control
        /// </summary>
        public virtual Unit Width
        {
            get { return _width; }
            set { _width = value;}
        }
    }
}