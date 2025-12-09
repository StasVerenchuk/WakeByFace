using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeByFace.App.Services.ML
{
    public class EmotionPredictionResult
    {
        /// <summary>
        /// Назва класу (емоції), яка має найбільшу ймовірність.
        /// </summary>
        public string Label { get; init; } = string.Empty;

        /// <summary>
        /// Вектор ймовірностей для всіх класів у тому ж порядку, що й у моделі.
        /// </summary>
        public float[] Probabilities { get; init; } = Array.Empty<float>();

        public override string ToString() => $"{Label} [{string.Join(", ", Probabilities.Select(p => p.ToString("0.000")))}]";
    }
}
