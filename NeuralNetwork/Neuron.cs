using System;
using System.Collections.Generic;
using System.Linq;

namespace RockPaperScissorsAI
{
    public class Connection
    {
        public INeuron _neuron;
        public double _weight;

        public Connection(INeuron neuron, double weight)
        {
            _neuron = neuron;
            _weight = weight;
        }

        public double Activation()
        {
            return _neuron.Activation() * _weight;
        }
    }

    public interface INeuron
    {
        public double Activation();
    }
        
    public class InputNeuron : INeuron
    {
        public double Value;

        public InputNeuron(double value)
        {
            Value = value;
        }

        public double Activation()
        {
            return Value;
        }
    }
    public class Neuron : INeuron
    {
        public readonly Connection[] Inputs;
        public Neuron(Connection[] inputs)
        {
            Inputs = inputs;
        }

        public double Activation()
        {
            
            return Utils.Sigmoid(WeightedSum());
        }

        public double WeightedSum()
        {
            return Inputs.Sum(input => input.Activation());
        }
        
        public void CorrectWeight(double error)
        {
            foreach (var connection in Inputs)
            {
                connection._weight -= 0.1 * error * connection._neuron.Activation();
            }   
        }
    }
}