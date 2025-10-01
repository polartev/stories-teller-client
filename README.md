# 📚 Stories Teller
import requests

url = "https://opendata.dwd.de/climate_environment/CDC/help/zehn_min_ff_Beschreibung_Stationen.txt"
filename = "zehn_min_ff_Beschreibung_Stationen.txt"

# используем stream=True
response = requests.get(url, stream=True)

if response.status_code == 200:
    # пытаемся записать как байты
    with open(filename, "wb") as f:
        for chunk in response.iter_content(8192):
            f.write(chunk)
    print("Скачивание с записью байтов завершено")
else:
    print("Ошибка при запросе:", response.status_code)
AI-powered cross-platform application that generates text descriptions for user-uploaded photos.

This project consists of three main parts:
- **Cross-platform client app** (built with .NET MAUI)
- **Server** (FastAPI running on lightweight VPS)
- **Admin desktop tool** (Python)

> Bachelor’s thesis project. The goal: to provide users with meaningful text stories based on their photos, using AI generation.

---

## ✨ Latest Update: Global update of user-app, server, and admin parts
_This_ update was targeted at developing an app in all directions. Future updates will enhance today's results.

### ✅ Added library page
On this page, users can view, edit, and open all generated stories.

![Library](https://github.com/user-attachments/assets/0a495cdf-3f23-471c-afeb-22381e0523dd)

Clicking the edit button opens a pop-up window:

![Edit](https://github.com/user-attachments/assets/837fc5a8-af19-404b-aa55-959cf96292ed)

- Deleting history will erase the story and all associated photos.
- Editing allows renaming the story.

---

### ✏️ Changes to edit page
1. Visual update for the "add image" button.
2. Improved text editing.
3. New generations now append text instead of replacing it.
4. Added gallery view for used photos.
5. Many small UX improvements.

![Edit Page](https://github.com/user-attachments/assets/a222b8cd-1e40-4eb1-8d4c-4409e09af0b7)

Added image preview:

![Image Preview](https://github.com/user-attachments/assets/57d620a2-4be4-4dbc-a93c-3070fbe6796d)

Users can now cancel or approve edited text. If story sending succeeds, all editions will be automatically approved.

![Approve Edit](https://github.com/user-attachments/assets/76eb0cd7-c106-4acd-84c9-db061113ed09)

Side button opens the gallery of photos used for generation:

![Gallery](https://github.com/user-attachments/assets/5228e7a9-1127-4533-b048-d7bc8ae26843)

---

## 🛠 Server & Admin updates
- Developed server and admin parts to support:
  - New AI generation logic
  - New API endpoint
- Improved task queue and image management.
- Added support for appending new text to stories.

---

## 🧩 Architecture

### 1️⃣ MAUI Mobile Client
- Uploads photos (HTTPS POST)
- Connects via WebSocket to receive task updates
- Fetches generated text from server
- Manages stories and gallery locally (JSON-based storage)

### 2️⃣ FastAPI Server (on VPS)
- Accepts user uploads & saves them in queue
- Notifies admin about new tasks via WebSocket
- Receives AI-generated text from admin
- Sends final story to user

### 3️⃣ Admin Desktop App (Python)
- Receives tasks via WebSocket
- Downloads images
- Generates text descriptions via Gemini API
- Sends results back to server

---

## 🧰 Technologies
- .NET MAUI
- FastAPI (Python)
- Gemini AI / PyTorch / OpenCV
- WebSocket & REST API
- JSON-based task queue
- Cross-platform support

---

## 🚀 Future plans
- Improved AI prompt design
- Local caching & offline mode
- Advanced gallery features
- UI/UX refinements

---

## 📜 License
Apache 2.0

---

Feel free to fork, open issues, or contribute!  
⭐ If you like this project — give it a star!
