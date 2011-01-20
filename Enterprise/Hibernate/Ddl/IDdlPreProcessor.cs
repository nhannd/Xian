#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Cfg;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Defines an interface to a DDL pre-processor.
	/// </summary>
	/// <remarks>
	/// Pre-processors are executed prior to generating any DDL output, for the purpose of modifying the <see cref="Configuration"/>.
	/// </remarks>
    public interface IDdlPreProcessor
    {
		/// <summary>
		/// Processes the specified configuration.
		/// </summary>
		/// <remarks>
		/// The pre-processor may modify the configuration object.
		/// </remarks>
		/// <param name="config"></param>
        void Process(Configuration config);
    }
}
