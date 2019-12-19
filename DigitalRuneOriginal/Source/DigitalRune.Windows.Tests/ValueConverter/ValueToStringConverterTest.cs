using System.Globalization;
using System.Windows;
using System.Windows.Input;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using TestFixtureAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using TestFixtureSetUp = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.ClassInitializeAttribute;
using TestFixtureTearDown = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.ClassCleanupAttribute;
using SetUpAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestInitializeAttribute;
using TearDownAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestCleanupAttribute;
using TestAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
#else
using NUnit.Framework;




using System.Windows.Controls;



namespace DigitalRune.Windows.Tests
{
    [TestFixture]
    public class ValueToStringConverterTest
    {
        [Test]
        public void TestNull()
        {
            Assert.AreEqual(DependencyProperty.UnsetValue, new ValueToStringConverter().Convert(null, typeof(string), null, CultureInfo.InvariantCulture));
        }



        [Test]
        public void Convert()
        {
            Assert.AreEqual("Ctrl+A", new ValueToStringConverter().Convert(new KeyGesture(Key.A, ModifierKeys.Control), typeof(string), null, CultureInfo.InvariantCulture));
        }




        [Test]
        public void Convert()
        {
            Assert.AreEqual("2*", new ValueToStringConverter().Convert(new DataGridLength(2, DataGridLengthUnitType.Star), typeof(string), null, CultureInfo.InvariantCulture));
        }

    }
}
