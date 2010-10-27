#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// <see cref="IStatisticsContext"/> implemenation class 
    /// </summary>
    public class StatisticsContext : IStatisticsContext
    {
        #region Private Memebers

        private string _id;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="StatisticsContext"/> with a specified ID.
        /// </summary>
        /// <param name="id"></param>
        public StatisticsContext(string id)
        {
            _id = id;
        }

        #endregion

        #region IStatisticsContext Members

        /// <summary>
        /// Gets or sets the ID of the context
        /// </summary>
        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        #endregion
    }
}