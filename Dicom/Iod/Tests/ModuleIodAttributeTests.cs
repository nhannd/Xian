#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if	UNIT_TESTS

using NUnit.Framework;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.Dicom.Tests;

namespace ClearCanvas.Dicom.Iod.Tests
{
	[TestFixture]
	public class ModuleIodAttributeTests : AbstractTest
	{
		/// <summary>
		/// Test saving/loading values from the <see cref="PatientModuleIod"/>
		/// </summary>
		/// <remarks>
		/// Only selective attributes are tested.
		/// </remarks>
		[Test]
		public void TestPatientModuleIod()
		{
			var dicomFile = new DicomFile();

			var module = new PatientModuleIod(dicomFile.DataSet);
			module.ResponsibleOrganization = "Walt Disney";
			module.ResponsiblePerson = "Roger Radcliffe";
			module.ResponsiblePersonRole = ResponsiblePersonRole.Owner;
			module.PatientSpeciesDescription = "Canine species";
			module.PatientSpeciesCodeSequence = SpeciesContextGroup.CanineSpecies;
			module.PatientBreedDescription = "Dalmatian dog";
			module.PatientBreedCodeSequence = new[] {BreedContextGroup.DalmatianDog};
			module.BreedRegistrationSequence = new[]
				{
					new BreedRegistrationSequence
						{
							BreedRegistrationNumber = "101",
							BreedRegistryCodeSequence = new BreedRegistry(
								"WD",
								"101",
								"WALT_DISNESY_101",
								"One hundred and one dalmatians")
						}
				};

			Assert.AreEqual(module.ResponsibleOrganization, "Walt Disney");
			Assert.AreEqual(module.ResponsiblePerson, "Roger Radcliffe");
			Assert.AreEqual(module.ResponsiblePersonRole, ResponsiblePersonRole.Owner);
			Assert.AreEqual(module.PatientSpeciesDescription, "Canine species");
			Assert.AreEqual(module.PatientSpeciesCodeSequence, SpeciesContextGroup.CanineSpecies);
			Assert.AreEqual(module.PatientBreedDescription, "Dalmatian dog");
			Assert.AreEqual(module.PatientBreedCodeSequence.Length, 1);
			Assert.AreEqual(module.PatientBreedCodeSequence[0], BreedContextGroup.DalmatianDog);
			Assert.AreEqual(module.BreedRegistrationSequence.Length, 1);
			Assert.AreEqual(module.BreedRegistrationSequence[0].BreedRegistrationNumber, "101");
			Assert.AreEqual(module.BreedRegistrationSequence[0].BreedRegistryCodeSequence.CodingSchemeDesignator, "WD");
			Assert.AreEqual(module.BreedRegistrationSequence[0].BreedRegistryCodeSequence.CodingSchemeVersion, "101");
			Assert.AreEqual(module.BreedRegistrationSequence[0].BreedRegistryCodeSequence.CodeValue, "WALT_DISNESY_101");
			Assert.AreEqual(module.BreedRegistrationSequence[0].BreedRegistryCodeSequence.CodeMeaning, "One hundred and one dalmatians");

			dicomFile.Save("TestPatientModuleIod.dcm");

			var reloadedDicomFile = new DicomFile("TestPatientModuleIod.dcm");
			reloadedDicomFile.Load();

			var realoadedModule = new PatientModuleIod(reloadedDicomFile.DataSet);
			Assert.AreEqual(realoadedModule.ResponsibleOrganization, "Walt Disney");
			Assert.AreEqual(realoadedModule.ResponsiblePerson, "Roger Radcliffe");
			Assert.AreEqual(realoadedModule.ResponsiblePersonRole, ResponsiblePersonRole.Owner);
			Assert.AreEqual(realoadedModule.PatientSpeciesDescription, "Canine species");
			Assert.AreEqual(realoadedModule.PatientSpeciesCodeSequence, SpeciesContextGroup.CanineSpecies);
			Assert.AreEqual(realoadedModule.PatientBreedDescription, "Dalmatian dog");
			Assert.AreEqual(realoadedModule.PatientBreedCodeSequence.Length, 1);
			Assert.AreEqual(realoadedModule.PatientBreedCodeSequence[0], BreedContextGroup.DalmatianDog);
			Assert.AreEqual(realoadedModule.BreedRegistrationSequence.Length, 1);
			Assert.AreEqual(realoadedModule.BreedRegistrationSequence[0].BreedRegistrationNumber, "101");
			Assert.AreEqual(realoadedModule.BreedRegistrationSequence[0].BreedRegistryCodeSequence.CodingSchemeDesignator, "WD");
			Assert.AreEqual(realoadedModule.BreedRegistrationSequence[0].BreedRegistryCodeSequence.CodingSchemeVersion, "101");
			Assert.AreEqual(realoadedModule.BreedRegistrationSequence[0].BreedRegistryCodeSequence.CodeValue, "WALT_DISNESY_101");
			Assert.AreEqual(realoadedModule.BreedRegistrationSequence[0].BreedRegistryCodeSequence.CodeMeaning, "One hundred and one dalmatians");
		}

		/// <summary>
		/// Test saving/loading values from the <see cref="GeneralSeriesModuleIod"/>
		/// </summary>
		/// <remarks>
		/// Only selective attributes are tested.
		/// </remarks>
		[Test]
		public void TestGeneralSeriesModuleIod()
		{
			var dicomFile = new DicomFile();

			var module = new GeneralSeriesModuleIod(dicomFile.DataSet);
			Assert.AreEqual(module.AnatomicalOrientationType, AnatomicalOrientationType.None);

			module.AnatomicalOrientationType = AnatomicalOrientationType.Quadruped;
			Assert.AreEqual(module.AnatomicalOrientationType, AnatomicalOrientationType.Quadruped);

			dicomFile.Save("TestGeneralSeriesModuleIod.dcm");

			var reloadedDicomFile = new DicomFile("TestGeneralSeriesModuleIod.dcm");
			reloadedDicomFile.Load();

			var realoadedModule = new PatientModuleIod(reloadedDicomFile.DataSet);
			Assert.AreEqual(module.AnatomicalOrientationType, AnatomicalOrientationType.Quadruped);
		}
	}
}

#endif