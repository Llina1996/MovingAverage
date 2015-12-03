using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Xml;

namespace pres
{
    using OxyPlot;
    using System.ComponentModel;
    using System.Globalization;

    public class MainWindowModel
    {
        public MainWindowModel()
        {
            LoadUserPrefs();
            UpdateData();
        }

       public  PlotModel _model = null;


       public DateTime dateFrom { get; set; }
       public DateTime dateTo { get; set; }
       public string  StockId { get; set; }
       public TimeFrame timeFrame { get; set; }

       public void SaveUserPrefs()
       {
           save();
       }
        
       public void save()
       {
           XmlDocument c = new XmlDocument();
           c.Load("С:\\xml_document.xml");
           XmlElement saveNode = c["save"];
           if (saveNode != null)
           {
               saveNode.SetAttribute("stock_id", StockId);
               saveNode.SetAttribute("date_from", dateFrom.ToString("yyyy-MM-dd"));
               saveNode.SetAttribute("date_to", dateTo.ToString("yyyy-MM-dd"));
               saveNode.SetAttribute("time_frame", timeFrame.ToString());
              
           }
           c.Save("С:\\xml_document.xml");
       }
       
       CultureInfo provider1 = CultureInfo.InvariantCulture;
       public void LoadUserPrefs()
       {
       
           try
           {
               XmlDocument c = new XmlDocument();
               c.Load("D:\\xml_document.xml");
               XmlElement saveNode = c["save"];
               StockId=saveNode.GetAttribute("stock_id");
               dateFrom = DateTime.ParseExact( saveNode.GetAttribute("date_from"), "yyyy-MM-dd", provider1);
               dateTo = DateTime.ParseExact(saveNode.GetAttribute("date_to"), "yyyy-MM-dd", provider1);
               string timeFr=saveNode.GetAttribute("time_frame");

               if (timeFr == "Day")
               {
                   timeFrame = TimeFrame.Day;
                   
               }
               else if (timeFr == "Month")
               {
                   timeFrame = TimeFrame.Month;
               }
               else if (timeFr == "Week")
               {
                   timeFrame = TimeFrame.Week;
               }

             
              
           }
           catch(Exception exp) {
               dateFrom = new DateTime(2012, 1, 1);
               dateTo = new DateTime(2013, 1, 1);
               StockId = "AAPL";
               timeFrame = TimeFrame.Month;
               System.Diagnostics.Trace.WriteLine("exc: " + exp.Message);
           }

       }
        Worker _worker = null;
        public Worker worker
        {
            get {
                if (_worker == null)
                {
                    _worker = new Worker();
                }
                return _worker;
            }

        }

       public void OnClosing(object sender, CancelEventArgs e)
        {
            SaveUserPrefs();
        }

        OxyPlot.Axes.DateTimeAxis _dateAxis = null;
        public OxyPlot.Axes.DateTimeAxis DateAxis
        {
            get
            {
                if (_dateAxis == null)
                {
                    _dateAxis = new OxyPlot.Axes.DateTimeAxis
                    {
                        Position=OxyPlot.Axes.AxisPosition.Bottom
                        , StringFormat = "dd/MM/yyyy"
                        , Title = "Date"
                        , MinorIntervalType = OxyPlot.Axes.DateTimeIntervalType.Months
                        , IntervalType = OxyPlot.Axes.DateTimeIntervalType.Months
                        , MajorGridlineStyle = LineStyle.Solid
                        , MinorGridlineStyle = LineStyle.None
                        , Angle = -90
                    };
                    _dateAxis.ExtraGridlineStyle = LineStyle.None;
                    _dateAxis.MinorTickSize = 0;
                }
                return _dateAxis;
            }
        }

        OxyPlot.Series.CandleStickSeries _series = null;
        public OxyPlot.Series.CandleStickSeries CandleSeries
        {
            get
            {
                if (_series == null)
                {
                    _series = new OxyPlot.Series.CandleStickSeries();
                }
                return _series;
            }
        }
         OxyPlot.Series.LineSeries _average = null;
         public OxyPlot.Series.LineSeries Average
         {
             get
             {
                 if (_average == null)
                 {
                     _average = new OxyPlot.Series.LineSeries();
                 }
                 return _average;
             }
         }

        public PlotModel DataPlot
        {
            get 
            {
                if (_model == null)
                {
                    _model = new PlotModel();
                    _model.Axes.Add(DateAxis);
                    _model.Axes.Add(PriceAxis);
                    _model.Series.Add(CandleSeries);
                    _model.Series.Add(Average);
                }
                return _model;
            }
        }

        OxyPlot.Axes.LinearAxis _priceAxis=null;
        public OxyPlot.Axes.LinearAxis PriceAxis
        {
            get
            {
                if (_priceAxis == null)
                {
                    _priceAxis = new OxyPlot.Axes.LinearAxis
                    {
                        Position = OxyPlot.Axes.AxisPosition.Left,
           
                    };
                }
                return _priceAxis;
            }
        }

