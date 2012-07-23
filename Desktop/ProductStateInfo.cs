using System;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{

    /// <summary>
    /// Represents the interface of a class that can interpret the license information
    /// </summary>
    /// <remarks>
    /// For internal framework use only.
    /// </remarks>
    public interface IDesktopProductLicenseInfoInterpreter
    {
        bool IsEvaluation { get; }
        string GetTitlebarText();
        string GetProductStateDescription();
    }

    [ExtensionPoint]
    public sealed class DesktopLicenseInfoInterpreterExtensionPoint: ExtensionPoint<IDesktopProductLicenseInfoInterpreter>
    {
        internal DesktopLicenseInfoInterpreterExtensionPoint() { }

        internal static IDesktopProductLicenseInfoInterpreter CreateInstance()
		{
			try
			{
				// check for a provider extension
                return (IDesktopProductLicenseInfoInterpreter)new DesktopLicenseInfoInterpreterExtensionPoint().CreateExtension();
			}
			catch (NotSupportedException)
			{
                return new DefaultDesktopLicenseInfoInterpreter();
			}
		}

        private sealed class DefaultDesktopLicenseInfoInterpreter : IDesktopProductLicenseInfoInterpreter
		{
            public bool IsEvaluation
            {
                get { return false; }
            }

            public string GetTitlebarText()
            {
                return null;
            }

            public string GetProductStateDescription()
            {
                return null;
            }
		}
    }

    /// <summary>
    /// Helper class to retrieve information related to the current state of the product (usually determined on the license)
    /// </summary>
    public static class ProductStateInfo
    {
        private static readonly IDesktopProductLicenseInfoInterpreter _licenseInterpreter;

        static ProductStateInfo()
        {
            _licenseInterpreter = DesktopLicenseInfoInterpreterExtensionPoint.CreateInstance();
        }

        public static bool IsEvaluationCopy
        {
            get
            {
                return _licenseInterpreter == null ? false: _licenseInterpreter.IsEvaluation;
            }
        }

        public static string GetProductLicenseStateDescription()
        {
            return _licenseInterpreter == null ? null : _licenseInterpreter.GetProductStateDescription();
        }

        public static string GetTitlebarText()
        {
            return _licenseInterpreter == null ? null : _licenseInterpreter.GetTitlebarText();
        }
    }
}