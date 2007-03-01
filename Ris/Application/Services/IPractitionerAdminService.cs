using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Services
{
    public interface IPractitionerAdminService : IHealthcareServiceLayer
    {
        /// <summary>
        /// Search for a practitioner by name
        /// </summary>
        /// <param name="surname">The practitioner surname to search for.  May not be null</param>
        /// <param name="givenName">The practitioner givenname to search for.  May be null</param>
        /// <returns>A list of matching practitioners</returns>
        IList<Practitioner> FindPractitioners(string surname, string givenName);

        /// <summary>
        /// Return all practitioner
        /// </summary>
        /// <returns>A list of all practitioners</returns>
        IList<Practitioner> GetAllPractitioners();

        /// <summary>
        /// Add a practitioner
        /// </summary>
        /// <param name="practitioner"></param>
        void AddPractitioner(Practitioner practitioner);

        /// <summary>
        /// Update a practitioner
        /// </summary>
        /// <param name="practitioner"></param>
        /// <returns></returns>
        void UpdatePractitioner(Practitioner practitioner);
        
        /// <summary>
        /// Load a practitioner from an entity ref
        /// </summary>
        /// <param name="practitionerRef"></param>
        /// <param name="withDetails">If true, will also load the related detail collections</param>
        /// <returns></returns>
        Practitioner LoadPractitioner(EntityRef practitionerRef, bool withDetails);
    }
}
