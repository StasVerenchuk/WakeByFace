import tensorflow as tf

model_path = "Emotion_mobilenetv2_4cls.h5"
model = tf.keras.models.load_model(model_path, compile=False)

print("Keras model loaded")
print("Input shape:", model.input_shape)
print("Output shape", model.output_shape)

converter = tf.lite.TFLiteConverter.from_keras_model(model)

tflite_model = converter.convert()

tflite_path = "emotion_mobilenetv2_cls.tflite"
with open(tflite_path, "wb") as f:
    f.write(tflite_model)


print("Saved TFLite model to:", tflite_path)