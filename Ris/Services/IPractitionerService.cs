using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Services
{
    public interface IPractitionerService
    {
        /// <summary>
        /// Search for a practitioner by name
        /// </summary>
        /// <param name="surname">The practitioner surname to search for.  May not be null</param>
        /// <param name="givenName">The practitioner givenname to search for.  May be null</param>
        /// <returns>A list of matching practitioners</returns>
        IList<Practitioner> FindPractitioners(string surname, string givenName);
        
        /// <summary>
        /// Load a practitioner from an entity ref
        /// </summary>
        /// <param name="practitionerRef"></param>
        /// <returns></returns>
        Practitioner LoadPractitioner(EntityRef<Practitioner> practitionerRef);

        /// <summary>
        /// Add a practitioner
        /// </summary>
        /// <param name="practitioner"></param>
        void AddPractitioner(Practitioner practitioner);
    }
}
