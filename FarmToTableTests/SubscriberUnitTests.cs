using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using FarmToTableData.Models;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FarmToTableTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void VerifyNothing_ReturnsTrue()
        {
            bool a = true;
            bool b = false;
            Assert.That(a, Is.Not.EqualTo(b));
        }
    }
}