        public void ShowCandleSeries(pres.TimeFrame timeFrame,IEnumerable<StockIntervalData> candelSeries) 
        {
           CandleSeries.Items.Clear();
           IEnumerable<OxyPlot.Series.HighLowItem> hl = candelSeries.Select(
               x => new OxyPlot.Series.HighLowItem(OxyPlot.Axes.DateTimeAxis.ToDouble(x.date),
               (double)x.high,(double)x.low,(double)x.open,(double)x.close)
           );
           CandleSeries.Items.AddRange(hl);
           if (timeFrame == TimeFrame.Month)
           {
               DateAxis.IntervalType = OxyPlot.Axes.DateTimeIntervalType.Months;
               DateAxis.MinorIntervalType = OxyPlot.Axes.DateTimeIntervalType.Months;
           }
           else if (timeFrame == TimeFrame.Week)
           {
               DateAxis.IntervalType = OxyPlot.Axes.DateTimeIntervalType.Weeks;
               DateAxis.MinorIntervalType = OxyPlot.Axes.DateTimeIntervalType.Weeks;
           }
           else if (timeFrame == TimeFrame.Day)
           {
               DateAxis.IntervalType = OxyPlot.Axes.DateTimeIntervalType.Days;
               DateAxis.MinorIntervalType = OxyPlot.Axes.DateTimeIntervalType.Days;
           }

        }
        public void ShowMovingAverage(pres.TimeFrame timeFrame, IList<StockIntervalData> candelSeries)
        {
        
            Average.Points.Clear();
            foreach (Worker.Average c in worker.movingAverage(candelSeries, 50))
            {
                DataPoint p = new DataPoint(OxyPlot.Axes.DateTimeAxis.ToDouble(c.date),(double)c.valueAverage);
                Average.Points.Add(p);
            }
        }
        public void showData(TimeFrame time,DateTime start,DateTime end,string StockId){
           
             List<StockIntervalData> c= worker.GetStockData(time,StockId,start,end);
             ShowCandleSeries(time, c);
             ShowMovingAverage(time, c);
        }
        public void UpdateData()
        {
            try
            {
                showData(timeFrame, dateFrom, dateTo, StockId);
                DataPlot.InvalidatePlot(true);
            }
            catch (WebException webEx)
            {
                System.Diagnostics.Trace.WriteLine("exc: " + webEx.Message);
            }
        }
    }
    



    public partial class MainWindow : Window
    {
        public void PrintXmlNode(XmlNode node, int offset)
        {
            for (int i = 0; i < offset; ++i)
                System.Diagnostics.Trace.Write("   ");
            System.Diagnostics.Trace.Write(node.Name + " ");
            if (node.Attributes != null)
                foreach (XmlAttribute c in node.Attributes)
                    System.Diagnostics.Trace.Write(c.Name + "=" + c.Value + " ");
            System.Diagnostics.Trace.WriteLine("");
            foreach (XmlNode child in node.ChildNodes)
                PrintXmlNode(child, offset + 1);
        }



        private void OnDisplayClick(object sender, RoutedEventArgs e)
        {
            ((MainWindowModel)DataContext).StockId = StockId.Text;
            if (DateFrom.SelectedDate.HasValue)
            {
                ((MainWindowModel)DataContext).dateFrom = DateFrom.SelectedDate.Value;
            }
            if (DateTo.SelectedDate.HasValue)
            {
                ((MainWindowModel)DataContext).dateTo = DateTo.SelectedDate.Value;
            }
            if (timeFrame.SelectedIndex >= 0)
            {
                if (timeFrame.SelectedIndex == 0)
                    ((MainWindowModel)DataContext).timeFrame = TimeFrame.Day;
                else if (timeFrame.SelectedIndex == 1)
                    ((MainWindowModel)DataContext).timeFrame = TimeFrame.Week;
                else
                    ((MainWindowModel)DataContext).timeFrame = TimeFrame.Month;
            }
            ((MainWindowModel)DataContext).UpdateData();
        }

        public MainWindow()
        {
            InitializeComponent();
            
            ((MainWindowModel)DataContext).LoadUserPrefs();
            DateTo.SelectedDate = ((MainWindowModel)DataContext).dateTo;
            DateFrom.SelectedDate = ((MainWindowModel)DataContext).dateFrom;
            StockId.Text = ((MainWindowModel)DataContext).StockId;
            if (((MainWindowModel)DataContext).timeFrame==TimeFrame.Day) {
                timeFrame.SelectedIndex = 0;
            }
            else if (((MainWindowModel)DataContext).timeFrame == TimeFrame.Week) {
                 timeFrame.SelectedIndex = 1;
            }
            else if (((MainWindowModel)DataContext).timeFrame == TimeFrame.Month)
            {
                timeFrame.SelectedIndex = 2;
            }

            timeFrame.Focus();

            Closing += ((MainWindowModel)DataContext).OnClosing;
      
        }
    }
}
