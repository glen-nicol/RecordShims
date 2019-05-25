// <copyright file="RecordBase.cs" company="Glen Nicol">
//     Copyright (c) Glen Nicol. All rights reserved. Licensed under the MIT license. See LICENSE
//     file in the project root for full license information.
// </copyright>

namespace RecordShims
{
    /// <summary>
    /// This abstract class provides a starting place for record classes that do not already have a
    /// base class.
    /// </summary>
    /// <typeparam name="T">
    /// The record type. This should be the type of the class inheriting this base class.
    /// </typeparam>
    public abstract class RecordBase<T> : IRecord<T>
        where T : class
    {
        /// <summary>
        /// Gets <see langword="this"/> as <typeparamref name="T"/>.
        /// </summary>
        protected T RecordType { get { return this as T; } }

        /// <summary>
        /// Creates a shallow copy using <see cref="object.MemberwiseClone"/>.
        /// </summary>
        /// <returns>a new instance of a <typeparamref name="T"/> record.</returns>
        T IRecord<T>.ShallowCopy()
        {
            return RecordUtil.MemberwiseClone(RecordType);
        }

        /// <summary>
        /// Throws if any internal constraints between properties has been violated. This is called
        /// after mutations have been applied to the shallow copy.
        /// </summary>
        /// <param name="record"></param>
        public abstract void ThrowIfConstraintsAreViolated(T record);
    }
}