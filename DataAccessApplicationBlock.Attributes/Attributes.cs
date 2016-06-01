using System;

namespace DataAccessApplicationBlock.Attributes
{
   public static class ColumnAttributes
   {
      public class Identity : Attribute
      {
      }

      public class PrimaryKey : Attribute
      {
      }

      public class NotInTable : Attribute
      {
      }

      public class HasColumnDefault : Attribute
      {
      }
   }

   public static class ClassAttributes
   {

      public class Table : Attribute
      {
      }

      public class StoredProcedure : Attribute
      {
      }

      public class View : Attribute
      {
      }
   }

}