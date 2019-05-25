// <copyright file="RecordUtil.cs" company="Glen Nicol">
// Copyright (c) Glen Nicol. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RecordShims
{
    /// <summary>
    /// Common functions that <see cref="IRecord{T}"/> may need.
    /// </summary>
    public static class RecordUtil
    {
        private const string MEMBERWISE_CLONE_FUNCTION_NAME = "MemberwiseClone";
        private static readonly Lazy<Func<object, object>> MEMBER_WISE_CLONE = new Lazy<Func<object, object>>(GetProtectedMemberWiseClone);

        /// <summary>
        /// This exposes the protected <see cref="object.MemberwiseClone"/> method to be used by any class on any object. Use with caution.
        /// </summary>
        /// <typeparam name="T">the type of the clone to create.</typeparam>
        /// <param name="toCopy"></param>
        /// <returns>A new shallow copy of <paramref name="toCopy"/>.</returns>
        public static T MemberwiseClone<T>(T toCopy)
        {
            return (T)MEMBER_WISE_CLONE.Value(toCopy);
        }

        private static Func<object, object> GetProtectedMemberWiseClone()
        {
            var objectType = typeof(object);
            var memberWiseCloneMethod = objectType.GetMethod(MEMBERWISE_CLONE_FUNCTION_NAME, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if(memberWiseCloneMethod == null)
            {
                throw new NotSupportedException("object." + MEMBERWISE_CLONE_FUNCTION_NAME + " could not be found.");
            }

            return (Func<object, object>)memberWiseCloneMethod.CreateDelegate(typeof(Func<object, object>));
        }
    }
}