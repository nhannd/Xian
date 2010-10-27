#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Common.Scripting
{
	/// <summary>
	/// Used by the <see cref="ActiveTemplate"/> class.
	/// </summary>
	[Serializable]
	public class ActiveTemplateException : Exception
    {
		/// <summary>
		/// Constructor.
		/// </summary>
        public ActiveTemplateException(string message, Exception inner)
            :base(message, inner)
        {
        }
    }
}
