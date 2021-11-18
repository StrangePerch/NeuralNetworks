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
                var (inputLayer, layers) = NeuralNetwork.CreateNetwork(new []{4,1});
                trainer = new Trainer(inputLayer, layers);

                var rnd = new Random(); 
                var batch = new double[6][][];
                batch[0] = new []{ new []{1d,0,0,0},new []{0d}};
                batch[1] = new []{ new []{0d,1,0,0},new []{0d}};
                batch[2] = new []{ new []{0d,0,1,0},new []{1d}};
                batch[3] = new []{ new []{1d,0,0,1},new []{0d}};
                batch[4] = new []{ new []{0d,1,0,1},new []{0d}};
                batch[5] = new []{ new []{0d,0,1,1},new []{0d}};
                for (int i = 0; i < 10000; i++)
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
            var prediction = trainer.Predict(new double[] { Red, Yellow, Green, Human});
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
    }
}