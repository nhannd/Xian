using System;
using ClearCanvas.ImageServer.Core.Validation;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    /// <summary>
    /// Attribute to specify what type of validation must be made when a work queue entry is processed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class StudyIntegrityValidationAttribute:Attribute
    {
        #region Private Memebers
        private StudyIntegrityValidationModes _validationTypes;
        #endregion

        #region Public Properties
        public StudyIntegrityValidationModes ValidationTypes
        {
            get { return _validationTypes; }
            set { _validationTypes = value; }
        }
        #endregion
    }
}