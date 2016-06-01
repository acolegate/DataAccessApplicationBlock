using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using DataAccessApplicationBlock;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataAccessApplicationBlockTests
{
   [TestClass]
   [ExcludeFromCodeCoverage]
   public class DataTests
   {
      private const string ConnectionStringName = "main";
      private const string SqlSelectCustomers = "declare @temptable table(CustomerId int, Name varchar(50), Address varchar(50));insert into @temptable(CustomerId, Name, Address) values(1, 'A', 'B');insert into @temptable(CustomerId, Name, Address) values(2, 'C', 'D');insert into @temptable(CustomerId, Name, Address) values(3, 'E', 'F');select * from @temptable;";
      private const string SqlSelectItem = "Select 1 as ItemId, 'Item name' as Name, 1.234 as Unitprice";
      private const string SqlSelectItems = "declare @temptable table(ItemId int, Name varchar(50), UnitPrice decimal(19,6));insert into @temptable(ItemId, Name, UnitPrice) values(1, 'A', 123.45);insert into @temptable(ItemId, Name, UnitPrice) values(2, 'B', 234.56);insert into @temptable(ItemId, Name, UnitPrice) values(3, 'C', 345.67);select * from @temptable;";

      private const string SqlSelectOrder = "Select 1 as OrderId, 1 as CustomerId";
      private const string SqlSelectOrderItem = "Select 1 as OrderItemId, 1 as OrderId, 1 as ItemId, 1 as Quantity";
      private const string SqlSelectOrderItems = "declare @temptable table(OrderItemId int, OrderId int, ItemId int, Quantity decimal(19,6));insert into @temptable(OrderItemId, OrderId, ItemId, Quantity) values(1, 2, 3, 123.45);insert into @temptable(OrderItemId, OrderId, ItemId, Quantity) values(4, 5, 6, 234.56);insert into @temptable(OrderItemId, OrderId, ItemId, Quantity) values(7, 8, 9, 345.67);select * from @temptable;";
      private const string SqlSelectOrders = "declare @temptable table(OrderId int, CustomerId int);insert into @temptable(OrderId, CustomerId) values(1, 2);insert into @temptable(OrderId, CustomerId) values(3, 4);insert into @temptable(OrderId, CustomerId) values(5, 6);select * from @temptable;";
      
      private Database _classUndertest;

      [TestCleanup]
      public void Cleanup()
      {
         MapCaches_AreCleared_Success();

         _classUndertest.Dispose();
      }

      [TestMethod]
      public void FieldMapCache_IsPopulated_Success()
      {
         Data.Instance.ClearCaches();

         const string SqlSelectCustomer = "Select 1 as CustomerId, 'Name value' as Name, 'Address value' as Address";

         for (int i = 0; i < 3; i++)
         {
            Structs.Customer customerStruct = _classUndertest.Retrieve<Structs.Customer>(SqlSelectCustomer);
            Structs.Order orderStruct = _classUndertest.Retrieve<Structs.Order>(SqlSelectOrder);
            Structs.OrderItem orderItemStruct = _classUndertest.Retrieve<Structs.OrderItem>(SqlSelectOrderItem);
            Structs.Item itemStruct = _classUndertest.Retrieve<Structs.Item>(SqlSelectItem);

            List<Structs.Customer> customerStructs = _classUndertest.RetrieveList<Structs.Customer>(SqlSelectCustomers);
            List<Structs.Order> orderStructs = _classUndertest.RetrieveList<Structs.Order>(SqlSelectOrders);
            List<Structs.OrderItem> orderItemStructs = _classUndertest.RetrieveList<Structs.OrderItem>(SqlSelectOrderItems);
            List<Structs.Item> itemStructs = _classUndertest.RetrieveList<Structs.Item>(SqlSelectItems);

            Assert.AreEqual(4, Data.Instance.FieldMapCache.Count, "Unexpected number of items in fieldmapcache");
            Assert.AreEqual(0, Data.Instance.PropertyMapCache.Count, "Unexpected number of items in propertyMapcache");
         }

         Data.Instance.ClearCaches();

         for (int i = 0; i < 3; i++)
         {
            Classes.Customer customerClass = _classUndertest.Retrieve<Classes.Customer>(SqlSelectCustomer);
            Classes.Order orderClass = _classUndertest.Retrieve<Classes.Order>(SqlSelectOrder);
            Classes.OrderItem orderItemClass = _classUndertest.Retrieve<Classes.OrderItem>(SqlSelectOrderItem);
            Classes.Item itemClass = _classUndertest.Retrieve<Classes.Item>(SqlSelectItem);

            List<Classes.Customer> customerClasses = _classUndertest.RetrieveList<Classes.Customer>(SqlSelectCustomers);
            List<Classes.Order> orderClasses = _classUndertest.RetrieveList<Classes.Order>(SqlSelectOrders);
            List<Classes.OrderItem> orderItemClasses = _classUndertest.RetrieveList<Classes.OrderItem>(SqlSelectOrderItems);
            List<Classes.Item> itemClasses = _classUndertest.RetrieveList<Classes.Item>(SqlSelectItems);

            Assert.AreEqual(0, Data.Instance.FieldMapCache.Count, "Unexpected number of items in fieldmapcache");
            Assert.AreEqual(4, Data.Instance.PropertyMapCache.Count, "Unexpected number of items in propertyMapcache");
         }
      }

      [TestInitialize]
      public void Initialise()
      {
         MapCaches_AreCleared_Success();

         _classUndertest = new Database(ConnectionStringName);
      }

      [TestMethod]
      public void MapCaches_AreCleared_Success()
      {
         Data.Instance.ClearCaches();

         Dictionary<string, IEnumerable<FieldInfo>> fieldMapCache = Data.Instance.FieldMapCache;
         Assert.AreEqual(0, fieldMapCache.Count, "Fieldmapcache should be empty");

         Dictionary<string, IEnumerable<PropertyInfo>> propertyMapCache = Data.Instance.PropertyMapCache;
         Assert.AreEqual(0, propertyMapCache.Count, "propertyMapCache should be empty");
      }
   }
}
