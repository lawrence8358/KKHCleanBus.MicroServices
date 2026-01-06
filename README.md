# 🚌 高雄垃圾車即時資訊 API

> 提供高雄市垃圾車路線查詢與環保新聞的 RESTful API 服務

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

---

## 📖 關於專案

這是一個提供高雄市垃圾車即時資訊查詢的 API 服務，資料來源為高雄市環保局公開資訊。

API 目前部署於免費雲端空間（Render），提供給需要整合垃圾車資訊的開發者使用。

### 主要功能

- 🚛 **垃圾車路線查詢** - 依車牌號碼或座標位置查詢清運路線
- 📰 **環保新聞公告** - 取得高雄市環保局最新消息
- 🕒 **時段篩選** - 可指定早上、下午、晚上時段查詢
- 📍 **距離計算** - 根據座標計算最近的清運點

---

## 🚀 API 使用說明

### 基本資訊

- **Base URL**: `https://cleanbus.primeeagle.net/` (部署後更新)
- **回應格式**: JSON
- **無需驗證**: 開放使用，無需 API Key

### 主要端點

#### 1️⃣ 垃圾車路線查詢

```http
GET /api/car/route/{車牌號碼}
```

**範例**
```bash
GET /api/car/route/043-UV
```

**回應**
```json
{
  "type": "Ok",
  "data": [
    {
      "seq": "1234",
      "carLicence": "043-UV",
      "caption": "民權二路與長江街口",
      "deptName": "前鎮區",
      "villageName": "興東里",
      "timeRange": "18:30~18:32",
      "longitude": 120.313514,
      "latitude": 22.607238
    }
  ]
}
```

#### 2️⃣ 依時段查詢

```http
GET /api/car/route/{車牌號碼}/{時段}
```

**時段代碼**
- `0` - 早上 (00:00-12:00)
- `1` - 下午 (12:00-18:00)  
- `2` - 晚上 (18:00-24:00)

**範例**
```bash
GET /api/car/route/043-UV/2
```

#### 3️⃣ 座標查詢附近清運點

```http
POST /api/car/route
Content-Type: application/json

{
  "lat": 22.607238,
  "lng": 120.313514,
  "distance": 500,
  "type": 2,
  "inMinutes": 30
}
```

**參數說明**
- `lat` - 緯度
- `lng` - 經度
- `distance` - 搜尋半徑（公尺）
- `type` - 時段類型
- `inMinutes` - 幾分鐘內

#### 4️⃣ 環保新聞

```http
GET /api/news/topic        # 取得前 5 則新聞標題
GET /api/news/all          # 取得所有新聞
GET /api/news/{id}         # 取得單一新聞內容
```

#### 5️⃣ 健康檢查

```http
GET /health
```

---

## 💡 使用範例

### JavaScript (Fetch API)

```javascript
// 查詢垃圾車路線
fetch('https://cleanbus.primeeagle.net//api/car/route/043-UV/2')
  .then(response => response.json())
  .then(data => {
    console.log('今晚垃圾車路線:', data.data);
  });
```

### Python

```python
import requests

# 座標查詢
payload = {
    "lat": 22.607238,
    "lng": 120.313514,
    "distance": 500,
    "type": 2
}

response = requests.post(
    'https://cleanbus.primeeagle.net//api/car/route',
    json=payload
)
print(response.json())
```

### cURL

```bash
curl -X POST https://cleanbus.primeeagle.net//api/car/route \
  -H "Content-Type: application/json" \
  -d '{"lat":22.607238,"lng":120.313514,"distance":500,"type":2}'
```

---

## 📱 相關應用

這個 API 服務支援以下應用程式：

- **高雄搜垃圾** (iOS/Android) - [隱私權政策](https://lawrencetech.blogspot.com/p/blog-page.html)

---

## ⚠️ 使用限制

由於服務部署於免費雲端平台，有以下限制：

- 🕒 **冷啟動時間** - 閒置 15 分鐘後首次請求可能需等待約 30 秒
- 📊 **流量限制** - 每月有一定流量限制，請勿濫用
- 💾 **資料更新** - 資料會定期從高雄市環保局同步

---

## 📝 回應格式

### 成功回應

```json
{
  "type": "Ok",
  "data": { ... },
  "errors": null
}
```

### 錯誤回應

```json
{
  "type": "BadRequest",
  "data": null,
  "errors": ["E01"]
}
```

**錯誤代碼**
- `E01` - 參數錯誤

---

## 🔗 資料來源

本服務資料來源：
- [高雄市環保局 - 垃圾車動態查詢](https://kepbgps.kcg.gov.tw/)

---

## 📧 聯絡與回饋

如果您有任何問題、建議或想要回報錯誤，歡迎透過以下方式聯繫：

- 📝 [功能建議表單](https://docs.google.com/forms/d/e/1FAIpQLSc0muoQ7ILvgy5mVSJvH_Y6xRNXck4s8gPR7zxbjMTH-GYu9g/viewform?usp=sf_link)
- 🐛 [GitHub Issues](https://github.com/lawrence8358/KKHCleanBus.MicroServices/issues)

---

## 📄 授權

本專案採用 MIT 授權條款。

> ⚠️ **免責聲明**  
> 本 API 服務僅供參考，實際垃圾車到達時間可能因路況、天候等因素有所延遲。請以現場實際狀況為準。

---

**最後更新：** 2026-01-07  
**維護者：** Lawrence
