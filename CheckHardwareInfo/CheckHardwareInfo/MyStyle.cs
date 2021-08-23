using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace CheckHardwareInfo
{
    public class MyDataGridHeaderStyle : Style
    {
        public MyDataGridHeaderStyle() : base(typeof(DataGridColumnHeader))
        {
            Setters.Add(new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Center));

            Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Color.FromRgb(40, 50, 69))));

            Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Color.FromRgb(255, 199, 56))));

            Setters.Add(new Setter(Control.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(255, 199, 56))));

            Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(0, 0, 0, 1)));
        }
    }

    public class MyDataGridStyle : Style
    {
        public MyDataGridStyle() : base(typeof(DataGrid))
        {
            Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Color.FromRgb(40, 50, 69))));

            Setters.Add(new Setter(DataGrid.AutoGenerateColumnsProperty, false));

            Setters.Add(new Setter(DataGrid.IsReadOnlyProperty, false));

            Setters.Add(new Setter(DataGrid.CanUserResizeColumnsProperty, false));

            Setters.Add(new Setter(DataGrid.CanUserResizeRowsProperty, false));

            Setters.Add(new Setter(DataGrid.CanUserDeleteRowsProperty, false));

            Setters.Add(new Setter(DataGrid.CanUserAddRowsProperty, false));

            Setters.Add(new Setter(DataGrid.HeadersVisibilityProperty, DataGridHeadersVisibility.Column));

            Setters.Add(new Setter(UIElement.IsEnabledProperty, false));

            Setters.Add(new Setter(DataGrid.IsReadOnlyProperty, false));

            Setters.Add(new Setter(DataGrid.VerticalGridLinesBrushProperty, new SolidColorBrush(Colors.Transparent)));

            Setters.Add(new Setter(DataGrid.HorizontalGridLinesBrushProperty, new SolidColorBrush(Color.FromRgb(255, 199, 56))));

            Setters.Add(new Setter(Control.BorderBrushProperty, new SolidColorBrush(Colors.Transparent)));
        }
    }

    public class MyDataGridTextColumnElementStyle : Style
    {
        public MyDataGridTextColumnElementStyle() : base(typeof(TextBlock))
        {
            Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
        }
    }

    public class MyDataGridRowStyle : Style
    { 
        public MyDataGridRowStyle() : base(typeof(DataGridRow))
        {
            Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Colors.Transparent)));
        }
    }

    public class MyBorderStyle : Style
    { 
        public MyBorderStyle() : base(typeof(Border))
        {
            Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(4)));
            Setters.Add(new Setter(Border.BorderBrushProperty, new SolidColorBrush(Colors.White)));
            Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1)));
            Setters.Add(new Setter(Border.MarginProperty, new Thickness(5)));
            Setters.Add(new Setter(Border.BackgroundProperty, new SolidColorBrush(Color.FromRgb(42, 56, 101))));
        }
    }
}
