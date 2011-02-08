#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
// 
// For information about the licensing and copyright of this software please
// contact ClearCanvas, Inc. at info@clearcanvas.ca

#endregion

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Web;
using System.Web.Compilation;
using System.Web.UI;

namespace ClearCanvas.ImageServer.Web.Common
{
    class ImageExpressionBuilder : ExpressionBuilder
    {
        public override System.CodeDom.CodeExpression GetCodeExpression(System.Web.UI.BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
        {
            return new CodePrimitiveExpression(entry.Expression.Trim());
        }
    }
}