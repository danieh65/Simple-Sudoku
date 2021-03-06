﻿using System;
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
        //REVIEW: possibly make these private and add getter/setters if necessary?
        //RESPONSE: because this program just consists of one window, I didn't consider 
        //      public/private for these variables. I agree that they should be private.
        public SudokuBoard sudokuBoard;
        public PossibleValuesDisplay possibleValues;
        public Handlers(SudokuBoard board, PossibleValuesDisplay display)
        {
            sudokuBoard = board;
            possibleValues = display;
        }
        public void textBox_Pasted(object sender, DataObjectPastingEventArgs e)
        {
            // REVIEW: this logic is a bit confusing/verbose, perhaps it could be simplified.
            // RESPONSE: I agree, I could probably simplify this into one if statement
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
            // REVIEW: possibly mark cell as invalid before checking if the board is solved.
            // this may or may not cause bugs. 
            // RESPONSE: IsSolved enforces that all cells are valid, so I don't think this should cause bugs.
            if (sudokuBoard.IsSolved)
                sudokuBoard.MarkBoardAsCompleted();

            // REVIEW: consider including the sender cell in the interesecting cells for
            // checking is valid to avoid duplication here
            // RESPONSE: I think this may cause problems with cell coloring and other intersecting cell references.
            //      Perhaps I could add the current cell to the temporary collection for this?
            (sender as TextBox).Foreground = sudokuBoard.IsCellValid(sender as TextBox) ? Brushes.Black : Brushes.Red;

            foreach (var cell in sudokuBoard.GetIntersectingCells(sender as TextBox))
                cell.Foreground = sudokuBoard.IsCellValid(cell) ? Brushes.Black : Brushes.Red;
        }
        public void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // REVIEW: I really like this. I didn't think to use GotFocus and LostFocus
            // instead of attempting to bind to the focused property. This is a much
            // cleaner solution. 
            // RESPONSE: Thank you! I liked how useful got and lost focus were for managing application flow.
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
        // REVIEW: I like the use of these input filters to avoid code duplication. 
        // RESPONSE: Thanks. It definitely made it easier to manage.
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
