// <copyright file="PropertyMutatorTests.cs" company="Glen Nicol">
// Copyright (c) Glen Nicol. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

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
        public void Proparty_is_exposed()
        {
            var tc = new TestClass();
            var propertyInfo = PropertyMutator<TestClass>.GetPropertyFromExpression(t => t.TestProp);
            var mutator = PropertyMutator<TestClass>.FromAssignment(propertyInfo, 0);
            Assert.AreSame(propertyInfo, mutator.Property);
        }

        private class TestClass
        {
            public int TestProp { get; private set; }
        }
    }
}