using DataAccessApplicationBlock.Attributes;

namespace ConsoleApp
{
   public static class Models
   {
      // ReSharper disable once ClassNeverInstantiated.Global
      public class TestObject
      {
         [ColumnAttributes.PrimaryKey]
         public string Column01 { get; set; }
         public string Column02 { get; set; }
         public string Column03 { get; set; }
         public string Column04 { get; set; }
         public string Column05 { get; set; }
         public string Column06 { get; set; }
         public string Column07 { get; set; }
         public string Column08 { get; set; }
         public string Column09 { get; set; }
         public string Column10 { get; set; }
      }
   }
}
