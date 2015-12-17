using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Globalization;

namespace pres
{



    public enum TimeFrame
    {
        Day, Week, Month

    }

    public struct StockIntervalData
    {
        public DateTime date;
        public decimal open;
        public decimal close;
        public decimal low;
        public decimal high;
        public decimal volume;
    }
    public class Worker
    {
        public byte[] Download(string address)
        {
            
            WebClient client = new WebClient();

            Byte[] pageData = client.DownloadData(address);
        
            return pageData;
        }
       public struct Average{
           public DateTime date;
           public decimal valueAverage;
        }

        public decimal calculateAveage (List<Average> c,int start,int end )
        {
            decimal res = 0;
            for (int i = start; i <= end; ++i)
            {
                res += c[i].valueAverage;
            }
            return res / (end - start + 1);
        }
        public List<Average> movingAverage(IList<StockIntervalData> intervals, int N){
        List<Average> result  = new List<Average>();
            for (int i = 0; i < intervals.Count; ++i)
            {
                
                decimal c = (intervals[i].open + intervals[i].close) / 2;
                Average res = new Average { date = intervals[i].date , valueAverage = c };
                result.Add(res);
            }
            for (int i =result.Count-1;i >=0; --i ){
                
                if (( i+1)<N) {
                 Average a= result[i] ;
                    a.valueAverage=calculateAveage(result,0,i);
                    result[i]=a;
                }
                else {
                    Average a = result[i];
                  a.valueAverage=calculateAveage(result,i-N+1,i);
                  result[i] = a;
                }
            }
            return result;

        }

        public List<StockIntervalData> ConvertTimeFrame(List<StockIntervalData> intervals ,TimeFrame timeFrame )
        {
            if (timeFrame == TimeFrame.Day)
            {
                return intervals;
            }
            else if (timeFrame == TimeFrame.Week)
            {
                bool first = true;
                StockIntervalData weekData = new StockIntervalData();
                List<StockIntervalData> weeksIntervals = new List<StockIntervalData>();
                foreach (StockIntervalData dayData in intervals)
                {
                    if(first || (dayData.date - weekData.date).Days>=7 )
                    {
                        if (!first)
                            weeksIntervals.Add(weekData);
                        weekData = dayData;
                        weekData.date.AddDays(-(int)(weekData.date.DayOfWeek));   /// откат  даты на воскресенье
                        first = false;
                    }
                    else 
                    {
                        weekData.close = dayData.close;
                        if (dayData.low < weekData.low)
                            weekData.low = dayData.low;
                        if (dayData.high > weekData.high)
                            weekData.high = dayData.high;
                        weekData.volume += dayData.volume;
                    }

                }
                if (!first)
                {
                    weeksIntervals.Add(weekData);
                }
                return weeksIntervals;
            }
            else if (timeFrame == TimeFrame.Month)
            {
                bool first = true;
                StockIntervalData monthData = new StockIntervalData();
                List<StockIntervalData> monthsIntervals = new List<StockIntervalData>();
                foreach (StockIntervalData dayData in intervals)
                {
                    if (first || monthData.date.Month != dayData.date.Month || monthData.date.Year != dayData.date.Year)
                    {
                        if (!first)
                            monthsIntervals.Add(monthData);
                        monthData = dayData;
                        monthData.date = new DateTime(dayData.date.Year, dayData.date.Month, 1);
                        first = false;
                    }
                    else
                    {
                        monthData.close = dayData.close;
                        if (dayData.low < monthData.low)
                            monthData.low = dayData.low;
                        if (dayData.high > monthData.high)
                            monthData.high = dayData.high;
                        monthData.volume += dayData.volume;
                    }
                }
                if (!first)
                {
                    monthsIntervals.Add(monthData);
                }
                return monthsIntervals;
            }
            else
            {
               
                return intervals;
            }
        }


        public List<StockIntervalData> ParseStockData(byte[] data)
        {
            List<StockIntervalData> result = new List<StockIntervalData>();
            StreamReader reader = new StreamReader(new MemoryStream(data), Encoding.ASCII);
            reader.ReadLine();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                try
                {
                    StockIntervalData d = new StockIntervalData();
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    string[] tokens = line.Split(new char[] { ',' });
                    d.date = DateTime.ParseExact(tokens[0], "yyyy-MM-dd", provider);
                    d.open = System.Decimal.Parse(tokens[1], provider);
                    d.high = System.Convert.ToDecimal(tokens[2], provider);
                    d.low = System.Convert.ToDecimal(tokens[3], provider);
                    d.close = System.Convert.ToDecimal(tokens[4], provider);
                    d.volume = System.Convert.ToDecimal(tokens[5], provider);
                    result.Insert(0, d);
                }
                catch (FormatException fex)
                {
                    Console.WriteLine(fex.Message);
                }
            }
            return result;
        }

        public List<StockIntervalData> GetStockData(TimeFrame timeframe, string stockId, DateTime from, DateTime to)
        { 
            /*http://ichart.finance.yahoo.com/table.csv?s=CB&d=9&e=30&f=2014&g=m&a=8&b=7&c=2013&ignore=.csv*/
            
            string url = "http://ichart.finance.yahoo.com/table.csv?s=" + stockId
                + "&d=" + (to.Month - 1) + "&e=" + to.Day + "&f=" + to.Year 
                + "&a=" + (from.Month - 1) + "&b=" + (from.Day) + "&c=" + (from.Year)  
                + "&g=d&ignore=.csv";


            byte[] data = Download(url);
            List<StockIntervalData> intervals = ParseStockData(data);
            intervals = ConvertTimeFrame(intervals, timeframe);
            return intervals;
        }

    }
}
