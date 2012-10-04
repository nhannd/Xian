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
using NHibernate;
using NHibernate.Cfg;

namespace hbm2ddl
{
    class Program
    {
	    static void Main(string[] args)
        {
            Configuration cfg = new Configuration();
            cfg.Configure(args[0] + ".cfg.xml");
            //cfg.SetProperty("hibernate.dialect", "NHibernate.Dialect.SQLiteDialect");
            //cfg.SetProperty("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
            //cfg.SetProperty("hibernate.connection.driver_class", "NHibernate.Driver.SQLiteDriver");
            //cfg.SetProperty("hibernate.connection.connection_string", "Data Source=nhibernate.db;Version=3");
            //cfg.SetProperty("hibernate.query.substitutions", "true=1;false=0");
            if (args.Length > 0)
                cfg.AddAssembly(args[0]);
            else
                return;

            if (args.Length > 1)
                cfg.AddAssembly(args[1]);

            String[] strDDL = cfg.GenerateSchemaCreationScript(NHibernate.Dialect.SQLiteDialect.GetDialect(cfg.Properties));
            StreamWriter SW;
            SW = File.CreateText("CreateTables." + args[0] + ".ddl");

            foreach (String s in strDDL)
            {
                SW.Write(s);
                SW.WriteLine(";");
            }

            SW.Close();

            strDDL = cfg.GenerateDropSchemaScript(NHibernate.Dialect.SQLiteDialect.GetDialect(cfg.Properties));
            SW = File.CreateText("DropTables." + args[0] + ".ddl");

            foreach (String s in strDDL)
            {
                SW.Write(s);
                SW.WriteLine(";");
            }

            SW.Close();
        }
    }
}
