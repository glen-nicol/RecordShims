// <copyright file="RecordExtensions.cs" company="Glen Nicol">
// Copyright (c) Glen Nicol. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RecordShims
{
    /// <summary>
    /// This extension class provides the main function of the entire library.
    /// </summary>
    public static class RecordExtensions
    {
        /// <summary>
        /// Creates a shallow copy of <paramref name="record"/> and then applies the mutations specified in the <see cref="PropertyChangeSet{TRecord}"/> after <paramref name="changeSetBuilder"/> is called.
        /// </summary>
        /// <example>
        /// <code>
        /// var newRecord = record.With(copy => copy.Mutate(r => r.Name, "New Name").AndMutate(r => r.Counter, r => r.Counter + 1));
        /// </code></example>
        /// <typeparam name="TRecord">The record type.</typeparam>
        /// <param name="record">The original record that is copied.</param>
        /// <param name="changeSetBuilder">The builder function used to configure the properties that will be mutated.</param>
        /// <returns>A new instance of the <typeparamref name="TRecord"/> type.</returns>
        public static TRecord With<TRecord>(this IRecord<TRecord> record, Action<PropertyChangeSet<TRecord>> changeSetBuilder)
        {
            var copy = record.ShallowCopy();
            var changeSet = new PropertyChangeSet<TRecord>();
            changeSetBuilder(changeSet);

            foreach(var mutator in changeSet.Mutators)
            {
                // potentially unsafe cast?
                mutator.ApplyMutation((TRecord)record, copy);
            }

            record.ThrowIfConstraintsAreViolated(copy);

            return copy;
        }
    }
}