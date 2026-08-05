# 肉桂情事 (Cinnamon Rendezvous) - GTA5 繁體中文模組

本模組為 GTA5 腳本模組 **Cinnamon Coffee** 的繁體中文漢化與維護版本。

## 📌 上游來源與聲明
- **原始模組名稱**：Cinnamon Coffee
- **原始作者**：StrelokCheeki
- **上游來源連結**：https://www.gta5-mods.com/scripts/cinnamon-coffee
- **漢化版本**：v1.2
- **測試環境**：ScriptHookV .NET Legacy 3725

---

## 🎮 模組簡介
「肉桂情事」是基於 ScriptHook 的輕量化互動腳本，靈感源自經典「熱咖啡（Hot Coffee）」概念。從零開始全新編寫，具備精緻的互動選單、沙盒與賣春模式、手把原生支援與豐富的互動動畫。

---

## 📁 檔案結構拆分說明
本專案已將龐大的對話庫抽離，實現程式碼解耦：
- `CinnamonCoffee.cs`：模組主邏輯、事件循環、選單控制器與動畫處理。
- `CinnamonCoffeeDialogue.cs`：獨立對話台詞資料庫與文本生成邏輯（`partial class CinnamonCoffee`）。

---

## 📦 安裝說明

### 方式 A：直接下載編譯好的成品 (推薦一般玩家)
1. 從 GitHub Releases 或 Actions 下載 `CinnamonRendezvous-v1.2.zip`。
2. 解壓縮後將 `scripts\` 資料夾內的檔案（包含 `CinnamonCoffee.dll`、`CinnamonCoffee.cfg`、`CinnamonCoffeeALife.ini`）複製至你的 GTA V 遊戲目錄：
   ```text
   \Grand Theft Auto V\scripts\
   ```

### 方式 B：自行編譯（開發者）
本專案已配置完整的 `.csproj` 與建置腳本：
- 本地編譯：於 PowerShell 中執行 `./build.ps1`
- CI/CD：提交至 GitHub 後將自動觸發 `.github/workflows/build.yml` 編譯產生最新 `.dll`。

---

## ⚠️ 注意事項
- `.cfg` 與 `.ini` 設定檔內的**鍵名（Key）必須維持全英文**，請勿隨意修改設定檔內的語法鍵名，否則可能導致腳本讀取失敗。
- 遊戲內選單與通知 UI 已完整繁體中文化。
