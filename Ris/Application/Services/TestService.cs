using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Services
{
    [ServiceImplementsContract(typeof(ITestService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class TestService : HealthcareServiceLayer, ITestService
    {
        #region ITestService Members

        [ReadOperation]
        public PatientProfilePreview GetPatientProfilePreview(EntityRef profileRef)
        {
            profileRef = Echo(profileRef);

            PatientProfile profile = (PatientProfile)CurrentContext.Load(profileRef);

            AddressTypeEnumTable addressTypes = CurrentContext.GetBroker<IAddressTypeEnumBroker>().Load();
            TelephoneUseEnumTable phoneUses = CurrentContext.GetBroker<ITelephoneUseEnumBroker>().Load();

            PatientProfilePreview preview = new PatientProfilePreview();
            preview.Name = profile.Name.ToString();
            preview.Mrn = profile.Mrn.ToString();
            preview.Healthcard = profile.Healthcard.ToString();
            preview.DateOfBirth = profile.DateOfBirth.ToString();
            preview.Sex = profile.Sex.ToString();


            preview.Addresses = CollectionUtils.Map<Address, PatientProfilePreview.Address, List<PatientProfilePreview.Address>>(
                profile.Addresses, delegate(Address a)
                {
                    return new PatientProfilePreview.Address(
                        addressTypes[a.Type].Value,
                        a.ToString(),
                        a.ValidRange.Until,
                        a.IsCurrent);
                }).ToArray();

            preview.PhoneNumbers = CollectionUtils.Map<TelephoneNumber, PatientProfilePreview.TelephoneNumber, List<PatientProfilePreview.TelephoneNumber>>(
                profile.TelephoneNumbers, delegate(TelephoneNumber t)
                {
                    string phoneType = (t.Use == TelephoneUse.PRN && t.Equipment == TelephoneEquipment.CP) ?
                        "Mobile" : phoneUses[t.Use].Value;
                    return new PatientProfilePreview.TelephoneNumber(
                        phoneType,
                        t.ToString(),
                        t.ValidRange.Until,
                        t.IsCurrent);
                }).ToArray();

            IPatientProfileBroker broker = CurrentContext.GetBroker<IPatientProfileBroker>();

            IPatientReconciliationStrategy strategy = (IPatientReconciliationStrategy)(new PatientReconciliationStrategyExtensionPoint()).CreateExtension();
            preview.HasUnreconciledMatches = strategy.FindReconciliationMatches(profile, broker).Count > 0;

            return preview;
        }

        [ReadOperation]
        public List<PatientProfilePreview.Address> GetAddresses(EntityRef profileRef)
        {
            PatientProfile profile = (PatientProfile)CurrentContext.Load(profileRef);

            return CollectionUtils.Map<Address, PatientProfilePreview.Address, List<PatientProfilePreview.Address>>(
                profile.Addresses, delegate(Address a)
                {
                    return new PatientProfilePreview.Address(
                        a.Type.ToString(),
                        a.ToString(),
                        a.ValidRange.Until,
                        a.IsCurrent);
                });
        }

        public string GetName(int i)
        {
            return "fred";
        }

        [ReadOperation]
        public EntityRef Echo(EntityRef profileRef)
        {
            return profileRef;
        }


        #endregion
    }
}
