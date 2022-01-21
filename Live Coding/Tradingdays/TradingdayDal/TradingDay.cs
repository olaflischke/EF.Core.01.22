using System.Globalization;
using System.Xml.Linq;

namespace TradingdayDal
{
    public class TradingDay
    {
        public TradingDay()
        {

        }

        public TradingDay(XElement xElement)
        {
            this.Date = Convert.ToDateTime(xElement.Attribute("time").Value);

            NumberFormatInfo nfi = new NumberFormatInfo() { NumberDecimalSeparator = "." };

            this.Currencies = xElement.Elements().Select(el => new Currency()
                                                                            {
                                                                                Symbol = el.Attribute("currency").Value,
                                                                                Rate = Convert.ToDouble(el.Attribute("rate").Value, nfi)
                                                                            }
                                                        ).ToList();
        }

        public DateTime Date { get; set; }
        public List<Currency> Currencies { get; set; }
        public int Id { get; set; }
    }
}