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
