using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DataAccessApplicationBlock
{
   public class Database : IDisposable
   {
      // Internal to allow testing
      internal readonly string ConnectionString;

      /// <summary>Initializes a new instance of the <see cref="Database"/> class.</summary>
      /// <param name="connectionStringName">Name of the connection string.</param>
      public Database(string connectionStringName)
      {
         ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
         InitialiseClass();
      }

      public Database(ConnectionStringSettings connectionStringSettings)
      {
         ConnectionString = connectionStringSettings.ConnectionString;
         InitialiseClass();
      }

      /// <summary>Gets the SQL parameters.</summary>
      /// <value>The SQL parameters.</value>
      public SqlParameters SqlParameters { get; private set; }

      /// <summary>Creates the specified object in the database and returns's it's primary ID</summary>
      /// <typeparam name="T">The type of primary key to return</typeparam>
      /// <param name="thisObject">The object to create</param>
      /// <returns>The ID of the primary key</returns>
      /// <exception cref="System.ApplicationException">No columns or values defined</exception>
      public T Create<T>(object thisObject)
      {
         Type type = thisObject.GetType();

         List<SqlParameter> sqlParameters = new List<SqlParameter>();

         const string SqlTemplate = "insert into [{0}]({1}) values({2});select scope_identity();";
         string sqlColumnNames = string.Empty;
         string sqlParameterNames = string.Empty;

         foreach (MemberInfo memberInfo in HelperFunctions.GetMemberInfo(thisObject).Where(memberInfo => memberInfo.IsIdentity == false && memberInfo.NotInTable == false))
         {
            if (!memberInfo.IsNullable || memberInfo.Value != null)
            {
               sqlParameters.Add(new SqlParameter {
                                                     ParameterName = "@" + memberInfo.Name,
                                                     IsNullable = memberInfo.IsNullable,
                                                     Value = memberInfo.Value,
                                                     SqlDbType = TypeConvertor.ToSqlDbType(memberInfo.Type)
                                                  });

               sqlColumnNames += string.Format("[{0}],", memberInfo.Name);
               sqlParameterNames += string.Format("@{0},", memberInfo.Name);
            }
         }

         if (sqlColumnNames.Length > 0 && sqlParameterNames.Length > 0)
         {
            // remove the last commas
            sqlColumnNames = sqlColumnNames.Substring(0, sqlColumnNames.Length - 1);
            sqlParameterNames = sqlParameterNames.Substring(0, sqlParameterNames.Length - 1);
         }
         else
         {
            throw new ApplicationException("No columns or values defined");
         }

         SqlParameters.Clear();
         SqlParameters.AddRange(sqlParameters.ToArray());

         return RetrieveScalarValue<T>(string.Format(SqlTemplate, type.Name, sqlColumnNames, sqlParameterNames));
      }

      /// <summary>Deletes an objetc from the database using the supplied primary key.</summary>
      /// <typeparam name="T">The type of object</typeparam>
      /// <param name="primaryKeyColumnValue">The primary key column value.</param>
      /// <exception cref="System.ApplicationException">
      /// [PrimaryKey] attribute not set for object
      /// or
      /// PrimaryKeyColumnValue not provided
      /// </exception>
      public void Delete<T>(object primaryKeyColumnValue)
      {
         Type type = typeof(T);

         MemberInfo primaryKeymember = type.GetMemberInfo().FirstOrDefault(x => x.IsPrimaryKey);

         if (primaryKeymember != null)
         {
            if (primaryKeyColumnValue == null)
            {
               throw new ApplicationException("PrimaryKeyColumnValue not provided");
            }

            SqlParameters.Clear();
            SqlParameters.AddWithValue("@PrimaryKeyColumnValue", TypeConvertor.ToSqlDbType(primaryKeymember.Type), primaryKeyColumnValue, primaryKeymember.IsNullable);

            ExecuteNonQuery(string.Format("delete [{0}] where [{1}]=@PrimaryKeyColumnValue;", type.Name, primaryKeymember.Name));
         }
         else
         {
            throw new ApplicationException("[PrimaryKey] attribute not set for object");
         }
      }

      /// <summary>
      /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
      /// </summary>
      public void Dispose()
      {
         SqlParameters.Dispose();
      }

      /// <summary>Executes a non query.</summary>
      /// <param name="sql">The SQL.</param>
      public void ExecuteNonQuery(string sql)
      {
         using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
         {
            sqlConnection.Open();

            using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) {
                                                                                 CommandType = CommandType.Text
                                                                              })
            {
               if (SqlParameters.Count > 0)
               {
                  sqlCommand.Parameters.AddRange(SqlParameters.ToArray);
               }

               sqlCommand.ExecuteNonQuery();
            }
         }
      }

      /// <summary>Retrieves the specified object from the database using the supplied primary key.</summary>
      /// <typeparam name="T">The type of object to retrieve</typeparam>
      /// <param name="primaryKeyColumnValue">The primary key column value.</param>
      /// <returns>A populated object</returns>
      /// <exception cref="System.ApplicationException">
      /// [PrimaryKey] attribute not set for object
      /// or
      /// PrimaryKeyColumnValue not provided
      /// </exception>
      public T Retrieve<T>(object primaryKeyColumnValue)
      {
         Type type = typeof(T);

         MemberInfo primaryKeymember = type.GetMemberInfo().FirstOrDefault(x => x.IsPrimaryKey);

         if (primaryKeymember != null)
         {
            if (primaryKeyColumnValue == null)
            {
               throw new ApplicationException("PrimaryKeyColumnValue not provided");
            }

            SqlParameters.Clear();
            SqlParameters.AddWithValue("@PrimaryKeyColumnValue", TypeConvertor.ToSqlDbType(primaryKeymember.Type), primaryKeyColumnValue, primaryKeymember.IsNullable);

            return Retrieve<T>(string.Format("select * from [{0}] where [{1}]=@PrimaryKeyColumnValue;", type.Name, primaryKeymember.Name));
         }
         else
         {
            throw new ApplicationException("[PrimaryKey] attribute not set for object");
         }
      }

      /// <summary>Retrieves an object of type T using the supplied sql</summary>
      /// <typeparam name="T">The type of object</typeparam>
      /// <param name="sql">The SQL.</param>
      /// <returns>A populated object</returns>
      public T Retrieve<T>(string sql)
      {
         using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
         {
            sqlConnection.Open();

            using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) {
                                                                                 CommandType = CommandType.Text
                                                                              })
            {
               if (SqlParameters.Count > 0)
               {
                  sqlCommand.Parameters.Clear();
                  sqlCommand.Parameters.AddRange(SqlParameters.ToArray);
               }

               using (DataTable dataTable = new DataTable())
               {
                  new SqlDataAdapter(sqlCommand).Fill(dataTable);
                  return dataTable.Rows.Count > 0 ? Data.Instance.ConvertTo<T>(dataTable.Rows[0]) : Activator.CreateInstance<T>();
               }
            }
         }
      }

      /// <summary>Retrieves an collection of objects of type T using the supplied sql</summary>
      /// <typeparam name="T">The type of object</typeparam>
      /// <param name="sql">The SQL.</param>
      /// <returns>A populated list of objects</returns>
      public List<T> RetrieveList<T>(string sql)
      {
         using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
         {
            sqlConnection.Open();

            using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) {
                                                                                 CommandType = CommandType.Text
                                                                              })
            {
               if (SqlParameters.Count > 0)
               {
                  sqlCommand.Parameters.Clear();
                  sqlCommand.Parameters.AddRange(SqlParameters.ToArray);
               }

               using (DataTable dataTable = new DataTable())
               {
                  new SqlDataAdapter(sqlCommand).Fill(dataTable);
                  return dataTable.Rows.Count > 0 ? Data.Instance.ToList<T>(dataTable) : new List<T>();
               }
            }
         }
      }

      public List<T> RetrieveList<T>()
      {
         return RetrieveList<T>("select * from [" + typeof(T).Name + "]");
      }

      /// <summary>Retrieves a scalar value.</summary>
      /// <typeparam name="T">The type</typeparam>
      /// <param name="sql">The SQL.</param>
      /// <returns>The first column of the first row from the first datatable.</returns>
      public T RetrieveScalarValue<T>(string sql)
      {
         using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
         {
            sqlConnection.Open();

            using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection) {
                                                                                 CommandType = CommandType.Text
                                                                              })
            {
               if (SqlParameters.Count > 0)
               {
                  sqlCommand.Parameters.Clear();
                  sqlCommand.Parameters.AddRange(SqlParameters.ToArray);
               }

               return (T)sqlCommand.ExecuteScalar().ChangeType(typeof(T));
            }
         }
      }

      /// <summary>
      /// Updates the object in the database. This only works if the type has a property/field with the [PrimaryKey] attribute set. If the [Identity] attribute is used, the column will not be included.
      /// </summary>
      /// <param name="thisObject">The object.</param>
      /// <exception cref="System.ApplicationException">No primary key attribute or value set for object</exception>
      public void Update(object thisObject)
      {
         List<SqlParameter> sqlParameters = new List<SqlParameter>();

         string sqlSets = string.Empty;

         MemberInfo primaryKeyMember = null;

         foreach (MemberInfo memberInfo in HelperFunctions.GetMemberInfo(thisObject))
         {
            if (memberInfo.IsIdentity == false && memberInfo.NotInTable == false)
            {
               sqlParameters.Add(new SqlParameter {
                                                     ParameterName = "@" + memberInfo.Name, 
                                                     IsNullable = memberInfo.IsNullable, 
                                                     Value = memberInfo.Value, 
                                                     SqlDbType = TypeConvertor.ToSqlDbType(memberInfo.Type)
                                                  });

               sqlSets += string.Format("[{0}]=@{0},", memberInfo.Name);
            }

            if (memberInfo.IsPrimaryKey)
            {
               if (memberInfo.IsNullable)
               {
                  throw new ApplicationException("Primary key cannot be a nullable type");
               }

               primaryKeyMember = memberInfo;
            }
         }

         if (primaryKeyMember == null)
         {

            throw new ApplicationException("No primary key attribute set for object");
         }

         if (sqlSets.Length > 0)
         {
            // remove the last comma
            sqlSets = sqlSets.Substring(0, sqlSets.Length - 1);
         }

         SqlParameters.Clear();
         SqlParameters.AddRange(sqlParameters);
         SqlParameters.AddWithValue("@PrimaryKeyColumnValue", TypeConvertor.ToSqlDbType(primaryKeyMember.Type), primaryKeyMember.Value, primaryKeyMember.IsNullable);

         ExecuteNonQuery(string.Format("update [{0}] set {1} where [{2}]=@PrimaryKeyColumnValue;", thisObject.GetType().Name, sqlSets, primaryKeyMember.Name));
      }

      private void InitialiseClass()
      {
         SqlParameters = new SqlParameters();

         ValidateConnection();
      }

      /// <summary>Validates the connection.</summary>
      private void ValidateConnection()
      {
         using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
         {
            try
            {
               sqlConnection.Open();
            }
            finally
            {
               sqlConnection.Close();
            }
         }
      }

      internal class MemberInfo
      {
         public bool HasColumnDefault { get; set; }
         public bool IsIdentity { get; set; }
         public bool IsNullable { get; set; }
         public bool IsPrimaryKey { get; set; }
         public string Name { get; set; }
         public bool NotInTable { get; set; }
         public Type Type { get; set; }
         public object Value { get; set; }
      }
   }
}
