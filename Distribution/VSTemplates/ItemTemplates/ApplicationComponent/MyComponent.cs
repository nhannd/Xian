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

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace $rootnamespace$
{
    /// <summary>
    /// Extension point for views onto <see cref="$app_comp$"/>.
    /// </summary>
    [ExtensionPoint]
    public sealed class $app_comp_view_extpoint$ : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// $app_comp$ class.
    /// </summary>
    [AssociateView(typeof($app_comp_view_extpoint$))]
    public class $app_comp$ : ApplicationComponent
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public $app_comp$()
        {
        }

        /// <summary>
        /// Called by the host to initialize the application component.
        /// </summary>
        public override void Start()
        {
            // TODO prepare the component for its live phase
            base.Start();
        }

        /// <summary>
        /// Called by the host when the application component is being terminated.
        /// </summary>
        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }
    }
}
