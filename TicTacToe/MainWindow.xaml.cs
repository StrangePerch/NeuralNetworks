using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;
using RockPaperScissorsAI;

namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly double[] _input = new double[9];
        private Trainer _trainer;
        private string _path = "../../../data.txt";
        private bool _onlyFull = false;
        public MainWindow()
        {
            InitializeComponent();
            var lines = new List<string>();
            var reader = new StreamReader(_path);
            while (!reader.EndOfStream)
            {
                lines.Add(reader.ReadLine());
            }
            reader.Close();
            var writer = new StreamWriter(_path, false);
            foreach (var line in lines.Distinct())
            {
                writer.WriteLine(line);
            }
            writer.Close();
        }

        private void HandleClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var index = int.Parse(button.Name.Last().ToString());
            switch (button.Content)
            {
                case "X":
                    button.Content = "O";
                    _input[index] = 2;
                    break;
                case "O":
                    button.Content = "";
                    _input[index] = 0;
                    break;
                default:
                    button.Content = "X";
                    _input[index] = 1;
                    break;
            }

            Predict();
        }

        private async void Train()
        {
            const int generations = 50000;
            await Task.Run(() =>
            {
                var (inputLayer, layers) = NeuralNetwork.CreateNetwork(new[] { 27, 20, 4 });
                _trainer = new Trainer(inputLayer, layers);

                var rnd = new Random();
                // var batch = BadIdea();
                var batch = ReadFromFile();
                // batch = batch.OrderBy(x => rnd.Next()).ToArray();
                // var x = batch.Where(a => (int)a[1][0] == 1).ToArray();
                // var o = batch.Where(a => (int)a[1][1] == 1).ToArray();
                // var draw = batch.Where(a => (int)a[1][2] == 1).ToArray();
                // var error = batch.Where(a => (int)a[1][3] == 1).ToArray();
                // for (var i = 0; i < generations; i++)
                // {
                //     const int size = 5;
                //     var miniBatch = new double[size * 4][][];
                //     
                //     x.OrderBy(a => rnd.Next()).Take(size).ToArray().CopyTo(miniBatch, 0);
                //     o.OrderBy(a => rnd.Next()).Take(size).ToArray().CopyTo(miniBatch, size);
                //     draw.OrderBy(a => rnd.Next()).Take(size).ToArray().CopyTo(miniBatch, 2 * size);
                //     error.OrderBy(a => rnd.Next()).Take(size).ToArray().CopyTo(miniBatch, 3 * size);
                //
                //     _trainer.Train(miniBatch.OrderBy(a => rnd.Next()).ToArray());
                //
                //     Dispatcher.Invoke(() => ProgressBar.Value = i / (generations / 100d));
                //     Predict();
                // }
                
                // batch = batch.OrderBy(x => rnd.Next()).ToArray();
                //
                // for (var i = 0; i < generations; i++)
                // {
                //     if (batch.Length < 32)
                //     {
                //         batch = ReadFromFile().OrderBy(x => rnd.Next()).ToArray();
                //     }
                //     _trainer.Train(batch.Take(32).ToArray());
                //     batch = batch.Skip(32).ToArray();
                //     if (i % 3 == 0)
                //     {
                //         Dispatcher.Invoke(() => ProgressBar.Value = i / (generations / 100d));
                //         Predict();
                //     }
                // }
                
                for (var i = 0; i < generations; i++)
                {
                    _trainer.Train(batch.OrderBy(x => rnd.Next()).Take(100).ToArray());
                    Dispatcher.Invoke(() => ProgressBar.Value = i / (generations / 100d));
                    Predict();
                }
            });
        }

        private void Predict()
        {
            if (TrainMenu.Visibility != Visibility.Visible) return;
            var inputs = new double[27];
            for (int i = 0; i < 9; i++)
            {
                inputs[(int)(i * 3 + _input[i])] = 1;
            }
            var prediction = _trainer.Predict(inputs);
            Dispatcher.Invoke(() =>
            {
                ResultX.Text = "X Win: " + prediction[0];
                ResultO.Text = "O Win: " + prediction[1];
                ResultDraw.Text = "Draw: " + prediction[2];
                ResultError.Text = "Error: " + prediction[3];
            });
        }
        
        private double[][][] ReadFromFile()
        {
            var dataset = new List<double[][]>();

            var reader = new StreamReader(_path);

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line == null) break;
                var split = line.Split(';');
                var inputStr = split[0];
                var resultStr = split[1];
                
                var inputsStr = inputStr.Split(',');
                var inputs = new double[(inputsStr.Length - 1) * 3];

                for (int i = 0; i < inputsStr.Length - 1; i++)
                {
                    var val = double.Parse(inputsStr[i]);
                    inputs[(int)(i * 3 + val)] = 1;
                }

                var results = new List<double>();
                foreach (var r in resultStr.Split(','))
                {
                    if (r == "") break;
                    results.Add(double.Parse(r));
                }
                dataset.Add(new []{inputs.ToArray(), results.ToArray()});
            }
            reader.Close();
            return dataset.ToArray();
        }
        
        private double[][][] BadIdea()
        {
            var batch = new double[32][][];

            batch[0] = new[]
            {
                new[]
                {
                    1, 0, 0,
                    2, 1, 0,
                    0, 2, 1d
                },
                new[] { 1d, 0, 0, 0 }
            };
            batch[1] = new[]
            {
                new[]
                {
                    0, 2, 1,
                    0, 1, 0,
                    1, 2, 0d
                },
                new[] { 1d, 0, 0, 0 }
            };

            batch[2] = new[]
            {
                new[]
                {
                    1, 2, 0,
                    1, 0, 2,
                    1, 0, 0d
                },
                new[] { 1d, 0, 0, 0 }
            };

            batch[3] = new[]
            {
                new[]
                {
                    0, 1, 2,
                    2, 1, 0,
                    0, 1, 0d
                },
                new[] { 1d, 0, 0, 0 }
            };

            batch[4] = new[]
            {
                new[]
                {
                    0, 0, 1,
                    2, 0, 1,
                    0, 2, 1d
                },
                new[] { 1d, 0, 0, 0 }
            };

            batch[5] = new[]
            {
                new[]
                {
                    1, 1, 1,
                    0, 2, 0,
                    0, 2, 0d
                },
                new[] { 1d, 0, 0, 0 }
            };

            batch[6] = new[]
            {
                new[]
                {
                    0, 2, 0,
                    1, 1, 1,
                    2, 0, 0d
                },
                new[] { 1d, 0, 0, 0 }
            };

            batch[7] = new[]
            {
                new[]
                {
                    0, 0, 0,
                    2, 0, 2,
                    1, 1, 1d
                },
                new[] { 1d, 0, 0, 0 }
            };

            batch[8] = new[]
            {
                new[]
                {
                    2, 0, 0,
                    1, 2, 0,
                    0, 1, 2d
                },
                new[] { 0d, 1, 0, 0 }
            };
            batch[9] = new[]
            {
                new[]
                {
                    0, 1, 2,
                    0, 2, 1,
                    2, 0, 0d
                },
                new[] { 0d, 1, 0, 0 }
            };

            batch[10] = new[]
            {
                new[]
                {
                    2, 0, 1,
                    2, 1, 0,
                    2, 0, 0d
                },
                new[] { 0d, 1, 0, 0 }
            };

            batch[11] = new[]
            {
                new[]
                {
                    1, 2, 0,
                    0, 2, 0,
                    1, 2, 0d
                },
                new[] { 0d, 1, 0, 0 }
            };

            batch[12] = new[]
            {
                new[]
                {
                    0, 0, 2,
                    1, 1, 2,
                    0, 0, 2d
                },
                new[] { 0d, 1, 0, 0 }
            };

            batch[13] = new[]
            {
                new[]
                {
                    2, 2, 2,
                    0, 1, 0,
                    1, 0, 0d
                },
                new[] { 0d, 1, 0, 0 }
            };

            batch[14] = new[]
            {
                new[]
                {
                    0, 1, 0,
                    2, 2, 2,
                    1, 0, 0d
                },
                new[] { 0d, 1, 0, 0 }
            };

            batch[15] = new[]
            {
                new[]
                {
                    1, 0, 0,
                    0, 1, 0,
                    2, 2, 2d
                },
                new[] { 0d, 1, 0, 0 }
            };

            batch[16] = new[]
            {
                new[]
                {
                    0, 0, 0,
                    0, 0, 0,
                    0, 0, 0d
                },
                new[] { 0d, 0, 0, 1 }
            };

            batch[17] = new[]
            {
                new[]
                {
                    1, 1, 1,
                    0, 0, 0,
                    0, 0, 0d
                },
                new[] { 0d, 0, 0, 1 }
            };

            batch[18] = new[]
            {
                new[]
                {
                    1, 2, 0,
                    1, 2, 0,
                    1, 2, 0d
                },
                new[] { 0d, 0, 0, 1 }
            };

            batch[19] = new[]
            {
                new[]
                {
                    0, 0, 0,
                    0, 0, 0,
                    0, 0, 0d
                },
                new[] { 0d, 0, 0, 1 }
            };

            batch[20] = new[]
            {
                new[]
                {
                    1, 0, 1,
                    0, 2, 0,
                    0, 1, 0d
                },
                new[] { 0d, 0, 0, 1 }
            };

            batch[20] = new[]
            {
                new[]
                {
                    2, 0, 1,
                    1, 2, 2,
                    2, 1, 1d
                },
                new[] { 0d, 0, 0, 1 }
            };

            batch[21] = new[]
            {
                new[]
                {
                    2, 0, 1,
                    1, 0, 2,
                    2, 1, 1d
                },
                new[] { 0d, 0, 0, 1 }
            };


            batch[22] = new[]
            {
                new[]
                {
                    2, 0, 1,
                    1, 0, 2,
                    0, 1, 1d
                },
                new[] { 0d, 0, 0, 1 }
            };

            batch[23] = new[]
            {
                new[]
                {
                    2, 0, 1,
                    1, 0, 2,
                    0, 0, 0d
                },
                new[] { 0d, 0, 0, 1 }
            };

            batch[24] = new[]
            {
                new[]
                {
                    1, 1, 2,
                    2, 1, 1,
                    1, 2, 2d
                },
                new[] { 0d, 0, 1, 0 }
            };

            batch[25] = new[]
            {
                new[]
                {
                    1, 2, 2,
                    2, 1, 1,
                    1, 2, 1d
                },
                new[] { 0d, 0, 1, 0 }
            };

            batch[26] = new[]
            {
                new[]
                {
                    1, 1, 2,
                    2, 1, 1,
                    2, 2, 1d
                },
                new[] { 0d, 0, 1, 0 }
            };

            batch[27] = new[]
            {
                new[]
                {
                    2, 1, 2,
                    2, 2, 1,
                    1, 1, 1d
                },
                new[] { 0d, 0, 1, 0 }
            };

            batch[28] = new[]
            {
                new[]
                {
                    1, 1, 2,
                    2, 2, 1,
                    1, 1, 2d
                },
                new[] { 0d, 0, 1, 0 }
            };

            batch[29] = new[]
            {
                new[]
                {
                    1, 2, 2,
                    2, 1, 1,
                    1, 2, 1d
                },
                new[] { 0d, 0, 1, 0 }
            };

            batch[30] = new[]
            {
                new[]
                {
                    2, 1, 1,
                    2, 2, 1,
                    1, 2, 1d
                },
                new[] { 0d, 0, 1, 0 }
            };

            batch[31] = new[]
            {
                new[]
                {
                    2, 1, 1,
                    2, 2, 1,
                    1, 2, 1d
                },
                new[] { 0d, 0, 1, 0 }
            };
            return batch;
        }

        private void ToTrainMenu(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Hidden;
            TrainMenu.Visibility = Visibility.Visible;
            DataMenu.Visibility = Visibility.Hidden;
            
            Train();
        }

        private void ToDataMenu(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Hidden;
            TrainMenu.Visibility = Visibility.Hidden;
            DataMenu.Visibility = Visibility.Visible;
            GenerateField();
        }

        private void GenerateField()
        {
            var rand = new Random();
            
            for (var i = 0; i < 9; i++)
            {
                var button = (Button)FindName("Button" + i);
                if(_onlyFull)
                    _input[i] = rand.Next() % 2 + 1;
                else
                    _input[i] = rand.Next() % 3;
                if (button != null) button.Content = ToChar(_input[i]);
            }

            var diff = _input.Count(i => (int)i == 1) - _input.Count(i => (int)i == 2);
            var emptyCount = _input.Count(i => (int)i == 0);
            DrawButton.Visibility = emptyCount > 0 ? Visibility.Hidden : Visibility.Visible;
            if ((!_onlyFull && diff is > 1 or < 0) 
                || (_onlyFull && diff != 1) 
                || (emptyCount > 4))
            {
                WriteData(new[] {0,0,0,1d});
            }
        }

        private string ToChar(double i)
        {
            return i switch
            {
                1 => "X",
                2 => "O",
                _ => ""
            };
        }

        private void XWon_Click(object sender, RoutedEventArgs e)
        {
            WriteData(new[] {1,0,0,0d});
        }

        private void OWon_Click(object sender, RoutedEventArgs e)
        {
            WriteData(new[] {0,1,0,0d});
        }

        private void Draw_Click(object sender, RoutedEventArgs e)
        {
            WriteData(new[] {0,0,1,0d});
        }

        private void Error_Click(object sender, RoutedEventArgs e)
        {
            WriteData(new[] {0,0,0,1d});
        }

        private void WriteData(IEnumerable<double> result)
        {
            var writer = new StreamWriter(_path, true);
            var toWrite = new StringBuilder();
            foreach (var input in _input)
            {
                toWrite.Append(input + ",");
            }
            toWrite.Append(';');
            foreach (var res in result)
            {
                toWrite.Append(res + ",");
            }
            writer.WriteLine(toWrite.ToString());
            writer.Close();
            GenerateField();
        }

        private void CheckBoxClick(object sender, RoutedEventArgs e)
        {
            var checkbox = (CheckBox)sender;
            _onlyFull = checkbox.IsChecked == true;
            GenerateField();
        }
    }
}