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

namespace pres
{
    using OxyPlot;

    public class MainWindowModel
    {
        PlotModel _model = null;

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
                       // , Minimum = OxyPlot.Axes.DateTimeAxis.ToDouble(new DateTime(2013, 1, 1))
                      //  , Maximum = OxyPlot.Axes.DateTimeAxis.ToDouble(new DateTime(2014, 1, 1))
                      //  , IntervalLength = 75
                      
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
                    //_series.CandleWidth = OxyPlot.Axes.DateTimeAxis.ToDouble(new DateTime(2013, 3, 20)) - OxyPlot.Axes.DateTimeAxis.ToDouble(new DateTime(2013, 3, 5));
                    //_series.Items.Add(new OxyPlot.Series.HighLowItem(
                    //    OxyPlot.Axes.DateTimeAxis.ToDouble(new DateTime(2013, 4, 15))
                    //    , 20, 15, 17, 19));
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
                     //_average.CandleWidth = OxyPlot.Axes.DateTimeAxis.ToDouble(new DateTime(2013, 3, 20)) - OxyPlot.Axes.DateTimeAxis.ToDouble(new DateTime(2013, 3, 5));
                     //_average.Items.Add(new OxyPlot.Series.HighLowItem(
                     //    OxyPlot.Axes.DateTimeAxis.ToDouble(new DateTime(2013, 4, 15))
                     //    , 20, 15, 17, 19));
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
        public void ShowMovingAverage(pres.TimeFrame timeFrame, IEnumerable<StockIntervalData> candelSeries)
        {
         
        }
    }
    

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Worker w = new Worker();
            try
            {
                ((MainWindowModel)DataContext).ShowCandleSeries(TimeFrame.Week, w.GetStockData(TimeFrame.Week, "AAPL", new DateTime(2013, 1, 1), new DateTime(2014, 1, 1)));
            }
            catch (WebException webEx)
            {
                Console.WriteLine(webEx.ToString());
            }

        }
    }
}
