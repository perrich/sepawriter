using System;
using NUnit.Framework;
using Perrich.SepaWriter.Utils;

namespace Perrich.SepaWriter.Test.Utils
{
    [TestFixture]
    public class StringUtilsTest
    {
        private const string FirstPart = "012345678";

        [Test]
        public void ShouldTruncateATooLongString()
        {
            const string str = FirstPart + "9" + "another part";
            Assert.AreEqual(FirstPart + "9", StringUtils.GetLimitedString(str, 10));
        }

        [Test]
        public void ShouldNotTruncateSmallString()
        {
            Assert.AreEqual(FirstPart, StringUtils.GetLimitedString(FirstPart, 10));
            Assert.Null(StringUtils.GetLimitedString(null, 10));
        }

        [Test]
        public void ShouldNotTruncateNullString()
        {
            Assert.Null(StringUtils.GetLimitedString(null, 10));
        }

        [Test]
        public void ShouldFormatADate()
        {
            var date = new DateTime(2013, 11, 27);
            Assert.AreEqual("2013-11-27T00:00:00", StringUtils.FormatDateTime(date));
        }

        [Test]
        public void ShouldCleanUpString()
        {
            Assert.AreEqual(FirstPart, StringUtils.RemoveInvalidChar(FirstPart));

            var allowedChars = "@/-?:(). ,'\"+";
            Assert.AreEqual(allowedChars, StringUtils.RemoveInvalidChar(allowedChars));

            Assert.AreEqual("EAEU", StringUtils.RemoveInvalidChar("éàèù"));
        }
    }
}