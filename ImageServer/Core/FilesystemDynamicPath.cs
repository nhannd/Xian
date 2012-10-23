#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Common;
using System.IO;
using ClearCanvas.ImageServer.Core.ModelExtensions;

namespace ClearCanvas.ImageServer.Core
{
    /// <summary>
    /// Represents an abstract path which can be converted into real path based on 
    /// the current system settings. Use this class if the path contains parts which may be configured (eg Partition Folder Name, Filesystem root)
    /// </summary>
    public sealed class FilesystemDynamicPath
    {
        public enum PathType
        {
            /// <summary>
            /// Path is absolute
            /// </summary>
            Absolute,

            /// <summary>
            /// Path is relative to the partition's Reconcile folder on the same filesystem where the study is stored
            /// </summary>
            RelativeToReconcileFolder,

            /// <summary>
            /// Path is relative to the study folder
            /// </summary>
            RelativeToStudyFolder
        }

        #region Constants

        const string PathTypeBeginMarkers = "$";
        const string PathTypeEndMarkers = ">>";

        #endregion

        #region Private Fields

        private PathType _type;
        private string _path;

        #endregion

        #region Constructor

        /// <summary>
        ///  Constructs a path 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        public FilesystemDynamicPath(string path, PathType type)
        {
            Platform.CheckForEmptyString(path, "path");
            _path = path;
            _type = type;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the type of the path represented by this instance
        /// </summary>
        public PathType Type
        {
            get { return _type; }
        }

        public override string ToString()
        {
            if (_type == PathType.Absolute)
                return _path;
            else
            {
                var value = new StringBuilder();
                value.Append(PathTypeBeginMarkers);
                value.Append(_type.ToString());
                value.Append(PathTypeEndMarkers);
                value.Append(_path);
                return value.ToString();
            }
        }

        /// <summary>
        /// Indicates if the path is a relative path
        /// </summary>
        public bool IsRelative
        {
            get
            {
                return _type != PathType.Absolute;
            }
        }

        #endregion

        /// <summary>
        /// Computes and returns the absolute path specified by this <see cref="FilesystemDynamicPath"/> for the <see cref="StudyStorageLocation"/>.
        /// Note: The path may or may not be valid.
        /// </summary>
        /// <param name="studyStorage"></param>
        /// <returns></returns>
        public string ConvertToAbsolutePath(StudyStorageLocation studyStorage)
        {
            switch (_type)
            {
                case PathType.Absolute: return _path;

                case PathType.RelativeToReconcileFolder:
                    var basePath = studyStorage.GetReconcileRootPath();
                    return Path.Combine(basePath, _path);

                case PathType.RelativeToStudyFolder:
                    basePath = studyStorage.GetStudyPath();
                    return Path.Combine(basePath, _path);

                default:
                    return _path;
            }
        }


        #region Helpers

        /// <summary>
        /// Converts a string to <see cref="FilesystemDynamicPath"/>. For relative path, the string must start with one of following formats:
        /// "$RelativeToStudyFolder>>" or "$RelativeToPartitionFolder>>"
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FilesystemDynamicPath Parse(string value)
        {
            Platform.CheckForEmptyString(value, "value");


            var isRelative = value.StartsWith(PathTypeBeginMarkers) && value.Contains(PathTypeEndMarkers);
            if (!isRelative)
                return new FilesystemDynamicPath(value, PathType.Absolute);

            PathType pathType;
            var startIndex = PathTypeBeginMarkers.Length;
            var endIndex = value.IndexOf(PathTypeEndMarkers);

            string type = value.Substring(startIndex, endIndex - startIndex);
            if (!Enum.TryParse<PathType>(type, true, out pathType))
                throw new Exception(string.Format("coud not convert {0} to a FilesystemDynamicPath. The specified type ('{1}') is unknown", value, type));

            startIndex = value.IndexOf(PathTypeEndMarkers) + PathTypeEndMarkers.Length;
            var path = value.Substring(startIndex);
            return new FilesystemDynamicPath(path, pathType);
        }

        #endregion
    }
}
