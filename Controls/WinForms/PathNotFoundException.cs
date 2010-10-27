#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.Runtime.Serialization;

namespace ClearCanvas.Controls.WinForms
{
	[Serializable]
	public class PathNotFoundException : DirectoryNotFoundException
	{
		private static readonly string _defaultMessage = new DirectoryNotFoundException().Message;
		private readonly string _path = null;

		public PathNotFoundException() : base() {}

		public PathNotFoundException(string path)
		{
			_path = path;
		}

		public PathNotFoundException(string message, string path) : base(message)
		{
			_path = path;
		}

		public PathNotFoundException(string path, Exception inner) : base(_defaultMessage, inner)
		{
			_path = path;
		}

		public PathNotFoundException(string message, string path, Exception inner) : base(message, inner)
		{
			_path = path;
		}

		protected PathNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {}

		public string Path
		{
			get { return _path; }
		}
	}
}