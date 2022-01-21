using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingdayDal
{
    public class TradingDayContext : DbContext
    {
        string connectionString;

        public Action<string> Log { get; set; }

        public TradingDayContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public DbSet<TradingDay> TradingDays { get; set; }
        public DbSet<Currency> Currencies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
            optionsBuilder.LogTo(l => Log?.Invoke(l));
        }
    }
}
