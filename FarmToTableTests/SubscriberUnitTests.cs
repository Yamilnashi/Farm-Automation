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
        public async Task VerifyNothing_ReturnsTrue()
        {
            Assert.That(1, Is.Not.EqualTo(2));
        }
    }
}