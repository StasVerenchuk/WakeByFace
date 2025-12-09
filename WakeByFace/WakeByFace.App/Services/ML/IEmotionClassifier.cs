using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeByFace.App.Services.ML
{
    public interface IEmotionClassifier
    {
        Task<EmotionPredictionResult> PredictAsync(Stream imageStream, CancellationToken ct = default);
    }
}
