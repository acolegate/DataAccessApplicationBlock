using System;
using System.Diagnostics.CodeAnalysis;

using DataAccessApplicationBlock;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataAccessApplicationBlockTests
{
   [TestClass]
   [ExcludeFromCodeCoverage]
   public static class AssemblySetup
   {
      private const string ConnectionStringName = "main";

      private static Database _classUnderTest;

      [AssemblyCleanup]
      public static void AssemblyCleanup()
      {
         _classUnderTest.ExecuteNonQuery("drop table dbo.OrderItem;");
         _classUnderTest.ExecuteNonQuery("drop table dbo.Item;");
         _classUnderTest.ExecuteNonQuery("drop table dbo.[Order];");
         _classUnderTest.ExecuteNonQuery("drop table Customer;");

         _classUnderTest.Dispose();
      }

      [AssemblyInitialize]
      public static void AssemblyInitialize(TestContext testContext)
      {
         _classUnderTest = new Database(ConnectionStringName);

         CreateTestTables();
      }

      private static void CreateTestTables()
      {
         _classUnderTest.ExecuteNonQuery("IF object_id('dbo.OrderItem') IS NOT NULL BEGIN DROP TABLE dbo.OrderItem;END;CREATE TABLE [dbo].[OrderItem]([OrderItemId] [int] IDENTITY(1,1) NOT NULL,[OrderId] [int] NOT NULL,[ItemId] [int] NOT NULL,CONSTRAINT [PK_OrderItem] PRIMARY KEY CLUSTERED ([OrderItemId] ASC));");
         _classUnderTest.ExecuteNonQuery("IF object_id('dbo.Item') IS NOT NULL BEGIN DROP TABLE dbo.Item;END;CREATE TABLE [dbo].[Item]([ItemId] [int] IDENTITY(1,1) NOT NULL,[Name] [varchar](50) NOT NULL,[Unitprice] [decimal](18, 6) NOT NULL,CONSTRAINT [PK_Item] PRIMARY KEY CLUSTERED ([ItemId] ASC));");
         _classUnderTest.ExecuteNonQuery("IF object_id('dbo.Order') IS NOT NULL BEGIN DROP TABLE dbo.[Order];END;CREATE TABLE [dbo].[Order] ([OrderId] [int] IDENTITY(1, 1) NOT NULL,[CustomerId] [int] NOT NULL,CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([OrderId] ASC))");
         _classUnderTest.ExecuteNonQuery("IF object_id('dbo.Customer') IS NOT NULL BEGIN DROP TABLE dbo.Customer;END;CREATE TABLE [dbo].[Customer]([CustomerId] [int] IDENTITY(1,1) NOT NULL,[Name] [varchar](50) NOT NULL,[Address] [varchar](50) NOT NULL,CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([CustomerId] ASC));");

         _classUnderTest.ExecuteNonQuery("ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Customer] FOREIGN KEY([CustomerId]) REFERENCES [dbo].[Customer] ([CustomerId])");
         _classUnderTest.ExecuteNonQuery("ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_Customer]");
         _classUnderTest.ExecuteNonQuery("ALTER TABLE [dbo].[OrderItem]  WITH CHECK ADD  CONSTRAINT [FK_OrderItem_Item] FOREIGN KEY([ItemId]) REFERENCES [dbo].[Item] ([ItemId])");
         _classUnderTest.ExecuteNonQuery("ALTER TABLE [dbo].[OrderItem] CHECK CONSTRAINT [FK_OrderItem_Item]");
         _classUnderTest.ExecuteNonQuery("ALTER TABLE [dbo].[OrderItem]  WITH CHECK ADD  CONSTRAINT [FK_OrderItem_Order] FOREIGN KEY([OrderId]) REFERENCES [dbo].[Order] ([OrderId])");
         _classUnderTest.ExecuteNonQuery("ALTER TABLE [dbo].[OrderItem] CHECK CONSTRAINT [FK_OrderItem_Order]");
      }

      private static void DropTestTables()
      {
         throw new NotImplementedException();
      }
   }
}
