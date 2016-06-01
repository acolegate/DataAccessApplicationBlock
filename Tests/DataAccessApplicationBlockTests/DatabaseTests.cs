using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

using DataAccessApplicationBlock;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataAccessApplicationBlockTests
{
   [TestClass]
   [ExcludeFromCodeCoverage]
   public class DatabaseTests
   {
      private const string ConnectionStringName = "main";

      private Database _classUndertest;

      [TestCleanup]
      public void Cleanup()
      {
         _classUndertest.Dispose();
      }

      [TestMethod]
      [ExpectedException(typeof(ApplicationException))]
      public void Create_CreateClassNoColumns_Exception()
      {
         _classUndertest.Create<int>(new Classes.BadClassNoColumns());
      }

      [TestMethod]
      [ExpectedException(typeof(SqlException))]
      public void Create_CreateClassNoPrimaryKey_Exception()
      {
         _classUndertest.Create<int>(new Classes.BadClassNoAttributes.Customer {
                                                                            CustomerId = 1, 
                                                                            Address = "AAA", 
                                                                            Name = "BBB"
                                                                         });
      }

      //[TestMethod]
      //public void Create_CreateObjectsInDatabaseWithNullableColumns_Success()
      //{
      //   Structs.TestWithNullableColumns testStruct = new Structs.TestWithNullableColumns() {
      //                                                                                         Id = 1,
      //                                                                                         NativeString = null,
      //                                                                                         NullableDateTime = null,
      //                                                                                         NullableInt = null
      //                                                                                      };

      //   testStruct.Id = _classUndertest.Create<int>(testStruct);

      //}


      [TestMethod]
      public void Create_CreateObjectsInDatabase_Success()
      {
         Assert.AreEqual(0, _classUndertest.RetrieveList<Structs.Customer>().Count, "Unexpected number of customers returned");

         Structs.Customer customerStruct1 = Structs.TestCustomer;
         customerStruct1.CustomerId = _classUndertest.Create<int>(customerStruct1);

         Structs.Customer customerStruct2 = Structs.TestCustomer;
         customerStruct2.CustomerId = _classUndertest.Create<int>(customerStruct2);

         // make sure there are 2 in the table
         Assert.AreEqual(2, _classUndertest.RetrieveList<Structs.Customer>().Count, "Unexpected number of customers returned");

         // make sure what we get back is correct
         Assert.AreEqual(customerStruct1, _classUndertest.Retrieve<Structs.Customer>(customerStruct1.CustomerId), "Unexpected customer returned");
         Assert.AreEqual(customerStruct2, _classUndertest.Retrieve<Structs.Customer>(customerStruct2.CustomerId), "Unexpected customer returned");

         // delete
         _classUndertest.Delete<Structs.Customer>(customerStruct1.CustomerId);
         Assert.AreEqual(1, _classUndertest.RetrieveList<Structs.Customer>().Count, "Unexpected number of customers returned");

         _classUndertest.Delete<Structs.Customer>(customerStruct2.CustomerId);
         Assert.AreEqual(0, _classUndertest.RetrieveList<Structs.Customer>().Count, "Unexpected number of customers returned");
      }

      [TestMethod]
      [ExpectedException(typeof(ApplicationException))]
      public void Create_CreateStructNoColumns_Exception()
      {
         _classUndertest.Create<int>(new Structs.BadStructNoColumns());
      }

      [TestMethod]
      [ExpectedException(typeof(SqlException))]
      public void Create_CreateStructNoAttributes_Exception()
      {
         _classUndertest.Create<int>(new Structs.BadStructNoAttributes.Customer {
                                                                             CustomerId = 1,
                                                                             Address = "AAA",
                                                                             Name = "BBB"
                                                                          });
      }

      [TestMethod]
      [ExpectedException(typeof(ApplicationException))]
      public void Delete_DeleteClassNoColumns_Exception()
      {
         _classUndertest.Delete<Classes.BadClassNoColumns>(null);
      }

      [TestMethod]
      [ExpectedException(typeof(ApplicationException))]
      public void Delete_DeleteClassNoPrimaryKey_Exception()
      {
         _classUndertest.Delete<Classes.BadClassNoAttributes.Customer>(null);
      }

      [TestMethod]
      [ExpectedException(typeof(ApplicationException))]
      public void Delete_DeleteCustomerClassNoPrimaryKeyValue_Exception()
      {
         _classUndertest.Delete<Classes.Customer>(null);
      }

      [TestMethod]
      [ExpectedException(typeof(ApplicationException))]
      public void Delete_DeleteCustomerStructNoPrimaryKeyValue_Exception()
      {
         _classUndertest.Delete<Structs.Customer>(null);
      }

      [TestMethod]
      [ExpectedException(typeof(ApplicationException))]
      public void Delete_DeleteStructNoColumns_Exception()
      {
         _classUndertest.Delete<Structs.BadStructNoColumns>(null);
      }

      [TestMethod]
      [ExpectedException(typeof(ApplicationException))]
      public void Delete_DeleteStructNoPrimaryKeye_Exception()
      {
         _classUndertest.Delete<Structs.BadStructNoAttributes.Customer>(null);
      }

      [TestInitialize]
      public void Initialise()
      {
         _classUndertest = new Database(ConnectionStringName);
      }

      [TestMethod]
      public void Retrieve_NoSqlPassed_Success()
      {
         // make sure we get an instance of the object back if the passed sql returns no rows
         Classes.Customer expected = (Classes.Customer)Activator.CreateInstance(typeof(Classes.Customer));
         Classes.Customer actual = _classUndertest.Retrieve<Classes.Customer>("set nocount off;");

         Assert.IsTrue(HelperFunctions.Helper<Classes.Customer>.Compare(expected, actual), "Unexpected customers returned");
      }

      [TestMethod]
      [ExpectedException(typeof(ApplicationException))]
      public void Retrieve_RetrieveClassNoPrimaryKeyValue_Exception()
      {
         _classUndertest.Retrieve<Classes.Customer>(primaryKeyColumnValue: null);
      }

      [TestMethod]
      [ExpectedException(typeof(ApplicationException))]
      public void Retrieve_RetrieveClassNoPrimaryKey_Exception()
      {
         _classUndertest.Retrieve<Classes.BadClassNoAttributes.Customer>(1);
      }

      [TestMethod]
      [ExpectedException(typeof(ApplicationException))]
      public void Retrieve_RetrieveStructNoPrimaryKeyValue_Exception()
      {
         _classUndertest.Retrieve<Structs.Customer>(primaryKeyColumnValue: null);
      }

      [TestMethod]
      [ExpectedException(typeof(ApplicationException))]
      public void Retrieve_RetrieveStructNoPrimaryKey_Exception()
      {
         _classUndertest.Retrieve<Structs.BadStructNoAttributes.Customer>(1);
      }

      [TestMethod]
      [ExpectedException(typeof(ApplicationException))]
      public void Update_ClassNoColumns_Exception()
      {
         _classUndertest.Update(new Classes.BadClassNoColumns());
      }

      [TestMethod]
      [ExpectedException(typeof(ApplicationException))]
      public void Update_ClassNoPrimaryKeyValue_Exception()
      {
         _classUndertest.Update(new Classes.BadClassNullablePrimaryKey());
      }

      [TestMethod]
      [ExpectedException(typeof(ApplicationException))]
      public void Update_ClassNoPrimaryKey_Exception()
      {
         _classUndertest.Update(new Classes.BadClassNoAttributes.Customer());
      }

      [TestMethod]
      [ExpectedException(typeof(ApplicationException))]
      public void Update_StructNoColumns_Exception()
      {
         _classUndertest.Update(new Structs.BadStructNoColumns());
      }

      [TestMethod]
      [ExpectedException(typeof(ApplicationException))]
      public void Update_StructNoPrimaryKeyValue_Exception()
      {
         _classUndertest.Update(new Structs.BadStructNullablePrimaryKey());
      }

      [TestMethod]
      [ExpectedException(typeof(ApplicationException))]
      public void Update_StructNoPrimaryKey_Exception()
      {
         _classUndertest.Update(new Structs.BadStructNoAttributes.Customer());
      }

      [TestMethod]
      public void Update_UpdateObjects_Success()
      {
         Structs.Customer testCustomer = Structs.TestCustomer;
         testCustomer.CustomerId = _classUndertest.Create<int>(testCustomer);

         Structs.Customer updatedCustomer = new Structs.Customer {
                                                                    CustomerId = testCustomer.CustomerId, 
                                                                    Address = "xxx", 
                                                                    Name = "yyy"
                                                                 };

         _classUndertest.Update(updatedCustomer);

         Structs.Customer actualUpdatedCustomer = _classUndertest.Retrieve<Structs.Customer>(testCustomer.CustomerId);

         Assert.AreEqual(updatedCustomer, actualUpdatedCustomer, "Customer not updated as expected");

         // cleanup
         _classUndertest.Delete<Structs.Customer>(testCustomer.CustomerId);
      }

      [TestMethod]
      public void DatabaseTest()
      {
         ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[ConnectionStringName];

         _classUndertest = new Database(connectionStringSettings);

         Assert.AreEqual(connectionStringSettings.ConnectionString, _classUndertest.ConnectionString, "Unexpected connection string");
      }
   }
}
