// <copyright file="RecordExtensions.cs" company="Glen Nicol">
//     Copyright (c) Glen Nicol. All rights reserved. Licensed under the MIT license. See LICENSE
//     file in the project root for full license information.
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
        /// Creates a shallow copy of <paramref name="record"/> and then applies the mutations
        /// specified in the <see cref="PropertyChangeSet{TRecord}"/> after <paramref
        /// name="changeSetBuilder"/> is called.
        /// </summary>
        /// <example>
        /// <code>
        /// var newRecord = record.With(copy =&gt; copy.Mutate(r =&gt; r.Name, "New Name").AndMutate(r =&gt; r.Counter, r =&gt; r.Counter + 1));
        /// </code>
        /// </example>
        /// <typeparam name="TRecord">The record type.</typeparam>
        /// <param name="record">The original record that is copied.</param>
        /// <param name="changeSetBuilder">
        /// The builder function used to configure the properties that will be mutated.
        /// </param>
        /// <returns>A new instance of the <typeparamref name="TRecord"/> type.</returns>
        public static TRecord With<TRecord>(this IRecord<TRecord> record, Action<IPropertyChangeSet<TRecord>> changeSetBuilder)
        {
            var changeSet = record.StartChangeSet();
            changeSetBuilder(changeSet);

            return changeSet.ToNewRecord(record);
        }

        /// <summary>
        /// Create a new empty change set for a <typeparamref name="TRecord"/> record.
        /// </summary>
        /// <typeparam name="TRecord">The record type.</typeparam>
        /// <param name="record"></param>
        /// <returns>A new change set object.</returns>
        public static IPropertyChangeSet<TRecord> StartChangeSet<TRecord>(this IRecord<TRecord> record)
        {
            return new PropertyChangeSet<TRecord>();
        }

        /// <summary>
        /// Copies <paramref name="record"/> and applies the changes in <paramref name="changeSet"/>.
        /// If there are no changes then <paramref name="record"/> is returned.
        /// </summary>
        /// <typeparam name="TRecord">The record type.</typeparam>
        /// <param name="changeSet"></param>
        /// <param name="record"></param>
        /// <returns>
        /// A copy of <paramref name="record"/> with changes or <paramref name="record"/> if there
        /// are no changes to apply.
        /// </returns>
        public static TRecord ToNewRecord<TRecord>(this IPropertyChangeSet<TRecord> changeSet, IRecord<TRecord> record)
        {
            // potentially unsafe cast?
            var original = (TRecord)record;
            if(changeSet.Mutators.Count > 0)
            {
                var copy = record.ShallowCopy();
                foreach(var mutator in changeSet.Mutators)
                {
                    mutator.ApplyMutation(original, copy);
                }

                record.ThrowIfConstraintsAreViolated(copy);

                return copy;
            }
            else
            {
                return original;
            }
        }

        /// <summary>
        /// If there are changes in <paramref name="changeset"/> a copy is made and the changeset is
        /// applied to the copy.
        /// </summary>
        /// <typeparam name="TRecord">The record type.</typeparam>
        /// <param name="record"></param>
        /// <param name="changeset"></param>
        /// <returns>
        /// A copy of <paramref name="record"/> with changes or <paramref name="record"/> if there
        /// are no changes to apply.
        /// </returns>
        public static TRecord CopyAndApply<TRecord>(this IRecord<TRecord> record, IPropertyChangeSet<TRecord> changeset)
        {
            return changeset.ToNewRecord(record);
        }
    }
}