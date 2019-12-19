using System;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using NUnit.Framework;



namespace DigitalRune.Windows.Charts.Tests
{
    public static class AssertHelper
    {
        public static void Throws<T>(Action action) where T : Exception
        {

            Assert.ThrowsException<T>(action);
#else
            Assert.Throws(typeof(T), () => action());

        }
    }
}
