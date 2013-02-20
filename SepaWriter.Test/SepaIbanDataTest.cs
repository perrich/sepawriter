using NUnit.Framework;

namespace SepaWriter.Test
{
    [TestFixture]
    public class SepaIbanDataTest
    {
        private const string Bic = "BANK_BIC";
        private const string Iban = "ACCOUNT_IBAN";
        private const string Name = "NAME";

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
    }
}