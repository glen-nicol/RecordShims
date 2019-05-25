// <copyright file="IRecord.cs" company="Glen Nicol">
//     Copyright (c) Glen Nicol. All rights reserved. Licensed under the MIT license. See LICENSE
//     file in the project root for full license information.
// </copyright>

namespace RecordShims
{
    /// <summary>
    /// The record interface provides a way to create a copy for the new record and a way to validate
    /// that new record after it has been modified. It is recommended to implement the interface
    /// explicitely to avoid polluting the namespace.
    /// </summary>
    /// <example>
    /// <code>
    /// class RecordType : IRecord&lt;RecordType&gt;
    /// {
    ///
    /// }
    /// </code>
    /// </example>
    /// <typeparam name="T">The record type.</typeparam>
    public interface IRecord<T>
    {
        /// <summary>
        /// Creates a new instance of the object. If this returns the same object then an exception
        /// will be thrown.
        /// </summary>
        /// <returns>A new non null instance of the record.</returns>
        T ShallowCopy();

        /// <summary>
        /// Throws an exception if business logic constraints were violated. It is up to the
        /// implementor to decide which exception to throw.
        /// <para>
        /// This method is called after a record is copied and modified to ensure that any intra
        /// record constraints are not violated by the mutation.
        /// </para>
        /// </summary>
        /// <param name="record">The record to validate.</param>
        void ThrowIfConstraintsAreViolated(T record);
    }
}