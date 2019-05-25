// <copyright file="PropertyMutatorTests.cs" company="Glen Nicol">
//     Copyright (c) Glen Nicol. All rights reserved. Licensed under the MIT license. See LICENSE
//     file in the project root for full license information.
// </copyright>

using NUnit.Framework;
using System;

namespace RecordShims.Tests
{
    [TestFixture]
    internal class PropertyMutatorTests
    {
        [Test]
        public void Proparty_name_is_the_same_as_in_code()
        {
            var tc = new TestClass();
            var propertyInfo = PropertyMutator<TestClass>.GetPropertyFromExpression(t => t.TestProp);
            var mutator = PropertyMutator<TestClass>.FromAssignment(propertyInfo, 0);
            Assert.AreEqual(nameof(TestClass.TestProp), mutator.PropertyName);
        }

        [Test]
        public void Property_is_exposed()
        {
            var tc = new TestClass();
            var propertyInfo = PropertyMutator<TestClass>.GetPropertyFromExpression(t => t.TestProp);
            var mutator = PropertyMutator<TestClass>.FromAssignment(propertyInfo, 0);
            Assert.AreSame(propertyInfo, mutator.Property);
        }

        [Test]
        public void Throws_if_property_does_not_have_setter()
        {
            var notSetterProperyInfo = PropertyMutator<TestClass>.GetPropertyFromExpression(t => t.NoSetter);
            Assert.Throws<ArgumentException>(() => PropertyMutator<TestClass>.FromAssignment(notSetterProperyInfo, DateTimeOffset.MaxValue));
            Assert.Throws<ArgumentException>(() => PropertyMutator<TestClass>.FromTransform(notSetterProperyInfo, r => DateTimeOffset.MaxValue));
            Assert.Throws<ArgumentException>(() => PropertyMutator<TestClass>.FromAssignment(notSetterProperyInfo.Name, DateTimeOffset.MaxValue));
            Assert.Throws<ArgumentException>(() => PropertyMutator<TestClass>.FromTransform(notSetterProperyInfo.Name, r => DateTimeOffset.MaxValue));
        }

        [Test]
        public void Can_modify_readonly_auto_properties()
        {
            var original = new TestClass();
            var newRecord = new TestClass();
            var autoPropertyInfo = PropertyMutator<TestClass>.GetPropertyFromExpression(t => t.ReadonlyAutoObject);

            newRecord = new TestClass();
            Assert.IsNull(newRecord.ReadonlyAutoObject);
            Assert.DoesNotThrow(() => PropertyMutator<TestClass>.FromAssignment(autoPropertyInfo, new object()).ApplyMutation(original, newRecord));
            Assert.IsNotNull(newRecord.ReadonlyAutoObject);

            newRecord = new TestClass();
            Assert.IsNull(newRecord.ReadonlyAutoObject);
            Assert.DoesNotThrow(() => PropertyMutator<TestClass>.FromTransform(autoPropertyInfo, r => new object()).ApplyMutation(original, newRecord));
            Assert.IsNotNull(newRecord.ReadonlyAutoObject);

            newRecord = new TestClass();
            Assert.IsNull(newRecord.ReadonlyAutoObject);
            Assert.DoesNotThrow(() => PropertyMutator<TestClass>.FromAssignment(autoPropertyInfo.Name, new object()).ApplyMutation(original, newRecord));
            Assert.IsNotNull(newRecord.ReadonlyAutoObject);

            newRecord = new TestClass();
            Assert.IsNull(newRecord.ReadonlyAutoObject);
            Assert.DoesNotThrow(() => PropertyMutator<TestClass>.FromTransform(autoPropertyInfo.Name, r => new object()).ApplyMutation(original, newRecord));
            Assert.IsNotNull(newRecord.ReadonlyAutoObject);
        }

        private class TestClass
        {
            public int TestProp { get; private set; }

            public DateTimeOffset NoSetter { get { return DateTimeOffset.Now; } }

            public object ReadonlyAutoObject { get; }
        }
    }
}