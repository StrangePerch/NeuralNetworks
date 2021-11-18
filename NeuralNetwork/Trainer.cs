using System;
using System.Linq;
using System.Threading.Tasks;

namespace RockPaperScissorsAI
{
    public class Trainer
    {
        public InputNeuron[] Inputs;
        public Neuron[][] Layers;

        public Trainer(InputNeuron[] inputs, Neuron[][] layers)
        {
            Layers = layers;
            Inputs = inputs;
        }

        public void Train(double[][][] batch)
        {
            var errors = new double[Layers.Length][][];
            
            for (var b = 0; b < batch.Length; b++)
            {
                var test = batch[b];
                var inputs = test[0];
                var targets = test[1];

                for (var j = 0; j < Inputs.Length; j++)
                {
                    Inputs[j].Value = inputs[j];
                }
                TrainOne(b, batch.Length, errors, targets);
            }
            
            for (var k = 0; k < batch.Length; k++)
            {
                for (var i = 0; i < errors.Length; i++)
                {
                    for (var j = 0; j < errors[i].Length; j++)
                    {
                        for (var u = 0; u < Inputs.Length; u++)
                        {
                            Inputs[u].Value = batch[k][0][u];
                        }
                        Layers[i][j].CorrectWeight(errors[i][j][k]);
                    }
                }
            }
           
        }

        private void TrainOne(int batchIndex, int batchLength, double[][][] errors, double[] targets)
        {
            for (var i = Layers.Length - 1; i >= 0; i--)
            {
                var layer = Layers[i];
                if (batchIndex == 0)
                    errors[i] = new double[layer.Length][];
                if (i == Layers.Length - 1) // OUTPUT LAYER
                {
                    for (var j = 0; j < layer.Length; j++)
                    {
                        var activation = layer[j].Activation();
                        var delta = (activation - targets[j]) * Utils.Derivative(activation);
                        if (batchIndex == 0)
                            errors[i][j] = new double[batchLength];
                        errors[i][j][batchIndex] = delta;
                    }
                }
                else //HIDDEN LAYERS
                {
                    for (var j = 0; j < layer.Length; j++)
                    {
                        var activation = layer[j].Activation();
                        var error = 0d;
                        for (var k = 0; k < Layers[i + 1].Length; k++)
                        {
                            var connection = Layers[i + 1][k].Inputs[j];
                            var outputDelta = errors[i + 1][k][batchIndex];
                            error += connection._weight * outputDelta;
                        }

                        var delta = (activation * error) * Utils.Derivative(activation);
                        if (batchIndex == 0)
                            errors[i][j] = new double[batchLength];
                        errors[i][j][batchIndex] = delta;
                    }
                }
            }
        }

        public double[] Predict(double[] inputs)
        {
            for (var j = 0; j < Inputs.Length; j++)
            {
                Inputs[j].Value = inputs[j];
            }

            var result = new double[Layers.Last().Length];
            for (var i = 0; i < Layers.Last().Length; i++)
            {
                result[i] = Layers.Last()[i].Activation();
            }

            return result;
        }
    }
}