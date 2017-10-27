using System.Text;
using Abp.Runtime.Security;
using Shouldly;
using NUnit.Framework;

namespace Orchard.Tests.Security
{
    public class SimpleStringCipher_Tests
    {
        [Theory]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("A")]
        [TestCase("This is a bit long text to test SimpleStringCipher: Istanbul, historically also known as Constantinople and Byzantium, is the most populous city in Turkey and the country's economic, cultural, and historic center.")]
        public void Should_Decrypt_Encrypted_Text(string plainText)
        {
            var encryptedText = SimpleStringCipher.Instance.Encrypt(plainText);
            SimpleStringCipher.Instance.Decrypt(encryptedText).ShouldBe(plainText);
        }

        [Test]
        public void Should_Be_Able_To_Change_InitVector_And_Key()
        {
            const string initVectorString = "1234BCHF9876skd*";
            const string myKey = "84ncpaKMC_!TuAna";
            const string plainText = "This is a plain text!";

            var cipher = new SimpleStringCipher
            {
                InitVectorBytes = Encoding.ASCII.GetBytes(initVectorString)
            };

            var enryptedText = cipher.Encrypt(plainText, myKey);
            cipher.Decrypt(enryptedText, myKey).ShouldBe(plainText);
        }
    }
}
