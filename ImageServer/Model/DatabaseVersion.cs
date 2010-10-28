#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageServer.Model
{
	public partial class DatabaseVersion
	{
		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types.
			if (obj == null || GetType() != obj.GetType()) return false;

			DatabaseVersion a = (DatabaseVersion)obj;

			return a.Build.Equals(Build) 
				&& a.Minor.Equals(Minor)
				&& a.Major.Equals(Major)
				&& a.Revision.Equals(Revision);
		}

		public override int GetHashCode()
		{
			return Build.GetHashCode() + Minor.GetHashCode() + Major.GetHashCode() + Revision.GetHashCode();
		}

		public string GetVersionString()
		{
			return string.Format("{0}.{1}.{2}.{3}", Major ?? "0", Minor ?? "0", Build ?? "0", Revision ?? "0");
		}
	}
}
