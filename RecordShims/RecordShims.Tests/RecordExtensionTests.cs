// <copyright file="RecordExtensionTests.cs" company="Glen Nicol">
//     Copyright (c) Glen Nicol. All rights reserved. Licensed under the MIT license. See LICENSE
//     file in the project root for full license information.
// </copyright>

using NUnit.Framework;
using System;

namespace RecordShims.Tests
{
    [TestFixture]
    public class RecordExtensionTests
    {
        [Test]
        public void With_copies_and_mutates()
        {
            var original = new TestingClass();
            Assert.AreEqual(0, original.AnInteger);

            var newRecord = original.With(copy => copy.Mutate(t => t.AnInteger, 1).AndMutate(t => t.ADouble, 1.0));

            Assert.AreEqual(1, newRecord.AnInteger);
            Assert.AreEqual(1.0, newRecord.ADouble);
            Assert.AreEqual(new TestingClass(), original, "The original was changed.");
        }

        [Test]
        public void Records_can_start_a_new_changeset_and_apply_them()
        {
            var original = new TestingClass();

            var newRecord = original.StartChangeSet()
                .Mutate(t => t.AnInteger, 2)
                .AndMutate(t => t.ADouble, 1.0)
                .ToNewRecord(original);

            Assert.AreEqual(2, newRecord.AnInteger);
            Assert.AreEqual(1.0, newRecord.ADouble);
            Assert.AreEqual(new TestingClass(), original, "The original was changed.");
        }

        [Test]
        public void Changesets_can_specify_transforms()
        {
            var original = new TestingClass();

            var changeSet = original.StartChangeSet()
                .Mutate(t => t.AnInteger, t => t.AnInteger + 1)
                .AndMutate(t => t.ADouble, t => t.ADouble + 1);

            var record2 = original.CopyAndApply(changeSet);

            Assert.AreEqual(1, record2.AnInteger);
            Assert.AreEqual(1, record2.ADouble);

            var record3 = record2.CopyAndApply(changeSet);

            Assert.AreEqual(2, record3.AnInteger);
            Assert.AreEqual(2, record3.ADouble);
        }

        [Test]
        public void Changesets_can_be_applied_multiple_times()
        {
            var original = new TestingClass();

            // can store a changeset
            var changeSet = original.StartChangeSet()
                .Mutate(t => t.AnInteger, 1);

            // applied once
            var record2 = changeSet.ToNewRecord(original);

            Assert.AreEqual(1, record2.AnInteger);
            Assert.AreEqual(new TestingClass(), original, "The original was changed.");

            var record3 = original.With(copy => copy.Mutate(t => t.AnInteger, 5).AndMutate(t => t.ADouble, 1.0));

            // reapply the stored changeset to a different record
            var record4 = changeSet.ToNewRecord(record3);
            Assert.AreEqual(1, record4.AnInteger);
            Assert.AreEqual(1.0, record4.ADouble, nameof(original.ADouble) + " was not part of th change set and should not have changed.");

            Assert.AreEqual(5, record3.AnInteger, "The original was changed.");
            Assert.AreEqual(1.0, record3.ADouble, "The original was changed.");
        }

        [Test]
        public void Duplicate_property_mutations_replace_earlier_ones()
        {
            var original = new TestingClass();

            var record2 = original.StartChangeSet()
                .Mutate(t => t.AnInteger, 1)
                .AndMutate(t => t.AnInteger, 2)
                .ToNewRecord(original);

            Assert.AreEqual(2, record2.AnInteger);
        }

        [Test]
        public void Constraints_are_validated_on_apply()
        {
            var original = new TestingClass();

            var changeset = original.StartChangeSet()
                .Mutate(t => t.RecordedAt, DateTimeOffset.Now);

            Assert.Throws<ArgumentException>(() => changeset.ToNewRecord(original));
            Assert.Throws<ArgumentException>(() => original.CopyAndApply(changeset));
        }

        [Test]
        public void Can_use_string_but_type_safety_is_not_static()
        {
            var original = new TestingClass();

            // This method should be a bit more performant becasue it avoids expression walking, but
            // it sacrifices type safety.
            var record2 = original.StartChangeSet()
                .Mutate(nameof(original.AnInteger), 1)
                .AndMutate(nameof(original.ADouble), 1.0)
                .ToNewRecord(original);

            Assert.AreEqual(1, record2.AnInteger);
            Assert.AreEqual(1, record2.ADouble);

            // pitfalls type mismatch
            Assert.Throws<InvalidCastException>(() => original.StartChangeSet().Mutate(nameof(original.RecordedAt), 1));

            // missing property
            Assert.Throws<MissingMemberException>(() => original.StartChangeSet().Mutate("NonExistant", 1));
        }

        [Test]
        public void Can_mutate_readonly_auto_properties()
        {
            var original = new TestingClass();

            var record2 = original.StartChangeSet()
                .Mutate(t => t.ReadonlyAutoBool, true)
                .ToNewRecord(original);

            Assert.IsTrue(record2.ReadonlyAutoBool);
            Assert.IsFalse(original.ReadonlyAutoBool);
        }

        private class TestingClass : RecordBase<TestingClass>, IEquatable<TestingClass>
        {
            public int AnInteger { get; private set; }

            public double ADouble { get; private set; }

            public DateTimeOffset RecordedAt { get; private set; }

            public bool ReadonlyAutoBool { get; }

            public bool Equals(TestingClass other)
            {
                return ReferenceEquals(this, other)
                    || (other != null
                    && AnInteger == other.AnInteger
                    && ADouble == other.ADouble // not worried about double precision when copying values.
                    && RecordedAt == other.RecordedAt);
            }

            public override void ThrowIfConstraintsAreViolated(TestingClass record)
            {
                // if changed at all
                if(record.RecordedAt > DateTimeOffset.MinValue)
                {
                    throw new ArgumentException("Constraint violation example.");
                }
            }
        }
    }
}