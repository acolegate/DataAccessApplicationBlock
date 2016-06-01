using System;
using System.Diagnostics.CodeAnalysis;

using DataAccessApplicationBlock;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataAccessApplicationBlockTests
{
   [TestClass]
   [ExcludeFromCodeCoverage]
   public class ExtensionMethodTests
   {
      [TestMethod]
      [ExpectedException(typeof(ArgumentException))]
      public void ChangeType_NullIntoNonNullableType2_Exception()
      {
         Assert.AreEqual(Activator.CreateInstance(typeof(int)), DBNull.Value.ChangeType(typeof(int)), "Unexpected result");
      }

      [TestMethod]
      [ExpectedException(typeof(ArgumentException))]
      public void ChangeType_NullIntoNonNullabletype_Exception()
      {
         object nullObject = null;

         Assert.AreEqual(null, nullObject.ChangeType(typeof(int)), "Exception not raised");
      }

      [TestMethod]
      public void ChangeType_Nulls_Success()
      {
         object nullObject = null;

         Assert.AreEqual(null, DBNull.Value.ChangeType(typeof(int?)), "Unexpected result");
         Assert.AreEqual(null, nullObject.ChangeType(typeof(int?)), "Unexpected result");
         Assert.AreEqual((int?)1, 1.ChangeType(typeof(int?)), "Unexpected result");

         Assert.AreEqual(null, nullObject.ChangeType(typeof(string)), "Unexpected result");
         Assert.AreEqual(Activator.CreateInstance(typeof(int)), nullObject.ChangeType(typeof(int), true), "Unexpected result");
      }
   }
}
