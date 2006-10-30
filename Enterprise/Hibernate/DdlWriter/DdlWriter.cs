using System;
using System.IO;
using System.Collections.Generic;

using NHibernate.Cfg;

namespace ClearCanvas.Enterprise.Hibernate.DdlWriter
{
    public delegate void WriterStatusChangedEventHandler(String Description);

    class DdlWriter
    {
        private List<Generator> _generators;

        public DdlWriter()
        {
            _generators = new List<Generator>();
        }

        public void AddGenerator(Generator gen)
        {
            _generators.Add(gen);
        }

        public void Execute(string outputFile)
        {
            PersistentStore store = new PersistentStore();
            store.Initialize();

            StreamWriter sw = File.CreateText(outputFile);

            foreach(Generator gen in _generators)
            {
                string[] scripts = gen.Run(store);
                foreach (string s in scripts)
                {
                    sw.WriteLine(s + ";");
                }
            }

            sw.Close();
        }
    }
}
