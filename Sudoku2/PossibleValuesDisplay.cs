using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Sudoku2
{
    public class PossibleValuesDisplay
    {
        Grid grid;
        SudokuBoard sourceBoard;
        public Visibility IsVisible { get => grid.Visibility; set { grid.Visibility = value; } }
        public PossibleValuesDisplay(SudokuBoard s, Grid g)
        {
            sourceBoard = s;
            grid = g;
            grid.Children.Clear();
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();
            grid.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < 9; i++)
            {
                Border gridBorder = new Border
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(2),
                    Margin = new Thickness(10, 5, 10, 5)
                };

                grid.ColumnDefinitions.Add(new ColumnDefinition());
                TextBlock txt = new TextBlock();
                gridBorder.Child = txt;
                grid.Children.Add(gridBorder);
                txt.Text = $"{i + 1}";
                txt.Margin = new Thickness(5);
                txt.HorizontalAlignment = HorizontalAlignment.Center;
                txt.FontSize = 18;
                txt.FontWeight = FontWeights.Bold;
                Grid.SetColumn(gridBorder, i);
                Grid.SetRow(gridBorder, 0);
            }
            grid.Visibility = Visibility.Hidden;
        }
        public void Refresh(TextBox cell)
        {
            int[] countValues = sourceBoard.CountValues(cell);
            for (int i = 0; i < grid.Children.Count; i++)
            {
                switch (countValues[i])
                {
                    case 0:
                        ((Border)grid.Children[i]).BorderBrush = Brushes.Black;
                        ((TextBlock)(((Border)grid.Children[i]).Child)).Foreground = Brushes.Black;
                        break;
                    case 1:
                        ((Border)grid.Children[i]).BorderBrush = Brushes.LightGray;
                        ((TextBlock)(((Border)grid.Children[i]).Child)).Foreground = Brushes.LightGray;
                        break;
                    default:
                        ((Border)grid.Children[i]).BorderBrush = Brushes.Red;
                        ((TextBlock)(((Border)grid.Children[i]).Child)).Foreground = Brushes.Red;
                        break;
                }
            }
        }
    }
}
