using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Workflow
{
    public class Operations
    {
        public abstract class Operation
        {

        }

        public class CheckIn : Operation
        {
            public void Execute(Order o, Staff currentUserStaff, IWorkflow workflow)
            {
                CollectionUtils.ForEach<RequestedProcedure>(o.RequestedProcedures, new Action<RequestedProcedure>(
                    delegate(RequestedProcedure rp)
                    {
                        CheckInProcedureStep cps = (CheckInProcedureStep)CollectionUtils.SelectFirst<ProcedureStep>(rp.ProcedureSteps,
                                delegate(ProcedureStep step)
                                {
                                    return step is CheckInProcedureStep;
                                });

                        // The CPS should be created when each RP of an order is created, but just in case it's not
                        if (cps == null)
                        {
                            cps = new CheckInProcedureStep(rp);
                            workflow.AddActivity(cps);
                        }

                        cps.Start(currentUserStaff);
                    }));
            }
        }

        public class Cancel : Operation
        {
            public void Execute(Order order, OrderCancelReason reason)
            {
                order.Cancel(reason);
            }
        }

        public class ReconcilePatient : Operation
        {
            public void Execute(List<Patient> patientsToReconcile, IPersistenceContext context)
            {
                // reconcile all patients
                for (int i = 1; i < patientsToReconcile.Count; i++)
                {
                    Reconcile(patientsToReconcile[0], patientsToReconcile[i], context);
                }
            }

            /// <summary>
            /// Reconciles the specified patient to this patient
            /// </summary>
            /// <param name="thisPatient"></param>
            /// <param name="otherPatient"></param>
            /// <param name="context"></param>
            private void Reconcile(Patient thisPatient, Patient otherPatient, IPersistenceContext context)
            {
                if (PatientIdentifierConflictsFound(thisPatient, otherPatient))
                    throw new PatientReconciliationException("assigning authority conflict - cannot reconcile");

                foreach (PatientProfile profile in otherPatient.Profiles)
                {
                    thisPatient.AddProfile(profile);
                }

                foreach (Note note in otherPatient.Notes)
                {
                    thisPatient.Notes.Add(note.Clone());
                }

                OrderSearchCriteria orderCriteria = new OrderSearchCriteria();
                orderCriteria.Patient.EqualTo(otherPatient);
                IList<Order> otherOrders = context.GetBroker<IOrderBroker>().Find(orderCriteria);
                foreach (Order order in otherOrders)
                {
                    order.Patient = thisPatient;
                }

                VisitSearchCriteria visitCriteria = new VisitSearchCriteria();
                visitCriteria.Patient.EqualTo(otherPatient);
                IList<Visit> otherVisits = context.GetBroker<IVisitBroker>().Find(visitCriteria);
                foreach (Visit visit in otherVisits)
                {
                    visit.Patient = thisPatient;
                }
            }

            /// <summary>
            /// Check if any of the profiles in each patient have an Mrn with the same assigning authority.
            /// </summary>
            /// <param name="thisPatient"></param>
            /// <param name="otherPatient"></param>
            /// <returns>Returns true if any profiles for the other patient and any profiles for this patient have an Mrn with the same assigning authority.</returns>
            private bool PatientIdentifierConflictsFound(Patient thisPatient, Patient otherPatient)
            {
                foreach (PatientProfile x in thisPatient.Profiles)
                    foreach (PatientProfile y in otherPatient.Profiles)
                        if (x.Mrn.AssigningAuthority.Equals(y.Mrn.AssigningAuthority))
                            return true;

                return false;
            }
        }
    }
}
