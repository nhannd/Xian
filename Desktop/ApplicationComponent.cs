using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
    public abstract class ApplicationComponent : IApplicationComponent
    {
        /// <summary>
        /// This class is intentionally not exposed as an extension point because
        /// it is not intended that any extensions ever be implemented - it is only
        /// used by the StubToolContext
        /// </summary>
        public class StubToolExtensionPoint : ExtensionPoint<ITool>
        {
        }

        private IApplicationComponentHost _host;
        private ToolSet _stubToolSet;

        public ApplicationComponent()
        {
        }

        protected IApplicationComponentHost Host
        {
            get { return _host; }
        }


        #region IApplicationComponent Members

        public void SetHost(IApplicationComponentHost host)
        {
            _host = host;
        }

        /// <summary>
        /// Returns an empty <see cref="IToolSet"/>.  Subclasses may override this property if they
        /// wish to expose a toolset to the framework.
        /// </summary>
        public virtual IToolSet ToolSet
        {
            get
            {
                if (_stubToolSet == null)
                {
                    _stubToolSet = new ToolSet(new StubToolExtensionPoint(), new ToolContext());
                }
                return _stubToolSet;
            }
        }

        #endregion
    }
}
