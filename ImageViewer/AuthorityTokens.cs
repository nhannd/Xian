#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Authorization;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Common viewer authority tokens.
	/// </summary>
	public class AuthorityTokens
	{
		/// <summary>
		/// Permission required in order to see any viewer components (e.g. without this, all viewer components are hidden).
		/// </summary>
		[AuthorityToken(Description = "Permission required in order to see any viewer components (e.g. without this, all viewer components are hidden).")]
		public const string ViewerVisible = "Viewer/Visible";

		/// <summary>
		/// Study tokens.
		/// </summary>
		public class Study
		{
			/// <summary>
			/// Permission to open a study in the viewer.
			/// </summary>
			[AuthorityToken(Description = "Permission to open a study in the viewer.")]
			public const string Open = "Viewer/Study/Open";

            [AuthorityToken(Description = "Permission to send a study to another DICOM device (e.g. another workstation or PACS).")]
            public const string Send = "Viewer/Study/Send";

            [AuthorityToken(Description = "Permission to delete a study from the local store.")]
            public const string Delete = "Viewer/Study/Delete";

            [AuthorityToken(Description = "Permission to retrieve a study to the local store.")]
            public const string Retrieve = "Viewer/Study/Retrieve";

            [AuthorityToken(Description = "Permission to import study data into the local store.")]
            public const string Import = "Viewer/Study/Import";
        }

	    public class ActivityMonitor
	    {
            [AuthorityToken(Description = "Permission to view the Activity Monitor.")]
            public const string View = "Viewer/Activity Monitor/View";
            
            public class WorkItems
            {
                [AuthorityToken(Description = "Permission to stop a work item.")]
                public const string Stop = "Viewer/Activity Monitor/Work Items/Stop";

                [AuthorityToken(Description = "Permission to restart a work item.")]
                public const string Restart = "Viewer/Activity Monitor/Work Items/Restart";

                [AuthorityToken(Description = "Permission to delete a work item.")]
                public const string Delete = "Viewer/Activity Monitor/Work Items/Delete";

                [AuthorityToken(Description = "Permission to stat or re-prioritize work items.")]
                public const string Prioritize = "Viewer/Activity Monitor/Work Items/Prioritize";
            }
        }


	    public static class Administration
	    {
	        [AuthorityToken(Description = "Permission to re-index the local file store.", Formerly = "Viewer/Administration/Reindex Local Data Store")]
	        public const string ReIndex = "Viewer/Administration/Re-index";

	        [AuthorityToken(Description = "Allow administration of the viewer services (e.g. Start/Stop/Restart).")]
	        public const string Services = "Viewer/Administration/Services";
	    }
	}
}
