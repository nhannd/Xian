using System;
using System.Collections.Generic;

using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.PatientReconciliation
{
    public interface IPatientReconciliationStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetProfile"></param>
        /// <returns></returns>
        IList<PatientProfileMatch> FindReconciliationMatches(PatientProfile targetProfile, IPersistenceContext context);
    }
}
