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
                    sudokuBoard.ImportGrid(openFileDialog.FileName, handlers);
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
        public static bool IsTextNumeric(string text)
        {
            Regex regex = new Regex("[1-9]+"); //regex that matches allowed text
            return regex.IsMatch(text);
        }

    }
}
