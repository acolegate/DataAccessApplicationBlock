using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace DataAccessApplicationBlock
{
   /// <summary>Convert a base data type to another base data type</summary>
   [DebuggerStepThrough]
   public static class TypeConvertor
   {
      private static readonly ArrayList DbTypeList = new ArrayList();

      #region Constructors

      static TypeConvertor()
      {
         DbTypeMapEntry dbtypeMapEntry = new DbTypeMapEntry(typeof(bool), DbType.Boolean, SqlDbType.Bit);
         DbTypeList.Add(dbtypeMapEntry);

         dbtypeMapEntry = new DbTypeMapEntry(typeof(byte), DbType.Double, SqlDbType.TinyInt);
         DbTypeList.Add(dbtypeMapEntry);

         dbtypeMapEntry = new DbTypeMapEntry(typeof(byte[]), DbType.Binary, SqlDbType.Image);
         DbTypeList.Add(dbtypeMapEntry);

         dbtypeMapEntry = new DbTypeMapEntry(typeof(DateTime), DbType.DateTime, SqlDbType.DateTime);
         DbTypeList.Add(dbtypeMapEntry);

         dbtypeMapEntry = new DbTypeMapEntry(typeof(decimal), DbType.Decimal, SqlDbType.Decimal);
         DbTypeList.Add(dbtypeMapEntry);

         dbtypeMapEntry = new DbTypeMapEntry(typeof(double), DbType.Double, SqlDbType.Float);
         DbTypeList.Add(dbtypeMapEntry);

         dbtypeMapEntry = new DbTypeMapEntry(typeof(Guid), DbType.Guid, SqlDbType.UniqueIdentifier);
         DbTypeList.Add(dbtypeMapEntry);

         dbtypeMapEntry = new DbTypeMapEntry(typeof(short), DbType.Int16, SqlDbType.SmallInt);
         DbTypeList.Add(dbtypeMapEntry);

         dbtypeMapEntry = new DbTypeMapEntry(typeof(int), DbType.Int32, SqlDbType.Int);
         DbTypeList.Add(dbtypeMapEntry);

         dbtypeMapEntry = new DbTypeMapEntry(typeof(long), DbType.Int64, SqlDbType.BigInt);
         DbTypeList.Add(dbtypeMapEntry);

         dbtypeMapEntry = new DbTypeMapEntry(typeof(object), DbType.Object, SqlDbType.Variant);
         DbTypeList.Add(dbtypeMapEntry);

         dbtypeMapEntry = new DbTypeMapEntry(typeof(string), DbType.String, SqlDbType.VarChar);
         DbTypeList.Add(dbtypeMapEntry);
      }

      #endregion

      #region Methods

      /// <summary>Convert .Net type to Db type</summary>
      /// <param name="type">The type.</param>
      /// <returns></returns>
      public static DbType ToDbType(Type type)
      {
         DbTypeMapEntry entry = Find(type);
         return entry.DbType;
      }

      /// <summary>Convert TSQL data type to DbType</summary>
      /// <param name="sqlDbType">Type of the SQL database.</param>
      /// <returns></returns>
      public static DbType ToDbType(SqlDbType sqlDbType)
      {
         DbTypeMapEntry entry = Find(sqlDbType);
         return entry.DbType;
      }

      /// <summary>Convert db type to .Net data type</summary>
      /// <param name="dbType">Type of the database.</param>
      /// <returns></returns>
      public static Type ToNetType(DbType dbType)
      {
         DbTypeMapEntry entry = Find(dbType);
         return entry.Type;
      }

      /// <summary>Convert TSQL type to .Net data type</summary>
      /// <param name="sqlDbType">Type of the SQL database.</param>
      /// <returns></returns>
      public static Type ToNetType(SqlDbType sqlDbType)
      {
         DbTypeMapEntry entry = Find(sqlDbType);
         return entry.Type;
      }

      /// <summary>Convert .Net type to TSQL data type</summary>
      /// <param name="type">The type.</param>
      /// <returns></returns>
      public static SqlDbType ToSqlDbType(Type type)
      {
         DbTypeMapEntry entry = Find(type);
         return entry.SqlDbType;
      }

      /// <summary>Convert DbType type to TSQL data type</summary>
      /// <param name="dbType">Type of the database.</param>
      /// <returns></returns>
      public static SqlDbType ToSqlDbType(DbType dbType)
      {
         DbTypeMapEntry entry = Find(dbType);
         return entry.SqlDbType;
      }

      private static DbTypeMapEntry Find(Type type)
      {
         object retObj = null;
         foreach (DbTypeMapEntry entry in DbTypeList.Cast<DbTypeMapEntry>().Where(entry => entry.Type == type))
         {
            retObj = entry;
            break;
         }

         if (retObj == null)
         {
            throw new ApplicationException("Referenced an unsupported Type");
         }

         return (DbTypeMapEntry)retObj;
      }

      /// <summary>Finds the specified database type.</summary>
      /// <param name="dbType">Type of the database.</param>
      /// <returns></returns>
      /// <exception cref="System.ApplicationException">Referenced an unsupported DbType</exception>
      private static DbTypeMapEntry Find(DbType dbType)
      {
         object retObj = null;
         foreach (DbTypeMapEntry entry in DbTypeList.Cast<DbTypeMapEntry>().Where(entry => entry.DbType == dbType))
         {
            retObj = entry;
            break;
         }

         if (retObj == null)
         {
            throw new ApplicationException("Referenced an unsupported DbType");
         }

         return (DbTypeMapEntry)retObj;
      }

      private static DbTypeMapEntry Find(SqlDbType sqlDbType)
      {
         object retObj = null;
         foreach (DbTypeMapEntry entry in DbTypeList.Cast<DbTypeMapEntry>().Where(entry => entry.SqlDbType == sqlDbType))
         {
            retObj = entry;
            break;
         }

         if (retObj == null)
         {
            throw new ApplicationException("Referenced an unsupported SqlDbType");
         }

         return (DbTypeMapEntry)retObj;
      }

      #endregion

      private struct DbTypeMapEntry
      {
         public readonly DbType DbType;
         public readonly SqlDbType SqlDbType;
         public readonly Type Type;

         /// <summary>Initializes a new instance of the <see cref="DbTypeMapEntry"/> struct.</summary>
         /// <param name="type">The type.</param>
         /// <param name="dbType">Type of the database.</param>
         /// <param name="sqlDbType">Type of the SQL database.</param>
         public DbTypeMapEntry(Type type, DbType dbType, SqlDbType sqlDbType)
         {
            Type = type;
            DbType = dbType;
            SqlDbType = sqlDbType;
         }
      }
   }
}