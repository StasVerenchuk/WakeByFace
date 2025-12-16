import cv2
import numpy as np
from keras.models import load_model
from keras.preprocessing.image import img_to_array

# === ШЛЯХИ ДО МОДЕЛІ ТА КАСКАДУ ===
face_cascade_path = "haarcascade_frontalface_default.xml"
model_path = "Emotion_mobilenetv2_4cls.h5"

# === Завантаження каскаду ===
face_classifier = cv2.CascadeClassifier(face_cascade_path)

# === Завантаження моделі ===
classifier = load_model(model_path)

# === Твої класи ===
class_labels = ['happy', 'neutral', 'sad', 'surprise']

# === Старт відеопотоку ===
cap = cv2.VideoCapture(0)

print("Press 'q' to quit.")

while True:
    ret, frame = cap.read()
    if not ret:
        continue

    # Перетворення в grayscale для каскаду
    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)

    # Знаходження облич
    faces = face_classifier.detectMultiScale(gray, 1.3, 5)

    for (x, y, w, h) in faces:
        # Малюємо рамку
        cv2.rectangle(frame, (x, y), (x + w, y + h), (255, 0, 0), 2)

        # Вирізаємо область обличчя
        roi_gray = gray[y:y + h, x:x + w]

        # Розмір під модель
        roi_gray = cv2.resize(roi_gray, (224, 224))

        # Нормалізація (як у MobileNetV2 grayscale)
        roi = roi_gray.astype("float32") / 127.5 - 1.0
        roi = np.stack((roi, roi, roi), axis=-1)   # робимо 3-канальний grayscale
        roi = np.expand_dims(roi, axis=0)

        # Прогноз
        preds = classifier.predict(roi)[0]
        label = class_labels[np.argmax(preds)]

        # Відображення
        cv2.putText(frame, label, (x, y - 10),
                    cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)

    cv2.imshow("Emotion Detector (H5)", frame)

    if cv2.waitKey(1) & 0xFF == ord("q"):
        break

cap.release()
cv2.destroyAllWindows()
