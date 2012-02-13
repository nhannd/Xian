#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
// 
// For information about the licensing and copyright of this software please
// contact ClearCanvas, Inc. at info@clearcanvas.ca

#endregion

namespace ClearCanvas.Web.Client.Silverlight.Views
{
    /// <summary>
    /// Names and helpers for visual states in the controls.
    /// </summary>
    internal static class VisualStates
    {
        #region GroupBusyStatus
        /// <summary>
        /// Busy state for BusyIndicator.
        /// </summary>
        public const string StateBusy = "Busy";

        /// <summary>
        /// Idle state for BusyIndicator.
        /// </summary>
        public const string StateIdle = "Idle";

        /// <summary>
        /// Busy states group name.
        /// </summary>
        public const string GroupBusyStatus = "BusyStatusStates";
        #endregion

        #region GroupVisibility
        /// <summary>
        /// Visible state name for BusyIndicator.
        /// </summary>
        public const string StateVisible = "Visible";

        /// <summary>
        /// Hidden state name for BusyIndicator.
        /// </summary>
        public const string StateHidden = "Hidden";

        /// <summary>
        /// BusyDisplay group.
        /// </summary>
        public const string GroupVisibility = "VisibilityStates";
        #endregion
    }
}
