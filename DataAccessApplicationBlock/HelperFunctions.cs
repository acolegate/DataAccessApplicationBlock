using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DataAccessApplicationBlock.Attributes;

namespace DataAccessApplicationBlock
{
   internal static class HelperFunctions
   {
      internal static IEnumerable<Database.MemberInfo> GetMemberInfo(this Type type)
      {
         return GetMemberInfo(Activator.CreateInstance(type));
      }

      /// <summary>Gets the member information from an instance of a type</summary>
      /// <param name="thisObject">The object.</param>
      /// <returns>A collection of MemberInfo objects. The Value peoperty is set.</returns>
      internal static IEnumerable<Database.MemberInfo> GetMemberInfo(object thisObject)
      {
         Type type = thisObject.GetType();

         List<Database.MemberInfo> memberInfo = new List<Database.MemberInfo>();

         memberInfo.AddRange(type.GetProperties().Select(propertyInfo => new Database.MemberInfo {
                                                                                                    Name = propertyInfo.Name, 
                                                                                                    IsIdentity = propertyInfo.GetCustomAttributes().HasAttribute(typeof(ColumnAttributes.Identity)), 
                                                                                                    IsPrimaryKey = propertyInfo.GetCustomAttributes().HasAttribute(typeof(ColumnAttributes.PrimaryKey)), 
                                                                                                    NotInTable = propertyInfo.GetCustomAttributes().HasAttribute(typeof(ColumnAttributes.NotInTable)), 
                                                                                                    HasColumnDefault = propertyInfo.GetCustomAttributes().HasAttribute(typeof(ColumnAttributes.HasColumnDefault)), 
                                                                                                    Value = propertyInfo.GetValue(thisObject),
                                                                                                    IsNullable = Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null,
                                                                                                    Type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType
                                                                                                 }));

         memberInfo.AddRange(type.GetFields().Select(fieldInfo => new Database.MemberInfo {
                                                                                             Name = fieldInfo.Name, 
                                                                                             IsIdentity = fieldInfo.GetCustomAttributes().HasAttribute(typeof(ColumnAttributes.Identity)), 
                                                                                             IsPrimaryKey = fieldInfo.GetCustomAttributes().HasAttribute(typeof(ColumnAttributes.PrimaryKey)), 
                                                                                             NotInTable = fieldInfo.GetCustomAttributes().HasAttribute(typeof(ColumnAttributes.NotInTable)), 
                                                                                             HasColumnDefault = fieldInfo.GetCustomAttributes().HasAttribute(typeof(ColumnAttributes.HasColumnDefault)),
                                                                                             Value = fieldInfo.GetValue(thisObject),
                                                                                             IsNullable = Nullable.GetUnderlyingType(fieldInfo.FieldType) != null,
                                                                                             Type = Nullable.GetUnderlyingType(fieldInfo.FieldType) ?? fieldInfo.FieldType
                                                                                          }));

         return memberInfo;
      }
   }
}
