using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace DataAccessApplicationBlock
{
   /// <summary>
   /// Singleton class to convert datatables or datarows into populated type and type lists.
   /// A cache is maintained of typenames and sql column names and which properties/fields they can be mapped to.
   /// </summary>
   internal sealed class Data
   {
      private static readonly Data SingletonInstance = new Data();

      [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1409:RemoveUnnecessaryCode", Justification = "Reviewed. Suppression is OK here.")]
      static Data()
      {
         // reqiured by the singleton - do not remove
      }

      /// <summary>Prevents a default instance of the <see cref="Data"/> class from being created.</summary>
      private Data()
      {
         ClearCaches();
      }

      /// <summary>Gets the singleton instance.</summary>
      /// <value>The instance.</value>
      public static Data Instance
      {
         get
         {
            return SingletonInstance;
         }
      }

      /// <summary>Gets the field map cache.</summary>
      /// <value>The field map cache.</value>
      // ReSharper disable once MemberCanBePrivate.Global
      internal Dictionary<string, IEnumerable<FieldInfo>> FieldMapCache { get; private set; }

      /// <summary>Gets the property map cache.</summary>
      /// <value>The property map cache.</value>
      // ReSharper disable once MemberCanBePrivate.Global
      internal Dictionary<string, IEnumerable<PropertyInfo>> PropertyMapCache { get; private set; }

      /// <summary>Clears the caches.</summary>
      // ReSharper disable once MemberCanBePrivate.Global
      // ReSharper disable once MemberCanBeMadeStatic.Global
      internal void ClearCaches()
      {
         // DO NOT MAKE THIS STATIC
         PropertyMapCache = new Dictionary<string, IEnumerable<PropertyInfo>>();
         FieldMapCache = new Dictionary<string, IEnumerable<FieldInfo>>();
      }

      /// <summary>Converts a datarow to a populated type</summary>
      /// <typeparam name="T">The type</typeparam>
      /// <param name="dataRow">The data row.</param>
      /// <returns>A populated type</returns>
      internal T ConvertTo<T>(DataRow dataRow)
      {
         // DO NOT MAKE THIS STATIC OR AN EXTENSION METHOD
         Type type = typeof(T);
         string[] columnNames = dataRow.Table.ColumnNames();
         string key = MakeKey(type.FullName, columnNames);

         return dataRow.ToType<T>(SelectedPropertyInfos(type, columnNames, key), SelectedFieldInfos(type, columnNames, key));
      }

      /// <summary>Converts a datatable to a list of a given type</summary>
      /// <typeparam name="T">The type</typeparam>
      /// <param name="dataTable">The data table.</param>
      /// <returns>A list of populated types</returns>
      internal List<T> ToList<T>(DataTable dataTable)
      {
         // DO NOT MAKE THIS STATIC OR AN EXTENSION METHOD
         Type type = typeof(T);
         string[] columnNames = dataTable.ColumnNames();
         string key = MakeKey(type.FullName, columnNames);

         IEnumerable<PropertyInfo> selectedPropertyInfos = SelectedPropertyInfos(type, columnNames, key);
         IEnumerable<FieldInfo> selectedFieldInfos = SelectedFieldInfos(type, columnNames, key);

         return (from DataRow dataRow in dataTable.Rows select dataRow.ToType<T>(selectedPropertyInfos, selectedFieldInfos)).ToList();
      }

      /// <summary>Makes a compound key from the type name + | + a sorted CSV of the colum names</summary>
      /// <param name="typeName">Name of the type.</param>
      /// <param name="columnNames">The column names.</param>
      /// <returns>A unique key</returns>
      private static string MakeKey(string typeName, string[] columnNames)
      {
         Array.Sort(columnNames);

         return typeName + "|" + string.Join(",", columnNames).ToLower();
      }

      /// <summary>Gets a list of FieldInfos for a given type that can be mapped from the supplied columns</summary>
      /// <param name="type">The type.</param>
      /// <param name="columnNames">The column names.</param>
      /// <param name="key">The key to use to uniquely identify this type and the columns names</param>
      /// <returns>A collection of FieldInfos</returns>
      private IEnumerable<FieldInfo> SelectedFieldInfos(Type type, IEnumerable<string> columnNames, string key)
      {
         List<FieldInfo> selectedFieldInfos;

         FieldInfo[] fieldInfos = type.GetFields();
         if (fieldInfos.Any())
         {
            if (FieldMapCache.ContainsKey(key))
            {
               // already exists - retrieve it
               return FieldMapCache[key];
            }

            // else: this is new - store it away for later
            selectedFieldInfos = fieldInfos.Where(field => columnNames.Contains(field.Name, StringComparer.OrdinalIgnoreCase)).ToList();

            FieldMapCache.Add(key, selectedFieldInfos);
         }
         else
         {
            selectedFieldInfos = new List<FieldInfo>();
         }

         return selectedFieldInfos;
      }

      /// <summary>Gets a list of PropertyInfos for a given type that can be mapped from the supplied columns</summary>
      /// <param name="type">The type.</param>
      /// <param name="columnNames">The column names.</param>
      /// <param name="key">The key to use to uniquely identify this type and the columns names</param>
      /// <returns>A collection of PropertyInfos</returns>
      private IEnumerable<PropertyInfo> SelectedPropertyInfos(Type type, IEnumerable<string> columnNames, string key)
      {
         List<PropertyInfo> selectedPropertyInfos;

         PropertyInfo[] propertyInfos = type.GetProperties();

         if (propertyInfos.Any())
         {
            if (PropertyMapCache.ContainsKey(key))
            {
               // already exists - retrieve it
               return PropertyMapCache[key];
            }

            // else: this is new - store it away for later
            selectedPropertyInfos = propertyInfos.Where(property => columnNames.Contains(property.Name, StringComparer.OrdinalIgnoreCase)).ToList();

            PropertyMapCache.Add(key, selectedPropertyInfos);
         }
         else
         {
            selectedPropertyInfos = new List<PropertyInfo>();
         }

         return selectedPropertyInfos;
      }
   }
}
