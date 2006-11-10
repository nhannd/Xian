using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Services
{
    public interface IAdtReferenceDataService : IHealthcareServiceLayer
    {
        /// <summary>
        /// Return all facility options
        /// </summary>
        /// <returns></returns>
        IList<Facility> GetFacilities();

        /// <summary>
        /// Add a facility
        /// </summary>
        /// <returns></returns>
        void AddFacility(Facility facility);
    }
}
