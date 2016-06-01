using System;
using System.Diagnostics.CodeAnalysis;

using DataAccessApplicationBlock.Attributes;

namespace DataAccessApplicationBlockTests
{
   [ExcludeFromCodeCoverage]
   public static class Structs
   {
      public static Customer TestCustomer
      {
         get
         {
            return new Customer {
                                   Address = "A", 
                                   Name = "B"
                                };
         }
      }

      public struct BadStructNoColumns
      {
      }

      public struct BadStructNoAttributes
      {
         public struct Customer
         {
            public string Address;

            public int CustomerId;

            public string Name;
         }
      }

      public struct BadStructNullablePrimaryKey
      {
         [ColumnAttributes.PrimaryKey]
         public int? Id;
      }

      public struct Customer
      {
         public string Address;

         [ColumnAttributes.PrimaryKey]
         [ColumnAttributes.Identity]
         public int CustomerId;

         public string Name;
      }

      public struct TestWithNullableColumns
      {
         [ColumnAttributes.PrimaryKey]
         [ColumnAttributes.Identity]
         public int Id;

         public int? NullableInt;
         public string NativeString;
         public DateTime? NullableDateTime;
      }

      public struct Item
      {
         [ColumnAttributes.PrimaryKey]
         [ColumnAttributes.Identity]
         public int ItemId;

         public string Name;
         public decimal UnitPrice;
      }

      public struct Order
      {
         public int CustomerId;

         [ColumnAttributes.Identity]
         [ColumnAttributes.PrimaryKey]
         public int OrderId;
      }

      public struct OrderItem
      {
         public int ItemId;
         public int OrderId;

         [ColumnAttributes.PrimaryKey]
         [ColumnAttributes.Identity]
         public int OrderItemId;

         public int Quantity;
      }
   }

   [ExcludeFromCodeCoverage]
   public static class Classes
   {
      public class BadClassNoColumns
      {
      }

      public class BadClassNoAttributes
      {
         public class Customer
         {
            public string Address { get; set; }

            public int CustomerId { get; set; }

            public string Name { get; set; }
         }
      }

      public class BadClassNullablePrimaryKey
      {
         [ColumnAttributes.PrimaryKey]
         public int? Id { get; set; }
      }

      public class Customer
      {
         public string Address { get; set; }

         [ColumnAttributes.PrimaryKey]
         [ColumnAttributes.Identity]
         public int CustomerId { get; set; }

         public string Name { get; set; }
      }

      public class Item
      {
         [ColumnAttributes.PrimaryKey]
         [ColumnAttributes.Identity]
         public int ItemId { get; set; }

         public string Name { get; set; }
         public decimal UnitPrice { get; set; }
      }

      public class Order
      {
         public int CustomerId { get; set; }

         [ColumnAttributes.Identity]
         [ColumnAttributes.PrimaryKey]
         public int OrderId { get; set; }
      }

      public class OrderItem
      {
         public int ItemId { get; set; }
         public int OrderId { get; set; }

         [ColumnAttributes.PrimaryKey]
         [ColumnAttributes.Identity]
         public int OrderItemId { get; set; }

         public int Quantity { get; set; }
      }
   }
}
