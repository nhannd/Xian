#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Tests
{
//    [TestFixture]
//    public class VoiLutPresetTests
//    {
//        public List<VoiLutPresetConfigurationKey> _configurations;

//        public VoiLutPresetTests()
//        { 
		
//        }

//        [TestFixtureSetUp]
//        public void Initialize()
//        {
//            _configurations = new List<VoiLutPresetConfigurationKey>();

//            _configurations.Add(new VoiLutPresetConfigurationKey("CT", XKeys.F2, "Chest"));
//            _configurations.Add(new VoiLutPresetConfigurationKey("CT", XKeys.F3, "Bone"));
//            _configurations.Add(new VoiLutPresetConfigurationKey("CT", XKeys.F4, "Chest/Abdomen/Pelvis"));
//            _configurations.Add(new VoiLutPresetConfigurationKey("CT", XKeys.F5, "Lung"));
//            _configurations.Add(new VoiLutPresetConfigurationKey("CT", XKeys.F6, "Abdomen"));
//            _configurations.Add(new VoiLutPresetConfigurationKey("CT", XKeys.None, "Curvy"));
//            _configurations.Add(new VoiLutPresetConfigurationKey("CT", XKeys.None, "Straight"));
//            _configurations.Add(new VoiLutPresetConfigurationKey("CT", XKeys.None, "Vertical"));

//            _configurations.Add(new VoiLutPresetConfigurationKey("MR", XKeys.F2, "Chest"));
//            _configurations.Add(new VoiLutPresetConfigurationKey("MR", XKeys.F3, "Bone"));
//            _configurations.Add(new VoiLutPresetConfigurationKey("MR", XKeys.F4, "Chest/Abdomen/Pelvis"));
//            _configurations.Add(new VoiLutPresetConfigurationKey("MR", XKeys.F5, "Lung"));
//            _configurations.Add(new VoiLutPresetConfigurationKey("MR", XKeys.F6, "Abdomen"));
//            _configurations.Add(new VoiLutPresetConfigurationKey("MR", XKeys.None, "Curvy"));
//            _configurations.Add(new VoiLutPresetConfigurationKey("MR", XKeys.None, "Straight"));
//            _configurations.Add(new VoiLutPresetConfigurationKey("MR", XKeys.None, "Vertical"));

//            _configurations.Add(new VoiLutPresetConfigurationKey("", XKeys.F2, "Histogram"));
//            _configurations.Add(new VoiLutPresetConfigurationKey("", XKeys.F3, "Default"));
//            _configurations.Add(new VoiLutPresetConfigurationKey("", XKeys.F4, "Magic"));
//            _configurations.Add(new VoiLutPresetConfigurationKey("", XKeys.None, "Curvy"));
//            _configurations.Add(new VoiLutPresetConfigurationKey("", XKeys.None, "Straight"));
//            _configurations.Add(new VoiLutPresetConfigurationKey("", XKeys.None, "Vertical"));
//        }

//        private VoiLutPresetConfigurationCollection BuildCollection()
//        {
//            VoiLutPresetConfigurationCollection collection = new VoiLutPresetConfigurationCollection();

//            foreach (VoiLutPresetConfigurationKey key in _configurations)
//            {
//                collection.UnsafeUpdate(new VoiLutPresetConfiguration(key.ModalityFilter, key.KeyStroke, key.Name,
//                    new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>())));
//            }

//            foreach (VoiLutPresetConfiguration confignration in collection)
//                Console.WriteLine("{0}, {1}, {2}", confignration.ModalityFilter, confignration.KeyStroke, confignration.Name);

//            Assert.AreEqual(collection.Count, _configurations.Count);

//            return collection;
//        }

//        [Test]
//        public void TestSortedList()
//        {
//            VoiLutPresetConfigurationCollection collection = new VoiLutPresetConfigurationCollection();

//            List<VoiLutPresetConfigurationKey> configurationsCopy = new List<VoiLutPresetConfigurationKey>(_configurations);

//            Random rand = new Random();

//            while(configurationsCopy.Count > 0)
//            {
//                int next = rand.Next(0, configurationsCopy.Count - 1);
//                VoiLutPresetConfigurationKey key = configurationsCopy[next];

//                collection.UnsafeUpdate(new VoiLutPresetConfiguration(key.ModalityFilter, key.KeyStroke, key.Name, 
//                    new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>())));
//                configurationsCopy.RemoveAt(next);
//            }

//            foreach (VoiLutPresetConfiguration confignration in collection)
//                Console.WriteLine("{0}, {1}, {2}", confignration.ModalityFilter, confignration.KeyStroke, confignration.Name);

//            int i = 0;
//            foreach (VoiLutPresetConfiguration confignration in collection)
//            {
//                Assert.AreEqual(confignration.ModalityFilter, _configurations[i].ModalityFilter);
//                Assert.AreEqual(confignration.KeyStroke, _configurations[i].KeyStroke);			
//                Assert.AreEqual(confignration.Name, _configurations[i].Name);			
//                ++i;
//            }

//            Assert.AreEqual(_configurations.Count, i);
//        }

//        [Test]
//        public void TestKeyEquality()
//        {
//            VoiLutPresetConfigurationCollection collection = BuildCollection();

//            //non-matching name, matching keystroke.
//            VoiLutPresetConfiguration[] configurations = collection.FindMatchingConfigurations(new VoiLutPresetConfigurationKey("MR", XKeys.F4, "junk"));
//            Assert.AreEqual(configurations.Length, 1);
//            Assert.AreEqual(configurations[0].ModalityFilter, "MR");
//            Assert.AreEqual(configurations[0].KeyStroke, XKeys.F4);
//            Assert.AreEqual(configurations[0].Name, "Chest/Abdomen/Pelvis");

//            //matching name, non-matching keystroke.
//            configurations = collection.FindMatchingConfigurations(new VoiLutPresetConfigurationKey("CT", XKeys.None, "Lung"));
//            Assert.AreEqual(configurations.Length, 1); 
//            Assert.AreEqual(configurations[0].ModalityFilter, "CT"); 
//            Assert.AreEqual(configurations[0].KeyStroke, XKeys.F5);
//            Assert.AreEqual(configurations[0].Name, "Lung");

//            //matching name, non-matching keystroke, actual keystroke = none.
//            configurations  = collection.FindMatchingConfigurations(new VoiLutPresetConfigurationKey("", XKeys.F6, "Straight"));
//            Assert.AreEqual(configurations.Length, 1); 
//            Assert.AreEqual(configurations[0].ModalityFilter, "");
//            Assert.AreEqual(configurations[0].KeyStroke, XKeys.None);
//            Assert.AreEqual(configurations[0].Name, "Straight");

//            //perfect match.
//            configurations = collection.FindMatchingConfigurations(new VoiLutPresetConfigurationKey("MR", XKeys.F3, "Bone"));
//            Assert.AreEqual(configurations.Length, 1); 
//            Assert.AreEqual(configurations[0].ModalityFilter, "MR");
//            Assert.AreEqual(configurations[0].KeyStroke, XKeys.F3);
//            Assert.AreEqual(configurations[0].Name, "Bone");

//            //perfect match.
//            configurations = collection.FindMatchingConfigurations(new VoiLutPresetConfigurationKey("CT", XKeys.None, "Straight"));
//            Assert.AreEqual(configurations.Length, 1); 
//            Assert.AreEqual(configurations[0].ModalityFilter, "CT");
//            Assert.AreEqual(configurations[0].KeyStroke, XKeys.None);
//            Assert.AreEqual(configurations[0].Name, "Straight");

//            //non-matching modality.
//            configurations = collection.FindMatchingConfigurations(new VoiLutPresetConfigurationKey("DX", XKeys.F6, "Straight"));
//            Assert.AreEqual(configurations.Length, 0);

//            //non-matching keystroke and name.
//            configurations = collection.FindMatchingConfigurations(new VoiLutPresetConfigurationKey("MR", XKeys.F12, "blah"));
//            Assert.AreEqual(configurations.Length, 0);

//            //conflicting key.
//            configurations = collection.FindMatchingConfigurations(new VoiLutPresetConfigurationKey("CT", XKeys.F6, "Lung"));
//            Assert.AreEqual(configurations.Length, 2);
//        }

//        [Test]
//        public void TestUnsafeUpdate()
//        {
//            VoiLutPresetConfigurationCollection collection = BuildCollection();

//            collection.UnsafeUpdate(new VoiLutPresetConfiguration("CT", XKeys.F6, "Lung",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>())));

//            //2 items got replaced due to conflicts with name and keystroke.
//            Assert.AreEqual(collection.Count, _configurations.Count - 1);

//            collection.UnsafeUpdate(new VoiLutPresetConfiguration("CT", XKeys.F6, "Histogram",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>())));

//            //the same item got replaced because it has the same keystroke, but a new unique name.
//            Assert.AreEqual(collection.Count, _configurations.Count - 1);

//            collection.UnsafeUpdate(new VoiLutPresetConfiguration("CT", XKeys.F12, "Histogram",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>())));

//            //the same item got replaced because it has the same name but a unique keystroke.
//            Assert.AreEqual(collection.Count, _configurations.Count - 1);

//            collection.UnsafeUpdate(new VoiLutPresetConfiguration("CT", XKeys.F11, "Horizontal",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>())));

//            //a new item got added because it has a unique keystroke and name.
//            Assert.AreEqual(collection.Count, _configurations.Count);

//            collection.UnsafeUpdate(new VoiLutPresetConfiguration("CT", XKeys.None, "Horizontal",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>())));

//            //the same item got replaced because it has the same name.
//            Assert.AreEqual(collection.Count, _configurations.Count);

//            collection.UnsafeUpdate(new VoiLutPresetConfiguration("CT", XKeys.None, "Liver",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>())));

//            //a new item got added because its name is unique.
//            Assert.AreEqual(collection.Count, _configurations.Count + 1);

//            collection.UnsafeUpdate(new VoiLutPresetConfiguration("CT", XKeys.F5, "Liver",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>())));

//            //the same item got replaced because it has the same name.
//            Assert.AreEqual(collection.Count, _configurations.Count + 1);

//            collection.UnsafeUpdate(new VoiLutPresetConfiguration("CT", XKeys.F5, "Kidney",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>())));

//            //the same item got replaced because it has the same keystroke.
//            Assert.AreEqual(collection.Count, _configurations.Count + 1);

//            Console.WriteLine();
//            foreach (VoiLutPresetConfiguration confignration in collection)
//                Console.WriteLine("{0}, {1}, {2}", confignration.ModalityFilter, confignration.KeyStroke, confignration.Name);
//        }

//        [Test]
//        public void TestAddNoConflict()
//        {
//            VoiLutPresetConfigurationCollection collection = BuildCollection();

//            VoiLutPresetConfiguration addConfiguration = new VoiLutPresetConfiguration("MR", XKeys.F8, "Histogram",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            collection.Add(addConfiguration);
//        }

//        [Test]
//        [ExpectedException(typeof(InvalidOperationException))]
//        public void TestAddSameItem()
//        {
//            VoiLutPresetConfigurationCollection collection = BuildCollection();

//            VoiLutPresetConfiguration addConfiguration = new VoiLutPresetConfiguration("MR", XKeys.F5, "Lung",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            collection.Add(addConfiguration);
//        }

//        [Test]
//        [ExpectedException(typeof(InvalidOperationException))]
//        public void TestAddNameConflict()
//        {
//            VoiLutPresetConfigurationCollection collection = BuildCollection();

//            VoiLutPresetConfiguration addConfiguration = new VoiLutPresetConfiguration("MR", XKeys.F8, "Abdomen",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            collection.Add(addConfiguration);
//        }

//        [Test]
//        [ExpectedException(typeof(InvalidOperationException))]
//        public void TestAddNameConflictNoKeyStroke()
//        {
//            VoiLutPresetConfigurationCollection collection = BuildCollection();

//            VoiLutPresetConfiguration addConfiguration = new VoiLutPresetConfiguration("MR", XKeys.None, "Straight",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            collection.Add(addConfiguration);
//        }

//        [Test]
//        [ExpectedException(typeof(InvalidOperationException))]
//        public void TestAddKeyStrokeConflict()
//        {
//            VoiLutPresetConfigurationCollection collection = BuildCollection();

//            VoiLutPresetConfiguration addConfiguration = new VoiLutPresetConfiguration("MR", XKeys.F2, "Histogram",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            collection.Add(addConfiguration);
//        }

//        [Test]
//        [ExpectedException(typeof(InvalidOperationException))]
//        public void TestAddNameAndKeyStrokeConflict()
//        {
//            VoiLutPresetConfigurationCollection collection = BuildCollection();

//            VoiLutPresetConfiguration addConfiguration = new VoiLutPresetConfiguration("MR", XKeys.F2, "Bone",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            collection.Add(addConfiguration);
//        }

//        [Test]
//        [ExpectedException(typeof(ArgumentNullException))]
//        public void TestReplaceItemNull()
//        {
//            VoiLutPresetConfigurationCollection collection = BuildCollection();

//            VoiLutPresetConfiguration existingConfiguration = new VoiLutPresetConfiguration("CT", XKeys.F5, "Lung",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            collection.Replace(existingConfiguration, null);
//        }

//        [Test]
//        [ExpectedException(typeof(ArgumentException))]
//        public void TestReplaceItemDoesNotExist()
//        {
//            VoiLutPresetConfigurationCollection collection = BuildCollection();

//            VoiLutPresetConfiguration existingConfiguration = new VoiLutPresetConfiguration("CT", XKeys.F5, "Histogram",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            VoiLutPresetConfiguration newConfiguration = new VoiLutPresetConfiguration("CT", XKeys.F5, "Lung",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            collection.Replace(existingConfiguration, newConfiguration);
//        }

//        [Test]
//        public void TestReplaceSuccessfulNewName()
//        {
//            VoiLutPresetConfigurationCollection collection = BuildCollection();

//            VoiLutPresetConfiguration existingConfiguration = new VoiLutPresetConfiguration("CT", XKeys.F5, "Lung",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            VoiLutPresetConfiguration newConfiguration = new VoiLutPresetConfiguration("CT", XKeys.F5, "Histogram",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            collection.Replace(existingConfiguration, newConfiguration);
//        }

//        [Test]
//        public void TestReplaceSuccessfulNoChange()
//        {
//            VoiLutPresetConfigurationCollection collection = BuildCollection();

//            VoiLutPresetConfiguration existingConfiguration = new VoiLutPresetConfiguration("CT", XKeys.F5, "Lung",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            VoiLutPresetConfiguration newConfiguration = new VoiLutPresetConfiguration("CT", XKeys.F5, "Lung",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            collection.Replace(existingConfiguration, newConfiguration);
//        }

//        [Test]
//        public void TestReplaceSuccessfulNewKeyStroke()
//        {
//            VoiLutPresetConfigurationCollection collection = BuildCollection();

//            VoiLutPresetConfiguration existingConfiguration = new VoiLutPresetConfiguration("CT", XKeys.F5, "Lung",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            VoiLutPresetConfiguration newConfiguration = new VoiLutPresetConfiguration("CT", XKeys.F12, "Lung",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            collection.Replace(existingConfiguration, newConfiguration);
//        }

//        [Test]
//        public void TestReplaceSuccessfulNewKeyStrokeNewName()
//        {
//            VoiLutPresetConfigurationCollection collection = BuildCollection();

//            VoiLutPresetConfiguration existingConfiguration = new VoiLutPresetConfiguration("CT", XKeys.F5, "Lung",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            VoiLutPresetConfiguration newConfiguration = new VoiLutPresetConfiguration("CT", XKeys.F12, "Histogram",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            collection.Replace(existingConfiguration, newConfiguration);
//        }

//        [Test]
//        public void TestReplaceSuccessfulNoKeyStroke()
//        {
//            VoiLutPresetConfigurationCollection collection = BuildCollection();

//            VoiLutPresetConfiguration existingConfiguration = new VoiLutPresetConfiguration("CT", XKeys.F5, "Lung",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            VoiLutPresetConfiguration newConfiguration = new VoiLutPresetConfiguration("CT", XKeys.None, "Lung",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            collection.Replace(existingConfiguration, newConfiguration);
//        }

//        [Test]
//        public void TestReplaceSuccessfulNoKeyStrokeNewName()
//        {
//            VoiLutPresetConfigurationCollection collection = BuildCollection();

//            VoiLutPresetConfiguration existingConfiguration = new VoiLutPresetConfiguration("CT", XKeys.F5, "Lung",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            VoiLutPresetConfiguration newConfiguration = new VoiLutPresetConfiguration("CT", XKeys.None, "Histogram",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            collection.Replace(existingConfiguration, newConfiguration);
//        }

//        [Test]
//        [ExpectedException(typeof(InvalidOperationException))]
//        public void TestReplaceFailedNameConflict()
//        {
//            VoiLutPresetConfigurationCollection collection = BuildCollection();

//            VoiLutPresetConfiguration existingConfiguration = new VoiLutPresetConfiguration("CT", XKeys.F5, "Lung",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            VoiLutPresetConfiguration newConfiguration = new VoiLutPresetConfiguration("CT", XKeys.F5, "Abdomen",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            collection.Replace(existingConfiguration, newConfiguration);
//        }

//        [Test]
//        [ExpectedException(typeof(InvalidOperationException))]
//        public void TestReplaceFailedKeyStrokeConflict()
//        {
//            VoiLutPresetConfigurationCollection collection = BuildCollection();

//            VoiLutPresetConfiguration existingConfiguration = new VoiLutPresetConfiguration("CT", XKeys.F5, "Lung",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            VoiLutPresetConfiguration newConfiguration = new VoiLutPresetConfiguration("CT", XKeys.F6, "Lung",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            collection.Replace(existingConfiguration, newConfiguration);
//        }

//        [Test]
//        [ExpectedException(typeof(InvalidOperationException))]
//        public void TestReplaceFailedDualConflict()
//        {
//            VoiLutPresetConfigurationCollection collection = BuildCollection();

//            VoiLutPresetConfiguration existingConfiguration = new VoiLutPresetConfiguration("CT", XKeys.F5, "Lung",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            VoiLutPresetConfiguration newConfiguration = new VoiLutPresetConfiguration("CT", XKeys.F6, "Straight",
//                new VoiLutPresetApplicatorConfiguration("test", new Dictionary<string, string>()));

//            collection.Replace(existingConfiguration, newConfiguration);
//        }
//    }
}


#endif