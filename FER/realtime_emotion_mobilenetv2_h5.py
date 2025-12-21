import cv2
import numpy as np
from keras.models import load_model
from keras.preprocessing.image import img_to_array

# === ШЛЯХИ ДО МОДЕЛІ ТА КАСКАДУ ===
face_cascade_path = "haarcascade_frontalface_default.xml"
model_path = "5classes_best_model.h5"

# === Завантаження каскаду ===
face_classifier = cv2.CascadeClassifier(face_cascade_path)

if face_classifier.empty():
    raise IOError("Не вдалося завантажити файл haarcascade_frontalface_default.xml. Перевір шлях.")

# === Завантаження моделі ===
classifier = load_model(model_path)

# === Класи емоцій ===
class_labels = ['angry', 'happy', 'neutral', 'sad', 'surprise']

# === Старт камери ===
cap = cv2.VideoCapture(0)

print("Натисни 'q' для виходу.")

while True:
    ret, frame = cap.read()
    if not ret:
        continue

    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)

    faces = face_classifier.detectMultiScale(gray, scaleFactor=1.3, minNeighbors=5)

    for (x, y, w, h) in faces:
        cv2.rectangle(frame, (x, y), (x + w, y + h), (255, 0, 0), 2)

        roi_gray = gray[y:y + h, x:x + w]
        roi_gray = cv2.resize(roi_gray, (48, 48))  # Твоя модель очікує (48, 48, 1)

        roi = roi_gray.astype("float32") / 255.0  # нормалізація
        roi = np.expand_dims(roi, axis=-1)        # (48, 48, 1)
        roi = np.expand_dims(roi, axis=0)         # (1, 48, 48, 1)

        preds = classifier.predict(roi, verbose=0)[0]
        label = class_labels[np.argmax(preds)]

        # Вивід на зображення
        cv2.putText(frame, label, (x, y - 10),
                    cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)

    cv2.imshow("Emotion Detector (5 класів)", frame)

    if cv2.waitKey(1) & 0xFF == ord("q"):
        break

cap.release()
cv2.destroyAllWindows()
