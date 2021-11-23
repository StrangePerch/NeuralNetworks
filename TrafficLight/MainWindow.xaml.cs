using System;
using System.Collections.Generic;
using System.Globalization;
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
using RockPaperScissorsAI;

namespace TrafficLight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int Red = 1;
        public int Yellow = 0;
        public int Green = 0;
        public int Human = 0;
        public int Broken = 0;
        public int Car = 0;
        private Trainer trainer;
        public MainWindow()
        {
            InitializeComponent();
            Train();
        }
        
        private async void Train()
        {
            await Task.Run(() =>
            {   
                var (inputLayer, layers) = NeuralNetwork.CreateNetwork(new []{6,6,1});
                trainer = new Trainer(inputLayer, layers);

                var rnd = new Random(); 
                var batch = new double[19][][];
                batch[0] = new []{ new []{1d,0,0,0,0,0},new []{0d}};
                batch[1] = new []{ new []{0d,1,0,0,0,0},new []{0d}};
                batch[2] = new []{ new []{0d,0,1,0,0,0},new []{1d}};
                batch[3] = new []{ new []{1d,0,0,1,0,0},new []{0d}};
                batch[4] = new []{ new []{0d,1,0,1,0,0},new []{0d}};
                batch[5] = new []{ new []{0d,0,1,1,0,0},new []{0d}};
                batch[6] = new []{ new []{1d,0,0,0,0,1},new []{0d}};
                batch[7] = new []{ new []{0d,1,0,0,0,1},new []{0d}};
                batch[8] = new []{ new []{0d,0,1,0,0,1},new []{0d}};
                batch[9] = new []{ new []{1d,0,0,1,0,1},new []{0d}};
                batch[10] = new []{ new []{0d,1,0,1,0,1},new []{0d}};
                batch[11] = new []{ new []{0d,0,1,1,0,1},new []{0d}};
                batch[12] = new []{ new []{0d,0,1,1,1,1},new []{0d}};
                batch[13] = new []{ new []{1d,0,0,1,1,1},new []{0d}};
                batch[14] = new []{ new []{0d,1,0,1,1,1},new []{0d}};
                batch[15] = new []{ new []{0d,0,1,1,1,1},new []{0d}};
                batch[16] = new []{ new []{0d,0,1,0,1,0},new []{1d}};
                batch[17] = new []{ new []{0d,1,0,0,1,0},new []{1d}};
                batch[18] = new []{ new []{1d,0,0,0,1,0},new []{1d}};
                for (int i = 0; i < 100000; i++)
                {
                    trainer.Train(batch.OrderBy(x => rnd.Next()).ToArray());
                    Dispatcher.Invoke(Predict);
                }
            });
        }
        
        private void Predict_OnClick(object sender, RoutedEventArgs e)
        {
            Predict();
        }

        private void Predict()
        {
            var prediction = trainer.Predict(new double[] { Red, Yellow, Green, Human, Broken, Car});
            Prediction.Text = prediction[0].ToString(CultureInfo.CurrentCulture);
        }

        private void RedChecked(object sender, RoutedEventArgs e)
        {
            Red = 1;
            Yellow = 0;
            Green = 0;
        }

        private void YellowChecked(object sender, RoutedEventArgs e)
        {
            Red = 0;
            Yellow = 1;
            Green = 0;
        }

        private void GreenChecked(object sender, RoutedEventArgs e)
        {
            Red = 0;
            Yellow = 0;
            Green = 1;
        }

        private void HumanChecked(object sender, RoutedEventArgs e)
        {
            Human = 1;
        }

        private void HumanUnchecked(object sender, RoutedEventArgs e)
        {
            Human = 0;
        }

        private void BrokenChecked(object sender, RoutedEventArgs e)
        {
            Broken = 1;
        }

        private void BrokenUnchecked(object sender, RoutedEventArgs e)
        {
            Broken = 0;
        }

        private void CarUnchecked(object sender, RoutedEventArgs e)
        {
            Car = 0;
        }

        private void CarChecked(object sender, RoutedEventArgs e)
        {
            Car = 1;
        }
    }
}