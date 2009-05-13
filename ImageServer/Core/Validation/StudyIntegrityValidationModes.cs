using System;

namespace ClearCanvas.ImageServer.Core.Validation
{
    [Flags]
    public enum StudyIntegrityValidationModes
    {
        
        /// <summary>
        /// Validate all
        /// </summary>
        Default,


        /// <summary>
        /// Do not validate
        /// </summary>
        None,
        
        /// <summary>
        /// Validate the instance count in the database and the xml.
        /// </summary>
        InstanceCount
    }
}