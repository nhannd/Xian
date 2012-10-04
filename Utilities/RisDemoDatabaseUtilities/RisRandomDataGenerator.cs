#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Services;
using ClearCanvas.Common;

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities
{
    public class RisRandomDataGenerator
    {
        private static Random _random = new Random();
        private RandomDictionaryHelper _dictionaries;

        private bool _performanceLogging;
        private IEnumerable<InsertionStatistic> _lastPatientInsertionTimes;
        private IEnumerable<InsertionStatistic> _lastVisitInsertionTimes;
        private IEnumerable<InsertionStatistic> _lastOrderInsertionTimes;
        private long _start;
        private long _stop;
        private TimeSpan _span;
        private string _perflog;

        private PatientGeneratorSettings _patientSettings;
        private VisitGeneratorSettings _visitSettings;
        private OrderGeneratorSettings _orderSettings;

        public RisRandomDataGenerator()
        {
            _dictionaries = new RandomDictionaryHelper();

            _patientSettings = new PatientGeneratorSettings();
            _visitSettings = new VisitGeneratorSettings();
            _orderSettings = new OrderGeneratorSettings();
        }

        public RisRandomDataGenerator(bool enablePerformanceLogging) : this()
        {
            _performanceLogging = enablePerformanceLogging;
        }

        

        #region Public Generator Methods
        /// <summary>
        /// Creates a new patient in the database with a randomly generated profile, and returns the <see cref="Patient"/> object
        /// </summary>
        public Patient GeneratePatient()
        {
            IAdtService adtservice = ApplicationContext.GetService<IAdtService>();
            InsertionStatistic insertStat;

            _start = DateTime.Now.Ticks;
            Patient patient = adtservice.CreatePatientForProfile(GeneratePatientProfile());
            _stop = DateTime.Now.Ticks;

            if (_performanceLogging == true)
            {
                TimeSpan _span = new TimeSpan(_stop - _start);
                insertStat.TimeOfSample = DateTime.Now;
                insertStat.ExecutionTime = _span.TotalSeconds;
                _perflog = "Patient Insertion: " + insertStat.ExecutionTime.ToString() + " s";
                Platform.Log(_perflog);

                _lastPatientInsertionTimes = new InsertionStatistic[] { insertStat };
            }

            return patient;
;
        }

        /// <summary>
        /// Creates a pair of new patient in the database with a randomly generated profiles, ready to be reconciled, and returns the <see cref="Patient"/> object
        /// </summary>
        public Patient GenerateReplicationPatient()
        {
            IAdtService adtservice = ApplicationContext.GetService<IAdtService>();
            PatientProfile profile = GeneratePatientProfile();

            adtservice.CreatePatientForProfile(profile);
            return adtservice.CreatePatientForProfile(GenerateReplicationPatientProfile(profile));
        }

        /// <summary>
        /// Creates a random number, between 1 and 4, of visits in the database for the patient and returns the <see cref="Visit"/> objects in an array
        /// </summary>
        public Visit[] GenerateVisits(Patient Patient)
        {
            IList<InsertionStatistic> insertStats = new List<InsertionStatistic>();
            InsertionStatistic insertStat;
            
            Visit[] visits = new Visit[_random.Next(_visitSettings.MinVisits, _visitSettings.MaxVisits + 1)];
            for (int i = 0; i < visits.Length; i++)
            {
                Visit visit = new Visit();
                visit.Patient = Patient;
                do
                {
                    visit.VisitStatus = GenerateVisitStatus();
                } while (visit.VisitStatus == VisitStatus.PND);  //not expecting to use PND
                do
                {
                    visit.PatientClass = GeneratePatientClass();
                } while (visit.PatientClass == PatientClass.P);

                if (visit.VisitStatus == VisitStatus.PRE)
                {
                    visit.PreadmitNumber = GenerateVisitNumber().Id;  //assume for now that PreAdmit Number is like the VisitNumber in format
                    visit.PatientClass = PatientClass.P;
                }

                if (visit.VisitStatus != VisitStatus.PRE && visit.VisitStatus != VisitStatus.PND)
                {
                    //A & D case
                    visit.AdmissionType = GenerateAdmissionType();
                    visit.AdmitDateTime = GetDateTimeWithinLast5YearsOfToday();

                    if (visit.VisitStatus == VisitStatus.D)
                    {
                        //visit.DischargeDateTime is set later

                        visit.DischargeDisposition = "Discharge Disposition";
                    }
                }

                visit.VisitNumber = GenerateVisitNumber();
                visit.PatientType = GeneratePatientType();
                visit.VipIndicator = GenerateVipIndicator();
                visit.Facility = GenerateFacility();

                VisitLocation[] visitLocations = GenerateVisitLocations(visit.Facility, visit.AdmitDateTime, visit.VisitStatus);
                foreach (VisitLocation location in visitLocations)
                {
                    visit.Locations.Add(location);

                    if (visit.Locations.Count == visitLocations.Length)
                    {
                        visit.DischargeDateTime = location.EndTime;
                    }
                }

                VisitPractitioner[] visitPractitioners = GenerateVisitPractitioners();
                foreach (VisitPractitioner practitioner in visitPractitioners)
                {
                    visit.Practitioners.Add(practitioner);
                }

                IAdtService adtservice = ApplicationContext.GetService<IAdtService>();

                _start = DateTime.Now.Ticks;
                adtservice.SaveNewVisit(visit, new EntityRef<Patient>(visit.Patient));
                _stop = DateTime.Now.Ticks;

                if (_performanceLogging == true)
                {
                    _span = new TimeSpan(_stop - _start);
                    insertStat.TimeOfSample = DateTime.Now;
                    insertStat.ExecutionTime = _span.TotalSeconds;
                    _perflog = "Visit Insertion: " + insertStat.ExecutionTime.ToString() + " s";

                    Platform.Log(_perflog);
                    insertStats.Add(insertStat);
                }

                visits[i] = visit;
            }

            if (_performanceLogging == true)
            {
                _lastVisitInsertionTimes = insertStats;
            }

            return visits;
        }

        /// <summary>
        /// Creates a random number, between 1 and 4, of orders in the database for each visit, returns number of orders generated
        /// </summary>
        public int GeneratePlacedOrders(Visit[] visits)
        {
            IList<InsertionStatistic> insertStats = new List<InsertionStatistic>();
            InsertionStatistic insertStat;

            IOrderEntryService orderservice = ApplicationContext.GetService<IOrderEntryService>();
            int totalOrders = 0;

            foreach (Visit visit in visits)
            {
                DateTime schedulingRequestTime = visit.AdmitDateTime != null ? (DateTime)visit.AdmitDateTime : GetDateTimeWithinLast5YearsOfToday();

                int numberoforders = _random.Next(_orderSettings.MinOrders, _orderSettings.MaxOrders + 1);
                for (int i = 0; i < numberoforders; i++)
                {
                    _start = DateTime.Now.Ticks;
                    orderservice.PlaceOrder(visit.Patient, visit, GenerateDiagnosticService(), GenerateOrderPriority(), GetAnExistingPractitioner(), visit.Facility, schedulingRequestTime);
                    _stop = DateTime.Now.Ticks;

                    if (_performanceLogging == true)
                    {
                        _span = new TimeSpan(_stop - _start);
                        insertStat.TimeOfSample = DateTime.Now;
                        insertStat.ExecutionTime = _span.TotalSeconds;
                        _perflog = "Order Insertion: " + insertStat.ExecutionTime.ToString() + " s";
                        
                        Platform.Log(_perflog);
                        insertStats.Add(insertStat);
                    }
                }
                //Platform.ShowMessageBox("Order has been placed for visit " + visit.VisitNumber.Id);

                totalOrders += numberoforders;
            }

            if (_performanceLogging == true)
            {
                _lastOrderInsertionTimes = insertStats;
            }

            return totalOrders;
        }

        #endregion

        #region Settings Accessors & Methods
        public IEnumerable<IEntityGeneratorSettingsList> EntitySettings
        {
            get 
            {
                List<IEntityGeneratorSettingsList> settings = new List<IEntityGeneratorSettingsList>();
                settings.Add(_patientSettings);
                settings.Add(_visitSettings);
                settings.Add(_orderSettings);
                return settings;
            } 
            set 
            {
                foreach (IEntityGeneratorSettingsList entitySettings in value)
                {
                    if (entitySettings is PatientGeneratorSettings)
                    {
                        _patientSettings = (PatientGeneratorSettings)entitySettings;
                    }
                    if (entitySettings is VisitGeneratorSettings)
                    {
                        _visitSettings = (VisitGeneratorSettings)entitySettings;
                    }
                    if (entitySettings is OrderGeneratorSettings)
                    {
                        _orderSettings = (OrderGeneratorSettings)entitySettings;
                    }
                }
            }
        }

        #endregion


        #region Profile Component Generators

        private PatientProfile GeneratePatientProfile()
        {
            PatientProfile profile = new PatientProfile();

            profile.Sex = GenerateSex();
            profile.DeathIndicator = false;
            profile.DateOfBirth = GenerateDateOfBirth();
            profile.Name = GenerateName(profile.Sex);
            profile.Healthcard = GenerateHealthcard();
            profile.Mrn = GenerateMrn();

            foreach (Address address in GenerateAddresses())
            {
                profile.Addresses.Add(address);
            }
            foreach (TelephoneNumber number in GenerateTelephoneNumbers())
            {
                profile.TelephoneNumbers.Add(number);
            }

            return profile;
        }

        private PatientProfile GenerateReplicationPatientProfile(PatientProfile baseProfile)
        {
            PatientProfile newProfile = new PatientProfile();
            CompositeIdentifier newMrn = GenerateMrn();

            newProfile.Sex = baseProfile.Sex;
            newProfile.DeathIndicator = false;
            newProfile.DateOfBirth = baseProfile.DateOfBirth;
            newProfile.Name = baseProfile.Name;

            //newProfile.Healthcard = baseProfile.Healthcard; //retain the old healhcard # if a high probability match is desired
            newProfile.Healthcard = GenerateHealthcard(); //ensures a moderate match only 

            foreach (Address address in baseProfile.Addresses)
            {
                newProfile.Addresses.Add(address);
            }
            foreach (TelephoneNumber number in baseProfile.TelephoneNumbers)
            {
                newProfile.TelephoneNumbers.Add(number);
            }

            if (baseProfile.Mrn.AssigningAuthority == "MSH")
            {
                while (newMrn.AssigningAuthority != "UHN")
                {
                    newMrn = GenerateMrn();
                }
                newProfile.Mrn = newMrn;
            }
            else
            {
                while (newMrn.AssigningAuthority != "MSH")
                {
                    newMrn = GenerateMrn();
                }
                newProfile.Mrn = newMrn;
            }

            return newProfile;

        }

        private Sex GenerateSex()
        {
            return _patientSettings.SexEnumValues[_random.Next(0, _patientSettings.SexEnumValues.Length)]; //Random's max is one more than expected...
        }

        private DateTime GenerateDateOfBirth()
        {
            return new DateTime(_random.Next(1901, 2005),
                                                _random.Next(1, 13), //The Random.Next method is a little strange hence the (1,13)
                                                _random.Next(1, 29)); //eventually fix this so it'll go up to 31 depending on month
        }

        private PersonName GenerateName(Sex sex)
        {
            PersonName name = new PersonName();

            name.FamilyName = _dictionaries.FamilyName;
            if (sex == Sex.M)
            {
                name.GivenName = _dictionaries.MaleName;
                if (_random.Next(0, 5) != 0) //totally arbitrary
                {
                    name.MiddleName = _dictionaries.MaleName;
                }
                if (_random.Next(0, 500) == 0) //again totally arbitrary
                {
                    name.Prefix = "Fr.";
                }
            }
            else if (sex == Sex.F)
            {
                name.GivenName = _dictionaries.FemaleName;
                if (_random.Next(0, 5) != 0) //again totally arbitrary
                {
                    name.MiddleName = _dictionaries.FemaleName;
                }
            }
            else
            {
                name.GivenName = _dictionaries.GivenName;
            }

            if (_random.Next(0, 100) == 0)
            {
                name.Prefix = "Dr.";
                name.Suffix = "M.D.";
            }
            if (_random.Next(0, 500) == 0) //again totally arbitrary
            {
                name.Degree = "Ph.D";
            }

            return name;
        }
 
        private HealthcardNumber GenerateHealthcard()
        {
            HealthcardNumber ohip = new HealthcardNumber();
            ohip.AssigningAuthority = "Ontario";
            ohip.Id = _random.Next(1000, 9999).ToString("####") + _random.Next(100000, 999999).ToString("######");
            ohip.VersionCode = GetUpperCaseLetter() + GetUpperCaseLetter();
            ohip.ExpiryDate = GetFutureDateTimeWithin5YearsOfToday();

            return ohip;
        }

        private CompositeIdentifier GenerateMrn()
        {
            CompositeIdentifier mrn = new CompositeIdentifier();
            mrn.AssigningAuthority = _patientSettings.AssigningAuthorityEnumValues[_random.Next(0, _patientSettings.AssigningAuthorityEnumValues.Length)]; //Random's max is one more than expected...
            if (mrn.AssigningAuthority == "UHN")
            {
                mrn.Id = _random.Next(1000000, 9999999).ToString("#######");
            }
            else
            {
                //ris presently doesn't support searching for ### ### ###
                //mrn.Id = _random.Next(100000000, 999999999).ToString("### ### ###");
                mrn.Id = _random.Next(100000000, 999999999).ToString("#########");
            }

            return mrn;
        }

        private Address GenerateAddress()
        {
            Address address = new Address();

            address.Type = (AddressType)_random.Next(0, Enum.GetValues(typeof(AddressType)).Length); //Random's max is one more than expected...
            address.Street = _random.Next(0, 9999).ToString() + " " + _dictionaries.Street + " " + _dictionaries.StreetType + " " + _patientSettings.AddressStreetDirectionEnumValues[_random.Next(0, _patientSettings.AddressStreetDirectionEnumValues.Length)];  //Random's max is one more than expected...
            address.Street.Trim();
            address.Unit = _random.Next(0,3) == 0 ? _random.Next(1000,3999).ToString() : null;
            address.City = "Toronto";
            address.Province = "Ontario";
            address.PostalCode = "M" + GetDigit() + GetUpperCaseLetter() + " " + GetDigit() + GetUpperCaseLetter() + GetDigit();
            address.Country = "Canada";

            return address;
        }

        private Address[] GenerateAddresses()
        {
            Address address = new Address();
            List<Address> addresses = new List<Address>();
            List<AddressType> typesGenerated = new List<AddressType>();
            DateTime to = DateTime.Today;

            int loop = _random.Next(_patientSettings.MinAddresses, _patientSettings.MaxAddresses); //Random's max is one more than expected

            for (int i = 0; i < loop; i++)
            {
                address = GenerateAddress();

                if (typesGenerated.Contains(address.Type))
                {
                    DateTimeRange prior = addresses.FindLast(delegate(Address a) { return a.Type == address.Type; }).ValidRange;
                    address.ValidRange = new DateTimeRange(new DateTime(_random.Next(prior.From.Value.Year - 5, prior.From.Value.Year),
                                                                        _random.Next(1, 13),
                                                                         _random.Next(1, 29)),
                                                           prior.From);
                }
                else
                {
                    address.ValidRange = new DateTimeRange(new DateTime(_random.Next(to.Year - 5, to.Year),
                                                                   _random.Next(1, 13),
                                                                   _random.Next(1, 29)),
                                                      null);
                }

                addresses.Add(address);
                typesGenerated.Add(address.Type);
            }

            return addresses.ToArray();
        }

        private TelephoneNumber GenerateTelephoneNumber()
        {
            TelephoneNumber telephoneNumber = new TelephoneNumber();

            telephoneNumber.Use = (TelephoneUse)_random.Next(0, Enum.GetValues(typeof(TelephoneUse)).Length); //Random's max is one more than expected...
            telephoneNumber.Equipment = (TelephoneEquipment)_random.Next(0, Enum.GetValues(typeof(TelephoneEquipment)).Length); //Random's max is one more than expected...
            telephoneNumber.CountryCode = "1";
            telephoneNumber.AreaCode = "416";
            telephoneNumber.Number = _random.Next(2000000, 9999999).ToString("#######");
            if (telephoneNumber.Use == TelephoneUse.WPN && telephoneNumber.Equipment != TelephoneEquipment.CP)
            {
                telephoneNumber.Extension = _random.Next(10, 9999).ToString();
            }
            else
            {
                telephoneNumber.Extension = null;
            }

            return telephoneNumber;
        }

        private TelephoneNumber[] GenerateTelephoneNumbers()
        {
            TelephoneNumber number = new TelephoneNumber();
            List<TelephoneNumber> numbers = new List<TelephoneNumber>();
            List<TelephoneUse> typesGenerated = new List<TelephoneUse>();
            DateTime to = DateTime.Today;

            int loop = _random.Next(_patientSettings.MinTelephoneNumbers, _patientSettings.MaxTelephoneNumbers);
            for (int i = 0; i < loop; i++)
            {
                number = GenerateTelephoneNumber();

                if (typesGenerated.Contains(number.Use))
                {
                    DateTimeRange prior = numbers.FindLast(delegate(TelephoneNumber n) { return n.Use == number.Use; }).ValidRange;
                    number.ValidRange = new DateTimeRange(new DateTime(_random.Next(prior.From.Value.Year - 5, prior.From.Value.Year),
                                                                        _random.Next(1, 13),
                                                                         _random.Next(1, 29)),
                                                           prior.From);
                }
                else
                {
                    number.ValidRange = new DateTimeRange(new DateTime(_random.Next(to.Year - 5, to.Year),
                                                   _random.Next(1, 13),
                                                   _random.Next(1, 29)),
                                      null);
                }

                numbers.Add(number);
                typesGenerated.Add(number.Use);
            }

            return numbers.ToArray();
        }
        
        #endregion

        #region Visit Component Generators

        private CompositeIdentifier GenerateVisitNumber()
        {
            CompositeIdentifier visitNumber = new CompositeIdentifier();
            visitNumber.AssigningAuthority = _visitSettings.AssigningAuthorityEnumValues[_random.Next(0, _visitSettings.AssigningAuthorityEnumValues.Length)]; //Random's max is one more than expected...
            if (visitNumber.AssigningAuthority == "UHN")
            {
                visitNumber.Id = _random.Next(1000000, 9999999).ToString("#######");  //actual UHN Format is to be determined
            }
            else
            {
                visitNumber.Id = _random.Next(2007000000, 2007999999).ToString("##########");
            }

            return visitNumber;
        }

        private VisitStatus GenerateVisitStatus()
        {
            return (VisitStatus) _random.Next(0, Enum.GetValues(typeof(VisitStatus)).Length); //Random's max is one more than expected...
        }

        private PatientClass GeneratePatientClass()
        {
            return (PatientClass)_random.Next(0, Enum.GetValues(typeof(PatientClass)).Length); //Random's max is one more than expected...
        }

        private AdmissionType GenerateAdmissionType()
        {
            //Eventually take in patient data and return reasonable Admissiontypes e.g. Labor and Delivery only applies to women
            //AdmissionType adminissionType = (AdmissionType)_random.Next(0, Enum.GetValues(typeof(AdmissionType)).Length - 1);

            //ArrayList profiles = new ArrayList(Patient.Profiles);
            //profiles[0]

            //return adminissionType;

            return (AdmissionType)_random.Next(0, Enum.GetValues(typeof(AdmissionType)).Length); //Random's max is one more than expected...
        }

        private PatientType GeneratePatientType()
        {
            return (PatientType)_random.Next(0, Enum.GetValues(typeof(PatientType)).Length); //Random's max is one more than expected...
        }

        private Facility GenerateFacility()
        {
            IFacilityAdminService facilityAdminService = ApplicationContext.GetService<IFacilityAdminService>();

            IList<Facility> facilities = facilityAdminService.GetAllFacilities();
            if (facilities.Count == 0)
            {
                facilityAdminService.AddFacility("Test Facility");
                facilities = facilityAdminService.GetAllFacilities();
            }

            return facilities[_random.Next(0, facilities.Count)]; 
        }

        private VisitLocation[] GenerateVisitLocations(Facility Facility, DateTime? AdminDateTime, VisitStatus Status)
        {
            ILocationAdminService locationAdminService = ApplicationContext.GetService<ILocationAdminService>();
            //locationAdminService.GetLocations(facilityRef) not yet implemented
            //EntityRef<Facility> facilityRef = new EntityRef<Facility>(Facility);

            //IList<Location> locations = locationAdminService.GetLocations(facilityRef);
            IList<Location> locations = locationAdminService.GetAllLocations();

            if (locations.Count == 0)
            {
                locationAdminService.AddLocation(new Location(Facility, "Building", "Floor", "PointOfCare", "Room", "Bed", true, null));
                //locations = locationAdminService.GetLocations(facilityRef);
                locations = locationAdminService.GetAllLocations();
            }

            VisitLocation[] visitLocations = new VisitLocation[_random.Next(_visitSettings.MinVisitLocations, _visitSettings.MaxVisitLocations + 1)];
            
            VisitLocationRole role = VisitLocationRole.CR;
            Location visitLocation;
            DateTime? startTime = null;
            DateTime? endTime = null;

            for (int i=0; i < visitLocations.Length; i++)
            {
                visitLocation = locations[_random.Next(1, locations.Count)];

                switch (Status)
                {
                    case VisitStatus.PRE:
                        role = VisitLocationRole.PN;
                        break;
                    case VisitStatus.PND:
                        role = VisitLocationRole.PN;
                        break;
                    case VisitStatus.A:
                        if (i == 0)
                        {
                            role = _random.Next(0,100) % 3 == 1 ? VisitLocationRole.CR : VisitLocationRole.PR;
                        }
                        else
                        {
                            if (visitLocations[i-1].Role == VisitLocationRole.PR)
                            {
                                if (i == visitLocations.Length - 1)
                                {
                                    role = VisitLocationRole.CR;
                                }
                                else
                                {
                                //roll the dice for CR, PR, TM
                                do
                                    {
                                        role = (VisitLocationRole)_random.Next(0, Enum.GetValues(typeof(VisitLocationRole)).Length);
                                    } while (role == VisitLocationRole.PN);
                                }
                            }
                            else
                            {
                                role = VisitLocationRole.PN;
                            }
                        }

                        //section that randomizes start/end times
                        if (role == VisitLocationRole.PR || role == VisitLocationRole.CR)
                        {
                            if (i == 0)
                            {
                                startTime = AdminDateTime;
                            }
                            else
                            {
                                startTime = visitLocations[i-1].EndTime;
                            }

                            if (role == VisitLocationRole.PR)
                            {
                                endTime = GetDateTimeInTheFutureButSameDay(startTime);     
                            }
                        }
                        break;
                    case VisitStatus.D:
                        role = VisitLocationRole.PR;

                        if (i == 0)
                        {
                            startTime = AdminDateTime;
                        }
                        else
                        {
                            startTime = visitLocations[i-1].EndTime;
                        }
                        endTime = GetDateTimeInTheFutureButSameDay(startTime);                     
                        
                        break;
                    default:
                        break;
                }


                visitLocations[i] = new VisitLocation(visitLocation, role, startTime, endTime);
            }

            return visitLocations;
        }

        private VisitPractitioner[] GenerateVisitPractitioners()
        {
            //Practitioner newPractitioner = GeneratePractitionerInDatabase();

            Practitioner newPractitioner = GetAnExistingPractitioner();

            VisitPractitioner vp = new VisitPractitioner(newPractitioner, VisitPractitionerRole.AD);

            VisitPractitioner[] visitPractitioners = new VisitPractitioner[] { vp };

            //eventually add more to this
            return visitPractitioners;
        }

        private Practitioner GeneratePractitionerInDatabase()
        {
            Practitioner doctor = new Practitioner(GenerateName(GenerateSex()), GenerateLicenseNumber());
            doctor.Name.Prefix = "Dr.";

            IPractitionerAdminService service = ApplicationContext.GetService<IPractitionerAdminService>();
            
            service.AddPractitioner(doctor);

            return doctor;
        }

        private Practitioner GetAnExistingPractitioner()
        {
            IPractitionerAdminService service = ApplicationContext.GetService<IPractitionerAdminService>();
            IList<Practitioner> practitioners = service.GetAllPractitioners();

            return practitioners[_random.Next(0, practitioners.Count)]; //Random's max is one more than expected...
        }

        private bool GenerateVipIndicator()
        {
            return _random.Next(1, 100) % 5 == 1 ? true : false;
        }

        private string GenerateLicenseNumber()
        {
            return _random.Next(10000, 99999).ToString("#####"); 
        }

        #endregion

        #region Order Component Generators

        private DiagnosticService GenerateDiagnosticService()
        {
            IOrderEntryService service = ApplicationContext.GetService<IOrderEntryService>();
            IList<DiagnosticService> dslist = service.ListDiagnosticServiceChoices();

            return dslist[_random.Next(0, dslist.Count)];
        }

        private OrderPriority GenerateOrderPriority()
        {
            return (OrderPriority)_random.Next(0, Enum.GetValues(typeof(OrderPriority)).Length); //Random's max is one more than expected...
        }

        #endregion

        #region General Functions
        public string GetDigit()
        {
            return _random.Next(0, 10).ToString(); //Random's max is one more than expected...
        }

        public string GetUpperCaseLetter()
        {
            //A = 65
            //Z = 90
            int i = _random.Next(65, 91); //Random's max is one more than expected...
            char letter = (char) i;

            return letter.ToString();
        }

        public DateTime GetDateTimeWithinLast5YearsOfToday()
        {
            //5 years in seconds is 157680000 secs assuming 365 days
            return DateTime.Today.AddSeconds((double) -_random.Next(0,157680000));
        }

        public DateTime GetFutureDateTimeWithin5YearsOfToday()
        {
            //5 years in seconds is 157680000 secs assuming 365 days
            return DateTime.Today.AddSeconds((double) _random.Next(0, 157680000));
        }

        public DateTime? GetDateTimeInTheFutureButSameDay(DateTime? StartTime)
        {
            return StartTime.Value.AddMinutes((double)_random.Next(0, 240));
        }

        #endregion

        #region Public Field Accessors

        public IEnumerable<InsertionStatistic> LastPatientInsertionTimes
        {
            get { return _lastPatientInsertionTimes; }
        }
        public IEnumerable<InsertionStatistic> LastVisitInsertionTimes
        {
            get { return _lastVisitInsertionTimes; }
        }
        public IEnumerable<InsertionStatistic> LastOrderInsertionTimes
        {
            get { return _lastOrderInsertionTimes; }
        }

        #endregion
    }
}
