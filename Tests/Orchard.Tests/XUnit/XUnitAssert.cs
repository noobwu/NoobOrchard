using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Tests
{
    public  class XUnitAssert
    {
        public static void Contains(string expectedSubstring, string actualString)
        {
            bool condition = false;
            if (string.IsNullOrEmpty(actualString))
            {
                condition=((string.IsNullOrEmpty(actualString)) ? true : false);
            }
            else
            {
                condition = actualString.IndexOf(expectedSubstring)>-1;
            }
            NUnit.Framework.Assert.IsTrue(condition);
        }
        /// <summary>
        /// Verifies that two objects are equal, using a custom equatable comparer.
        /// </summary>
        /// <typeparam name="T">The type of the objects to be compared</typeparam>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The value to be compared against</param>
        /// <param name="comparer">The comparer used to compare the two objects</param>
        /// <exception cref="EqualException">Thrown when the objects are not equal</exception>
        public static void Equal<T>(T expected, T actual, IEqualityComparer<T> comparer)
        {
            XUnitAssert.GuardArgumentNotNull("comparer", comparer);
            Assert.IsTrue(comparer.Equals(expected, actual));
        }


        /// <summary/>
        internal static void GuardArgumentNotNull(string argName, object argValue)
        {
            if (argValue == null)
                throw new ArgumentNullException(argName);
        }

        /// <summary>
        /// Verifies that a string contains a given sub-string, using the given comparison type.
        /// </summary>
        /// <param name="expectedSubstring">The sub-string expected to be in the string</param>
        /// <param name="actualString">The string to be inspected</param>
        /// <param name="comparisonType">The type of string comparison to perform</param>
        /// <exception cref="ContainsException">Thrown when the sub-string is not present inside the string</exception>
        public static void Contains(string expectedSubstring, string actualString, StringComparison comparisonType)
        {
            if (actualString == null || actualString.IndexOf(expectedSubstring, comparisonType) < 0)
            {
                Assert.Fail("expected:{0},actual:[1}", expectedSubstring, actualString);
            }
        }

        /// <summary>
        /// Verifies that an object is of the given type or a derived type.
        /// </summary>
        /// <typeparam name="T">The type the object should be</typeparam>
        /// <param name="object">The object to be evaluated</param>
        /// <returns>The object, casted to type T when successful</returns>
        /// <exception cref="IsAssignableFromException">Thrown when the object is not the given type</exception>
        public static T IsAssignableFrom<T>(object @object)
        {
            IsAssignableFrom(typeof(T), @object);
            return (T)@object;
        }

        /// <summary>
        /// Verifies that an object is of the given type or a derived type.
        /// </summary>
        /// <param name="expectedType">The type the object should be</param>
        /// <param name="object">The object to be evaluated</param>
        /// <exception cref="IsAssignableFromException">Thrown when the object is not the given type</exception>
        public static void IsAssignableFrom(Type expectedType, object @object)
        {
            GuardArgumentNotNull("expectedType", expectedType);

            if (@object == null || !expectedType.GetTypeInfo().IsAssignableFrom(@object.GetType().GetTypeInfo()))
            {
                Assert.Fail("expectedType:{0},actualType:{1}",expectedType.GetTypeInfo(),@object.GetType().GetTypeInfo());
            }
        }


        /// <summary>
        /// Verifies that the exact exception is thrown (and not a derived exception type).
        /// </summary>
        /// <typeparam name="T">The type of the exception expected to be thrown</typeparam>
        /// <param name="testCode">A delegate to the task to be tested</param>
        /// <returns>The exception that was thrown, when successful</returns>
        /// <exception cref="ThrowsException">Thrown when an exception was not thrown, or when an exception of the incorrect type is thrown</exception>
        public static async Task<T> ThrowsAsync<T>(Func<Task> testCode)
            where T : Exception
        {
            return (T)Throws(typeof(T), await RecordExceptionAsync(testCode));
        }
        static Exception Throws(Type exceptionType, Exception exception)
        {
            GuardArgumentNotNull("exceptionType", exceptionType);

            if (exception == null)
                throw new Exception(exceptionType.ToString());

            if (!exceptionType.Equals(exception.GetType()))
                throw new Exception(exceptionType.ToString(), exception);

            return exception;
        }
        /// <summary>
        /// Records any exception which is thrown by the given task.
        /// </summary>
        /// <param name="testCode">The task which may thrown an exception.</param>
        /// <returns>Returns the exception that was thrown by the code; null, otherwise.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The caught exception is resurfaced to the user.")]
        protected static async Task<Exception> RecordExceptionAsync(Func<Task> testCode)
        {
            GuardArgumentNotNull("testCode", testCode);

            try
            {
                await testCode();
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        /// <summary>
        /// Verifies that a value is within a given range.
        /// </summary>
        /// <typeparam name="T">The type of the value to be compared</typeparam>
        /// <param name="actual">The actual value to be evaluated</param>
        /// <param name="low">The (inclusive) low value of the range</param>
        /// <param name="high">The (inclusive) high value of the range</param>
        /// <exception cref="InRangeException">Thrown when the value is not in the given range</exception>
        public static void InRange<T>(T actual, T low, T high) where T : IComparable
        {
            InRange(actual, low, high, GetComparer<T>());
        }

        /// <summary>
        /// Verifies that a value is within a given range, using a comparer.
        /// </summary>
        /// <typeparam name="T">The type of the value to be compared</typeparam>
        /// <param name="actual">The actual value to be evaluated</param>
        /// <param name="low">The (inclusive) low value of the range</param>
        /// <param name="high">The (inclusive) high value of the range</param>
        /// <param name="comparer">The comparer used to evaluate the value's range</param>
        /// <exception cref="InRangeException">Thrown when the value is not in the given range</exception>
        public static void InRange<T>(T actual, T low, T high, IComparer<T> comparer)
        {
            GuardArgumentNotNull("comparer", comparer);

            if (comparer.Compare(low, actual) > 0 || comparer.Compare(actual, high) > 0)
            {
                string strActual= actual == null ? null : actual.ToString();
                Assert.Fail(string.Format(CultureInfo.CurrentCulture,
                                     "{0}\r\nRange:  ({1} - {2})\r\nActual: {3}",
                                     "Assert.InRange() Failure", low, high, strActual));
            }
        }

        static IComparer<T> GetComparer<T>() where T : IComparable
        {
            return new AssertComparer<T>();
        }

        static IEqualityComparer<T> GetEqualityComparer<T>(IEqualityComparer innerComparer = null)
        {
            return new AssertEqualityComparer<T>(innerComparer);
        }

        static Exception ThrowsAny(Type exceptionType, Exception exception)
        {
            GuardArgumentNotNull("exceptionType", exceptionType);

            if (exception == null)
            {
                Assert.Fail($"Assert.Throws() Failure,exceptionType:{nameof(exceptionType)},(No exception was thrown)");
            }

            if (!exceptionType.GetTypeInfo().IsAssignableFrom(exception.GetType().GetTypeInfo()))
            {
                Assert.Fail($"Assert.Throws() Failure,exceptionType:{nameof(exceptionType)},exception:{nameof(Exception)}");
            }

            return exception;
        }
    }
   
}
