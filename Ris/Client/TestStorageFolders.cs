#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client
{
    public class TestStorageFolders
    {
        public class Dog
        {
            public string Name;
            public string Breed;
            public string Color;
            public int Age;

            public Dog(string name, string breed, string color, int age)
            {
                Name = name;
                Breed = breed;
                Color = color;
                Age = age;
            }
        }

        public class Cat
        {
            public string Name;
            public string Breed;
            public string Color;
            public int Age;

            public Cat(string name, string breed, string color, int age)
            {
                Name = name;
                Breed = breed;
                Color = color;
                Age = age;
            }
        }


        public class DogTable : Table<Dog>
        {
            public DogTable()
            {
                this.Columns.Add(new TableColumn<Dog, string>("Name", delegate(Dog d) { return d.Name; }));
                this.Columns.Add(new TableColumn<Dog, string>("Breed", delegate(Dog d) { return d.Breed; }));
                this.Columns.Add(new TableColumn<Dog, string>("Color", delegate(Dog d) { return d.Color; }));
                this.Columns.Add(new TableColumn<Dog, string>("Age", delegate(Dog d) { return d.Age.ToString(); }));
            }
        }

        public class CatTable : Table<Cat>
        {
            public CatTable()
            {
                this.Columns.Add(new TableColumn<Cat, string>("Name", delegate(Cat d) { return d.Name; }));
                this.Columns.Add(new TableColumn<Cat, string>("Breed", delegate(Cat d) { return d.Breed; }));
                this.Columns.Add(new TableColumn<Cat, string>("Color", delegate(Cat d) { return d.Color; }));
                this.Columns.Add(new TableColumn<Cat, string>("Age", delegate(Cat d) { return d.Age.ToString(); }));
            }
        }


        public class DogFolder : StorageFolder<Dog>
        {
            public DogFolder(string folderName)
                :base(folderName, new DogTable())
            {
            }
        }

        public class CatFolder : StorageFolder<Cat>
        {
            public CatFolder(string folderName)
                : base(folderName, new CatTable())
            {
            }
        }

        public class MyDogsFolder : DogFolder
        {
            public MyDogsFolder()
                :base("My Dogs")
            {
                this.Items.Add(new Dog("Big Ben", "Terrier", "Brown", 2));
                this.Items.Add(new Dog("Marvin", "Pug", "White", 6));
                this.Items.Add(new Dog("Beauty", "Poodle", "Black", 3));
            }
        }

        public class LostDogsFolder : DogFolder
        {
            public LostDogsFolder()
                :base("Lost Dogs")
            {

            }
        }

        public class FoundDogsFolder : DogFolder
        {
            public FoundDogsFolder()
                : base("Found Dogs")
            {

            }
        }

        public class CryingCatsFolder : CatFolder
        {
            public CryingCatsFolder()
                :base("Crying Cats")
            {
            }
        }

        public class FriendlyCatsFolder : CatFolder
        {
            public FriendlyCatsFolder()
                :base("Friendly Cats")
            {

            }
        }

        public class StrayCatsFolder : CatFolder
        {
            public StrayCatsFolder()
                :base("Stray Cats")
            {
                this.Items.Add(new Cat("Smokey", "Unknown", "Brown", 4));
                this.Items.Add(new Cat("Blue Eyes", "Unknown", "White", 1));
                this.Items.Add(new Cat("Kat", "Siamese", "Black", 6));
            }
        }

    }
}
