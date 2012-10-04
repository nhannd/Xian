#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Distribution.Update.Services
{
	internal class ReleaseNames
	{
		public const string Development = "Development";
		public const string DailyBuild = "Daily Build";
		public const string Alpha = "Alpha";
		public const string Beta = "Beta";
		public const string Official = "";
	}

	internal class ComponentNames
	{
		public const string ImageServer = "ClearCanvas ImageServer";
		public const string Workstation = "ClearCanvas Workstation";
	}

	internal class EditionNames
	{
		public const string CommunityOld = "";
		public const string Community = "Community";
		public const string Clinical = "Clinical";
		public const string SmallClinic = "Small Clinic";
	}

	internal class VersionSuffixes
	{
		public const string SP1 = "SP1";
		public const string SP2 = "SP2";
		public const string SP3 = "SP3";
		public const string RC1 = "RC1";
		public const string RC2 = "RC2";
		public const string RC3 = "RC3";
	}

	internal class WorkstationVersions
	{
		public const string v15SP1 = "1.5.11250.33957";
		public const string v20SP1 = "2.0.12729.37986";
		public const string v30ClinicalBeta = "3.0.14992.45038";
		public const string v30ClinicalOfficial = "3.0.15000.50000";
	}

	internal class DownloadUrls
	{
		public const string Default = "http://www.clearcanvas.ca/dnn/Downloads/tabid/70/Default.aspx";
	}
}