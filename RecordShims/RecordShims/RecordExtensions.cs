using System;
using System.Collections.Generic;
using System.Text;

namespace RecordShims
{
    public static class RecordExtensions
    {
        public static TRecord With<TRecord>(this IRecord<TRecord> record, Action<PropertyChangeSet<TRecord>> changeSetBuilder)
        {
            var copy = record.ShallowCopy();
            var changeSet = new PropertyChangeSet<TRecord>();
            changeSetBuilder(changeSet);

            foreach (var mutator in changeSet.Mutators)
            {
                // potentially unsafe cast
                mutator.DoMutation((TRecord)record, copy);
            }

            record.ThrowIfConstraintsAreViolated(copy);

            return copy;
        }
    }
}
