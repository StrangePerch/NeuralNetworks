using System;
using System.Collections.Generic;
using System.Linq;

namespace RockPaperScissorsAI
{
    public class NeuralNetwork
    {
        public static Dictionary<string, INeuron>[] BuildNetwork(int[] input)
        {
            var inputLayer = new Dictionary<string, INeuron>
            {
                ["firstRock"] = new InputNeuron(input[0]),
                ["firstPaper"] = new InputNeuron(input[1]),
                ["firstScissors"] = new InputNeuron(input[2]),
                ["secondRock"] = new InputNeuron(input[3]),
                ["secondPaper"] = new InputNeuron(input[4]),
                ["secondScissors"] = new InputNeuron(input[5])
            };

            var firstLayer = new Dictionary<string, INeuron>
            {
                ["bothRock"] = new Neuron( new Connection[]{
                    new (inputLayer["firstRock"], 0.4),
                    new (inputLayer["firstPaper"], 0),
                    new (inputLayer["firstScissors"], 0),
                    new (inputLayer["secondRock"], 0.4),
                    new (inputLayer["secondPaper"], 0),
                    new (inputLayer["secondScissors"], 0)
                }),
                ["bothPaper"] = new Neuron( new Connection[]{
                    new (inputLayer["firstRock"], 0),    
                    new (inputLayer["firstPaper"], 0.4),
                    new (inputLayer["firstScissors"], 0),
                    new (inputLayer["secondRock"], 0),
                    new (inputLayer["secondPaper"], 0.4),
                    new (inputLayer["secondScissors"], 0)
                }),
                ["bothScissors"] = new Neuron( new Connection[]{
                    new (inputLayer["firstRock"], 0),
                    new (inputLayer["firstPaper"], 0),
                    new (inputLayer["firstScissors"], 0.4),
                    new (inputLayer["secondRock"], 0),
                    new (inputLayer["secondPaper"], 0),
                    new (inputLayer["secondScissors"], 0.4)
                }),
                ["rockPaper"] = new Neuron( new Connection[]{
                    new (inputLayer["firstRock"], 0.4),
                    new (inputLayer["firstPaper"], 0),
                    new (inputLayer["firstScissors"], 0),
                    new (inputLayer["secondRock"], 0),
                    new (inputLayer["secondPaper"], 0.4),
                    new (inputLayer["secondScissors"], 0)
                }),
                ["rockScissors"] = new Neuron( new Connection[]{
                    new (inputLayer["firstRock"], 0.4),
                    new (inputLayer["firstPaper"], 0),
                    new (inputLayer["firstScissors"], 0),
                    new (inputLayer["secondRock"], 0),
                    new (inputLayer["secondPaper"], 0),
                    new (inputLayer["secondScissors"], 0.4)
                }),
                ["paperRock"] = new Neuron( new Connection[]{
                    new (inputLayer["firstRock"], 0),
                    new (inputLayer["firstPaper"], 0.4),
                    new (inputLayer["firstScissors"], 0),
                    new (inputLayer["secondRock"], 0.4),
                    new (inputLayer["secondPaper"], 0),
                    new (inputLayer["secondScissors"], 0)
                }),
                ["paperScissors"] = new Neuron( new Connection[]{
                    new (inputLayer["firstRock"], 0),
                    new (inputLayer["firstPaper"], 0.4),
                    new (inputLayer["firstScissors"], 0),
                    new (inputLayer["secondRock"], 0),
                    new (inputLayer["secondPaper"], 0),
                    new (inputLayer["secondScissors"], 0.4)
                }),
                ["scissorsRock"] = new Neuron( new Connection[]{
                    new (inputLayer["firstRock"], 0),
                    new (inputLayer["firstPaper"], 0),
                    new (inputLayer["firstScissors"], 0.4),
                    new (inputLayer["secondRock"], 0.4),
                    new (inputLayer["secondPaper"], 0),
                    new (inputLayer["secondScissors"], 0)
                }),
                ["scissorsPaper"] = new Neuron( new Connection[]{
                    new (inputLayer["firstRock"], 0),
                    new (inputLayer["firstPaper"], 0),
                    new (inputLayer["firstScissors"], 0.4),
                    new (inputLayer["secondRock"], 0),
                    new (inputLayer["secondPaper"], 0.4),
                    new (inputLayer["secondScissors"], 0)
                }),
            };
            
            var output = new Dictionary<string, INeuron>
            {
                ["draw"] = new Neuron( new Connection[]{
                    new (firstLayer["bothRock"], 1),
                    new (firstLayer["bothPaper"], 1),
                    new (firstLayer["bothScissors"], 1),
                    new (firstLayer["rockPaper"], 0),
                    new (firstLayer["rockScissors"], 0),
                    new (firstLayer["paperRock"], 0),
                    new (firstLayer["paperScissors"], 0),
                    new (firstLayer["scissorsRock"], 0),
                    new (firstLayer["scissorsPaper"], 0),
                }),
                ["firstWin"] = new Neuron( new Connection[]{
                    new (firstLayer["bothRock"], 0),
                    new (firstLayer["bothPaper"], 0),
                    new (firstLayer["bothScissors"], 0),
                    new (firstLayer["rockPaper"], 0),
                    new (firstLayer["rockScissors"], 1),
                    new (firstLayer["paperRock"], 1),
                    new (firstLayer["paperScissors"], 0),
                    new (firstLayer["scissorsRock"], 0),
                    new (firstLayer["scissorsPaper"], 1),
                }),
                ["secondWin"] = new Neuron( new Connection[]{
                    new (firstLayer["bothRock"], 0),
                    new (firstLayer["bothPaper"], 0),
                    new (firstLayer["bothScissors"], 0),
                    new (firstLayer["rockPaper"], 1),
                    new (firstLayer["rockScissors"], 0),
                    new (firstLayer["paperRock"], 0),
                    new (firstLayer["paperScissors"], 1),
                    new (firstLayer["scissorsRock"], 1),
                    new (firstLayer["scissorsPaper"], 0),
                })
            };

            return new []{inputLayer, firstLayer, output };
        }

        public static (InputNeuron[], Neuron[][]) CreateNetwork(int[] sizes)
        {
            var inputLayer = new InputNeuron[sizes[0]];
            var layers = new Neuron[sizes.Length - 1][];
            var prevLayer = Array.Empty<INeuron>();
            var rand = new Random();
            for (var i = 0; i < sizes.Length; i++)
            {
                if (i == 0)
                {
                    for (var c = 0; c < sizes[i]; c++)
                    {
                        inputLayer[c] = new InputNeuron(0);
                    }

                    prevLayer = inputLayer.Cast<INeuron>().ToArray();
                }
                else
                {
                    layers[i - 1] = new Neuron[sizes[i]];
                    for (var j = 0; j < sizes[i]; j++)
                    {
                        layers[i - 1][j] = new Neuron(prevLayer.Select(
                            neuron => new Connection(neuron, rand.Next() % 1000 / 1000f)).ToArray());
                    }

                    prevLayer = layers[i - 1].Cast<INeuron>().ToArray();
                }
            }
        
            return (inputLayer, layers);
        }
    }
}