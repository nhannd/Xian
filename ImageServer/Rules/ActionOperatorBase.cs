using System;
using System.Xml;
using System.Xml.Schema;
using ClearCanvas.Common.Actions;

namespace ClearCanvas.ImageServer.Rules
{
    /// <summary>
    /// Base class for server rule action operator
    /// </summary>
    public abstract class ActionOperatorBase: IXmlActionCompilerOperator<ServerActionContext>
    {
        #region Private Members
        private string _operatorTag;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="ActionOperatorBase"/>
        /// </summary>
        /// <param name="operatorTag">The operator tag for the operator</param>
        public ActionOperatorBase(string operatorTag)
        {
            _operatorTag = operatorTag;
        }

        #endregion

        #region Public Properties
        public string OperatorTag
        {
            get { return _operatorTag; }
            set { _operatorTag = value; }
        }

        #endregion
        
        #region IXmlActionCompilerOperator<ServerActionContext> Members

        public abstract IActionItem<ServerActionContext> Compile(XmlElement xmlNode);
        public abstract XmlSchemaElement GetSchema();

        #endregion
    }
}