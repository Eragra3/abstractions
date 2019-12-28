﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity
{
    internal partial class UnityDefaults
    {
        #region Supported declared members

        /// <summary>
        /// Method to query <see cref="Type"/> for supported constructors
        /// </summary>
        public static Func<Type, IEnumerable<ConstructorInfo>> SupportedConstructors { get; set; } = (Type type) =>
             type.GetConstructors();


        /// <summary>
        /// Method to query <see cref="Type"/> for supported fields
        /// </summary>
        public static Func<Type, IEnumerable<FieldInfo>> SupportedFields { get; set; } = (Type type) =>
        {
            return type.GetFields()
                       .Where(member => !member.IsFamily &&
                                        !member.IsPrivate &&
                                        !member.IsInitOnly &&
                                        !member.IsStatic);
        };


        /// <summary>
        /// Method to query <see cref="Type"/> for supported properties
        /// </summary>
        public static Func<Type, IEnumerable<PropertyInfo>> SupportedProperties { get; set; } = (Type type) =>
        {
            return type.GetProperties()
                       .Where(member =>
                       {
                           if (!member.CanWrite || 0 != member.GetIndexParameters().Length)
                               return false;

                           var setter = member.GetSetMethod(true);
                           if (null == setter || setter.IsPrivate || setter.IsFamily)
                               return false;

                           return true;
                       });

        };


        /// <summary>
        /// Method to query <see cref="Type"/> for supported methods
        /// </summary>
        public static Func<Type, IEnumerable<MethodInfo>> SupportedMethods { get; set; } = (Type type) =>
        {
            return type.GetMethods()
                       .Where(member => !member.IsFamily && !member.IsPrivate && !member.IsStatic);

        };

        #endregion
    }
}
