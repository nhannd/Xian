﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.832
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.ImageViewer.Common {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0")]
    internal sealed partial class MemoryManagementSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static MemoryManagementSettings defaultInstance = ((MemoryManagementSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new MemoryManagementSettings())));
        
        public static MemoryManagementSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public long HighWatermarkMegaBytes {
            get {
                return ((long)(this["HighWatermarkMegaBytes"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public long LowWatermarkMegaBytes {
            get {
                return ((long)(this["LowWatermarkMegaBytes"]));
            }
        }
    }
}
