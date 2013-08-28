using Development.Materia.Database;
using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Development.Materia.UnitTest
{
    [TestClass]
    public class DatabaseTest
    {
        [TestMethod]
        public void TestCreateConnection()
        {
            string _connectionstring = "SERVER=localhost;DATABASE=fmshras;UID=root;PWD=fms2011;CONNECT TIMEOUT=120;";
            IDbConnection _connection = Development.Materia.Database.Database.CreateConnection(_connectionstring);

            if (_connection == null) Assert.Fail("No database connection created! Fuck!");
            else
            {
                try
                {
                    Console.WriteLine("Opening connection with settings : " + _connection.ConnectionString + ".");
                    if (_connection.State != ConnectionState.Open) _connection.Open();
                    Console.WriteLine("Connection Timeout : " + _connection.ConnectionTimeout.ToString());
                    Console.WriteLine("Connection successful.");
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex.Message);
                    Assert.Fail("Failed to connection"); 
                }

                if (_connection.State == ConnectionState.Open) _connection.Close();
                _connection.Dispose(); _connection = null;
            }
        }
    }
}
