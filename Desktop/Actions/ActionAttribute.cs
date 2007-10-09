using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Abstract base class for the set of attributes that are used to specify actions declaratively 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public abstract class ActionAttribute : Attribute
    {
        private readonly string _actionID;

        /// <summary>
        /// Attribute constructor.
        /// </summary>
        /// <param name="actionID">A logical action identifier</param>
        public ActionAttribute(string actionID)
        {
            _actionID = actionID;
        }

        /// <summary>
        /// Returns the logical action ID qualified by the type name of the specified target object.
        /// </summary>
        /// <param name="target">The object whose type should be used to qualify the action ID.</param>
        /// <returns>The qualified action ID</returns>
        public string QualifiedActionID(object target)
        {
            // create a fully qualified action ID
            return string.Format("{0}:{1}", target.GetType().FullName, _actionID);
        }

        /// <summary>
        /// Applies this attribute to the specified <see cref="IActionBuildingContext"/>.
        /// </summary>
        /// <param name="builder">The builder to which this attribute should be applied</param>
        public abstract void Apply(IActionBuildingContext builder);
    }
}
