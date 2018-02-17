using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Sudoku2
{
    public class SudokuBoard
    {
        Grid grid;
        public SudokuBoard(Grid g)
        {
            grid = g;
        }
        #region Grid Actions
        public void InitializeGrid(Handlers handlers)
        {
            {
                grid.Children.Clear();
                grid.ColumnDefinitions.Clear();
                grid.RowDefinitions.Clear();
                for (int i = 0; i < 9; i++)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    for (int j = 0; j < 9; j++)
                    {
                        RowDefinition rowDefinition = new RowDefinition
                        {
                            Height = new GridLength(0.5, GridUnitType.Star)
                        };
                        grid.RowDefinitions.Add(rowDefinition);
                        TextBox txt = new TextBox();
                        txt.PreviewTextInput += handlers.NumericOnly;
                        txt.MaxLength = 1;
                        txt.FontSize = 24;
                        txt.FontWeight = FontWeights.Normal;
                        txt.SetBinding(TextBox.HeightProperty, new Binding()
                        {
                            Path = new PropertyPath("ActualWidth"),
                            Source = txt
                        });
                        
                        txt.AddHandler(TextBox.TextChangedEvent, new RoutedEventHandler(handlers.textBox_Changed));
                        txt.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler(handlers.textBox_GotFocus));
                        txt.AddHandler(TextBox.LostFocusEvent, new RoutedEventHandler(handlers.textBox_LostFocus));
                        txt.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(handlers.textBox_Pasted));
                        txt.HorizontalContentAlignment = HorizontalAlignment.Center;
                        txt.VerticalContentAlignment = VerticalAlignment.Center;
                        txt.HorizontalAlignment = HorizontalAlignment.Stretch;
                        txt.VerticalAlignment = VerticalAlignment.Stretch;
                        Grid.SetColumn(txt, i);
                        Grid.SetRow(txt, j);
                        grid.Children.Add(txt);
                    }
                }
            }
        }
        public void ExportGrid(string filePath)
        {
            List<List<int?>> gridList;
            gridList = new List<List<int?>>(9);
            for (int i = 0; i < gridList.Capacity; i++)
                gridList.Add(new List<int?>(new int?[9]));

            foreach (var textBox in grid.Children.OfType<TextBox>())
                gridList[Grid.GetRow(textBox)][Grid.GetColumn(textBox)] = int.TryParse(textBox.Text, out int n) ? (int?)n : null;


            List<string> rows = new List<string>();
            foreach (List<int?> gridRow in gridList)
            {
                var row = gridRow.Select(r => r == null ? "X" : r.ToString()).ToList();
                row.Insert(6, "\t");
                row.Insert(3, "\t");
                rows.Add(String.Join("", row));
            }
            rows.Insert(6, "");
            rows.Insert(3, "");

            File.WriteAllText(filePath, String.Join("\n", rows));
        }
        public void ImportGrid(string filePath, Handlers handlers)
        {
            List<List<int?>> gridList;
            gridList = new List<List<int?>>(9);
            for (int i = 0; i < gridList.Capacity; i++)
                gridList.Add(new List<int?>(new int?[9]));

            List<string> lines = new List<string>();
            InitializeGrid(handlers);
            lines = File.ReadLines(filePath).Where(r => r.Any(x => !char.IsWhiteSpace(x))).ToList();
            for (int i = 0; i < lines.Count(); i++)
            {
                var line = lines[i].Where(r => !char.IsWhiteSpace(r)).ToList();
                for (int j = 0; j < line.Count; j++)
                {
                    gridList[i][j] = char.IsDigit(line[j]) ? (int?)int.Parse(line[j].ToString()) : null;
                    TextBox currentCell = GetCellAtPosition(i, j);
                    currentCell.Text = gridList[i][j].ToString();
                    if (gridList[i][j] != null)
                    {
                        currentCell.IsReadOnly = true;
                        currentCell.FontWeight = FontWeights.Bold;
                    }
                    else
                    {
                        currentCell.IsReadOnly = false;
                        currentCell.FontWeight = FontWeights.Normal;
                    }
                }
            }
        }
        public void MarkImportBoardInvalid()
        {
            foreach (var cell in grid.Children.OfType<TextBox>())
            {
                if (!IsCellValid(cell) && cell.Text.Length > 0) cell.Background = Brushes.DarkRed;
                cell.IsReadOnly = true;
            }
        }
        public void MarkBoardAsCompleted()
        {
            foreach (TextBox cell in grid.Children.OfType<TextBox>())
            {
                cell.Background = Brushes.Green;
                cell.IsReadOnly = true;
            }
        }
        public void CommitCellValues()
        {
            foreach (var cell in grid.Children.OfType<TextBox>())
            {
                if (cell.Text.Length > 0)
                {
                    cell.IsReadOnly = true;
                    cell.FontWeight = FontWeights.Bold;
                }
                else
                {
                    cell.IsReadOnly = false;
                    cell.FontWeight = FontWeights.Normal;
                }

            }
        }
        #endregion
        #region Board Properties
        public bool IsSolved => grid.Children.OfType<TextBox>().All(r => r.Text.Length > 0 && IsCellValid(r));
        public bool IsValid => grid.Children.OfType<TextBox>().All(r => IsCellValid(r));
        #endregion
        TextBox GetCellAtPosition(int r, int c)
        {
            foreach (TextBox cell in grid.Children.OfType<TextBox>())
                if (Grid.GetRow(cell) == r && Grid.GetColumn(cell) == c)
                    return cell;
            return null;
        }
        public bool IsCellValid(TextBox cell)
        {
            if (cell.Text.Length == 0) return true;
            if (int.TryParse(cell.Text, out var n))
                return CountValues(cell)[n - 1] < 1;
            return false;
        }
        public int[] CountValues(TextBox selectedCell)
        {
            int[] result = new int[9];
            var currentRowElements = GetIntersectingRowCells(selectedCell);
            var currentColumnElements = GetIntersectingColumnCells(selectedCell);
            var currentBoxElements = GetIntersectingBoxCells(selectedCell);
            for (int i = 0; i < 9; i++)
            {
                result[i] = Math.Max(result[i], currentRowElements.Where(r => int.TryParse((r).Text, out var n) && n == i + 1).Count());
                result[i] = Math.Max(result[i], currentColumnElements.Where(r => int.TryParse((r).Text, out var n) && n == i + 1).Count());
                result[i] = Math.Max(result[i], currentBoxElements.Where(r => int.TryParse((r).Text, out var n) && n == i + 1).Count());
            }
            return result;
        }
        
        public IEnumerable<TextBox> GetIntersectingCells(TextBox selectedCell)
        {
            foreach (TextBox currentCell in grid.Children.OfType<TextBox>())
            {
                if (!currentCell.Equals(selectedCell))
                {
                    if (Grid.GetRow(currentCell) == Grid.GetRow(selectedCell)
                        || Grid.GetColumn(currentCell) == Grid.GetColumn(selectedCell)
                        || (Grid.GetRow(currentCell) / 3 == Grid.GetRow(selectedCell) / 3 && Grid.GetColumn(currentCell) / 3 == Grid.GetColumn(selectedCell) / 3))
                        yield return currentCell;
                }
            }
        }
        public IEnumerable<TextBox> GetIntersectingRowCells(TextBox selectedCell)
        {
            foreach (TextBox currentCell in grid.Children.OfType<TextBox>())
            {
                if (!currentCell.Equals(selectedCell))
                {
                    if (Grid.GetRow(currentCell) == Grid.GetRow(selectedCell))
                        yield return currentCell;
                }
            }
        }
        public IEnumerable<TextBox> GetIntersectingColumnCells(TextBox selectedCell)
        {
            foreach (TextBox currentCell in grid.Children.OfType<TextBox>())
            {
                if (!currentCell.Equals(selectedCell))
                {
                    if (Grid.GetColumn(currentCell) == Grid.GetColumn(selectedCell))
                        yield return currentCell;
                }
            }
        }
        public IEnumerable<TextBox> GetIntersectingBoxCells(TextBox selectedCell)
        {
            foreach (TextBox currentCell in grid.Children.OfType<TextBox>())
            {
                if (!currentCell.Equals(selectedCell))
                {
                    if ((Grid.GetRow(currentCell) / 3 == Grid.GetRow(selectedCell) / 3 && Grid.GetColumn(currentCell) / 3 == Grid.GetColumn(selectedCell) / 3))
                        yield return currentCell;
                }
            }
        }
    }
}
