using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PubComp.Testing.TestingUtils
{
    public static class NumericAssert
    {
        public static void AreSimilarWithinEpsilon(double expected, double actual, double epsilon)
        {
            if (Math.Abs(expected - actual) >= epsilon)
                Assert.Fail("Abs(expected - actual) >= epsilon. expected: {0}, actual {1}, epsilon: {2}",
                    expected, actual, epsilon);
        }

        public static void AreSimilarWithinPercentage(double expected, double actual, double percentage)
        {
            if (Math.Abs(expected - actual) / Math.Max(Math.Abs(expected), Math.Abs(actual)) * 100.0 >= percentage)
                Assert.Fail("Abs(expected - actual) / Max(Abs(expected), Abs(actual)) * 100.0 >= percentage. expected: {0}, actual {1}, percentage: {2}",
                    expected, actual, percentage);
        }
    }
}
