// <copyright file="SubsequentPropertyChangeSetApi.cs" company="Glen Nicol">
// Copyright (c) Glen Nicol. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Linq.Expressions;

namespace RecordShims
{
    /// <summary>
    /// This class is a simple wrapper around <see cref="PropertyChangeSet{TRecord}"/> to enable a more readable set of functions when chained together.
    /// </summary>
    /// <typeparam name="TRecord">The record type.</typeparam>
    public class SubsequentPropertyChangeSetApi<TRecord> : PropertyChangeSet<TRecord>
    {
        private readonly PropertyChangeSet<TRecord> _changeSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubsequentPropertyChangeSetApi{TRecord}"/> class.
        /// </summary>
        /// <param name="changeSet"></param>
        public SubsequentPropertyChangeSetApi(PropertyChangeSet<TRecord> changeSet)
        {
            _changeSet = changeSet ?? throw new ArgumentNullException(nameof(changeSet));
        }

        /// <summary>
        /// The same as <see cref="PropertyChangeSet{TRecord}.Mutate{TVal}(Expression{Func{TRecord, TVal}}, TVal)"/>.
        /// </summary>
        /// <typeparam name="TVal">The property type.</typeparam>
        /// <param name="propertyAccessor"></param>
        /// <param name="newValue"></param>
        /// <returns>this API.</returns>
        public SubsequentPropertyChangeSetApi<TRecord> AndMutate<TVal>(Expression<Func<TRecord, TVal>> propertyAccessor, TVal newValue)
        {
            _changeSet.Mutate(propertyAccessor, newValue);
            return this;
        }

        /// <summary>
        /// The same as <see cref="PropertyChangeSet{TRecord}.Mutate{TVal}(Expression{Func{TRecord, TVal}}, Func{TRecord, TVal})"/>.
        /// </summary>
        /// <typeparam name="TVal">The property type.</typeparam>
        /// <param name="propertyAccessor"></param>
        /// <param name="mutatorFunc"></param>
        /// <returns>this API.</returns>
        public SubsequentPropertyChangeSetApi<TRecord> AndMutate<TVal>(Expression<Func<TRecord, TVal>> propertyAccessor, Func<TRecord, TVal> mutatorFunc)
        {
            _changeSet.Mutate(propertyAccessor, mutatorFunc);
            return this;
        }

        /// <summary>
        /// The same as <see cref="PropertyChangeSet{TRecord}.Mutate(PropertyMutator{TRecord})"/>.
        /// </summary>
        /// <param name="mutator"></param>
        /// <returns>this API.</returns>
        public SubsequentPropertyChangeSetApi<TRecord> AndMutate(PropertyMutator<TRecord> mutator)
        {
            _changeSet.Mutate(mutator);
            return this;
        }
    }
}