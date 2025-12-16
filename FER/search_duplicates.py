import os
from collections import defaultdict

# Папка з класами
train_dir = "train2"

# Очікувані класи
classes = ["happy", "neutral", "sad", "surprise"]

# Словник: file_name → список класів, у яких він зустрічається
duplicates = defaultdict(list)

for emotion in classes:
    emotion_dir = os.path.join(train_dir, emotion)
    if not os.path.isdir(emotion_dir):
        print(f"Warning: folder {emotion_dir} not found")
        continue

    for file_name in os.listdir(emotion_dir):
        # Пропускаємо службові файли
        if file_name.startswith('.') or not file_name.lower().endswith(('.png', '.jpg', '.jpeg', '.bmp')):
            continue

        duplicates[file_name].append(emotion)

# Фільтруємо лише ті файли, що є у 2+ класах
conflicts = {name: cls_list for name, cls_list in duplicates.items() if len(cls_list) > 1}

# Вивід
print("\n=== Файли, що зустрічаються в кількох класах ===\n")

if not conflicts:
    print("Дублікатів не знайдено — все чисто!")
else:
    for file_name, cls_list in conflicts.items():
        print(f"{file_name}: {', '.join(cls_list)}")

    print(f"\nЗагальна кількість проблемних файлів: {len(conflicts)}")

# Якщо хочеш отримати список назв у Python-структурі:
duplicate_names = list(conflicts.keys())

print("\nСписок дублікатів у змінній duplicate_names:")
print(duplicate_names)
