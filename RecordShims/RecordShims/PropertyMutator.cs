// <copyright file="PropertyMutator.cs" company="Glen Nicol">
//     Copyright (c) Glen Nicol. All rights reserved. Licensed under the MIT license. See LICENSE
//     file in the project root for full license information.
// </copyright>

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace RecordShims
{
    /// <summary>
    /// An encapsulated property mutation that can be applied to a record at runtime.
    /// </summary>
    /// <typeparam name="TRecord">The record type.</typeparam>
    public class PropertyMutator<TRecord>
    {
        /// <summary>
        /// A transform from the original record value to a new value for the new record.
        /// </summary>
        /// <param name="originalRecord">The record that is being copied.</param>
        /// <returns>the value for the new record.</returns>
        public delegate object Mutator(TRecord originalRecord);

        private readonly PropertyInfo _propertyInfo;
        private readonly FieldInfo _readonlyAutoPropertyField;
        private readonly Mutator _mutator;

        /// <summary>
        /// Gets the property that this mutation will affect.
        /// </summary>
        public PropertyInfo Property => _propertyInfo;

        /// <summary>
        /// Gets the name of the property as it would appear in source. Ie. PropertyName not set_PropertyName.
        /// </summary>
        public string PropertyName => Property.Name;

        private bool IsReadonlyAutoProperty => _readonlyAutoPropertyField != null;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMutator{TRecord}"/> class.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="mutator"></param>
        private PropertyMutator(PropertyInfo property, Mutator mutator)
        {
            _propertyInfo = property ?? throw new ArgumentNullException(nameof(property));
            _mutator = mutator ?? throw new ArgumentNullException(nameof(mutator));

            if(!_propertyInfo.CanWrite)
            {
                var backingFieldName = GetBackingFieldName(PropertyName);
                _readonlyAutoPropertyField = typeof(TRecord).GetField(backingFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                if(!IsReadonlyAutoProperty)
                {
                    throw new ArgumentException("The " + PropertyName + " property does not have a setter and is not a readonly auto property.");
                }
            }

            string GetBackingFieldName(string propertyName)
            {
                // https://stackoverflow.com/a/14210097
                return string.Format("<{0}>k__BackingField", propertyName);
            }
        }

        /// <summary>
        /// Creates a mutator from a provided value. The result is similar to an assignment of a constant.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="newValue"></param>
        /// <returns>
        /// The property mutator that will apply the value to any record it is applied to.
        /// </returns>
        public static PropertyMutator<TRecord> FromAssignment(PropertyInfo property, object newValue) => new PropertyMutator<TRecord>(property, _ => newValue);

        /// <summary>
        /// Creates a mutator from a provided value. The result is similar to an assignment of a constant.
        /// </summary>
        /// <typeparam name="TVal">The transform type. It must be assignable to the property.</typeparam>
        /// <param name="propertyName"></param>
        /// <param name="newValue"></param>
        /// <exception cref="MissingMemberException">
        /// Thrown if <paramref name="propertyName"/> does not exist on <typeparamref name="TRecord"/>.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// Thrown if <paramref name="propertyName"/> cannot be assigned from <typeparamref name="TVal"/>.
        /// </exception>
        /// <returns>
        /// The property mutator that will apply the value to any record it is applied to.
        /// </returns>
        public static PropertyMutator<TRecord> FromAssignment<TVal>(string propertyName, TVal newValue)
            => FromAssignment(GetByNameAndThrowIfMissingOrUncastable<TVal>(propertyName), newValue);

        /// <summary>
        /// Creates a mutator from a provided transform function. The result depends on how the
        /// function transforms from the previous record value.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="mutator"></param>
        /// <returns>The property mutator that will apply the transform to get the result.</returns>
        public static PropertyMutator<TRecord> FromTransform(PropertyInfo property, Mutator mutator) => new PropertyMutator<TRecord>(property, mutator);

        /// <summary>
        /// Creates a mutator from a provided transform function. The result depends on how the
        /// function transforms from the previous record value.
        /// </summary>
        /// <typeparam name="TVal">The transform type. It must be assignable to the property.</typeparam>
        /// <param name="propertyName"></param>
        /// <param name="mutator"></param>
        /// <exception cref="MissingMemberException">
        /// Thrown if <paramref name="propertyName"/> does not exist on <typeparamref name="TRecord"/>.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// Thrown if <paramref name="propertyName"/> cannot be assigned from <typeparamref name="TVal"/>.
        /// </exception>
        /// <returns>The property mutator that will apply the transform to get the result.</returns>
        public static PropertyMutator<TRecord> FromTransform<TVal>(string propertyName, Func<TRecord, TVal> mutator)
            => FromTransform(GetByNameAndThrowIfMissingOrUncastable<TVal>(propertyName), r => mutator(r));

        /// <summary>
        /// Gets the property info for an expression that looks like this x =&gt; x.PropertyName.
        /// </summary>
        /// <remarks>Taken from "https://stackoverflow.com/a/672212".</remarks>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="propertyLambda"></param>
        /// <exception cref="ArgumentException">Expression is malformed.</exception>
        /// <returns>The property info. never null.</returns>
        public static PropertyInfo GetPropertyFromExpression<TProperty>(
            Expression<Func<TRecord, TProperty>> propertyLambda)
        {
            Type type = typeof(TRecord);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if(member == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));
            }

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if(propInfo == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));
            }

            if(type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));
            }

            return propInfo;
        }

        /// <summary>
        /// Applies the mutation to the <paramref name="newRecord"/> by using <paramref
        /// name="originalRecord"/> to do any transforms.
        /// </summary>
        /// <param name="originalRecord"></param>
        /// <param name="newRecord"></param>
        public void ApplyMutation(TRecord originalRecord, TRecord newRecord)
        {
            var newValue = _mutator(originalRecord);
            if(IsReadonlyAutoProperty)
            {
                // This is a bit scary since these are readonly fields. But we don't have much choice
                // if we want to support modifying readonly auto properties. We are already violating
                // the private accessibility so why not keep going for great justice!
                _readonlyAutoPropertyField.SetValue(newRecord, newValue);
            }
            else
            {
                _propertyInfo.SetValue(newRecord, newValue);
            }
        }

        private static PropertyInfo GetByNameAndThrowIfMissingOrUncastable<TVal>(string propertyName)
        {
            var propertyInfo = typeof(TRecord).GetProperty(propertyName);
            if(propertyInfo == null)
            {
                throw new MissingMemberException(typeof(TRecord).FullName, propertyName);
            }

            // throwing here will make it more obvious to the caller which mutation is causing the
            // problem rather than waiting for the mutation to be applied.
            if(!propertyInfo.PropertyType.IsAssignableFrom(typeof(TVal)))
            {
                throw new InvalidCastException("Cannot cast " + typeof(TVal) + " to " + propertyInfo.PropertyType);
            }

            return propertyInfo;
        }
    }
}