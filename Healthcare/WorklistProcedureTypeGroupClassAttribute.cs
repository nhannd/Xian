using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// When applied to a subclass of <see cref="Worklist"/>, declares the subclass of <see cref="ProcedureTypeGroup"/>
    /// that the worklist is based on.
    /// </summary>
    /// <remarks>
    /// This attribute is inherited.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WorklistProcedureTypeGroupClassAttribute : Attribute
    {
        private readonly Type _procedureTypeGroupClass;

        public WorklistProcedureTypeGroupClassAttribute(Type procedureTypeGroupClass)
        {
            _procedureTypeGroupClass = procedureTypeGroupClass;
        }

        public Type ProcedureTypeGroupClass
        {
            get { return _procedureTypeGroupClass; }    
        }
    }
}
