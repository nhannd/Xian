#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Rules.Specifications;

namespace ClearCanvas.ImageServer.Rules
{
    /// <summary>
    /// Defines time unitS used in server rule actions
    /// </summary>
    public enum TimeUnit
    {
        Minutes,
        Hours,
        Days,
        Weeks,
        Months,
        Years
    }


    /// <summary>
    /// Exception that is thrown when value conversion fails.
    /// </summary>
    public class TypeConversionException : Exception
    {
        public TypeConversionException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Base class for all server rule actions implementing <see cref="IActionItem{ServerActionContext}"/> 
    /// </summary>
    public abstract class ServerActionItemBase : IActionItem<ServerActionContext>
    {
        private string _failureReason = "Success";
        private string _name;

        #region Constructors

        public ServerActionItemBase(string name)
        {
            Name = name;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the action
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the description of the failure when the action execution fails.
        /// </summary>
        public string FailureReason
        {
            get { return _failureReason; }
            set { _failureReason = value; }
        }

        #endregion

        #region IActionItem<ServerActionContext> Members

        public bool Execute(ServerActionContext context)
        {
            try
            {
                return OnExecute(context);
            }
            catch (Exception e)
            {
                FailureReason = String.Format("{0} {1}", e.Message, e.StackTrace);
                return false;
            }
        }

        #region Public Static Methods

        /// <summary>
        /// Calculates the new time of the specified time, offset by a specified period.
        /// </summary>
        /// <param name="start">Starting time</param>
        /// <param name="offset">The offset period</param>
        /// <param name="unit">The unit of the offset period</param>
        /// <returns></returns>
        protected static DateTime CalculateOffsetTime(DateTime start, int offset, TimeUnit unit)
        {
            DateTime time = start;

            switch (unit)
            {
                case TimeUnit.Minutes:
                    time = time.AddMinutes(offset);
                    break;

                case TimeUnit.Hours:
                    time = time.AddHours(offset);
                    break;

                case TimeUnit.Days:
                    time = time.AddDays(offset);
                    break;

                case TimeUnit.Weeks:
                    time = time.AddDays(offset*7);
                    break;

                case TimeUnit.Months:
                    time = time.AddMonths(offset);
                    break;

                case TimeUnit.Years:
                    time = time.AddYears(offset);
                    break;

                default:
                    break;
            }

            return time;
        }


    	/// <summary>
        /// Evaluates an expression in the specified context.
        /// </summary>
        /// <param name="expression">The expression to be evaluated</param>
        /// <param name="context">The context to evaluate the expression</param>
        /// <returns></returns>
        protected static object Evaluate(Expression expression, ServerActionContext context)
        {
            Platform.CheckForNullReference(expression, "expression");
            Platform.CheckForNullReference(context, "context");
            Platform.CheckForNullReference(context.Message, "context.Message");

            return expression.Evaluate(context.Message);
        }

        /// <summary>
        /// Evaluates an expression in the specified context and converts it to the specified type.
        /// </summary>
        /// <typeparam name="T">Expected returne type</typeparam>
        /// <param name="expression">The expression to be evaluated</param>
        /// <param name="context">The context to evaluate the expression</param>
        /// <returns></returns>
        /// <exception cref="TypeConversionException"/> thrown if the value of the expression cannot be converted into specified type</exception>
        protected static T Evaluate<T>(Expression expression, ServerActionContext context)
        {
            object value = Evaluate(expression, context);

            if (value is T)
            {
                // if the expression was evaluated to the same type then just return it
                return (T) value;
            }


            if (typeof (T) == typeof (string))
            {
                return (T) (object) value.ToString();
            }
            else if (typeof (T) == typeof (int))
            {
                int result = int.Parse(value.ToString());
                return (T) (object) result;
            }
            else if (typeof (T) == typeof (uint))
            {
                uint result = uint.Parse(value.ToString());
                return (T) (object) result;
            }
            else if (typeof (T) == typeof (long))
            {
                long result = long.Parse(value.ToString());
                return (T) (object) result;
            }
            else if (typeof (T) == typeof (ulong))
            {
                ulong result = ulong.Parse(value.ToString());
                return (T) (object) result;
            }
            else if (typeof (T) == typeof (float))
            {
                float result = float.Parse(value.ToString());
                return (T) (object) result;
            }
            else if (typeof (T) == typeof (double))
            {
                double result = double.Parse(value.ToString());
                return (T) (object) result;
            }
            else if (typeof (T) == typeof (DateTime))
            {
                if (value == null || String.IsNullOrEmpty(value.ToString()))
                {
                    string error =
                        String.Format("Unable to convert value for expression {0} (value={1}) to {2}", expression.Text,
                                      value, typeof (T));
                    throw new TypeConversionException(error);
                }

                DateTime? result = DateTimeParser.Parse(value.ToString());
                if (result == null)
                {
                    string error =
                        String.Format("Unable to convert value for expression {0} (value={1}) to {2}", expression.Text,
                                      value, typeof (T));
                    throw new TypeConversionException(error);
                    ;
                }
                else
                    return (T) (object) result.Value;
            }
            else
            {
                string error =
                    String.Format("Unable to retrieve value for expression {0} as {1}: Unsupported conversion.",
                                  expression.Text, typeof (T));
                throw new TypeConversionException(error);
            }
        }

        /// <summary>
        /// Evaluates an expression in the specified context and converts it to the specified type.
        /// </summary>
        /// <typeparam name="T">Expected returne type</typeparam>
        /// <param name="expression">The expression to be evaluated</param>
        /// <param name="context">The context to evaluate the expression</param>
        /// <param name="defaultValue">Returned value if the expression cannot be evaluated</param>
        /// <returns></returns>
        protected static T Evaluate<T>(Expression expression, ServerActionContext context, T defaultValue)
        {
            try
            {
                return Evaluate<T>(expression, context);
            }
            catch(NoSuchDicomTagException)
            {
                ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Critical, SR.AlertComponentRules, AlertTypeCodes.InvalidConfiguration,
                               SR.AlertRuleInvalid, expression.Text
                    );

                return defaultValue;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unable to evaluate expression {0}. Using default value={1}", expression.Text, defaultValue);
                return defaultValue;
            }
        }

        #endregion

        #region Protected Abstract Methods

        /// <summary>
        /// Called to execute the action.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true if the action execution succeeds. false otherwise.</returns>
        protected abstract bool OnExecute(ServerActionContext context);

        #endregion

        #endregion
    }
}