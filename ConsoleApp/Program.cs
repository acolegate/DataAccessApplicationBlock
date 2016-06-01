using System;
using System.Data;
using System.Diagnostics;

using DataAccessApplicationBlock;

namespace ConsoleApp
{
   public static class Program
   {
      private static readonly Database DatabaseInstance = new Database("main");
      private static readonly Random Rand = new Random();

      [Flags]
      private enum StringType
      {
         Uppercase = 1, 
         Lowercase = 2, 
         Numerics = 4, 
         Symbols = 8
      }

      private static void Main()
      {
         // PopulateTestTable();
         PerformReadTests();

         DatabaseInstance.Dispose();

         PressAnyKeyToContinue();
      }

      private static void PerformReadTests()
      {
         long totalTimeWithCache = 0;

         Stopwatch stopwatch;

         for (int i = 0; i < 10; i++)
         {
            stopwatch = Stopwatch.StartNew();
            DatabaseInstance.RetrieveList<Models.TestObject>("select * from TestTable");
            stopwatch.Stop();
            Console.WriteLine("Using cache : " + stopwatch.ElapsedMilliseconds);
            totalTimeWithCache += stopwatch.ElapsedMilliseconds;

            Console.WriteLine();
         }

         Console.WriteLine("Total with cache : " + totalTimeWithCache);
      }

      private static void PopulateTestTable()
      {
         DatabaseInstance.SqlParameters.Clear();

         DatabaseInstance.ExecuteNonQuery("truncate table testtable");

         DatabaseInstance.SqlParameters.Add("@column01", SqlDbType.VarChar, 400, false);
         DatabaseInstance.SqlParameters.Add("@column02", SqlDbType.VarChar, 400, false);
         DatabaseInstance.SqlParameters.Add("@column03", SqlDbType.VarChar, 400, false);
         DatabaseInstance.SqlParameters.Add("@column04", SqlDbType.VarChar, 400, false);
         DatabaseInstance.SqlParameters.Add("@column05", SqlDbType.VarChar, 400, false);
         DatabaseInstance.SqlParameters.Add("@column06", SqlDbType.VarChar, 400, false);
         DatabaseInstance.SqlParameters.Add("@column07", SqlDbType.VarChar, 400, false);
         DatabaseInstance.SqlParameters.Add("@column08", SqlDbType.VarChar, 400, false);
         DatabaseInstance.SqlParameters.Add("@column09", SqlDbType.VarChar, 400, false);
         DatabaseInstance.SqlParameters.Add("@column10", SqlDbType.VarChar, 400, false);
         {
            for (int i = 0; i < 10000; i++)
            {
               DatabaseInstance.SqlParameters["@column01"].Value = RandomString(StringType.Lowercase | StringType.Uppercase, 400);
               DatabaseInstance.SqlParameters["@column02"].Value = RandomString(StringType.Lowercase | StringType.Uppercase, 400);
               DatabaseInstance.SqlParameters["@column03"].Value = RandomString(StringType.Lowercase | StringType.Uppercase, 400);
               DatabaseInstance.SqlParameters["@column04"].Value = RandomString(StringType.Lowercase | StringType.Uppercase, 400);
               DatabaseInstance.SqlParameters["@column05"].Value = RandomString(StringType.Lowercase | StringType.Uppercase, 400);
               DatabaseInstance.SqlParameters["@column06"].Value = RandomString(StringType.Lowercase | StringType.Uppercase, 400);
               DatabaseInstance.SqlParameters["@column07"].Value = RandomString(StringType.Lowercase | StringType.Uppercase, 400);
               DatabaseInstance.SqlParameters["@column08"].Value = RandomString(StringType.Lowercase | StringType.Uppercase, 400);
               DatabaseInstance.SqlParameters["@column09"].Value = RandomString(StringType.Lowercase | StringType.Uppercase, 400);
               DatabaseInstance.SqlParameters["@column10"].Value = RandomString(StringType.Lowercase | StringType.Uppercase, 400);

               DatabaseInstance.ExecuteNonQuery("insert into testtable(column01,column02,column03,column04,column05,column06,column07,column08,column09,column10) values(@column01, @column02, @column03, @column04, @column05, @column06, @column07, @column08, @column09, @column10)");
            }
         }

         DatabaseInstance.SqlParameters.Clear();
      }

      private static void PressAnyKeyToContinue()
      {
         Console.WriteLine();
         Console.WriteLine("Press any key to continue . . .");
         Console.ReadKey(true);
      }

      /// <summary>Generates a random string.</summary>
      /// <param name="stringType">Type of the string.</param>
      /// <param name="size">The size.</param>
      /// <returns>A random string</returns>
      private static string RandomString(StringType stringType, int size)
      {
         const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
         const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
         const string Numerics = "0123456789";
         const string Symbols = "!\"£$%^&*()_+-={}[]:@~;'#<>?,./\\|";

         string possibleChars = string.Empty;

         if ((stringType & StringType.Uppercase) != 0)
         {
            possibleChars += Uppercase;
         }

         if ((stringType & StringType.Lowercase) != 0)
         {
            possibleChars += Lowercase;
         }

         if ((stringType & StringType.Numerics) != 0)
         {
            possibleChars += Numerics;
         }

         if ((stringType & StringType.Symbols) != 0)
         {
            possibleChars += Symbols;
         }

         int possibleCharsLength = possibleChars.Length;

         char[] buffer = new char[size];

         for (int i = 0; i < size; i++)
         {
            buffer[i] = possibleChars[Rand.Next(possibleCharsLength)];
         }

         return new string(buffer);
      }
   }
}
