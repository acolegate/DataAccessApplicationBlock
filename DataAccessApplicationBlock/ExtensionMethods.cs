using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace DataAccessApplicationBlock
{
   internal static class ExtensionMethods
   {
      /// <summary>Changes the type taking into account DBNull values and nullable types.</summary>
      /// <param name="value">The value.</param>
      /// <param name="type">The type.</param>
      /// <param name="defaultIfNull">if set to <c>true</c> [default if null].</param>
      /// <returns>A suitable value for the type</returns>
      /// <exception cref="System.ArgumentException">Cannot use null with a non-nullable type</exception>
      internal static object ChangeType(this object value, Type type, bool defaultIfNull = false)
      {
         Type underlyingType = Nullable.GetUnderlyingType(type);

         if (underlyingType != null)
         {
            // nullable type
            return value == DBNull.Value || value == null ? null : Convert.ChangeType(value, underlyingType);
         }

         // else: non-nullable type - return null for strings or a default instance
         if (value == DBNull.Value || value == null)
         {
            if (type == typeof(string))
            {
               return null;
            }

            // else
            // {
            if (defaultIfNull)
            {
               return Activator.CreateInstance(type);
            }

            // else
            // {
            throw new ArgumentException("Cannot use null with a non-nullable type");

            // }
            // }
         }

         // else: the value can be converted to the type
         return Convert.ChangeType(value, type);
      }

      /// <summary>Gets the column names as a string[]</summary>
      /// <param name="dataTable">The data table.</param>
      /// <returns>The column names</returns>
      internal static string[] ColumnNames(this DataTable dataTable)
      {
         return (from DataColumn column in dataTable.Columns select column.ColumnName).ToArray();
      }

      /// <summary>Determines whether the specified custom attributes has a given attribute.</summary>
      /// <param name="customAttributes">The custom attributes.</param>
      /// <param name="attributeType">The attribute.</param>
      /// <returns>True if the custom attributes has the given attribute</returns>
      internal static bool HasAttribute(this IEnumerable<Attribute> customAttributes, Type attributeType)
      {
         return customAttributes.Any(customAttribute => customAttribute.GetType() == attributeType);
      }

      /// <summary>Converts a datarow into a populated Type</summary>
      /// <typeparam name="T">The Type</typeparam>
      /// <param name="dataRow">The data row.</param>
      /// <param name="propertyInfos">The property infos for the target type</param>
      /// <param name="fieldInfos">The field infos for the target type</param>
      /// <returns>A populated type</returns>
      internal static T ToType<T>(this DataRow dataRow, IEnumerable<PropertyInfo> propertyInfos, IEnumerable<FieldInfo> fieldInfos)
      {
         // This has to be an object as opposed to the generic type T
         object objectToReturn = Activator.CreateInstance<T>();

         foreach (PropertyInfo property in propertyInfos)
         {
            property.SetValue(objectToReturn, dataRow[property.Name].ChangeType(property.PropertyType), null);
         }

         foreach (FieldInfo field in fieldInfos)
         {
            field.SetValue(objectToReturn, dataRow[field.Name].ChangeType(field.FieldType));
         }

         // This has to be unboxed into the generic type. Explicitly using the generic T doesn't work
         return (T)objectToReturn;
      }
   }
}
