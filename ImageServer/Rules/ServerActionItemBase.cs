using System;
using ClearCanvas.Common.Actions;

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
    /// Base class for all server rule actions implementing <see cref="IActionItem{ServerActionContext}"/> 
    /// </summary>
    public abstract class ServerActionItemBase : IActionItem<ServerActionContext>
    {
        #region Private Members
        private string _failureReason = "Success";
        private string _name;
        #endregion

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

        #endregion

        #region Public Static Methods
        /// <summary>
        /// Calculates the new time of the specified time, offset by a specified period.
        /// </summary>
        /// <param name="start">Starting time</param>
        /// <param name="offset">The offset period</param>
        /// <param name="unit">The unit of the offset period</param>
        /// <returns></returns>
        public static DateTime CalculateOffsetTime(DateTime start, int offset, TimeUnit unit)
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
                    time = time.AddDays(offset * 7);
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
        #endregion

        #region Protected Abstract Methods
        /// <summary>
        /// Called to execute the action.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true if the action execution succeeds. false otherwise.</returns>
        protected abstract bool OnExecute(ServerActionContext context);
        #endregion
    }
}