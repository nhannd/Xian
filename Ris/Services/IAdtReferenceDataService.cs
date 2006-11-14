using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Services
{
    public interface IAdtReferenceDataService : IHealthcareServiceLayer
    {
        /// <summary>
        /// Return all facility options
        /// </summary>
        /// <returns></returns>
        IList<Facility> GetAllFacilities();

        /// <summary>
        /// Add a facility
        /// </summary>
        /// <returns></returns>
        void AddFacility(string facilityName);

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
        IList<Location> GetLocations(EntityRef<Facility> facility);

        /// <summary>
        /// Add the specified location
        /// </summary>
        /// <param name="location"></param>
        void AddLocation(Location location);
    }
}
