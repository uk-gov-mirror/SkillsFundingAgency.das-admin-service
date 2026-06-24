using NUnit.Framework;
using System;
using SFA.DAS.AdminService.Web.Extensions;

namespace SFA.DAS.AdminService.Web.Tests.Extensions
{
    [TestFixture]
    public class DateExtensionsTests
    {
        [Test]
        public void UtcToUkLocalTime_Winter_ReturnsSameHour()
        {
            var utc = new DateTime(2021, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            var local = utc.UtcToUkLocalTime();

            Assert.That(local.Year, Is.EqualTo(2021));
            Assert.That(local.Month, Is.EqualTo(1));
            Assert.That(local.Day, Is.EqualTo(1));
            Assert.That(local.Hour, Is.EqualTo(12));
            Assert.That(local.Minute, Is.EqualTo(0));
        }

        [Test]
        public void UtcToUkLocalTime_Summer_ReturnsOffsetHour()
        {
            var utc = new DateTime(2021, 7, 1, 12, 0, 0, DateTimeKind.Utc);
            var local = utc.UtcToUkLocalTime();

            Assert.That(local.Year, Is.EqualTo(2021));
            Assert.That(local.Month, Is.EqualTo(7));
            Assert.That(local.Day, Is.EqualTo(1));
            Assert.That(local.Hour, Is.EqualTo(13));
            Assert.That(local.Minute, Is.EqualTo(0));
        }

        [Test]
        public void ToUkDateTimeString_FormatsCorrectly_WinterAndSummer()
        {
            var janUtc = new DateTime(2021, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            Assert.That(janUtc.ToUkDateTimeString(), Is.EqualTo("1 January 2021 12:00"));

            var julUtc = new DateTime(2021, 7, 1, 12, 0, 0, DateTimeKind.Utc);
            Assert.That(julUtc.ToUkDateTimeString(), Is.EqualTo("1 July 2021 13:00"));
        }
    }
}
