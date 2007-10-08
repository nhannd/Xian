using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Specifications
{
    public interface IExpressionFactory
    {
        Expression CreateExpression(string text);
    }
}
