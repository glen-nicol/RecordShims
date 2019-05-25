// <copyright file="ISubsequentPropertyChangeSetApi.cs" company="Glen Nicol">
// Copyright (c) Glen Nicol. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Linq.Expressions;

namespace RecordShims
{
    /// <summary>
    /// This interface enables a more readable set of functions when chained together.
    /// </summary>
    /// <typeparam name="TRecord">The record type.</typeparam>
    public interface ISubsequentPropertyChangeSetApi<TRecord> : IPropertyChangeSet<TRecord>
    {
        /// <summary>
        /// The same as <see cref="IPropertyChangeSet{TRecord}.Mutate(PropertyMutator{TRecord})"/>.
        /// </summary>
        /// <param name="mutator"></param>
        /// <returns>this API.</returns>
        ISubsequentPropertyChangeSetApi<TRecord> AndMutate(PropertyMutator<TRecord> mutator);

        /// <summary>
        /// The same as <see cref="IPropertyChangeSet{TRecord}.Mutate{TVal}(Expression{Func{TRecord,
        /// TVal}}, Func{TRecord, TVal})"/>.
        /// </summary>
        /// <typeparam name="TVal">The property type.</typeparam>
        /// <param name="propertyAccessor"></param>
        /// <param name="mutatorFunc"></param>
        /// <returns>this API.</returns>
        ISubsequentPropertyChangeSetApi<TRecord> AndMutate<TVal>(Expression<Func<TRecord, TVal>> propertyAccessor, Func<TRecord, TVal> mutatorFunc);

        /// <summary>
        /// The same as <see cref="IPropertyChangeSet{TRecord}.Mutate{TVal}(Expression{Func{TRecord,
        /// TVal}}, TVal)"/>.
        /// </summary>
        /// <typeparam name="TVal">The property type.</typeparam>
        /// <param name="propertyAccessor"></param>
        /// <param name="newValue"></param>
        /// <returns>this API.</returns>
        ISubsequentPropertyChangeSetApi<TRecord> AndMutate<TVal>(Expression<Func<TRecord, TVal>> propertyAccessor, TVal newValue);

        /// <summary>
        /// The same as <see cref="IPropertyChangeSet{TRecord}.Mutate{TVal}(string, Func{TRecord, TVal})"/>.
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
        /// <returns>this API.</returns>
        ISubsequentPropertyChangeSetApi<TRecord> AndMutate<TVal>(string propertyName, Func<TRecord, TVal> mutatorFunc);

        /// <summary>
        /// The same as <see cref="IPropertyChangeSet{TRecord}.Mutate{TVal}(string, TVal)"/>.
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
        /// <returns>this API.</returns>
        ISubsequentPropertyChangeSetApi<TRecord> AndMutate<TVal>(string propertyName, TVal newValue);
    }
}