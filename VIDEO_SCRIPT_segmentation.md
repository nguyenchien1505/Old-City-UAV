# 🎬 Kịch bản video: Gán nhãn mô hình (Semantic Segmentation)

**Người làm:** Hiep · **Phần:** 3/4 của dự án Old City UAV (PRU213)
**Style:** Không lời, text overlay mô tả mỗi cảnh
**Thời lượng mục tiêu:** ~3–5 phút
**Tool quay:** Unity Recorder (com.unity.recorder) — Game View 1920×1080, 30fps
**Tool edit:** Clipchamp (built-in Windows 11)

---

## 📋 Trước khi quay — checklist 5 phút

- [ ] Mở project trong Unity, đợi import xong
- [ ] Mở scene `Assets/Tiny_Toony_City_Demo/Scenes/Demo_Scene 1.unity` (đã set làm scene mặc định)
- [ ] Vào `Window > Package Manager > Unity Registry`, tìm **Recorder**, Install nếu chưa có
- [ ] Mở `Window > General > Recorder > Recorder Window`, add **Movie Recorder**, source = Game View, output folder = `Recordings/`
- [ ] **Kiểm tra layer:** mở `Edit > Project Settings > Tags and Layers` → đảm bảo slot 6–11 là: Building / Ground / UAV / Bird / **Vegetation** / **Vehicle** *(2 layer cuối tôi vừa thêm)*
- [ ] **Gán layer cho GameObject trong scene** *(việc Hiep phải tự làm tay — script chỉ swap material được nếu object có layer đúng)*:
  - Building meshes → layer `Building`
  - Tree / foliage prefabs (Foliage_1, Foliage_2, Birchtree...) → layer `Vegetation`
  - Car_B1 + citizens prefabs → layer `Vehicle`
  - Sidewalk, Demo_Asphalt, Demo_Block → layer `Ground`
- [ ] Mở `Assets/Scripts/SemanticSegmentation.cs` trong VS Code/Rider — chuẩn bị show code
- [ ] Tắt console log, đóng panel không cần thiết, layout Unity gọn gàng

---

## 🎬 CẢNH 1 — Intro (0:00 → 0:20)

**Quay gì:** Mở Unity, scene Demo_Scene 1 đang load. Pan camera scene qua một góc phố. Sau đó zoom vào hierarchy panel cho thấy 3 camera rig: `CameraRig_01`, `SemanticCamera`.

**Text overlay (xuất hiện 1.5s mỗi câu):**
```
Old City UAV — Phần 3: Gán nhãn mô hình
Mục tiêu: tạo ground-truth segmentation mask
cho dữ liệu huấn luyện ML từ góc nhìn UAV
```

**Mẹo:** Hiep di chuyển camera Scene view chậm thôi, đừng giật.

---

## 🎬 CẢNH 2 — Vấn đề (0:20 → 0:50)

**Quay gì:**
1. Click vào RGB Camera trong CameraRig_01, show preview Game view (cảnh phố bình thường, đẹp đẽ)
2. Click vào IR_Camera, show preview (cảnh tone IR)
3. Click vào SemanticCamera, show preview (sẽ là cảnh flat color khi Play)
4. Hiển thị Project window mở folder `Assets/` show 4 material: `Seg_Building.mat`, `Seg_Ground.mat`, `Seg_Tree.mat`, `Seg_Vehicle.mat`

**Text overlay:**
```
Rig 3 camera:  RGB + IR + Semantic
Bài toán:  cần render 1 ảnh nơi mỗi loại vật thể
có 1 màu phẳng riêng → làm "đáp án" cho ML
4 material flat-color đã chuẩn bị sẵn:
Seg_Building / Seg_Tree / Seg_Vehicle / Seg_Ground
```

---

## 🎬 CẢNH 3 — Hệ thống Layer (0:50 → 1:30)

**Quay gì:**
1. `Edit > Project Settings > Tags and Layers` → focus panel Layers
2. Show layer 6 = Building, 7 = Ground, 8 = UAV, 9 = Bird, **10 = Vegetation, 11 = Vehicle**
3. Click 1 building prefab trong hierarchy → Inspector → góc trên phải dropdown **Layer** → chọn `Building` (highlight chuyển layer)
4. Click 1 tree → Layer = `Vegetation`
5. Click Car_B1 → Layer = `Vehicle`

**Text overlay:**
```
Bước 1: phân loại vật thể bằng Unity Layer
Mỗi loại vật thể → 1 layer riêng
(slot 10/11 là 2 layer mới mình thêm)
```

**Mẹo:** Khi gán layer cho prefab có nhiều child, Unity sẽ hỏi "Change children layer too?" — chọn **Yes, change children**.

---

## 🎬 CẢNH 4 — Script SemanticSegmentation (1:30 → 2:30)

**Quay gì:**
1. Mở `Assets/Scripts/SemanticSegmentation.cs` trong IDE
2. Highlight (chậm rãi) các phần chính:
   - Field `buildingMat`, `treeMat`, `vehicleMat`, `groundMat` — Inspector slots
   - Hàm `BuildCache()` — quét renderer 1 lần lúc enable
   - Hàm `PickSegMaterial(int layer)` — switch theo tên layer
   - Hàm `HandlePreCull(Camera cam)` — swap material **chỉ** khi SemanticCamera sắp render
   - Hàm `HandlePostRender(Camera cam)` — restore material gốc sau khi SemanticCamera render xong
