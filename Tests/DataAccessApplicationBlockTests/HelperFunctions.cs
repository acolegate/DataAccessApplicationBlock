using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DataAccessApplicationBlockTests
{
   [ExcludeFromCodeCoverage]
   public static class HelperFunctions
   {
      public static class Helper<T>
      {
         /// <summary>Compares the specified x and y objects</summary>
         /// <param name="x">The x.</param>
         /// <param name="y">The y.</param>
         /// <returns>Whether the two objects are the same</returns>
         public static bool Compare(T x, T y)
         {
            Type type = typeof(T);

            foreach (PropertyInfo p in type.GetProperties())
            {
               IComparable valueOfX = p.GetValue(x, null) as IComparable;
               if (valueOfX != null)
               {
                  object valueOfY = p.GetValue(y, null);
                  if (valueOfX.CompareTo(valueOfY) != 0)
                  {
                     Console.Write("Compare failed when comparing {0} objects. Property: {1}. Values: {2} and {3}", type.Name, p.Name, valueOfX, valueOfY);
                     return false;
                  }
               }
            }

            foreach (FieldInfo f in type.GetFields())
            {
               IComparable valueOfX = f.GetValue(x) as IComparable;
               if (valueOfX != null)
               {
                  object valueOfY = f.GetValue(y);
                  if (valueOfX.CompareTo(valueOfY) != 0)
                  {
                     Console.Write("Compare failed when comparing {0} objects. Field: {1}. Values: {2} and {3}", type.Name, f.Name, valueOfX, valueOfY);
                     return false;
                  }
               }
            }

            return true;
         }
      }
   }
}
