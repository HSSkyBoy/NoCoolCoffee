# 肉桂情事 (Cinnamon Rendezvous) - GTA5 中文熱咖啡與動態社交模組

本專案為 GTA5 腳本模組 **Cinnamon Coffee** 的繁體中文漢化、維護與功能強化增強版本。

---

## 📌 上游來源與聲明

* **原始模組名稱**：Cinnamon Coffee
* **原始作者**：StrelokCheeki
* **上游來源連結**：[GTA5-Mods Cinnamon Coffee](https://www.gta5-mods.com/scripts/cinnamon-coffee)
* **漢化維護版本**：v1.2+ (Enhanced Edition)
* **測試環境**：ScriptHookV / ScriptHookV .NET (Legacy & Enhanced)

---

## 🛠️ 前置需求 (Dependencies)

安裝本模組前，請確保你的 GTA V 已安裝以下前置工具：

* [Script Hook V](https://www.gta5-mods.com/tools/script-hook-v)
* [ScriptHookV .NET (Legacy)](https://www.gta5-mods.com/tools/scripthookv-net) 或 [ScriptHookV .NET (Enhanced)](https://www.gta5-mods.com/tools/script-hook-v-net-enhanced)

---

## 🎮 模組特色與最新優化

「肉桂情事」是基於 ScriptHook 的輕量化動態社交與互動腳本，靈感源自經典「熱咖啡（Hot Coffee）」概念。具備精緻的互動選單、動態 A-Life 關係系統、車內與街頭服務模式、手把原生支援以及豐富的互動動畫。

### ✨ 核心功能亮點

1. **全新優化的 NPC 車輛上車與座位分配系統 (Smart Vehicle Entry)**
   * **智慧座位匹配（副駕被佔用時自動移至後座）**：
     * **副駕空著時**：NPC 優先進入【副駕駛座】。
     * **副駕已有其他 NPC 佔用時**：NPC 會透過 GTA 引擎原生檢測 (`IS_VEHICLE_SEAT_FREE`)，自動尋找並跑步進入【後座 (左後/右後座)】，徹底解決因副駕被佔用而卡在車外傻住的問題。
     * **玩家坐後座時**：NPC 優先進入【後座另一側】或【前座】。
   * **自動解鎖車門與防受驚**：發起上車指令時自動解鎖車輛車門（防止因鎖門上車失敗），並設定跑步速度 (`2.0f`) 與受驚免疫，確保上車迅速順暢。
   * **超時防卡牆保護**：若 NPC 在車門旁或地形卡住超過 16 秒，系統會自動安全轉移 (Warp) 入座。

2. **車內後座與前座純聊天模式 (Backseat & Front Seat Chat)**
   * 雙方入座車內後，若未主動選擇性愛服務，選單維持在【動作 / 對話】層級。
   * 隨時可開啟「對話 (Conversation)」選單交流聊天、問候、傾聽心事與提升親密度，**不會強制觸發性愛動畫**。

3. **「請上車 / 邀請上車」語音對話指令**
   * 無論玩家坐在車內或是在車外靠近載具（10 米內），選單皆提供「邀請上車 / 請上車 [>]」選項。
   * 選取後播放專屬性格字幕對話（*「上車吧，我們出發！」* / *「好啊，馬上來！」*）並即刻引導 NPC 上車。

4. **動態 A-Life 社交與關係演進系統**
   * 每位女性 NPC 擁有獨特的姓名、性格（害羞、甜美、傲嬌、高冷、火辣、狂野等）與獨立親密度。
   * 從陌生人到友好、女朋友或專屬夥伴，對話台詞與互動選項會隨關係演進解鎖。

5. **豐富的服務與互動體驗**
   * 支援街頭立姿、坐姿、臥姿服務以及車內專屬服務動畫。
   * 提供金錢贈予、表情切換與手把震動反饋。

---

## 📦 安裝說明

### 方式 A：下載編譯好的成品 (推薦)

1. 從 GitHub Releases 或 Actions 下載最新版的 `CinnamonRendezvous.zip`。
2. 解壓縮後將 `scripts\` 資料夾內的所有檔案（包含 `CinnamonCoffee.dll`、`CinnamonCoffee.cfg`、`CinnamonCoffeeALife.ini`）複製至你的 GTA V 遊戲安裝目錄：
   ```text
   \Grand Theft Auto V\scripts\
   ```

### 方式 B：自行編譯專案

於 PowerShell 中執行編譯腳本：
```powershell
./build.ps1
```

---

## 📁 檔案結構拆分說明

本專案將龐大的對話庫抽離，實現清晰的程式碼解耦：

* `CinnamonCoffee.cs`：模組主邏輯、狀態機、車輛座位計算、事件循環與選單控制器。
* `CinnamonCoffeeDialogue.cs`：獨立對話台詞資料庫與動態文本生成邏輯（`partial class CinnamonCoffee`）。
* `CinnamonCoffee.cfg`：模組設定檔（可設定熱鍵、選單顏色與行為）。
* `CinnamonCoffeeALife.ini`：A-Life NPC 記憶與關係存檔資料。

---

## 📄 授權與感謝

* 感謝原作者 **StrelokCheeki** 創作 Cinnamon Coffee 原始腳本。
* 感謝所有 GTA5 模組社群與 ScriptHookV / ScriptHookV .NET 開發者的無私貢獻。