3. Quay lại Unity, click vào SemanticCamera, show Inspector → 4 ô material đã kéo Seg_* vào

**Text overlay:**
```
Script chạy trên SemanticCamera
Logic 3 bước:
1. Cache toàn bộ Renderer trong scene
2. Trước khi SemanticCamera render → swap sang Seg_*
3. Sau khi render xong → trả material gốc về
→ RGB và IR camera KHÔNG bị ảnh hưởng
```

**Mẹo:** Có thể dùng tính năng "zoom code" của IDE để frame riêng từng method.

---

## 🎬 CẢNH 5 — Play mode demo (2:30 → 3:30)

**Quay gì:**
1. Sắp xếp Game view + Hierarchy + Inspector cho dễ nhìn
2. Press **Play** ▶
3. Trong Game view, switch tab Camera display (Display 1 / 2 / 3) hoặc đổi `Target Display` của các camera trước khi quay:
   - Display 1 → RGB camera output (cảnh phố thật)
   - Display 2 → IR_Camera output (cảnh tone IR)
   - Display 3 → SemanticCamera output (cảnh flat-color segmentation)
4. Quay slow pan qua từng display

**Text overlay:**
```
3 camera render đồng thời, độc lập:
[Display 1]   RGB — ảnh thật
[Display 2]   IR — phổ nhiệt
[Display 3]   Semantic — ground-truth mask
```

**Lưu ý kỹ thuật:** Nếu chỉ có 1 monitor, set `Target Display` từng camera bằng cách click camera → Inspector → field "Target Display". Trong Game view tab, có dropdown "Display X" góc trên trái để switch.

**Mẹo:** Khi pan, thấy tree màu xanh phẳng (Seg_Tree), nhà màu đỏ (Seg_Building), xe màu vàng (Seg_Vehicle), đường màu xám (Seg_Ground) → đó là ground-truth mask hoàn chỉnh.

---

## 🎬 CẢNH 6 — Kết quả + Use case (3:30 → 4:30)

**Quay gì:**
1. Game view full screen Display 3 (Semantic output) trong khi camera UAV bay 1 vòng qua phố (nếu có flight controller, nếu không thì pan camera scene chậm rãi)
2. Sau đó split-screen hoặc cut-to: bên trái RGB, bên phải Semantic — cho thấy paired data
3. Outro: text closing

**Text overlay:**
```
Output: paired data RGB ↔ Semantic mask
→ huấn luyện model detect được:
   building / vegetation / vehicle / ground
   từ ảnh UAV chụp trên không

Phần 3 hoàn thành 🚁
```

---

## 📝 Sau khi quay xong — workflow edit

1. Mở MP4 trong **Clipchamp** (Win+S → "Clipchamp" → tạo project mới)
2. Drag MP4 vào timeline
3. Tab **Text** bên trái → chọn template caption đơn giản (vd: "Title 1" hoặc "Lower third")
4. Drag text vào timeline ở đúng frame, type nội dung từ phần "Text overlay" ở trên
5. Adjust timing để text overlay xuất hiện chậm rãi (1.5s/dòng)
6. Trim các đoạn thừa
7. Export: 1080p, MP4

**Free alternative nếu Clipchamp không đủ:**
- **CapCut Desktop** — dễ hơn nữa, nhiều template hơn
- **DaVinci Resolve Free** — pro hơn, learning curve cao hơn

---

## 🐛 Troubleshooting

| Vấn đề | Nguyên nhân | Fix |
|---|---|---|
| SemanticCamera ra cảnh giống RGB camera | Object trong scene chưa được gán đúng layer | Quay lại Cảnh 3, gán layer cho từng prefab |
| RGB camera bị flat color (script cũ phá pass) | Đang chạy code cũ chưa fix | Đảm bảo `SemanticSegmentation.cs` đã là version mới (có method `HandlePreCull` / `HandlePostRender`) |
| Treee/cars ra màu sai | Tên layer không match | Check `Edit > Project Settings > Tags and Layers` slot 10 phải là `Vegetation`, slot 11 là `Vehicle` |
| Unity Recorder không hiện trong menu | Package chưa cài | `Window > Package Manager > Unity Registry` → search "Recorder" → Install |
| Game view chỉ show 1 camera | Camera khác chưa set Target Display | Click camera → Inspector → field `Target Display` chọn 1/2/3 |

---

## 🎯 Tips quay phim cho coder

- **Đừng nói** trong khi quay — text overlay là đủ
- **Pan / zoom chậm** — người xem cần thời gian đọc text
- **Đóng Discord/Slack/notification** trước khi quay
- **Tăng font Unity Inspector** lên 12-14pt cho dễ nhìn (`Edit > Preferences > UI Scaling`)
- **Quay segment ngắn 10-30s** từng cảnh thay vì 1 take dài — dễ edit lại
- **Save scene trước khi Play** (Ctrl+S) — phòng crash mất scene
