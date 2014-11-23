using NUnit.Framework;

namespace Perrich.SepaWriter.Test
{
    [TestFixture]
    public class SepaIbanDataTest
    {
        private const string Bic = "SOGEFRPPXXX";
        private const string Iban = "FR7030002005500000157845Z02";
        private const string IbanWithSpace = "FR70 30002  005500000157845Z    02";
        private const string Name = "A_NAME";

        [Test]
        public void ShouldBeValidIfAllDataIsNotNull()
        {
            var data = new SepaIbanData
                {
                    Bic = Bic,
                    Iban = Iban,
                    Name = Name
                };

            Assert.True(data.IsValid);
        }

        [Test]
        public void ShouldBeValidIfAllDataIsNotNullAndBicIsUnknown()
        {
            var data = new SepaIbanData
            {
                UnknownBic = true,
                Iban = Iban,
                Name = Name
            };

            Assert.True(data.IsValid);
        }
        
        [Test]
        public void ShouldRemoveSpaceInIban()
        {
            var data = new SepaIbanData
            {
                Bic = Bic,
                Iban = IbanWithSpace,
                Name = Name
            };

            Assert.True(data.IsValid);
            Assert.AreEqual(Iban, data.Iban);
        }

        [Test]
        public void ShouldKeepNameIfLessThan70Chars()
        {
            var data = new SepaIbanData
                {
                    Bic = Bic,
                    Iban = Iban,
                    Name = Name
                };

            Assert.AreEqual(Bic, data.Bic);
            Assert.AreEqual(Name, data.Name);
            Assert.AreEqual(Iban, data.Iban);
        }

        [Test]
        public void ShouldNotBeValidIfBicIsNull()
        {
            var data = new SepaIbanData
                {
                    Iban = Iban,
                    Name = Name
                };

            Assert.False(data.IsValid);
        }

        [Test]
        public void ShouldNotBeValidIfIbanIsNull()
        {
            var data = new SepaIbanData
                {
                    Bic = Bic,
                    Name = Name
                };

            Assert.False(data.IsValid);
        }

        [Test]
        public void ShouldNotBeValidIfNameIsNull()
        {
            var data = new SepaIbanData
                {
                    Bic = Bic,
                    Iban = Iban
                };

            Assert.False(data.IsValid);
        }

        [Test]
        public void ShouldReduceNameIfGreaterThan70Chars()
        {
            const string longName = "12345678901234567890123456789012345678901234567890123456789012345678901234567890";
            const string expectedName = "1234567890123456789012345678901234567890123456789012345678901234567890";
            var data = new SepaIbanData
                {
                    Bic = Bic,
                    Iban = Iban,
                    Name = longName
                };

            Assert.AreEqual(expectedName, data.Name);
        }

        [Test]
        [ExpectedException(typeof (SepaRuleException), ExpectedMessage = "Invalid BIC code.",
            MatchType = MessageMatch.Exact)]
        public void ShouldRejectBadBic()
        {
            new SepaIbanData
                {
                    Bic = "BIC"
                };
        }

        [Test]
        [ExpectedException(typeof (SepaRuleException), ExpectedMessage = "Invalid IBAN code.",
            MatchType = MessageMatch.Exact)]
        public void ShouldRejectTooLongIban()
        {
            new SepaIbanData
                {
                    Iban = "FR012345678901234567890123456789012"
                };
        }

        [Test]
        [ExpectedException(typeof (SepaRuleException), ExpectedMessage = "Invalid IBAN code.",
            MatchType = MessageMatch.Exact)]
        public void ShouldRejectTooShortIban()
        {
            new SepaIbanData
                {
                    Iban = "FR01234567890"
                };
        }
    }
}