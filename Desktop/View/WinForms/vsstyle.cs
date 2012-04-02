#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Adapted from C:\Program Files\Microsoft SDKs\Windows\v6.0A\Include\vsstyle.h
	/// </summary>
	public class VsStyles
	{
		public class ProgressBar
		{
			public const string ProgressStyle = "PROGRESSSTYLE";
			public const string Progress = "PROGRESS";

			public class ProgressParts
			{
				public const int PP_BAR = 1;
				public const int PP_BARVERT = 2;
				public const int PP_CHUNK = 3;
				public const int PP_CHUNKVERT = 4;
				public const int PP_FILL = 5;
				public const int PP_FILLVERT = 6;
				public const int PP_PULSEOVERLAY = 7;
				public const int PP_MOVEOVERLAY = 8;
				public const int PP_PULSEOVERLAYVERT = 9;
				public const int PP_MOVEOVERLAYVERT = 10;
				public const int PP_TRANSPARENTBAR = 11;
				public const int PP_TRANSPARENTBARVERT = 12;
			};



			public class TransparentBarStates
			{
				public const int PBBS_NORMAL = 1;
				public const int PBBS_PARTIAL = 2;
			};

			public class TransparentBarVertStates
			{
				public const int PBBVS_NORMAL = 1;
				public const int PBBVS_PARTIAL = 2;
			};

			public class FillStates
			{
				public const int PBFS_NORMAL = 1;
				public const int PBFS_ERROR = 2;
				public const int PBFS_PAUSED = 3;
				public const int PBFS_PARTIAL = 4;
			};

			public class FillVertStates
			{
				public const int PBFVS_NORMAL = 1;
				public const int PBFVS_ERROR = 2;
				public const int PBFVS_PAUSED = 3;
				public const int PBFVS_PARTIAL = 4;
			};
		}
	}
}
