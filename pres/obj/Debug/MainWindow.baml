   M S B A M L   `   `   `  ����?  ;pres, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:prespres  5   	       pres.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   R NOxyPlot.Wpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0 oxyhttp://oxyplot.org/wpf  5   	   localclr-namespace:pres  5   	     � Title$  
MainWindow��5   	   $	��600��6   $	��525��6)    � DataContext 5         pres.MainWindowModel  5	   
   .��5      �  � RowDefinitions 5   
   �� 5      �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
     OxyPlot.Wpf.PlotView  $��0��62      Model 6   �� *
DataPlot+62   �� 5   
   -   5        $��1��5   (   .��66   	Display6?   (  "System.Windows.Controls.DatePicker  5   
   -   5          Name$ DateFrom��5      $��2��6'     5   
   -   5        $ DateTo��5      $��3��6%   �� 5   
   -   5         ��Name$ StockId��5      $��4��6#   �� 5   
   -   5         ��Name$ 	timeFrame��5      $��5��6&   .@�5      �� .��6   Day6   �� 5      .��6   Week6    �� 5      .��6   Month6!   5      5      5!      