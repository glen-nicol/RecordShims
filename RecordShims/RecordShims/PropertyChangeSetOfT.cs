using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace RecordShims
{
    public class PropertyChangeSet<TRecord>
    {
        private readonly Dictionary<string, PropertyMutator<TRecord>> _mutationSet = new Dictionary<string, PropertyMutator<TRecord>>();

        public IEnumerable<PropertyMutator<TRecord>> Mutators => _mutationSet.Values;

        public SubsequentPropertyChangeSetApi<TRecord> Mutate<TVal>(Expression<Func<TRecord,TVal>> propertyAccessor, TVal newValue)
        {
            var propertyInfo = PropertyMutator<TRecord>.GetPropertyFromExpression(propertyAccessor);
            var mutator = PropertyMutator<TRecord>.FromExternalValue(propertyInfo, newValue);
            return Mutate(mutator);
        }

        public SubsequentPropertyChangeSetApi<TRecord> Mutate<TVal>(Expression<Func<TRecord, TVal>> propertyAccessor, Func<TRecord,TVal> mutatorFunc)
        {
            var propertyInfo = PropertyMutator<TRecord>.GetPropertyFromExpression(propertyAccessor);
            var mutator = PropertyMutator<TRecord>.FromMutation(propertyInfo, r => mutatorFunc(r));
            return Mutate(mutator);
        }

        public SubsequentPropertyChangeSetApi<TRecord> Mutate(PropertyMutator<TRecord> mutator)
        {
            _mutationSet[mutator.PropertyName] = mutator;
            return new SubsequentPropertyChangeSetApi<TRecord>(this);
        }
    }

    public class SubsequentPropertyChangeSetApi<TRecord> : PropertyChangeSet<TRecord>
    {
        private readonly PropertyChangeSet<TRecord> _changeSet;

        public SubsequentPropertyChangeSetApi(PropertyChangeSet<TRecord> changeSet)
        {
            _changeSet = changeSet;
        }

        public SubsequentPropertyChangeSetApi<TRecord> AndMutate<TVal>(Expression<Func<TRecord, TVal>> propertyAccessor, TVal newValue)
        {
            _changeSet.Mutate(propertyAccessor, newValue);
            return this;
        }

        public SubsequentPropertyChangeSetApi<TRecord> AndMutate<TVal>(Expression<Func<TRecord, TVal>> propertyAccessor, Func<TRecord, TVal> mutatorFunc)
        {
            _changeSet.Mutate(propertyAccessor, mutatorFunc);
            return this;
        }

        public SubsequentPropertyChangeSetApi<TRecord> AndMutate(PropertyMutator<TRecord> mutator)
        {
            _changeSet.Mutate(mutator);
            return this;
        }
    }

}
