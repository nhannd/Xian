#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Text;

namespace ClearCanvas.Distribution.Update.Services
{
	//We'll continue to use this one, for now.
	[Obsolete("Product is CC RIS/PACS.")]
	[Serializable]
	public class Product : Component
	{
	}

	[Serializable]
	public class Component
	{
		public string Name = "";
		public string Edition = "";
		public string Version = "";
		public string VersionSuffix = "";
		public string Release = "";

		internal static readonly Component Empty = new Component();

		internal bool IsCommunityEdition
		{
			get { return (Edition ?? String.Empty) == EditionNames.CommunityOld || Edition == EditionNames.Community; }
		}

		internal bool IsOfficialRelease
		{
			get { return (Release ?? String.Empty) == ReleaseNames.Official; }
		}

		internal void Validate()
		{
			if (String.IsNullOrEmpty(Name))
				throw new ApplicationException("Component name must be specified.");
			if (String.IsNullOrEmpty(Version))
				throw new ApplicationException("Component version must be specified.");

			try
			{
				GetVersion(); //will throw if invalid.
			}
			catch (Exception e)
			{
				string message = String.Format("Component version must be valid ({0}).", Version);
				throw new ApplicationException(message, e);
			}

			if (VersionSuffix == null)
				VersionSuffix = String.Empty;

			if (Release == null)
				Release = String.Empty;

			if (Edition == null)
				Edition = String.Empty;
		}

		internal Version GetVersion()
		{
			return new Version(Version);
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			builder.Append(String.IsNullOrEmpty(Name) ? "Unknown" : Name);
			builder.AppendFormat(" {0}", String.IsNullOrEmpty(Version) ? "?" : Version);

			if (!String.IsNullOrEmpty(Edition))
				builder.AppendFormat(" {0}", Edition);

			if (!String.IsNullOrEmpty(VersionSuffix))
				builder.AppendFormat(" {0}", VersionSuffix);

			if (!String.IsNullOrEmpty(Release))
				builder.AppendFormat(" {0}", Release);

			return builder.ToString();
		}

		#region Factory Methods

		internal static Component CreateCommunityWorkstation(string version, bool old)
		{
			return new Component
			       	{
			       		Name = ComponentNames.Workstation,
			       		Edition = old ? EditionNames.CommunityOld : EditionNames.Community,
			       		Release = ReleaseNames.Official,
			       		Version = version
			       	};
		}

		internal static Component CreateClinicalWorkstation(string version)
		{
			return new Component
			       	{
			       		Name = ComponentNames.Workstation,
			       		Edition = EditionNames.Clinical,
			       		Release = ReleaseNames.Official,
			       		Version = version
			       	};
		}

		internal static Component CreateSmallClinicWorkstation(string version)
		{
			return new Component
			       	{
			       		Name = ComponentNames.Workstation,
			       		Edition = EditionNames.SmallClinic,
			       		Release = ReleaseNames.Official,
			       		Version = version
			       	};
		}

		internal static Component CreateCommunityImageServer(string version, bool old)
		{
			return new Component
			       	{
			       		Name = ComponentNames.ImageServer,
			       		Edition = old ? EditionNames.CommunityOld : EditionNames.Community,
			       		Release = ReleaseNames.Official,
			       		Version = version
			       	};
		}

		internal static Component CreateSmallClinicImageServer(string version)
		{
			return new Component
			       	{
			       		Name = ComponentNames.ImageServer,
			       		Edition = EditionNames.SmallClinic,
			       		Release = ReleaseNames.Official,
			       		Version = version
			       	};
		}

		#endregion
	}
}