using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

using DataAccessApplicationBlock;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataAccessApplicationBlockTests
{
   [TestClass]
   [ExcludeFromCodeCoverage]
   public class SqlParametersTests
   {
      private SqlParameters _classUnderTest;

      [TestMethod]
      public void Add_AddToCollection_Success()
      {
         Assert.AreEqual(0, _classUnderTest.Count, "Unexpected number of SqlParameters returned");

         _classUnderTest.Add("@param1", SqlDbType.VarChar, 50, false);
         Assert.AreEqual(1, _classUnderTest.Count, "Unexpected number of SqlParameters returned");

         _classUnderTest.Add("@param2", SqlDbType.Int, false, ParameterDirection.Output);
         Assert.AreEqual(2, _classUnderTest.Count, "Unexpected number of SqlParameters returned");

         _classUnderTest.AddWithValue("@param3", SqlDbType.Int, 1, false);
         Assert.AreEqual(3, _classUnderTest.Count, "Unexpected number of SqlParameters returned");

         _classUnderTest.AddWithValue("@param4", SqlDbType.VarChar, 50, "hello", false);
         Assert.AreEqual(4, _classUnderTest.Count, "Unexpected number of SqlParameters returned");

         _classUnderTest.AddWithValue("@param5", SqlDbType.VarChar, 50, "hello", false, ParameterDirection.Output);
         Assert.AreEqual(5, _classUnderTest.Count, "Unexpected number of SqlParameters returned");
         
         _classUnderTest.AddWithValue("@param6", SqlDbType.Int, null, true);
         Assert.AreEqual(6, _classUnderTest.Count, "Unexpected number of SqlParameters returned");

         // ReSharper disable RedundantAssignment
         SqlParameter sqlParameter1 = _classUnderTest[0];
         SqlParameter sqlParameter2 = _classUnderTest[1];
         SqlParameter sqlParameter3 = _classUnderTest[2];
         SqlParameter sqlParameter4 = _classUnderTest[3];
         SqlParameter sqlParameter5 = _classUnderTest[4];
         SqlParameter sqlParameter6 = _classUnderTest[5];
         // ReSharper restore RedundantAssignment

         using (SqlCommand sqlCommand = new SqlCommand())
         {
            sqlCommand.Parameters.Add("@param1", SqlDbType.VarChar, 50);
            sqlCommand.Parameters.Add("@param2", SqlDbType.Int);
            sqlCommand.Parameters[1].Direction = ParameterDirection.Output;

            sqlCommand.Parameters.Add("@param3", SqlDbType.Int);
            sqlCommand.Parameters.Add("@param4", SqlDbType.VarChar, 50).Value = "hello";
            sqlCommand.Parameters.Add("@param5", SqlDbType.VarChar, 50).Value = "hello";
            sqlCommand.Parameters[4].Direction = ParameterDirection.Output;
            sqlCommand.Parameters.Add("@param6", SqlDbType.Int).Value = null;

            sqlParameter1 = _classUnderTest["@param1"];
            sqlParameter2 = _classUnderTest["@param2"];
            sqlParameter3 = _classUnderTest["@param3"];
            sqlParameter4 = _classUnderTest["@param4"];
            sqlParameter5 = _classUnderTest["@param5"];
            sqlParameter6 = _classUnderTest["@param6"];

            Assert.IsTrue(HelperFunctions.Helper<SqlParameter>.Compare(sqlCommand.Parameters["@param1"], sqlParameter1), "Unexpected sqlparameter properties");
            Assert.IsTrue(HelperFunctions.Helper<SqlParameter>.Compare(sqlCommand.Parameters["@param2"], sqlParameter2), "Unexpected sqlparameter properties");
            Assert.IsTrue(HelperFunctions.Helper<SqlParameter>.Compare(sqlCommand.Parameters["@param3"], sqlParameter3), "Unexpected sqlparameter properties");
            Assert.IsTrue(HelperFunctions.Helper<SqlParameter>.Compare(sqlCommand.Parameters["@param4"], sqlParameter4), "Unexpected sqlparameter properties");
            Assert.IsTrue(HelperFunctions.Helper<SqlParameter>.Compare(sqlCommand.Parameters["@param5"], sqlParameter5), "Unexpected sqlparameter properties");
            Assert.IsTrue(HelperFunctions.Helper<SqlParameter>.Compare(sqlCommand.Parameters["@param6"], sqlParameter6), "Unexpected sqlparameter properties");
         }

         // Cleanup
         _classUnderTest.Clear();
      }

      [TestCleanup]
      public void Cleanup()
      {
         _classUnderTest.Dispose();
      }

      [TestInitialize]
      public void Initialise()
      {
         _classUnderTest = new SqlParameters();
      }
   }
}
