using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Sudoku2
{
    class SudokuBorder
    {
        Grid borderGrid;
        public SudokuBorder(Grid g)
        {
            // REVIEW: this could also be in the XAML I think.
            borderGrid = g;
            borderGrid.Children.Clear();
            borderGrid.RowDefinitions.Clear();
            borderGrid.ColumnDefinitions.Clear();
            for (int i = 0; i < 3; i++)
            {
                borderGrid.ColumnDefinitions.Add(new ColumnDefinition());
                for (int j = 0; j < 3; j++)
                {
                    borderGrid.RowDefinitions.Add(new RowDefinition());
                    Border gridBorder = new Border
                    {
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(2)
                    };
                    borderGrid.Children.Add(gridBorder);
                    gridBorder.SetBinding(Border.HeightProperty, new Binding()
                    {
                        Path = new PropertyPath("ActualWidth"),
                        Source = gridBorder
                    });
                    Grid.SetRow(gridBorder, i);
                    Grid.SetColumn(gridBorder, j);
                }
            }
        }
    }
}
