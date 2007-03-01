using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    public interface ILocationAdminService
    {
        /// <summary>
        /// Return all location options
        /// </summary>
        /// <returns></returns>
        IList<Location> GetAllLocations();

        /// <summary>
        /// Get all locations for a specific facility
        /// </summary>
        /// <param name="facility"></param>
        /// <returns></returns>
        IList<Location> GetLocations(EntityRef facility);

        /// <summary>
        /// Add the specified location
        /// </summary>
        /// <param name="location"></param>
        void AddLocation(Location location);

        /// <summary>
        /// Update the specified location
        /// </summary>
        /// <param name="location"></param>
        void UpdateLocation(Location location);

        /// <summary>
        /// Loads the location for the specified location reference
        /// </summary>
        /// <param name="locationRef"></param>
        /// <returns></returns>
        Location LoadLocation(EntityRef locationRef);
    }
}
