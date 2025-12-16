import cv2
import numpy as np
import tensorflow as tf

# === ШЛЯХИ ДО ФАЙЛІВ ===
cascade_path = "haarcascade_frontalface_default.xml"
tflite_path = "emotion_mobilenetv2_cls.tflite"

# === Завантажуємо каскад ===
face_classifier = cv2.CascadeClassifier(cascade_path)

# === Завантажуємо tflite модель ===
interpreter = tf.lite.Interpreter(model_path=tflite_path)
interpreter.allocate_tensors()

input_details = interpreter.get_input_details()
output_details = interpreter.get_output_details()

class_labels = ['happy', 'neutral', 'sad', 'surprise']

cap = cv2.VideoCapture(0)
print("Press 'q' to quit.")

while True:
    ret, frame = cap.read()
    if not ret:
        continue

    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    faces = face_classifier.detectMultiScale(gray, 1.3, 5)

    for (x, y, w, h) in faces:
        cv2.rectangle(frame, (x, y), (x + w, y + h), (255, 0, 0), 2)

        roi_gray = gray[y:y + h, x:x + w]
        roi_gray = cv2.resize(roi_gray, (224, 224))

        # === grayscale → 3-channel, нормалізація ===
        roi = roi_gray.astype("float32") / 127.5 - 1.0
        roi = np.stack((roi, roi, roi), axis=-1)
        roi = np.expand_dims(roi, axis=0)

        # === запуск моделі ===
        interpreter.set_tensor(input_details[0]["index"], roi)
        interpreter.invoke()
        preds = interpreter.get_tensor(output_details[0]["index"])[0]

        label = class_labels[np.argmax(preds)]

        cv2.putText(frame, label, (x, y - 10),
                    cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)

    cv2.imshow("Emotion Detector (TFLite)", frame)
    if cv2.waitKey(1) & 0xFF == ord("q"):
        break

cap.release()
cv2.destroyAllWindows()
