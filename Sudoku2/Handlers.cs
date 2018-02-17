using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Sudoku2
{
    public class Handlers
    {
        public SudokuBoard sudokuBoard;
        public PossibleValuesDisplay possibleValues;
        public Handlers(SudokuBoard board, PossibleValuesDisplay display)
        {
            sudokuBoard = board;
            possibleValues = display;
        }
        public void textBox_Pasted(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextNumeric(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
            textBox_Changed(sender, e);
        }
        public void textBox_Changed(object sender, RoutedEventArgs e)
        {
            if (sudokuBoard.IsSolved)
                sudokuBoard.MarkBoardAsCompleted();

            (sender as TextBox).Foreground = sudokuBoard.IsCellValid(sender as TextBox) ? Brushes.Black : Brushes.Red;

            foreach (var cell in sudokuBoard.GetIntersectingCells(sender as TextBox))
                cell.Foreground = sudokuBoard.IsCellValid(cell) ? Brushes.Black : Brushes.Red;
        }
        public void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!((TextBox)sender).IsReadOnly)
            {
                possibleValues.IsVisible = Visibility.Visible;

                foreach (var cell in sudokuBoard.GetIntersectingCells(sender as TextBox))
                    cell.Background = Brushes.LightYellow;

                possibleValues.Refresh(sender as TextBox);
            }
        }
        public void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!((TextBox)sender).IsReadOnly)
            {
                possibleValues.IsVisible = Visibility.Hidden;
                foreach (var cell in sudokuBoard.GetIntersectingCells(sender as TextBox))
                    cell.Background = Brushes.Transparent;
            }
        }
        #region Input Filters
        public void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }
        public static bool IsTextNumeric(string text)
        {
            Regex regex = new Regex("[1-9]+"); //regex that matches allowed text
            return regex.IsMatch(text);
        }
        #endregion
    }
}
