using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using System.IO;

namespace ClearCanvas.Enterprise.Core.Imex
{
    public abstract class ImexApplicationBase : IApplicationRoot
    {
        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            ImexCommandLine cmdLine = new ImexCommandLine();

            try
            {
                cmdLine.Parse(args);
                Execute(cmdLine);
            }
            catch (CommandLineException e)
            {
                Console.WriteLine(e.Message);
                cmdLine.PrintUsage(Console.Out);
                Console.WriteLine("List of supported data-classes:");
                ImexUtils.PrintImexDataClasses(Console.Out);
            }
            catch (NotSupportedException e)
            {
                Console.WriteLine("Invalid data class: " + e.Message);
                Console.WriteLine("List of supported data-classes:");
                ImexUtils.PrintImexDataClasses(Console.Out);
            }
        }

        #endregion

        /// <summary>
        /// Finds the imex that supports the specified data-class.
        /// </summary>
        /// <param name="dataClass"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Indicates that no imex was found that supports the specified data-class.</exception>
        protected IXmDataImex FindImexForDataClass(string dataClass)
        {
            return (IXmDataImex)new XmlDataImexExtensionPoint().CreateExtension(
                delegate(ExtensionInfo info)
                {
                    return CollectionUtils.Contains(AttributeUtils.GetAttributes<ImexDataClassAttribute>(info.ExtensionClass),
                        delegate(ImexDataClassAttribute a)
                        {
                            return a != null && a.DataClass.Equals(
                                dataClass, StringComparison.InvariantCultureIgnoreCase);
                        });
                });
        }

        protected abstract void Execute(ImexCommandLine cmdLine);
    }
}
