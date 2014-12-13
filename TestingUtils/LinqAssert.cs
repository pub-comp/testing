using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PubComp.Testing.TestingUtils
{
    public static class LinqAssert
    {
        public static void AreSame<TEntity>(IEnumerable<TEntity> expected, IEnumerable<TEntity> actual)
        {
            if (expected != null)
                Assert.IsNotNull(actual);

            if (expected == null)
                Assert.IsNull(actual);

            Assert.AreEqual(expected.Count(), actual.Count());

            var enumer1 = expected.GetEnumerator();
            var enumer2 = actual.GetEnumerator();

            while (enumer1.MoveNext() && enumer2.MoveNext())
            {
                Assert.AreEqual(enumer1.Current, enumer2.Current);
            }
        }

        public static void Any<TEntity>(IEnumerable<TEntity> collection, Func<TEntity, bool> predicate, String conditionDescription = null)
        {
            Assert.IsTrue(
                collection.Any(predicate),
                "Collection does not contain a matching item" + (string.IsNullOrEmpty(conditionDescription) ? string.Empty : ": " + conditionDescription));
        }

        public static void All<TEntity>(IEnumerable<TEntity> collection, Func<TEntity, bool> predicate, String conditionDescription = null)
        {
            Assert.IsTrue(
                collection.All(predicate),
                "Collection contains a non-matching item" + (string.IsNullOrEmpty(conditionDescription) ? string.Empty : ": " + conditionDescription));
        }

        public static void Single<TEntity>(IEnumerable<TEntity> collection, Func<TEntity, bool> predicate, String conditionDescription = null)
        {
            var predicateCount = collection.Count(predicate);

            Assert.AreEqual(1, predicateCount,
                "Collection does not contain a single item (found " + predicateCount + ")"
                    + (string.IsNullOrEmpty(conditionDescription) ? string.Empty : ": " + conditionDescription));
        }

        public static void Count<TEntity>(IEnumerable<TEntity> collection, int expectedCount)
        {
            var actualCount = collection.Count();
            Assert.AreEqual(expectedCount, actualCount,
                "Collection expectedCount expected: " + expectedCount + ", actual: " + actualCount);
        }
    }
}
