using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace RecordShims
{
    public class PropertyMutator<TRecord>
    {
        public delegate object Mutator(TRecord originalRecord);

        private readonly PropertyInfo _propertyInfo;
        private readonly Mutator _mutator;

        public PropertyInfo Property => _propertyInfo;

        public string PropertyName => Property.Name;

        private PropertyMutator(PropertyInfo property, Mutator mutator)
        {
            _propertyInfo = property ?? throw new ArgumentNullException(nameof(property));
            _mutator = mutator ?? throw new ArgumentNullException(nameof(mutator));
        }

        public static PropertyMutator<TRecord> FromExternalValue(PropertyInfo property, object newValue) => new PropertyMutator<TRecord>(property, _ => newValue);
        public static PropertyMutator<TRecord> FromMutation(PropertyInfo property, Mutator mutator) => new PropertyMutator<TRecord>(property, mutator);

        public static PropertyInfo GetPropertyFromExpression<TVal>(Expression<Func<TRecord,TVal>> expression)
        {
            return GetPropertyInfo(expression);
        }

        public void DoMutation(TRecord originalRecord, TRecord newRecord)
        {
            _propertyInfo.SetValue(newRecord, _mutator(originalRecord));
        }

        /// <summary>
        /// Taken from https://stackoverflow.com/a/672212
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="propertyLambda"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(
            Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }
    }
}
