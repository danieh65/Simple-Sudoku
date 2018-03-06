using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

namespace Sudoku2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // REVIEW: unclear what Utility is supposed to do?
        // RESPONSE: this object was in the cs file before I started working on it, so I didn't touch it. It is unused and should be deleted.
        public object Utility { get; private set; }
        SudokuBoard sudokuBoard;
        SudokuBorder sudokuBorder;
        PossibleValuesDisplay possibleValues;

        Handlers handlers;
        public MainWindow()
        {
            InitializeComponent();

            sudokuBoard = new SudokuBoard(sudokuGrid);
            sudokuBorder = new SudokuBorder(sudokuBorders);
            possibleValues = new PossibleValuesDisplay(sudokuBoard, possibleValuesGrid);
            // REVIEW: it was a good idea to separate the handler logic from the
            // rest of the logic. I need to do something like this.
            // RESPONSE: thanks! I wanted to separate as much as possible so that I was never referencing global variables except in MainWindow.
            //      This did result in some weird interdependencies though.
            handlers = new Handlers(sudokuBoard, possibleValues);

            sudokuBoard.InitializeGrid(handlers);
        }
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog { Filter = "Text file (*.txt)|*.txt" };
            if (saveFileDialog.ShowDialog() == true)
                sudokuBoard.ExportGrid(saveFileDialog.FileName);
        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Loading a new puzzle will erase the current board. Are you sure you want to do this?", "Load Puzzle Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    newButton.Content = "New";
                    sudokuBoard.ImportGrid(openFileDialog.FileName, handlers);
                    // REVIEW: I like that it checks the board on import
                    // and notifies the user if the puzzle is already solved/invalid.
                    // RESPONSE: Thank you. I wanted to, on top of indicating the status with color, make the state of the board clear to the user 
                    if (sudokuBoard.IsSolved)
                        MessageBox.Show("Solved puzzle loaded successfully!");
                    else if (!sudokuBoard.IsValid)
                    {
                        sudokuBoard.MarkImportBoardInvalid();
                        MessageBox.Show("Puzzle is invalid!");
                    }
                    else MessageBox.Show("Puzzle loaded successfully!");
                }
            }
        }

        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            // REVIEW: I like the reuse of the new button for setting the initial values.
            // The code here is a bit unclear, however, in this intention.
            // Possibly making two handlers and toggling between them would be clearer?
            // RESPONSE: I feel like that would be a clearer option. I think working with a switch-case statement (or perhaps enum) would be the easiest and clearest, 
            //      but I didn't consider this; I figured that an if-else statement would suffice because there are only two states of the button.
            if (newButton.Content.Equals("New"))
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Creating a new puzzle will erase the current board. Are you sure you want to do this?", "New Puzzle Confirmation", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    sudokuBoard.InitializeGrid(handlers);
                    newButton.Content = "Start";
                }
            }
            else
            {
                sudokuBoard.CommitCellValues();
                newButton.Content = "New";
            }
        }
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
        // REVIEW: seems to be redundant from the handler class
        // RESPONSE: it is redundant from the handler class (as there are 0 references). I forgot to delete this when migrating a chunk of code.
        public static bool IsTextNumeric(string text)
        {
            Regex regex = new Regex("[1-9]+"); //regex that matches allowed text
            return regex.IsMatch(text);
        }

    }
}
