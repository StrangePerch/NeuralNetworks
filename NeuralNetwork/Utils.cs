using System;

namespace RockPaperScissorsAI
{
    public static class Utils
    {
        public static double Sigmoid(double value) {
            var k = Math.Exp(-value);
            return 1 / (1.0f + k);
        }

        public static double Derivative(double value)
        {
            return value * (1.0 - value);
        }
    }
}