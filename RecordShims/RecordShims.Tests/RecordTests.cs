// <copyright file="RecordTests.cs" company="Glen Nicol">
// Copyright (c) Glen Nicol. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using NUnit.Framework;
using RecordShims;
using System;

namespace RecordShims.Tests
{
    [TestFixture]
    public class RecordTests
    {
        [Test]
        public void With_copies_and_mutates()
        {
            var tc = new TestingClass();
            Assert.AreEqual(0, tc.AnInteger);

            var tc2 = tc.With(copy => copy.Mutate(t => t.AnInteger, 1));
            Assert.AreEqual(1, tc2.AnInteger);

            Assert.AreEqual(0, tc.AnInteger, "The original was changed.");
        }

        private class TestingClass : RecordBase<TestingClass>, IEquatable<TestingClass>
        {
            public int AnInteger { get; private set; }

            public bool Equals(TestingClass other)
            {
                return ReferenceEquals(this, other)
                    || (other != null && AnInteger == other.AnInteger);
            }

            public override void ThrowIfConstraintsAreViolated(TestingClass record)
            {
                // no validation
            }
        }
    }
}