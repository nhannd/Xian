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
using System.IO;
using System.Xml;

using ClearCanvas.Common;


namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities
{
    public class RandomDictionaryHelper
    {
        public RandomDictionaryHelper()
        {
            _random = new Random();
            _dictionaryFamilyNames = DictionaryReader(Platform.PluginDir + @"\Dictionary.Name.Family.txt");
            _dictionaryFemaleNames = DictionaryReader(Platform.PluginDir + @"\Dictionary.Name.Female.txt");
            _dictionaryMaleNames = DictionaryReader(Platform.PluginDir + @"\Dictionary.Name.Male.txt");

            _dictionaryStreets = DictionaryReader(Platform.PluginDir + @"\Dictionary.Address.Street.txt");
            _dictionaryStreetTypes = DictionaryReader(Platform.PluginDir + @"\Dictionary.Address.StreetType.txt");
        }

        #region Name Dictionary Randomizers

        public string FamilyName
        {
            get { return _dictionaryFamilyNames[_random.Next(0, _dictionaryFamilyNames.Length - 1)]; }
        }

        public string MaleName
        {
            get { return _dictionaryMaleNames[_random.Next(0, _dictionaryMaleNames.Length - 1)]; }
        }

        public string FemaleName
        {
            get { return _dictionaryFemaleNames[_random.Next(0, _dictionaryFemaleNames.Length - 1)]; }
        }

        public string GivenName
        {
            get
            {
                List<string> given = new List<string>(_dictionaryFemaleNames);
                given.AddRange(_dictionaryMaleNames);
                return given[_random.Next(0, given.Count - 1)];
            }
        }

        #endregion

        #region Address Dictionary Randomizers

        public string Street
        {
            get { return _dictionaryStreets[_random.Next(0, _dictionaryStreets.Length - 1)]; }
        }

        public string StreetType
        {
            get { return _dictionaryStreetTypes[_random.Next(0, _dictionaryStreetTypes.Length - 1)]; }
        }

        #endregion

        private string[] DictionaryReader(string path)
        {
            if (File.Exists(path))
            {
                List<string> list = new List<string>();

                StreamReader reader = File.OpenText(path);
                string input = null;
                while ((input = reader.ReadLine()) != null)
                {
                    if (input.Trim() != null || input.Trim() != "")
                    {
                        list.Add(input);
                    }
                }
                reader.Close();
                return list.ToArray();
            }
            else
            {
                Platform.ShowMessageBox("Could not locate this file, " + path);
                return new string[0];
            }
        }

        #region Private Name Fields

        private string[] _dictionaryFamilyNames = null;
        private string[] _dictionaryFemaleNames = null;
        private string[] _dictionaryMaleNames = null;

        #endregion

        #region Private Address Fields

        private string[] _dictionaryStreets = null;
        private string[] _dictionaryStreetTypes = null;

        private Random _random;

        #endregion
    }
}
