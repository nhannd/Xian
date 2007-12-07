using System;
using System.Web.UI;

namespace ClearCanvas.ImageServer.Web.Application.Search
{
    public partial class SearchToolBar : UserControl
    {
        /// <summary>
        /// Defines the handler for <seealso cref="OnRefreshButtonClick"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ev"></param>
        public delegate void RefreshButtonClick(object sender, ImageClickEventArgs ev);

        /// <summary>
        /// Occurs when user clicks on "Refresh" button.
        /// </summary>
        /// <remarks>
        /// This event is fired on the server side.
        /// </remarks>
        public event RefreshButtonClick OnRefreshButtonClick;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void RefreshButton_Click(object sender, ImageClickEventArgs e)
        {
            if (OnRefreshButtonClick != null)
                OnRefreshButtonClick(sender, e);
        }
    }
}