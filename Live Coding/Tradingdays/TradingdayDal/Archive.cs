using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TradingdayDal
{
    public class Archive
    {
        public Archive(string url)
        {
            this.Url = url;
            GetData(url);
            //SaveData();
        }

        public int SaveData(string connectionString)
        {
            TradingDayContext context = new TradingDayContext(connectionString);

            context.Database.EnsureDeleted();
            // Sicherstellen, dass DB existiert
            context.Database.EnsureCreated();

            //context.Log = LogIt;

            context.TradingDays.AddRange(this.TradingDays);
            int rowsAffected = context.SaveChanges();
            return rowsAffected;
        }

        private void LogIt(string obj)
        {
            Debug.WriteLine(obj);
        }

        private void GetData(string url)
        {
            XDocument document = XDocument.Load(url);

            this.TradingDays = document.Root.Descendants().Where(nd => nd.Name.LocalName == "Cube" && nd.Attributes().Any(at => at.Name == "time"))
                                                        .Select(nd => new TradingDay(nd))
                                                        .ToList();
        }

        public List<TradingDay> TradingDays { get; set; }

        public string Url { get; set; }
    }
}
