using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    /// <summary>
    /// Extension point for views onto <see cref="AEServerEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class AEServerEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// AEServerEditorComponent class
    /// </summary>
    [AssociateView(typeof(AEServerEditorComponentViewExtensionPoint))]
    public class AEServerEditorComponent : ApplicationComponent
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AEServerEditorComponent()
        {
        }

        public override void Start()
        {
            // TODO prepare the component for its live phase
            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }
    }
}
