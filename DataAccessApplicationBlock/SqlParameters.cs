using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DataAccessApplicationBlock
{
   /// <summary>
   /// A custom Sql Parameters collection class. Unfortunately if a SqlParameter is used in a SqlCommand, it can't be re-used against another SqlCommand.
   /// This restriction requires that this custom collection class was written which performs similarly to the standard SqlParameterCollection object,
   /// but SqlParameter instances are only created at the last minute when they're converted to an array for use in the SqlCommand.AddRange() method.
   /// </summary>
   public class SqlParameters : IDisposable
   {
      private readonly List<CustomSqlParameter> _customSqlParameters;

      /// <summary>Initializes a new instance of the <see cref="SqlParameters"/> class.</summary>
      public SqlParameters()
      {
         _customSqlParameters = new List<CustomSqlParameter>();
      }

      /// <summary>Gets the count.</summary>
      /// <value>The count.</value>
      public int Count
      {
         get
         {
            return _customSqlParameters.Count;
         }
      }

      /// <summary>Gets the colleciton as an array for use with the SqlCommand.AddRange() method</summary>
      /// <value>An array of SqlParameters</value>
      public SqlParameter[] ToArray
      {
         get
         {
            return new List<SqlParameter>(_customSqlParameters.Select(MakeSqlParameter)).ToArray();
         }
      }

      /// <summary>Gets or sets the <see cref="SqlParameter" /> at the specified index.</summary>
      /// <value>The <see cref="SqlParameter" />.</value>
      /// <param name="index">The index.</param>
      /// <returns>A sql parameter</returns>
      public SqlParameter this[int index]
      {
         get
         {
            return MakeSqlParameter(_customSqlParameters[index]);
         }
      }

      /// <summary>Gets the <see cref="CustomSqlParameter" /> with the specified name.</summary>
      /// <value>The <see cref="CustomSqlParameter" />.</value>
      /// <param name="name">The name.</param>
      /// <returns>A Sql Parameter</returns>
      public SqlParameter this[string name]
      {
         get
         {
            return MakeSqlParameter(_customSqlParameters.First(x => x.Name == name));
         }
      }

      /// <summary>Adds a new sql parameter to the collection</summary>
      /// <param name="name">The name.</param>
      /// <param name="sqlDbType">Type of the SQL database.</param>
      /// <param name="size">The size.</param>
      /// <param name="isNullable">if set to <c>true</c> [is nullable].</param>
      public void Add(string name, SqlDbType sqlDbType, int size, bool isNullable)
      {
         _customSqlParameters.Add(new CustomSqlParameter(name, sqlDbType, size, isNullable));
      }

      /// <summary>Adds a new Sql Parameter to the collection</summary>
      /// <param name="name">The name.</param>
      /// <param name="sqlDbType">Type of the SQL database.</param>
      /// <param name="isNullable">if set to <c>true</c> [is nullable].</param>
      /// <param name="direction">The direction.</param>
      public void Add(string name, SqlDbType sqlDbType, bool isNullable, ParameterDirection direction)
      {
         _customSqlParameters.Add(new CustomSqlParameter(name, sqlDbType, isNullable, direction));
      }

      /// <summary>Adds a range of SqlParameters to the collection</summary>
      /// <param name="sqlParameters">The SQL parameters.</param>
      public void AddRange(IEnumerable<SqlParameter> sqlParameters)
      {
         foreach (SqlParameter sqlParameter in sqlParameters)
         {
            AddWithValue(sqlParameter.ParameterName, sqlParameter.SqlDbType, sqlParameter.Size, sqlParameter.Value, sqlParameter.IsNullable, sqlParameter.Direction);
         }
      }

      /// <summary>Adds a new Sql Parameter to the collection.</summary>
      /// <param name="name">The name.</param>
      /// <param name="sqlDbType">Type of the SQL database.</param>
      /// <param name="size">The size.</param>
      /// <param name="value">The value.</param>
      /// <param name="isNullable">if set to <c>true</c> [is nullable].</param>
      public void AddWithValue(string name, SqlDbType sqlDbType, int size, object value, bool isNullable)
      {
         _customSqlParameters.Add(new CustomSqlParameter(name, sqlDbType, size, value, isNullable));
      }

      /// <summary>Adds a new Sql Parameter to the collection.</summary>
      /// <param name="name">The name.</param>
      /// <param name="sqlDbType">Type of the SQL database.</param>
      /// <param name="size">The size.</param>
      /// <param name="value">The value.</param>
      /// <param name="isNullable">if set to <c>true</c> [is nullable].</param>
      /// <param name="direction">The direction.</param>
      public void AddWithValue(string name, SqlDbType sqlDbType, int size, object value, bool isNullable, ParameterDirection direction)
      {
         _customSqlParameters.Add(new CustomSqlParameter(name, sqlDbType, size, value, isNullable, direction));
      }

      /// <summary>Adds the with value.</summary>
      /// <param name="name">The name.</param>
      /// <param name="sqlDbType">Type of the SQL database.</param>
      /// <param name="value">The value.</param>
      /// <param name="isNullable">if set to <c>true</c> [is nullable].</param>
      public void AddWithValue(string name, SqlDbType sqlDbType, object value, bool isNullable)
      {
         _customSqlParameters.Add(new CustomSqlParameter(name, sqlDbType, value, isNullable));
      }

      /// <summary>Adds the with value.</summary>
      /// <param name="name">The name.</param>
      /// <param name="sqlDbType">Type of the SQL database.</param>
      /// <param name="value">The value.</param>
      /// <param name="isNullable">if set to <c>true</c> [is nullable].</param>
      /// <param name="direction">The direction.</param>
      public void AddWithValue(string name, SqlDbType sqlDbType, object value, bool isNullable, ParameterDirection direction)
      {
         _customSqlParameters.Add(new CustomSqlParameter(name, sqlDbType, value, isNullable, direction));
      }

      /// <summary>Clears the collection</summary>
      public void Clear()
      {
         _customSqlParameters.Clear();
      }

      /// <summary>
      /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
      /// </summary>
      public void Dispose()
      {
         _customSqlParameters.Clear();
      }

      /// <summary>Makes the SQL parameter.</summary>
      /// <param name="customSqlParameter">The custom SQL parameter.</param>
      /// <returns>A Sql Parameter</returns>
      private static SqlParameter MakeSqlParameter(CustomSqlParameter customSqlParameter)
      {
         SqlParameter sqlParameter = new SqlParameter(customSqlParameter.Name, customSqlParameter.Value) {
                                                                                                            IsNullable = customSqlParameter.IsNullable
                                                                                                         };

         if (customSqlParameter.Direction != null)
         {
            sqlParameter.Direction = customSqlParameter.Direction.Value;
         }

         if (customSqlParameter.SqlDbType != null)
         {
            sqlParameter.SqlDbType = customSqlParameter.SqlDbType.Value;
         }

         if (customSqlParameter.Size != null)
         {
            sqlParameter.Size = customSqlParameter.Size.Value;
         }

         return sqlParameter;
      }

      private class CustomSqlParameter
      {
         public CustomSqlParameter(string name, object value, bool isNullable)
         {
            Name = name;
            Value = value;
            IsNullable = isNullable;
         }

         public CustomSqlParameter(string name, SqlDbType sqlDbType, bool isNullable, ParameterDirection direction)
         {
            Name = name;
            Direction = direction;
            SqlDbType = sqlDbType;
            IsNullable = isNullable;
         }

         public CustomSqlParameter(string name, SqlDbType sqlDbType, int size, bool isNullable)
         {
            Name = name;
            SqlDbType = sqlDbType;
            Size = size;
            IsNullable = isNullable;
         }

         public CustomSqlParameter(string name, SqlDbType sqlDbType, int size, object value, bool isNullable)
         {
            Name = name;
            SqlDbType = sqlDbType;
            Size = size;
            Value = value;
            IsNullable = isNullable;
         }

         public CustomSqlParameter(string name, SqlDbType sqlDbType, int size, object value, bool isNullable, ParameterDirection direction)
         {
            Name = name;
            SqlDbType = sqlDbType;
            Size = size;
            Value = value;
            Direction = direction;
            IsNullable = isNullable;
         }

         public CustomSqlParameter(string name, SqlDbType sqlDbType, object value, bool isNullable)
         {
            Name = name;
            SqlDbType = sqlDbType;
            Value = value;
            IsNullable = isNullable;
         }

         public CustomSqlParameter(string name, SqlDbType sqlDbType, object value, bool isNullable, ParameterDirection direction)
         {
            Name = name;
            SqlDbType = sqlDbType;
            Value = value;
            Direction = direction;
            IsNullable = isNullable;
         }

         public ParameterDirection? Direction { get; private set; }
         public string Name { get; private set; }
         public int? Size { get; private set; }
         public SqlDbType? SqlDbType { get; private set; }
         public object Value { get; private set; }
         public bool IsNullable { get; private set; }
      }
   }
}
