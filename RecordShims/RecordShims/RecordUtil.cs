using System;
using System.Collections.Generic;
using System.Text;

namespace RecordShims
{
    public static class RecordUtil
    {
        private const string MEMBERWISE_CLONE_FUNCTION_NAME = "MemberwiseClone";
        private static readonly Lazy<Func<object,object>> _memberWiseClone = new Lazy<Func<object, object>>(GetProtectedMemberWiseClone);

        private static Func<object, object> GetProtectedMemberWiseClone()
        {
            var objectType = typeof(object);
            var memberWiseCloneMethod = objectType.GetMethod(MEMBERWISE_CLONE_FUNCTION_NAME, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if(memberWiseCloneMethod == null)
            {
                throw new NotSupportedException(MEMBERWISE_CLONE_FUNCTION_NAME + " could not be found.");
            }

            return (Func<object, object>)memberWiseCloneMethod.CreateDelegate(typeof(Func<object, object>));
        }

        /// <summary>
        /// This exposes the protected <see cref="object.MemberwiseClone"/> method to be used by any class on any object. Use with caution.
        /// </summary>
        /// <typeparam name="T">the type of the clone to create.</typeparam>
        /// <param name="toCopy"></param>
        /// <returns>A new shallow copy of <paramref name="toCopy"/></returns>
        public static T MemberwiseClone<T>(T toCopy)
        {
            return (T)_memberWiseClone.Value(toCopy);
        }
    }
}
