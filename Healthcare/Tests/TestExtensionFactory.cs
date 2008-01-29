using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Tests
{
    class TestExtensionFactory : IExtensionFactory
    {
        #region IExtensionFactory Members

        public object[] CreateExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter, bool justOne)
        {
            if(extensionPoint.GetType() == typeof(ProcedureStepBuilderExtensionPoint))
            {
                return new object[]
                    {
                        new ModalityProcedureStepBuilder()
                    };
            }

            return new object[]{};
        }

        public ExtensionInfo[] ListExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
