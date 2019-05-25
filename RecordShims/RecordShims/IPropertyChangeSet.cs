// <copyright file="IPropertyChangeSet.cs" company="Glen Nicol">
//     Copyright (c) Glen Nicol. All rights reserved. Licensed under the MIT license. See LICENSE
//     file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RecordShims
{
    /// <summary>
    /// A change set holds execution plans to modify a record and can be applied to a record to
    /// create a new one.
    /// </summary>
    /// <typeparam name="TRecord">The record type.</typeparam>
    public interface IPropertyChangeSet<TRecord>
    {
        /// <summary>
        /// Gets the mutators in the change set.
        /// </summary>
        IReadOnlyCollection<PropertyMutator<TRecord>> Mutators { get; }

        /// <summary>
        /// Adds a mutator to the change set.
        /// </summary>
        /// <param name="mutator"></param>
        /// <returns>A new API to enable a more fluent API.</returns>
        ISubsequentPropertyChangeSetApi<TRecord> Mutate(PropertyMutator<TRecord> mutator);

        /// <summary>
        /// Adds a mutator for the property pointed to by <paramref name="propertyAccessor"/> and
        /// specified <paramref name="mutatorFunc"/> .
        /// </summary>
        /// <typeparam name="TVal">The property type.</typeparam>
        /// <param name="propertyAccessor"></param>
        /// <param name="mutatorFunc"></param>
        /// <returns>A new API to enable a more fluent API.</returns>
        ISubsequentPropertyChangeSetApi<TRecord> Mutate<TVal>(Expression<Func<TRecord, TVal>> propertyAccessor, Func<TRecord, TVal> mutatorFunc);

        /// <summary>
        /// Adds a mutator for the property pointed to by <paramref name="propertyAccessor"/> and
        /// specified <paramref name="newValue"/>.
        /// </summary>
        /// <typeparam name="TVal">The property type.</typeparam>
        /// <param name="propertyAccessor"></param>
        /// <param name="newValue"></param>
        /// <returns>A new API to enable a more fluent API.</returns>
        ISubsequentPropertyChangeSetApi<TRecord> Mutate<TVal>(Expression<Func<TRecord, TVal>> propertyAccessor, TVal newValue);

        /// <summary>
        /// Adds a mutator for the property named <paramref name="propertyName"/> and specified
        /// <paramref name="mutatorFunc"/>.
        /// </summary>
        /// <typeparam name="TVal">The property type.</typeparam>
        /// <param name="propertyName"></param>
        /// <param name="mutatorFunc"></param>
        /// <exception cref="MissingMemberException">
        /// Thrown if <paramref name="propertyName"/> is not a property on <typeparamref name="TRecord"/>.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// Thrown if <paramref name="propertyName"/> cannot be assigned from <typeparamref name="TVal"/>.
        /// </exception>
        /// <returns>A new API to enable a more fluent API.</returns>
        ISubsequentPropertyChangeSetApi<TRecord> Mutate<TVal>(string propertyName, Func<TRecord, TVal> mutatorFunc);

        /// <summary>
        /// Adds a mutator for the property named <paramref name="propertyName"/> and specified
        /// <paramref name="newValue"/>.
        /// </summary>
        /// <typeparam name="TVal">The property type.</typeparam>
        /// <param name="propertyName"></param>
        /// <param name="newValue"></param>
        /// <exception cref="MissingMemberException">
        /// Thrown if <paramref name="propertyName"/> is not a property on <typeparamref name="TRecord"/>.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// Thrown if <paramref name="propertyName"/> cannot be assigned from <typeparamref name="TVal"/>.
        /// </exception>
        /// <returns>A new API to enable a more fluent API.</returns>
        ISubsequentPropertyChangeSetApi<TRecord> Mutate<TVal>(string propertyName, TVal newValue);
    }
}