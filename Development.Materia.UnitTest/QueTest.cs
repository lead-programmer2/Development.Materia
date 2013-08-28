using System;
using System.Data;
using Development.Materia.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Development.Materia.UnitTest
{
    [TestClass]
    public class QueTest
    {

        [TestMethod]
        public void CommandTimeoutTest()
        {
            string _connectionstring = "SERVER=192.168.1.124;DATABASE=fmshras;UID=root;PWD=fms2011;";
            IDbConnection _connection = Database.Database.CreateConnection(_connectionstring);
            Database.Database.CommandTimeout = 1;
            string _sql = "SELECT * FROM `logs`;\n" +
                          "SELECT * FROM `employees`";
            QueResult _result = Que.Execute(_connection, _sql, CommandExecution.ExecuteNonQuery);
            if (_result.RowsAffected < 0)
            {
                Console.WriteLine(_result.Error);
                Assert.Fail("Fail to execute statements");
            }
            else
            {
                Console.WriteLine("Tables : " + _result.ResultSet.Tables.Count);
                Console.WriteLine("Records : " + _result.RowsAffected);
            }

            _result.Dispose(QueResultDisposition.WithAssociatedQue);
        }

        [TestMethod]
        public void ExecuteTest()
        {
            string _sql = @"INSERT INTO `training_evaluation`
                            (`TrainingId`, `ReferenceNo`, `RefStartDate`, `RefEndDate`, `EmployeeId`, `OverallRating`, `Preparedby`, `Comments`, `DateCreated`)
                            VALUES
                            (19, '', '2013-08-19 16:45:51', '2013-08-19 16:45:51', 10, 0, 'John Paulo Navarro', '', '2013-08-22 14:12:30');
                            SELECT CAST(LAST_INSERT_ID() AS signed) AS `HeaderId`;
                            INSERT INTO `training_evaluation_details`
                            (`EvaluationId`, `ExamItems1`, `ExamItems2`, `ExamItems3`, `ExamScore1`, `ExamScore2`, `ExamScore3`, `ExamGrade1`, `ExamGrade2`, `ExamGrade3`)
                            VALUES
                            (" + Database.Database.LastInsertIdCall + ", 0, 0, 0, 0, 0, 0, 0, 0, 0);" +
                            "\nSELECT " + Database.Database.LastInsertIdCall + " AS `Id`;";

            string _connectionstring = "SERVER=localhost;DATABASE=fmshras;UID=root;PWD=fms2011;";
            IDbConnection _connection = Database.Database.CreateConnection(_connectionstring);
            QueResult _result = Que.Execute(_connection, _sql, CommandExecution.ExecuteNonQuery);

            if (_result.Error.Trim() != "") Assert.Fail(_result.Error);
            else
            {
                Console.WriteLine(_sql);
                Console.WriteLine("Executed queries : " + _result.RowsAffected);
                if (_result.ResultSet != null)
                {
                    if (_result.ResultSet.Tables.Count >= 2)
                    {
                        if (_result.ResultSet.Tables[0].Rows.Count > 0) Console.WriteLine("Header Id : " + _result.ResultSet.Tables[0].Rows[0]["HeaderId"].ToString());
                        if (_result.ResultSet.Tables[1].Rows.Count > 0) Console.WriteLine("@lastid : " + _result.ResultSet.Tables[1].Rows[0]["Id"].ToString());
                    }
                }
            }

            _result.Dispose();
        }


    }
}
