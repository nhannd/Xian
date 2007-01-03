using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Services
{
    public interface IFacilityAdminService : IHealthcareServiceLayer
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
        /// Add a facility
        /// </summary>
        /// <returns></returns>
        void AddFacility(Facility facility);

        /// <summary>
        /// Update a facility
        /// </summary>
        /// <returns></returns>
        void UpdateFacility(Facility facility);

        /// <summary>
        /// Loads the Facility for the specified Facility reference
        /// </summary>
        /// <param name="facilityRef"></param>
        /// <returns></returns>
        Facility LoadFacility(EntityRef<Facility> facilityRef);
    }
}
