#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#pragma warning disable 1591

namespace ClearCanvas.Common.Specifications
{
    public abstract class Expression
    {

        #region NullExpression

        class NullExpression : Expression
        {
            public NullExpression()
                :base(null) { }

            public override object Evaluate(object arg)
            {
                return null;
            }
        }

        public static readonly Expression Null = new NullExpression();

        #endregion


        private string _text;

        public Expression(string text)
        {
            // treat "" as null
            _text = string.IsNullOrEmpty(text) ? null : text;
        }

        public string Text
        {
            get { return _text; }
        }

        public abstract object Evaluate(object arg);

        public override bool Equals(object obj)
        {
            Expression other = obj as Expression;
            return other != null && other._text == this._text;
        }

        public override int GetHashCode()
        {
            return _text == null ? 0 : _text.GetHashCode();
        }
    }
}
