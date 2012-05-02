#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ComponentModel;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageViewer.Shreds.DiskspaceManager
{
	internal sealed partial class DiskspaceManagerSettings
	{
		public const float LowWaterMarkDefault = 60F;
		public const float HighWaterMarkDefault = 80F;
		public const int CheckFrequencyDefault = 10;
		public const int StudyLimitDefault = 500;
		public const int MinStudyLimitDefault = 30;
		public const int MaxStudyLimitDefault = 10000;

		private static readonly Proxy _instance = new Proxy();

		public static Proxy Instance
		{
			get { return _instance; }
		}

		public sealed class Proxy
		{
			internal Proxy() {}

			private object this[string propertyName]
			{
				get { return Default[propertyName]; }
				set { ApplicationSettingsExtensions.SetSharedPropertyValue(Default, propertyName, value); }
			}

			[DefaultValue(LowWaterMarkDefault)]
			public float LowWatermark
			{
				get { return (float) this["LowWatermark"]; }
				set { this["LowWatermark"] = value; }
			}

			[DefaultValue(HighWaterMarkDefault)]
			public float HighWatermark
			{
				get { return (float) this["HighWatermark"]; }
				set { this["HighWatermark"] = value; }
			}

			[DefaultValue(CheckFrequencyDefault)]
			public int CheckFrequency
			{
				get { return (int) this["CheckFrequency"]; }
				set { this["CheckFrequency"] = value; }
			}

			[DefaultValue(false)]
			public bool EnforceStudyLimit
			{
				get { return (bool) this["EnforceStudyLimit"]; }
				set { this["EnforceStudyLimit"] = value; }
			}

			[DefaultValue(MinStudyLimitDefault)]
			public int MinStudyLimit
			{
				get { return (int) this["MinStudyLimit"]; }
				set { this["MinStudyLimit"] = value; }
			}

			[DefaultValue(MaxStudyLimitDefault)]
			public int MaxStudyLimit
			{
				get { return (int) this["MaxStudyLimit"]; }
				set { this["MaxStudyLimit"] = value; }
			}

			[DefaultValue(StudyLimitDefault)]
			public int StudyLimit
			{
				get { return (int) this["StudyLimit"]; }
				set { this["StudyLimit"] = Math.Min(Math.Max(value, MinStudyLimit), MaxStudyLimit); }
			}

			public void Save()
			{
				Default.Save();
			}
		}
	}
}