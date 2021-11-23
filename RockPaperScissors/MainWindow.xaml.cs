using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace RockPaperScissorsAI
{
    enum Move
    {
        Rock,Paper,Scissors
    }

    enum Winner
    {
        Player1,Player2,Draw        
    }
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Move _player1Move = Move.Rock;
        private Move _player2Move = Move.Rock;
        private Trainer _trainer;
        public MainWindow()
        {
            InitializeComponent();
            Train();
        }

        private void SetResult(Winner result)
        {
            switch (result)
            {
                case Winner.Player1:
                    ResultPlayer1.IsChecked = true;
                    break;
                case Winner.Player2:
                    ResultPlayer2.IsChecked = true;
                    break;
                case Winner.Draw:
                    ResultDraw.IsChecked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }
        
        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            switch (_player1Move)
            {
                case Move.Rock:
                    switch (_player2Move)
                    {
                        case Move.Rock:
                            SetResult(Winner.Draw);
                            break;
                        case Move.Paper:
                            SetResult(Winner.Player2);
                            break;
                        case Move.Scissors:
                            SetResult(Winner.Player1);
                            break;
                    }
                    break;
                case Move.Paper:
                    switch (_player2Move)
                    {
                        case Move.Rock:
                            SetResult(Winner.Player1);
                            break;
                        case Move.Paper:
                            SetResult(Winner.Draw);
                            break;
                        case Move.Scissors:
                            SetResult(Winner.Player2);
                            break;
                    }
                    break;
                case Move.Scissors:
                    switch (_player2Move)
                    {
                        case Move.Rock:
                            SetResult(Winner.Player2);
                            break;
                        case Move.Paper:
                            SetResult(Winner.Player1);
                            break;
                        case Move.Scissors:
                            SetResult(Winner.Draw);
                            break;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var arr = new double[6];
            switch (_player1Move)
            {
                case Move.Rock:
                    arr[0] = 1;
                    break;
                case Move.Paper:
                    arr[1] = 1;
                    break;
                case Move.Scissors:
                    arr[2] = 1;
                    break;
            }
            
            switch (_player2Move)
            {
                case Move.Rock:
                    arr[3] = 1;
                    break;
                case Move.Paper:
                    arr[4] = 1;
                    break;
                case Move.Scissors:
                    arr[5] = 1;
                    break;
            }
            
            var prediction = _trainer.Predict(arr);
            var err1 = Math.Pow(prediction[0] -(ResultDraw.IsChecked == true ? 1 : 0), 2);
            var err2 = Math.Pow(prediction[1] - (ResultPlayer1.IsChecked == true ? 1 : 0), 2);
            var err3 = Math.Pow(prediction[2] - (ResultPlayer2.IsChecked == true ? 1 : 0), 2);
            var errAvg = (err1 + err2 + err3) / 3;

            Dispatcher.Invoke(() =>
            {
                NNResultDraw.Text = "Draw: " + prediction[0] + ", Error: " + err1;
                NNResultPlayer1.Text = "Player1: " + prediction[1] + ", Error: " + err2;
                NNResultPlayer2.Text = "Player2: " + prediction[2] + ", Error: " + err2;
                NNError.Text = "Error: " + errAvg;
            });
        }

        private async void Train()
        {
            await Task.Run(() =>
            {
                var (inputLayer, layers) = NeuralNetwork.CreateNetwork(new []{6, 9, 3});
                _trainer = new Trainer(inputLayer, layers);

                    var batch = new double[9][][];
                    var rnd = new Random();
                    batch[0] = new []{ new []{1d,0,0,1,0,0},new []{1d,0,0}};
                    batch[1] = new []{ new []{0d,1,0,0,1,0},new []{1d,0,0}};
                    batch[2] = new []{ new []{0d,0,1,0,0,1},new []{1d,0,0}};
                    batch[3] = new []{ new []{1d,0,0,0,0,1},new []{0d,1,0}};
                    batch[4] = new []{ new []{0d,1,0,1,0,0},new []{0d,1,0}};
                    batch[5] = new []{ new []{0d,0,1,0,1,0},new []{0d,1,0}};
                    batch[6] = new []{ new []{1d,0,0,0,1,0},new []{0d,0,1}};
                    batch[7] = new []{ new []{0d,1,0,0,0,1},new []{0d,0,1}};
                    batch[8] = new []{ new []{0d,0,1,1,0,0},new []{0d,0,1}};
                    for (var i = 0; i < 10000; i++)
                    {
                        _trainer.Train(batch.OrderBy(x => rnd.Next()).ToArray());
                        Dispatcher.Invoke(() => ButtonBase_OnClick(null, null));
                    }

            });
        }
        
        private void Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = (RadioButton)sender;
            Move move;
            switch (radioButton.Content)
            {
                case "Rock":
                    move = Move.Rock;
                    break;
                case "Paper":
                    move = Move.Paper;
                    break;
                case "Scissors":
                    move = Move.Scissors;
                    break;
                default:
                    return;
            }

            if (radioButton.GroupName == "Player1")
            {
                _player1Move = move;
            }
            else
            {
                _player2Move = move;
            }
        }

        private async void TrainClick(object sender, RoutedEventArgs e)
        {
            Train();
        }
    }
}