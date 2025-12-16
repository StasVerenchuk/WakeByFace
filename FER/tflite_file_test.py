import numpy as np
import tensorflow as tf

interpreter = tf.lite.Interpreter(model_path="emotion_mobilenetv2_cls.tflite")
interpreter.allocate_tensors()

input_details = interpreter.get_input_details()
output_details = interpreter.get_output_details()

print("Input:", input_details)
print("Output:", output_details)

# Створюємо фейковий вхід того ж розміру
input_shape = input_details[0]["shape"]
dummy = np.zeros(input_shape, dtype=np.float32)

interpreter.set_tensor(input_details[0]["index"], dummy)
interpreter.invoke()
output = interpreter.get_tensor(output_details[0]["index"])
print("Output probs:", output)
