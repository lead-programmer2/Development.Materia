using System;
using System.Data;
using Development.Materia.Database;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Development.Materia.UnitTest
{
    [TestClass]
    public class QueryGeneratorTest
    {

        private DataTable SampleTable()
        {
            DataTable _table = new DataTable();
            _table.TableName = "users";

            DataColumn _pkCol = _table.Columns.Add("UserId", typeof(int));
            _table.Columns.Add("Username", typeof(string));
            _table.Columns.Add("Active", typeof(byte));
            _table.Columns.Add("HeaderId", typeof(int));
            _table.Columns.Add("Image", typeof(byte[]));
            _table.Columns.Add("DateCreated", typeof(DateTime));
            _table.Columns.Add("LastModified", typeof(DateTime));
            _table.Constraints.Add("users_PK", _pkCol, true);
            _pkCol.AutoIncrement = true; _pkCol.AutoIncrementSeed = 1;
            _pkCol.AutoIncrementStep = 1;

            return _table;
        }

        /// <summary>
        ///A test for GetSqlStatements
        ///</summary>
        [TestMethod()]
        public void GetSqlStatementsTest()
        {
            DataTable _table = SampleTable();
            _table.Rows.Add(new object[] { null, "jsph", 1, null, null, DateTime.Now, DateTime.Now });

            QueryGenerator _generator = new QueryGenerator(_table);
            _generator.ExcludedFields.Add("LastModified");
            _generator.ForeignKey.Field = "HeaderId";
            _generator.ForeignKey.Value = Database.Database.LastInsertIdCall;
            string[] _queries = _generator.GetSqlStatements();

            if (_queries.Length <= 0) Assert.Fail();
            else
            {
                for (int i = 0; i < _queries.Length; i++) Console.WriteLine(_queries[i]);
            }

            _table.AcceptChanges(); _generator = null;

            DataRow _row = _table.Rows[0];
            _row["Active"] = 0;
            _generator = new QueryGenerator(_table);
            _generator.ExcludedFields.Add("LastModified");
            _queries = null; _queries = _generator.GetSqlStatements();

            if (_queries.Length <= 0) Assert.Fail();
            else Console.WriteLine(_queries[0]);

            _table.AcceptChanges(); _generator = null;
            _row.Delete();

            _generator = new QueryGenerator(_table);
            _generator.ExcludedFields.Add("LastModified");
            _queries = null; _queries = _generator.GetSqlStatements();

            if (_queries.Length <= 0) Assert.Fail();
            else Console.WriteLine(_queries[0]);

            _table.Dispose(); _table = null;
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod()]
        public void ToStringTest()
        {
            DataTable _table = SampleTable();
            _table.Rows.Add(new object[] { null, "jsph", 1, null, null, DateTime.Now, DateTime.Now });
            _table.Rows.Add(new object[] { null, "dqadmin", 1, null, null, DateTime.Now, DateTime.Now });
            _table.AcceptChanges();

            _table.Rows.Add(new object[] { null, "admin", 1, null, null, DateTime.Now, DateTime.Now });

            DataRow[] _jsphRows = _table.Select("[Username] = 'jsph'");
            if (_jsphRows.Length > 0)
            {
                _jsphRows[0]["Active"] = 0;
                _jsphRows[0]["Username"] = "seph";
            }

            DataRow[] _dqRows = _table.Select("[Username] = 'dqadmin'");
            if (_dqRows.Length > 0) _dqRows[0].Delete();

            QueryGenerator _generator = new QueryGenerator(_table);
            _generator.ExcludedFields.Add("LastModified");
            string _sql = _generator.ToString();
            if (_sql.Trim() == "") Assert.Fail();
            else Console.WriteLine(_sql);

            _table.AcceptChanges(); _table.Dispose(); _table = null;
        }
    }
}
