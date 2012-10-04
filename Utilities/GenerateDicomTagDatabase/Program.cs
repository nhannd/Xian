#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace GenerateDicomTagDatabase
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Data.SqlClient;
    using System.Data;
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            using (SqlConnection connection = new SqlConnection("Data Source=localhost\\SQLEXPRESS;Initial Catalog=ripp_version5;User ID=sa;Password=root"))
            {
                connection.Open();

                try
                {
                    using (StreamWriter writer = new StreamWriter(@"CreateDicomTags.sql"))
                    {
                        SqlCommand command = new SqlCommand("SELECT TagName, Path, IsComputed FROM DicomTag", connection);
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            StringBuilder insertStatement = new StringBuilder();
                            insertStatement.AppendFormat("INSERT INTO DicomTag (TagName, Path, IsComputed)\r\n" +
                                "VALUES('{0}', '{1}', {2})", reader.GetString(0), reader.GetString(1), Convert.ToInt16(reader.GetBoolean(2)));

                            writer.WriteLine(insertStatement.ToString());
                        }
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
