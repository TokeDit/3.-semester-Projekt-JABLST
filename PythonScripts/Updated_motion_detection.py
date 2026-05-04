import cv2
import datetime
import time
import numpy as np
import base64
import json
from picamera2 import Picamera2
import requests
from ai_edge_litert.interpreter import Interpreter

# ─── Configuration ────────────────────────────────────────────────────────────

API_ENDPOINT = "https://sikkerheds-app-jablst-f0ewdphzhsf0hqcr.swedencentral-01.azurewebsites.net/api/PI"
COOLDOWN_SECONDS = 30
JPEG_QUALITY = 75
MIN_CONF_THRESHOLD = 0.5

# ─── TFLite Model Setup ───────────────────────────────────────────────────────

interpreter = Interpreter(model_path="detect.tflite")
interpreter.allocate_tensors()

input_details = interpreter.get_input_details()
output_details = interpreter.get_output_details()

floating_model = input_details[0]['dtype'] == np.float32

labels = [
    "background", "person", "bicycle", "car", "motorcycle", "airplane", "bus",
    "train", "truck", "boat", "traffic light", "fire hydrant", "street sign",
    "stop sign", "parking meter", "bench", "bird", "cat", "dog", "horse", "sheep",
    "cow", "elephant", "bear", "zebra", "giraffe", "hat", "backpack", "umbrella",
    "shoe", "eye glasses", "handbag", "tie", "suitcase", "frisbee", "skis",
    "snowboard", "sports ball", "kite", "baseball bat", "baseball glove",
    "skateboard", "surfboard", "tennis racket", "bottle", "plate", "wine glass",
    "cup", "fork", "knife", "spoon", "bowl", "banana", "apple", "sandwich",
    "orange", "broccoli", "carrot", "hot dog", "pizza", "donut", "cake", "chair",
    "couch", "potted plant", "bed", "mirror", "dining table", "window", "desk",
    "toilet", "door", "tv", "laptop", "mouse", "remote", "keyboard", "cell phone",
    "microwave", "oven", "toaster", "sink", "refrigerator", "blender", "book",
    "clock", "vase", "scissors", "teddy bear", "hair drier", "toothbrush"
]

# ─── Camera Setup ─────────────────────────────────────────────────────────────

picam = Picamera2()
picam.configure(picam.create_preview_configuration(
    main={"size": (1280, 720)},
    raw={"size": (3280, 2464)}
))
picam.start()
time.sleep(2)

# ─── Helper Functions ─────────────────────────────────────────────────────────

def detect_humans(image):
    rgb = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    input_data = cv2.resize(rgb, (300, 300))
    input_data = np.expand_dims(input_data, axis=0)

    if floating_model:
        input_data = (np.float32(input_data) - 127.5) / 127.5

    interpreter.set_tensor(input_details[0]['index'], input_data)
    interpreter.invoke()

    classes = interpreter.get_tensor(output_details[1]['index'])[0]
    scores  = interpreter.get_tensor(output_details[2]['index'])[0]

    best_score = None
    for i in range(len(scores)):
        label_index = int(classes[i]) + 1
        if scores[i] > MIN_CONF_THRESHOLD and label_index < len(labels) and labels[label_index] == "person":
            if best_score is None or scores[i] > best_score:
                best_score = float(scores[i])

    return best_score


def send_to_api(frame, timestamp, confidence):
    success, buffer = cv2.imencode(
        ".jpg",
        frame,
        [cv2.IMWRITE_JPEG_QUALITY, JPEG_QUALITY]
    )

    if not success:
        print("[ERROR] Failed to encode frame as JPEG")
        return False

    image_b64 = base64.b64encode(buffer.tobytes()).decode("utf-8")

    payload = {
        "ImageDataBase64": image_b64,
        "ImageType": "image/jpeg",
        "Description": "person",
        "DetectedObject": "person",
        "Confidence": round(confidence, 4),
        "TimeStamp": timestamp.isoformat(timespec = 'minutes'),
    }

    try:
        response = requests.post(
            API_ENDPOINT,
            data=json.dumps(payload),
            headers={"Content-Type": "application/json"},
            timeout=10
        )
        response.raise_for_status()
        print(f"[OK] Uploaded — person detected with {confidence:.1%} confidence")
        return True

    except requests.exceptions.Timeout:
        print("[ERROR] Request timed out")
        return False

    except requests.exceptions.ConnectionError:
        print("[ERROR] Could not connect to API — check network connection")
        return False

    except requests.exceptions.HTTPError as e:
        print(f"[ERROR] API returned {e.response.status_code} — {e.response.text}")
        return False

    except requests.exceptions.RequestException as e:
        print(f"[ERROR] Unexpected request error: {e}")
        return False

# ─── Frame Capture ────────────────────────────────────────────────────────────

frame1 = cv2.cvtColor(picam.capture_array(), cv2.COLOR_RGB2BGR)
frame2 = cv2.cvtColor(picam.capture_array(), cv2.COLOR_RGB2BGR)

print("Watching for motion...")

last_upload_time = 0

# ─── Main Loop ────────────────────────────────────────────────────────────────

while True:
    diff = cv2.absdiff(frame1, frame2)
    grey = cv2.cvtColor(diff, cv2.COLOR_BGR2GRAY)
    blur = cv2.GaussianBlur(grey, (5, 5), 0)
    _, thresh = cv2.threshold(blur, 20, 255, cv2.THRESH_BINARY)
    dilated = cv2.dilate(thresh, None, iterations=3)
    contours, _ = cv2.findContours(dilated, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)

    motion_detected = False
    for contour in contours:
        if cv2.contourArea(contour) < 500:
            continue
        motion_detected = True
        break

    if motion_detected:
        confidence = detect_humans(frame2)

        if confidence is not None:
            current_time = time.time()

            if current_time - last_upload_time >= COOLDOWN_SECONDS:
                timestamp = datetime.datetime.now()
                print(f"Human detected at {timestamp.strftime('%Y-%m-%d %H:%M')} ({confidence:.1%} confidence) — uploading...")

                success = send_to_api(frame1, timestamp, confidence)

                if success:
                    last_upload_time = current_time
            else:
                remaining = COOLDOWN_SECONDS - (current_time - last_upload_time)
                print(f"Human detected — cooldown active, {remaining:.0f}s remaining")
        else:
            print("Motion detected — no human confirmed, skipping upload")

    frame1 = frame2
    frame2 = cv2.cvtColor(picam.capture_array(), cv2.COLOR_RGB2BGR)
