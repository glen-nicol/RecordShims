﻿// <copyright file="PropertyChangeSet.cs" company="Glen Nicol">
// Copyright (c) Glen Nicol. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RecordShims
{
    /// <summary>
    /// A set of mutations that can be applied to a record.
    /// </summary>
    /// <typeparam name="TRecord">The type of the record.</typeparam>
    public class PropertyChangeSet<TRecord>
    {
        private readonly Dictionary<string, PropertyMutator<TRecord>> _mutationSet = new Dictionary<string, PropertyMutator<TRecord>>();

        /// <summary>
        /// Gets The mutations that will be applied to a record.
        /// </summary>
        public IEnumerable<PropertyMutator<TRecord>> Mutators => _mutationSet.Values;

        /// <summary>
        /// Adds a mutator for the property pointed to by <paramref name="propertyAccessor"/> and specified <paramref name="newValue"/>.
        /// </summary>
        /// <typeparam name="TVal">The property type.</typeparam>
        /// <param name="propertyAccessor"></param>
        /// <param name="newValue"></param>
        /// <returns>A new API to enable a more fluent API. If this is called from a <see cref="SubsequentPropertyChangeSetApi{TRecord}"/> then the same wrapper is returned.</returns>
        public SubsequentPropertyChangeSetApi<TRecord> Mutate<TVal>(Expression<Func<TRecord, TVal>> propertyAccessor, TVal newValue)
        {
            var propertyInfo = PropertyMutator<TRecord>.GetPropertyFromExpression(propertyAccessor);
            var mutator = PropertyMutator<TRecord>.FromAssignment(propertyInfo, newValue);
            return Mutate(mutator);
        }

        /// <summary>
        /// Adds a mutator for the property pointed to by <paramref name="propertyAccessor"/> and specified <paramref name="mutatorFunc"/> .
        /// </summary>
        /// <typeparam name="TVal">The property type.</typeparam>
        /// <param name="propertyAccessor"></param>
        /// <param name="mutatorFunc"></param>
        /// <returns>A new API to enable a more fluent API. If this is called from a <see cref="SubsequentPropertyChangeSetApi{TRecord}"/> then the same wrapper is returned.</returns>
        public SubsequentPropertyChangeSetApi<TRecord> Mutate<TVal>(Expression<Func<TRecord, TVal>> propertyAccessor, Func<TRecord, TVal> mutatorFunc)
        {
            var propertyInfo = PropertyMutator<TRecord>.GetPropertyFromExpression(propertyAccessor);
            var mutator = PropertyMutator<TRecord>.FromTransform(propertyInfo, r => mutatorFunc(r));
            return Mutate(mutator);
        }

        /// <summary>
        /// Adds a mutator to the change set.
        /// </summary>
        /// <param name="mutator"></param>
        /// <returns>A new API to enable a more fluent API. If this is called from a <see cref="SubsequentPropertyChangeSetApi{TRecord}"/> then the same wrapper is returned.</returns>
        public SubsequentPropertyChangeSetApi<TRecord> Mutate(PropertyMutator<TRecord> mutator)
        {
            _mutationSet[mutator.PropertyName] = mutator;
            if(this is SubsequentPropertyChangeSetApi<TRecord>)
            {
                return this as SubsequentPropertyChangeSetApi<TRecord>;
            }
            else
            {
                return new SubsequentPropertyChangeSetApi<TRecord>(this);
            }
        }
    }
}