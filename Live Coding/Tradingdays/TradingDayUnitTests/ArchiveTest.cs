using TradingdayDal;
using Xunit;

namespace TradingDayUnitTests
{
    public class ArchiveTest
    {
        string url = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist.xml";
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TradingDays;Integrated Security=True;MultipleActiveResultSets=True";

        [Fact]
        public void IsArchiveInitializing()
        {
            Archive archive = new Archive(url);

            Assert.Equal(64, archive.TradingDays.Count);
        }

        [Fact]
        public void IsDataSaved()
        {
            Archive archive = new Archive(url);
            int result = archive.SaveData(connectionString);

            Assert.NotEqual(0, result);
        }
    }
}