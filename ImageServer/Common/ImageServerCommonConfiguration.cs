#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Dicom.Utilities.Xml;

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// Provides access to the common settings of the image server from external assemlies.
    /// </summary>
    public class ImageServerCommonConfiguration
    {
        /// <summary>
        /// Retrieves the default <see cref="StudyXmlOutputSettings"/> based on the configuration settings.
        /// </summary>
        static public StudyXmlOutputSettings DefaultStudyXmlOutputSettings
        {
            get
            {
                StudyXmlOutputSettings settings = new StudyXmlOutputSettings();
				if (Settings.Default.StudyHeaderIncludePrivateTags)
					settings.IncludePrivateValues = StudyXmlTagInclusion.IncludeTagValue;
				else
					settings.IncludePrivateValues = StudyXmlTagInclusion.IgnoreTag;

				if (Settings.Default.StudyHeaderIncludeUNTags)
					settings.IncludeUnknownTags = StudyXmlTagInclusion.IncludeTagValue;
				else
					settings.IncludeUnknownTags = StudyXmlTagInclusion.IgnoreTag;

                settings.MaxTagLength = Settings.Default.StudyHeaderMaxValueLength;
            	settings.IncludeLargeTags = StudyXmlTagInclusion.IncludeTagExclusion;

            	settings.IncludeSourceFileName = false;
                return settings;
            }
        }

        static public string DefaultStudyRootFolder
        {
            get
            {
                return Settings.Default.DefaultStudyRootFolder;
            }
        }

        static public bool UseReceiveDateAsStudyFolder
        {
            get { return Settings.Default.UseReceiveDateAsFolder; }
        }

        static public int TooManyStudyMoveWarningThreshold
        {
            get
            {
                return Settings.Default.TooManyStudyMoveWarningThreshold;
            }
        }

        public static String TemporaryPath
        {
            get
            {
                return Settings.Default.TemporaryPath;
            }
        }

        public static class Device
        {
            public static short MaxConnections
            {
                get
                {
                    return Settings.Default.DeviceConfig_MaxConnections;
                }
            }
        }

		public static int WorkQueueMaxFailureCount
    	{
    		get
    		{
				return Settings.Default.WorkQueueMaxFailureCount;
    		}
    	}
        
    }
}
