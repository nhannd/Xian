using ClearCanvas.Common.Specifications;

namespace ClearCanvas.ImageServer.Rules
{
    /// <summary>
    /// Base class for server rule action operator compiler.
    /// </summary>
    public abstract class ActionOperatorCompilerBase
    {
        
        #region Private Memebers
        private string _operatorTag;
        private static readonly IExpressionFactory _defaultExpressionFactory = new ExpressionFactoryExtensionPoint().CreateExtension("dicom");
        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="ActionOperatorCompilerBase"/>
        /// </summary>
        /// <param name="operatorTag">The operator tag for the operator</param>
        public ActionOperatorCompilerBase(string operatorTag)
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

        #region Protected Static Methods

        protected static IExpressionFactory CreateExpressionFactory(string language)
        {
            IExpressionFactory factory = new ExpressionFactoryExtensionPoint().CreateExtension(language);
            if (factory == null)
            {
                return _defaultExpressionFactory;
            }
            else
                return factory;
        }

        protected static Expression CreateExpression(string text, string language)
        {
            IExpressionFactory exprFactory = CreateExpressionFactory(language);
            if (language != null)
                exprFactory = _defaultExpressionFactory;

            return exprFactory.CreateExpression(text);
        }

        protected static Expression CreateExpression(string text)
        {
            return _defaultExpressionFactory.CreateExpression(text);
        }

        #endregion
    }
}