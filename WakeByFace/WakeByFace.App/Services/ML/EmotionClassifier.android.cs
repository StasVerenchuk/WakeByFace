#if ANDROID

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.Graphics;
using Java.Nio;
using Java.Lang;
using Microsoft.Maui.Storage;
using Xamarin.TensorFlow.Lite;

namespace WakeByFace.App.Services.ML
{
    public class EmotionClassifier : IEmotionClassifier, IAsyncDisposable
    {
        private readonly Interpreter _interpreter;

        private readonly int _inputWidth = 224;
        private readonly int _inputHeight = 224;

        // Порядок як у Python: ['happy', 'neutral', 'sad', 'surprise']
        private readonly string[] _labels = { "happy", "neutral", "sad", "surprise" };

        private const string ModelFileName = "emotion_mobilenetv2_cls.tflite";

        public EmotionClassifier()
        {
            // 1. Читаємо .tflite із MauiAsset (Resources/Raw)
            var assetStreamTask = FileSystem.OpenAppPackageFileAsync(ModelFileName);
            assetStreamTask.Wait();
            using var assetStream = assetStreamTask.Result;

            using var ms = new MemoryStream();
            assetStream.CopyTo(ms);
            var modelBytes = ms.ToArray();

            // 2. Кладемо модель у ByteBuffer
            ByteBuffer buffer = ByteBuffer.AllocateDirect(modelBytes.Length);
            buffer.Order(ByteOrder.NativeOrder());
            buffer.Put(modelBytes);
            buffer.Rewind();

            var options = new Interpreter.Options();
            options.SetNumThreads(2); // можна підібрати

            _interpreter = new Interpreter(buffer, options);
            _interpreter.AllocateTensors();
        }

        public async Task<EmotionPredictionResult> PredictAsync(
            Stream imageStream,
            CancellationToken ct = default)
        {
            if (imageStream is null)
                throw new ArgumentNullException(nameof(imageStream));

            using var ms = new MemoryStream();
            await imageStream.CopyToAsync(ms, ct);
            var bytes = ms.ToArray();

            using var bitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
            if (bitmap is null)
                throw new InvalidOperationException("Cannot decode image to Bitmap.");

            // Вхід у мережу готуємо повністю в окремому методі
            var inputBuffer = PreprocessBitmapToBuffer(bitmap);

            // ВИХІД: зубчастий масив [1][4]
            float[][] rawOutput = new float[1][];
            rawOutput[0] = new float[_labels.Length];

            // Обгортаємо у Java.Lang.Object
            Java.Lang.Object javaOutput = Java.Lang.Object.FromArray(rawOutput);

            // Запуск моделі
            _interpreter.Run(inputBuffer, javaOutput);

            // Повертаємося до C# масиву
            float[][] output = javaOutput.ToArray<float[]>();

            // Знаходимо максимальну ймовірність
            int bestIdx = 0;
            float bestVal = output[0][0];
            for (int i = 1; i < _labels.Length; i++)
            {
                if (output[0][i] > bestVal)
                {
                    bestVal = output[0][i];
                    bestIdx = i;
                }
            }

            // Копія ймовірностей
            var probs = new float[_labels.Length];
            for (int i = 0; i < _labels.Length; i++)
                probs[i] = output[0][i];

            return new EmotionPredictionResult
            {
                Label = _labels[bestIdx],
                Probabilities = probs
            };
        }

        /// <summary>
        /// Center-crop → resize до 224×224 → grayscale → дублікат у 3 канали
        /// + нормалізація x / 127.5 - 1 (як у MobileNetV2 preprocess_input).
        /// </summary>
        private ByteBuffer PreprocessBitmapToBuffer(Bitmap bitmap)
        {
            // 1. Center-crop до квадрата з оригінального кадру
            int srcWidth = bitmap.Width;
            int srcHeight = bitmap.Height;

            int size = System.Math.Min(srcWidth, srcHeight);
            int left = (srcWidth - size) / 2;
            int top = (srcHeight - size) / 2;

            using var cropped = Bitmap.CreateBitmap(bitmap, left, top, size, size);

            // 2. Масштабуємо до 224×224
            using var scaled = Bitmap.CreateScaledBitmap(cropped, _inputWidth, _inputHeight, true);

            int width = _inputWidth;
            int height = _inputHeight;

            int[] pixels = new int[width * height];
            scaled.GetPixels(pixels, 0, width, 0, 0, width, height);

            int floatSize = 4;
            var byteBuffer = ByteBuffer.AllocateDirect(width * height * 3 * floatSize);
            byteBuffer.Order(ByteOrder.NativeOrder());

            int pixelIndex = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int color = pixels[pixelIndex++];

                    int r = (color >> 16) & 0xFF;
                    int g = (color >> 8) & 0xFF;
                    int b = color & 0xFF;

                    // Grayscale (люмінанс)
                    float gray = 0.299f * r + 0.587f * g + 0.114f * b;

                    // Нормалізація, як у MobileNetV2 preprocess_input
                    float norm = gray / 127.5f - 1f;

                    // Дублюємо одне й те саме значення в R,G,B
                    byteBuffer.PutFloat(norm);
                    byteBuffer.PutFloat(norm);
                    byteBuffer.PutFloat(norm);
                }
            }

            byteBuffer.Rewind();
            return byteBuffer;
        }

        public ValueTask DisposeAsync()
        {
            try
            {
                _interpreter?.Close();
                _interpreter?.Dispose();
            }
            catch
            {
                // ігноруємо можливі помилки при звільненні
            }

            return ValueTask.CompletedTask;
        }
    }
}

#endif
