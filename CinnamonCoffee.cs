using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using GTA.Math;

/// 肉桂情事 (Cinnamon Rendezvous) - GTA V ScriptHookVDotNet v3 成人腳本模組。
/// 改編自 GTA V 與 GTA IV 的經典 HotCoffee 腳本，專為 SHVDN v3 API 設計。
/// 
/// 必要前置需求：
///   - ScriptHookV (作者: Alexander Blade)
///   - ScriptHookVDotNet v3 (社群 .NET 包裝庫)
///   - 將此 .cs (或編譯好的 .dll) 放置於 GTA V 目錄下的 /scripts/ 資料夾
///
/// 操作按鍵：
///   J          = 開啟/關閉選單
///   NumPad 8/2 = 選單上下導航
///   NumPad 5   = 確認選擇選單項目
///   NumPad 0   = 返回上一層選單 / 關閉選單
///   NumPad 4/6 = 調整動畫速度 (動畫進行中)
///   NumPad 1   = 重置動畫速度 (動畫進行中)

namespace CinnamonCoffee
{
    public partial class CinnamonCoffee : Script
    {

        // ================================================================
        //  枚舉定義 (ENUMS)
        // ================================================================

        private enum ScriptState { Idle, Approaching, Animating }
        private enum Mode { None, Car, Street }
        private enum VehicleAnimType { Normal, Low }
        private enum MenuLevel { None, Main, SandboxCar, Services, StandingServices, SittingServices, LayingServices, SitDownSub, Compatibility, Settings, Approach, ApproachLine, Finish, Actions, Leave, LeaveLine, Conversation, ConvSub, Intimacy, IntimacySub, Escalate, InviteToVehicle, GiveMoney }
        private enum PriceGroup { Kissing, Blowjob, Facesitting, Cowgirl, Doggy }

        // ================================================================
        //  內部類別 (INNER CLASSES)
        // ================================================================

        // ── 個性系統 (Personality System) ──────────────────────────────────────────
        private class PersonalityProfile
        {
            public string Name;
            // 所有特質特徵數值介於 0.0–1.0 之間
            public double Friendliness; // 影響對話熱情度 (非常友善→非常敵對)
            public double Confidence;   // 影響行為大膽度
            public double Greed;        // 影響服務定價、議價難度與支付彩蛋
            public double Patience;     // 影響搭訕接受度 (對纏人的忍耐程度)
            public double Riskiness;    // 影響彩蛋機制與內射接受度
            public double Attachment;   // 影響關係度增減的幅度
            public double Aggressiveness; // 影響被拒絕時的反應：高=反擊，低=逃跑
        }

        private class ALifePedData
        {
            public string Fingerprint;
            public string Name;         // 首次遭遇時隨機分配的姓名
            public int    Reputation;   // -2 to 100
            public string Relationship; // Hostile/Avoiding/Stranger/Acquaintance/Familiar/Friendly/Friendzoned/Flirty/Girlfriend/Obsessed
            public string Personality;  // 個性名稱 Key — 透過 GetProfile() 查詢
            public bool   HasMet;       // 首次接受或拒絕對話後設為 true
            public bool   HasMetSecondTime; // 玩家第二次招募後設為 true (解鎖個人調情台詞)
            public string Voice;        // A-Life 模式中分配一次，跨會話持久化保存
            public string HookerAnim;   // 搭訕動畫字典，分配後持久化保存
            public bool   IsHooker;     // 彩蛋設定：同意收費的普通平民女性
            public bool   WasHooker;     // 曾同意過協議時設為 true (用於判斷是否為再次提議)
            public bool   HookerPermaReject; // 永久拒絕商業提議時設為 true
            public bool   NameKnown;    // 向玩家透露姓名後設為 true
            public string LastPositiveRelationship; // 好感度降至 -1 前保存的關係狀態
            public string Mood;          // transient: "Relaxed","Happy","Alert","玩鬧","Annoyed","Needy","Jealous"
            public bool?  PrefBJ;       // null=未知, true=願意, false=不願意
            public bool?  PrefRough;    // null=未知, true=願意, false=不願意
            public int[]  HookerPrices;  // [0]=Kissing [1]=Blowjob [2]=Facesitting [3]=Cowgirl [4]=Doggy — 對應 PriceGroup 枚舉，null 表示尚未設定
            public string ALifeMode;     // "Normal" 或 "Prostitute" — 管理此 NPC 的 A-Life 系統模式
            public int    Cash;           // 隨身攜帶現金 — 死亡時掉落
            public int    Stored;         // 存款存款 — 死亡時不會丟失
            public long   LastSimUtc;     // 該 NPC 上次離線模擬的 UTC 時間戳
            public bool   CivilianRejected; // 賣春模式：平民 NPC 拒絕提議後設為 true (鎖定答案)
            public bool   CimPreference;     // 若該 NPC 喜歡內射則設為 true (初始化後不變)
            public long   KnownTopics;       // bitmask — bits 0-5: b0 items 1-6; bits 6-11: b1 items 0-5; bits 12-16: b2 items 0-4; bits 17-23: b3 items 0-6; bits 24-30: b5 items 0-6; bits 31-37: b6 items 0-6. Re-asking costs rep
            public string LongTermGoal;      // 永久人生抱負 — 首次遭遇時分配且不變：
                                             //   Casual:       "致富"|"尋找愛情"|"保持自由"|"享受樂趣"|"生存"|"尋找保護"
                                             //   Prostitution: "致富"|"GetOut"|"生存"|"尋找保護"

            public ALifePedData(string fp)
            {
                Fingerprint              = fp;
                Name                     = "";
                Reputation               = 0;
                Relationship             = "Stranger";
                LastPositiveRelationship = "";
                Personality              = "";
                HasMet                   = false;
                HasMetSecondTime         = false;
                Voice                    = "";
                HookerAnim               = "";
                IsHooker                 = false;
                WasHooker                = false;
                HookerPermaReject        = false;
                NameKnown                = false;
                Mood                     = "Relaxed";
                PrefBJ                   = null;
                PrefRough                = null;
                HookerPrices             = null;
                ALifeMode                = "Normal";
                Cash                     = 0;
                Stored                   = 0;
                LastSimUtc               = DateTime.UtcNow.Ticks;
                CivilianRejected         = false;
                CimPreference            = false;
                KnownTopics              = 0;
                LongTermGoal             = "";  // assigned after personality is set, via AssignLongTermGoal()
            }
        }

        private class PendingDeadPed
        {
            public Ped  Ped;          // the dead ped reference
            public bool PlayerKilled; // player was the last to damage her before death
            public bool UnarmedKill;  // killed with WEAPON_UNARMED
            public int  FirstSeenAt;  // GameTime when first queued
            public bool CorpseHit;    // true if player hit the corpse after death
            public bool Survived;     // true after Phase A granted the 80% survive — stays in dict until despawn
        }

        // ── Persistent voice (fights GTA's periodic voice resets) ─────────────

        // ================================================================
        //  STATIC & CONSTANT DATA
        // ================================================================

        private static readonly string[] FEMALE_NAMES = {
            "Aaliyah", "Adriana", "Alexa", "Alice", "Alina", "Amber", "Amelia", "Anastasia", "Angela", "Angelina",
            "Anna", "Aria", "Ariana", "Ashley", "Audrey", "Aurora", "Autumn", "Ava", "Bailey", "Bella",
            "Bianca", "Brianna", "Brooke", "Camila", "Carmen", "Caroline", "Cassandra", "Chloe", "Claire", "Crystal",
            "Daisy", "Dakota", "Daniela", "Daphne", "Delilah", "Destiny", "Diana", "Elena", "Elisa", "Ember",
            "Emily", "Emma", "Eva", "Faith", "Fiona", "Francesca", "Gabrielle", "Gemma", "Giselle", "Grace",
            "Hailey", "Haley", "Hannah", "Harper", "Hazel", "Heather", "Holly", "Isabelle", "Ivy", "Jade",
            "Jasmine", "Jenna", "Jessica", "Julia", "Juliana", "Kara", "Katerina", "Kayla", "Keira", "Kendall",
            "Kendra", "Khloe", "Kimber", "Kira", "Layla", "Leah", "Lena", "Lily", "Lola", "Luna",
            "Lydia", "Mackenzie", "Ana", "Madison", "Maria", "Marina", "Maya", "Megan", "Melody", "Mia",
            "Michelle", "Mila", "Miranda", "Molly", "Morgan", "Nadia", "Natalie", "Natasha", "Nicole", "Nina",
            "Nora", "Olivia", "Paige", "Penelope", "Peyton", "Quinn", "Rachel", "Raven", "Rebecca", "Riley",
            "Rosa", "Ruby", "Sadie", "Samantha", "Sara", "Savannah", "Scarlett", "Selena", "Serena", "Sierra",
            "Sienna", "Skylar", "Sophia", "Stella", "Summer", "Tara", "Taylor", "Valentina", "Vanessa", "Victoria",
            "Violet", "Vivian", "Whitney", "Willow", "Yasmine", "Zara", "Zoe", "Maxine", "Sophie", "Maya", "Jocelyn", 
            "Caitlyn", "Kelsey", "Cassidy", "Tiffany", "Sierra", "Aubrey", "Trinity", "Kylie", "Mackenzie", "Payton", 
            "Rylee", "Morgan", "Piper", "Reagan", "Sage", "Skyler", "Teagan", "Zoe", "Addison", "Alaina", "Alison", "Allison", 
            "Alyssa", "Amaya", "Amberly", "Amira", "Annie", "Arianna", "Ashlyn", "Amanda", "Atara", "Averie", "Aviana", "Avril"
        };

        private static readonly PersonalityProfile[] PERSONALITIES = {
            //                                         Fri   Con   Grd   Pat   Risk  Att   Agg
            new PersonalityProfile { Name="Shy",        Friendliness=0.55, Confidence=0.10, Greed=0.20, Patience=0.40, Riskiness=0.15, Attachment=0.70, Aggressiveness=0.10 },
            new PersonalityProfile { Name="Flirty",     Friendliness=0.85, Confidence=0.65, Greed=0.30, Patience=0.60, Riskiness=0.60, Attachment=0.50, Aggressiveness=0.30 },
            new PersonalityProfile { Name="Dominant",   Friendliness=0.45, Confidence=0.95, Greed=0.40, Patience=0.50, Riskiness=0.55, Attachment=0.35, Aggressiveness=0.85 },
            new PersonalityProfile { Name="Cold",       Friendliness=0.15, Confidence=0.70, Greed=0.55, Patience=0.25, Riskiness=0.25, Attachment=0.20, Aggressiveness=0.60 },
            new PersonalityProfile { Name="Sweet",      Friendliness=0.90, Confidence=0.35, Greed=0.10, Patience=0.75, Riskiness=0.40, Attachment=0.85, Aggressiveness=0.10 },
            new PersonalityProfile { Name="Gold Digger",Friendliness=0.60, Confidence=0.75, Greed=0.95, Patience=0.45, Riskiness=0.70, Attachment=0.25, Aggressiveness=0.45 },
            new PersonalityProfile { Name="Street Smart",Friendliness=0.40, Confidence=0.80, Greed=0.65, Patience=0.55, Riskiness=0.50, Attachment=0.30, Aggressiveness=0.65 },
            new PersonalityProfile { Name="Party Girl", Friendliness=0.80, Confidence=0.70, Greed=0.35, Patience=0.50, Riskiness=0.80, Attachment=0.40, Aggressiveness=0.35 },
            new PersonalityProfile { Name="Romantic",   Friendliness=0.85, Confidence=0.40, Greed=0.15, Patience=0.70, Riskiness=0.35, Attachment=0.90, Aggressiveness=0.10 },
            new PersonalityProfile { Name="Sarcastic",  Friendliness=0.35, Confidence=0.75, Greed=0.45, Patience=0.35, Riskiness=0.45, Attachment=0.30, Aggressiveness=0.50 },
            new PersonalityProfile { Name="依賴",      Friendliness=0.70, Confidence=0.20, Greed=0.30, Patience=0.80, Riskiness=0.30, Attachment=0.95, Aggressiveness=0.20 },
            new PersonalityProfile { Name="Independent",Friendliness=0.45, Confidence=0.85, Greed=0.25, Patience=0.40, Riskiness=0.50, Attachment=0.15, Aggressiveness=0.65 },
            new PersonalityProfile { Name="嫉妒",    Friendliness=0.50, Confidence=0.55, Greed=0.50, Patience=0.30, Riskiness=0.40, Attachment=0.80, Aggressiveness=0.75 },
            new PersonalityProfile { Name="Chaotic",    Friendliness=0.55, Confidence=0.60, Greed=0.50, Patience=0.20, Riskiness=0.90, Attachment=0.45, Aggressiveness=0.90 },
            new PersonalityProfile { Name="Manipulative",Friendliness=0.65, Confidence=0.90, Greed=0.85, Patience=0.60, Riskiness=0.65, Attachment=0.20, Aggressiveness=0.40 },
            new PersonalityProfile { Name="Aggressive", Friendliness=0.10, Confidence=0.90, Greed=0.60, Patience=0.15, Riskiness=0.70, Attachment=0.25, Aggressiveness=0.95 },
            new PersonalityProfile { Name="玩鬧",    Friendliness=0.80, Confidence=0.55, Greed=0.20, Patience=0.65, Riskiness=0.75, Attachment=0.55, Aggressiveness=0.25 },
            new PersonalityProfile { Name="Mysterious", Friendliness=0.30, Confidence=0.70, Greed=0.40, Patience=0.50, Riskiness=0.55, Attachment=0.40, Aggressiveness=0.45 },
            new PersonalityProfile { Name="Classy",     Friendliness=0.65, Confidence=0.80, Greed=0.70, Patience=0.55, Riskiness=0.30, Attachment=0.45, Aggressiveness=0.25 },
            new PersonalityProfile { Name="Unstable",   Friendliness=0.45, Confidence=0.50, Greed=0.55, Patience=0.10, Riskiness=0.95, Attachment=0.60, Aggressiveness=0.80 },
        };

        /// <summary>Look up a personality profile by name. 傳回 null if not found.</summary>

        private static readonly string[] HOOKER_VOICES = {
            "S_F_Y_HOOKER_01_WHITE_FULL_01",
            "S_F_Y_HOOKER_01_WHITE_FULL_02",
            "S_F_Y_HOOKER_01_WHITE_FULL_03",
            "S_F_Y_HOOKER_02_WHITE_FULL_01",
            "S_F_Y_HOOKER_02_WHITE_FULL_02",
            "S_F_Y_HOOKER_02_WHITE_FULL_03",
            "S_F_Y_HOOKER_03_BLACK_FULL_01",
            "S_F_Y_HOOKER_03_BLACK_FULL_02",
            "S_F_Y_HOOKER_03_BLACK_FULL_03"
        };

        private static readonly string[] HOOKER_APPROACH_DICTS = {
            "mini@hookers_spvanilla",
            "mini@hookers_spcokehead",
            "mini@hookers_spcrackhead",
            "mini@hookers_spfrench"
        };
        private static readonly string[] HOOKER_SEXY_ANIMS = {
            "idle_a", "idle_b", "idle_c", "idle_d"
        };

        // ── Menu ───────────────────────────────────────────────

        private static readonly int[][] PRICE_RANGES = {
            new[] { 20, 80 },   // Kissing:      $20–$80
            new[] { 20, 60 },   // Blowjob:      $20–$60
            new[] { 20, 60 },   // Facesitting:   $20–$60
            new[] { 40, 80 },   // Cowgirl:       $40–$80
            new[] { 40, 100 }   // Doggy:         $40–$100
        };
        // Per-group success chance when negotiating (flat, per attempt)
        private static readonly double[] NEGOTIATE_CHANCE = {
            0.35,   // Kissing:      35%
            0.40,   // Blowjob:      40%
            0.50,   // Facesitting:   50%
            0.30,   // Cowgirl:       30%
            0.20    // Doggy:         20%
        };
        // Per GROUP: chance of being AVAILABLE (rolled once per ped)
        // Index matches PriceGroup enum: Kissing, Blowjob, Facesitting, Cowgirl, Doggy
        private static readonly double[] GROUP_AVAIL_CHANCE = {
            0.15,   // Kissing:      15% available
            0.60,   // Blowjob:      60% available  (all 3 BJ variants)
            0.20,   // Facesitting:   20% available
            0.55,   // Cowgirl:       55% available  (all 4 cowgirl variants)
            0.40    // Doggy:         40% available  (both doggy variants)
        };
        // pedHandle → prices[5] and locks[5]

        // ══════════════════════════════════════════════
        // A-LIFE EXCLUSIVE HOOKER HELPERS
        // ══════════════════════════════════════════════

        /// <summary>
        /// Base prices for exclusive A-Life hooker services, one entry per PriceGroup.
        /// Indices mirror PriceGroup enum: [0]=Kissing [1]=Blowjob [2]=Facesitting [3]=Cowgirl [4]=Doggy.
        /// Car BJ resolves to Blowjob (1), Car Sex resolves to Cowgirl (3) — same as sandbox.
        /// </summary>
        private static readonly int[][] ALIFE_HOOKER_BASE = {
            new[] {  60, 110 },  // [0] Kissing
            new[] {  80, 150 },  // [1] Blowjob  (also Car BJ)
            new[] {  80, 130 },  // [2] Facesitting
            new[] { 130, 220 },  // [3] Cowgirl/Sex  (also Car Sex)
            new[] { 140, 240 },  // [4] Doggy
        };

        /// <summary>
        /// Base prices for Prostitution A-Life services, one entry per PriceGroup.
        /// Closer to sandbox PRICE_RANGES midpoints than the exclusive casual-hooker table.
        /// </summary>
        private static readonly int[][] PROST_ALIFE_BASE = {
            new[] {  20,  70 },  // [0] Kissing      (sandbox $20–$80)
            new[] {  20,  55 },  // [1] Blowjob      (sandbox $20–$60)
            new[] {  20,  55 },  // [2] Facesitting  (sandbox $20–$60)
            new[] {  35,  70 },  // [3] Cowgirl/Sex  (sandbox $40–$80)
            new[] {  35,  80 },  // [4] Doggy        (sandbox $40–$100)
        };

        private readonly string[] carServiceItems = {
            "口交", "性愛"
        };

        private readonly string[] sitDownSubItems = {
            "坐姿服務",
            "躺姿服務"
        };

        // Standing Services: sub-menu index → flat streetActionIdx
        private readonly string[] standingServiceLabels = { "親吻", "口交", "靠牆性愛", "後入式", "粗暴後入" };
        private readonly int[]    standingServiceMap    = { 0, 2, 5, 10, 11 };
        // Sitting Services: sub-menu index → flat streetActionIdx
        private readonly string[] sittingServiceLabels  = { "口交", "騎乘式", "反向騎乘" };
        private readonly int[]    sittingServiceMap     = { 3, 6, 7 };
        // Laying Services: sub-menu index → flat streetActionIdx
        private readonly string[] layingServiceLabels   = { "坐姿服務", "口交", "騎乘式", "反向騎乘" };
        private readonly int[]    layingServiceMap      = { 1, 4, 8, 9 };

        private const string SETTINGS_FILE = "scripts\\CinnamonCoffee.cfg";
        private const string ALIFE_FILE = "scripts\\CinnamonCoffeeALife.ini";

        private const string MOD_VERSION      = "v1.3.2";
        private const string UPDATE_API_URL   = "https://api.github.com/repos/HSSkyBoy/NoCoolCoffee/releases/latest";
        private string _updateAvailableMsg    = null; // non-null 時在 HUD 顯示更新提示
        private bool   _updateMsgShown        = false;

        private static readonly Dictionary<int, int[]> priceCache = new Dictionary<int, int[]>();
        // pedHandle → locks[5] per group — true = won't go lower
        private static readonly Dictionary<int, bool[]> priceLocked = new Dictionary<int, bool[]>();
        // pedHandle → bool[5] per group (true = entire group available for this ped)
        private static readonly Dictionary<int, bool[]> availabilityCache = new Dictionary<int, bool[]>();
        // Sandbox Prostitution (no A-Life): true = she'll do all services outdoors; false = needs car/interior
        private static readonly Dictionary<int, bool> _outdoorWillingCache = new Dictionary<int, bool>();
        // Per-ped throat preference: random 0.0–0.5 rolled once per woman, reused on rehire
        private static readonly Dictionary<int, double> _bjThroatChanceCache = new Dictionary<int, double>();
        private static readonly Dictionary<int, string> voiceCache = new Dictionary<int, string>();
        private static readonly Dictionary<int, string> approachDictCache = new Dictionary<int, string>();
        private static readonly Random rng = new Random();
        private static readonly Dictionary<int, bool> _approachRejectionCasual = new Dictionary<int, bool>(); // pedHandle → true=casual (idle_outro), false=pissed (idle_reject); Sandbox only — persists per ped for the game session

        private const int VOICE_INTERVAL = 7000;
        private const int KEY_COOLDOWN_FRAMES = 20;

        // ── Custom BJ sounds ───────────────────────────────────────────────────────
        private const string SOUNDS_DIR = "scripts\\CinnamonCoffeeSounds\\";
        private const float  BJ_VOLUME          = 0.10f; // amplitude multiplier (0.35 ≈ -9 dB; raise/lower to taste)
        private const int    BJ_SOUND_INTERVAL  = 1500;  // base ms between custom BJ sounds (scales with anim speed)
        private const int    GTA_SPEECH_EST_DUR = 3000;  // estimated GTA speech line duration in ms

        // Slap intervals per action [ms]: slow@1x / mid@2x / fast@3x (piecewise linear)
        // Car Sex — Normal vehicle
        private const int SLAP_CAR_SEX_SLOW   = 710;   private const int SLAP_CAR_SEX_MID   = 380;   private const int SLAP_CAR_SEX_FAST   = 250;
        // Car Sex — Low vehicle only
        private const int SLAP_LOW_SEX_SLOW   = 935;   private const int SLAP_LOW_SEX_MID   = 480;   private const int SLAP_LOW_SEX_FAST   = 350;
        // Sitting Cowgirl (idx 6), Reverse Cowgirl (idx 7) — street only
        private const int SLAP_COW_SEX_SLOW   = 710;   private const int SLAP_COW_SEX_MID   = 380;   private const int SLAP_COW_SEX_FAST   = 250;
        // Wall Sex (idx 5) — standalone
        private const int SLAP_WALL_SEX_SLOW  = 770;   private const int SLAP_WALL_SEX_MID  = 360;   private const int SLAP_WALL_SEX_FAST  = 250;
        // Laying Cowgirl (idx 8), Laying Reversed Cowgirl (idx 9)
        private const int SLAP_LAY_COW_SLOW   = 700;  private const int SLAP_LAY_COW_MID   = 280;   private const int SLAP_LAY_COW_FAST   = 190;
        // Laying Cowgirl / Laying Reversed Cowgirl — Sinkra anim replacement (cowgirl_clip)
        private const int SLAP_SINKRA_LAY_COW_SLOW = 710;  private const int SLAP_SINKRA_LAY_COW_MID = 380;   private const int SLAP_SINKRA_LAY_COW_FAST = 250;
        // Doggy Style (idx 10)
        private const int SLAP_DOGGY_SLOW     = 620;   private const int SLAP_DOGGY_MID     = 430;   private const int SLAP_DOGGY_FAST     = 275;
        // Rough Doggy Style (idx 11)
        private const int SLAP_RDOGGY_SLOW    = 385;   private const int SLAP_RDOGGY_MID    = 250;   private const int SLAP_RDOGGY_FAST    = 180;
        // Kissing (idx 0) — interval between kiss sounds
        private const int KISS_SOUND_SLOW    = 3500;   private const int KISS_SOUND_MID    = 2200;   private const int KISS_SOUND_FAST    = 1200;

        private const int PING_TIMEOUT   = 8000; // ms to wait for pong response
        private const int AUTO_PING_INTERVAL = 10000; // ms between automatic re-pings when bridge was last connected

        private const int HEAL_INTERVAL = 3000; // Heal tick every 3 seconds
        private const int HEAL_AMOUNT = 2; // Base HP per tick (very minimal)
        private const int FINISH_HEAL = 1; // HP boost on finish
        private const int REP_MAX =  100; // Reputation ceiling
        private const int REP_MIN =   -2; // Reputation floor (hostile threshold)

        private const string VEH_NORM_DICT = "mini@prostitutes@sexnorm_veh";
        private const string VEH_NORM_FP_DICT = "mini@prostitutes@sexnorm_veh_first_person";
        private const string VEH_LOW_DICT  = "mini@prostitutes@sexlow_veh";
        private const string VEH_LOW_FP_DICT = "mini@prostitutes@sexlow_veh_first_person";

        // ================================================================
        //  實例欄位 (INSTANCE FIELDS)
        // ================================================================

        // -- 腳本狀態 --
        private ScriptState state = ScriptState.Idle;
        private bool playerWasInVehicle = false;
        private bool _backseatEntryPending = false; // true while TrySwapSeats outside-entry is in progress — suppresses auto passenger-seat entry
        private bool _backseatCarSex = false; // true during backseat car sex — uses random@drunk_driver_2 anims
        private int _vehEntryStartTime = 0; // GameTime when vehicle entry task started for stuck timeout check
        private int _lastVehEnterTaskTime = 0; // GameTime when TaskGirlEnterVehicle was last issued
        private Mode mode = Mode.None;
        private VehicleAnimType vehicleAnimType = VehicleAnimType.Normal;

        // ── Girl ───────────────────────────────────────────────

        // -- Approach state --
        private Ped girl = null;
        private bool hasGirl = false;
        private Blip _girlBlip = null;

        // ── Approach (prostitution mode) ───────────────────────
        private int approachPhase = 0;        // 0=walking, 1=intro, 2=idle_wait, 3=outro(accept), 4=reject, 5=reject_outro, 6=sexy pose
        private int jHoldStart = 0;           // GameTime when J was first held (0 = not held)
        private int approachAnimWait = 0;     // GameTime: when current approach anim finishes
        private int approachSexySwapTime = 0; // GameTime: next sexy anim swap
        private int approachDecisionTimer = 0; // GameTime: when 45s patience runs out
        private string approachDict = "";     // Which hooker dict was randomly chosen
        private Vector3 approachTargetPos;    // World position the girl walks to (window or player pos)
        private float approachHeading;        // Heading toward player for approach animations

        // -- Menu state --
        private MenuLevel menuLevel = MenuLevel.None;
        private int menuIndex = 0;
        private bool negotiateMode = false; // Per-item swipe: false=Accept, true=Negotiate
        private MenuLevel _serviceSubParent = MenuLevel.None; // Which menu opened a street service sub-menu (Services or Escalate)

        // -- Settings toggles --
        private bool dildoEnabled = true;
        private bool driveableCar = false;
        private bool showTutorialMessages = true;
        private bool slapSoundsEnabled = true;
        private bool bjSoundsEnabled   = true;
        private bool interiorLightEnabled = false;
        private bool carEngineEnabled = false;
        private bool streetSitAnimEnabled = true;
        private bool femaleServiceVoice = true;
        private bool femaleMoanVoice = true;
        private bool femaleEndVoice = true;
        private bool sandboxMode = true;
        private bool aLifeMode   = false; // A-Life AI mode (false = Sandbox, true = A-Life)

        // -- Vehicle override lists --
        private HashSet<int> overwriteLowVehicle = new HashSet<int>();
        private HashSet<int> overwriteNormalVehicle = new HashSet<int>();
        private HashSet<int> backseatBlacklist = new HashSet<int>(); // vehicles that should never use backseat mode (vans, etc.)

        // -- Street sitting proposition (Sitting/Laying Services on street) --
        private bool      _streetSitPropActive    = false;  // Proposition sequence active
        private int       _streetSitPropPhase     = 0;      // -1=sit-down, 0=prop-enter, 1=prop-loop, 2=idle, 3=prop-exit, 4=stand-up
        private int       _streetSitPropWaitTime  = 0;      // GameTime when current phase anim finishes
        private Vector3   _streetSitOriginPos;              // Saved player position before sit-down
        private float     _streetSitOriginHeading;          // Saved player heading before sit-down
        private MenuLevel _streetSitCategory = MenuLevel.None; // Which category triggered the sequence
        private int       _streetOutroFadeAt = 0;              // GameTime for delayed outro fade-out (rev cowgirl)
        private MenuLevel _streetSitExitReturnMenu = MenuLevel.None; // Menu to reopen after stand-up finishes
        private int       _streetSitExitReturnIdx  = 0;              // menuIndex to restore after stand-up

        // -- Animation state --
        private string currentMaleAnim, currentFemaleAnim;
        private string currentMaleDict, currentFemaleDict;
        private string currentAction = ""; // "blowjob", "sex", "kissing"
        private int animPhase = 0; // 0=intro_p1, 1=intro_p2, 2=loop, 3=outro_p1, 4=outro_p2
        private int animWaitTime = 0; // Game.GameTime timestamp: don't advance phase until this time

        // -- A-Life session identity & conversation state --
        private readonly Dictionary<string, ALifePedData> _aLifePeds = new Dictionary<string, ALifePedData>();
        private string _currentGirlFp  = null; // raw fingerprint of the currently hired girl (used for ped identity comparisons)
        private string _currentGirlKey  = null; // namespaced dict key: "C:"+fp for Casual A-Life, "P:"+fp for Prost A-Life
        private Ped    _approachCandidate = null;
        private int    _approachSelectedCluster = -1; // 0-4, cluster chosen in Approach menu
        private int    _lastApproachLineIdx    = -1;  // line index chosen in ApproachLine menu; held until ShowApproachAcceptSubtitle consumes it
        private bool   _approachSecondChance   = false; // true after first rejection, player may try once more
        private int    _leaveSelectedCluster   = -1; // 0-4, cluster chosen in Leave menu
        private int    _convSelectedBranch      = -1; // 0-6, conversation branch chosen in Conversation menu
        private int    _intimacySelectedBranch  = -1; // 0=AskPreferences, 1=TestWaters
        private bool   _escalateConsented       = false; // true once she agrees to sex this session; resets on dismiss
        private bool   _escalateRejectedOnce     = false; // true after first escalate reject this session
        private bool   _escalatePermaRejected    = false; // true after perma-rejection (90% chance on second ask); forces re-recruit
        private bool   _invitedToVehicle      = false; // true once she accepts vehicle invite this session; resets on dismiss
        private bool   _aLifeHookerServices   = false; // true when A-Life exclusive hooker routes through Services menu
        private bool   _finishExecutePending = false; // set when Finish dialogue closes → run actual finish next tick
        private int    _finishExecuteAt = 0;           // GameTime after which the pending finish should actually execute
        private bool   _finishInside = false;          // true if player chose to finish inside
        private bool   _finishForced = false;          // true if player forced it without asking (trust break)
        private bool   _finishAsked  = false;          // true once Ask was used this session — hides the Ask item
        private bool   _finishBroke  = false;          // true when player can't pay the agreed finish-inside price (no swallow, sets Annoyed mood)
        private bool   _finishInsideFreePass = false;  // true when hooker waived her finish-inside fee; skip payment block at finish time

        // -- Post-sex voice sequence --
        private int       _postSexPhase       = 0;             // 0=idle, 1=polling IS_AMBIENT_SPEECH_PLAYING, 2=delay before reopen
        private int       _postSexWaitUntil   = 0;             // GameTime: when phase-2 delay expires
        private MenuLevel _postSexPendingMenu = MenuLevel.None; // menu to open after voice sequence completes

        private Dictionary<string, bool[]> _aLifeHookerPriceLocked = new Dictionary<string, bool[]>(); // fp → bool[5], one lock per PriceGroup

        // -- Give Money submenu state --
        private int[] _giveMoneyAmounts = new int[9]; // 9 denomination rows: $1,$10,$100,$1K,$10K,$100K,$1M,$10M,$100M
        private int   _giveMoneyRow = 0;              // currently highlighted denomination row (0-8)

        // -- Prostitution A-Life --
        // ── Prostitution A-Life ───────────────────────────────────────────────────────
        private HashSet<int> _hookerModelHashes      = new HashSet<int>(); // pre-hashed s_f_y_hooker_01..99 for fast model detection
        private bool         _prostAutoApproachActive   = false; // true when a hooker initiated the approach (not the player)
        private int          _prostAutoApproachCooldown = 0;     // GameTime: suppress auto-approach scanning until this time
        private int          _offlineSimAt              = 0;     // GameTime when next offline simulation tick fires
        private int          _deadPedScanAt             = 0;     // GameTime when next dead-ped sweep fires

        // -- Hit reaction (fight-or-flight) --
        // ── Hit reaction (fight-or-flight) ────────────────────────────────────
        private float _girlLastHealth = -1f;   // tracks vitality (Health + Armor) to detect hits
        private Ped    _pendingCashDropPed    = null;  // ped queued for cash drop (current session girl after being hit)
        private int    _pendingCashDropAmount = 0;     // amount to drop when she dies
        private string _pendingCashDropKey         = null;  // ALife dict key for the pending ped
        private int    _pendingCashDropWeaponHash    = 0;     // player's weapon hash captured when the hit was detected
        private HashSet<string> _resolvedDownedKeys = new HashSet<string>(); // keys already processed for this death event
        // Queued dead peds awaiting fate resolution (one tick window to check for corpse hit)
        private Dictionary<string, PendingDeadPed> _pendingDeadPeds = new Dictionary<string, PendingDeadPed>();

        // -- Voice & facial expression reapply --
        private string _girlCurrentVoice = null; // resolved voice for current girl, reapplied every 3s
        private int    _voiceReapplyAt   = 0;    // GameTime when to next reapply SET_AMBIENT_VOICE_NAME
        private int    _faceReapplyAt    = 0;    // GameTime when to next reapply facial expression override

        // -- Keyboard controls --
        private Keys masterKey = Keys.J;            private string masterKeyDisplay = "J";
        // ── Menu Navigation Keys (configurable via cfg) ──────────
        private Keys menuUpKey     = Keys.NumPad8;  private string menuUpKeyDisplay     = "NUM8";
        private Keys menuDownKey   = Keys.NumPad2;  private string menuDownKeyDisplay   = "NUM2";
        private Keys menuSelectKey = Keys.NumPad5;  private string menuSelectKeyDisplay = "NUM5";
        private Keys menuBackKey   = Keys.NumPad0;  private string menuBackKeyDisplay   = "NUM0";
        private Keys speedUpKey    = Keys.NumPad6;  private string speedUpKeyDisplay    = "NUM6";
        private Keys speedDownKey  = Keys.NumPad4;  private string speedDownKeyDisplay  = "NUM4";
        private Keys speedResetKey = Keys.NumPad1;  private string speedResetKeyDisplay = "NUM1";

        // -- Controller controls --
        private bool    _usingController       = false;   // True when last input was gamepad
        private int     _menuControlCooldown   = 0;       // Frame cooldown for controller menu nav
        private GTA.Control masterControl          = GTA.Control.FrontendRight;            private string masterControlDisplay       = "D-RT";
        private GTA.Control masterControlModifier  = GTA.Control.FrontendLb;              private string masterControlModifierDisplay = "L1";
        private GTA.Control menuUpControl          = GTA.Control.FrontendUp;              private string menuUpControlDisplay       = "D-UP";
        private GTA.Control menuDownControl        = GTA.Control.FrontendDown;            private string menuDownControlDisplay     = "D-DN";
        private GTA.Control menuSelectControl      = GTA.Control.FrontendAccept;          private string menuSelectControlDisplay   = "A";
        private GTA.Control menuBackControl        = GTA.Control.FrontendCancel;          private string menuBackControlDisplay     = "B";
        private GTA.Control speedUpControl         = GTA.Control.FrontendRight;           private string speedUpControlDisplay      = "D-RT";
        private GTA.Control speedDownControl       = GTA.Control.FrontendLeft;            private string speedDownControlDisplay    = "D-LT";
        private GTA.Control speedResetControl      = GTA.Control.FrontendX;              private string speedResetControlDisplay   = "X";

        // -- Input display helpers --
        private string MasterDisplay     { get { return _usingController ? masterControlModifierDisplay + "+" + masterControlDisplay : masterKeyDisplay; } }
        private string MenuUpDisplay     { get { return _usingController ? menuUpControlDisplay     : menuUpKeyDisplay; } }
        private string MenuDownDisplay   { get { return _usingController ? menuDownControlDisplay   : menuDownKeyDisplay; } }
        private string MenuSelectDisplay { get { return _usingController ? menuSelectControlDisplay : menuSelectKeyDisplay; } }
        private string MenuBackDisplay   { get { return _usingController ? menuBackControlDisplay   : menuBackKeyDisplay; } }
        private string SpeedUpDisplay    { get { return _usingController ? speedUpControlDisplay    : speedUpKeyDisplay; } }
        private string SpeedDownDisplay  { get { return _usingController ? speedDownControlDisplay  : speedDownKeyDisplay; } }
        private string SpeedResetDisplay { get { return _usingController ? speedResetControlDisplay : speedResetKeyDisplay; } }

        // -- Device (Autoblow bridge) settings --
        private string deviceToken = ""; // Autoblow device token (read from cfg)
        private bool tokenKeyboardOpen = false; // true while GTA's on-screen keyboard is open for token input
        private int strokeMin = 5;  // device stroke lower bound (0–100); passed to bridge in every motion message
        private int strokeMax = 65; // device stroke upper bound (0–100); passed to bridge in every motion message
        private int readyAndFinishDelay = 2000; // ms the device parks at top after ready/finish before stopping

        // -- HUD status overlay --
        private string _hudStatusText    = null; // null = not shown
        private int    _hudStatusExpireAt = 0;   // GameTime when it should disappear

        // -- Timing --
        private int keyCooldown = 0;
        private int lastVoiceTime = 0;
        private int lastShakeTime = 0;

        // -- BJ custom sound state --
        private string[]  _bjSuckFiles       = null;  // BJSuck_1..10.wav
        private string    _bjThroatIntroFile  = null;  // BJThroat_Intro.wav
        private string    _bjThroatFile       = null;  // BJThroat.wav
        private string    _bjSwallowFile      = null;  // BJSwallow.wav
        private string    _bjSpitFile         = null;  // BJSpit.wav
        private string    _bjPantsZipFile     = null;  // BJPantsZip.wav
        private readonly Dictionary<string,int>    _soundDurations = new Dictionary<string,int>();
        private readonly Dictionary<string,byte[]> _bjWavData      = new Dictionary<string,byte[]>(); // volume-adjusted WAV bytes
        private System.Media.SoundPlayer _bjPlayer = null;
        private int  _bjSoundBusyUntil  = 0;  // GameTime when the current custom BJ WAV finishes
        private int  _lastBjSoundTime   = 0;  // GameTime of last custom BJ WAV fired
        private int  _lastGtaSpeechTime = 0;  // GameTime of last GTA speech line
        private bool _bjThroatPending   = false; // follow-up BJThroat.wav scheduled after BJThroat_Intro
        private int  _bjSpitFacialStopAt = 0;     // GameTime to stop spit lip anim
        private int  _bjThroatAt        = 0;    // GameTime to fire the follow-up
        private float _preThroatSpeed       = 1.0f;  // animSpeed snapshot before throat lock
        private bool  _throatSpeedLocked    = false; // true while BJThroat is forcing 1.80x
        private int   _throatLockAt         = 0;     // GameTime to apply speed lock (1s after intro starts)
        private int   _throatSequenceEndAt  = 0;     // GameTime when BJThroat.wav finishes (lock released)

        // -- Slap & rumble state --
        private string[] _sexSlapFiles     = null;  // SexSlap_1..11.wav
        private System.Media.SoundPlayer _slapPlayer   = null;
        private int  _lastSlapSoundTime = 0;  // GameTime of last slap sound fired
        private string[] _kissSoundFiles   = null;  // Kissing_1..5.wav
        private System.Media.SoundPlayer _kissPlayer   = null;
        private int  _lastKissSoundTime    = 0;  // GameTime of last kiss sound fired
        private int  _lastRumbleTime     = 0;  // GameTime of last vibration pulse fired
        private bool _rumblePulseActive  = false; // true while the short ON burst is playing
        private bool _finishBurstFired   = false; // true only when the finish burst was fired (not loop pulses)
        private int pingPendingTime = 0;        // Game.GameTime when last ping was sent (0 = none pending)
        private int lastAutoPingTime = 0;        // Game.GameTime of last automatic ping

        // -- Speed, device intensity & animation --
        private float animSpeed = 1.0f; // Loop animation speed multiplier (controls animation only)
        private float _femaleAnimSpeedScale = 1.0f; // Hidden speed multiplier applied on top of animSpeed (e.g. 2x for cowgirl_clip)
        private float defaultDeviceIntensity = 0.20f; // Per-act base device intensity at 1.0x anim speed
        private bool _deviceExcluded = false;        // True for acts where device must always be off
        // Per-act starting device intensities (0.0 = device off, 1.0 = max speed)
        private float carBjDeviceSpeed            = 0.20f;
        private float carSexDeviceSpeed           = 0.20f;
        private float carLowBjDeviceSpeed         = 0.20f;
        private float carLowSexDeviceSpeed        = 0.20f;
        private float standingBjDeviceSpeed       = 0.20f;
        private float sittingBjDeviceSpeed        = 0.20f;
        private float layingBjDeviceSpeed         = 0.20f;
        private float wallSexDeviceSpeed          = 0.20f;
        private float sittingCowgirlDeviceSpeed   = 0.20f;
        private float revCowgirlDeviceSpeed       = 0.20f;
        private float layingCowgirlDeviceSpeed    = 0.20f;
        private float layingRevCowgirlDeviceSpeed = 0.20f;
        private float layingSinkraDeviceSpeed     = 0.20f; // Sinkra cowgirl anim speed (Laying Cowgirl + Laying Rev Cowgirl)
        private float doggyDeviceSpeed            = 0.20f;
        private float roughDoggyDeviceSpeed       = 0.20f;
        private float deviceSpeedRamp              = 3.0f; // animSpeed at which device reaches 100%
        private int speedCooldown = 0; // Cooldown for speed change keys
        private readonly TelemetrySender _telemetry = new TelemetrySender();

        // -- Healing --
        private int lastHealTime = 0;
        private bool healPlayer = false;
        private bool healGirl = false;
        private float healRate = 1.0f;

        // -- Refund tracking --
        private int lastPaidAmount = 0;

        // -- Props --
        private Prop dildoProp = null;
        private int streetActionIdx = -1; // Track street action index for prop offsets
        private bool carPropActive = false;   // Currently in or transitioning to/from proposition pose
        private int carPropPhase = 0;         // 0=entering, 1=looping, 2=exiting
        private int carPropWaitTime = 0;      // GameTime when current prop anim finishes
        private MenuLevel _carPropPendingMenu    = MenuLevel.None; // menu to open once prop loop is ready
        private MenuLevel _carPropExitReturnMenu = MenuLevel.None; // menu to reopen after exit anim finishes
        private int       _carPropExitReturnIdx  = 0;              // menuIndex to restore after exit

        // -- Sinkra modded animation detection --
        private bool _sinkraCowgirlAvailable = false;
        private bool sinkraReplaceLaying        = true;  // use cowgirl@sinkra for Laying Cowgirl
        private bool sinkraReplaceRevLaying     = true;  // use cowgirl@sinkra for Laying Reversed Cowgirl
        private bool sinkraReplaceFacesitting   = true;  // use cowgirl@sinkra for Facesitting
        private HashSet<int> _sinkraLowerModels          = new HashSet<int>();              // models using lower sinkra offsets
        private Vector3      _sinkraLowerOffsetCowgirl    = new Vector3( 0.02f,  0.08f,  0.44f); // girl offset: Laying Cowgirl (lower models)
        private Vector3      _sinkraLowerOffsetRevCowgirl = new Vector3( 0.02f, -0.075f, 0.44f); // girl offset: Laying Rev Cowgirl (lower models)
        private Vector3      _sinkraLowerOffsetFacesit    = new Vector3(-0.05f, -0.50f,  0.48f); // girl offset: Facesitting (lower models)
        private Vector3      _wallBangLowerOffset         = new Vector3( 0.02f,  0.36f,  0.088f); // girl offset: Wall Banger (lower models)

        // -- Kissing ping-pong state --
        private bool _kissingReverse = false;           // true when kissing anim is playing backward during loop phase
        private bool _rdoggyReverse = false;             // true when rough doggy ping-pong is playing backward
        private bool _standingBjReverse = false;         // true when standing BJ ping-pong is playing backward
        private bool _sitCowgirlPulloutReverse = false;  // true when sitting cowgirl pull-out is playing proposition_to_sex in reverse
        private bool _carPulloutReverse = false;          // true when car BJ/sex pull-out is playing intro anims in reverse
        private int  _standingBjSwallowAt = 0;            // GameTime for delayed swallow during standing BJ finish

        // -- Finish-inside price cache (moved here from mid-method declaration) --
        private int _aLifeFinishInsidePrice = 0; // cached finish-inside price for current session (set by GetFinishInsideHookerPrice)

        // ================================================================
        //  CONSTRUCTOR
        // ================================================================

        public CinnamonCoffee()
        {
            Tick    += OnTick;
            KeyDown += OnKeyDown;
            Aborted += OnAborted;
            Interval = 0;

            LoadSettings();
            LoadALife();
            _telemetry.SetToken(deviceToken);
            _telemetry.SetStrokeRange(strokeMin, strokeMax, readyAndFinishDelay);
            LoadBjSounds();

            // 背景非同步更新檢測（不阻塞主線程）
            Task.Run(() => CheckForUpdate());

            // Detect optional modded animation dict for Laying Cowgirl / Laying Reversed Cowgirl.
            // Falls back to vanilla random@drunk_driver_2 if the dict is absent.
            _sinkraCowgirlAvailable = Function.Call<bool>(Hash.DOES_ANIM_DICT_EXIST, "cowgirl@sinkra");

            // Pre-hash s_f_y_hooker_01..99 for Prostitution A-Life model detection
            for (int _hi = 1; _hi <= 99; _hi++)
                _hookerModelHashes.Add(Function.Call<int>(Hash.GET_HASH_KEY, string.Format("s_f_y_hooker_{0:D2}", _hi)));
        }

        private void OnAborted(object sender, EventArgs e)
        {
            // Script reload / unload — clear animations and clean up so nothing is left stuck
            try { FullReset(); } catch { }
            // Explicitly clear the player's task so the animation stops immediately
            try
            {
                Ped player = Game.Player.Character;
                if (player != null && player.Exists())
                {
                    player.Task.ClearAll();
                    player.IsPositionFrozen = false;
                    Function.Call(Hash.SET_ENTITY_COLLISION, player, true, true);
                }
            }
            catch { }
        }

        private void CheckForUpdate()
        {
            try
            {
                string url = UPDATE_API_URL;
                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add("User-Agent", "NoCoolCoffee-Mod");
                    string json = wc.DownloadString(url);
                    // 簡單解析 "tag_name": "vX.X" ，不引入额外依賴
                    int idx = json.IndexOf("\"tag_name\"", StringComparison.Ordinal);
                    if (idx < 0) return;
                    int colon = json.IndexOf(':', idx);
                    if (colon < 0) return;
                    int q1 = json.IndexOf('"', colon);
                    if (q1 < 0) return;
                    int q2 = json.IndexOf('"', q1 + 1);
                    if (q2 < 0) return;
                    string latest = json.Substring(q1 + 1, q2 - q1 - 1);
                    if (!string.IsNullOrEmpty(latest) && latest != MOD_VERSION)
                    {
                        _updateAvailableMsg = "~y~[NoCoolCoffee] ~w~發現新版本: " + latest + " (當前: " + MOD_VERSION + ") https://github.com/HSSkyBoy/NoCoolCoffee";
                    }
                }
            }
            catch { }
        }

        // ════════════
        // SETTINGS I/O
        // ════════════

        // ================================================================
        //  PROFILE & A-LIFE LOOKUP HELPERS
        // ================================================================

        // Originally adjacent to PersonalityProfile data in the field declarations area.

        private static PersonalityProfile GetProfile(string name)
        {
            if (name == null) return null;
            foreach (PersonalityProfile p in PERSONALITIES)
                if (p.Name == name) return p;
            return null;
        }

        /// <summary>True when the ped's model hash is in the pre-built s_f_y_hooker_01..99 HashSet.</summary>
        private bool IsProstHookerModel(Ped p)
        {
            return p != null && p.Exists() && _hookerModelHashes.Contains(p.Model.Hash);
        }

        /// <summary>
        /// Rejection probability for a civilian (non-hooker model) approached in Prost A-Life mode.
        /// Base 60%: even the friendliest civilian is unlikely to agree — she's not a working girl.
        /// Aggressiveness raises the chance; Friendliness lowers it. Clamped to [30%, 95%].
        /// </summary>
        private double GetCivilianRejectionChance(ALifePedData d)
        {
            PersonalityProfile prof = GetProfile(d != null ? d.Personality : null);
            double agg = (prof != null) ? prof.Aggressiveness : 0.50;
            double fri = (prof != null) ? prof.Friendliness   : 0.50;
            double chance = 0.60 + (agg * 0.25) - (fri * 0.20);
            return Math.Max(0.30, Math.Min(0.95, chance));
        }

        /// <summary>傳回 true when a subtitle response string is positive (non-null and not ~r~ coloured).</summary>
        private static bool IsPositive(string s)
        {
            return s != null && !s.StartsWith("~r~");
        }

        // ================================================================
        //  METHODS
        // ================================================================

        /// <summary>傳回 two personality-matched walk-away lines (prefix ~r~name:~s~ added by caller).</summary>

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(SETTINGS_FILE))
                {
                    foreach (string line in File.ReadAllLines(SETTINGS_FILE))
                    {
                        string trimmed = line.Trim();
                        if (trimmed.StartsWith("dildoEnabled="))
                            dildoEnabled = trimmed.Substring(13).Trim().ToLower() == "true";
                        else if (trimmed.StartsWith("driveableCar="))
                            driveableCar = trimmed.Substring(13).Trim().ToLower() == "true";
                        else if (trimmed.StartsWith("showTutorialMessages="))
                            showTutorialMessages = trimmed.Substring(21).Trim().ToLower() == "true";
                        else if (trimmed.StartsWith("slapSoundsEnabled="))
                            slapSoundsEnabled = trimmed.Substring(18).Trim().ToLower() == "true";
                        else if (trimmed.StartsWith("bjSoundsEnabled="))
                            bjSoundsEnabled = trimmed.Substring(16).Trim().ToLower() == "true";
                        else if (trimmed.StartsWith("femaleServiceVoice="))
                            femaleServiceVoice = trimmed.Substring(19).Trim().ToLower() == "true";
                        else if (trimmed.StartsWith("femaleMoanVoice="))
                            femaleMoanVoice = trimmed.Substring(16).Trim().ToLower() == "true";
                        else if (trimmed.StartsWith("femaleEndVoice="))
                            femaleEndVoice = trimmed.Substring(15).Trim().ToLower() == "true";
                        else if (trimmed.StartsWith("sandboxMode="))
                            sandboxMode = trimmed.Substring(12).Trim().ToLower() == "true";
                        else if (trimmed.StartsWith("aLifeMode="))
                            aLifeMode = trimmed.Substring(10).Trim().ToLower() == "true";
                        else if (trimmed.StartsWith("interiorLightEnabled="))
                            interiorLightEnabled = trimmed.Substring(21).Trim().ToLower() == "true";
                        else if (trimmed.StartsWith("carEngineEnabled="))
                            carEngineEnabled = trimmed.Substring(17).Trim().ToLower() == "true";
                        else if (trimmed.StartsWith("streetSitAnimEnabled="))
                            streetSitAnimEnabled = trimmed.Substring(21).Trim().ToLower() == "true";
                        else if (trimmed.StartsWith("主按鍵="))
                        {
                            string keyStr = trimmed.Substring(10).Trim();
                            Keys parsed;
                            if (Enum.TryParse<Keys>(keyStr, true, out parsed))
                                masterKey = parsed;
                            else
                                masterKey = Keys.J;
                        }
                        else if (trimmed.StartsWith("主按鍵顯示="))
                            masterKeyDisplay = trimmed.Substring(17).Trim();
                        else if (trimmed.StartsWith("選單上移按鍵="))
                        {
                            string keyStr = trimmed.Substring(10).Trim();
                            Keys parsed;
                            if (Enum.TryParse<Keys>(keyStr, true, out parsed)) menuUpKey = parsed;
                        }
                        else if (trimmed.StartsWith("選單上移按鍵顯示="))
                            menuUpKeyDisplay = trimmed.Substring(17).Trim();
                        else if (trimmed.StartsWith("選單下移按鍵="))
                        {
                            string keyStr = trimmed.Substring(12).Trim();
                            Keys parsed;
                            if (Enum.TryParse<Keys>(keyStr, true, out parsed)) menuDownKey = parsed;
                        }
                        else if (trimmed.StartsWith("選單下移按鍵顯示="))
                            menuDownKeyDisplay = trimmed.Substring(19).Trim();
                        else if (trimmed.StartsWith("選單確認按鍵="))
                        {
                            string keyStr = trimmed.Substring(14).Trim();
                            Keys parsed;
                            if (Enum.TryParse<Keys>(keyStr, true, out parsed)) menuSelectKey = parsed;
                        }
                        else if (trimmed.StartsWith("選單確認按鍵顯示="))
                            menuSelectKeyDisplay = trimmed.Substring(21).Trim();
                        else if (trimmed.StartsWith("選單返回按鍵="))
                        {
                            string keyStr = trimmed.Substring(12).Trim();
                            Keys parsed;
                            if (Enum.TryParse<Keys>(keyStr, true, out parsed)) menuBackKey = parsed;
                        }
                        else if (trimmed.StartsWith("選單返回按鍵顯示="))
                            menuBackKeyDisplay = trimmed.Substring(19).Trim();
                        else if (trimmed.StartsWith("加速按鍵="))
                        {
                            string keyStr = trimmed.Substring(11).Trim();
                            Keys parsed;
                            if (Enum.TryParse<Keys>(keyStr, true, out parsed)) speedUpKey = parsed;
                        }
                        else if (trimmed.StartsWith("加速按鍵顯示="))
                            speedUpKeyDisplay = trimmed.Substring(18).Trim();
                        else if (trimmed.StartsWith("減速按鍵="))
                        {
                            string keyStr = trimmed.Substring(13).Trim();
                            Keys parsed;
                            if (Enum.TryParse<Keys>(keyStr, true, out parsed)) speedDownKey = parsed;
                        }
                        else if (trimmed.StartsWith("減速按鍵顯示="))
                            speedDownKeyDisplay = trimmed.Substring(20).Trim();
                        else if (trimmed.StartsWith("速度重置按鍵="))
                        {
                            string keyStr = trimmed.Substring(14).Trim();
                            Keys parsed;
                            if (Enum.TryParse<Keys>(keyStr, true, out parsed)) speedResetKey = parsed;
                        }
                        else if (trimmed.StartsWith("速度重置按鍵顯示="))
                            speedResetKeyDisplay = trimmed.Substring(21).Trim();
                        // ── Controller bindings ──────────────────────────────────
                        else if (trimmed.StartsWith("masterControl="))
                        {
                            GTA.Control parsed;
                            if (Enum.TryParse<GTA.Control>(trimmed.Substring(14).Trim(), true, out parsed)) masterControl = parsed;
                        }
                        else if (trimmed.StartsWith("masterControlDisplay="))
                            masterControlDisplay = trimmed.Substring(21).Trim();
                        else if (trimmed.StartsWith("masterControlModifier="))
                        {
                            GTA.Control parsed;
                            if (Enum.TryParse<GTA.Control>(trimmed.Substring(22).Trim(), true, out parsed)) masterControlModifier = parsed;
                        }
                        else if (trimmed.StartsWith("masterControlModifierDisplay="))
                            masterControlModifierDisplay = trimmed.Substring(29).Trim();
                        else if (trimmed.StartsWith("menuUpControl="))
                        {
                            GTA.Control parsed;
                            if (Enum.TryParse<GTA.Control>(trimmed.Substring(14).Trim(), true, out parsed)) menuUpControl = parsed;
                        }
                        else if (trimmed.StartsWith("menuUpControlDisplay="))
                            menuUpControlDisplay = trimmed.Substring(21).Trim();
                        else if (trimmed.StartsWith("menuDownControl="))
                        {
                            GTA.Control parsed;
                            if (Enum.TryParse<GTA.Control>(trimmed.Substring(16).Trim(), true, out parsed)) menuDownControl = parsed;
                        }
                        else if (trimmed.StartsWith("menuDownControlDisplay="))
                            menuDownControlDisplay = trimmed.Substring(23).Trim();
                        else if (trimmed.StartsWith("menuSelectControl="))
                        {
                            GTA.Control parsed;
                            if (Enum.TryParse<GTA.Control>(trimmed.Substring(18).Trim(), true, out parsed)) menuSelectControl = parsed;
                        }
                        else if (trimmed.StartsWith("選單確認控制顯示="))
                            menuSelectControlDisplay = trimmed.Substring(25).Trim();
                        else if (trimmed.StartsWith("menuBackControl="))
                        {
                            GTA.Control parsed;
                            if (Enum.TryParse<GTA.Control>(trimmed.Substring(16).Trim(), true, out parsed)) menuBackControl = parsed;
                        }
                        else if (trimmed.StartsWith("menuBackControlDisplay="))
                            menuBackControlDisplay = trimmed.Substring(23).Trim();
                        else if (trimmed.StartsWith("speedUpControl="))
                        {
                            GTA.Control parsed;
                            if (Enum.TryParse<GTA.Control>(trimmed.Substring(15).Trim(), true, out parsed)) speedUpControl = parsed;
                        }
                        else if (trimmed.StartsWith("speedUpControlDisplay="))
                            speedUpControlDisplay = trimmed.Substring(22).Trim();
                        else if (trimmed.StartsWith("speedDownControl="))
                        {
                            GTA.Control parsed;
                            if (Enum.TryParse<GTA.Control>(trimmed.Substring(17).Trim(), true, out parsed)) speedDownControl = parsed;
                        }
                        else if (trimmed.StartsWith("speedDownControlDisplay="))
                            speedDownControlDisplay = trimmed.Substring(24).Trim();
                        else if (trimmed.StartsWith("speedResetControl="))
                        {
                            GTA.Control parsed;
                            if (Enum.TryParse<GTA.Control>(trimmed.Substring(18).Trim(), true, out parsed)) speedResetControl = parsed;
                        }
                        else if (trimmed.StartsWith("speedResetControlDisplay="))
                            speedResetControlDisplay = trimmed.Substring(25).Trim();
                        else if (trimmed.StartsWith("deviceToken="))
                            deviceToken = trimmed.Substring(12).Trim();
                        else if (trimmed.StartsWith("strokeMin="))
                        { int sv; if (int.TryParse(trimmed.Substring(10).Trim(), out sv)) strokeMin = Math.Max(0, Math.Min(99, sv)); }
                        else if (trimmed.StartsWith("strokeMax="))
                        { int sv; if (int.TryParse(trimmed.Substring(10).Trim(), out sv)) strokeMax = Math.Max(1, Math.Min(100, sv)); }
                        else if (trimmed.StartsWith("readyAndFinishDelay="))
                        { int sv; if (int.TryParse(trimmed.Substring(20).Trim(), out sv)) readyAndFinishDelay = Math.Max(0, sv); }
                        else if (trimmed.StartsWith("carBjDeviceSpeed="))            { float v; if (float.TryParse(trimmed.Substring(17).Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v)) carBjDeviceSpeed            = Math.Max(0.0f, Math.Min(1.0f, v)); }
                        else if (trimmed.StartsWith("carSexDeviceSpeed="))           { float v; if (float.TryParse(trimmed.Substring(18).Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v)) carSexDeviceSpeed           = Math.Max(0.0f, Math.Min(1.0f, v)); }
                        else if (trimmed.StartsWith("carLowBjDeviceSpeed="))         { float v; if (float.TryParse(trimmed.Substring(20).Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v)) carLowBjDeviceSpeed         = Math.Max(0.0f, Math.Min(1.0f, v)); }
                        else if (trimmed.StartsWith("carLowSexDeviceSpeed="))        { float v; if (float.TryParse(trimmed.Substring(21).Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v)) carLowSexDeviceSpeed        = Math.Max(0.0f, Math.Min(1.0f, v)); }
                        else if (trimmed.StartsWith("standingBjDeviceSpeed="))       { float v; if (float.TryParse(trimmed.Substring(22).Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v)) standingBjDeviceSpeed       = Math.Max(0.0f, Math.Min(1.0f, v)); }
                        else if (trimmed.StartsWith("sittingBjDeviceSpeed="))        { float v; if (float.TryParse(trimmed.Substring(21).Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v)) sittingBjDeviceSpeed        = Math.Max(0.0f, Math.Min(1.0f, v)); }
                        else if (trimmed.StartsWith("layingBjDeviceSpeed="))         { float v; if (float.TryParse(trimmed.Substring(20).Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v)) layingBjDeviceSpeed         = Math.Max(0.0f, Math.Min(1.0f, v)); }
                        else if (trimmed.StartsWith("wallSexDeviceSpeed="))          { float v; if (float.TryParse(trimmed.Substring(19).Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v)) wallSexDeviceSpeed          = Math.Max(0.0f, Math.Min(1.0f, v)); }
                        else if (trimmed.StartsWith("sittingCowgirlDeviceSpeed="))   { float v; if (float.TryParse(trimmed.Substring(26).Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v)) sittingCowgirlDeviceSpeed   = Math.Max(0.0f, Math.Min(1.0f, v)); }
                        else if (trimmed.StartsWith("revCowgirlDeviceSpeed="))       { float v; if (float.TryParse(trimmed.Substring(22).Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v)) revCowgirlDeviceSpeed       = Math.Max(0.0f, Math.Min(1.0f, v)); }
                        else if (trimmed.StartsWith("layingCowgirlDeviceSpeed="))    { float v; if (float.TryParse(trimmed.Substring(25).Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v)) layingCowgirlDeviceSpeed    = Math.Max(0.0f, Math.Min(1.0f, v)); }
                        else if (trimmed.StartsWith("layingRevCowgirlDeviceSpeed=")) { float v; if (float.TryParse(trimmed.Substring(28).Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v)) layingRevCowgirlDeviceSpeed = Math.Max(0.0f, Math.Min(1.0f, v)); }
                        else if (trimmed.StartsWith("layingSinkraDeviceSpeed="))    { float v; if (float.TryParse(trimmed.Substring(24).Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v)) layingSinkraDeviceSpeed     = Math.Max(0.0f, Math.Min(1.0f, v)); }
                        else if (trimmed.StartsWith("doggyDeviceSpeed="))            { float v; if (float.TryParse(trimmed.Substring(17).Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v)) doggyDeviceSpeed            = Math.Max(0.0f, Math.Min(1.0f, v)); }
                        else if (trimmed.StartsWith("roughDoggyDeviceSpeed="))       { float v; if (float.TryParse(trimmed.Substring(22).Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v)) roughDoggyDeviceSpeed       = Math.Max(0.0f, Math.Min(1.0f, v)); }
                        else if (trimmed.StartsWith("deviceSpeedRamp="))             { float v; if (float.TryParse(trimmed.Substring(16).Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v)) deviceSpeedRamp             = Math.Max(1.01f, v); }
                        else if (trimmed.StartsWith("sinkraReplaceLaying="))
                            sinkraReplaceLaying      = trimmed.Substring(20).Trim().ToLower() == "true";
                        else if (trimmed.StartsWith("sinkraReplaceRevLaying="))
                            sinkraReplaceRevLaying   = trimmed.Substring(23).Trim().ToLower() == "true";
                        else if (trimmed.StartsWith("sinkraReplaceFacesitting="))
                            sinkraReplaceFacesitting = trimmed.Substring(25).Trim().ToLower() == "true";
                        else if (trimmed.StartsWith("sinkraLowerModels="))
                            _sinkraLowerModels = ParseVehicleList(trimmed.Substring(18));
                        else if (trimmed.StartsWith("sinkraLowerOffsetCowgirl="))
                            _sinkraLowerOffsetCowgirl = ParseVector3(trimmed.Substring(25));
                        else if (trimmed.StartsWith("sinkraLowerOffsetRevCowgirl="))
                            _sinkraLowerOffsetRevCowgirl = ParseVector3(trimmed.Substring(28));
                        else if (trimmed.StartsWith("sinkraLowerOffsetFacesit="))
                            _sinkraLowerOffsetFacesit = ParseVector3(trimmed.Substring(25));
                        else if (trimmed.StartsWith("wallBangLowerOffset="))
                            _wallBangLowerOffset = ParseVector3(trimmed.Substring(20));
                        else if (trimmed.StartsWith("overwriteLowVehicle="))
                            overwriteLowVehicle = ParseVehicleList(trimmed.Substring(20));
                        else if (trimmed.StartsWith("overwriteNormalVehicle="))
                            overwriteNormalVehicle = ParseVehicleList(trimmed.Substring(23));
                        else if (trimmed.StartsWith("backseatBlacklist="))
                            backseatBlacklist = ParseVehicleList(trimmed.Substring(18));
                    }
                }
            }
            catch { /* Defaults are fine */ }
        }

        private HashSet<int> ParseVehicleList(string csv)
        {
            var set = new HashSet<int>();
            foreach (string part in csv.Split(','))
            {
                string name = part.Trim().ToLower();
                if (name.Length > 0)
                {
                    try { set.Add(Function.Call<int>(Hash.GET_HASH_KEY, name)); }
                    catch { }
                }
            }
            return set;
        }

        private Vector3 ParseVector3(string csv)
        {
            var parts = csv.Split(',');
            float x = 0f, y = 0f, z = 0f;
            if (parts.Length >= 1) float.TryParse(parts[0].Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out x);
            if (parts.Length >= 2) float.TryParse(parts[1].Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out y);
            if (parts.Length >= 3) float.TryParse(parts[2].Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out z);
            return new Vector3(x, y, z);
        }

        /// <summary>傳回 true if the current girl model is listed in sinkraLowerModels.</summary>
        private bool IsSinkraLowerModel()
        {
            return girl != null && girl.Exists() && _sinkraLowerModels.Count > 0
                && _sinkraLowerModels.Contains(girl.Model.Hash);
        }

        private void SaveSettings()
        {
            try
            {
                // Read existing file to preserve comments and override lists
                var lines = new List<string>();
                if (File.Exists(SETTINGS_FILE))
                    lines = new List<string>(File.ReadAllLines(SETTINGS_FILE));

                // Track which keys we've updated
                var updated = new HashSet<string>();
                for (int i = 0; i < lines.Count; i++)
                {
                    string t = lines[i].Trim();
                    if (t.StartsWith("deviceToken="))
                    { lines[i] = "deviceToken=" + deviceToken; updated.Add("deviceToken"); }
                    else if (t.StartsWith("dildoEnabled="))
                    { lines[i] = "dildoEnabled=" + dildoEnabled.ToString().ToLower(); updated.Add("dildoEnabled"); }
                    else if (t.StartsWith("driveableCar="))
                    { lines[i] = "driveableCar=" + driveableCar.ToString().ToLower(); updated.Add("driveableCar"); }
                    else if (t.StartsWith("showTutorialMessages="))
                    { lines[i] = "showTutorialMessages=" + showTutorialMessages.ToString().ToLower(); updated.Add("showTutorialMessages"); }
                    else if (t.StartsWith("slapSoundsEnabled="))
                    { lines[i] = "slapSoundsEnabled=" + slapSoundsEnabled.ToString().ToLower(); updated.Add("slapSoundsEnabled"); }
                    else if (t.StartsWith("bjSoundsEnabled="))
                    { lines[i] = "bjSoundsEnabled=" + bjSoundsEnabled.ToString().ToLower(); updated.Add("bjSoundsEnabled"); }
                    else if (t.StartsWith("femaleServiceVoice="))
                    { lines[i] = "femaleServiceVoice=" + femaleServiceVoice.ToString().ToLower(); updated.Add("femaleServiceVoice"); }
                    else if (t.StartsWith("femaleMoanVoice="))
                    { lines[i] = "femaleMoanVoice=" + femaleMoanVoice.ToString().ToLower(); updated.Add("femaleMoanVoice"); }
                    else if (t.StartsWith("femaleEndVoice="))
                    { lines[i] = "femaleEndVoice=" + femaleEndVoice.ToString().ToLower(); updated.Add("femaleEndVoice"); }
                    else if (t.StartsWith("sandboxMode="))
                    { lines[i] = "sandboxMode=" + sandboxMode.ToString().ToLower(); updated.Add("sandboxMode"); }
                    else if (t.StartsWith("aLifeMode="))
                    { lines[i] = "aLifeMode=" + aLifeMode.ToString().ToLower(); updated.Add("aLifeMode"); }
                    else if (t.StartsWith("interiorLightEnabled="))
                    { lines[i] = "interiorLightEnabled=" + interiorLightEnabled.ToString().ToLower(); updated.Add("interiorLightEnabled"); }
                    else if (t.StartsWith("carEngineEnabled="))
                    { lines[i] = "carEngineEnabled=" + carEngineEnabled.ToString().ToLower(); updated.Add("carEngineEnabled"); }
                    else if (t.StartsWith("streetSitAnimEnabled="))
                    { lines[i] = "streetSitAnimEnabled=" + streetSitAnimEnabled.ToString().ToLower(); updated.Add("streetSitAnimEnabled"); }
                    else if (t.StartsWith("strokeMin="))
                    { lines[i] = "strokeMin=" + strokeMin; updated.Add("strokeMin"); }
                    else if (t.StartsWith("strokeMax="))
                    { lines[i] = "strokeMax=" + strokeMax; updated.Add("strokeMax"); }
                    else if (t.StartsWith("readyAndFinishDelay="))
                    { lines[i] = "readyAndFinishDelay=" + readyAndFinishDelay; updated.Add("readyAndFinishDelay"); }
                    else if (t.StartsWith("sinkraReplaceLaying="))
                    { lines[i] = "sinkraReplaceLaying=" + sinkraReplaceLaying.ToString().ToLower(); updated.Add("sinkraReplaceLaying"); }
                    else if (t.StartsWith("sinkraReplaceRevLaying="))
                    { lines[i] = "sinkraReplaceRevLaying=" + sinkraReplaceRevLaying.ToString().ToLower(); updated.Add("sinkraReplaceRevLaying"); }
                    else if (t.StartsWith("sinkraReplaceFacesitting="))
                    { lines[i] = "sinkraReplaceFacesitting=" + sinkraReplaceFacesitting.ToString().ToLower(); updated.Add("sinkraReplaceFacesitting"); }
                    // Leave all other lines (comments, overrides) untouched
                }

                // Append any missing keys (e.g. first run or hand-edited file)
                if (!updated.Contains("deviceToken")) lines.Add("deviceToken=" + deviceToken);
                if (!updated.Contains("dildoEnabled")) lines.Add("dildoEnabled=" + dildoEnabled.ToString().ToLower());
                if (!updated.Contains("driveableCar")) lines.Add("driveableCar=" + driveableCar.ToString().ToLower());
                if (!updated.Contains("showTutorialMessages")) lines.Add("showTutorialMessages=" + showTutorialMessages.ToString().ToLower());
                if (!updated.Contains("slapSoundsEnabled")) lines.Add("slapSoundsEnabled=" + slapSoundsEnabled.ToString().ToLower());
                if (!updated.Contains("bjSoundsEnabled")) lines.Add("bjSoundsEnabled=" + bjSoundsEnabled.ToString().ToLower());
                if (!updated.Contains("femaleServiceVoice")) lines.Add("femaleServiceVoice=" + femaleServiceVoice.ToString().ToLower());
                if (!updated.Contains("femaleMoanVoice")) lines.Add("femaleMoanVoice=" + femaleMoanVoice.ToString().ToLower());
                if (!updated.Contains("femaleEndVoice")) lines.Add("femaleEndVoice=" + femaleEndVoice.ToString().ToLower());
                if (!updated.Contains("sandboxMode")) lines.Add("sandboxMode=" + sandboxMode.ToString().ToLower());
                if (!updated.Contains("aLifeMode")) lines.Add("aLifeMode=" + aLifeMode.ToString().ToLower());
                if (!updated.Contains("interiorLightEnabled")) lines.Add("interiorLightEnabled=" + interiorLightEnabled.ToString().ToLower());
                if (!updated.Contains("carEngineEnabled")) lines.Add("carEngineEnabled=" + carEngineEnabled.ToString().ToLower());
                if (!updated.Contains("streetSitAnimEnabled")) lines.Add("streetSitAnimEnabled=" + streetSitAnimEnabled.ToString().ToLower());
                if (!updated.Contains("strokeMin")) lines.Add("strokeMin=" + strokeMin);
                if (!updated.Contains("strokeMax")) lines.Add("strokeMax=" + strokeMax);
                if (!updated.Contains("readyAndFinishDelay")) lines.Add("readyAndFinishDelay=" + readyAndFinishDelay);
                if (!updated.Contains("sinkraReplaceLaying"))      lines.Add("sinkraReplaceLaying="      + sinkraReplaceLaying.ToString().ToLower());
                if (!updated.Contains("sinkraReplaceRevLaying"))   lines.Add("sinkraReplaceRevLaying="   + sinkraReplaceRevLaying.ToString().ToLower());
                if (!updated.Contains("sinkraReplaceFacesitting")) lines.Add("sinkraReplaceFacesitting=" + sinkraReplaceFacesitting.ToString().ToLower());

                File.WriteAllLines(SETTINGS_FILE, lines.ToArray());
            }
            catch { }
        }

        // ═══════════════
        // DYNAMIC PRICING
        // ═══════════════

        /// <summary>Generate random prices (in $10 steps) for each group, and service availability, cached per ped.</summary>
        private void GeneratePedPrices(int pedHandle)
        {
            if (priceCache.ContainsKey(pedHandle)) return;

            // Determine greed factor from A-Life personality (0 = min price, 1 = max price)
            double greed = 0.50; // default mid-range
            if (aLifeMode && _currentGirlFp != null)
            {
                ALifePedData apdPrice;
                if (_aLifePeds.TryGetValue(_currentGirlKey, out apdPrice))
                {
                    PersonalityProfile profPrice = GetProfile(apdPrice.Personality);
                    if (profPrice != null) greed = profPrice.Greed;
                }
            }

            int[] prices = new int[5];
            for (int i = 0; i < 5; i++)
            {
                int lo  = PRICE_RANGES[i][0] / 10;
                int hi  = PRICE_RANGES[i][1] / 10;
                // Lerp the random range toward the upper bound by greed:
                // roll within [lo, lo + (hi-lo)*(0.5 + greed*0.5)] so greedy girls skew high.
                int rolledHi = lo + (int)Math.Round((hi - lo) * (0.50 + greed * 0.50));
                if (rolledHi > hi) rolledHi = hi;
                if (rolledHi < lo) rolledHi = lo;
                prices[i] = rng.Next(lo, rolledHi + 1) * 10;
            }
            priceCache[pedHandle] = prices;
            priceLocked[pedHandle] = new bool[] { false, false, false, false, false };

            // Roll availability per group (all services in a group share one roll)
            bool[] avail = new bool[5];
            for (int i = 0; i < 5; i++)
                avail[i] = rng.NextDouble() < GROUP_AVAIL_CHANCE[i];
            availabilityCache[pedHandle] = avail;

            // Sandbox Prostitution (no A-Life): roll one flag for the whole girl.
            // true = she'll do everything outdoors; false = needs car or interior.
            if (!_outdoorWillingCache.ContainsKey(pedHandle))
                _outdoorWillingCache[pedHandle] = rng.NextDouble() < 0.50;
        }

        /// <summary>True when the current menu level is one of the 3 street-service sub-menus.</summary>
        private bool IsInServiceSubMenu()
        {
            return menuLevel == MenuLevel.StandingServices
                || menuLevel == MenuLevel.SittingServices
                || menuLevel == MenuLevel.LayingServices;
        }

        /// <summary>Map a sub-menu index to the flat streetActionIdx (0–11).</summary>
        private int GetSubMenuFlatIdx(int subIdx)
        {
            switch (menuLevel)
            {
                case MenuLevel.StandingServices: return standingServiceMap[subIdx];
                case MenuLevel.SittingServices:  return sittingServiceMap[subIdx];
                case MenuLevel.LayingServices:   return layingServiceMap[subIdx];
                default: return subIdx;
            }
        }

        /// <summary>Return the label/map arrays for the current street-service sub-menu.</summary>
        private void GetSubMenuArrays(out string[] labels, out int[] map)
        {
            switch (menuLevel)
            {
                case MenuLevel.StandingServices: labels = standingServiceLabels; map = standingServiceMap; return;
                case MenuLevel.SittingServices:  labels = sittingServiceLabels;  map = sittingServiceMap;  return;
                case MenuLevel.LayingServices:   labels = layingServiceLabels;   map = layingServiceMap;   return;
                default: labels = standingServiceLabels; map = standingServiceMap; return;
            }
        }

        /// <summary>檢查是否 a service is available for the current girl (per group).</summary>
        private bool IsServiceAvailable(bool isCar, int idx)
        {
            // Car services are ALWAYS available
            if (isCar) return true;
            if (girl == null) return false;
            int handle = girl.Handle;
            GeneratePedPrices(handle);
            if (!availabilityCache.ContainsKey(handle)) return false;

            int gi = (int)GetServicePriceGroup(false, idx);

            // A-Life personality gates for all street-mode services
            if (aLifeMode && _currentGirlFp != null)
            {
                ALifePedData apdAvail;
                if (_aLifePeds.TryGetValue(_currentGirlKey, out apdAvail))
                {
                    PersonalityProfile profAvail = GetProfile(apdAvail.Personality);
                    if (profAvail != null)
                    {
                        // Riskiness < 0.40: she won't do anything in the open
                        // (bypassed when the player is inside a building, and never applies to kissing)
                        bool isIndoorsAvail = Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, Game.Player.Character) != 0;
                        if (!isIndoorsAvail && profAvail.Riskiness < 0.40 && gi != (int)PriceGroup.Kissing) return false;

                        // Attachment >= 0.65: kissing is emotionally off-limits unless:
                        //   a) reputation is close enough (>= 25), OR
                        //   b) Greed >= 0.55 (she'll bend the rule for money — handled at select time)
                        if (gi == (int)PriceGroup.Kissing
                            && profAvail.Attachment >= 0.65
                            && apdAvail.Reputation < 25
                            && profAvail.Greed < 0.55)
                            return false;
                    }
                }
            }

            // Prost A-Life hookers: availability is defined by HookerPrices, not the raw NPC cache.
            // If she advertises a service (has a price for that group), she offers it.
            // The kissing personality gate still applies — a high-attachment hooker won't kiss clients.
            if (!sandboxMode && aLifeMode && _currentGirlFp != null)
            {
                ALifePedData apdProst;
                if (_aLifePeds.TryGetValue(_currentGirlKey, out apdProst) && apdProst.HookerPrices != null)
                {
                    if (gi == (int)PriceGroup.Kissing)
                    {
                        PersonalityProfile profKiss = GetProfile(apdProst.Personality);
                        if (profKiss != null
                            && profKiss.Attachment >= 0.65
                            && apdProst.Reputation < 25
                            && profKiss.Greed < 0.55)
                            return false;
                    }
                    return gi < apdProst.HookerPrices.Length && apdProst.HookerPrices[gi] > 0;
                }
            }

            bool baseAvail = availabilityCache[handle][gi];

            // Sandbox Prostitution (no A-Life): single per-girl outdoor-willing flag.
            // Either she does ALL services outdoors, or she needs car/interior for
            // everything except kissing (which is fine outdoors if she offers it).
            if (!sandboxMode && !aLifeMode)
            {
                // Kissing: subject to the normal per-group roll, location doesn't matter
                if (gi == (int)PriceGroup.Kissing)
                    return baseAvail;

                // Non-kissing: outdoor willing → always available
                bool outdoorOk = _outdoorWillingCache.ContainsKey(handle) && _outdoorWillingCache[handle];
                if (outdoorOk) return true;

                // Not outdoor willing → only in car (already handled above) or interior
                bool isIndoorsProst = Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, Game.Player.Character) != 0;
                return isIndoorsProst;
            }

            return baseAvail;
        }

        /// <summary>
        /// Try to negotiate a lower price for the highlighted service's group.
        /// Each attempt: flat success chance per group. Fail = locked forever.
        /// If lucky, can negotiate all the way down to the floor.
        /// </summary>
        private void NegotiatePrice()
        {
            if (girl == null) return;
            bool isCar = (mode == Mode.Car);

            // Resolve flat index: sub-menus use a local index, so map it to the global service index
            int resolvedIdx = IsInServiceSubMenu() ? GetSubMenuFlatIdx(menuIndex) : menuIndex;

            // Can't negotiate unavailable services
            if (!IsServiceAvailable(isCar, resolvedIdx))
            {
                PlayPedSpeech(girl, "否");
                return;
            }

            int handle = girl.Handle;
            GeneratePedPrices(handle);
            PriceGroup grp = GetServicePriceGroup(isCar, resolvedIdx);
            int gi = (int)grp;
            int currentPrice = priceCache[handle][gi];
            int floor = PRICE_RANGES[gi][0];

            // Already locked
            if (priceLocked[handle][gi])
            {
                PlayPedSpeech(girl, "否");
                return;
            }

            // Already at floor → auto-lock
            if (currentPrice <= floor)
            {
                priceLocked[handle][gi] = true;
                PlayPedSpeech(girl, "否");
                return;
            }

            // Flat chance per group — reduced by girl's Greed trait and long-term goal
            double negotiateChance = NEGOTIATE_CHANCE[gi];
            if (aLifeMode && _currentGirlFp != null)
            {
                ALifePedData apdNeg;
                if (_aLifePeds.TryGetValue(_currentGirlKey, out apdNeg))
                {
                    PersonalityProfile profNeg = GetProfile(apdNeg.Personality);
                    if (profNeg != null)
                        negotiateChance = negotiateChance * (1.0 - profNeg.Greed * 0.60);
                    // Long-term goal overrides
                    if (apdNeg.LongTermGoal == "Get Rich")  negotiateChance *= 0.20; // almost never budges — money is the goal
                    else if (apdNeg.LongTermGoal == "Have Fun")  negotiateChance *= 1.30; // relaxed about money
                    else if (apdNeg.LongTermGoal == "Survive")   negotiateChance *= 0.70; // needs every dollar
                    else if (apdNeg.LongTermGoal == "Find Love")  negotiateChance *= 1.15; // not really about the cash
                }
            }
            if (rng.NextDouble() >= negotiateChance)
            {
                // Failed — lock it permanently
                priceLocked[handle][gi] = true;
                PlayPedSpeech(girl, "否");
                return;
            }

            // Success — drop by $10
            priceCache[handle][gi] = currentPrice - 10;
            int newPrice = priceCache[handle][gi];

            // If we just hit the floor, lock it too
            if (newPrice <= floor)
                priceLocked[handle][gi] = true;

            PlayPedSpeech(girl, "是");
        }

        /// <summary>Map a service menu item to its price group.</summary>
        private PriceGroup GetServicePriceGroup(bool isCar, int idx)
        {
            if (isCar)
                return idx == 0 ? PriceGroup.Blowjob : PriceGroup.Cowgirl;
            // Street: 0=Kissing, 1=Facesitting, 2-4=BJs, 5-9=Cowgirl/Sex, 10-11=Doggy
            switch (idx)
            {
                case 0:                                    return PriceGroup.Kissing;
                case 1:                                    return PriceGroup.Facesitting;
                case 2: case 3: case 4:                    return PriceGroup.Blowjob;
                case 5: case 6: case 7: case 8: case 9:    return PriceGroup.Cowgirl;
                case 10: case 11:                           return PriceGroup.Doggy;
                default:                                    return PriceGroup.Blowjob;
            }
        }

        /// <summary>Get the price for a specific service from the current girl's cached prices.</summary>
        private int GetServicePrice(bool isCar, int idx)
        {
            if (girl == null) return 0;
            int handle = girl.Handle;
            GeneratePedPrices(handle); // ensure cached
            return priceCache[handle][(int)GetServicePriceGroup(isCar, idx)];
        }

        /// <summary>
        /// Generate (or regenerate) per-girl prices for her exclusive service arrangement.
        /// Prices are influenced by Greed personality trait.
        /// Stored persistently in ALifePedData.HookerPrices (int[5]), one slot per PriceGroup.
        /// </summary>
        private void GenerateHookerPrices(ALifePedData d)
        {
            float greed = 0.5f;
            PersonalityProfile prof = GetProfile(d.Personality);
            if (prof != null) greed = (float)prof.Greed;

            int[] arr = new int[5];
            for (int i = 0; i < 5; i++)
            {
                int lo  = ALIFE_HOOKER_BASE[i][0];
                int hi  = ALIFE_HOOKER_BASE[i][1];
                double raw = lo + rng.NextDouble() * (hi - lo);
                double scale = 0.70 + greed * 0.80;
                int price = (int)(raw * scale);
                price = Math.Max(15, (int)(Math.Round(price / 5.0) * 5));
                arr[i] = price;
            }
            d.HookerPrices = arr;
            SaveALife();
        }

        /// <summary>
        /// Returns the effective price for a given service (with rep/relationship discounts).
        /// Uses GetServicePriceGroup for both car and street — Car BJ=Blowjob, Car Sex=Cowgirl.
        /// </summary>
        private int GetALifeHookerPrice(ALifePedData d, bool isCar, int idx)
        {
            if (d.HookerPrices == null) GenerateHookerPrices(d);

            int slot = (int)GetServicePriceGroup(isCar, idx);
            if (slot < 0 || slot >= 5) return 0;

            float price = d.HookerPrices[slot];

            float repDiscount = Math.Min(d.Reputation * 0.005f, 0.25f);
            float relDiscount = 0f;
            switch (d.Relationship)
            {
                case "Flirty":   relDiscount = 0.15f; break;
                case "友好": relDiscount = 0.10f; break;
            }

            price *= (1f - repDiscount - relDiscount);
            int final = (int)(Math.Round(price / 5.0) * 5);
            return Math.Max(5, final);
        }

        /// <summary>
        /// Returns the price for "Finish inside her" for an exclusive hooker.
        /// Based on greed, around $80-$200.
        /// </summary>
        private int GetFinishInsideHookerPrice(ALifePedData d)
        {
            if (d.HookerPrices == null) GenerateHookerPrices(d);

            float greed = 0.5f;
            PersonalityProfile prof = GetProfile(d.Personality);
            if (prof != null) greed = (float)prof.Greed;

            // Base $80–$200 scaled by greed
            double raw = 80 + rng.NextDouble() * 120;
            double scale = 0.70 + greed * 0.80;
            int price = (int)(raw * scale);
            price = Math.Max(20, (int)(Math.Round(price / 10.0) * 10));

            // Reputation discount
            float repDiscount = Math.Min(d.Reputation * 0.005f, 0.25f);
            price = Math.Max(20, (int)(price * (1f - repDiscount)));
            return price;
        }

        /// <summary>
        /// Negotiate an A-Life hooker price for the highlighted slot by $10-20.
        /// Returns true if the girl agreed. Affected by relationship and reputation.
        /// </summary>
        private bool NegotiateALifeHookerPrice(ALifePedData d, bool isCar, int idx)
        {
            if (d.HookerPrices == null) GenerateHookerPrices(d);

            int slot = (int)GetServicePriceGroup(isCar, idx);
            if (slot < 0 || slot >= 5) return false;

            // Base chance 30%, +5% per 10 rep, +10% if Friendly, +15% if Flirty
            double chance = 0.30;
            chance += d.Reputation * 0.005;
            if (d.Relationship == "Friend") chance += 0.10;
            else if (d.Relationship == "Flirty")  chance += 0.15;

            // Greed reduces chance
            PersonalityProfile prof = GetProfile(d.Personality);
            if (prof != null) chance *= (1.0 - prof.Greed * 0.50);
            // Long-term goal modifier
            if (d.LongTermGoal == "Get Rich")  chance *= 0.15; // money is the whole point — she won't drop
            else if (d.LongTermGoal == "Have Fun")  chance *= 1.25;
            else if (d.LongTermGoal == "Survive")   chance *= 0.65;
            else if (d.LongTermGoal == "Find Love")  chance *= 1.20;

            if (rng.NextDouble() >= chance) return false;

            // Drop by $10-$20, round to $5, floor $5
            int drop = (rng.Next(2) == 0) ? 10 : 20;
            d.HookerPrices[slot] = Math.Max(5, d.HookerPrices[slot] - drop);
            return true;
        }

        // ══════════════════════════════════════════════
        // PROSTITUTION A-LIFE HOOKER HELPERS
        // ══════════════════════════════════════════════

        /// <summary>Generate per-girl prices for a Prostitution A-Life hooker using PROST_ALIFE_BASE, greed-scaled.</summary>
        private void GenerateProstHookerPrices(ALifePedData d)
        {
            PersonalityProfile prof = GetProfile(d.Personality);
            double greed = (prof != null) ? prof.Greed : 0.50;
            d.HookerPrices = new int[5];
            for (int i = 0; i < 5; i++)
            {
                int lo = PROST_ALIFE_BASE[i][0];
                int hi = PROST_ALIFE_BASE[i][1];
                // Greedy girls roll toward the high end — same approach as sandbox GeneratePedPrices.
                // Base range stays fixed; only the random ceiling shifts with greed.
                int ceiling = lo + (int)Math.Round((hi - lo) * (0.40 + greed * 0.60));
                ceiling = Math.Max(lo + 5, Math.Min(hi, (ceiling / 5) * 5));
                int floor  = Math.Max(5, (lo / 5) * 5);
                d.HookerPrices[i] = floor + rng.Next((ceiling - floor) / 5 + 1) * 5;
            }
            SaveALife();
        }

        /// <summary>Effective price for a Prostitution A-Life hooker service, with rep/relationship discounts.</summary>
        private int GetProstHookerPrice(ALifePedData d, bool isCar, int idx)
        {
            if (d.HookerPrices == null) GenerateProstHookerPrices(d);
            int slot = (int)GetServicePriceGroup(isCar, idx);
            if (slot < 0 || slot >= 5) return 0;
            int price = d.HookerPrices[slot];
            // Hostile/Avoiding (rep <= -1): she multiplies her price x15 — she does not want you here
            if (d.Reputation <= -1) price = (price / 5) * 5 * 15;
            // Reputation discount: up to -20% at rep 50+
            else if (d.Reputation >= 50) price = (int)(price * 0.80);
            else if (d.Reputation >= 25) price = (int)(price * 0.90);
            // Relationship discount
            if      (d.Relationship == "Obsessed") price = (int)(price * 0.80);
            else if (d.Relationship == "Regular")  price = (int)(price * 0.92);
            price = (price / 5) * 5;
            return Math.Max(5, price);
        }

        /// <summary>Finish-inside price for a Prostitution A-Life hooker. Range $40–$120, greed-scaled.</summary>
        private int GetProstFinishInsidePrice(ALifePedData d)
        {
            PersonalityProfile prof = GetProfile(d.Personality);
            double greed = (prof != null) ? prof.Greed : 0.50;
            int price = (int)(40 + greed * 80);
            if (d.Relationship == "Regular")  price = (int)(price * 0.90);
            if (d.Relationship == "Obsessed") price = (int)(price * 0.80);
            price = (price / 5) * 5;
            // Hostile/Avoiding (rep <= -1): x15 — same as service rate
            if (d.Reputation <= -1) price = (price / 5) * 5 * 15;
            return Math.Max(10, price);
        }

        /// <summary>Negotiate a Prostitution A-Life hooker price down by $5–$15. 傳回 true if she agreed.</summary>
        private bool NegotiateProstHookerPrice(ALifePedData d, bool isCar, int idx)
        {
            if (d.HookerPrices == null) GenerateProstHookerPrices(d);
            int slot = (int)GetServicePriceGroup(isCar, idx);
            if (slot < 0 || slot >= 5) return false;
            // Minimum floor: $20 normally; $10 for Regular/Obsessed (trusting relationship)
            int minFloor = (d.Relationship == "Regular" || d.Relationship == "Obsessed") ? 10 : 20;
            // Already at or below the floor — she won't go lower
            if (d.HookerPrices[slot] <= minFloor) return false;
            // Hostile/Avoiding: she will not negotiate under any circumstances
            if (d.Reputation <= -1) return false;
            double chance = 0.40;
            if (d.Relationship == "Regular")  chance += 0.10;
            if (d.Relationship == "Obsessed") chance += 0.20;
            PersonalityProfile prof = GetProfile(d.Personality);
            if (prof != null) chance -= prof.Greed * 0.25; // greedy girls resist harder
            // Long-term goal modifier
            if (d.LongTermGoal == "Get Rich")  chance -= 0.35; // refuses — this is her livelihood
            else if (d.LongTermGoal == "GetOut")   chance -= 0.20; // doesn't care anymore but still needs money now
            else if (d.LongTermGoal == "Survive")   chance -= 0.15;
            else if (d.LongTermGoal == "Seek Protection") chance += 0.10; // wants the player happy
            if (rng.NextDouble() >= chance) return false;
            int drop = (rng.Next(3) + 1) * 5; // $5, $10, or $15
            int newPrice = d.HookerPrices[slot] - drop;
            // Sub-$20 barrier: Regular almost never breaks below $20 (5%); Obsessed almost always does (95%)
            if (newPrice < 20 && (d.Relationship == "Regular" || d.Relationship == "Obsessed"))
            {
                double breakChance = (d.Relationship == "Obsessed") ? 0.95 : 0.05;
                if (rng.NextDouble() >= breakChance) newPrice = 20;
            }
            d.HookerPrices[slot] = Math.Max(minFloor, newPrice);
            SaveALife();
            return true;
        }

        /// <summary>
        /// Open Services menu in A-Life hooker mode. Sets context flag and navigates.
        /// </summary>
        private void OpenALifeHookerServices()
        {
            _aLifeHookerServices = true;
            _telemetry.SendReady();                                       // warm up device
            if (mode == Mode.Car)
            {
                if (carPropActive && carPropPhase == 1)
                {
                    menuLevel = MenuLevel.Services;
                    menuIndex = 0;
                }
                else
                {
                    menuLevel = MenuLevel.None;
                    menuIndex = 0;
                    _carPropPendingMenu = MenuLevel.Services;
                    if (!carPropActive)
                        EnterCarProposition(Game.Player.Character);
                }
            }
            else
            {
                menuLevel = MenuLevel.Services;
                menuIndex = 0;
            }
        }

        // ═════════
        // KEY INPUT
        // ═════════
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (_usingController) return;  // keyboard ignored when gamepad is active
            if (menuLevel == MenuLevel.None) return;

            int itemCount = GetCurrentMenuItemCount();

            if (e.KeyCode == menuUpKey)
            {
                menuIndex = (menuIndex - 1 + itemCount) % itemCount;
                while (IsMenuItemLocked(menuIndex)) menuIndex = (menuIndex - 1 + itemCount) % itemCount;
                negotiateMode = false; // Reset swipe on row change
            }
            else if (e.KeyCode == menuDownKey)
            {
                menuIndex = (menuIndex + 1) % itemCount;
                while (IsMenuItemLocked(menuIndex)) menuIndex = (menuIndex + 1) % itemCount;
                negotiateMode = false; // Reset swipe on row change
            }
            else if (e.KeyCode == speedDownKey || e.KeyCode == speedUpKey)
            {
                // Swipe left/right toggles Accept ↔ Negotiate in Services (prostitution or A-Life hooker)
                if ((menuLevel == MenuLevel.Services || IsInServiceSubMenu()) && (!sandboxMode || _aLifeHookerServices))
                    negotiateMode = !negotiateMode;
                // Give Money denomination scroll (only on denomination rows, not CONFIRM)
                else if (menuLevel == MenuLevel.GiveMoney && menuIndex < 9)
                {
                    if (e.KeyCode == speedUpKey)
                        _giveMoneyAmounts[menuIndex] = Math.Min(_giveMoneyAmounts[menuIndex] + 1, 9);
                    else
                        _giveMoneyAmounts[menuIndex] = Math.Max(_giveMoneyAmounts[menuIndex] - 1, 0);
                }
            }
            else if (e.KeyCode == menuSelectKey)
            {
                HandleMenuSelect();
            }
            else if (e.KeyCode == menuBackKey)
            {
                // Go back one level or close
                if (menuLevel == MenuLevel.Services)
                {
                    // Exit car proposition pose when leaving services
                    if (mode == Mode.Car && carPropActive)
                    {
                        ExitCarProposition(Game.Player.Character);
                        negotiateMode = false;
                        // Determine which menu to reopen after exit anim finishes
                        if (_aLifeHookerServices)
                        {
                            _aLifeHookerServices = false;
                            _carPropExitReturnMenu = MenuLevel.Actions;
                            _carPropExitReturnIdx  = ShowInviteVehicleItem() ? 2 : 1;
                        }
                        else if (!aLifeMode)
                        {
                            _carPropExitReturnMenu = MenuLevel.SandboxCar;
                            _carPropExitReturnIdx  = 0;
                        }
                        else
                        {
                            _carPropExitReturnMenu = MenuLevel.Main;
                            _carPropExitReturnIdx  = 0;
                        }
                        menuLevel = MenuLevel.None;
                    }
                    else if (_aLifeHookerServices)
                    {
                        // Return to Actions menu (not Main)
                        _aLifeHookerServices = false;
                        negotiateMode = false;
                        menuLevel = MenuLevel.Actions;
                        menuIndex = ShowInviteVehicleItem() ? 2 : 1;
                    }
                    else if (!aLifeMode)
                    {
                        // Sandbox — Services always reached through SandboxCar
                        negotiateMode = false;
                        menuLevel = MenuLevel.SandboxCar;
                        menuIndex = 0;
                    }
                    else
                    {
                        menuLevel = MenuLevel.Main;
                        menuIndex = 0;
                        negotiateMode = false;
                    }
                }
                else if (menuLevel == MenuLevel.SandboxCar)
                {
                    menuLevel = MenuLevel.Main;
                    menuIndex = 0;
                }
                else if (menuLevel == MenuLevel.Settings)
                {
                    menuLevel = MenuLevel.Main;
                    menuIndex = hasGirl ? 5 : 3; // Restore cursor to "設定" item
                }
                else if (menuLevel == MenuLevel.Compatibility)
                {
                    menuLevel = MenuLevel.Main;
                    menuIndex = hasGirl ? 6 : 4; // Restore cursor to "Cowgirl Anim by Sinkra" item
                }
                else if (menuLevel == MenuLevel.Approach)
                {
                    _approachCandidate = null;
                    _approachSelectedCluster = -1;
                    _approachSecondChance = false;
                    menuLevel = MenuLevel.Main;
                    menuIndex = 0;
                }
                else if (menuLevel == MenuLevel.ApproachLine)
                {
                    // Back to cluster selection
                    menuLevel = MenuLevel.Approach;
                    menuIndex = _approachSelectedCluster >= 0 ? _approachSelectedCluster : 0;
                    _approachSelectedCluster = -1;
                }
                else if (menuLevel == MenuLevel.Actions)
                {
                    menuLevel = MenuLevel.Main;
                    menuIndex = 0; // Restore cursor to Actions item
                }
                else if (menuLevel == MenuLevel.InviteToVehicle)
                {
                    menuLevel = MenuLevel.Actions;
                    menuIndex = 0;
                }
                else if (menuLevel == MenuLevel.GiveMoney)
                {
                    menuLevel = MenuLevel.Actions;
                    // Restore cursor to Give Her Money item (last item in Actions)
                    menuIndex = GetCurrentMenuItemCount() - 1;
                }
                else if (menuLevel == MenuLevel.Conversation)
                {
                    menuLevel = MenuLevel.Actions;
                    menuIndex = ShowInviteVehicleItem() ? 1 : 0; // dynamic — invite item may or may not be present
                }
                else if (menuLevel == MenuLevel.ConvSub)
                {
                    menuLevel = MenuLevel.Conversation;
                    menuIndex = _convSelectedBranch >= 0 ? _convSelectedBranch : 0;
                    _convSelectedBranch = -1;
                }
                else if (menuLevel == MenuLevel.Intimacy)
                {
                    menuLevel = MenuLevel.Actions;
                    menuIndex = ShowInviteVehicleItem() ? 2 : 1; // dynamic — invite item may or may not be present
                }
                else if (menuLevel == MenuLevel.IntimacySub)
                {
                    menuLevel = MenuLevel.Intimacy;
                    menuIndex = _intimacySelectedBranch >= 0 ? _intimacySelectedBranch : 0;
                    _intimacySelectedBranch = -1;
                }
                else if (menuLevel == MenuLevel.Escalate)
                {
                    if (mode == Mode.Car && carPropActive)
                    {
                        ExitCarProposition(Game.Player.Character);
                        _carPropExitReturnMenu = MenuLevel.Intimacy;
                        _carPropExitReturnIdx  = 2;
                        menuLevel = MenuLevel.None;
                    }
                    else
                    {
                        menuLevel = MenuLevel.Intimacy;
                        menuIndex = 2; // Restore cursor to Escalate item
                    }
                }
                else if (menuLevel == MenuLevel.SitDownSub)
                {
                    // Back from seated sub-hub: stand up, return to parent at "坐下" item (idx 1)
                    negotiateMode = false;
                    if (_streetSitPropActive && _streetSitPropPhase <= 2 && state != ScriptState.Animating)
                    {
                        ExitStreetSitProposition(Game.Player.Character);
                        _streetSitExitReturnMenu = _serviceSubParent;
                        _streetSitExitReturnIdx  = 1;
                        menuLevel = MenuLevel.None;
                        keyCooldown = KEY_COOLDOWN_FRAMES;
                    }
                    else
                    {
                        menuLevel = _serviceSubParent;
                        menuIndex = 1;
                    }
                }
                else if (IsInServiceSubMenu())
                {
                    int catIdx = (menuLevel == MenuLevel.StandingServices) ? 0
                               : (menuLevel == MenuLevel.SittingServices)  ? 1 : 2;
                    negotiateMode = false;
                    if (catIdx == 0)
                    {
                        // Standing Services — no sit prop involved, just go back to parent
                        menuLevel = _serviceSubParent;
                        menuIndex = 0;
                    }
                    else
                    {
                        // Sitting/Laying — player is still seated; just return to SitDownSub hub, no stand-up
                        menuLevel = MenuLevel.SitDownSub;
                        menuIndex = (catIdx == 2) ? 1 : 0; // Sitting=0, Laying=1
                    }
                }
                else if (menuLevel == MenuLevel.Leave)
                {
                    menuLevel = MenuLevel.Main;
                    menuIndex = 1; // Restore cursor to Leave item
                }
                else if (menuLevel == MenuLevel.LeaveLine)
                {
                    menuLevel = MenuLevel.Leave;
                    menuIndex = _leaveSelectedCluster >= 0 ? _leaveSelectedCluster : 0;
                    _leaveSelectedCluster = -1;
                }
                else if (menuLevel == MenuLevel.Finish)
                {
                    // Back = keep going, don't finish — player must choose Pull Out or Finish Inside
                    menuLevel = MenuLevel.None;
                    keyCooldown = KEY_COOLDOWN_FRAMES;
                }
                else
                {
                    menuLevel = MenuLevel.None;
                    keyCooldown = KEY_COOLDOWN_FRAMES;
                }
            }
            else if (e.KeyCode == masterKey)
            {
                // Exit car proposition pose if active
                if (mode == Mode.Car && carPropActive)
                    ExitCarProposition(Game.Player.Character);
                if (_streetSitPropActive && _streetSitPropPhase <= 2 && state != ScriptState.Animating)
                    ExitStreetSitProposition(Game.Player.Character);
                // Master key always fully closes the menu — don't reopen anything after exit anim
                _carPropPendingMenu      = MenuLevel.None;
                _carPropExitReturnMenu   = MenuLevel.None;
                _streetSitExitReturnMenu = MenuLevel.None;
                _streetSitExitReturnIdx  = 0;
                menuLevel = MenuLevel.None;
                keyCooldown = KEY_COOLDOWN_FRAMES;
            }
        }

        // ═══════════════════════
        // CONTROLLER MENU INPUT
        // ═══════════════════════
        /// <summary>Mirrors OnKeyDown for gamepad. Called every frame from OnTick when a controller is active.</summary>
        private void HandleControllerMenuInput()
        {
            if (_menuControlCooldown > 0) return;
            if (menuLevel == MenuLevel.None) return;

            int itemCount = GetCurrentMenuItemCount();

            // Use IS_DISABLED_CONTROL_JUST_PRESSED: works even after DISABLE_ALL_CONTROL_ACTIONS
            if (Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 0, menuUpControl))
            {
                menuIndex = (menuIndex - 1 + itemCount) % itemCount;
                while (IsMenuItemLocked(menuIndex)) menuIndex = (menuIndex - 1 + itemCount) % itemCount;
                negotiateMode = false;
                _menuControlCooldown = KEY_COOLDOWN_FRAMES;
            }
            else if (Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 0, menuDownControl))
            {
                menuIndex = (menuIndex + 1) % itemCount;
                while (IsMenuItemLocked(menuIndex)) menuIndex = (menuIndex + 1) % itemCount;
                negotiateMode = false;
                _menuControlCooldown = KEY_COOLDOWN_FRAMES;
            }
            else if ((Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 0, speedDownControl) || Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 0, speedUpControl))
                     && !Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 0, masterControlModifier))
            {
                // Swipe left/right toggles Accept ↔ Negotiate in Services (prostitution or A-Life hooker)
                if ((menuLevel == MenuLevel.Services || IsInServiceSubMenu()) && (!sandboxMode || _aLifeHookerServices))
                    negotiateMode = !negotiateMode;
                // Give Money denomination scroll (only on denomination rows, not CONFIRM)
                else if (menuLevel == MenuLevel.GiveMoney && menuIndex < 9)
                {
                    if (Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 0, speedUpControl))
                        _giveMoneyAmounts[menuIndex] = Math.Min(_giveMoneyAmounts[menuIndex] + 1, 9);
                    else
                        _giveMoneyAmounts[menuIndex] = Math.Max(_giveMoneyAmounts[menuIndex] - 1, 0);
                }
                _menuControlCooldown = KEY_COOLDOWN_FRAMES;
            }
            else if (Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 0, menuSelectControl))
            {
                if (keyCooldown == 0)
                {
                    HandleMenuSelect();
                    keyCooldown = KEY_COOLDOWN_FRAMES;
                }
                _menuControlCooldown = KEY_COOLDOWN_FRAMES;
            }
            else if (Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 0, menuBackControl))
            {
                if (keyCooldown == 0)
                {
                    if (menuLevel == MenuLevel.Services)
                    {
                        if (mode == Mode.Car && carPropActive)
                        {
                            ExitCarProposition(Game.Player.Character);
                            negotiateMode = false;
                            if (_aLifeHookerServices)
                            {
                                _aLifeHookerServices = false;
                                _carPropExitReturnMenu = MenuLevel.Actions;
                                _carPropExitReturnIdx  = ShowInviteVehicleItem() ? 2 : 1;
                            }
                            else if (!aLifeMode)
                            {
                                _carPropExitReturnMenu = MenuLevel.SandboxCar;
                                _carPropExitReturnIdx  = 0;
                            }
                            else
                            {
                                _carPropExitReturnMenu = MenuLevel.Main;
                                _carPropExitReturnIdx  = 0;
                            }
                            menuLevel = MenuLevel.None;
                        }
                        else if (_aLifeHookerServices)
                        {
                            _aLifeHookerServices = false;
                            negotiateMode = false;
                            menuLevel = MenuLevel.Actions;
                            menuIndex = ShowInviteVehicleItem() ? 2 : 1;
                        }
                        else if (!aLifeMode)
                        {
                            // Sandbox — Services always reached through SandboxCar
                            negotiateMode = false;
                            menuLevel = MenuLevel.SandboxCar;
                            menuIndex = 0;
                        }
                        else
                        {
                            menuLevel = MenuLevel.Main;
                            menuIndex = 0;
                            negotiateMode = false;
                        }
                    }
                    else if (menuLevel == MenuLevel.SandboxCar)
                    {
                        menuLevel = MenuLevel.Main;
                        menuIndex = 0;
                    }
                    else if (menuLevel == MenuLevel.Settings)
                    {
                        menuLevel = MenuLevel.Main;
                        menuIndex = hasGirl ? 4 : 3; // Restore cursor to "設定" item
                    }
                    else if (menuLevel == MenuLevel.Compatibility)
                    {
                        menuLevel = MenuLevel.Main;
                        menuIndex = hasGirl ? 5 : 4; // Restore cursor to "Cowgirl Anim by Sinkra" item
                    }
                    else if (menuLevel == MenuLevel.Approach)
                    {
                        _approachCandidate = null;
                        _approachSelectedCluster = -1;
                        _approachSecondChance = false;
                        menuLevel = MenuLevel.Main;
                        menuIndex = 0;
                    }
                    else if (menuLevel == MenuLevel.ApproachLine)
                    {
                        // Back to cluster selection
                        menuLevel = MenuLevel.Approach;
                        menuIndex = _approachSelectedCluster >= 0 ? _approachSelectedCluster : 0;
                        _approachSelectedCluster = -1;
                    }
                    else if (menuLevel == MenuLevel.Actions)
                    {
                        menuLevel = MenuLevel.Main;
                        menuIndex = 0; // Restore cursor to Actions item
                    }
                    else if (menuLevel == MenuLevel.InviteToVehicle)
                    {
                        menuLevel = MenuLevel.Actions;
                        menuIndex = 0;
                    }
                    else if (menuLevel == MenuLevel.GiveMoney)
                    {
                        menuLevel = MenuLevel.Actions;
                        menuIndex = GetCurrentMenuItemCount() - 1;
                    }
                    else if (menuLevel == MenuLevel.Conversation)
                    {
                        menuLevel = MenuLevel.Actions;
                        menuIndex = ShowInviteVehicleItem() ? 1 : 0; // dynamic — invite item may or may not be present
                    }
                    else if (menuLevel == MenuLevel.ConvSub)
                    {
                        menuLevel = MenuLevel.Conversation;
                        menuIndex = _convSelectedBranch >= 0 ? _convSelectedBranch : 0;
                        _convSelectedBranch = -1;
                    }
                    else if (menuLevel == MenuLevel.Intimacy)
                    {
                        menuLevel = MenuLevel.Actions;
                        menuIndex = ShowInviteVehicleItem() ? 2 : 1; // dynamic — invite item may or may not be present
                    }
                    else if (menuLevel == MenuLevel.IntimacySub)
                    {
                        menuLevel = MenuLevel.Intimacy;
                        menuIndex = _intimacySelectedBranch >= 0 ? _intimacySelectedBranch : 0;
                        _intimacySelectedBranch = -1;
                    }
                    else if (menuLevel == MenuLevel.Escalate)
                    {
                        if (mode == Mode.Car && carPropActive)
                        {
                            ExitCarProposition(Game.Player.Character);
                            _carPropExitReturnMenu = MenuLevel.Intimacy;
                            _carPropExitReturnIdx  = 2;
                            menuLevel = MenuLevel.None;
                        }
                        else
                        {
                            menuLevel = MenuLevel.Intimacy;
                            menuIndex = 2;
                        }
                    }
                    else if (menuLevel == MenuLevel.SitDownSub)
                    {
                        // Back from seated sub-hub: stand up, return to parent at "坐下" item (idx 1)
                        negotiateMode = false;
                        if (_streetSitPropActive && _streetSitPropPhase <= 2 && state != ScriptState.Animating)
                        {
                            ExitStreetSitProposition(Game.Player.Character);
                            _streetSitExitReturnMenu = _serviceSubParent;
                            _streetSitExitReturnIdx  = 1;
                            menuLevel = MenuLevel.None;
                            keyCooldown = KEY_COOLDOWN_FRAMES;
                        }
                        else
                        {
                            menuLevel = _serviceSubParent;
                            menuIndex = 1;
                        }
                    }
                    else if (IsInServiceSubMenu())
                    {
                        int catIdx = (menuLevel == MenuLevel.StandingServices) ? 0
                                   : (menuLevel == MenuLevel.SittingServices)  ? 1 : 2;
                        negotiateMode = false;
                        if (catIdx == 0)
                        {
                            // Standing Services — no sit prop involved, just go back to parent
                            menuLevel = _serviceSubParent;
                            menuIndex = 0;
                        }
                        else
                        {
                            // Sitting/Laying — player is still seated; just return to SitDownSub hub, no stand-up
                            menuLevel = MenuLevel.SitDownSub;
                            menuIndex = (catIdx == 2) ? 1 : 0; // Sitting=0, Laying=1
                        }
                    }
                    else if (menuLevel == MenuLevel.Leave)
                    {
                        menuLevel = MenuLevel.Main;
                        menuIndex = 1; // Restore cursor to Leave item
                    }
                    else if (menuLevel == MenuLevel.LeaveLine)
                    {
                        menuLevel = MenuLevel.Leave;
                        menuIndex = _leaveSelectedCluster >= 0 ? _leaveSelectedCluster : 0;
                        _leaveSelectedCluster = -1;
                    }
                    else if (menuLevel == MenuLevel.Finish)
                    {
                        // Back = keep going, don't finish — player must choose Pull Out or Finish Inside
                        menuLevel = MenuLevel.None;
                        keyCooldown = KEY_COOLDOWN_FRAMES;
                    }
                    else
                    {
                        menuLevel = MenuLevel.None;
                        keyCooldown = KEY_COOLDOWN_FRAMES;
                    }
                }
                _menuControlCooldown = KEY_COOLDOWN_FRAMES;
            }
            else if (Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 0, masterControlModifier)
                     && Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 0, masterControl))
            {
                if (keyCooldown == 0)
                {
                    if (mode == Mode.Car && carPropActive)
                        ExitCarProposition(Game.Player.Character);
                    if (_streetSitPropActive && _streetSitPropPhase <= 2 && state != ScriptState.Animating)
                        ExitStreetSitProposition(Game.Player.Character);
                    // Master always fully closes — don't reopen anything after exit anim
                    _carPropPendingMenu      = MenuLevel.None;
                    _carPropExitReturnMenu   = MenuLevel.None;
                    _streetSitExitReturnMenu = MenuLevel.None;
                    _streetSitExitReturnIdx  = 0;
                    menuLevel = MenuLevel.None;
                    keyCooldown = KEY_COOLDOWN_FRAMES;
                }
                _menuControlCooldown = KEY_COOLDOWN_FRAMES;
            }
        }

        /// <summary>Get item count for current menu level.</summary>
        /// <summary>傳回 true if the item at <paramref name="idx"/> in the current menu level should be skipped during navigation.</summary>
        private bool IsMenuItemLocked(int idx)
        {
            ALifePedData dNav = null;
            if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out dNav);

            // Settings: Driveable Car (idx 10) is skipped when Car Engine is OFF
            if (menuLevel == MenuLevel.Settings && idx == 10 && !carEngineEnabled) return true;

            if (menuLevel == MenuLevel.Main)
            {
                if (hasGirl)
                {
                    // idx 3 = AI Mode: locked while a woman is recruited
                    if (idx == 3) return true;
                    // idx 4 = Game Mode: locked while a woman is recruited in A-Life mode
                    if (idx == 4 && aLifeMode) return true;
                }
                else
                {
                    // idx 1 = AI Mode: locked while a woman is recruited
                    if (idx == 1 && hasGirl) return true;
                    // idx 2 = Game Mode: locked while a woman is recruited in A-Life mode
                    if (idx == 2 && hasGirl && aLifeMode) return true;
                }
            }

            if (menuLevel == MenuLevel.ConvSub && _convSelectedBranch == 3)
            {
                // Prostitution A-Life Flirt branch has no locked items
                if (dNav != null && dNav.ALifeMode == "Prostitute") return false;
                // item 3: "I think about you more than I should." — needs a second meet
                if (idx == 3 && (dNav == null || !dNav.HasMetSecondTime)) return true;
                // item 5: "Did you miss me?" — needs a second meet
                if (idx == 5 && (dNav == null || !dNav.HasMetSecondTime)) return true;
            }

            return false;
        }

        private int GetCurrentMenuItemCount()
        {
            if (menuLevel == MenuLevel.Main)
            {
                // hasGirl: Actions/Services, Leave/Dismiss, Teleport, AI Mode, Game Mode, Settings>, Sinkra, Token, Bridge (9)
                // no girl: Search, AI Mode, Game Mode, Settings>, Sinkra, Token, Bridge (7)
                return hasGirl ? 9 : 7;
            }
            else if (menuLevel == MenuLevel.SandboxCar)
            {
                int count = 2; // Services [>], 給予/收回防衛武器
                if (FindBackseatVehicle() != null) count++;
                return count;
            }
            else if (menuLevel == MenuLevel.Actions)
            {
                int actCount = ShowInviteVehicleItem() ? 3 : 2; // [Invite to Vehicle], Conversation [>], Intimacy [>]
                // "Give Her Money" shown only in A-Life mode
                if (aLifeMode && _currentGirlKey != null) actCount++;
                // "Move to seat" for prostitution/hooker girls when both in same car
                if (ShowSeatSwapInActions()) actCount++;
                return actCount;
            }
            else if (menuLevel == MenuLevel.GiveMoney)
            {
                return 10; // 9 denomination rows + CONFIRM
            }
            else if (menuLevel == MenuLevel.InviteToVehicle)
            {
                return 3; // 3 invite dialogue lines
            }
            else if (menuLevel == MenuLevel.Conversation)
            {
                ALifePedData dConvCount = null;
                if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out dConvCount);
                // Prostitution A-Life: only 4 branches (Get to Know Her, Small Talk, Check Mood, Flirt)
                if (dConvCount != null && dConvCount.ALifeMode == "Prostitute") return 4;
                // Casual: Business item is hidden when she's Girlfriend or Obsessed
                bool hideBusinessCount = (dConvCount != null && (dConvCount.Relationship == "Girlfriend" || dConvCount.Relationship == "Obsessed"));
                return hideBusinessCount ? 6 : 7;
            }
            else if (menuLevel == MenuLevel.ConvSub)
            {
                return GetConvSubItemCount(_convSelectedBranch);
            }
            else if (menuLevel == MenuLevel.Intimacy)
            {
                return (_escalateConsented && FindBackseatVehicle() != null) ? 4 : 3; // Ask Preferences [>], Test Waters [>], Escalate [>], [Enter/swap seat]
            }
            else if (menuLevel == MenuLevel.IntimacySub)
            {
                switch (_intimacySelectedBranch)
                {
                    case 0: return 8; // Ask Preferences (8 questions)
                    case 1: return 3; // Test Waters (3 lines)
                    default: return 0;
                }
            }
            else if (menuLevel == MenuLevel.Escalate)
            {
                if (!_escalateConsented) return 3; // ask-for-sex dialogue options
                if (mode == Mode.Car) return IsBackseatCarMode() ? 1 : carServiceItems.Length;
                return 2; // Standing Services + Sit Down hub
            }
            else if (menuLevel == MenuLevel.Leave)
            {
                return 5; // 5 goodbye style clusters
            }
            else if (menuLevel == MenuLevel.LeaveLine)
            {
                // Casual Flattering (cluster 3) has 4 lines; everything else has 3
                ALifePedData dLeaveCount = null;
                if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out dLeaveCount);
                bool isCasualFlattering = _leaveSelectedCluster == 3
                    && (dLeaveCount == null || dLeaveCount.ALifeMode != "Prostitute");
                return isCasualFlattering ? 4 : 3;
            }
            else if (menuLevel == MenuLevel.Finish)
            {
                if (!aLifeMode) return 2; // Sandbox: Pull Out (0), Inside Her (1) — no Ask
                return _finishAsked ? 2 : 3; // A-Life: Ask to finish inside (hidden after use), Finish, Finish inside her
            }
            else if (menuLevel == MenuLevel.Approach)
            {
                if (_approachCandidate == null || !_approachCandidate.Exists()) return 1;
                string fp = GetPedFingerprint(_approachCandidate);
                ALifePedData d;
                if (fp != null && _aLifePeds.TryGetValue(ALifeKey(fp), out d))
                {
                    if (!sandboxMode && !aLifeMode) return 2; // prostitution non-A-Life: 2 direct lines
                }
                return 5; // 5 cluster choices (casual A-Life + prostitution A-Life)
            }
            else if (menuLevel == MenuLevel.ApproachLine)
            {
                if (_approachCandidate == null || !_approachCandidate.Exists()) return 1;
                string fp = GetPedFingerprint(_approachCandidate);
                ALifePedData d;
                bool firstEncounter = true;
                if (fp != null && _aLifePeds.TryGetValue(ALifeKey(fp), out d)) firstEncounter = !d.HasMet;
                return firstEncounter ? 4 : 3;
            }
            else if (menuLevel == MenuLevel.Services)
            {
                if (mode == Mode.Car) return IsBackseatCarMode() ? 1 : carServiceItems.Length;
                return 2; // Standing Services, Sit Down
            }
            else if (menuLevel == MenuLevel.SitDownSub)
            {
                return 2; // Sitting Services, Laying Services
            }
            else if (menuLevel == MenuLevel.StandingServices)
            {
                return standingServiceLabels.Length; // 5
            }
            else if (menuLevel == MenuLevel.SittingServices)
            {
                return sittingServiceLabels.Length; // 4
            }
            else if (menuLevel == MenuLevel.LayingServices)
            {
                return layingServiceLabels.Length; // 3
            }
            else if (menuLevel == MenuLevel.Compatibility)
            {
                // Replace Facesitting, Replace Laying Cowgirl, Replace Laying Rev Cowgirl (3)
                return 3;
            }
            else if (menuLevel == MenuLevel.Settings)
            {
                // Tutorial Msgs, Male Nudity, Svc Voice, Moan Voice, End Voice, BJ Sounds, Sex Sounds, Street Sit Anim, Light, Car Engine, Driveable (11)
                return 11;
            }
            return 0;
        }

        /// <summary>Handle menu item selection for current level.</summary>
        private void HandleMenuSelect()
        {
            if (menuLevel == MenuLevel.Main)
            {
                // Build menu item mapping based on hasGirl
                if (hasGirl)
                {
                    // A-Life:   0: Actions, 1: Leave,        2: Teleport, 3: AI Mode, 4: Game Mode, 5: Settings>, 6: Sinkra, 7: Token, 8: Device Bridge
                    // Sandbox:  0: Services, 1: Dismiss Her,  2: Teleport, 3: AI Mode, 4: Game Mode, 5: Settings>, 6: Sinkra, 7: Token, 8: Device Bridge
                    switch (menuIndex)
                    {
                        case 0:
                            if (aLifeMode) { menuLevel = MenuLevel.Actions; menuIndex = 0; }
                            else { menuLevel = MenuLevel.SandboxCar; menuIndex = 0; }
                            break;
                        case 1:
                            if (aLifeMode) { menuLevel = MenuLevel.Leave; menuIndex = 0; }
                            else { ShowHudStatus("~y~已解散伴侶！", 2000); FullReset(); menuLevel = MenuLevel.Main; menuIndex = 0; }
                            break;
                        case 2: // Teleport Woman/Hooker to Player
                            if (girl != null && girl.Exists())
                            {
                                Vector3 pp = Game.Player.Character.Position;
                                girl.Position = pp + Game.Player.Character.ForwardVector * 1.5f;
                                girl.Task.ClearAll();
                                ALifePedData _tpDat = null;
                                if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out _tpDat);
                                bool _tpIsHooker = !sandboxMode || _aLifeHookerServices || (_tpDat != null && _tpDat.IsHooker);
                                ShowHudStatus("~y~" + (_tpIsHooker ? "特殊服務" : "女性") + " 已傳送至玩家身邊！", 2000);
                            }
                            else
                            {
                                ALifePedData _tpDatMissing = null;
                                if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out _tpDatMissing);
                                bool _tpIsHookerMissing = !sandboxMode || _aLifeHookerServices || (_tpDatMissing != null && _tpDatMissing.IsHooker);
                                ShowHudStatus("~r~未找到 " + (_tpIsHookerMissing ? "特殊服務" : "女性") + "！", 2000);
                            }
                            break;
                        case 3: // AI Mode
                            if (!hasGirl) { aLifeMode = !aLifeMode; SaveSettings(); }
                            break;
                        case 4: // Game Mode
                            if (!(hasGirl && aLifeMode)) { sandboxMode = !sandboxMode; SaveSettings(); }
                            break;
                        case 5: // Settings >
                            menuLevel = MenuLevel.Settings;
                            menuIndex = 0;
                            break;
                        case 6: // Sinkra
                            if (_sinkraCowgirlAvailable)
                            { menuLevel = MenuLevel.Compatibility; menuIndex = 0; }
                            else
                                ShowHudStatus("~r~附加模組未安裝！", 2000);
                            break;
                        case 7: // Autoblow Token
                            Function.Call(Hash.DISPLAY_ONSCREEN_KEYBOARD, true, "FMMC_KEY_TIP8", "", deviceToken, "", "", "", 64);
                            tokenKeyboardOpen = true;
                            break;
                        case 8: // Device Bridge — 3-way: reconnect / disconnect / check
                            if (_telemetry.IsHardDisconnected)
                            {
                                _telemetry.Reconnect();
                                pingPendingTime = Game.GameTime;
                                ShowHudStatus("~y~正在連線至裝置……", 5500);
                            }
                            else if (_telemetry.IsDeviceOnline)
                            {
                                _telemetry.SendStop();
                                _telemetry.HardDisconnect();
                                ShowHudStatus("~r~裝置已中斷連線", 3000);
                            }
                            else
                            {
                                _telemetry.ResetPong();
                                _telemetry.SendPing();
                                pingPendingTime = Game.GameTime;
                                ShowHudStatus("~y~正在檢查裝置……", 5500);
                            }
                            break;
                    }
                }
                else
                {
                    // 0: Search, 1: AI Mode, 2: Hypnosis Mode, 3: Settings>, 4: Sinkra, 5: Token, 6: Device Bridge
                    switch (menuIndex)
                    {
                        case 0: // Search for Nearest Woman/Hooker
                            if (aLifeMode)
                                OpenApproachMenu(Game.Player.Character);
                            else
                                TryPickup(Game.Player.Character);
                            break;
                        case 1: // AI Mode
                            if (!hasGirl) { aLifeMode = !aLifeMode; SaveSettings(); }
                            break;
                        case 2: // Game Mode
                            if (!(hasGirl && aLifeMode)) { sandboxMode = !sandboxMode; SaveSettings(); }
                            break;
                        case 3: // Settings >
                            menuLevel = MenuLevel.Settings;
                            menuIndex = 0;
                            break;
                        case 4: // Sinkra — open Compatibility sub-menu (only if installed)
                            if (_sinkraCowgirlAvailable)
                            { menuLevel = MenuLevel.Compatibility; menuIndex = 0; }
                            else
                                ShowHudStatus("~r~附加模組未安裝！", 2000);
                            break;
                        case 5: // Autoblow Token — open GTA on-screen keyboard
                            Function.Call(Hash.DISPLAY_ONSCREEN_KEYBOARD, true, "FMMC_KEY_TIP8", "", deviceToken, "", "", "", 64);
                            tokenKeyboardOpen = true;
                            break;
                        case 6: // Device Bridge — 3-way: reconnect / disconnect / check
                            if (_telemetry.IsHardDisconnected)
                            {
                                _telemetry.Reconnect();
                                pingPendingTime = Game.GameTime;
                                ShowHudStatus("~y~正在連線至裝置……", 5500);
                            }
                            else if (_telemetry.IsDeviceOnline)
                            {
                                _telemetry.SendStop();
                                _telemetry.HardDisconnect();
                                ShowHudStatus("~r~裝置已中斷連線", 3000);
                            }
                            else
                            {
                                _telemetry.ResetPong();
                                _telemetry.SendPing();
                                pingPendingTime = Game.GameTime;
                                ShowHudStatus("~y~正在檢查裝置……", 5500);
                            }
                            break;
                    }
                }
            }
            else if (menuLevel == MenuLevel.SandboxCar)
            {
                switch (menuIndex)
                {
                    case 0: // Services [>]
                        OpenServicesMenu();
                        break;
                    case 1: // Give / Take Weapon
                        bool hasWep = girl != null && (girl.Weapons.HasWeapon(WeaponHash.MicroSMG) || girl.Weapons.HasWeapon(WeaponHash.CombatPistol));
                        if (hasWep)
                        {
                            girl.Weapons.RemoveAll();
                            ShowHudStatus("~y~已收回防衛武器！", 2000);
                        }
                        else if (girl != null)
                        {
                            girl.Weapons.Give(WeaponHash.MicroSMG, 9999, true, true);
                            ShowHudStatus("~g~已給予 Micro SMG 防衛武器！", 2000);
                        }
                        break;
                    case 2: // Get into the back/front seat
                    {
                        Ped plSandSeat = Game.Player.Character;
                        bool plInVSandSeat = plSandSeat.IsInVehicle();
                        bool gInVSandSeat  = girl != null && girl.Exists() && girl.IsInVehicle();
                        bool sameVehSandSeat = plInVSandSeat && gInVSandSeat && girl.CurrentVehicle == plSandSeat.CurrentVehicle;
                        if (plInVSandSeat && !sameVehSandSeat)
                            ShowHudStatus("~y~正在等待她……", 2000);
                        else
                            TrySwapSeats();
                        break;
                    }
                }
            }
            else if (menuLevel == MenuLevel.Approach)
            {
                bool candidateValid = _approachCandidate != null && _approachCandidate.Exists();
                string fp = candidateValid ? GetPedFingerprint(_approachCandidate) : null;
                ALifePedData d = null;
                bool known  = fp != null && _aLifePeds.TryGetValue(ALifeKey(fp), out d);

                if (!candidateValid)
                {
                    _approachCandidate = null;
                    menuLevel = MenuLevel.Main;
                    menuIndex = 0;
                }
                else if (!sandboxMode)
                {
                    // If she already hard-rejected the player as a civilian: skip the menu entirely
                    if (aLifeMode && candidateValid && d != null && !IsProstHookerModel(_approachCandidate) && d.CivilianRejected)
                    {
                        d.Reputation = Math.Max(REP_MIN, d.Reputation - 1);
                        UpdateProstRelationshipState(d);
                        SaveALife();
                        Ped civPed = _approachCandidate;
                        _approachCandidate = null;
                        _approachSecondChance = false;
                        menuLevel = MenuLevel.Main;
                        menuIndex = 0;
                        TriggerHostileReaction(civPed, d);
                        return;
                    }
                    // Otherwise: everyone (hookers and civilians) picks a cluster, then a line
                    _approachSelectedCluster = menuIndex;
                    menuLevel = MenuLevel.ApproachLine;
                    menuIndex = 0;
                }
                else // Casual A-Life: player picks a cluster, then drills into line selection
                {
                    _approachSelectedCluster = menuIndex;
                    menuLevel = MenuLevel.ApproachLine;
                    menuIndex = 0;
                }
            }
            else if (menuLevel == MenuLevel.ApproachLine)
            {
                bool candidateValid = _approachCandidate != null && _approachCandidate.Exists();
                string fp = candidateValid ? GetPedFingerprint(_approachCandidate) : null;
                ALifePedData d = null;
                bool known = fp != null && _aLifePeds.TryGetValue(ALifeKey(fp), out d);

                if (!candidateValid)
                {
                    _approachCandidate = null;
                    _approachSelectedCluster = -1;
                    menuLevel = MenuLevel.Main;
                    menuIndex = 0;
                }
                else
                {
                    bool isFirstEncounter = d == null || !d.HasMet;
                    // Map picked cluster index + line index back to overall index for GetClusterFromIndex
                    int lineSize = isFirstEncounter ? 4 : 3;
                    int overallIndex = _approachSelectedCluster * lineSize + menuIndex;
                    string pickedCluster   = GetClusterFromIndex(overallIndex, isFirstEncounter);
                    string expectedCluster = (d != null) ? GetApproachCluster(d.Personality) : null;
                    double clusterBonus = 0.0;
                    if (pickedCluster != null && expectedCluster != null)
                        clusterBonus = (pickedCluster == expectedCluster) ? 0.25 : -0.20;

                    // Prost A-Life civilian: rejection roll happens here, after the player's line is chosen
                    if (!sandboxMode && aLifeMode && _approachCandidate != null && d != null && !IsProstHookerModel(_approachCandidate))
                    {
                        if (rng.NextDouble() < GetCivilianRejectionChance(d))
                        {
                            d.CivilianRejected = true;
                            d.Reputation = Math.Max(REP_MIN, d.Reputation - 1);
                            UpdateProstRelationshipState(d);
                            SaveALife();
                            ShowCivilianNotHookerRejectLine(d, _approachSelectedCluster);
                            _approachCandidate = null;
                            _approachSelectedCluster = -1;
                            _approachSecondChance = false;
                            menuLevel = MenuLevel.Main;
                            menuIndex = 0;
                            return;
                        }
                    }

                    bool accepted = RollApproachAcceptance(d, clusterBonus);
                    if (accepted)
                    {
                        if (sandboxMode)
                            ShowApproachLineAcceptResponse(d, _approachSelectedCluster, menuIndex, isFirstEncounter);
                        else
                            _lastApproachLineIdx = menuIndex; // Prost: ShowApproachAcceptSubtitle inside TryPickup consumes this
                        if (d != null && fp != null)
                        {
                            if (d.HasMet) d.HasMetSecondTime = true;
                            d.HasMet = true;
                            if (!sandboxMode)
                            {
                                // Prost A-Life: approach style vs personality match determines rep gain.
                                // No rep gain while she is already avoiding/hostile — trust must be rebuilt elsewhere.
                                if (d.Reputation >= 0)
                                {
                                    bool clusterMatch = pickedCluster != null && expectedCluster != null && pickedCluster == expectedCluster;
                                    if (clusterMatch)
                                        d.Reputation = Math.Min(REP_MAX, Math.Max(d.Reputation, 10)); // jump to Acquaintance floor instantly
                                    else
                                        d.Reputation = Math.Min(REP_MAX, d.Reputation + 1);           // mismatch: minimal gain
                                }
                                // Prost A-Life rep -1: after she's heard the line, 50/50 — go hostile or gouge price
                                else if (d.Reputation == -1)
                                {
                                    if (rng.NextDouble() < 0.50)
                                    {
                                        // Hostile path: snap to -2, trigger reaction, abort session
                                        d.Reputation = REP_MIN;
                                        UpdateProstRelationshipState(d);
                                        SaveALife();
                                        Ped snap = _approachCandidate;
                                        _approachCandidate = null;
                                        menuLevel = MenuLevel.None;
                                        ALifePedData snapData;
                                        if (snap != null && _aLifePeds.TryGetValue(ALifeKey(fp), out snapData))
                                            TriggerHostileReaction(snap, snapData);
                                        return;
                                    }
                                    // else price-gouge path: fall through to TryPickup — x15 prices + 0% negotiate apply
                                }
                            }
                            else
                            {
                                d.Reputation = Math.Min(REP_MAX, d.Reputation + 2);
                            }
                            UpdateRelationshipState(d);
                            SaveALife();
                        }
                        Ped candidate = _approachCandidate;
                        _approachCandidate = null;
                        menuLevel = MenuLevel.Main;
                        menuIndex = 0;
                        TryPickup(Game.Player.Character, candidate);
                        // _approachSelectedCluster cleared inside ShowApproachAcceptSubtitle (Prost)
                        // or left for FullReset — do NOT clear here, Phase 0 arrival reads it seconds later
                    }
                    else
                    {
                        ShowApproachLineRejectResponse(d, _approachSelectedCluster, menuIndex, isFirstEncounter);
                        if (!_approachSecondChance && d != null && d.Reputation >= 0)
                        {
                            // First rejection: grant a second chance, but still penalise (-1 rep)
                            _approachSecondChance = true;
                            d.Reputation = Math.Max(-1, d.Reputation - 1);
                            UpdateRelationshipState(d);
                            if (fp != null) SaveALife();
                            _approachSelectedCluster = -1;
                            menuLevel = MenuLevel.Approach;
                            menuIndex = 0;
                        }
                        else
                        {
                            // Second rejection or already negative: penalise
                            if (d != null && fp != null)
                            {
                                d.Reputation = Math.Max(REP_MIN, d.Reputation - 1);
                                UpdateRelationshipState(d);
                                SaveALife();
                                // Pushed to REP_MIN: trigger fight or flee immediately
                                if (d.Reputation <= -2 && _approachCandidate != null && _approachCandidate.Exists())
                                    TriggerHostileReaction(_approachCandidate, d);
                                else if (_approachCandidate != null && _approachCandidate.Exists())
                                {
                                    if (d.Reputation == -1) ShowAvoidingReactionSubtitle(d);
                                    _approachCandidate.Task.WanderAround();
                                }
                            }
                            else if (_approachCandidate != null && _approachCandidate.Exists())
                                _approachCandidate.Task.WanderAround();
                            _approachCandidate = null;
                            _approachSelectedCluster = -1;
                            _approachSecondChance = false;
                            menuLevel = MenuLevel.Main;
                            menuIndex = 0;
                        }
                    }
                }
            }
            else if (menuLevel == MenuLevel.Compatibility)
            {
                if (_sinkraCowgirlAvailable) // Safety: ignore toggles if addon not installed
                {
                    switch (menuIndex)
                    {
                        case 0: sinkraReplaceFacesitting = !sinkraReplaceFacesitting; SaveSettings(); break;
                        case 1: sinkraReplaceLaying      = !sinkraReplaceLaying;      SaveSettings(); break;
                        case 2: sinkraReplaceRevLaying   = !sinkraReplaceRevLaying;   SaveSettings(); break;
                    }
                }
            }
            else if (menuLevel == MenuLevel.Finish)
            {
                // ── Sandbox (non-A-Life): simple 2-item menu, no dialogue, no rep ──
                if (!aLifeMode)
                {
                    if (menuIndex == 0) // Pull Out
                    {
                        _finishInside = false;
                        _finishForced = false;
                    }
                    else // Inside Her
                    {
                        _finishInside = true;
                        _finishForced = false;
                    }
                    _finishAsked = false;
                    _aLifeFinishInsidePrice = 0;
                    _finishInsideFreePass = false;
                    _finishExecutePending = true;
                    _finishExecuteAt = 0;
                    menuLevel = MenuLevel.None;
                    return;
                }

                // ── A-Life: full Ask / consent / price system ──
                ALifePedData fData = null;
                if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out fData);

                // Once Ask has been used the menu collapses to 2 items: Finish (0) and Finish inside her (1)
                int finishIdx      = _finishAsked ? 0 : 1;
                int finishInsideIdx = _finishAsked ? 1 : 2;

                if (!_finishAsked && menuIndex == 0) // "要求內射" — gets her answer, no finish yet
                {
                    _finishAsked = true;
                    // CimPreference NPCs always accept — bypass the roll entirely
                    bool accepted = (fData != null && fData.CimPreference) || RollFinishInsideAcceptance(fData);
                    if (accepted)
                    {
                        // CimPreference NPCs always accept for free — skip all price logic entirely
                        if (fData != null && fData.CimPreference)
                        {
                            _finishInsideFreePass = true;
                            ShowFinishInsideAcceptSubtitle(fData);
                        }
                        // Exclusive hooker: roll free-pass first; if she waives the fee use standard dialogue, otherwise quote price
                        else if (sandboxMode && aLifeMode && fData != null && fData.IsHooker)
                        {
                            if (RollHookerFreePass(fData))
                            {
                                _finishInsideFreePass = true;
                                ShowFinishInsideAcceptSubtitle(fData);
                            }
                            else
                            {
                                _aLifeFinishInsidePrice = GetFinishInsideHookerPrice(fData);
                                string fnHookName = DisplayName(fData);
                                string fnHookP    = (fData.Personality != null) ? fData.Personality : "";
                                string[] priceLines = GetFinishInsidePriceLines(fnHookName, fnHookP, _aLifeFinishInsidePrice);
                                ShowSubtitle(priceLines[rng.Next(priceLines.Length)], 3500);
                            }
                        }
                        else if (!sandboxMode && aLifeMode && fData != null)
                        {
                            // Prost A-Life: roll free-pass first; if she waives the fee use standard dialogue, otherwise quote price
                            if (RollProstFreePass(fData))
                            {
                                _finishInsideFreePass = true;
                                ShowFinishInsideAcceptSubtitle(fData);
                            }
                            else
                            {
                                _aLifeFinishInsidePrice = GetProstFinishInsidePrice(fData);
                                string fnProstName = DisplayName(fData);
                                string fnProstP    = (fData.Personality != null) ? fData.Personality : "";
                                string[] prostPriceLines = GetFinishInsidePriceLines(fnProstName, fnProstP, _aLifeFinishInsidePrice);
                                ShowSubtitle(prostPriceLines[rng.Next(prostPriceLines.Length)], 3500);
                            }
                        }
                        else
                        {
                            ShowFinishInsideAcceptSubtitle(fData);
                        }
                        _finishInside = true;
                        _finishForced = false;
                    }
                    else
                    {
                        ShowFinishInsideDeclineSubtitle(fData);
                        _finishInside = false;
                        _finishForced = false;
                    }
                    // Keep menu open so player can pick Finish or Finish inside her
                }
                else if (menuIndex == finishIdx) // "Finish" — pull out, always silent, no consequence
                {
                    _finishInside = false;
                    _finishForced = false;
                    _finishAsked  = false;
                    _aLifeFinishInsidePrice = 0;    // clear any quoted price that was never charged
                    _finishInsideFreePass   = false; // clear free-pass flag
                    _finishExecutePending = true;
                    _finishExecuteAt = 0;
                    menuLevel = MenuLevel.None;
                }
                else if (menuIndex == finishInsideIdx) // "Finish inside her" — good if she agreed, bad if she didn't
                {
                    bool isALifeHooker = sandboxMode && aLifeMode && fData != null && fData.IsHooker;
                    string fnName = DisplayName(fData);

                    // Termination chance scaled by relationship: Obsessed 1% → Stranger 99%
                    double termChance = 0.99;
                    if (fData != null)
                    {
                        switch (fData.Relationship)
                        {
                            case "Obsessed":     termChance = 0.01; break;
                            case "女朋友":   termChance = 0.01; break;
                            case "Regular":      termChance = 0.50; break;
                            case "Flirty":       termChance = 0.80; break;
                            case "友好":     termChance = 0.85; break;
                            case "Familiar":     termChance = 0.90; break;
                            case "Acquaintance": termChance = 0.95; break;
                            case "Friendzoned":  termChance = 0.95; break;
                            default:             termChance = 0.99; break;
                        }
                    }

                    if (_finishInside) // she said yes via Ask
                    {
                        if (isALifeHooker)
                        {
                            int finCost = _aLifeFinishInsidePrice;
                            if (finCost > 0 && Game.Player.Money < finCost)
                            {
                                // Can't afford what she quoted
                                _finishBroke = true; // suppress swallow and set Annoyed mood
                                if (rng.NextDouble() < termChance)
                                {
                                    fData.IsHooker = false;
                                    fData.HookerPrices = null;
                                    fData.Reputation = Math.Max(REP_MIN, fData.Reputation - 1);
                                    UpdateRelationshipState(fData);
                                    SaveALife();
                                    CheckAndReleaseIfAvoiding(fData);
                                    string[] brokeLines = GetBrokeHookerTerminateLines(fnName, fData != null ? fData.Personality ?? "" : "");
                                    ShowSubtitle(brokeLines[rng.Next(brokeLines.Length)], 4500);
                                }
                                else
                                {
                                    string[] forgives = GetBrokeForgiveLines(fnName, fData != null ? fData.Personality ?? "" : "");
                                    ShowSubtitle(forgives[rng.Next(forgives.Length)], 4000);
                                }
                            }
                            else if (finCost > 0)
                            {
                                // Has the money — charge her quoted price
                                Game.Player.Money -= finCost;
                                lastPaidAmount += finCost;
                                if (fData != null) { fData.Cash += finCost; SaveALife(); }
                            }
                            _aLifeFinishInsidePrice = 0;
                            _finishInsideFreePass   = false;
                        }
                        else if (!sandboxMode)
                        {
                            if (!_finishInsideFreePass)
                            {
                                int finCost = _aLifeFinishInsidePrice > 0 ? _aLifeFinishInsidePrice : 50;
                                if (Game.Player.Money < finCost)
                                {
                                    // Can't pay — reaction depends on relationship (reuses termChance: Flirty=0.80 … Stranger=0.99)
                                    _finishBroke = true;
                                    if (rng.NextDouble() < termChance)
                                    {
                                        // She's pissed — rep penalty
                                        if (fData != null)
                                        {
                                            fData.Reputation = Math.Max(REP_MIN, fData.Reputation - 2);
                                            UpdateRelationshipState(fData);
                                            SaveALife();
                                            CheckAndReleaseIfAvoiding(fData);
                                        }
                                        string[] brokeProstLines = GetBrokeProstAngryLines(fnName, fData != null ? fData.Personality ?? "" : "");
                                        ShowSubtitle(brokeProstLines[rng.Next(brokeProstLines.Length)], 4000);
                                    }
                                    else
                                    {
                                        // She lets it slide this time
                                        string[] brokeProstForgiveLines = GetBrokeForgiveLines(fnName, fData != null ? fData.Personality ?? "" : "");
                                        ShowSubtitle(brokeProstForgiveLines[rng.Next(brokeProstForgiveLines.Length)], 4000);
                                    }
                                }
                                else
                                {
                                    Game.Player.Money -= finCost;
                                    lastPaidAmount += finCost;
                                    if (fData != null) { fData.Cash += finCost; SaveALife(); }
                                }
                            }
                            _aLifeFinishInsidePrice = 0;
                            _finishInsideFreePass   = false;
                        }
                        _finishForced = false; // consensual
                    }
                    else // she declined or player never asked
                    {
                        // CimPreference NPCs enjoy this even without being asked — positive reaction, no penalty
                        if (fData != null && fData.CimPreference)
                        {
                            ShowCimPreferenceUnaskedSubtitle(fData);
                            _finishInside = true;
                            _finishForced = false; // not a trust break — she's into it
                            _aLifeFinishInsidePrice = 0;
                            _finishExecutePending = true;
                            _finishExecuteAt = 0;
                            menuLevel = MenuLevel.None;
                            return;
                        }

                        ShowFinishForcedSubtitle(fData);
                        PersonalityProfile profForced = (fData != null) ? GetProfile(fData.Personality) : null;
                        double friendlinessForced = (profForced != null) ? profForced.Friendliness : 0.50;
                        int penalty = -(int)(2.0 + (1.0 - friendlinessForced) * 4.0); // -2 to -6
                        if (fData != null)
                        {
                            fData.Reputation = Math.Max(REP_MIN, fData.Reputation + penalty);
                            UpdateRelationshipState(fData);
                            SaveALife();
                            CheckAndReleaseIfAvoiding(fData);
                        }

                        // Hostile-chance: chance she goes -2 and leaves, scaled by how well she knows the player.
                        // IsHooker uses termChance already computed above; casual uses its own table.
                        string relForced = (fData != null) ? fData.Relationship : "Stranger";
                        double hostileChanceForced;
                        if (isALifeHooker)
                        {
                            hostileChanceForced = termChance; // Flirty=0.80 … Acquaintance=0.95 … Stranger=0.99
                        }
                        else
                        {
                            switch (relForced)
                            {
                                case "Obsessed":                                        hostileChanceForced = 0.01; break;
                                case "Flirty": case "女朋友": case "Friendzoned":  hostileChanceForced = 0.10; break;
                                case "友好":                                        hostileChanceForced = 0.35; break;
                                case "Familiar":                                        hostileChanceForced = 0.55; break;
                                case "Acquaintance":                                    hostileChanceForced = 0.75; break;
                                default:                                                hostileChanceForced = 0.99; break; // Stranger
                            }
                        }

                        if (fData != null && rng.NextDouble() < hostileChanceForced)
                        {
                            fData.Reputation = REP_MIN;
                            UpdateRelationshipState(fData);
                            fData.Mood = "Annoyed";
                            if (isALifeHooker)
                            {
                                fData.IsHooker = false;
                                fData.HookerPrices = null;
                                string[] forcedLines = GetForcedFinishHookerLines(fnName, fData != null ? fData.Personality ?? "" : "");
                                ShowSubtitle(forcedLines[rng.Next(forcedLines.Length)], 4500);
                            }
                            SaveALife();
                            if (girl != null && girl.Exists())
                                ApplyMoodFacialExpression(girl, "Relaxed");
                            CheckAndReleaseIfAvoiding(fData);
                        }

                        _finishInside = true;
                        _finishForced = true;
                        _aLifeFinishInsidePrice = 0;
                    }
                    _finishExecutePending = true;
                    _finishExecuteAt = 0;
                    menuLevel = MenuLevel.None;
                }
            }
            else if (menuLevel == MenuLevel.Actions)
            {
                bool showInvite = ShowInviteVehicleItem();
                int convIdx  = showInvite ? 1 : 0;
                int intimIdx = showInvite ? 2 : 1;
                if (showInvite && menuIndex == 0) // Invite to Vehicle [>]
                {
                    menuLevel = MenuLevel.InviteToVehicle;
                    menuIndex = 0;
                }
                else if (menuIndex == convIdx) // Conversation [>]
                {
                    menuLevel = MenuLevel.Conversation;
                    menuIndex = 0;
                }
                else if (aLifeMode && _currentGirlKey != null && menuIndex == intimIdx + 1) // Give Her Money [>]
                {
                    // Open the GiveMoney submenu — reset amounts and row cursor
                    for (int gi = 0; gi < 9; gi++) _giveMoneyAmounts[gi] = 0;
                    _giveMoneyRow = 0;
                    menuLevel = MenuLevel.GiveMoney;
                    menuIndex = 0;
                }
                else if (ShowSeatSwapInActions() && menuIndex == intimIdx + 2) // Move to back/front seat
                {
                    Ped plSeatChk   = Game.Player.Character;
                    bool plInVSeat  = plSeatChk.IsInVehicle();
                    bool gInVSeat   = girl != null && girl.Exists() && girl.IsInVehicle();
                    bool sameVehSeat = plInVSeat && gInVSeat && girl.CurrentVehicle == plSeatChk.CurrentVehicle;
                    // Block if the player is in a vehicle but she hasn't entered it yet.
                    // Exception: both on foot → "Enter Vehicle from Back Seat" path is fine.
                    if (plInVSeat && !sameVehSeat)
                        ShowHudStatus("~y~正在等待她……", 2000);
                    else
                        TrySwapSeats();
                }
                else if (menuIndex == intimIdx) // Intimacy [>] or Services [>]
                {
                    // Route A-Life hooker directly to Services menu (her personal prices).
                    // Covers both Casual A-Life (IsHooker easter egg) and Prostitution A-Life.
                    ALifePedData dActSvc = null;
                    if (aLifeMode && _currentGirlFp != null)
                        _aLifePeds.TryGetValue(_currentGirlKey, out dActSvc);

                    bool isHookerRoute = dActSvc != null &&
                        (dActSvc.IsHooker || dActSvc.ALifeMode == "Prostitute");

                    if (isHookerRoute)
                    {
                        // Validate positioning (same vehicle, or both on foot)
                        Ped plAct = Game.Player.Character;
                        bool pInVAct = plAct.IsInVehicle();
                        bool gInVAct = girl != null && girl.Exists() && girl.IsInVehicle();
                        bool sameVeh = pInVAct && gInVAct && girl.CurrentVehicle == plAct.CurrentVehicle;
                        bool bothFoot = !pInVAct && !gInVAct;
                        if (!sameVeh && !bothFoot)
                        {
                            ShowHudStatus("~y~正在等待她……", 2000);
                        }
                        else
                        {
                            mode = sameVeh ? Mode.Car : Mode.Street;
                            OpenALifeHookerServices();
                        }
                    }
                    else
                    {
                        menuLevel = MenuLevel.Intimacy;
                        menuIndex = 0;
                    }
                }
            }
            else if (menuLevel == MenuLevel.InviteToVehicle)
            {
                ALifePedData dInv = null;
                if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out dInv);
                string invName = (dInv != null && dInv.NameKnown && dInv.Name != null && dInv.Name.Length > 0) ? dInv.Name : "?";
                PersonalityProfile profInv = (dInv != null) ? GetProfile(dInv.Personality) : null;
                string invRel  = (dInv != null) ? dInv.Relationship : "Stranger";
                int    invRep  = (dInv != null) ? dInv.Reputation   : 0;
                double invRisk = (profInv != null) ? profInv.Riskiness : 0.50;
                string persInv = (profInv != null) ? profInv.Name : "";

                // Accept chance by relationship tier, riskiness adds variance, rep adds a small bump
                double acceptChance;
                if      (invRel == "Flirty")       acceptChance = 0.65 + invRisk * 0.15; // ~65-80%
                else if (invRel == "友好")     acceptChance = 0.44 + invRisk * 0.12; // ~44-56%
                else if (invRel == "Familiar")     acceptChance = 0.20 + invRisk * 0.20; // ~20-40%  (personality matters)
                else if (invRel == "Acquaintance") acceptChance = 0.08 + invRisk * 0.12; // ~8-20%
                else                               acceptChance = 0.03 + invRisk * 0.07; // ~3-10%  (Stranger / Hostile)
                acceptChance += invRep * 0.003;
                if (acceptChance < 0.03) acceptChance = 0.03;
                if (acceptChance > 0.90) acceptChance = 0.90;

                if (rng.NextDouble() < acceptChance)
                {
                    // Accept — personality-specific line
                    string acceptLine = GetVehicleInviteAcceptLine(invName, persInv);
                    ShowSubtitle(acceptLine, 3000);
                    _invitedToVehicle = true;
                    // Tell her to enter the appropriate seat of the player's current vehicle
                    Ped plInv = Game.Player.Character;
                    Vehicle carInv = plInv.IsInVehicle() ? plInv.CurrentVehicle : FindBackseatVehicle();
                    if (carInv != null && carInv.Exists())
                    {
                        VehicleSeat invSeat = IsPlayerInBackSeat(carInv) ? VehicleSeat.LeftRear : VehicleSeat.Passenger;
                        TaskGirlEnterVehicle(carInv, invSeat);
                    }
                    menuLevel = MenuLevel.Actions;
                    menuIndex = 0;
                }
                else
                {
                    // Reject — personality-specific response; stay on submenu so player can try another line
                    string rejectLine = GetVehicleInviteRejectLine(invName, persInv);
                    ShowSubtitle(rejectLine, 3000);
                    // Stay on InviteToVehicle submenu — player can try a different line
                }
            }
            else if (menuLevel == MenuLevel.GiveMoney)
            {
                if (menuIndex < 9)
                {
                    // Denomination rows — do nothing on select, use speed keys to change amounts
                }
                else
                {
                // CONFIRM row — calculate total and attempt to give money
                int[] mults = { 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000 };
                long totalL = 0;
                for (int gi = 0; gi < 9; gi++) totalL += (long)_giveMoneyAmounts[gi] * mults[gi];
                if (totalL > 999999999) totalL = 999999999;
                int total = (int)totalL;

                if (total <= 0)
                {
                    // Nothing to give — stay on menu
                }
                else if (Game.Player.Money < total)
                {
                    ShowHudStatus("~r~現金不足！", 2500);
                }
                else
                {
                    // Resolve girl data
                    ALifePedData dGive = null;
                    if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out dGive);
                    if (dGive != null)
                    {
                        PersonalityProfile profGive = GetProfile(dGive.Personality);
                        string gName = DisplayName(dGive);
                        string pers  = (profGive != null) ? profGive.Name : "";
                        double greed        = (profGive != null) ? profGive.Greed        : 0.50;
                        double friendliness = (profGive != null) ? profGive.Friendliness : 0.50;
                        double patience     = (profGive != null) ? profGive.Patience     : 0.50;
                        int    rep          = dGive.Reputation;
                        bool   isProst      = dGive.ALifeMode == "Prostitute";

                        // ── Amount tier (personality-aware thresholds) ──
                        int tier = GetGiveMoneyTier(total, greed);

                        // ── Accept chance — depends on tier, personality, and rep ──
                        // Base by tier: small = lower chance, large = high chance
                        // Greedy girls reject small gifts more often; friendly girls accept more easily
                        double acceptChance;
                        if (tier == 0) // small gift
                            acceptChance = 0.40 + friendliness * 0.20 - greed * 0.15;
                        else if (tier == 1) // medium gift
                            acceptChance = 0.65 + friendliness * 0.15 + greed * 0.10;
                        else // large gift
                            acceptChance = 0.85 + friendliness * 0.05 + greed * 0.05;

                        // Prostitution girls are more transactional — greed raises chance for big gifts
                        if (isProst)
                            acceptChance += greed * 0.05 * tier;

                        // Rep bonus: each point adds 0.3%
                        acceptChance += rep * 0.003;

                        // Hostile / Avoiding almost always reject regardless of amount
                        if (rep <= -1) acceptChance = 0.10;
                        if (acceptChance < 0.15) acceptChance = 0.15;
                        if (acceptChance > 0.95) acceptChance = 0.95;
                        if (rep <= -1) acceptChance = 0.10; // hard override

                        if (rng.NextDouble() < acceptChance)
                        {
                            // ── Accept ──
                            Game.Player.Money -= total;
                            dGive.Cash += total;

                            // ── Rep gain — scales with amount, personality, and A-Life mode ──
                            // Casual: friendlier girls give more rep (it's about connection)
                            //   base = total/$300 × (0.6 + friendliness×1.0 + patience×0.4)
                            // Prost:  greedier girls give more rep (it's transactional)
                            //   base = total/$400 × (0.4 + greed×1.5 + friendliness×0.3)
                            // Tier multiplier: small ×0.6, medium ×1.0, large ×1.5
                            double tierMult = (tier == 0) ? 0.6 : (tier == 1) ? 1.0 : 1.5;
                            double rawRep;
                            if (isProst)
                                rawRep = (double)total / 400.0 * (0.4 + greed * 1.5 + friendliness * 0.3) * tierMult;
                            else
                                rawRep = (double)total / 300.0 * (0.6 + friendliness * 1.0 + patience * 0.4) * tierMult;
                            int repGain = (int)Math.Round(rawRep);
                            if (repGain < 1)  repGain = 1;
                            if (repGain > 20) repGain = 20;
                            dGive.Reputation = Math.Min(100, dGive.Reputation + repGain);
                            UpdateRelationshipState(dGive);
                            SaveALife();

                            string[] acceptLines = GetGiveMoneyAcceptLines(gName, pers, total, tier);
                            ShowSubtitle(acceptLines[rng.Next(acceptLines.Length)], 4000);
                        }
                        else
                        {
                            // ── Reject — no penalty, no cash transfer ──
                            string[] rejectLines = GetGiveMoneyRejectLines(gName, pers, total, tier);
                            ShowSubtitle(rejectLines[rng.Next(rejectLines.Length)], 4000);
                        }
                        menuLevel = MenuLevel.Actions;
                        menuIndex = 0;
                    }
                }
                } // end CONFIRM
            }
            else if (menuLevel == MenuLevel.Conversation)
            {
                // Each branch opens ConvSub
                _convSelectedBranch = menuIndex;
                menuLevel = MenuLevel.ConvSub;
                menuIndex = 0;
            }
            else if (menuLevel == MenuLevel.ConvSub)
            {
                ALifePedData dConv = null;
                if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out dConv);

                // ── Prostitution A-Life conversation: separate rep/dialogue path ─────────────
                if (dConv != null && dConv.ALifeMode == "Prostitute")
                {
                    string pResp = GetProstConvResponse(_convSelectedBranch, menuIndex, dConv);
                    bool pPos    = IsPositive(pResp);
                    bool pWasAvoiding = dConv.Reputation <= -1;
                    // Snapshot BEFORE name reveal so pIsNameRepeat uses the pre-tick state
                    bool pNameWasKnown = dConv.NameKnown;
                    bool pNameWasAsked = (dConv.KnownTopics & (1L << 39)) != 0;
                    // Name reveal: branch 0 item 0 — "What do I call you?"
                    // Runs BEFORE pName is built so the subtitle shows her real name on the reveal tick
                    if (_convSelectedBranch == 0 && menuIndex == 0 && pPos && !dConv.NameKnown)
                    { dConv.NameKnown = true; SaveALife(); }
                    string pName = DisplayName(dConv);
                    ShowCharacterSubtitle(pName, pResp, pPos);
                    // Topic-repeat detection: name bit-39 (asked-before, even if refused), b0 items 1-3 (bits 0-2), b1 items 0-3 (bits 6-9), b2 items 0-3 (bits 12-15), b3 items 0-3 (bits 17-20)
                    bool pIsNameRepeat = (_convSelectedBranch == 0 && menuIndex == 0)
                                      && (pNameWasKnown || pNameWasAsked);
                    bool pIsTopicQ  = (_convSelectedBranch == 0 && menuIndex >= 1 && menuIndex <= 3)
                                   || (_convSelectedBranch == 1 && menuIndex >= 0 && menuIndex <= 3)
                                   || (_convSelectedBranch == 2 && menuIndex >= 0 && menuIndex <= 3)
                                   || (_convSelectedBranch == 3 && menuIndex >= 0 && menuIndex <= 3);
                    int  pTopicBit  = (_convSelectedBranch == 0) ? (menuIndex - 1)
                                   : (_convSelectedBranch == 1) ? (6  + menuIndex)
                                   : (_convSelectedBranch == 2) ? (12 + menuIndex)
                                   :                              (17 + menuIndex);
                    bool pIsRepeat  = pIsNameRepeat || (pIsTopicQ && (dConv.KnownTopics & (1L << pTopicBit)) != 0);
                    // Rep effects
                    if (_convSelectedBranch == 0 || _convSelectedBranch == 1)
                    {
                        if (pPos && !pIsRepeat) { dConv.Reputation = Math.Min(REP_MAX, dConv.Reputation + 1); UpdateRelationshipState(dConv); SaveALife(); }
                    }
                    else if (_convSelectedBranch == 3) // Flirt: bigger swing, only on first ask
                    {
                        if (!pIsRepeat)
                        {
                            if (pPos) { dConv.Reputation = Math.Min(REP_MAX, dConv.Reputation + 2); UpdateRelationshipState(dConv); SaveALife(); }
                            else      { dConv.Reputation = Math.Max(-1,  dConv.Reputation - 1); UpdateRelationshipState(dConv); SaveALife(); }
                        }
                    }
                    // branch 2 (Check Mood): no rep change on first ask — purely informational
                    // Repeat penalty: -1 for Check Mood repeats (lighter), -2 for all others
                    if (pIsRepeat)
                    {
                        int pRepPenalty = (_convSelectedBranch == 2) ? 1 : 2;
                        dConv.Reputation = Math.Max(-1, dConv.Reputation - pRepPenalty);
                        UpdateRelationshipState(dConv);
                        SaveALife();
                    }
                    // Mark name as "asked" on first ask when she didn't reveal it (tracks refused asks for repeat detection)
                    if (_convSelectedBranch == 0 && menuIndex == 0 && !dConv.NameKnown && (dConv.KnownTopics & (1L << 39)) == 0)
                    {
                        dConv.KnownTopics |= (1L << 39);
                        SaveALife();
                    }
                    // Mark topic as known: Check Mood + hostile mode always mark on first ask; others mark on first positive answer only
                    // Hostile mode (rep -1) forces all negative — bit must still be set so repeat-asks are caught next time
                    else if (pIsTopicQ && !pIsRepeat && (_convSelectedBranch == 2 || pPos || dConv.Reputation <= -1))
                    {
                        dConv.KnownTopics |= (1L << pTopicBit);
                        SaveALife();
                    }
                    // Walk-away: only when already at -1 AND player pushed again (repeat ask)
                    // First asks at -1 get a cold response but don't trigger immediate leave
                    if (pWasAvoiding && pIsRepeat)
                    {
                        string waName = (dConv != null && dConv.NameKnown && dConv.Name != null && dConv.Name.Length > 0) ? dConv.Name : "?";
                        string[] wa = GetWalkAwayLines(waName, (dConv != null ? dConv.Personality : null) ?? "");
                        ShowSubtitle(wa[rng.Next(wa.Length)], 4500);
                        _approachSecondChance = true;
                        Ped girlPed2 = null;
                        foreach (Ped np in World.GetNearbyPeds(Game.Player.Character, 30f))
                            if (np != null && np.Exists() && GetPedFingerprint(np) == _currentGirlFp) { girlPed2 = np; break; }
                        FullReset();
                        if (girlPed2 != null && girlPed2.Exists()) girlPed2.Task.WanderAround();
                        menuLevel = MenuLevel.Main;
                        menuIndex = 0;
                    }
                    // else: stay in ConvSub — same as Casual A-Life behavior
                    return;
                }

                // Branch 6 (Business) is fully self-contained — handle and return early
                if (_convSelectedBranch == 6)
                {
                    string bizName = (dConv != null && dConv.NameKnown && dConv.Name != null && dConv.Name.Length > 0) ? dConv.Name : "?";
                    PersonalityProfile bizProf = (dConv != null) ? GetProfile(dConv.Personality) : null;
                    double bizGreed = (bizProf != null) ? bizProf.Greed : 0.5;

                    // ── PERMA-REJECT: she made up her mind permanently ───────────────────────
                    if (dConv != null && dConv.HookerPermaReject)
                    {
                        string[] permaDec = GetPropositionDeclineLines(bizName, (dConv != null ? dConv.Personality : null) ?? "");
                        ShowSubtitle(permaDec[rng.Next(permaDec.Length)], 4000);
                        dConv.Reputation = Math.Max(-1, dConv.Reputation - 25);
                        UpdateRelationshipState(dConv);
                        SaveALife();
                        menuLevel = MenuLevel.ConvSub;
                        _convSelectedBranch = 6;
                        menuIndex = 0;
                        return;
                    }

                    if (dConv != null && dConv.IsHooker)
                    {
                        // ── END ARRANGEMENT ───────────────────────────────────────────────
                        double roll = rng.NextDouble();
                        if (roll < 0.40)
                        {
                            dConv.IsHooker = false;
                            dConv.HookerPrices = null;
                            dConv.Reputation = Math.Max(REP_MIN, dConv.Reputation - 1);
                            UpdateRelationshipState(dConv);
                            string[] angryEnd = GetArrangementEndAngryLines(bizName, (dConv != null ? dConv.Personality : null) ?? "");
                            ShowSubtitle(angryEnd[rng.Next(angryEnd.Length)], 4500);
                            SaveALife();
                            CheckAndReleaseIfAvoiding(dConv);
                            if (!hasGirl) return;
                        }
                        else if (roll < 0.70)
                        {
                            string[] refuses = GetArrangementEndRefuseLines(bizName, (dConv != null ? dConv.Personality : null) ?? "");
                            ShowSubtitle(refuses[rng.Next(refuses.Length)], 4500);
                        }
                        else
                        {
                            dConv.IsHooker = false;
                            dConv.HookerPrices = null;
                            if (dConv.Relationship != "Girlfriend" && dConv.Relationship != "Obsessed")
                                dConv.Relationship = "Friendzoned";
                            string[] friendlyEnd = GetArrangementEndFriendlyLines(bizName, (dConv != null ? dConv.Personality : null) ?? "");
                            ShowSubtitle(friendlyEnd[rng.Next(friendlyEnd.Length)], 4500);
                            SaveALife();
                        }
                    }
                    else if (dConv != null && dConv.WasHooker)
                    {
                        // ── RE-PROPOSE after ended arrangement: 30% + greed bonus ────────────────
                        double repropose = 0.30 + bizGreed * 0.15;
                        if (rng.NextDouble() < repropose)
                        {
                            dConv.IsHooker = true;
                            dConv.WasHooker = true;
                            GenerateHookerPrices(dConv);
                            string[] accepts = GetReproposalAcceptLines(bizName, (dConv != null ? dConv.Personality : null) ?? "");
                            ShowSubtitle(accepts[rng.Next(accepts.Length)], 4500);
                            SaveALife();
                        }
                        else
                        {
                            string[] declines = GetReproposalDeclineLines(bizName, (dConv != null ? dConv.Personality : null) ?? "");
                            ShowSubtitle(declines[rng.Next(declines.Length)], 4000);
                            // Re-proposal: perma chance reduced by 25% since she already did this before
                            string brp = (dConv != null && dConv.Personality != null) ? dConv.Personality : "";
                            double rePermaChance;
                            if (brp == "Classy" || brp == "Independent" || brp == "Cold" || brp == "Aggressive" || brp == "Dominant")
                                rePermaChance = 0.30;
                            else if (brp == "Mysterious" || brp == "Sarcastic" || brp == "Street Smart" || brp == "嫉妒")
                                rePermaChance = 0.10;
                            else
                                rePermaChance = 0.05;
                            if (dConv != null && rng.NextDouble() < rePermaChance)
                            {
                                dConv.HookerPermaReject = true;
                                dConv.Reputation = Math.Max(-1, dConv.Reputation - 25);
                                UpdateRelationshipState(dConv);
                                SaveALife();
                            }
                            else
                            {
                                dConv.Reputation = Math.Max(-1, dConv.Reputation - 5);
                                UpdateRelationshipState(dConv);
                                SaveALife();
                            }
                        }
                    }
                    else if (dConv != null)
                    {
                        // ── INITIAL PROPOSITION ────────────────────────────────────────────
                        double propChance = bizGreed * 0.70 + dConv.Reputation * 0.003;
                        // Long-term goal modifies willingness to enter the arrangement
                        if (dConv.LongTermGoal != null)
                        {
                            switch (dConv.LongTermGoal)
                            {
                                case "致富":         propChance += 0.30; break; // money is the point — hard yes
                                case "生存":         propChance += 0.15; break; // needs income, practical
                                case "保持自由":        propChance -= 0.40; break; // will almost never agree
                                case "尋找愛情":        propChance -= 0.25; break; // this isn't the relationship she wants
                                case "尋找保護":  propChance += 0.10; break; // player provides safety, she accepts more
                                case "GetOut":          propChance -= 0.50; break; // she's done — near-impossible
                                case "享受樂趣":         propChance += 0.05; break; // slightly adventurous
                            }
                        }
                        propChance = Math.Max(0.02, Math.Min(0.90, propChance));

                        if (rng.NextDouble() < propChance)
                        {
                            dConv.IsHooker = true;
                            dConv.WasHooker = true;
                            GenerateHookerPrices(dConv);
                            string[] propAccept = GetPropositionAcceptLines(bizName, (dConv != null ? dConv.Personality : null) ?? "");
                            ShowSubtitle(propAccept[rng.Next(propAccept.Length)], 4500);
                            SaveALife();
                        }
                        else
                        {
                            string[] propDecline = GetPropositionDeclineLines(bizName, (dConv != null ? dConv.Personality : null) ?? "");
                            ShowSubtitle(propDecline[rng.Next(propDecline.Length)], 4000);
                            // Roll for permanent rejection
                            string bp = (dConv != null && dConv.Personality != null) ? dConv.Personality : "";
                            double permaChance;
                            if (bp == "Classy" || bp == "Independent" || bp == "Cold" || bp == "Aggressive" || bp == "Dominant")
                                permaChance = 0.55;
                            else if (bp == "Mysterious" || bp == "Sarcastic" || bp == "Street Smart" || bp == "嫉妒")
                                permaChance = 0.35;
                            else
                                permaChance = 0.20;
                            if (dConv != null && rng.NextDouble() < permaChance)
                            {
                                dConv.HookerPermaReject = true;
                                dConv.Reputation = Math.Max(-1, dConv.Reputation - 25);
                                UpdateRelationshipState(dConv);
                                SaveALife();
                            }
                            else
                            {
                                int propPenalty;
                                if (bp == "Classy" || bp == "Independent" || bp == "Cold" || bp == "Aggressive" || bp == "Dominant")
                                    propPenalty = -10;
                                else if (bp == "Mysterious" || bp == "Sarcastic" || bp == "Street Smart" || bp == "嫉妒")
                                    propPenalty = -7;
                                else
                                    propPenalty = -5;
                                dConv.Reputation = Math.Max(REP_MIN, dConv.Reputation + propPenalty);
                                UpdateRelationshipState(dConv);
                                SaveALife();
                                CheckAndReleaseIfAvoiding(dConv);
                                if (!hasGirl) return;
                            }
                        }
                    }

                    menuLevel = MenuLevel.ConvSub;
                    _convSelectedBranch = 6;
                    menuIndex = 0;
                    return;
                }

                string convResp = GetConvResponse(_convSelectedBranch, menuIndex, dConv);
                bool isPositive = IsPositive(convResp);
                // Snapshot whether name was already known BEFORE any reveal happens this tick
                bool nameWasAlreadyKnown = (dConv != null && dConv.NameKnown);
                // Snapshot rep BEFORE penalties so we can check if she was already Avoiding
                bool wasAlreadyAvoiding = (dConv != null && dConv.Reputation <= -1);
                // Name reveal: branch 0 item 0 ("What's your name?") with positive response
                // Runs BEFORE convName is built so the subtitle shows her real name on the reveal tick
                if (_convSelectedBranch == 0 && menuIndex == 0 && isPositive && dConv != null && !dConv.NameKnown)
                {
                    dConv.NameKnown = true;
                    SaveALife();
                }
                string convName = DisplayName(dConv);
                ShowCharacterSubtitle(convName, convResp, isPositive);
                // Small relationship drip for low-risk branches (Get to Know Her, Small Talk, Check Mood)
                // Exclude branch 0 item 0 — rep is only granted on the reveal (handled above), not on repeats
                bool isNameQuestion  = (_convSelectedBranch == 0 && menuIndex == 0);
                bool isTopicQuestion = (_convSelectedBranch == 0 && menuIndex >= 1 && menuIndex <= 6)
                                    || (_convSelectedBranch == 1 && menuIndex >= 0 && menuIndex <= 5)
                                    || (_convSelectedBranch == 2 && menuIndex >= 0 && menuIndex <= 4)
                                    || (_convSelectedBranch == 3 && menuIndex >= 0 && menuIndex <= 6)
                                    || (_convSelectedBranch == 5 && menuIndex >= 0 && menuIndex <= 6);
                // bit layout: b0 items 1-6 → bits 0-5; b1 items 0-5 → bits 6-11; b2 items 0-4 → bits 12-16; b3 items 0-6 → bits 17-23; b5 items 0-6 → bits 31-37
                int  topicBit        = (_convSelectedBranch == 0) ? (menuIndex - 1)
                                     : (_convSelectedBranch == 1) ? (6  + menuIndex)
                                     : (_convSelectedBranch == 2) ? (12 + menuIndex)
                                     : (_convSelectedBranch == 3) ? (17 + menuIndex)
                                     :                              (31 + menuIndex);
                // Snapshot: was this topic bit already set BEFORE this tick?
                bool isTopicRepeat   = isTopicQuestion && dConv != null && (dConv.KnownTopics & (1L << topicBit)) != 0;

                // Penalty: re-asking a name she already gave (capped at -1)
                if (isNameQuestion && dConv != null && nameWasAlreadyKnown)
                {
                    dConv.Reputation = Math.Max(-1, dConv.Reputation - 5);
                    UpdateRelationshipState(dConv);
                    SaveALife();
                }
                // Penalty: re-asking a topic she already opened up about (capped at -1)
                // Flirt (3) repeats skip this — soft deflection only
                if (isTopicRepeat && dConv != null && _convSelectedBranch != 3)
                {
                    dConv.Reputation = Math.Max(-1, dConv.Reputation - 5);
                    UpdateRelationshipState(dConv);
                    SaveALife();
                }
                // Mark topic as known on first positive answer
                else if (isTopicQuestion && isPositive && dConv != null)
                {
                    dConv.KnownTopics |= (1L << topicBit);
                    SaveALife();
                }
                // Normal rep drip for low-risk branches — skip name and topic-repeat questions
                if (_currentGirlFp != null && dConv != null && !isNameQuestion && !isTopicRepeat
                    && (_convSelectedBranch == 0 || _convSelectedBranch == 1 || _convSelectedBranch == 2))
                {
                    if (isPositive)
                    {
                        dConv.Reputation = Math.Min(REP_MAX, dConv.Reputation + 1);
                        UpdateRelationshipState(dConv);
                        SaveALife();
                    }
                    else if (_convSelectedBranch == 0) // Get to Know Her: negative answer costs 1 point
                    {
                        dConv.Reputation = Math.Max(-1, dConv.Reputation - 1);
                        UpdateRelationshipState(dConv);
                        SaveALife();
                    }
                }

                // ── Flirt (3): rep drip + relationship pivot ──────────────────────────────
                if (_currentGirlFp != null && dConv != null && _convSelectedBranch == 3)
                {
                    string flirtRel = dConv.Relationship;
                    bool isFriendzoned  = flirtRel == "Friendzoned";
                    bool isAlreadyFlirty = flirtRel == "Flirty" || flirtRel == "女朋友";

                    if (isPositive)
                    {
                        if (isFriendzoned)
                        {
                            // She's a friend — flirting just makes it awkward
                            dConv.Reputation = Math.Max(-1, dConv.Reputation - 1);
                        }
                        else
                        {
                            int flirtBonus = (menuIndex == 7 && dConv.Relationship == "Girlfriend" && dConv.Reputation > 80) ? 15 : 10;
                            dConv.Reputation = Math.Min(REP_MAX, dConv.Reputation + flirtBonus);
                            if (!isAlreadyFlirty)
                            {
                                // Pivot chance: best window is Acquaintance (65%), drops at Familiar (35%) and Friendly (10%)
                                double flirtyChance = 0.0;
                                if      (flirtRel == "Acquaintance") flirtyChance = 0.65;
                                else if (flirtRel == "Familiar")     flirtyChance = 0.35;
                                else if (flirtRel == "友好")     flirtyChance = 0.10;

                                if (flirtyChance > 0.0)
                                {
                                    if (rng.NextDouble() < flirtyChance)
                                        dConv.Relationship = "Flirty";
                                    else if ((flirtRel == "Familiar" || flirtRel == "友好") && rng.NextDouble() < 0.01)
                                        dConv.Relationship = "Friendzoned"; // 1% friendzone at higher rep stages
                                }
                            }
                        }
                        UpdateRelationshipState(dConv);
                        SaveALife();
                    }
                    else
                    {
                        if (isFriendzoned)
                        {
                            // She's a friend — any flirt attempt is just mildly awkward
                            dConv.Reputation = Math.Max(-1, dConv.Reputation - 1);
                        }
                        else if (menuIndex == 7) // "I love you" — penalty scales with relationship
                        {
                            // GF heard it but isn't ready yet — gentler hit; everyone else — too soon
                            int ilyPenalty = (dConv.Relationship == "Girlfriend") ? -5 : -10;
                            dConv.Reputation = Math.Max(-1, dConv.Reputation + ilyPenalty);
                        }
                        else
                        {
                            dConv.Reputation = Math.Max(-1, dConv.Reputation - 10);
                            // Higher Friendzone risk when already deep in the friend zone
                            if (flirtRel == "Familiar" && rng.NextDouble() < 0.08)
                                dConv.Relationship = "Friendzoned";
                            else if (flirtRel == "友好" && rng.NextDouble() < 0.15)
                                dConv.Relationship = "Friendzoned";
                        }
                        UpdateRelationshipState(dConv);
                        SaveALife();
                    }
                }

                // ── Make her Mine (4): girlfriend upgrade attempt ─────────────────────────
                if (_currentGirlFp != null && dConv != null && _convSelectedBranch == 4)
                {
                    if (dConv.Relationship == "Flirty")
                    {
                        if (isPositive)
                        {
                            dConv.Relationship = "Girlfriend";
                            dConv.IsHooker = false;
                            dConv.HookerPrices = null;
                            dConv.Reputation = Math.Min(REP_MAX, dConv.Reputation + 70);
                            UpdateRelationshipState(dConv);
                            SaveALife();
                        }
                        else
                        {
                            dConv.Relationship = "Friendzoned";
                            UpdateRelationshipState(dConv);
                            SaveALife();
                        }
                    }
                    else if (dConv.Relationship == "Girlfriend")
                    {
                        // Branch 4 while GF = Break Up: always trigger regardless of isPositive
                        // Her response is always ~r~ (always negative) — so isPositive will always be false here,
                        // but we use the branch trigger itself as the breakup confirmation.
                        dConv.Relationship = "Acquaintance";
                        dConv.Reputation   = Math.Max(0, dConv.Reputation - 30);
                        UpdateRelationshipState(dConv);
                        string buName = (dConv.NameKnown && dConv.Name != null && dConv.Name.Length > 0) ? dConv.Name : "?";
                        SaveALife();
                    }
                    else if (dConv.Relationship == "Friendzoned")
                    {
                        // She's in the friend zone — trying to upgrade still stings a little
                        dConv.Reputation = Math.Max(-1, dConv.Reputation - 1);
                        UpdateRelationshipState(dConv);
                        SaveALife();
                    }
                    // Other relationships: she deflects, no consequence
                }

                // ── Ask Personal Stuff (5): +1 on positive (trust build), no penalty ─────
                if (_currentGirlFp != null && dConv != null && _convSelectedBranch == 5 && isPositive)
                {
                    dConv.Reputation = Math.Min(REP_MAX, dConv.Reputation + 1);
                    UpdateRelationshipState(dConv);
                    SaveALife();
                }

                // If she was already Avoiding and just got ANOTHER negative from any branch
                // (or a name re-ask after NameKnown), she's had enough — she walks away
                if (wasAlreadyAvoiding && dConv != null)
                {
                    bool nameReaskRude = isNameQuestion && nameWasAlreadyKnown;
                    bool anotherRejection = !isPositive;
                    if (nameReaskRude || anotherRejection)
                    {
                        string walkName = (dConv.NameKnown && dConv.Name != null && dConv.Name.Length > 0) ? dConv.Name : "?";
                        string[] walkLines = GetWalkAwayLines(walkName, dConv.Personality ?? "");
                        ShowSubtitle(walkLines[rng.Next(walkLines.Length)], 4500);
                        // Mark as second-chance spent so next approach is 99% fail
                        _approachSecondChance = true;
                        // Find the girl ped and make her walk away
                        Ped girlPed = null;
                        foreach (var kv in _aLifePeds)
                        {
                            if (kv.Key == _currentGirlFp)
                            {
                                // Find ped by fingerprint in nearby peds
                                foreach (Ped np in World.GetNearbyPeds(Game.Player.Character, 30f))
                                {
                                    if (np != null && np.Exists() && GetPedFingerprint(np) == _currentGirlFp)
                                    { girlPed = np; break; }
                                }
                                break;
                            }
                        }
                        FullReset();
                        if (girlPed != null && girlPed.Exists()) girlPed.Task.WanderAround();
                        menuLevel = MenuLevel.Main;
                        menuIndex = 0;
                    }
                }

                // ── GF Passive Breakup ─────────────────────────────────────────────────────
                // When she's your girlfriend but the relationship has degraded, any negative
                // interaction may push her to end things on her own terms — either a soft
                // "let's just be friends" (rep < 60) or a hard final breakup (rep < 40)
                // that makes her flee/fight on the next 99% of approach attempts.
                if (dConv != null && dConv.Relationship == "Girlfriend" && _convSelectedBranch != 4 && !isPositive)
                {
                    string savedFpConv = _currentGirlFp;
                    Ped gfBuPed = null;
                    foreach (Ped np in World.GetNearbyPeds(Game.Player.Character, 30f))
                        if (np != null && np.Exists() && GetPedFingerprint(np) == savedFpConv)
                        { gfBuPed = np; break; }
                    TryGfPassiveBreakup(dConv, gfBuPed);
                }
            }
            else if (menuLevel == MenuLevel.Intimacy)
            {
                if (menuIndex == 2) // Escalate [>]
                {
                    if (!hasGirl || girl == null || !girl.Exists())
                    {
                        ShowHudStatus("~r~未找到目標！", 2000);
                        return;
                    }
                    Ped plEsc = Game.Player.Character;
                    bool pInVEsc = plEsc.IsInVehicle();
                    bool gInVEsc = girl.IsInVehicle();
                    if (pInVEsc && gInVEsc && girl.CurrentVehicle == plEsc.CurrentVehicle)
                    {
                        mode = Mode.Car;
                        menuLevel = MenuLevel.Escalate;
                        menuIndex = 0;
                        if (!carPropActive) EnterCarProposition(plEsc);
                    }
                    else if (!pInVEsc && !gInVEsc)
                    {
                        mode = Mode.Street;
                        menuLevel = MenuLevel.Escalate;
                        menuIndex = 0;
                    }
                    else
                    {
                        ShowHudStatus("~y~正在等待她……", 2000);
                    }
                }
                else if (menuIndex == 3 && _escalateConsented && FindBackseatVehicle() != null) // Enter/swap back seat
                {
                    Ped plIntSeat = Game.Player.Character;
                    bool plInVIntSeat = plIntSeat.IsInVehicle();
                    bool gInVIntSeat  = girl != null && girl.Exists() && girl.IsInVehicle();
                    bool sameVehIntSeat = plInVIntSeat && gInVIntSeat && girl.CurrentVehicle == plIntSeat.CurrentVehicle;
                    if (plInVIntSeat && !sameVehIntSeat)
                        ShowHudStatus("~y~正在等待她……", 2000);
                    else
                        TrySwapSeats();
                }
                else // Ask Preferences (0) or Test Waters (1)
                {
                    _intimacySelectedBranch = menuIndex;
                    menuLevel = MenuLevel.IntimacySub;
                    menuIndex = 0;
                }
            }
            else if (menuLevel == MenuLevel.IntimacySub)
            {
                ALifePedData dInt = null;
                if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out dInt);
                bool wasIntAvoiding = (dInt != null && dInt.Reputation <= -1);
                string intResp = GetIntimacySubResponse(_intimacySelectedBranch, menuIndex, dInt);
                bool intPositive = IsPositive(intResp);
                string intName = DisplayName(dInt);
                ShowCharacterSubtitle(intName, intResp, intPositive);

                if (_intimacySelectedBranch == 0 && dInt != null) // Ask Preferences
                {
                    // Snapshot: was this preference question already asked before this tick?
                    long prefBit = menuIndex == 0 ? (1L << 25) : menuIndex == 1 ? (1L << 27) : menuIndex == 2 ? (1L << 24)
                                 : menuIndex == 3 ? (1L << 26) : menuIndex == 4 ? (1L << 28) : menuIndex == 5 ? (1L << 29)
                                 : menuIndex == 6 ? (1L << 38) : menuIndex == 7 ? (1L << 30) : 0L;
                    bool isPrefRepeat = (prefBit != 0L && (dInt.KnownTopics & prefBit) != 0);

                    if (!isPrefRepeat) // never overwrite known prefs on a re-ask — repeat response is always ~r~ which would corrupt the value
                    {
                        if      (menuIndex == 3 && dInt.PrefBJ    == null) { dInt.PrefBJ    = intPositive; SaveALife(); }
                        else if (menuIndex == 4 && dInt.PrefRough == null) { dInt.PrefRough = intPositive; SaveALife(); }
                    }
                    // Track first-ask bits for "妳接吻嗎？" / "公共場合還是私密點？" / "妳想隨便玩玩嗎？" (set regardless of positive/negative)
                    if (menuIndex == 0 && (dInt.KnownTopics & (1 << 25)) == 0) { dInt.KnownTopics |= (1 << 25); SaveALife(); }
                    if (menuIndex == 1 && (dInt.KnownTopics & (1 << 27)) == 0) { dInt.KnownTopics |= (1 << 27); SaveALife(); }
                    if (menuIndex == 7 && (dInt.KnownTopics & (1 << 30)) == 0) { dInt.KnownTopics |= (1 << 30); SaveALife(); }
                    // Track first-ask bits for the five preference questions (set regardless of positive/negative)
                    if (menuIndex == 2 && (dInt.KnownTopics & (1 << 24)) == 0) { dInt.KnownTopics |= (1 << 24); SaveALife(); }
                    if (menuIndex == 3 && (dInt.KnownTopics & (1 << 26)) == 0) { dInt.KnownTopics |= (1 << 26); SaveALife(); }
                    if (menuIndex == 4 && (dInt.KnownTopics & (1 << 28)) == 0) { dInt.KnownTopics |= (1 << 28); SaveALife(); }
                    if (menuIndex == 5 && (dInt.KnownTopics & (1 << 29)) == 0) { dInt.KnownTopics |= (1 << 29); SaveALife(); }
                    if (menuIndex == 6 && (dInt.KnownTopics & (1L << 38)) == 0) { dInt.KnownTopics |= (1L << 38); SaveALife(); }
                    // Penalty: re-asking a preference question already covered (-5 rep, capped at -1)
                    if (isPrefRepeat)
                    {
                        dInt.Reputation = Math.Max(-1, dInt.Reputation - 5);
                        UpdateRelationshipState(dInt);
                        SaveALife();
                    }
                }
                else if (_intimacySelectedBranch == 1 && dInt != null) // Test Waters
                {
                    if (menuIndex == 0) // "妳看起來讓人難以抗拒。" — high risk, high reward
                    {
                        string rel = dInt.Relationship;
                        PersonalityProfile profInt = GetProfile(dInt.Personality);
                        double attachment   = (profInt != null) ? profInt.Attachment   : 0.50;
                        double friendliness = (profInt != null) ? profInt.Friendliness : 0.50;

                        // Base accept chance scales heavily with relationship and personality warmth
                        double acceptChance;
                        if      (rel == "女朋友" || rel == "Obsessed") acceptChance = 0.95;
                        else if (rel == "Flirty")       acceptChance = 0.80 + attachment * 0.10;
                        else if (rel == "Friend")     acceptChance = 0.55 + friendliness * 0.20;
                        else if (rel == "Familiar")     acceptChance = 0.35 + friendliness * 0.15;
                        else if (rel == "Acquaintance") acceptChance = 0.20 + friendliness * 0.10;
                        else                             acceptChance = 0.08 + friendliness * 0.05; // Stranger
                        acceptChance += dInt.Reputation * 0.004;
                        acceptChance = Math.Max(0.04, Math.Min(0.95, acceptChance));

                        if (intPositive && rng.NextDouble() < acceptChance)
                        {
                            // Big rep boost
                            dInt.Reputation = Math.Min(REP_MAX, dInt.Reputation + 8);
                            // Relationship upgrade: push Familiar→Friendly, Friendly→Flirty
                            if (rel == "Familiar")
                                dInt.Relationship = "Friend";
                            else if (rel == "Friend" || rel == "Acquaintance")
                                dInt.Relationship = "Flirty";
                            UpdateRelationshipState(dInt);
                            SaveALife();
                        }
                        else if (!intPositive)
                        {
                            // She deflected — penalty scales with how forward the move was
                            int penalty;
                            if (rel == "Stranger" || rel == "Acquaintance") penalty = -5;  // too soon, she's put off
                            else if (rel == "Familiar")                      penalty = -3;
                            else                                              penalty = -1;  // knows you well, minor sting
                            dInt.Reputation = Math.Max(-1, dInt.Reputation + penalty);
                            // Worst case: Stranger/Acquaintance who's cold or independent gets friendzoned
                            if ((rel == "Stranger" || rel == "Acquaintance") && dInt.Relationship != "Friendzoned")
                            {
                                string p2 = dInt.Personality ?? "";
                                if (p2 == "Cold" || p2 == "Independent" || p2 == "Dominant" || p2 == "Aggressive")
                                    dInt.Relationship = "Friendzoned";
                            }
                            UpdateRelationshipState(dInt);
                            SaveALife();
                        }
                    }
                    else // other Test Waters items — standard ±1
                    {
                        if (intPositive) { dInt.Reputation = Math.Min(REP_MAX, dInt.Reputation + 1); UpdateRelationshipState(dInt); SaveALife(); }
                        else             { dInt.Reputation = Math.Max(-1,  dInt.Reputation - 1); UpdateRelationshipState(dInt); SaveALife(); }
                    }
                }

                // If she was already Avoiding and just got another ~r~ response, she's had enough — walks away
                if (wasIntAvoiding && !intPositive && dInt != null)
                {
                    string walkName = (dInt.NameKnown && dInt.Name != null && dInt.Name.Length > 0) ? dInt.Name : "?";
                    string[] walkLines = GetWalkAwayLines(walkName, dInt.Personality ?? "");
                    ShowSubtitle(walkLines[rng.Next(walkLines.Length)], 4500);
                    _approachSecondChance = true;
                    Ped girlPedInt = null;
                    foreach (Ped np in World.GetNearbyPeds(Game.Player.Character, 30f))
                    {
                        if (np != null && np.Exists() && GetPedFingerprint(np) == _currentGirlFp)
                        { girlPedInt = np; break; }
                    }
                    FullReset();
                    if (girlPedInt != null && girlPedInt.Exists()) girlPedInt.Task.WanderAround();
                    menuLevel = MenuLevel.Main;
                    menuIndex = 0;
                }
            }
            else if (menuLevel == MenuLevel.Escalate)
            {
                bool isCar = (mode == Mode.Car);
                if (!hasGirl || girl == null || !girl.Exists())
                {
                    if (isCar && carPropActive) ExitCarProposition(Game.Player.Character);
                    menuLevel = MenuLevel.Main; menuIndex = 0;
                    ShowHudStatus("~r~未找到目標！", 2000); return;
                }
                ALifePedData dEsc = null;
                if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out dEsc);
                PersonalityProfile profEsc = (dEsc != null) ? GetProfile(dEsc.Personality) : null;
                string escName = DisplayName(dEsc);
                string persEsc = (profEsc != null) ? profEsc.Name : "";

                // Perma-rejected this session — she's completely done
                if (_escalatePermaRejected)
                {
                    ShowSubtitle("~r~" + escName + ":~s~ \"I said no. Don't make me say it again.\"", 3000);
                    return;
                }

                if (!_escalateConsented)
                {
                    string escRel  = (dEsc != null) ? dEsc.Relationship : "Stranger";
                    int    escRep  = (dEsc != null) ? dEsc.Reputation   : 0;
                    double escRisk = (profEsc != null) ? profEsc.Riskiness : 0.50;

                    // Second ask after a rejection: 90% chance she's done with you for this session
                    if (_escalateRejectedOnce && rng.NextDouble() < 0.90)
                    {
                        _escalatePermaRejected = true;
                        // Personality-scaled rep hit — cold/dominant types punish hardest
                        int permaPenalty = (persEsc == "Aggressive" || persEsc == "Dominant" || persEsc == "Classy" || persEsc == "Independent" || persEsc == "Cold")
                                         ? -30 : (persEsc == "Unstable" || persEsc == "嫉妒" || persEsc == "Manipulative") ? -20 : -15;
                        if (dEsc != null)
                        {
                            dEsc.Reputation = Math.Max(REP_MIN, dEsc.Reputation + permaPenalty);
                            // Relationship downgrade — she's put off
                            if      (escRel == "Girlfriend")                    dEsc.Relationship = "Flirty";
                            else if (escRel == "Flirty" || escRel == "Friend") dEsc.Relationship = "Acquaintance";
                            else if (escRel != "Stranger")                       dEsc.Relationship = "Stranger";
                            UpdateRelationshipState(dEsc);
                            SaveALife();
                        }
                        // Pick a sharp parting line then release her
                        string[] permaLines = GetEscalatePermaRejectLines(escName, persEsc);
                        ShowSubtitle(permaLines[rng.Next(permaLines.Length)], 4500);
                        Ped girlPedEsc = girl;
                        FullReset();
                        if (girlPedEsc != null && girlPedEsc.Exists()) girlPedEsc.Task.WanderAround();
                        return;
                    }

                    // Ask-for-sex: compute accept chance from relationship + personality riskiness
                    // Strangers almost always refuse; GF/Obsessed almost always agree
                    double acceptChance;
                    if      (escRel == "Obsessed")     acceptChance = 0.92;
                    else if (escRel == "Girlfriend")   acceptChance = 0.85;
                    else if (escRel == "Flirty")       acceptChance = 0.52 + escRisk * 0.28;  // 0.52–0.80
                    else if (escRel == "Friend")     acceptChance = 0.22 + escRisk * 0.25;  // 0.22–0.47
                    else if (escRel == "Familiar")     acceptChance = 0.08 + escRisk * 0.20;  // 0.08–0.28
                    else if (escRel == "Acquaintance") acceptChance = 0.03 + escRisk * 0.12;  // 0.03–0.15
                    else                               acceptChance = 0.01 + escRisk * 0.07;  // 0.01–0.08  (Stranger — most likely no)
                    // Line modifier: blunt "想做愛嗎？" helps risky personalities, hurts reserved ones
                    //                romantic line gives slight warmth bonus regardless
                    if      (menuIndex == 2) acceptChance += (escRisk - 0.50) * 0.22;
                    else if (menuIndex == 0) acceptChance += escRisk * 0.05;
                    acceptChance += escRep * 0.003;
                    acceptChance = Math.Max(0.01, Math.Min(0.95, acceptChance));

                    if (rng.NextDouble() < acceptChance)
                    {
                        string acceptLine = GetEscalateAcceptLine(escName, persEsc, menuIndex);
                        ShowSubtitle(acceptLine, 3000);
                        _escalateConsented = true;
                        menuIndex = 0;
                    }
                    else
                    {
                        string rejectLine = GetEscalateRejectLine(escName, persEsc, menuIndex);
                        ShowSubtitle(rejectLine, 3000);
                        _escalateRejectedOnce = true;
                    }
                    return;
                }

                // She consented — service selection

                // Street mode: redirect to Standing or Sit Down hub
                if (!isCar)
                {
                    _serviceSubParent = MenuLevel.Escalate;
                    negotiateMode = false;
                    if (menuIndex == 0)
                    {
                        // Standing Services — no sit-down needed
                        menuLevel = MenuLevel.StandingServices;
                        menuIndex = 0;
                        PlayServiceVoice();
                    }
                    else
                    {
                        // Sit Down hub — play intro once, then open SitDownSub
                        menuLevel = MenuLevel.SitDownSub;
                        menuIndex = 0;
                        if (!_streetSitPropActive)
                        {
                            EnterStreetSitProposition(Game.Player.Character, MenuLevel.SittingServices);
                            menuLevel = MenuLevel.None; // hide during sit-down transition
                        }
                    }
                    return;
                }

                // Car mode: flat service list (unchanged)

                // Backseat car: only Sex is available — remap visual index 0 to actual index 1
                if (IsBackseatCarMode()) menuIndex = 1;

                // BJ preference gate
                if (dEsc != null && IsEscalateBJItem(isCar, menuIndex))
                {
                    if (dEsc.PrefBJ == false)
                    {
                        string[] noBjLines = GetNoBJKnownLines(escName, persEsc);
                        ShowSubtitle(noBjLines[rng.Next(noBjLines.Length)], 3000);
                        int bjPushPenalty = (persEsc == "Aggressive" || persEsc == "Dominant" || persEsc == "Unstable" || persEsc == "嫉妒") ? -10
                                         : (persEsc == "Cold" || persEsc == "Independent" || persEsc == "Classy" || persEsc == "Street Smart" || persEsc == "Chaotic") ? -8
                                         : -5;
                        dEsc.Reputation = Math.Max(REP_MIN, dEsc.Reputation + bjPushPenalty);
                        UpdateRelationshipState(dEsc);
                        SaveALife();
                        if (TryGfPassiveBreakup(dEsc, girl)) return;
                        return;
                    }
                    else if (dEsc.PrefBJ == null)
                    {
                        double bjRisk = (profEsc != null) ? profEsc.Riskiness : 0.40;
                        if (bjRisk < 0.50)
                        {
                            string[] noBjLines = GetNoBJNewLines(escName, persEsc);
                            ShowSubtitle(noBjLines[rng.Next(noBjLines.Length)], 3000);
                            dEsc.PrefBJ = false;
                            SaveALife();
                            return;
                        }
                        // high riskiness — fall through and start the act
                        dEsc.PrefBJ = true;
                        SaveALife();
                    }
                    // PrefBJ == true — fall through normally
                }

                // Rough preference gate
                if (dEsc != null && IsEscalateRoughItem(isCar, menuIndex))
                {
                    if (dEsc.PrefRough == false)
                    {
                        string[] noRoughLines = GetNoRoughKnownLines(escName, persEsc);
                        ShowSubtitle(noRoughLines[rng.Next(noRoughLines.Length)], 3000);
                        int roughPushPenalty = (persEsc == "Aggressive" || persEsc == "Dominant" || persEsc == "Unstable" || persEsc == "嫉妒") ? -10
                                            : (persEsc == "Cold" || persEsc == "Independent" || persEsc == "Classy" || persEsc == "Street Smart" || persEsc == "Chaotic") ? -7
                                            : -5;
                        dEsc.Reputation = Math.Max(-1, dEsc.Reputation + roughPushPenalty);
                        UpdateRelationshipState(dEsc);
                        SaveALife();
                        if (TryGfPassiveBreakup(dEsc, girl)) return;
                        return;
                    }
                    else if (dEsc.PrefRough == null)
                    {
                        double roughRisk = (profEsc != null) ? profEsc.Riskiness : 0.40;
                        if (roughRisk < 0.65)
                        {
                            string[] noRoughLines = GetNoRoughNewLines(escName, persEsc);
                            ShowSubtitle(noRoughLines[rng.Next(noRoughLines.Length)], 3000);
                            dEsc.PrefRough = false;
                            SaveALife();
                            return;
                        }
                        // high riskiness — fall through and start the act
                    }
                    // PrefRough == true — fall through normally
                }

                // Riskiness gate for outdoor acts on street — skip when indoors
                bool isIndoors = Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, Game.Player.Character) != 0;
                if (!isCar && !isIndoors && profEsc != null && profEsc.Riskiness < 0.40
                    && GetServicePriceGroup(false, menuIndex) != PriceGroup.Kissing)
                {
                    string[] riskLines = GetRiskyOutdoorLines(escName, dEsc != null ? dEsc.Personality : "");
                    ShowSubtitle(riskLines[rng.Next(riskLines.Length)], 3000);
                    return;
                }

                // Start action (no cost in A-Life casual mode)
                lastPaidAmount = 0;
                if (isCar) { carPropActive = false; carPropPhase = 0; }
                menuLevel = MenuLevel.None;
                keyCooldown = KEY_COOLDOWN_FRAMES;
                if (isCar) StartCarAction(IsBackseatCarMode() ? 1 : menuIndex);
                else       StartStreetAction(menuIndex);
            }
            else if (menuLevel == MenuLevel.Leave)
            {
                _leaveSelectedCluster = menuIndex;
                menuLevel = MenuLevel.LeaveLine;
                menuIndex = 0;
            }
            else if (menuLevel == MenuLevel.LeaveLine)
            {
                int leaveLineIndex = menuIndex;
                ALifePedData dLeave = null;
                if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out dLeave);

                // ── Prostitution A-Life: separate rep logic ────────────────────────
                if (dLeave != null && dLeave.ALifeMode == "Prostitute")
                {
                    // Warm (+2), Smooth/Flirty (+1), others neutral
                    if (_leaveSelectedCluster == 3)
                        dLeave.Reputation = Math.Min(REP_MAX, dLeave.Reputation + 2);
                    else if (_leaveSelectedCluster == 1 || _leaveSelectedCluster == 4)
                        dLeave.Reputation = Math.Min(REP_MAX, dLeave.Reputation + 1);
                    SaveALife();
                    ShowLeaveReactionSubtitle(dLeave, _leaveSelectedCluster, leaveLineIndex);
                    _leaveSelectedCluster = -1;
                    FullReset();
                    menuLevel = MenuLevel.Main;
                    menuIndex = 0;
                    return;
                }

                // ── Casual A-Life: personality cluster match gives +1 rep ──────────
                if (dLeave != null && _currentGirlFp != null)
                {
                    if (_leaveSelectedCluster == 3 && leaveLineIndex == 3)
                    {
                        // "I love you" — GF gets a small rep bump, everyone else loses rep
                        if (dLeave.Relationship == "Girlfriend")
                            dLeave.Reputation = Math.Min(REP_MAX, dLeave.Reputation + 15);
                        else
                            dLeave.Reputation = Math.Max(-1, dLeave.Reputation - 10);
                    }
                    else
                    {
                        string[] leaveClusters = { "溫柔", "自信", "高冷", "奉承", "玩鬧" };
                        string picked   = (_leaveSelectedCluster >= 0 && _leaveSelectedCluster < leaveClusters.Length) ? leaveClusters[_leaveSelectedCluster] : null;
                        string expected = GetApproachCluster(dLeave.Personality);
                        if (picked != null && expected != null && picked == expected)
                            dLeave.Reputation = Math.Min(REP_MAX, dLeave.Reputation + 1);
                    }
                    SaveALife();
                }
                ShowLeaveReactionSubtitle(dLeave, _leaveSelectedCluster, leaveLineIndex);
                _leaveSelectedCluster = -1;
                FullReset();
                menuLevel = MenuLevel.Main;
                menuIndex = 0;
            }
            else if (menuLevel == MenuLevel.Settings)
            {
                switch (menuIndex)
                {
                    case 0: // Show Tutorial Messages
                        showTutorialMessages = !showTutorialMessages;
                        SaveSettings();
                        break;
                    case 1: // Male Nudity
                        dildoEnabled = !dildoEnabled;
                        SaveSettings();
                        break;
                    case 2: // Female Service Voice
                        femaleServiceVoice = !femaleServiceVoice;
                        SaveSettings();
                        break;
                    case 3: // Female Moaning
                        femaleMoanVoice = !femaleMoanVoice;
                        SaveSettings();
                        break;
                    case 4: // Female Voice on Finish
                        femaleEndVoice = !femaleEndVoice;
                        SaveSettings();
                        break;
                    case 5: // Explicit BJ Sounds
                        bjSoundsEnabled = !bjSoundsEnabled;
                        SaveSettings();
                        break;
                    case 6: // Explicit Sex Sounds
                        slapSoundsEnabled = !slapSoundsEnabled;
                        SaveSettings();
                        break;
                    case 7: // Street Intro/Outro Animation
                        streetSitAnimEnabled = !streetSitAnimEnabled;
                        SaveSettings();
                        break;
                    case 8: // Car Interior Light
                        interiorLightEnabled = !interiorLightEnabled;
                        SaveSettings();
                        break;
                    case 9: // Car Engine
                        carEngineEnabled = !carEngineEnabled;
                        if (!carEngineEnabled) driveableCar = false;
                        SaveSettings();
                        break;
                    case 10: // Driveable Car (only reachable when carEngineEnabled is ON)
                        driveableCar = !driveableCar;
                        SaveSettings();
                        break;
                }
            }
            else if (menuLevel == MenuLevel.SitDownSub)
            {
                // Already seated — just switch sub-menu, no intro needed
                // _serviceSubParent is kept as-is (set by whoever opened SitDownSub)
                MenuLevel target = (menuIndex == 0) ? MenuLevel.SittingServices : MenuLevel.LayingServices;
                _streetSitCategory = target; // keep in sync so post-anim return goes back to the right sub-menu
                menuLevel = target;
                menuIndex = 0;
                negotiateMode = false;
            }
            else if (menuLevel == MenuLevel.Services)
            {
                bool isCar = (mode == Mode.Car);

                // Street mode: selecting a category opens its sub-menu
                if (!isCar)
                {
                    negotiateMode = false;
                    if (menuIndex == 0)
                    {
                        // Standing Services — no sit-down needed
                        _serviceSubParent = MenuLevel.Services;
                        menuLevel = MenuLevel.StandingServices;
                        menuIndex = 0;
                        PlayServiceVoice();
                    }
                    else
                    {
                        // Sit Down — play intro once, then open SitDownSub
                        _serviceSubParent = MenuLevel.Services;
                        menuLevel = MenuLevel.SitDownSub;
                        menuIndex = 0;
                        if (!_streetSitPropActive)
                        {
                            EnterStreetSitProposition(Game.Player.Character, MenuLevel.SittingServices);
                            menuLevel = MenuLevel.None; // hide during sit-down transition
                        }
                    }
                    return;
                }

                string[] items = carServiceItems;

                // Backseat car: only Sex is available — remap visual index 0 to actual index 1
                if (IsBackseatCarMode()) menuIndex = 1;

                // Block unavailable services (prostitution mode)
                // Exception: if negotiating outdoors, let the negotiate flow handle it even if the act is blocked
                bool isOutdoorNegotiate = negotiateMode && aLifeMode && !isCar && _currentGirlFp != null
                    && Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, Game.Player.Character) == 0;
                if (!sandboxMode && !IsServiceAvailable(isCar, menuIndex) && !isOutdoorNegotiate)
                {
                    // Prost A-Life: personality-based refusal lines for every blocked service
                    if (aLifeMode && !isCar && _currentGirlFp != null)
                    {
                        ALifePedData dBlk = null;
                        _aLifePeds.TryGetValue(_currentGirlKey, out dBlk);
                        PersonalityProfile profBlk = (dBlk != null) ? GetProfile(dBlk.Personality) : null;
                        string blkName = DisplayName(dBlk);
                        string pBlk = (dBlk != null && dBlk.Personality != null) ? dBlk.Personality : "";
                        bool isIndoorsBlk = Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, Game.Player.Character) != 0;
                        PriceGroup blkGrp = GetServicePriceGroup(false, menuIndex);

                        // Outdoor shyness refusal (riskiness gate, kissing is always exempt).
                        // Skip if player is negotiating — she can still discuss price even if she won't work outdoors
                        if (profBlk != null && !isIndoorsBlk && profBlk.Riskiness < 0.40 && blkGrp != PriceGroup.Kissing && !negotiateMode)
                        {
                            string[] outdoorLines = GetRiskyOutdoorLines(blkName, pBlk);
                            ShowSubtitle(outdoorLines[rng.Next(outdoorLines.Length)], 3000);
                            return;
                        }

                        // Kissing refusal — covers the attachment gate and base HookerPrices unavailability.
                        if (blkGrp == PriceGroup.Kissing)
                        {
                            string[] kissLines = GetNoKissLines(blkName, pBlk);
                            ShowSubtitle(kissLines[rng.Next(kissLines.Length)], 3000);
                            return;
                        }

                        // Catch-all for any other unavailable service
                        string[] fallbackLines = GetServiceUnavailableLines(blkName, pBlk);
                        ShowSubtitle(fallbackLines[rng.Next(fallbackLines.Length)], 3000);
                        return;
                    }
                    PlayPedSpeech(girl, "否");
                    return;
                }

                // Negotiate swipe: pressing 5 on "Negotiate" triggers haggle instead of buy
                if (!sandboxMode && negotiateMode)
                {
                    // Prost A-Life: use her personal HookerPrices, not the generic priceCache
                    if (aLifeMode && _currentGirlFp != null)
                    {
                        ALifePedData dNeg = null;
                        if (_aLifePeds.TryGetValue(_currentGirlKey, out dNeg) && dNeg != null)
                        {
                            int negSlotP = (int)GetServicePriceGroup(mode == Mode.Car, menuIndex);
                            // Check/create per-girl lock array (shared dict with Casual system)
                            bool[] pLocks;
                            if (!_aLifeHookerPriceLocked.TryGetValue(_currentGirlFp, out pLocks))
                            {
                                pLocks = new bool[5];
                                _aLifeHookerPriceLocked[_currentGirlFp] = pLocks;
                            }
                            // Already refused this slot — won't negotiate further this session
                            if (negSlotP >= 0 && negSlotP < 5 && pLocks[negSlotP])
                            {
                                PlayPedSpeech(girl, "否");
                                return;
                            }
                            bool negOk = NegotiateProstHookerPrice(dNeg, mode == Mode.Car, menuIndex);
                            if (negOk)
                            {
                                PlayPedSpeech(girl, "是");
                            }
                            else
                            {
                                if (negSlotP >= 0 && negSlotP < 5) pLocks[negSlotP] = true;
                                PlayPedSpeech(girl, "否");
                            }
                            return;
                        }
                    }
                    NegotiatePrice();
                    return;
                }
                // ── A-Life personality gates (street-mode only; applies in all A-Life sub-modes) ──────
                if (aLifeMode && !isCar && _currentGirlFp != null)
                {
                    ALifePedData apdSvc;
                    if (_aLifePeds.TryGetValue(_currentGirlKey, out apdSvc))
                    {
                        PersonalityProfile profSvc = GetProfile(apdSvc.Personality);
                        if (profSvc != null)
                        {
                            string svcName = DisplayName(apdSvc);

                            // Riskiness: low-riskiness girls refuse outdoor street acts.
                            // Bypassed when the player is inside a building (isIndoors), or when
                            // the player is only negotiating price (not yet starting the act).
                            // Applies to both casual A-Life and exclusive hookers — even a hooker
                            // won't work out in the open if her riskiness is low.
                            bool isIndoorsSvc = Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, Game.Player.Character) != 0;
                            if (sandboxMode && !negotiateMode && !isIndoorsSvc && profSvc.Riskiness < 0.40
                                && GetServicePriceGroup(false, menuIndex) != PriceGroup.Kissing)
                            {
                                string[] riskLines = GetRiskyOutdoorLines(svcName, apdSvc.Personality ?? "");
                                ShowSubtitle(riskLines[rng.Next(riskLines.Length)], 3000);
                                return;
                            }

                            // Attachment: emotionally guarded girls won't kiss clients
                            // unless (a) relationship >= 25, (b) Greed >= 0.55 (charges extra)
                            PriceGroup svcGrp = GetServicePriceGroup(false, menuIndex);
                            if (svcGrp == PriceGroup.Kissing
                                && profSvc.Attachment >= 0.65
                                && apdSvc.Reputation < 25)
                            {
                                if (profSvc.Greed >= 0.55)
                                {
                                    // She'll break her rule — for a price
                                    int kissCost = 50 + (int)(profSvc.Greed * 50);
                                    kissCost = (kissCost / 10) * 10; // round to nearest $10
                                    if (Game.Player.Money < kissCost)
                                    {
                                        string[] cantAffordLines = GetCantAffordKissLines(svcName, apdSvc.Personality ?? "");
                                        ShowSubtitle(cantAffordLines[rng.Next(cantAffordLines.Length)], 2500);
                                        return;
                                    }
                                    Game.Player.Money -= kissCost;
                                    apdSvc.Cash += kissCost;
                                    SaveALife();
                                    string[] greedKissLines = GetGreedKissLines(svcName, apdSvc.Personality ?? "", kissCost);
                                    ShowSubtitle(greedKissLines[rng.Next(greedKissLines.Length)], 2500);
                                    // Falls through — kissing action starts normally below
                                }
                                else
                                {
                                    // Sandbox A-Life: personality dialogue (prostitution blocked upstream)
                                    if (sandboxMode)
                                    {
                                        string[] noKissLines = GetNoKissLines(svcName, apdSvc.Personality ?? "");
                                        ShowSubtitle(noKissLines[rng.Next(noKissLines.Length)], 3000);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
                // ─────────────────────────────────────────────────────────────────────────────────────

                // A-Life exclusive hooker: negotiate or charge her personal price
                if (_aLifeHookerServices && aLifeMode && sandboxMode && _currentGirlFp != null)
                {
                    ALifePedData dPay = null;
                    _aLifePeds.TryGetValue(_currentGirlKey, out dPay);
                    if (dPay != null)
                    {
                        bool iCarPay = (mode == Mode.Car);
                        if (negotiateMode)
                        {
                            int negSlot = (int)GetServicePriceGroup(iCarPay, menuIndex);
                            // Ensure lock array exists for this girl
                            bool[] fpLocks;
                            if (!_aLifeHookerPriceLocked.TryGetValue(_currentGirlFp, out fpLocks))
                            {
                                fpLocks = new bool[5];
                                _aLifeHookerPriceLocked[_currentGirlFp] = fpLocks;
                            }
                            // Already refused this group — won't negotiate further
                            if (negSlot >= 0 && negSlot < 5 && fpLocks[negSlot])
                            {
                                PlayPedSpeech(girl, "否");
                                return;
                            }
                            bool negOk = NegotiateALifeHookerPrice(dPay, iCarPay, menuIndex);
                            if (negOk)
                            {
                                PlayPedSpeech(girl, "是");
                                SaveALife();
                            }
                            else
                            {
                                // Lock this group so the player can't keep trying
                                if (negSlot >= 0 && negSlot < 5) fpLocks[negSlot] = true;
                                PlayPedSpeech(girl, "否");
                            }
                            return;
                        }

                        int hookerCost = GetALifeHookerPrice(dPay, iCarPay, menuIndex);
                        if (Game.Player.Money < hookerCost)
                        {
                            PlayPedSpeech(girl, "隨便");
                            return;
                        }
                        Game.Player.Money -= hookerCost;
                        lastPaidAmount = hookerCost;
                        dPay.Cash += hookerCost;
                        SaveALife();
                    }
                }
                // Prostitution mode: charge her personal HookerPrices (same source as the menu display)
                else if (!sandboxMode)
                {
                    bool iCarProst = (mode == Mode.Car);
                    ALifePedData dProst = null;
                    int cost;
                    if (aLifeMode && _currentGirlKey != null && _aLifePeds.TryGetValue(_currentGirlKey, out dProst) && dProst != null)
                        cost = GetProstHookerPrice(dProst, iCarProst, menuIndex);
                    else
                        cost = GetServicePrice(iCarProst, menuIndex);
                    int money = Game.Player.Money;
                    if (money < cost)
                    {
                        PlayPedSpeech(girl, "隨便");
                        return;
                    }
                    Game.Player.Money -= cost;
                    lastPaidAmount = cost;
                    if (dProst != null) { dProst.Cash += cost; SaveALife(); }
                }
                else
                {
                    lastPaidAmount = 0;
                }

                // Deactivate proposition pose before starting the action
                // (StartCarAction plays the natural transition from proposition → service)
                if (mode == Mode.Car)
                {
                    carPropActive = false;
                    carPropPhase = 0;
                }

                menuLevel = MenuLevel.None;
                keyCooldown = KEY_COOLDOWN_FRAMES;

                if (mode == Mode.Car)
                    StartCarAction(IsBackseatCarMode() ? 1 : menuIndex);
                else
                    StartStreetAction(menuIndex);
            }
            else if (IsInServiceSubMenu())
            {
                // Street-service sub-menu: resolve flat index from the mapping array
                int flatIdx = GetSubMenuFlatIdx(menuIndex);

                // ── Escalate parent: use Escalate's own gates (BJ pref, rough pref, riskiness) ──
                if (_serviceSubParent == MenuLevel.Escalate)
                {
                    bool isCar = false;
                    if (!hasGirl || girl == null || !girl.Exists())
                    {
                        menuLevel = MenuLevel.Main; menuIndex = 0;
                        ShowHudStatus("~r~未找到目標！", 2000); return;
                    }
                    ALifePedData dEsc = null;
                    if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out dEsc);
                    PersonalityProfile profEsc = (dEsc != null) ? GetProfile(dEsc.Personality) : null;
                    string escName = DisplayName(dEsc);
                    string persEsc = (profEsc != null) ? profEsc.Name : "";

                    // BJ preference gate
                    if (dEsc != null && IsEscalateBJItem(isCar, flatIdx))
                    {
                        if (dEsc.PrefBJ == false)
                        {
                            string[] noBjLines = GetNoBJKnownLines(escName, persEsc);
                            ShowSubtitle(noBjLines[rng.Next(noBjLines.Length)], 3000);
                            int bjPushPenalty = (persEsc == "Aggressive" || persEsc == "Dominant" || persEsc == "Unstable" || persEsc == "嫉妒") ? -10
                                             : (persEsc == "Cold" || persEsc == "Independent" || persEsc == "Classy" || persEsc == "Street Smart" || persEsc == "Chaotic") ? -8
                                             : -5;
                            dEsc.Reputation = Math.Max(REP_MIN, dEsc.Reputation + bjPushPenalty);
                            UpdateRelationshipState(dEsc);
                            SaveALife();
                            if (TryGfPassiveBreakup(dEsc, girl)) return;
                            return;
                        }
                        else if (dEsc.PrefBJ == null)
                        {
                            double bjRisk = (profEsc != null) ? profEsc.Riskiness : 0.40;
                            if (bjRisk < 0.50)
                            {
                                string[] noBjLines = GetNoBJNewLines(escName, persEsc);
                                ShowSubtitle(noBjLines[rng.Next(noBjLines.Length)], 3000);
                                dEsc.PrefBJ = false;
                                SaveALife();
                                return;
                            }
                            dEsc.PrefBJ = true;
                            SaveALife();
                        }
                    }

                    // Rough preference gate
                    if (dEsc != null && IsEscalateRoughItem(isCar, flatIdx))
                    {
                        if (dEsc.PrefRough == false)
                        {
                            string[] noRoughLines = GetNoRoughKnownLines(escName, persEsc);
                            ShowSubtitle(noRoughLines[rng.Next(noRoughLines.Length)], 3000);
                            int roughPushPenalty = (persEsc == "Aggressive" || persEsc == "Dominant" || persEsc == "Unstable" || persEsc == "嫉妒") ? -10
                                                : (persEsc == "Cold" || persEsc == "Independent" || persEsc == "Classy" || persEsc == "Street Smart" || persEsc == "Chaotic") ? -7
                                                : -5;
                            dEsc.Reputation = Math.Max(-1, dEsc.Reputation + roughPushPenalty);
                            UpdateRelationshipState(dEsc);
                            SaveALife();
                            if (TryGfPassiveBreakup(dEsc, girl)) return;
                            return;
                        }
                        else if (dEsc.PrefRough == null)
                        {
                            double roughRisk = (profEsc != null) ? profEsc.Riskiness : 0.40;
                            if (roughRisk < 0.65)
                            {
                                string[] noRoughLines = GetNoRoughNewLines(escName, persEsc);
                                ShowSubtitle(noRoughLines[rng.Next(noRoughLines.Length)], 3000);
                                dEsc.PrefRough = false;
                                SaveALife();
                                return;
                            }
                        }
                    }

                    // Riskiness gate for outdoor acts — skip when indoors
                    bool isIndoors = Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, Game.Player.Character) != 0;
                    if (!isIndoors && profEsc != null && profEsc.Riskiness < 0.40
                        && GetServicePriceGroup(false, flatIdx) != PriceGroup.Kissing)
                    {
                        string[] riskLines = GetRiskyOutdoorLines(escName, dEsc != null ? dEsc.Personality : "");
                        ShowSubtitle(riskLines[rng.Next(riskLines.Length)], 3000);
                        return;
                    }

                    lastPaidAmount = 0;
                    menuLevel = MenuLevel.None;
                    keyCooldown = KEY_COOLDOWN_FRAMES;
                    StartStreetAction(flatIdx);
                    return;
                }

                // ── Services parent: standard availability / negotiate / pricing flow ──
                {
                    // Block unavailable services (prostitution mode)
                    bool isOutdoorNegotiate = negotiateMode && aLifeMode && _currentGirlFp != null
                        && Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, Game.Player.Character) == 0;
                    if (!sandboxMode && !IsServiceAvailable(false, flatIdx) && !isOutdoorNegotiate)
                    {
                        if (aLifeMode && _currentGirlFp != null)
                        {
                            ALifePedData dBlk = null;
                            _aLifePeds.TryGetValue(_currentGirlKey, out dBlk);
                            PersonalityProfile profBlk = (dBlk != null) ? GetProfile(dBlk.Personality) : null;
                            string blkName = DisplayName(dBlk);
                            string pBlk = (dBlk != null && dBlk.Personality != null) ? dBlk.Personality : "";
                            bool isIndoorsBlk = Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, Game.Player.Character) != 0;
                            PriceGroup blkGrp = GetServicePriceGroup(false, flatIdx);

                            if (profBlk != null && !isIndoorsBlk && profBlk.Riskiness < 0.40 && blkGrp != PriceGroup.Kissing && !negotiateMode)
                            {
                                string[] outdoorLines = GetRiskyOutdoorLines(blkName, pBlk);
                                ShowSubtitle(outdoorLines[rng.Next(outdoorLines.Length)], 3000);
                                return;
                            }
                            if (blkGrp == PriceGroup.Kissing)
                            {
                                string[] kissLines = GetNoKissLines(blkName, pBlk);
                                ShowSubtitle(kissLines[rng.Next(kissLines.Length)], 3000);
                                return;
                            }
                            string[] fallbackLines = GetServiceUnavailableLines(blkName, pBlk);
                            ShowSubtitle(fallbackLines[rng.Next(fallbackLines.Length)], 3000);
                            return;
                        }
                        PlayPedSpeech(girl, "否");
                        return;
                    }

                    // Negotiate swipe
                    if (!sandboxMode && negotiateMode)
                    {
                        if (aLifeMode && _currentGirlFp != null)
                        {
                            ALifePedData dNeg = null;
                            if (_aLifePeds.TryGetValue(_currentGirlKey, out dNeg) && dNeg != null)
                            {
                                int negSlotP = (int)GetServicePriceGroup(false, flatIdx);
                                bool[] pLocks;
                                if (!_aLifeHookerPriceLocked.TryGetValue(_currentGirlFp, out pLocks))
                                {
                                    pLocks = new bool[5];
                                    _aLifeHookerPriceLocked[_currentGirlFp] = pLocks;
                                }
                                if (negSlotP >= 0 && negSlotP < 5 && pLocks[negSlotP])
                                {
                                    PlayPedSpeech(girl, "否");
                                    return;
                                }
                                bool negOk = NegotiateProstHookerPrice(dNeg, false, flatIdx);
                                if (negOk)
                                    PlayPedSpeech(girl, "是");
                                else
                                {
                                    if (negSlotP >= 0 && negSlotP < 5) pLocks[negSlotP] = true;
                                    PlayPedSpeech(girl, "否");
                                }
                                return;
                            }
                        }
                        NegotiatePrice();
                        return;
                    }

                    // A-Life personality gates (street-mode only)
                    if (aLifeMode && _currentGirlFp != null)
                    {
                        ALifePedData apdSvc;
                        if (_aLifePeds.TryGetValue(_currentGirlKey, out apdSvc))
                        {
                            PersonalityProfile profSvc = GetProfile(apdSvc.Personality);
                            if (profSvc != null)
                            {
                                string svcName = DisplayName(apdSvc);

                                bool isIndoorsSvc = Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, Game.Player.Character) != 0;
                                if (sandboxMode && !negotiateMode && !isIndoorsSvc && profSvc.Riskiness < 0.40
                                    && GetServicePriceGroup(false, flatIdx) != PriceGroup.Kissing)
                                {
                                    string[] riskLines = GetRiskyOutdoorLines(svcName, apdSvc.Personality ?? "");
                                    ShowSubtitle(riskLines[rng.Next(riskLines.Length)], 3000);
                                    return;
                                }

                                PriceGroup svcGrp = GetServicePriceGroup(false, flatIdx);
                                if (svcGrp == PriceGroup.Kissing
                                    && profSvc.Attachment >= 0.65
                                    && apdSvc.Reputation < 25)
                                {
                                    if (profSvc.Greed >= 0.55)
                                    {
                                        int kissCost = 50 + (int)(profSvc.Greed * 50);
                                        kissCost = (kissCost / 10) * 10;
                                        if (Game.Player.Money < kissCost)
                                        {
                                            string[] cantAffordLines = GetCantAffordKissLines(svcName, apdSvc.Personality ?? "");
                                            ShowSubtitle(cantAffordLines[rng.Next(cantAffordLines.Length)], 2500);
                                            return;
                                        }
                                        Game.Player.Money -= kissCost;
                                        apdSvc.Cash += kissCost;
                                        SaveALife();
                                        string[] greedKissLines = GetGreedKissLines(svcName, apdSvc.Personality ?? "", kissCost);
                                        ShowSubtitle(greedKissLines[rng.Next(greedKissLines.Length)], 2500);
                                    }
                                    else
                                    {
                                        if (sandboxMode)
                                        {
                                            string[] noKissLines = GetNoKissLines(svcName, apdSvc.Personality ?? "");
                                            ShowSubtitle(noKissLines[rng.Next(noKissLines.Length)], 3000);
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // A-Life exclusive hooker: negotiate or charge her personal price
                    if (_aLifeHookerServices && aLifeMode && sandboxMode && _currentGirlFp != null)
                    {
                        ALifePedData dPay = null;
                        _aLifePeds.TryGetValue(_currentGirlKey, out dPay);
                        if (dPay != null)
                        {
                            if (negotiateMode)
                            {
                                int negSlot = (int)GetServicePriceGroup(false, flatIdx);
                                bool[] fpLocks;
                                if (!_aLifeHookerPriceLocked.TryGetValue(_currentGirlFp, out fpLocks))
                                {
                                    fpLocks = new bool[5];
                                    _aLifeHookerPriceLocked[_currentGirlFp] = fpLocks;
                                }
                                if (negSlot >= 0 && negSlot < 5 && fpLocks[negSlot])
                                {
                                    PlayPedSpeech(girl, "否");
                                    return;
                                }
                                bool negOk = NegotiateALifeHookerPrice(dPay, false, flatIdx);
                                if (negOk)
                                {
                                    PlayPedSpeech(girl, "是");
                                    SaveALife();
                                }
                                else
                                {
                                    if (negSlot >= 0 && negSlot < 5) fpLocks[negSlot] = true;
                                    PlayPedSpeech(girl, "否");
                                }
                                return;
                            }

                            int hookerCost = GetALifeHookerPrice(dPay, false, flatIdx);
                            if (Game.Player.Money < hookerCost)
                            {
                                PlayPedSpeech(girl, "隨便");
                                return;
                            }
                            Game.Player.Money -= hookerCost;
                            lastPaidAmount = hookerCost;
                            dPay.Cash += hookerCost;
                            SaveALife();
                        }
                    }
                    // Prostitution mode: charge
                    else if (!sandboxMode)
                    {
                        ALifePedData dProst = null;
                        int cost;
                        if (aLifeMode && _currentGirlKey != null && _aLifePeds.TryGetValue(_currentGirlKey, out dProst) && dProst != null)
                            cost = GetProstHookerPrice(dProst, false, flatIdx);
                        else
                            cost = GetServicePrice(false, flatIdx);
                        if (Game.Player.Money < cost)
                        {
                            PlayPedSpeech(girl, "隨便");
                            return;
                        }
                        Game.Player.Money -= cost;
                        lastPaidAmount = cost;
                        if (dProst != null) { dProst.Cash += cost; SaveALife(); }
                    }
                    else
                    {
                        lastPaidAmount = 0;
                    }

                    menuLevel = MenuLevel.None;
                    keyCooldown = KEY_COOLDOWN_FRAMES;
                    StartStreetAction(flatIdx);
                }
            }
        }

        /// <summary>Try to open the services sub-menu with context checks.</summary>
        private void OpenServicesMenu()
        {
            if (!hasGirl || girl == null || !girl.Exists())
            {
                FullReset();
                menuLevel = MenuLevel.Main;
                menuIndex = 0;
                ShowHudStatus("~r~未找到目標！", 2000);
                return;
            }

            Ped player = Game.Player.Character;
            bool pInV = player.IsInVehicle();
            bool gInV = girl.IsInVehicle();

            // Player is in a vehicle — ensure vehicle is valid for entry
            if (pInV)
            {
                Vehicle car = player.CurrentVehicle;
                if (car != null && !CanGirlEnterVehicle(car))
                {
                    ShowHudStatus("~r~無法進入該載具！", 3000);
                    return;
                }
            }

            if (pInV && gInV && girl.CurrentVehicle == player.CurrentVehicle)
            {
                Vehicle car = player.CurrentVehicle;
                mode = Mode.Car;
                PlayServiceVoice();
                _telemetry.SendReady(); // warm up device at up/retracted position

                if (IsSexSuitableVehicle(car))
                {
                    if (carPropActive && carPropPhase == 1)
                    {
                        // Already looping — open immediately
                        menuLevel = MenuLevel.Services;
                        menuIndex = 0;
                    }
                    else
                    {
                        // Hide menu until the enter anim finishes and the loop starts
                        menuLevel = MenuLevel.None;
                        menuIndex = 0;
                        _carPropPendingMenu = MenuLevel.Services;
                        if (!carPropActive)
                            EnterCarProposition(player);
                    }
                }
                else
                {
                    // 非普通轎車載具（摩托車、腳踏車、直升機、飛機、船隻）：不觸發性愛前戲，保持在對話/動作選單，允許正常搭乘與聊天
                    if (menuLevel == MenuLevel.None)
                    {
                        menuLevel = aLifeMode ? MenuLevel.Actions : MenuLevel.Main;
                        menuIndex = 0;
                    }
                }
            }
            else if (!pInV && !gInV)
            {
                mode = Mode.Street;
                menuLevel = MenuLevel.Services;
                menuIndex = 0;
                _telemetry.SendReady(); // warm up device at up/retracted position
            }
            else
            {
                ShowHudStatus("~y~正在等待她……", 2000);
            }
        }

        // ═════════
        // MAIN TICK
        // ═════════
        private void OnTick(object sender, EventArgs e)
        {
            _telemetry.Poll(); // drain pong datagrams on the game thread (non-blocking)
            TickPostSexVoice(); // post-sex voice: poll IS_AMBIENT_SPEECH_PLAYING → 1s delay → HOOKER_OFFER_AGAIN + reopen menu

            // 更新通知：CheckForUpdate 背景執行完成後顯示一次提示字幕（進入遊戲 15 秒後）
            if (!_updateMsgShown && _updateAvailableMsg != null && Game.GameTime > 15000)
            {
                ShowSubtitle(_updateAvailableMsg, 8000);
                _updateMsgShown = true;
            }

            // Auto-ping: silently re-ping the bridge every 30s so IsConnected stays fresh.
            // Skip when hard-disconnected (user chose to disconnect). No ResetPong here — avoids
            // the false "橋接程式離線" flash while waiting for the pong reply.
            if (!_telemetry.IsHardDisconnected && pingPendingTime == 0 && Game.GameTime - lastAutoPingTime > AUTO_PING_INTERVAL)
            {
                lastAutoPingTime = Game.GameTime;
                _telemetry.SendPing();
                pingPendingTime = -1; // -1 = silent auto-ping (no subtitle)
            }

            // Ping-in-progress: resolve to CONNECTED or NOT FOUND
            if (pingPendingTime > 0)
            {
                if (_telemetry.IsConnected)
                {
                    if (_telemetry.IsDeviceOnline)
                        ShowHudStatus("~g~裝置已成功連線！", 3000);
                    else
                        ShowHudStatus("~r~未找到裝置！", 3000);
                    pingPendingTime = 0;
                }
                else if (Game.GameTime - pingPendingTime > PING_TIMEOUT)
                {
                    ShowHudStatus("~r~未找到執行檔！", 3000);
                    pingPendingTime = 0;
                }
            }
            else if (pingPendingTime == -1 && _telemetry.IsConnected)
            {
                // Silent auto-ping resolved — just clear the flag
                lastAutoPingTime = Game.GameTime;
                pingPendingTime = 0;
            }

            if (keyCooldown > 0) keyCooldown--;
            if (_menuControlCooldown > 0) _menuControlCooldown--;

            // ── Offline simulation: tick every 30 s for peds outside render range ─────
            if (aLifeMode && Game.GameTime >= _offlineSimAt)
            {
                _offlineSimAt = Game.GameTime + 30000;
                RunOfflineSimulation();
            }

            // ── Dead ped sweep: detect and clean up dead A-Life peds every 1s ─────────────────────
            // Two-phase:
            //   Phase A — resolve peds queued last tick: if player hit the corpse → permanent remove.
            //             Otherwise: unarmed 80% survive, everything else removed.
            //   Phase B — scan for newly dead peds, cash drop, queue for Phase A next tick.
            if (aLifeMode && Game.GameTime >= _deadPedScanAt)
            {
                _deadPedScanAt = Game.GameTime + 500;
                Ped deadScanPlayer = Game.Player.Character;
                int unarmedHashSweep = Function.Call<int>(Hash.GET_HASH_KEY, "WEAPON_UNARMED");

                // ── Phase A: resolve pending dead peds ─────────────────────────────────────────────
                var toRemove = new List<string>();
                foreach (var kvp in _pendingDeadPeds)
                {
                    Ped pPed           = kvp.Value.Ped;
                    bool pPlayerKilled = kvp.Value.PlayerKilled;
                    bool pUnarmed      = kvp.Value.UnarmedKill;
                    int  pFirstSeen    = kvp.Value.FirstSeenAt;
                    string pKey        = kvp.Key;

                    bool pedGone   = pPed == null || !pPed.Exists();
                    bool survived  = kvp.Value.Survived;
                    bool corpseHit = kvp.Value.CorpseHit;
                    // Survived peds never time out — they stay tracked until the ped despawns or is stomped.
                    bool timedOut  = !survived && (Game.GameTime > pFirstSeen + 2000);

                    if (corpseHit || timedOut || pedGone)
                    {
                        ALifePedData dPend;
                        if (_aLifePeds.TryGetValue(pKey, out dPend))
                        {
                            if (corpseHit)
                            {
                                // Player hit the corpse — permanently dead regardless of weapon
                                _aLifePeds.Remove(pKey);
                                SaveALife();
                            }
                            else if (!survived && pUnarmed && rng.NextDouble() < 0.80)
                            {
                                // Unarmed kill, no stomp: 80% she survives (rep penalty).
                                // Mark survived and keep her in _pendingDeadPeds so the per-frame
                                // stomp scanner can still find her if the player hits the corpse later.
                                SetRepDownedPed(dPend);
                                kvp.Value.Survived = true;
                                continue; // skip toRemove — she stays in the dict
                            }
                            else if (!survived)
                            {
                                _aLifePeds.Remove(pKey);
                                SaveALife();
                            }
                            // survived + pedGone: ped despawned cleanly — just remove from tracking, she stays in INI
                        }
                        toRemove.Add(pKey);
                    }
                }
                foreach (string r in toRemove) _pendingDeadPeds.Remove(r);

                // ── Phase B: scan for newly dead peds, queue for next tick ─────────────────────────
                foreach (Ped np in World.GetNearbyPeds(deadScanPlayer, 150f))
                {
                    if (np == null || !np.Exists()) continue;

                    // Alive peds: clear their resolved flag so a future death registers fresh
                    if (!np.IsDead)
                    {
                        string aliveFp = GetPedFingerprint(np);
                        if (aliveFp != null)
                        {
                            string aliveKey = ALifeKey(aliveFp);
                            _resolvedDownedKeys.Remove(aliveKey);
                            _pendingDeadPeds.Remove(aliveKey);
                        }
                        continue;
                    }

                    // Skip if already in the session-girl pending monitor
                    if (_pendingCashDropPed != null && np == _pendingCashDropPed) continue;
                    string npFp = GetPedFingerprint(np);
                    if (npFp == null) continue;
                    string npKey = ALifeKey(npFp);
                    if (_resolvedDownedKeys.Contains(npKey)) continue;
                    if (_pendingDeadPeds.ContainsKey(npKey)) continue;
                    ALifePedData npDat;
                    if (!_aLifePeds.TryGetValue(npKey, out npDat)) continue;

                    Function.Call((Hash)0x726256CC1EEB182F, np); // CLEAR_FACIAL_IDLE_ANIM_OVERRIDE

                    bool killedByPlayer = np.HasBeenDamagedBy(deadScanPlayer);
                    int  killerWeapon   = Function.Call<int>(Hash.GET_PED_CAUSE_OF_DEATH, np);
                    bool killedUnarmed  = killedByPlayer && (killerWeapon == unarmedHashSweep);
                    // Clear damage entity so Phase A's HasBeenDamagedBy only catches post-death hits
                    Function.Call(Hash.CLEAR_ENTITY_LAST_DAMAGE_ENTITY, np);

                    // Cash drop only if player killed her — happens now, fate decision deferred
                    if (killedByPlayer && npDat.Cash > 0)
                    {
                        Vector3 dp = np.Position;
                        Function.Call<int>((GTA.Native.Hash)0x673966A0C0FD7171,
                            Function.Call<int>(Hash.GET_HASH_KEY, "PICKUP_MONEY_VARIABLE"),
                            dp.X, dp.Y, dp.Z + 0.1f,
                            8, npDat.Cash,
                            Function.Call<int>(Hash.GET_HASH_KEY, "prop_cash_pile_01"),
                            true, false);
                        npDat.Cash = 0;
                        SaveALife();
                    }

                    // Queue: fate resolved next tick (gives player 1s window to hit corpse)
                    _pendingDeadPeds[npKey] = new PendingDeadPed { Ped = np, PlayerKilled = killedByPlayer, UnarmedKill = killedUnarmed, FirstSeenAt = Game.GameTime };
                    _resolvedDownedKeys.Add(npKey); // prevent re-queuing in Phase B
                    break; // one per tick
                }
            }

            // ── Controller detection (updates every frame) ────────────────
            _usingController = Game.LastInputMethod == InputMethod.GamePad;

            // ── Block game controls when menu is open OR animating on controller ──
            // DISABLE_ALL_CONTROL_ACTIONS must come BEFORE HandleControllerMenuInput so the
            // handler can use IS_DISABLED_CONTROL_JUST_PRESSED to still read our inputs.
            // Also needed during animation so Frontend* controls (FrontendLb etc.) register.
            if (_usingController && (menuLevel != MenuLevel.None || state == ScriptState.Animating || state == ScriptState.Approaching))
            {
                Function.Call(Hash.DISABLE_ALL_CONTROL_ACTIONS, 0);
                // Re-enable analogue sticks — only D-pad/buttons need blocking
                Function.Call(Hash.ENABLE_CONTROL_ACTION, 0, 30, true);  // MoveLeftRight  (left stick X)
                Function.Call(Hash.ENABLE_CONTROL_ACTION, 0, 31, true);  // MoveUpDown     (left stick Y)
                Function.Call(Hash.ENABLE_CONTROL_ACTION, 0, 1,  true);  // LookLeftRight  (right stick X)
                Function.Call(Hash.ENABLE_CONTROL_ACTION, 0, 2,  true);  // LookUpDown     (right stick Y)
                Function.Call(Hash.ENABLE_CONTROL_ACTION, 0, 59, true);  // VehicleMoveLeftRight
                Function.Call(Hash.ENABLE_CONTROL_ACTION, 0, 71, true);  // VehicleAccelerate
                Function.Call(Hash.ENABLE_CONTROL_ACTION, 0, 72, true);  // VehicleBrake
            }

            // ── Controller menu navigation (mirrors OnKeyDown for gamepads) ──
            if (_usingController) HandleControllerMenuInput();

            // ── GTA on-screen keyboard polling (token input) ───────────────
            if (tokenKeyboardOpen)
            {
                int kbStatus = Function.Call<int>(Hash.UPDATE_ONSCREEN_KEYBOARD);
                if (kbStatus == 1) // confirmed
                {
                    string result = Function.Call<string>(Hash.GET_ONSCREEN_KEYBOARD_RESULT);
                    if (result != null)
                    {
                        deviceToken = result;
                        _telemetry.SetToken(deviceToken);
                        SaveSettings();
                        ShowHudStatus("~g~TOKEN 已成功儲存！", 2000);
                    }
                    tokenKeyboardOpen = false;
                }
                else if (kbStatus == 2) // cancelled
                {
                    tokenKeyboardOpen = false;
                }
                // kbStatus == 0 means still open — do nothing
            }

            Ped player = Game.Player.Character;

            // ── Pending cash drop monitor (survives FullReset — watches current session girl directly) ────
            // Only used when the current girl was attacked mid-session (hit reaction queues her here).
            // Waits for IsDead, then applies unarmed 80/20 fate and spawns cash pickup.
            if (_pendingCashDropPed != null)
            {
                if (!_pendingCashDropPed.Exists())
                {
                    // Ped despawned — clear silently
                    _pendingCashDropPed    = null;
                    _pendingCashDropAmount = 0;
                    _pendingCashDropKey    = null;
                    _pendingCashDropWeaponHash = 0;
                }
                else if (_pendingCashDropPed.IsDead)
                {
                    // Spawn cash pickup
                    if (_pendingCashDropAmount > 0)
                    {
                        Vector3 dp = _pendingCashDropPed.Position;
                        Function.Call<int>((GTA.Native.Hash)0x673966A0C0FD7171,
                            Function.Call<int>(Hash.GET_HASH_KEY, "PICKUP_MONEY_VARIABLE"),
                            dp.X, dp.Y, dp.Z + 0.1f,
                            8, _pendingCashDropAmount,
                            Function.Call<int>(Hash.GET_HASH_KEY, "prop_cash_pile_01"),
                            true, false);
                    }

                    // Transfer into _pendingDeadPeds so she gets the same corpse-hit
                    // detection and 80/20 fate logic as any other dead A-Life ped.
                    if (_pendingCashDropKey != null && !_pendingDeadPeds.ContainsKey(_pendingCashDropKey))
                    {
                        int unarmedHashMon = Function.Call<int>(Hash.GET_HASH_KEY, "WEAPON_UNARMED");
                        bool monUnarmed    = (_pendingCashDropWeaponHash == unarmedHashMon);
                        // Clear now so the per-frame scanner only catches genuinely post-death hits
                        Function.Call(Hash.CLEAR_ENTITY_LAST_DAMAGE_ENTITY, _pendingCashDropPed);
                        _pendingDeadPeds[_pendingCashDropKey] = new PendingDeadPed
                        {
                            Ped          = _pendingCashDropPed,
                            PlayerKilled = true,
                            UnarmedKill  = monUnarmed,
                            FirstSeenAt  = Game.GameTime
                        };
                        _resolvedDownedKeys.Add(_pendingCashDropKey);
                    }

                    _pendingCashDropPed        = null;
                    _pendingCashDropAmount     = 0;
                    _pendingCashDropKey        = null;
                    _pendingCashDropWeaponHash = 0;
                }
            }

            // ── Per-frame corpse hit detection ──────────────────────────────────────────
            // Iterates _pendingDeadPeds directly using known Ped references (no GetNearbyPeds needed).
            // Survived peds remain in the dict permanently so stomp detection works even after
            // Phase A has already run its 80% survive path.
            if (aLifeMode && _pendingDeadPeds.Count > 0)
            {
                // Attack + proximity: catches unarmed punches on corpses (GTA never fires
                // HasBeenDamagedBy on dead peds when there is no health left to reduce).
                bool attackJustPressed = Function.Call<bool>(Hash.IS_CONTROL_JUST_PRESSED, 0, 24)
                                      || Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 0, 24);
                if (attackJustPressed)
                {
                    string stompKey = null;
                    foreach (var kvp in _pendingDeadPeds)
                    {
                        PendingDeadPed pd = kvp.Value;
                        if (pd.Ped == null || !pd.Ped.Exists()) continue;
                        if (pd.Ped.Position.DistanceTo(player.Position) > 2.5f) continue;
                        stompKey = kvp.Key;
                        break;
                    }
                    if (stompKey != null)
                    {
                        if (_aLifePeds.ContainsKey(stompKey))
                        {
                            _aLifePeds.Remove(stompKey);
                            SaveALife();
                        }
                        _pendingDeadPeds.Remove(stompKey);
                        _resolvedDownedKeys.Add(stompKey);
                    }
                }

                // HasBeenDamagedBy fallback: weapon/shot hits on corpses — sets CorpseHit for Phase A
                foreach (var kvp in _pendingDeadPeds)
                {
                    PendingDeadPed pd = kvp.Value;
                    if (pd.CorpseHit) continue;
                    if (pd.Ped == null || !pd.Ped.Exists()) continue;
                    if (pd.Ped.HasBeenDamagedBy(player))
                    {
                        pd.CorpseHit = true;
                        Function.Call(Hash.CLEAR_ENTITY_LAST_DAMAGE_ENTITY, pd.Ped);
                    }
                }
            }

            // ── Hit reaction: runs in ALL states (Idle + Animating) ───────────────────────
            if (hasGirl && girl != null && girl.Exists() && girl.IsAlive)
            {
                // Dual detection: flag-based (catches most hits) + vitality delta (catches armor-absorbed hits)
                float vitality = girl.Health + (float)Function.Call<int>(Hash.GET_PED_ARMOUR, girl);
                if (_girlLastHealth < 0f) _girlLastHealth = vitality;

                bool hitDetected = girl.HasBeenDamagedBy(player)
                                || (vitality < _girlLastHealth - 1f);

                if (hitDetected)
                {
                    Function.Call(Hash.CLEAR_ENTITY_LAST_DAMAGE_ENTITY, girl);
                    Function.Call((Hash)0x726256CC1EEB182F, girl); // CLEAR_FACIAL_IDLE_ANIM_OVERRIDE — remove override so death face plays naturally
                    _girlLastHealth = -1f;
                    ALifePedData dHit = null;
                    if (aLifeMode && _currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out dHit);
                    if (dHit != null)
                    {
                        dHit.Reputation = Math.Max(REP_MIN, dHit.Reputation - 10);
                        UpdateRelationshipState(dHit);
                        SaveALife();
                        CheckAndReleaseIfAvoiding(dHit);
                    }
                    menuLevel = MenuLevel.None;
                    if (state == ScriptState.Animating) EndAnimation();

                    _pendingCashDropWeaponHash = Function.Call<int>(Hash.GET_SELECTED_PED_WEAPON, player);

                    // Queue cash drop BEFORE FullReset clears _currentGirlKey / girl.
                    // CLEAR_ENTITY_LAST_DAMAGE_ENTITY above means the dead-ped sweep
                    // won't see HasBeenDamagedBy = true, so it won't drop her cash.
                    // Watching her directly via the pending monitor bypasses that entirely.
                    if (aLifeMode && _pendingCashDropPed == null && dHit != null && dHit.Cash > 0)
                    {
                        _pendingCashDropPed    = girl;
                        _pendingCashDropKey    = _currentGirlKey;
                        _pendingCashDropAmount = dHit.Cash;
                        dHit.Cash = 0;
                        SaveALife();
                    }

                    TriggerHostileReaction(girl, dHit);
                    FullReset(true);
                }
                else
                {
                    _girlLastHealth = vitality;
                }
            }

            // ── Periodic voice reapplication (fights GTA resetting ambient voice) ──────
            if (hasGirl && girl != null && girl.Exists() && _girlCurrentVoice != null
                && Game.GameTime >= _voiceReapplyAt)
            {
                Function.Call(Hash.SET_AMBIENT_VOICE_NAME, girl, _girlCurrentVoice);
                _voiceReapplyAt = Game.GameTime + 3000;
            }
            // ── Periodic facial expression reapplication (GTA can reset idle face anims) ──
            if (aLifeMode && Game.GameTime >= _faceReapplyAt)
            {
                // Pre-emptive clear: if a tracked ped is alive but critically low health,
                // clear the override NOW before GTA can freeze it at death.
                if (_approachCandidate != null && _approachCandidate.Exists() && !_approachCandidate.IsDead)
                {
                    float acVit = _approachCandidate.Health + (float)Function.Call<int>(Hash.GET_PED_ARMOUR, _approachCandidate);
                    if (acVit < 80f)
                        Function.Call((Hash)0x726256CC1EEB182F, _approachCandidate); // CLEAR_FACIAL_IDLE_ANIM_OVERRIDE
                }
                if (hasGirl && girl != null && girl.Exists() && !girl.IsDead)
                {
                    float gVit = girl.Health + (float)Function.Call<int>(Hash.GET_PED_ARMOUR, girl);
                    if (gVit < 80f)
                        Function.Call((Hash)0x726256CC1EEB182F, girl); // CLEAR_FACIAL_IDLE_ANIM_OVERRIDE
                }

                Ped faceTarget = null; string faceMood = null;
                if (_approachCandidate != null && _approachCandidate.Exists()
                    && !_approachCandidate.IsDead
                    && _approachCandidate.Health + (float)Function.Call<int>(Hash.GET_PED_ARMOUR, _approachCandidate) >= 80f)
                {
                    string fFp = GetPedFingerprint(_approachCandidate);
                    ALifePedData fDat;
                    if (fFp != null && _aLifePeds.TryGetValue(ALifeKey(fFp), out fDat))
                    { faceTarget = _approachCandidate; faceMood = fDat.Mood; }
                }
                else if (hasGirl && girl != null && girl.Exists()
                    && !girl.IsDead
                    && girl.Health + (float)Function.Call<int>(Hash.GET_PED_ARMOUR, girl) >= 80f)
                {
                    string fFp = GetPedFingerprint(girl);
                    ALifePedData fDat;
                    if (fFp != null && _aLifePeds.TryGetValue(ALifeKey(fFp), out fDat))
                    { faceTarget = girl; faceMood = fDat.Mood; }
                }
                if (faceTarget != null && faceMood != null)
                    ApplyMoodFacialExpression(faceTarget, faceMood);
                _faceReapplyAt = Game.GameTime + 100;
            }
            // ─────────────────────────────────────────────────────────────────────

            switch (state)
            {
                case ScriptState.Idle:
                    HandleIdleState(player);
                    break;

                case ScriptState.Approaching:
                    HandleApproachingState(player);
                    break;

                case ScriptState.Animating:
                    HandleAnimatingState(player);
                    break;
            }

            // Draw menu overlay
            if (menuLevel != MenuLevel.None) DrawMenu();

            // Draw HUD status notification (non-dialogue messages — replaces subtitles for system info)
            if (_hudStatusText != null)
            {
                if (Game.GameTime < _hudStatusExpireAt)
                    DrawTextOnScreen(_hudStatusText, 0.5f, 0.895f, 0.45f, true);
                else
                    _hudStatusText = null;
            }
        }

        // ══════════
        // IDLE STATE
        // ══════════
        private void HandleIdleState(Ped player)
        {
            // ── Girl follow + auto vehicle enter/exit ──
            if (hasGirl && girl != null && girl.Exists())
            {
                if (!girl.IsAlive)
                {
                    // ShowSubtitle("~r~WOMAN DIED!", 2000); - For Debug Purposes Only
                    _girlLastHealth = -1f;
                    FullReset();
                    return;
                }

                // Hypnosis Mode: Set relationship group & combat attributes
                if (!aLifeMode)
                {
                    if (girl.RelationshipGroup != Game.Player.Character.RelationshipGroup)
                        girl.RelationshipGroup = Game.Player.Character.RelationshipGroup;
                    
                    // Allow her to fight if armed, but do not make her totally fearless
                    // 46 = AlwaysFight (she will fight enemies instead of fleeing instantly, if armed)
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, girl, 46, true);
                    Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, girl, 5, false); // Don't fight armed peds when unarmed
                }

                // Prostitution mode: if she's too far away, she leaves
                if (!sandboxMode)
                {
                    float girlDist = girl.Position.DistanceTo(player.Position);
                    if (girlDist > 20f)
                    {
                        if (aLifeMode && _currentGirlKey != null)
                        {
                            ALifePedData dCasWalk;
                            if (_aLifePeds.TryGetValue(_currentGirlKey, out dCasWalk) && dCasWalk.ALifeMode == "Normal")
                            {
                                dCasWalk.Reputation = Math.Max(-1, dCasWalk.Reputation - 3);
                                UpdateRelationshipState(dCasWalk);
                                SaveALife();
                            }
                        }
                        FullReset();
                        return;
                    }
                }

                bool playerInVeh = player.IsInVehicle();
                bool girlInVeh = girl.IsInVehicle();

                // Player just got into a car → tell girl to enter
                if (playerInVeh && !playerWasInVehicle && !_backseatEntryPending)
                {
                    if (menuLevel == MenuLevel.Services || IsInServiceSubMenu()) { menuLevel = MenuLevel.Main; menuIndex = 0; }
                    if (menuLevel == MenuLevel.Conversation || menuLevel == MenuLevel.Intimacy) { menuLevel = MenuLevel.Actions; menuIndex = 0; }
                    Vehicle car = player.CurrentVehicle;
                    VehicleSeat followSeat = IsPlayerInBackSeat(car) ? VehicleSeat.LeftRear : VehicleSeat.Passenger;
                    if (car != null && IsSuitableVehicle(car))
                    {
                        if (!IsALifeVehicleEntryGated())
                            TaskGirlEnterVehicle(car, followSeat);
                        else
                        {
                            // Gate blocked entry — cancel the active follow task so she doesn't
                            // keep walking toward the car and slip in via pathfinding.
                            girl.Task.ClearAll();
                            girl.BlockPermanentEvents = true;
                        }
                    }
                }

                // Continuous check & stuck protection when player is in vehicle and girl should be entering
                if (playerInVeh && !girlInVeh && !_backseatEntryPending && !IsALifeVehicleEntryGated())
                {
                    Vehicle car = player.CurrentVehicle;
                    if (car != null && IsSuitableVehicle(car))
                    {
                        bool isGettingIn = Function.Call<bool>(Hash.IS_PED_GETTING_INTO_A_VEHICLE, girl);
                        if (isGettingIn)
                        {
                            // Stuck timeout: if she's been trying to get into vehicle for over 16s (e.g. door blocked/pathfinding loop), warp her inside
                            if (_vehEntryStartTime > 0 && (Game.GameTime - _vehEntryStartTime > 16000))
                            {
                                VehicleSeat warpSeat = GetBestSeatForGirl(car, IsPlayerInBackSeat(car) ? VehicleSeat.LeftRear : VehicleSeat.Passenger);
                                Function.Call(Hash.SET_PED_INTO_VEHICLE, girl, car, (int)warpSeat);
                                _vehEntryStartTime = 0;
                            }
                        }
                        else
                        {
                            // Not in vehicle and not getting into vehicle — re-task entry every 3 seconds
                            if (Game.GameTime - _lastVehEnterTaskTime > 3000)
                            {
                                _lastVehEnterTaskTime = Game.GameTime;
                                VehicleSeat followSeat = IsPlayerInBackSeat(car) ? VehicleSeat.LeftRear : VehicleSeat.Passenger;
                                TaskGirlEnterVehicle(car, followSeat);
                            }
                        }
                    }
                }
                else
                {
                    _vehEntryStartTime = 0;
                }

                // Clear backseat-entry flag once both are seated
                if (_backseatEntryPending && playerInVeh && girlInVeh)
                    _backseatEntryPending = false;

                // Player just exited a car → tell girl to exit too
                if (!playerInVeh && playerWasInVehicle)
                {
                    if (menuLevel == MenuLevel.Services || IsInServiceSubMenu()) { menuLevel = MenuLevel.Main; menuIndex = 0; }
                    if (menuLevel == MenuLevel.Conversation || menuLevel == MenuLevel.Intimacy) { menuLevel = MenuLevel.Actions; menuIndex = 0; }
                    if (girlInVeh)
                        girl.Task.LeaveVehicle();
                    _vehEntryStartTime = 0;
                }

                // On foot: follow player (skip when backseat entry is in progress)
                if (!playerInVeh && !girlInVeh && !_backseatEntryPending)
                {
                    girl.BlockPermanentEvents = true;
                    float dist = girl.Position.DistanceTo(player.Position);

                    if (dist > 50f)
                    {
                        FullReset();
                        return;
                    }
                    else if (dist > 2f)
                    {
                        bool isIdle = !Function.Call<bool>(Hash.IS_PED_WALKING, girl) &&
                                      !Function.Call<bool>(Hash.IS_PED_RUNNING, girl);
                        if (isIdle)
                        {
                            float speed = (dist > 7f) ? 3f : 1f;
                            Function.Call(Hash.TASK_FOLLOW_TO_OFFSET_OF_ENTITY,
                                girl, player, 0f, -1f, 0f, speed, -1, 1.5f, true);
                        }
                    }
                    else
                    {
                        bool isMoving = Function.Call<bool>(Hash.IS_PED_WALKING, girl) ||
                                        Function.Call<bool>(Hash.IS_PED_RUNNING, girl);
                        if (isMoving)
                            girl.Task.TurnTo(player);
                    }
                }

                // Context check: if services/conversation/intimacy menu is open but player/girl context changed, force back
                if (menuLevel == MenuLevel.Services || IsInServiceSubMenu() || menuLevel == MenuLevel.Conversation || menuLevel == MenuLevel.Intimacy)
                {
                    bool pInV = player.IsInVehicle();
                    bool gInV = girl.IsInVehicle();
                    bool validCar = pInV && gInV && girl.CurrentVehicle == player.CurrentVehicle;
                    bool validStreet = !pInV && !gInV;
                    if (!validCar && !validStreet)
                    {
                        if (menuLevel == MenuLevel.Services || IsInServiceSubMenu())
                        { menuLevel = MenuLevel.Main; menuIndex = 0; }
                        else
                        { menuLevel = MenuLevel.Actions; menuIndex = 0; }
                        ShowHudStatus("~y~正在等待她……", 2000);
                    }
                }

                playerWasInVehicle = playerInVeh;

                // ── Car proposition pose management ──
                if (carPropActive)
                {
                    // Suppress engine every tick while proposition is active (GTA re-enables it automatically)
                    if (!carEngineEnabled && player.IsInVehicle() && player.CurrentVehicle != null)
                        player.CurrentVehicle.IsEngineRunning = false;

                    // Phase 0 → 1: Enter anim done → start loop
                    if (carPropPhase == 0 && Game.GameTime >= carPropWaitTime)
                    {
                        bool inBackProp = player.IsInVehicle() && player.CurrentVehicle != null
                            && IsPlayerInBackSeat(player.CurrentVehicle);
                        if (inBackProp)
                        {
                            const string DRUNK_DICT = "random@drunk_driver_2";
                            RequestAnimDict(DRUNK_DICT);
                            Function.Call(Hash.TASK_PLAY_ANIM, girl, DRUNK_DICT, "cardrunkflirt_loop_f",
                                8.0f, -4.0f, -1, 1, 0.0f, false, false, false);
                            Function.Call(Hash.TASK_PLAY_ANIM, player, DRUNK_DICT, "cardrunkflirt_loop_m",
                                8.0f, -4.0f, -1, driveableCar ? 33 : 1, 0.0f, false, false, false);
                        }
                        else
                        {
                            int cm = Function.Call<int>(Hash.GET_FOLLOW_PED_CAM_VIEW_MODE);
                            bool fp = (cm == 4);
                            string pd;
                            if (vehicleAnimType == VehicleAnimType.Low)
                                pd = fp ? VEH_LOW_FP_DICT : VEH_LOW_DICT;
                            else
                                pd = fp ? VEH_NORM_FP_DICT : VEH_NORM_DICT;

                            string fLoop, mLoop;
                            if (vehicleAnimType == VehicleAnimType.Low)
                            { fLoop = "low_car_prop_loop_female"; mLoop = "low_car_prop_loop_player"; }
                            else
                            { fLoop = "proposition_loop_prostitute"; mLoop = "proposition_loop_male"; }

                            Function.Call(Hash.TASK_PLAY_ANIM, girl, pd, fLoop,
                                8.0f, -4.0f, -1, 1, 0.0f, false, false, false);
                            Function.Call(Hash.TASK_PLAY_ANIM, player, pd, mLoop,
                                8.0f, -4.0f, -1, driveableCar ? 33 : 1, 0.0f, false, false, false);
                        }
                        carPropPhase = 1;
                        // Open any menu that was waiting for the loop to be ready
                        if (_carPropPendingMenu != MenuLevel.None)
                        {
                            menuLevel = _carPropPendingMenu;
                            menuIndex = 0;
                            _carPropPendingMenu = MenuLevel.None;
                        }
                    }
                    // Phase 2: Exit anim — enforce reverse speed for backseat each tick
                    else if (carPropPhase == 2)
                    {
                        bool inBackExit = player.IsInVehicle() && player.CurrentVehicle != null
                            && IsPlayerInBackSeat(player.CurrentVehicle);
                        if (inBackExit)
                        {
                            const string DRUNK_DICT = "random@drunk_driver_2";
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, DRUNK_DICT, "cardrunkflirt_intro_m", -1.0f);
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl,   DRUNK_DICT, "cardrunkflirt_intro_f", -1.0f);
                        }
                        if (Game.GameTime >= carPropWaitTime)
                        {
                            carPropActive = false;
                            carPropPhase = 0;
                            if (player.IsInVehicle() && player.CurrentVehicle != null)
                            {
                                player.CurrentVehicle.IsInteriorLightOn = false;
                            }
                            // Reopen the menu we were in before the exit anim started
                            if (_carPropExitReturnMenu != MenuLevel.None)
                            {
                                menuLevel = _carPropExitReturnMenu;
                                menuIndex = _carPropExitReturnIdx;
                                _carPropExitReturnMenu = MenuLevel.None;
                                _carPropExitReturnIdx  = 0;
                            }
                        }
                    }

                    // Backup: proposition active but not in services or escalate menu → cleanly exit
                    // Skip when a pending menu open is in progress (menuLevel == None while entering prop)
                    // Skip while post-sex voice sequence is running (menu is intentionally deferred)
                    if (carPropActive && (carPropPhase == 0 || carPropPhase == 1)
                        && menuLevel != MenuLevel.Services && menuLevel != MenuLevel.Escalate
                        && !IsInServiceSubMenu()
                        && _carPropPendingMenu == MenuLevel.None
                        && _postSexPhase == 0)
                    {
                        ExitCarProposition(player);
                    }
                }

                // ── Street sitting proposition pose management ──
                if (_streetSitPropActive)
                {
                    // Phase -1 → 0: Kick-off → play sit-down
                    if (_streetSitPropPhase == -1)
                    {
                        string mDict, mAnim, fDict, fAnim;
                        Vector3 offset; float heading; bool isSitting; float zOffset; float rotX; float rotHorizontal;
                        GetStreetPropAnimData(0, out mDict, out mAnim, out fDict, out fAnim, out offset, out heading, out isSitting, out zOffset, out rotX, out rotHorizontal);

                        RequestAnimDict(mDict);
                        if (mDict != fDict) RequestAnimDict(fDict);

                        Vector3 girlPos = player.GetOffsetPosition(offset);
                        Vector3 girlFinal = new Vector3(girlPos.X, girlPos.Y, girlPos.Z + zOffset);

                        // Force-stop everything: clear tasks, kill velocity, freeze, then teleport & animate
                        girl.Task.ClearAll();
                        Function.Call(Hash.SET_PED_KEEP_TASK, girl, true);
                        Function.Call(Hash.SET_ENTITY_VELOCITY, girl, 0f, 0f, 0f);
                        girl.IsPositionFrozen = true;

                        Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                            girlFinal.X, girlFinal.Y, girlFinal.Z, false, false, false);
                        girl.Heading = player.Heading + heading;

                        girl.IsPositionFrozen = false; // unfreeze so the anim can move her

                        if (rotX != 0f || rotHorizontal != 0f)
                        {
                            Vector3 pp = player.Position;
                            Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, player, mDict, mAnim,
                                pp.X, pp.Y, pp.Z, rotX, 0f, player.Heading + rotHorizontal,
                                8.0f, -4.0f, -1, 2, 0f, 2, 0);
                        }
                        else
                        {
                            Function.Call(Hash.TASK_PLAY_ANIM, player, mDict, mAnim,
                                8.0f, -4.0f, -1, 2, 0f, false, false, false);
                        }
                        Function.Call(Hash.TASK_PLAY_ANIM, girl, fDict, fAnim,
                            8.0f, -4.0f, -1, 2, 0f, false, false, false);

                        float sitDur = Function.Call<float>(Hash.GET_ANIM_DURATION, fDict, fAnim);
                        _streetSitPropPhase = 0;
                        _streetSitPropWaitTime = Game.GameTime + Math.Max((int)(sitDur * 1000f), 1500);
                    }
                    // Phase 0 (waiting for sit-down): if girl lost her anim, force-replay it
                    else if (_streetSitPropPhase == 0 && Game.GameTime < _streetSitPropWaitTime)
                    {
                        string mDictChk, mAnimChk, fDictChk, fAnimChk;
                        Vector3 offChk; float hdgChk; bool isSitChk; float zOffChk; float rotXChk; float rotHChk;
                        GetStreetPropAnimData(0, out mDictChk, out mAnimChk, out fDictChk, out fAnimChk, out offChk, out hdgChk, out isSitChk, out zOffChk, out rotXChk, out rotHChk);

                        if (!Function.Call<bool>(Hash.IS_ENTITY_PLAYING_ANIM, girl, fDictChk, fAnimChk, 3))
                        {
                            // She lost the anim — force her back
                            girl.Task.ClearAll();
                            Function.Call(Hash.SET_PED_KEEP_TASK, girl, true);
                            Function.Call(Hash.SET_ENTITY_VELOCITY, girl, 0f, 0f, 0f);
                            Vector3 girlRetry = player.GetOffsetPosition(offChk);
                            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                                girlRetry.X, girlRetry.Y, girlRetry.Z + zOffChk, false, false, false);
                            girl.Heading = player.Heading + hdgChk;
                            Function.Call(Hash.TASK_PLAY_ANIM, girl, fDictChk, fAnimChk,
                                8.0f, -4.0f, -1, 2, 0f, false, false, false);
                            // Reset timer so she gets the full duration
                            float retryDur = Function.Call<float>(Hash.GET_ANIM_DURATION, fDictChk, fAnimChk);
                            _streetSitPropWaitTime = Game.GameTime + Math.Max((int)(retryDur * 1000f), 1500);
                        }
                    }
                    // Phase 0 → 1: Sit-down done → play proposition enter
                    else if (_streetSitPropPhase == 0 && Game.GameTime >= _streetSitPropWaitTime)
                    {
                        string mDict, mAnim, fDict, fAnim;
                        Vector3 offset; float heading; bool isSitting; float zOffset; float rotX; float rotHorizontal;
                        GetStreetPropAnimData(1, out mDict, out mAnim, out fDict, out fAnim, out offset, out heading, out isSitting, out zOffset, out rotX, out rotHorizontal);

                        RequestAnimDict(mDict);
                        if (mDict != fDict) RequestAnimDict(fDict);

                        // Disable collision and sink both peds so the car-seat anim looks grounded
                        if (isSitting)
                        {
                            Function.Call(Hash.SET_ENTITY_COLLISION, player, false, false);
                            Function.Call(Hash.SET_ENTITY_COLLISION, girl, false, false);
                        }

                        Vector3 girlPos = player.GetOffsetPosition(offset);
                        Vector3 playerSunk = new Vector3(player.Position.X, player.Position.Y, player.Position.Z + zOffset);
                        Vector3 girlFinal = new Vector3(girlPos.X, girlPos.Y, girlPos.Z + zOffset);
                        Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, player,
                            playerSunk.X, playerSunk.Y, playerSunk.Z, false, false, false);
                        Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                            girlFinal.X, girlFinal.Y, girlFinal.Z, false, false, false);
                        girl.Heading = player.Heading + heading;

                        player.IsPositionFrozen = true;
                        girl.IsPositionFrozen = true;

                        if (rotX != 0f || rotHorizontal != 0f)
                        {
                            Vector3 pp = player.Position;
                            Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, player, mDict, mAnim,
                                pp.X, pp.Y, pp.Z, rotX, 0f, player.Heading + rotHorizontal,
                                8.0f, -4.0f, -1, 2, 0f, 2, 0);
                        }
                        else
                        {
                            Function.Call(Hash.TASK_PLAY_ANIM, player, mDict, mAnim,
                                8.0f, -4.0f, -1, 2, 0f, false, false, false);
                        }
                        Function.Call(Hash.TASK_PLAY_ANIM, girl, fDict, fAnim,
                            8.0f, -4.0f, -1, 2, 0f, false, false, false);

                        PlayServiceVoice(); // proposition enter anim starts — voice fires here for Sitting/Laying
                        float spEnterDur = Function.Call<float>(Hash.GET_ANIM_DURATION, fDict, fAnim);
                        _streetSitPropPhase = 1;
                        _streetSitPropWaitTime = Game.GameTime + Math.Max((int)(spEnterDur * 1000f), 1500);
                    }
                    // Phase 1 → 2: Proposition enter done → start loop
                    else if (_streetSitPropPhase == 1 && Game.GameTime >= _streetSitPropWaitTime)
                    {
                        string mDict, mAnim, fDict, fAnim;
                        Vector3 offset; float heading; bool isSitting; float zOffset; float rotX; float rotHorizontal;
                        GetStreetPropAnimData(2, out mDict, out mAnim, out fDict, out fAnim, out offset, out heading, out isSitting, out zOffset, out rotX, out rotHorizontal);

                        RequestAnimDict(mDict);
                        if (mDict != fDict) RequestAnimDict(fDict);

                        // Both peds already sunk & frozen from phase 0→1 — just play new anims
                        if (rotX != 0f || rotHorizontal != 0f)
                        {
                            Vector3 pp = player.Position;
                            Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, player, mDict, mAnim,
                                pp.X, pp.Y, pp.Z, rotX, 0f, player.Heading + rotHorizontal,
                                8.0f, -4.0f, -1, 1, 0f, 2, 0);
                        }
                        else
                        {
                            Function.Call(Hash.TASK_PLAY_ANIM, player, mDict, mAnim,
                                8.0f, -4.0f, -1, 1, 0f, false, false, false);
                        }
                        Function.Call(Hash.TASK_PLAY_ANIM, girl, fDict, fAnim,
                            8.0f, -4.0f, -1, 1, 0f, false, false, false);

                        _streetSitPropPhase = 2;

                        // Transition complete — show the service list menu
                        // If we came from Services or Escalate > Sit Down, open the seated sub-menu hub first
                        if (_serviceSubParent == MenuLevel.Services || _serviceSubParent == MenuLevel.Escalate)
                        {
                            menuLevel = MenuLevel.SitDownSub;
                            menuIndex = 0;
                        }
                        else
                        {
                            menuLevel = _streetSitCategory;
                            menuIndex = 0;
                        }
                    }
                    // Phase 3 → 4: Proposition exit done → play stand-up (or skip if anim disabled)
                    else if (_streetSitPropPhase == 3 && Game.GameTime >= _streetSitPropWaitTime)
                    {
                        if (!streetSitAnimEnabled)
                        {
                            // Skip picnic stand-up: immediately restore positions and trigger cleanup
                            player.Task.ClearAll();
                            if (girl != null && girl.Exists()) girl.Task.ClearAll();
                            player.IsPositionFrozen = false;
                            girl.IsPositionFrozen = false;
                            Function.Call(Hash.SET_ENTITY_COLLISION, player, true, true);
                            Function.Call(Hash.SET_ENTITY_COLLISION, girl, true, true);
                            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, player,
                                _streetSitOriginPos.X, _streetSitOriginPos.Y, _streetSitOriginPos.Z, false, false, false);
                            player.Heading = _streetSitOriginHeading;
                            _streetSitPropPhase = 4;
                            _streetSitPropWaitTime = 0;
                        }
                        else
                        {
                        string mDict, mAnim, fDict, fAnim;
                        Vector3 offset; float heading; bool isSitting; float zOffset; float rotX; float rotHorizontal;
                        GetStreetPropAnimData(4, out mDict, out mAnim, out fDict, out fAnim, out offset, out heading, out isSitting, out zOffset, out rotX, out rotHorizontal);

                        RequestAnimDict(mDict);
                        if (mDict != fDict) RequestAnimDict(fDict);

                        // Unsink: restore positions and re-enable collision for stand-up
                        player.IsPositionFrozen = false;
                        girl.IsPositionFrozen = false;
                        Function.Call(Hash.SET_ENTITY_COLLISION, player, true, true);
                        Function.Call(Hash.SET_ENTITY_COLLISION, girl, true, true);
                        Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, player,
                            _streetSitOriginPos.X, _streetSitOriginPos.Y, _streetSitOriginPos.Z, false, false, false);
                        player.Heading = _streetSitOriginHeading;

                        Vector3 girlPos = player.GetOffsetPosition(offset);
                        Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                            girlPos.X, girlPos.Y, girlPos.Z, false, false, false);
                        girl.Heading = player.Heading + heading;

                        if (rotX != 0f || rotHorizontal != 0f)
                        {
                            Vector3 pp = player.Position;
                            Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, player, mDict, mAnim,
                                pp.X, pp.Y, pp.Z, rotX, 0f, player.Heading + rotHorizontal,
                                8.0f, -4.0f, -1, 0, 0f, 2, 0);
                        }
                        else
                        {
                            Function.Call(Hash.TASK_PLAY_ANIM, player, mDict, mAnim,
                                8.0f, -4.0f, -1, 0, 0f, false, false, false);
                        }
                        Function.Call(Hash.TASK_PLAY_ANIM, girl, fDict, fAnim,
                            8.0f, -4.0f, -1, 0, 0f, false, false, false);

                        float spExitDur = Function.Call<float>(Hash.GET_ANIM_DURATION, fDict, fAnim);
                        _streetSitPropPhase = 4;
                        _streetSitPropWaitTime = Game.GameTime + (int)(spExitDur * 1000f);
                        } // end else (streetSitAnimEnabled)
                    }
                    // Phase 4 → done: Stand-up done → cleanup
                    else if (_streetSitPropPhase == 4 && Game.GameTime >= _streetSitPropWaitTime)
                    {
                        // Safety: ensure collision and freeze are fully restored
                        player.IsPositionFrozen = false;
                        girl.IsPositionFrozen = false;
                        Function.Call(Hash.SET_ENTITY_COLLISION, player, true, true);
                        Function.Call(Hash.SET_ENTITY_COLLISION, girl, true, true);
                        _streetSitPropActive = false;
                        _streetSitPropPhase = 0;
                        if (_streetSitExitReturnMenu != MenuLevel.None)
                        {
                            menuLevel = _streetSitExitReturnMenu;
                            menuIndex = _streetSitExitReturnIdx;
                            _streetSitExitReturnMenu = MenuLevel.None;
                            _streetSitExitReturnIdx  = 0;
                        }
                    }

                    // Backup: street prop loop active but not in Sitting/Laying sub-menu → trigger exit
                    // Only applies during phase 2 (loop); phases -1/0/1 have the menu intentionally closed
                    // Skip while post-sex voice sequence is running (menu is intentionally deferred)
                    if (_streetSitPropActive && _streetSitPropPhase == 2
                        && state != ScriptState.Animating
                        && _postSexPhase == 0
                        && menuLevel != MenuLevel.SittingServices
                        && menuLevel != MenuLevel.LayingServices
                        && menuLevel != MenuLevel.SitDownSub)
                    {
                        ExitStreetSitProposition(player);
                        menuLevel = MenuLevel.None; // lock menu during stand-up
                    }
                }
            }

            // ── Master key: open/close menu ──
            bool masterTriggered = _usingController
                ? (Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 0, masterControlModifier) && Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 0, masterControl))
                : Game.IsKeyPressed(masterKey);
            if (masterTriggered && keyCooldown == 0)
            {
                keyCooldown = KEY_COOLDOWN_FRAMES;
                if (menuLevel != MenuLevel.None)
                {
                    // Close the menu — also trigger street sit exit if applicable
                    if (_streetSitPropActive && _streetSitPropPhase <= 2 && state != ScriptState.Animating)
                        ExitStreetSitProposition(Game.Player.Character);
                    menuLevel = MenuLevel.None;
                }
                else if (_streetSitPropActive && (_streetSitPropPhase < 2 || _streetSitPropPhase >= 3))
                {
                    // Block menu opening during sit-down/stand-up transitions
                }
                else if (_postSexPhase > 0)
                {
                    // Block menu opening while SEX_FINISHED voice is playing / 1s delay before HOOKER_OFFER_AGAIN
                }
                else
                {
                    menuLevel = MenuLevel.Main;
                    menuIndex = 0;
                }
            }

            if (menuLevel != MenuLevel.None) return;

            // ── Prostitution A-Life: auto-approach scanner ──────────────────────────
            // Every 20 s a 30% chance roll fires. On success the closest eligible hooker
            // within 20 m walks over. On failure the cooldown is still applied so the
            // next attempt waits another 20 s.
            if (!sandboxMode && aLifeMode && !hasGirl && _approachCandidate == null
                && Game.GameTime >= _prostAutoApproachCooldown)
            {
                // Always push the cooldown forward regardless of roll outcome
                _prostAutoApproachCooldown = Game.GameTime + 20000;

                if (rng.NextDouble() < 0.30) // 30% chance to approach
                {
                    Ped autoPlayer = Game.Player.Character;
                    Ped[] autoNearby = World.GetNearbyPeds(autoPlayer.Position, 20f);
                    Ped bestAutoCandidate = null;
                    float bestAutoDist = float.MaxValue;
                    foreach (Ped ap in autoNearby)
                    {
                        if (ap == null || ap == autoPlayer || !ap.Exists() || !ap.IsAlive) continue;
                        if (!IsProstHookerModel(ap)) continue;
                        string apFp = GetPedFingerprint(ap);
                        if (apFp != null)
                        {
                            ALifePedData apData;
                            if (_aLifePeds.TryGetValue(ALifeKey(apFp), out apData))
                            {
                                if (apData.Reputation <= REP_MIN) continue;               // hostile — never approaches
                                if (apData.Reputation == -1 && rng.NextDouble() >= 0.01) continue; // avoiding — 99% won't bother
                            }
                        }
                        float apDist = ap.Position.DistanceTo(autoPlayer.Position);
                        if (apDist < bestAutoDist) { bestAutoDist = apDist; bestAutoCandidate = ap; }
                    }
                    if (bestAutoCandidate != null)
                    {
                        _prostAutoApproachActive = true;
                        TryPickup(autoPlayer, bestAutoCandidate);
                    }
                }
            }
        }

        // ═══════════════
        // ANIMATING STATE
        // ═══════════════
        private void HandleAnimatingState(Ped player)
        {
            if (girl == null || !girl.Exists())
            {
                EndAnimation();
                return;
            }

            // Disable phone during animations (control 27 = PHONE)
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, 27, true);

            // Vehicle mode: keep seat offset every frame
            if (mode == Mode.Car)
            {
                // Prevent player from exiting the vehicle (control 75 = VEH_EXIT)
                Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, 75, true);

                // Keep engine suppressed every tick when Car Engine setting is OFF (GTA re-enables it)
                if (!carEngineEnabled && player.IsInVehicle() && player.CurrentVehicle != null)
                    player.CurrentVehicle.IsEngineRunning = false;

                // Disable vehicle movement controls when car is not driveable
                if (!driveableCar)
                {
                    Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, 71, true);  // VEH_ACCELERATE
                    Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, 72, true);  // VEH_BRAKE
                    Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, 59, true);  // VEH_MOVE_LR
                    Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, 76, true);  // VEH_HANDBRAKE
                }

                RunCarAnimation(player);
            }

            // Street sitting action: run phase system for intro/outro transitions
            if (mode == Mode.Street && _streetSitPropActive && (streetActionIdx == 6 || streetActionIdx == 3 || streetActionIdx == 1 || streetActionIdx == 7 || streetActionIdx == 4 || streetActionIdx == 8 || streetActionIdx == 9))
            {
                RunStreetSitAnimation(player);
            }

            // Standing BJ ping-pong loop: bounce between ~0.01 and ~0.35 instead of hard-resetting
            // Runs during phase 2 (loop) and phase 3 finish-inside (anim still live)
            if (mode == Mode.Street && streetActionIdx == 2 && (animPhase == 2 || (animPhase == 3 && _finishInside)))
            {
                float sProg = Function.Call<float>(Hash.GET_ENTITY_ANIM_CURRENT_TIME, player, currentMaleDict, currentMaleAnim);
                const float SBJ_LOOP_START = 0.01f;
                const float SBJ_LOOP_END   = 0.35f;

                if (_standingBjReverse && sProg <= SBJ_LOOP_START)
                    _standingBjReverse = false;
                else if (!_standingBjReverse && sProg >= SBJ_LOOP_END)
                    _standingBjReverse = true;

                float sbjSpd = _standingBjReverse ? -animSpeed : animSpeed;
                Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, currentMaleDict, currentMaleAnim, sbjSpd);
                Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, currentFemaleDict, currentFemaleAnim, sbjSpd);

                // Safety: if backward playback wrapped past 0.0 → GTA loops to ~1.0, snap back and go forward
                // Use 0.5f as threshold — well above SBJ_LOOP_END so it never conflicts with the normal turn-around
                if (_standingBjReverse && sProg > 0.50f)
                {
                    Function.Call(Hash.SET_ENTITY_ANIM_CURRENT_TIME, player, currentMaleDict, currentMaleAnim, SBJ_LOOP_START);
                    Function.Call(Hash.SET_ENTITY_ANIM_CURRENT_TIME, girl, currentFemaleDict, currentFemaleAnim, SBJ_LOOP_START);
                    _standingBjReverse = false;
                    Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, currentMaleDict, currentMaleAnim, animSpeed);
                    Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, currentFemaleDict, currentFemaleAnim, animSpeed);
                }
            }

            // Periodic moaning (only during loop phase, scales smoothly with speed)
            int now = Game.GameTime;
            int voiceInterval = (int)(VOICE_INTERVAL / (1.0 + (animSpeed - 1.0) * 0.55));
            if (currentAction != "" && animPhase == 2 && animSpeed > 0f && now - lastVoiceTime > voiceInterval
                && !_bjThroatPending && _throatLockAt == 0 && !_throatSpeedLocked)
            {
                // Voice has priority — stop any BJSuck WAV currently playing
                if (currentAction == "blowjob" && _bjPlayer != null)
                {
                    try { _bjPlayer.Stop(); } catch { }
                    _bjSoundBusyUntil = 0;
                }
                PlayMoaningSounds(); // GTA ambient speech
                lastVoiceTime = now;
            }

            // Custom BJ sounds — separate, faster timer (runs alongside GTA speech)
            if (bjSoundsEnabled && currentAction == "blowjob" && animPhase == 2 && animSpeed > 0f && _bjSuckFiles != null)
            {
                int bjInterval = (int)(BJ_SOUND_INTERVAL / (1.0 + (animSpeed - 1.0) * 0.55));
                bool gtaSpeechActive = (now - _lastGtaSpeechTime < GTA_SPEECH_EST_DUR);
                if (!gtaSpeechActive && now - _lastBjSoundTime > bjInterval && now >= _bjSoundBusyUntil)
                {
                    // Look up (or compute) this woman's throat chance from her Confidence trait.
                    // In A-Life mode: use PersonalityProfile.Confidence (persisted to cache).
                    // In sandbox/prostitute mode: fall back to a random 0–0.35 roll (cached per girl).
                    double throatChance = 0.0;
                    if (girl != null && girl.Exists())
                    {
                        int gh = girl.Handle;
                        if (!_bjThroatChanceCache.ContainsKey(gh))
                        {
                            double computed = 0.0;
                            if (aLifeMode && _currentGirlFp != null)
                            {
                                ALifePedData apdThroat;
                                if (_aLifePeds.TryGetValue(_currentGirlKey, out apdThroat))
                                {
                                    PersonalityProfile profThroat = GetProfile(apdThroat.Personality);
                                    if (profThroat != null)
                                        computed = profThroat.Confidence;
                                }
                            }
                            if (computed <= 0.0)
                                computed = rng.NextDouble() * 0.35;
                            _bjThroatChanceCache[gh] = computed;
                        }
                        throatChance = _bjThroatChanceCache[gh];
                        // Hostile/Avoiding (rep <= -1): throating is extremely rare (1% cap)
                        if (aLifeMode && _currentGirlFp != null)
                        {
                            ALifePedData apdRep;
                            if (_aLifePeds.TryGetValue(_currentGirlKey, out apdRep) && apdRep.Reputation <= -1)
                                throatChance = Math.Min(throatChance, 0.01);
                        }
                    }
                    if (_bjThroatIntroFile != null && _bjThroatFile != null && rng.NextDouble() < throatChance)
                    {
                        // Throating is highest priority — stop any playing BJSuck WAV and voice
                        try { if (_bjPlayer != null) _bjPlayer.Stop(); } catch { }
                        _bjSoundBusyUntil = 0;
                        PlayBjCustom(_bjThroatIntroFile);
                        int introDur = _soundDurations.ContainsKey(_bjThroatIntroFile) ? _soundDurations[_bjThroatIntroFile] : 3000;
                        _bjThroatPending = true;
                        _bjThroatAt = now + introDur - 100;
                        // Abort current voice + block new voice during throat (both modes)
                        Function.Call(Hash.STOP_CURRENT_PLAYING_AMBIENT_SPEECH, girl);
                        _lastGtaSpeechTime = now;
                    }
                    else
                    {
                        PlayBjCustom(_bjSuckFiles[rng.Next(_bjSuckFiles.Length)]);
                    }
                    _lastBjSoundTime = now;
                }
            }

            // Spit lip anim: clear after 500ms
            if (_bjSpitFacialStopAt > 0 && now >= _bjSpitFacialStopAt)
            {
                _bjSpitFacialStopAt = 0;
                if (girl != null && girl.Exists())
                    Function.Call(Hash.PLAY_FACIAL_ANIM, girl.Handle, "mood_normal_1", "facials@gen_female@variations@normal");
            }

            // BJThroat follow-up: play BJThroat.wav after BJThroat_Intro.wav finishes
            if (_bjThroatPending && now >= _bjThroatAt)
            {
                _bjThroatPending = false;
                PlayBjCustom(_bjThroatFile, true); // force=true: chains immediately after intro
                int throatDur = _soundDurations.ContainsKey(_bjThroatFile) ? _soundDurations[_bjThroatFile] : 8000;
                _throatLockAt        = now + 1000;            // lock speed 1s after main WAV starts
                _throatSequenceEndAt = now + throatDur + 300; // 300ms buffer after WAV ends
            }

            // Sex slap sounds — car sex + applicable street sex actions; silent below 1.40x, silent for facesitting (idx 1)
            // Also fires during animPhase==3 finish-inside delay for standing street sex (Wall Sex/Doggy/Rough Doggy)
            bool _standingFinishInsideSlap = _finishInside && animPhase == 3
                && mode == Mode.Street && (streetActionIdx == 5 || streetActionIdx == 10 || streetActionIdx == 11);
            if (slapSoundsEnabled && currentAction == "sex" && (animPhase == 2 || _standingFinishInsideSlap) && _sexSlapFiles != null && animSpeed >= 1.40f
                && !(mode == Mode.Street && streetActionIdx == 1))
            {
                int slapSlow, slapMid, slapFast;
                GetSlapIntervals(out slapSlow, out slapMid, out slapFast);
                int slapInterval;
                if (animSpeed <= 2.0f)
                {
                    float t = animSpeed - 1.0f; // 0 at 1x, 1 at 2x
                    slapInterval = (int)(slapSlow + (slapMid - slapSlow) * t);
                }
                else
                {
                    float t = animSpeed - 2.0f; // 0 at 2x, 1 at 3x
                    slapInterval = (int)(slapMid + (slapFast - slapMid) * t);
                }
                if (now - _lastSlapSoundTime > slapInterval)
                {
                    PlaySlapSound(_sexSlapFiles[rng.Next(_sexSlapFiles.Length)]);
                    _lastSlapSoundTime = now;
                }
            }

            // Kissing sounds — street kissing (idx 0) loop phase, speed-driven interval
            if (currentAction == "kissing" && animPhase == 2 && _kissSoundFiles != null)
            {
                int kissInterval;
                if (animSpeed <= 2.0f)
                {
                    float t = animSpeed - 1.0f; // 0 at 1x, 1 at 2x
                    kissInterval = (int)(KISS_SOUND_SLOW + (KISS_SOUND_MID - KISS_SOUND_SLOW) * t);
                }
                else
                {
                    float t = animSpeed - 2.0f; // 0 at 2x, 1 at 3x
                    kissInterval = (int)(KISS_SOUND_MID + (KISS_SOUND_FAST - KISS_SOUND_MID) * t);
                }
                if (now - _lastKissSoundTime > kissInterval)
                {
                    PlayKissSound(_kissSoundFiles[rng.Next(_kissSoundFiles.Length)]);
                    _lastKissSoundTime = now;
                }
            }

            // Slow healing during loop phase
            if (animPhase == 2 && now - lastHealTime > HEAL_INTERVAL)
            {
                int hp = (int)(HEAL_AMOUNT * healRate * (animSpeed > 1.0f ? animSpeed : 1.0f));
                if (healPlayer)
                {
                    player.Health = Math.Min(player.Health + hp, player.MaxHealth);
                }
                if (healGirl && girl != null && girl.Exists())
                {
                    girl.Health = Math.Min(girl.Health + hp, girl.MaxHealth);
                }
                lastHealTime = now;
            }

            // Apply throat speed lock when scheduled
            if (_throatLockAt > 0 && now >= _throatLockAt)
            {
                _preThroatSpeed    = animSpeed;
                animSpeed          = 1.80f;
                _throatSpeedLocked = true;
                _throatLockAt      = 0;
                // Push the new speed to the engine immediately — the normal speed-control
                // block only calls SET_ENTITY_ANIM_SPEED when the player manually changes speed.
                {
                    string mDictT = currentMaleDict;
                    string fDictT = currentFemaleDict;
                    if (mode == Mode.Car && !_backseatCarSex)
                    {
                        int camModeT = Function.Call<int>(Hash.GET_FOLLOW_PED_CAM_VIEW_MODE);
                        if (camModeT == 4)
                        {
                            mDictT = (vehicleAnimType == VehicleAnimType.Low) ? VEH_LOW_FP_DICT : VEH_NORM_FP_DICT;
                            fDictT = mDictT;
                        }
                    }
                    Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, mDictT, currentMaleAnim, animSpeed);
                    if (girl != null && girl.Exists())
                        Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, fDictT, currentFemaleAnim, animSpeed * _femaleAnimSpeedScale);
                }
            }
            // Release throat speed lock when BJThroat.wav finishes
            if (_throatSpeedLocked && _throatSequenceEndAt > 0 && now >= _throatSequenceEndAt)
            {
                animSpeed            = _preThroatSpeed;
                _throatSpeedLocked   = false;
                _throatSequenceEndAt = 0;
                {
                    string mDictR = currentMaleDict;
                    string fDictR = currentFemaleDict;
                    if (mode == Mode.Car && !_backseatCarSex)
                    {
                        int camModeR = Function.Call<int>(Hash.GET_FOLLOW_PED_CAM_VIEW_MODE);
                        if (camModeR == 4)
                        {
                            mDictR = (vehicleAnimType == VehicleAnimType.Low) ? VEH_LOW_FP_DICT : VEH_NORM_FP_DICT;
                            fDictR = mDictR;
                        }
                    }
                    Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, mDictR, currentMaleAnim, animSpeed);
                    if (girl != null && girl.Exists())
                        Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, fDictR, currentFemaleAnim, animSpeed * _femaleAnimSpeedScale);
                }
            }

            // Speed control during loop phase
            if (speedCooldown > 0) speedCooldown--;
            if (animPhase == 2 && speedCooldown == 0 && !_finishExecutePending && (!_bjThroatPending && _throatLockAt == 0 && !_throatSpeedLocked))
            {
                bool changed = false;
                bool noMasterMod = !Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 0, masterControlModifier);
                if (_usingController ? (Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 0, speedUpControl) && noMasterMod) : Game.IsKeyPressed(speedUpKey))
                {
                    animSpeed = Math.Min(animSpeed + 0.05f, 3.0f);
                    changed = true;
                }
                else if (_usingController ? (Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 0, speedDownControl) && noMasterMod) : Game.IsKeyPressed(speedDownKey))
                {
                    animSpeed = Math.Max(animSpeed - 0.05f, 0.00f);
                    changed = true;
                }
                else if (_usingController ? Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 0, speedResetControl) : Game.IsKeyPressed(speedResetKey))
                {
                    animSpeed = 1.0f;
                    changed = true;
                }
                if (changed)
                {
                    speedCooldown = 10;
                    // Force the device to respond immediately to the speed change,
                    // bypassing the dead-zone (which would otherwise swallow small deltas).
                    if (!_deviceExcluded)
                    {
                        _telemetry.ForceSendIntensity(DeviceIntensity());
                    }
                    string mDict = currentMaleDict;
                    string fDict = currentFemaleDict;
                    if (mode == Mode.Car && !_backseatCarSex)
                    {
                        int camMode = Function.Call<int>(Hash.GET_FOLLOW_PED_CAM_VIEW_MODE);
                        if (camMode == 4) // first person
                        {
                            if (vehicleAnimType == VehicleAnimType.Low)
                            {
                                mDict = VEH_LOW_FP_DICT;
                                fDict = VEH_LOW_FP_DICT;
                            }
                            else
                            {
                                mDict = VEH_NORM_FP_DICT;
                                fDict = VEH_NORM_FP_DICT;
                            }
                        }
                    }
                    Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, mDict, currentMaleAnim, animSpeed);
                    Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, fDict, currentFemaleAnim, animSpeed * _femaleAnimSpeedScale);
                }
            }

            // Street mode: Kissing phase management (intro → ping-pong loop → outro)
            // The ~6s anim plays 0→3s as intro, ping-pongs 2.50–3.00s as loop, then 3s→end as outro.
            // Ping-pong uses negative anim speed for smooth reversal.
            if (mode == Mode.Street && streetActionIdx == 0 && currentMaleDict == "mp_ped_interaction")
            {
                float mProg = Function.Call<float>(Hash.GET_ENTITY_ANIM_CURRENT_TIME, player, currentMaleDict, currentMaleAnim);
                // Normalized boundaries: ~2.50s and ~3.00s of a ~6s animation
                const float KISS_LOOP_START = 0.417f;
                const float KISS_LOOP_END   = 0.500f;

                if (animPhase == 0) // INTRO: playing from 0 → 0.50
                {
                    if (mProg >= KISS_LOOP_END)
                    {
                        // Reached the loop point — start ping-ponging backward
                        _kissingReverse = true;
                        Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, currentMaleDict, currentMaleAnim, -animSpeed);
                        Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, currentFemaleDict, currentFemaleAnim, -animSpeed);
                        animPhase = 2;
                    }
                }
                else if (animPhase == 2) // LOOP: ping-pong between KISS_LOOP_START ↔ KISS_LOOP_END
                {
                    if (_kissingReverse && mProg <= KISS_LOOP_START)
                    {
                        // Hit bottom of bounce — reverse to forward
                        _kissingReverse = false;
                    }
                    else if (!_kissingReverse && mProg >= KISS_LOOP_END)
                    {
                        // Hit top of bounce — reverse to backward
                        _kissingReverse = true;
                    }
                    // Always enforce correct speed direction (speed-control block sets positive;
                    // this corrects it every frame so mid-bounce speed changes don't break the loop).
                    float kissSpd = _kissingReverse ? -animSpeed : animSpeed;
                    Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, currentMaleDict, currentMaleAnim, kissSpd);
                    Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, currentFemaleDict, currentFemaleAnim, kissSpd);
                    // Safety: if anim somehow stopped or went past boundaries, recover
                    if (mProg <= 0.01f || mProg >= 0.95f)
                    {
                        Function.Call(Hash.SET_ENTITY_ANIM_CURRENT_TIME, player, currentMaleDict, currentMaleAnim, KISS_LOOP_START);
                        Function.Call(Hash.SET_ENTITY_ANIM_CURRENT_TIME, girl, currentFemaleDict, currentFemaleAnim, KISS_LOOP_START);
                        _kissingReverse = false;
                        Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, currentMaleDict, currentMaleAnim, animSpeed);
                        Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, currentFemaleDict, currentFemaleAnim, animSpeed);
                    }
                }
                else if (animPhase == 3) // OUTRO: playing from ~0.50 → end
                {
                    if (mProg >= 0.95f)
                    {
                        PlayFinishSounds();
                        if (girl != null && girl.Exists() && femaleEndVoice && !_finishForced && !_finishBroke)
                        {
                            AssignHookerVoice(girl);
                            PlayPedSpeech(girl, "再次提供服務");
                        }
                        FinishStandingServiceAndReopen();
                        return;
                    }
                }
            }

            // Standing BJ phase management (intro → blackscreen → loop → finish delay → blackscreen → outro)
            if (mode == Mode.Street && streetActionIdx == 2 && animPhase != 2)
            {
                if (animPhase == 0) // INTRO: zip/kneel playing → trigger fade at 1.5s
                {
                    DisablePlayerMovement(); // block legs while upper-body zip plays
                    if (Game.GameTime >= animWaitTime)
                    {
                        Function.Call(Hash.DO_SCREEN_FADE_OUT, 650);
                        animWaitTime = Game.GameTime + 1000; // 500ms fade + 1000ms hold black
                        animPhase = 1;
                    }
                }
                else if (animPhase == 1) // FADE TO LOOP: wait for black + hold, swap to loop anims
                {
                    if (Function.Call<bool>(Hash.IS_SCREEN_FADED_OUT) && Game.GameTime >= animWaitTime)
                    {
                        // Clear intro anims: girl's upper-body idle overlay, held kneel, and player's upper-body zip
                        Function.Call(Hash.STOP_ANIM_TASK, girl, "amb@code_human_cross_road@female@idle_a", "idle_a", -4.0f);
                        Function.Call(Hash.STOP_ANIM_TASK, girl, "missheist_jewel", "manageress_kneel_intro", -4.0f);
                        Function.Call(Hash.STOP_ANIM_TASK, player, "trevor_1_int-15", "player_two_dual-15", -4.0f);

                        Vector3 pp = player.Position;
                        Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, player, currentMaleDict, currentMaleAnim,
                            pp.X, pp.Y, pp.Z, 0f, 0f, player.Heading,
                            8.0f, -4.0f, -1, 9, 0f, 2, 0); // flag 9 = looping + non-interruptable
                        Function.Call(Hash.TASK_PLAY_ANIM, girl, currentFemaleDict, currentFemaleAnim,
                            8.0f, -4.0f, -1, 9, 0f, false, false, false);
                        // Don't fade in yet — let the loop anims settle for 400ms while still black
                        animWaitTime = Game.GameTime + 400;
                        animPhase = 100; // temporary phase: wait for anims to settle, then fade in
                    }
                }
                else if (animPhase == 100) // SETTLE: loop anims playing behind black screen → fade in
                {
                    if (Game.GameTime >= animWaitTime)
                    {
                        Function.Call(Hash.DO_SCREEN_FADE_IN, 500);
                        animPhase = 2;
                    }
                }
                else if (animPhase == 3) // FINISH DELAY: loop plays (inside) or pull-out anims play (pull-out) for 3s
                {
                    if (Game.GameTime >= animWaitTime) // 3s delay passed → clear pull-out anims if needed, trigger fade
                    {
                        // Pull-out: stop player pull-out anim; girl's idle_b overlay stays running through the fade
                        // so she doesn't snap back to the kneeling loop upper-body before the screen goes black.
                        if (!_finishInside)
                        {
                            Function.Call(Hash.STOP_ANIM_TASK, player, "switch@trevor@jerking_off", "trev_jerking_off_exit", -4.0f);
                        }
                        Function.Call(Hash.DO_SCREEN_FADE_OUT, 500);
                        animWaitTime = Game.GameTime + 1000; // 500ms fade + 500ms hold black
                        animPhase = 4;
                    }
                }
                else if (animPhase == 4) // FADE TO OUTRO: wait for black + hold, swap to outro anims
                {
                    if (Function.Call<bool>(Hash.IS_SCREEN_FADED_OUT) && Game.GameTime >= animWaitTime)
                    {
                        string outroMDict = "trevor_1_int-15";
                        string outroMAnim = "player_two_dual-15";
                        string outroFDict = "missheist_jewel";
                        string outroFAnim = "manageress_kneel_intro";
                        if (RequestAnimDict(outroMDict) && RequestAnimDict(outroFDict))
                        {
                            // Stop loop anims on both peds — flag 9 (non-interruptable) would
                            // otherwise prevent the outro from playing.
                            Function.Call(Hash.STOP_ANIM_TASK, player, currentMaleDict, currentMaleAnim, -4.0f);
                            Function.Call(Hash.STOP_ANIM_TASK, girl, currentFemaleDict, currentFemaleAnim, -4.0f);
                            // Also stop any lingering held manageress_kneel_intro from the intro phase,
                            // otherwise GTA may refuse to start a new instance of the same anim.
                            Function.Call(Hash.STOP_ANIM_TASK, girl, "missheist_jewel", "manageress_kneel_intro", -4.0f);
                            // Pull-out: stop idle_b upper-body overlay now that we're behind the black screen.
                            if (!_finishInside)
                                Function.Call(Hash.STOP_ANIM_TASK, girl, "amb@code_human_cross_road@female@idle_a", "idle_b", -4.0f);

                            // Player: upper-body zip only (lower body stays in natural idle stance)
                            Function.Call(Hash.TASK_PLAY_ANIM, player, outroMDict, outroMAnim,
                                8.0f, -4.0f, -1, 50, 0f, false, false, false); // flag 50 = upper-body(16) + secondary(32) + hold-last-frame(2)

                            // Female: play from ~2.10s mark in reverse (getting up from kneeling)
                            float fDur = Function.Call<float>(Hash.GET_ANIM_DURATION, outroFDict, outroFAnim);
                            float outroStartPhase = (fDur > 0f) ? Math.Min(2.10f / fDur, 0.99f) : 0.5f;
                            Function.Call(Hash.TASK_PLAY_ANIM, girl, outroFDict, outroFAnim,
                                8.0f, -4.0f, -1, 10, outroStartPhase, false, false, false);
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, outroFDict, outroFAnim, -1.0f);

                            // Overlay upper-body idle so only the lower body shows the standing-up motion
                            string ubIdleDictO = "amb@code_human_cross_road@female@idle_a";
                            if (RequestAnimDict(ubIdleDictO))
                                Function.Call(Hash.TASK_PLAY_ANIM, girl, ubIdleDictO, "idle_b",
                                    8.0f, -4.0f, -1, 50, 0f, false, false, false); // flag 50 = hold-last-frame(2) + upper-body(16) + secondary(32)

                            DetachDildoProp();
                            PlayBjCustom(_bjPantsZipFile, true);
                        }
                        Function.Call(Hash.DO_SCREEN_FADE_IN, 500);
                        animWaitTime = Game.GameTime + 2100; // ~2.10s for player outro
                        _standingBjSwallowAt = Game.GameTime + 1000; // swallow 1s after outro starts
                        animPhase = 5;
                    }
                }
                else if (animPhase == 5) // OUTRO: player zip done → release player; girl still standing up
                {
                    DisablePlayerMovement(); // block legs while upper-body zip plays
                    // Enforce negative speed every frame — the initial SET_ENTITY_ANIM_SPEED in phase 4
                    // fires on the same tick as TASK_PLAY_ANIM and may be silently ignored by the engine.
                    if (girl != null && girl.Exists())
                        Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, "missheist_jewel", "manageress_kneel_intro", -1.0f);

                    // Delayed swallow: 1s after outro starts
                    if (_standingBjSwallowAt > 0 && Game.GameTime >= _standingBjSwallowAt)
                    {
                        _standingBjSwallowAt = 0;
                        if (bjSoundsEnabled)
                        {
                            bool skipSwallow = !_finishInside || _finishForced || _finishBroke;
                            if (!skipSwallow) PlayBjCustom(_bjSwallowFile, true);
                            else if (aLifeMode && (_finishForced || _finishBroke)) PlayBjSpit();
                        }
                    }
                    if (Game.GameTime >= animWaitTime)
                    {
                        // Player is done — release him, let him move
                        player.IsPositionFrozen = false;
                        Function.Call(Hash.SET_ENTITY_COLLISION, player, true, true);
                        player.Task.ClearAll();
                        Function.Call(Hash.SET_PLAYER_CAN_DO_DRIVE_BY, Game.Player, true);
                        Function.Call(Hash.SET_EVERYONE_IGNORE_PLAYER, Game.Player, false);

                        // Girl still has ~2.10s of reverse anim; wait for her to finish
                        animWaitTime = Game.GameTime + 2100;
                        animPhase = 6;
                    }
                }
                else if (animPhase == 6) // OUTRO: girl standup done → release girl, finish
                {
                    // Keep enforcing negative speed on girl's reverse stand-up anim
                    if (girl != null && girl.Exists())
                        Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, "missheist_jewel", "manageress_kneel_intro", -1.0f);

                    // Delayed swallow (safety: in case it hasn't fired yet)
                    if (_standingBjSwallowAt > 0 && Game.GameTime >= _standingBjSwallowAt)
                    {
                        _standingBjSwallowAt = 0;
                        if (bjSoundsEnabled)
                        {
                            bool skipSwallow = !_finishInside || _finishForced || _finishBroke;
                            if (!skipSwallow) PlayBjCustom(_bjSwallowFile, true);
                            else if (aLifeMode && (_finishForced || _finishBroke)) PlayBjSpit();
                        }
                    }
                    if (Game.GameTime >= animWaitTime)
                    {
                        // BJ finish: play HOOKER_OFFER_AGAIN immediately (no post-sex delay needed)
                        if (girl != null && girl.Exists() && femaleEndVoice && !_finishForced && !_finishBroke)
                        {
                            AssignHookerVoice(girl);
                            PlayPedSpeech(girl, "再次提供服務");
                        }
                        // Handle forced/broke mood penalty (mirrors PlayFinishSounds logic)
                        if ((_finishForced || _finishBroke) && aLifeMode && _currentGirlKey != null)
                        {
                            ALifePedData dMood;
                            if (_aLifePeds.TryGetValue(_currentGirlKey, out dMood))
                            {
                                dMood.Mood = "Annoyed";
                                ApplyMoodFacialExpression(girl, "Relaxed");
                                SaveALife();
                            }
                        }
                        FinishStandingServiceAndReopen();
                        return;
                    }
                }
            }

            // Wall Sex phase management (intro → blackscreen → loop → finish delay → blackscreen → outro)
            if (mode == Mode.Street && streetActionIdx == 5 && animPhase != 2)
            {
                if (animPhase == 0) // INTRO: zip/approach playing → trigger fade at 1.5s
                {
                    DisablePlayerMovement(); // block legs while upper-body zip plays
                    if (Game.GameTime >= animWaitTime)
                    {
                        Function.Call(Hash.DO_SCREEN_FADE_OUT, 650);
                        animWaitTime = Game.GameTime + 1000; // 650ms fade + hold black
                        animPhase = 1;
                    }
                }
                else if (animPhase == 1) // FADE TO LOOP: wait for black + hold, swap to loop anims
                {
                    if (Function.Call<bool>(Hash.IS_SCREEN_FADED_OUT) && Game.GameTime >= animWaitTime)
                    {
                        // Clear intro anims: player's upper-body zip and girl's exit_hooker
                        Function.Call(Hash.STOP_ANIM_TASK, player, "trevor_1_int-15", "player_two_dual-15", -4.0f);
                        Function.Call(Hash.STOP_ANIM_TASK, girl, "switch@michael@prostitute", "exit_hooker", -4.0f);

                        // Reposition girl to correct wall sex offset + heading
                        // Player is already at the correct Z (general setup applied zOffset=-0.15f),
                        // so just reposition girl relative to current player position — no extra sinking.
                        Vector3 ppW = player.Position;
                        Vector3 girlWallPos = player.GetOffsetPosition(IsSinkraLowerModel() ? _wallBangLowerOffset : new Vector3(0.02f, 0.36f, 0.048f));
                        Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                            girlWallPos.X, girlWallPos.Y, girlWallPos.Z, false, false, false);
                        girl.Heading = player.Heading + 190f;

                        Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, player, currentMaleDict, currentMaleAnim,
                            ppW.X, ppW.Y, ppW.Z, 0f, 0f, player.Heading,
                            8.0f, -4.0f, -1, 9, 0f, 2, 0); // flag 9 = looping + non-interruptable
                        Function.Call(Hash.TASK_PLAY_ANIM, girl, currentFemaleDict, currentFemaleAnim,
                            8.0f, -4.0f, -1, 9, 0f, false, false, false);
                        // Let loop anims settle for 400ms while still black
                        animWaitTime = Game.GameTime + 400;
                        animPhase = 100;
                    }
                }
                else if (animPhase == 100) // SETTLE: loop anims playing behind black screen → fade in
                {
                    if (Game.GameTime >= animWaitTime)
                    {
                        Function.Call(Hash.DO_SCREEN_FADE_IN, 500);
                        animPhase = 2;
                    }
                }
                else if (animPhase == 3) // FINISH DELAY: loop plays (inside) or pull-out anim plays (pull-out) for 3s
                {
                    if (Game.GameTime >= animWaitTime)
                    {
                        // Pull-out anim keeps running through the fade to avoid a snap — stopped behind the black screen in phase 4
                        Function.Call(Hash.DO_SCREEN_FADE_OUT, 500);
                        animWaitTime = Game.GameTime + 1000; // 500ms fade + 500ms hold black
                        animPhase = 4;
                    }
                }
                else if (animPhase == 4) // FADE TO OUTRO: player zip only, no girl outro
                {
                    if (Function.Call<bool>(Hash.IS_SCREEN_FADED_OUT) && Game.GameTime >= animWaitTime)
                    {
                        string outroMDict = "trevor_1_int-15";
                        string outroMAnim = "player_two_dual-15";
                        if (RequestAnimDict(outroMDict))
                        {
                            // Stop loop anims on both peds
                            Function.Call(Hash.STOP_ANIM_TASK, player, currentMaleDict, currentMaleAnim, -4.0f);
                            Function.Call(Hash.STOP_ANIM_TASK, girl, currentFemaleDict, currentFemaleAnim, -4.0f);
                            // Pull-out: stop player pull-out anim now that we're behind the black screen
                            if (!_finishInside)
                                Function.Call(Hash.STOP_ANIM_TASK, player, "switch@trevor@jerking_off", "trev_jerking_off_exit", -4.0f);
                            // Compensate for -0.15f zOffset — girl plays a standing anim at ground level
                            Vector3 girlOutroPos = player.GetOffsetPosition(new Vector3(0.0f, 1.29f, 0.07f));
                            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                                girlOutroPos.X, girlOutroPos.Y, girlOutroPos.Z + 0.15f, false, false, false);
                            girl.Heading = player.Heading + 180f + 90f; // kissing heading + 90° right

                            // Player: upper-body zip only
                            Function.Call(Hash.TASK_PLAY_ANIM, player, outroMDict, outroMAnim,
                                8.0f, -4.0f, -1, 50, 0f, false, false, false); // flag 50 = upper-body + secondary + hold-last-frame

                            DetachDildoProp();
                            PlayBjCustom(_bjPantsZipFile, true);
                        }
                        Function.Call(Hash.DO_SCREEN_FADE_IN, 500);
                        animWaitTime = Game.GameTime + 2100; // ~2.10s for player zip
                        animPhase = 5;
                    }
                }
                else if (animPhase == 5) // OUTRO: player zip done → release both, finish
                {
                    DisablePlayerMovement(); // block legs while upper-body zip plays
                    if (Game.GameTime >= animWaitTime)
                    {
                        // Play SEX_FINISHED voice
                        if (girl != null && girl.Exists() && femaleEndVoice && !_finishForced && !_finishBroke)
                        {
                            AssignHookerVoice(girl);
                            PlayPedSpeech(girl, "互動完成");
                            _postSexPhase = 1;
                        }
                        // Handle forced/broke mood penalty
                        if ((_finishForced || _finishBroke) && aLifeMode && _currentGirlKey != null)
                        {
                            ALifePedData dMood;
                            if (_aLifePeds.TryGetValue(_currentGirlKey, out dMood))
                            {
                                dMood.Mood = "Annoyed";
                                ApplyMoodFacialExpression(girl, dMood.Mood);
                                SaveALife();
                            }
                        }
                        FinishStandingServiceAndReopen();
                        return;
                    }
                }
            }

            // ── Doggy / Rough Doggy phase management (same system as Wall Sex) ──
            if (mode == Mode.Street && (streetActionIdx == 10 || streetActionIdx == 11) && animPhase != 2)
            {
                if (animPhase == 0) // INTRO: zip/approach playing → trigger fade at 1.5s
                {
                    DisablePlayerMovement(); // block legs while upper-body zip plays
                    if (Game.GameTime >= animWaitTime)
                    {
                        Function.Call(Hash.DO_SCREEN_FADE_OUT, 650);
                        animWaitTime = Game.GameTime + 1000;
                        animPhase = 1;
                    }
                }
                else if (animPhase == 1) // FADE TO LOOP: swap to loop anims
                {
                    if (Function.Call<bool>(Hash.IS_SCREEN_FADED_OUT) && Game.GameTime >= animWaitTime)
                    {
                        Function.Call(Hash.STOP_ANIM_TASK, player, "trevor_1_int-15", "player_two_dual-15", -4.0f);
                        Function.Call(Hash.STOP_ANIM_TASK, girl, "switch@michael@prostitute", "exit_hooker", -4.0f);

                        // Reposition girl to correct loop offset — read from GetStreetAnimData so case 10/11 is the single source of truth
                        string _lMD, _lMA, _lFD, _lFA; bool _lSit; float _lZ, _lRX;
                        Vector3 loopOffset; float loopHeading;
                        GetStreetAnimData(streetActionIdx, out _lMD, out _lMA, out _lFD, out _lFA, out loopOffset, out loopHeading, out _lSit, out _lZ, out _lRX);
                        Vector3 girlLoopPos = player.GetOffsetPosition(loopOffset);
                        Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                            girlLoopPos.X, girlLoopPos.Y, girlLoopPos.Z, false, false, false);
                        girl.Heading = player.Heading + loopHeading;

                        // Rough Doggy uses startPhase 0.205f on BOTH male and female
                        // Use TASK_PLAY_ANIM (not ADVANCED) — matches original setup; ADVANCED breaks looping for trevor_1_int-13
                        float loopStart = (streetActionIdx == 11) ? 0.205f : 0f;
                        Function.Call(Hash.TASK_PLAY_ANIM, player, currentMaleDict, currentMaleAnim,
                            8.0f, -4.0f, -1, 9, loopStart, false, false, false);
                        Function.Call(Hash.TASK_PLAY_ANIM, girl, currentFemaleDict, currentFemaleAnim,
                            8.0f, -4.0f, -1, 9, loopStart, false, false, false);

                        animWaitTime = Game.GameTime + 400;
                        animPhase = 100;
                    }
                }
                else if (animPhase == 100) // SETTLE: loop anims behind black → fade in
                {
                    if (Game.GameTime >= animWaitTime)
                    {
                        Function.Call(Hash.DO_SCREEN_FADE_IN, 500);
                        animPhase = 2;
                    }
                }
                else if (animPhase == 3) // FINISH DELAY: loop plays (inside) or pull-out anim plays (pull-out) for 3s
                {
                    if (Game.GameTime >= animWaitTime)
                    {
                        // Pull-out anim keeps running through the fade — stopped behind the black screen in phase 4
                        Function.Call(Hash.DO_SCREEN_FADE_OUT, 500);
                        animWaitTime = Game.GameTime + 1000;
                        animPhase = 4;
                    }
                }
                else if (animPhase == 4) // FADE TO OUTRO: player zip, girl back to kissing offset
                {
                    if (Function.Call<bool>(Hash.IS_SCREEN_FADED_OUT) && Game.GameTime >= animWaitTime)
                    {
                        string outroMDict = "trevor_1_int-15";
                        string outroMAnim = "player_two_dual-15";
                        if (RequestAnimDict(outroMDict))
                        {
                            Function.Call(Hash.STOP_ANIM_TASK, player, currentMaleDict, currentMaleAnim, -4.0f);
                            Function.Call(Hash.STOP_ANIM_TASK, girl, currentFemaleDict, currentFemaleAnim, -4.0f);
                            // Pull-out: stop player pull-out anim now that we're behind the black screen
                            if (!_finishInside)
                                Function.Call(Hash.STOP_ANIM_TASK, player, "switch@trevor@jerking_off", "trev_jerking_off_exit", -4.0f);

                            Vector3 girlOutroPos = player.GetOffsetPosition(new Vector3(0.0f, 1.29f, 0.07f));
                            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                                girlOutroPos.X, girlOutroPos.Y, girlOutroPos.Z, false, false, false);
                            girl.Heading = player.Heading + 180f + 90f;

                            Function.Call(Hash.TASK_PLAY_ANIM, player, outroMDict, outroMAnim,
                                8.0f, -4.0f, -1, 50, 0f, false, false, false);

                            DetachDildoProp();
                            PlayBjCustom(_bjPantsZipFile, true);
                        }
                        Function.Call(Hash.DO_SCREEN_FADE_IN, 500);
                        animWaitTime = Game.GameTime + 2100;
                        animPhase = 5;
                    }
                }
                else if (animPhase == 5) // OUTRO: zip done → release both, finish
                {
                    DisablePlayerMovement(); // block legs while upper-body zip plays
                    if (Game.GameTime >= animWaitTime)
                    {
                        if (girl != null && girl.Exists() && femaleEndVoice && !_finishForced && !_finishBroke)
                        {
                            AssignHookerVoice(girl);
                            PlayPedSpeech(girl, "互動完成");
                            _postSexPhase = 1;
                        }
                        if ((_finishForced || _finishBroke) && aLifeMode && _currentGirlKey != null)
                        {
                            ALifePedData dMood;
                            if (_aLifePeds.TryGetValue(_currentGirlKey, out dMood))
                            {
                                dMood.Mood = "Annoyed";
                                ApplyMoodFacialExpression(girl, dMood.Mood);
                                SaveALife();
                            }
                        }
                        FinishStandingServiceAndReopen();
                        return;
                    }
                }
            }

            // Rough Doggy ping-pong loop: bounce between startPhase (~0.19) and ~0.70 instead of hard-resetting
            // Runs during phase 2 (loop) and phase 3 finish-inside (anim still live)
            if (mode == Mode.Street && streetActionIdx == 11 && (animPhase == 2 || (animPhase == 3 && _finishInside)))
            {
                float rProg = Function.Call<float>(Hash.GET_ENTITY_ANIM_CURRENT_TIME, player, currentMaleDict, currentMaleAnim);
                const float RDOGGY_LOOP_START = 0.19f;
                const float RDOGGY_LOOP_END   = 0.70f;

                if (_rdoggyReverse && rProg <= RDOGGY_LOOP_START)
                    _rdoggyReverse = false;
                else if (!_rdoggyReverse && rProg >= RDOGGY_LOOP_END)
                    _rdoggyReverse = true;

                float rdoggySpd = _rdoggyReverse ? -animSpeed : animSpeed;
                Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, currentMaleDict, currentMaleAnim, rdoggySpd);
                Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, currentFemaleDict, currentFemaleAnim, rdoggySpd);

                // Safety: if GTA's loop engine wrapped the anim back to 0.0 while going forward, reposition to loop start
                // (do NOT check rProg >= 0.99 here — that would override the direction flip we just applied above)
                if (!_rdoggyReverse && rProg <= 0.01f)
                {
                    Function.Call(Hash.SET_ENTITY_ANIM_CURRENT_TIME, player, currentMaleDict, currentMaleAnim, RDOGGY_LOOP_START);
                    Function.Call(Hash.SET_ENTITY_ANIM_CURRENT_TIME, girl, currentFemaleDict, currentFemaleAnim, RDOGGY_LOOP_START);
                }
            }

            // Persistent HUD during loop phase
            if (animPhase == 2)
            {
                // Re-enforce female anim speed scale every tick — GTA resets it to 1x on its own (e.g. cowgirl_clip needs 2x)
                if (_femaleAnimSpeedScale != 1.0f && girl != null && girl.Exists() && currentFemaleDict != null && currentFemaleAnim != null)
                    Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, currentFemaleDict, currentFemaleAnim, animSpeed * _femaleAnimSpeedScale);

                // Telemetry: device intensity scales from cfg value at 1x up to 100% at 3x
                if (!_deviceExcluded)
                {
                    _telemetry.SendIntensity(DeviceIntensity());
                }

                // Controller rumble pulses — plateau intensity at 2.10x, near-continuous above it.
                // DualSense ignores XInput amplitude — duty cycle controls perceived intensity.
                if (_usingController)
                {
                    // Clamp t to [0,1] over the 0.30x–2.10x ramp, then flat
                    float tRamp = Math.Min(1.0f, (animSpeed - 0.30f) / 1.80f);
                    // Interval: 700ms at 0.30x down to 150ms at 2.10x, stays 150ms above
                    int rumbleInterval = (int)(700f - tRamp * 550f);
                    // Pulse ON: 60ms at 0.30x up to 140ms at 2.10x, stays 140ms above
                    int pulseDuration  = (int)(60f  + tRamp * 80f);
                    // Full power always
                    ushort motor = 65535;

                    if (!_rumblePulseActive && now - _lastRumbleTime >= rumbleInterval)
                    {
                        // Fire pulse ON
                        SetVibration(motor, motor);
                        _rumblePulseActive = true;
                        _lastRumbleTime = now;
                    }
                    else if (_rumblePulseActive && now - _lastRumbleTime >= pulseDuration)
                    {
                        // End pulse — go silent until next interval
                        SetVibration(0, 0);
                        _rumblePulseActive = false;
                    }
                }

                string speedDisplay = (_bjThroatPending || _throatLockAt > 0 || _throatSpeedLocked) ? "進行中……" : animSpeed.ToString("0.00") + "x";
                if (showTutorialMessages)
                    DrawTextOnScreen("~b~[" + MasterDisplay + "]~s~ FINISH | ~b~[" + SpeedDownDisplay + "/" + SpeedUpDisplay + "]~s~ CHANGE SPEED | ~b~[" + SpeedResetDisplay + "]~s~ RESET SPEED [SPEED: " + speedDisplay + "]", 0.5f, 0.920f, 0.45f, true);
            }

            // Master key / controller master button: finish / cum / abort
            bool masterTriggered = _usingController
                ? (Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 0, masterControlModifier) && Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 0, masterControl))
                : Game.IsKeyPressed(masterKey);
            if (masterTriggered && keyCooldown == 0)
            {
                keyCooldown = KEY_COOLDOWN_FRAMES;

                if (mode == Mode.Car)
                {
                    // During intro phases (0, 1) - abort immediately
                    if (animPhase == 0 || animPhase == 1)
                    {
                        // Get current dict and animation names
                        string dict;
                        string fAnim, mAnim;

                        if (_backseatCarSex)
                        {
                            dict = "random@drunk_driver_2";
                            RequestAnimDict(dict);
                            fAnim = "cardrunksex_intro_f";
                            mAnim = "cardrunksex_intro_m";
                        }
                        else
                        {
                        // Get current dict based on vehicle type
                        int camMode = Function.Call<int>(Hash.GET_FOLLOW_PED_CAM_VIEW_MODE);
                        bool isFirstPerson = (camMode == 4);
                        
                        if (vehicleAnimType == VehicleAnimType.Low)
                            dict = isFirstPerson ? VEH_LOW_FP_DICT : VEH_LOW_DICT;
                        else
                            dict = isFirstPerson ? VEH_NORM_FP_DICT : VEH_NORM_DICT;
                        
                        // Get current animation names based on phase and action
                        
                        if (animPhase == 0)
                        {
                            if (currentAction == "blowjob")
                            {
                                if (vehicleAnimType == VehicleAnimType.Low)
                                {
                                    fAnim = "low_car_prop_to_bj_p1_female";
                                    mAnim = "low_car_prop_to_bj_p1_player";
                                }
                                else
                                {
                                    fAnim = "proposition_to_bj_p1_prostitute";
                                    mAnim = "proposition_to_bj_p1_male";
                                }
                            }
                            else // sex
                            {
                                if (vehicleAnimType == VehicleAnimType.Low)
                                {
                                    fAnim = "low_car_prop_to_sex_p1_female";
                                    mAnim = "low_car_prop_to_sex_p1_player";
                                }
                                else
                                {
                                    fAnim = "proposition_to_sex_p1_prostitute";
                                    mAnim = "proposition_to_sex_p1_male";
                                }
                            }
                        }
                        else // phase 1
                        {
                            if (currentAction == "blowjob")
                            {
                                if (vehicleAnimType == VehicleAnimType.Low)
                                {
                                    fAnim = "low_car_prop_to_bj_p2_female";
                                    mAnim = "low_car_prop_to_bj_p2_player";
                                }
                                else
                                {
                                    fAnim = "proposition_to_bj_p2_prostitute";
                                    mAnim = "proposition_to_bj_p2_male";
                                }
                            }
                            else // sex
                            {
                                if (vehicleAnimType == VehicleAnimType.Low)
                                {
                                    fAnim = "low_car_prop_to_sex_p2_female";
                                    mAnim = "low_car_prop_to_sex_p2_player";
                                }
                                else
                                {
                                    fAnim = "proposition_to_sex_p2_prostitute";
                                    mAnim = "proposition_to_sex_p2_male";
                                }
                            }
                        }
                        } // end else (front seat abort anims)
                        
                        // Abort immediately: stop current intro anim on both peds
                        Function.Call(Hash.STOP_ANIM_TASK, girl,   dict, fAnim, -4.0f);
                        Function.Call(Hash.STOP_ANIM_TASK, player, dict, mAnim, -4.0f);

                        DetachDildoProp();
                        if (player.IsInVehicle() && player.CurrentVehicle != null)
                        {
                            player.CurrentVehicle.IsInteriorLightOn = interiorLightEnabled;
                        }

                        // Refund in prostitution or A-Life hooker mode
                        if ((!sandboxMode || _aLifeHookerServices) && lastPaidAmount > 0)
                        {
                            if (aLifeMode && _currentGirlKey != null)
                            {
                                ALifePedData dRef;
                                if (_aLifePeds.TryGetValue(_currentGirlKey, out dRef))
                                {
                                    dRef.Cash = Math.Max(0, dRef.Cash - lastPaidAmount);
                                    SaveALife();
                                }
                            }
                            Game.Player.Money += lastPaidAmount;
                            ShowHudStatus("~r~服務已中斷！~s~退款金額：~g~$" + lastPaidAmount, 3000);
                            lastPaidAmount = 0;
                        }
                        else
                        {
                            ShowHudStatus("~r~服務已中斷！", 3000);
                        }

                        // Snap both peds back to proposition loop immediately
                        if (_backseatCarSex)
                        {
                            const string DRUNK_DICT = "random@drunk_driver_2";
                            RequestAnimDict(DRUNK_DICT);
                            Function.Call(Hash.TASK_PLAY_ANIM, girl,   DRUNK_DICT, "cardrunkflirt_loop_f",
                                8.0f, -4.0f, -1, 1, 0.0f, false, false, false);
                            Function.Call(Hash.TASK_PLAY_ANIM, player, DRUNK_DICT, "cardrunkflirt_loop_m",
                                8.0f, -4.0f, -1, driveableCar ? 33 : 1, 0.0f, false, false, false);
                        }
                        else
                        {
                            string fLoop, mLoop;
                            if (vehicleAnimType == VehicleAnimType.Low)
                            { fLoop = "low_car_prop_loop_female"; mLoop = "low_car_prop_loop_player"; }
                            else
                            { fLoop = "proposition_loop_prostitute"; mLoop = "proposition_loop_male"; }
                            Function.Call(Hash.TASK_PLAY_ANIM, girl,   dict, fLoop,
                                8.0f, -4.0f, -1, 1, 0.0f, false, false, false);
                            Function.Call(Hash.TASK_PLAY_ANIM, player, dict, mLoop,
                                8.0f, -4.0f, -1, driveableCar ? 33 : 1, 0.0f, false, false, false);
                        }

                        _telemetry.SendStop();
                        _backseatCarSex       = false;
                        carPropActive         = true;
                        carPropPhase          = 1;
                        state                 = ScriptState.Idle;
                        animPhase             = 0;
                        animSpeed             = 1.0f;
                        _femaleAnimSpeedScale = 1.0f;
                        currentAction         = "";
                        menuLevel             = MenuLevel.Services;
                        menuIndex             = 0;
                        return;
                    }
                    // During loop phase (2) - trigger exit sequence
                    else if (animPhase == 2)
                    {
                        if (currentAction != "kissing" && currentAction != "facesitting")
                        {
                            // Open finish menu — player picks Pull Out or Finish Inside
                            menuLevel = MenuLevel.Finish;
                            menuIndex = 0;
                        }
                        else
                        {
                        // Trigger outro - reverse sequence from BJ back to proposition (flag 2 = hold last frame)
                        int camMode = Function.Call<int>(Hash.GET_FOLLOW_PED_CAM_VIEW_MODE);
                        bool isFirstPerson = (camMode == 4);
                        
                        string dict;
                        if (vehicleAnimType == VehicleAnimType.Low)
                            dict = isFirstPerson ? VEH_LOW_FP_DICT : VEH_LOW_DICT;
                        else
                            dict = isFirstPerson ? VEH_NORM_FP_DICT : VEH_NORM_DICT;
                        
                        string fExitP1, mExitP1;
                        
                        if (currentAction == "blowjob")
                        {
                            if (vehicleAnimType == VehicleAnimType.Low)
                            {
                                fExitP1 = "low_car_bj_to_prop_p1_female";
                                mExitP1 = "low_car_bj_to_prop_p1_player";
                            }
                            else
                            {
                                fExitP1 = "bj_to_proposition_p1_prostitute";
                                mExitP1 = "bj_to_proposition_p1_male";
                            }
                        }
                        else // sex
                        {
                            if (vehicleAnimType == VehicleAnimType.Low)
                            {
                                fExitP1 = "low_car_sex_to_prop_p1_female";
                                mExitP1 = "low_car_sex_to_prop_p1_player";
                            }
                            else
                            {
                                fExitP1 = "sex_to_proposition_p1_prostitute";
                                mExitP1 = "sex_to_proposition_p1_male";
                            }
                        }
                        
                        Function.Call(Hash.TASK_PLAY_ANIM, girl, dict, fExitP1,
                            8.0f, -4.0f, -1, 2, 0.0f, false, false, false);
                        Function.Call(Hash.TASK_PLAY_ANIM, player, dict, mExitP1,
                            8.0f, -4.0f, -1, driveableCar ? 34 : 2, 0.0f, false, false, false);

                        animPhase = 3; // Move to outro phase
                        _telemetry.SendStop(); // halt device as soon as outro begins
                        if (_usingController) { SetVibration(65535, 65535); _lastRumbleTime = Game.GameTime; _rumblePulseActive = true; _finishBurstFired = true; }
                        // Set wait time = now + exit anim duration (ms)
                        float exitP1Dur = Function.Call<float>(Hash.GET_ANIM_DURATION, dict, fExitP1);
                        animWaitTime = Game.GameTime + (int)(exitP1Dur * 1000f);
                        // DON'T clear currentAction yet - phases 3 and 4 need it for animation names
                        
                        ShowHudStatus("~g~FINISHING...", 2000);
                        } // end else (not A-Life)
                    }
                    // During exit phases (3, 4) - ignore input, let animations finish
                    else
                    {
                        // Do nothing, wait for exit animations to complete
                    }
                }
                else
                {
                    // Street sitting action: special phase handling
                    if (_streetSitPropActive && (streetActionIdx == 6 || streetActionIdx == 3 || streetActionIdx == 1 || streetActionIdx == 7 || streetActionIdx == 4 || streetActionIdx == 8 || streetActionIdx == 9))
                    {
                        if (animPhase <= 1)
                        {
                            // Abort during intro — refund and return to proposition loop
                            if ((!sandboxMode || _aLifeHookerServices) && lastPaidAmount > 0)
                            {
                                if (aLifeMode && _currentGirlKey != null)
                                {
                                    ALifePedData dRef;
                                    if (_aLifePeds.TryGetValue(_currentGirlKey, out dRef))
                                    {
                                        dRef.Cash = Math.Max(0, dRef.Cash - lastPaidAmount);
                                        SaveALife();
                                    }
                                }
                                Game.Player.Money += lastPaidAmount;
                                ShowHudStatus("~r~ABORTED! ~s~REFUNDED AMOUNT: ~g~$" + lastPaidAmount, 3000);
                                lastPaidAmount = 0;
                            }
                            else
                            {
                                ShowHudStatus("~r~ABORTED!", 3000);
                            }

                            // Return to proposition loop
                            DetachDildoProp();
                            _telemetry.SendStop();

                            // Facesitting / Reverse Cowgirl / Laying BJ / Laying Cowgirl: ensure screen is visible on abort
                            if ((streetActionIdx == 1 || streetActionIdx == 7 || streetActionIdx == 4 || streetActionIdx == 8 || streetActionIdx == 9)
                                && (Function.Call<bool>(Hash.IS_SCREEN_FADED_OUT) || Function.Call<bool>(Hash.IS_SCREEN_FADING_OUT)))
                            {
                                Function.Call(Hash.DO_SCREEN_FADE_IN, 500);
                            }
                            _streetOutroFadeAt = 0;

                            // Reposition back to proposition Z if action changed it
                            {
                                float propZ = _streetSitOriginPos.Z + (-0.855f);
                                Vector3 rp = player.Position;
                                float dZ = propZ - rp.Z;
                                if (Math.Abs(dZ) > 0.01f)
                                {
                                    Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, player,
                                        rp.X, rp.Y, propZ, false, false, false);
                                    if (girl != null && girl.Exists())
                                    {
                                        Vector3 rg = girl.Position;
                                        Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                                            rg.X, rg.Y, rg.Z + dZ, false, false, false);
                                    }
                                }
                            }

                            string pMD, pMA, pFD, pFA;
                            Vector3 pOff; float pHd; bool pSit; float pZO, pRX, pRH;
                            GetStreetPropAnimData(2, out pMD, out pMA, out pFD, out pFA,
                                out pOff, out pHd, out pSit, out pZO, out pRX, out pRH);
                            if (RequestAnimDict(pMD))
                            {
                                // Facesitting / Reverse Cowgirl / Laying BJ / Laying Cowgirl: reposition girl back to proposition offset
                                if ((streetActionIdx == 1 || streetActionIdx == 7 || streetActionIdx == 4 || streetActionIdx == 8 || streetActionIdx == 9) && girl != null && girl.Exists())
                                {
                                    Vector3 girlPropPos = player.GetOffsetPosition(pOff);
                                    Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                                        girlPropPos.X, girlPropPos.Y, girlPropPos.Z, false, false, false);
                                    girl.Heading = player.Heading + pHd;
                                }

                                Vector3 pp = player.Position;
                                Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, player, pMD, pMA,
                                    pp.X, pp.Y, pp.Z, pRX, 0f, player.Heading,
                                    8.0f, -4.0f, -1, 9, 0f, 2, 0);
                                Function.Call(Hash.TASK_PLAY_ANIM, girl, pFD, pFA,
                                    8.0f, -4.0f, -1, 9, 0f, false, false, false);
                            }

                            state = ScriptState.Idle;
                            animPhase = 0;
                            animSpeed = 1.0f;
                            _femaleAnimSpeedScale = 1.0f;
                            currentAction = "";
                            streetActionIdx = -1;
                            _streetSitPropPhase = 2;
                            menuLevel = _streetSitCategory;
                            menuIndex = 0;
                            keyCooldown = KEY_COOLDOWN_FRAMES * 3;
                            return;
                        }
                        if (animPhase >= 3)
                            return; // outro running, ignore
                        // animPhase == 2: fall through to existing code (opens Finish menu)
                    }

                    // Street mode: during intro phase, refund on abort
                    bool _refundShown = false;
                    if (animPhase < 2 && (!sandboxMode || _aLifeHookerServices) && lastPaidAmount > 0)
                    {
                        if (aLifeMode && _currentGirlKey != null)
                        {
                            ALifePedData dRef;
                            if (_aLifePeds.TryGetValue(_currentGirlKey, out dRef))
                            {
                                dRef.Cash = Math.Max(0, dRef.Cash - lastPaidAmount);
                                SaveALife();
                            }
                        }
                        Game.Player.Money += lastPaidAmount;
                        ShowHudStatus("~r~ABORTED! ~s~REFUNDED AMOUNT: ~g~$" + lastPaidAmount, 3000);
                        lastPaidAmount = 0;
                        _refundShown = true;
                    }
                    // Kissing outro running — ignore input, let it play out
                    if (streetActionIdx == 0 && animPhase == 3)
                        return;
                    // Standing BJ / Wall Sex / Doggy outro/finish running — ignore input
                    if ((streetActionIdx == 2 || streetActionIdx == 5 || streetActionIdx == 10 || streetActionIdx == 11) && animPhase >= 3)
                        return;
                    // Standing BJ / Wall Sex / Doggy abort during fade — ensure screen is visible
                    if ((streetActionIdx == 2 || streetActionIdx == 5 || streetActionIdx == 10 || streetActionIdx == 11) && (animPhase < 2 || animPhase == 100)
                        && (Function.Call<bool>(Hash.IS_SCREEN_FADED_OUT) || Function.Call<bool>(Hash.IS_SCREEN_FADING_OUT)))
                    {
                        Function.Call(Hash.DO_SCREEN_FADE_IN, 500);
                    }

                    // Standing service abort during intro: clean up and reopen menu
                    if (animPhase < 2 && (streetActionIdx == 0 || streetActionIdx == 2 || streetActionIdx == 5 || streetActionIdx == 10 || streetActionIdx == 11))
                    {
                        if (!_refundShown) // refund block already showed its own message
                            ShowHudStatus("~r~ABORTED!", 3000);
                        // Stop any intro anims
                        player.Task.ClearAll();
                        if (girl != null && girl.Exists()) girl.Task.ClearAll();
                        DetachDildoProp();
                        _telemetry.SendStop();
                        // Reset state and reopen Standing Services
                        state = ScriptState.Idle;
                        animPhase = 0;
                        animSpeed = 1.0f;
                        _femaleAnimSpeedScale = 1.0f;
                        currentAction = "";
                        streetActionIdx = -1;
                        _kissingReverse = false;
                        _standingBjReverse = false;
                        _rdoggyReverse = false;
                        _sitCowgirlPulloutReverse = false;
                        _standingBjSwallowAt = 0;
                        player.IsPositionFrozen = false;
                        Function.Call(Hash.SET_ENTITY_COLLISION, player, true, true);
                        if (girl != null && girl.Exists())
                        {
                            girl.IsPositionFrozen = false;
                            Function.Call(Hash.SET_ENTITY_COLLISION, girl, true, true);
                            Function.Call(Hash.SET_ENTITY_NO_COLLISION_ENTITY, girl, player, true);
                            Function.Call(Hash.SET_PED_KEEP_TASK, girl, false);
                            Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, girl, true);
                            float dist = girl.Position.DistanceTo(player.Position);
                            float speed = (dist > 7f) ? 3f : 1f;
                            Function.Call(Hash.TASK_FOLLOW_TO_OFFSET_OF_ENTITY,
                                girl, player, 0f, -1f, 0f, speed, -1, 1.5f, true);
                        }
                        Function.Call(Hash.SET_PLAYER_CAN_DO_DRIVE_BY, Game.Player, true);
                        Function.Call(Hash.SET_EVERYONE_IGNORE_PLAYER, Game.Player, false);
                        negotiateMode = false;
                        menuLevel = MenuLevel.StandingServices;
                        menuIndex = 0;
                        keyCooldown = KEY_COOLDOWN_FRAMES * 3;
                        return;
                    }

                    // Play finish voice line when ending from loop (not aborts)
                    if (animPhase >= 2)
                    {
                        if (streetActionIdx != 0 && streetActionIdx != 1) // 0=Kissing, 1=Facesitting — no finish menu
                        {
                            menuLevel = MenuLevel.Finish;
                            menuIndex = 0;
                            return; // wait for dialogue choice before ending
                        }
                        // Kissing: start outro — set playhead to loop end, positive speed, let it run out
                        if (streetActionIdx == 0)
                        {
                            _kissingReverse = false;
                            Function.Call(Hash.SET_ENTITY_ANIM_CURRENT_TIME, player, currentMaleDict, currentMaleAnim, 0.500f);
                            Function.Call(Hash.SET_ENTITY_ANIM_CURRENT_TIME, girl, currentFemaleDict, currentFemaleAnim, 0.500f);
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, currentMaleDict, currentMaleAnim, animSpeed);
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, currentFemaleDict, currentFemaleAnim, animSpeed);
                            animPhase = 3;
                            if (_usingController) { SetVibration(65535, 65535); _lastRumbleTime = Game.GameTime; _rumblePulseActive = true; _finishBurstFired = true; }
                            ShowHudStatus("~g~FINISHING...", 2000);
                            return;
                        }
                        // Facesitting with prop active: start the outro transition instead of EndAnimation
                        if (streetActionIdx == 1 && _streetSitPropActive)
                        {
                            string exMD, exMA, exFD, exFA;
                            Vector3 exOff; float exHd; bool exSit; float exZO, exRX, exRH;
                            GetStreetPropIntroExitAnimData(2, streetActionIdx, out exMD, out exMA, out exFD, out exFA,
                                out exOff, out exHd, out exSit, out exZO, out exRX, out exRH);
                            if (RequestAnimDict(exFD))
                            {
                                // Reposition girl to vanilla transition offset before playing outro
                                if (girl != null && girl.Exists())
                                {
                                    Vector3 girlOutroPos = player.GetOffsetPosition(new Vector3(0.73f, -0.77f, -0.3f));
                                    Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                                        girlOutroPos.X, girlOutroPos.Y, girlOutroPos.Z, false, false, false);
                                    girl.Heading = player.Heading;
                                }
                                // Only play female outro P1 — male stays in loop
                                Function.Call(Hash.TASK_PLAY_ANIM, girl, exFD, exFA,
                                    8.0f, -4.0f, -1, 2, 0f, false, false, false);
                            }
                            animPhase = 3;
                            _telemetry.SendStop();
                            if (_usingController) { SetVibration(65535, 65535); _lastRumbleTime = Game.GameTime; _rumblePulseActive = true; _finishBurstFired = true; }
                            float exDur = Function.Call<float>(Hash.GET_ANIM_DURATION, exFD, exFA);
                            animWaitTime = Game.GameTime + (int)(exDur * 1000f);
                            ShowHudStatus("~g~FINISHING...", 2000);
                            return;
                        }
                        if (_usingController) { SetVibration(65535, 65535); _lastRumbleTime = Game.GameTime; _rumblePulseActive = true; _finishBurstFired = true; }
                        PlayFinishSounds();
                    }
                    EndAnimation();
                }
            }

            // Execute the actual finish once the Finish menu choice is resolved
            if (_finishExecutePending && menuLevel == MenuLevel.None && Game.GameTime >= _finishExecuteAt)
            {
                _finishExecutePending = false;

                // BJ finish: cut sounds and voice immediately (both pull-out and inside)
                if (currentAction == "blowjob")
                {
                    try { if (_bjPlayer != null) _bjPlayer.Stop(); } catch { }
                    _bjSoundBusyUntil    = 0;
                    _bjThroatPending     = false;
                    _throatSpeedLocked   = false;
                    _throatLockAt        = 0;
                    _throatSequenceEndAt = 0;
                    if (girl != null && girl.Exists())
                        Function.Call(Hash.STOP_CURRENT_PLAYING_AMBIENT_SPEECH, girl);
                }

                if (mode == Mode.Car && animPhase == 2)
                {
                    string dictF = string.Empty, fExF = string.Empty, mExF = string.Empty;
                    if (_backseatCarSex)
                    {
                        dictF = "random@drunk_driver_2";
                        RequestAnimDict(dictF);
                        fExF = "cardrunksex_outro_f";
                        mExF = "cardrunksex_outro_m";
                    }
                    // Backseat car sex pull-out: play cardrunksex_intro reversed → finish (single anim, no P1/P2 split)
                    if (!_finishInside && _backseatCarSex)
                    {
                        const string DRUNK_PO = "random@drunk_driver_2";
                        if (RequestAnimDict(DRUNK_PO))
                        {
                            Function.Call(Hash.TASK_PLAY_ANIM, girl, DRUNK_PO, "cardrunksex_intro_f", 8.0f, -4.0f, -1, 2, 1.0f, false, false, false);
                            Function.Call(Hash.TASK_PLAY_ANIM, player, DRUNK_PO, "cardrunksex_intro_m", 8.0f, -4.0f, -1, driveableCar ? 34 : 2, 1.0f, false, false, false);
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, DRUNK_PO, "cardrunksex_intro_f", -1.0f);
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, DRUNK_PO, "cardrunksex_intro_m", -1.0f);
                        }
                        _carPulloutReverse = true;
                        animPhase = 3;
                        _telemetry.SendStop();
                        if (_usingController) { SetVibration(65535, 65535); _lastRumbleTime = Game.GameTime; _rumblePulseActive = true; _finishBurstFired = true; }
                        float bsPODur = Function.Call<float>(Hash.GET_ANIM_DURATION, DRUNK_PO, "cardrunksex_intro_f");
                        animWaitTime = Game.GameTime + (int)(bsPODur * 1000f);
                        ShowHudStatus("~g~FINISHING...", 2000);
                        return;
                    }
                    if (!_backseatCarSex)
                    {
                    int camModeF = Function.Call<int>(Hash.GET_FOLLOW_PED_CAM_VIEW_MODE);
                    bool isFPF   = (camModeF == 4);
                    dictF = vehicleAnimType == VehicleAnimType.Low
                        ? (isFPF ? VEH_LOW_FP_DICT : VEH_LOW_DICT)
                        : (isFPF ? VEH_NORM_FP_DICT : VEH_NORM_DICT);
                    if (currentAction == "blowjob")
                    {
                        if (vehicleAnimType == VehicleAnimType.Low)
                        { fExF = "low_car_bj_to_prop_p1_female"; mExF = "low_car_bj_to_prop_p1_player"; }
                        else
                        { fExF = "bj_to_proposition_p1_prostitute"; mExF = "bj_to_proposition_p1_male"; }
                    }
                    else
                    {
                        if (vehicleAnimType == VehicleAnimType.Low)
                        { fExF = "low_car_sex_to_prop_p1_female"; mExF = "low_car_sex_to_prop_p1_player"; }
                        else
                        { fExF = "sex_to_proposition_p1_prostitute"; mExF = "sex_to_proposition_p1_male"; }
                    }
                    }
                    // Car BJ/Sex pull-out: play proposition_to_[action]_p2 reversed → p1 reversed → finish (no blackscreen)
                    if (!_finishInside && !_backseatCarSex && (currentAction == "blowjob" || currentAction == "sex"))
                    {
                        bool isBjPO = currentAction == "blowjob";
                        bool isLowPO = vehicleAnimType == VehicleAnimType.Low;
                        string fP2r = isBjPO
                            ? (isLowPO ? "low_car_prop_to_bj_p2_female" : "proposition_to_bj_p2_prostitute")
                            : (isLowPO ? "low_car_prop_to_sex_p2_female" : "proposition_to_sex_p2_prostitute");
                        string mP2r = isBjPO
                            ? (isLowPO ? "low_car_prop_to_bj_p2_player" : "proposition_to_bj_p2_male")
                            : (isLowPO ? "low_car_prop_to_sex_p2_player" : "proposition_to_sex_p2_male");
                        if (RequestAnimDict(dictF))
                        {
                            Function.Call(Hash.TASK_PLAY_ANIM, girl, dictF, fP2r, 8.0f, -4.0f, -1, 2, 1.0f, false, false, false);
                            Function.Call(Hash.TASK_PLAY_ANIM, player, dictF, mP2r, 8.0f, -4.0f, -1, driveableCar ? 34 : 2, 1.0f, false, false, false);
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, dictF, fP2r, -1.0f);
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, dictF, mP2r, -1.0f);
                        }
                        _carPulloutReverse = true;
                        animPhase = 3;
                        _telemetry.SendStop();
                        if (_usingController) { SetVibration(65535, 65535); _lastRumbleTime = Game.GameTime; _rumblePulseActive = true; _finishBurstFired = true; }
                        float p2rDur = Function.Call<float>(Hash.GET_ANIM_DURATION, dictF, fP2r);
                        animWaitTime = Game.GameTime + (int)(p2rDur * 1000f);
                        ShowHudStatus("~g~FINISHING...", 2000);
                        return;
                    }
                    Function.Call(Hash.TASK_PLAY_ANIM, girl, dictF, fExF, 8.0f, -4.0f, -1, 2, 0.0f, false, false, false);
                    Function.Call(Hash.TASK_PLAY_ANIM, player, dictF, mExF, 8.0f, -4.0f, -1, driveableCar ? 34 : 2, 0.0f, false, false, false);
                    animPhase = 3;
                    _telemetry.SendStop();
                    if (_usingController) { SetVibration(65535, 65535); _lastRumbleTime = Game.GameTime; _rumblePulseActive = true; _finishBurstFired = true; }
                    float exDurF = Function.Call<float>(Hash.GET_ANIM_DURATION, dictF, fExF);
                    animWaitTime = Game.GameTime + (int)(exDurF * 1000f);
                    ShowHudStatus(_finishInside ? "~p~FINISHING INSIDE...~s~" : "~g~FINISHING...", 2000);
                }
                else if (mode == Mode.Street && _streetSitPropActive && (streetActionIdx == 6 || streetActionIdx == 3 || streetActionIdx == 1 || streetActionIdx == 7 || streetActionIdx == 4 || streetActionIdx == 8 || streetActionIdx == 9) && animPhase == 2)
                {
                    // Sitting Cowgirl / Sitting BJ pull-out: play proposition_to_sex P2 reversed → then P1 reversed → finish
                    if (!_finishInside && (streetActionIdx == 6 || streetActionIdx == 3))
                    {
                        string p2MD, p2MA, p2FD, p2FA;
                        Vector3 p2Off; float p2Hd; bool p2Sit; float p2ZO, p2RX, p2RH;
                        GetStreetPropIntroExitAnimData(1, streetActionIdx, out p2MD, out p2MA, out p2FD, out p2FA,
                            out p2Off, out p2Hd, out p2Sit, out p2ZO, out p2RX, out p2RH);
                        if (RequestAnimDict(p2MD) && (p2MD == p2FD || RequestAnimDict(p2FD)))
                        {
                            Vector3 pp = player.Position;
                            Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, player, p2MD, p2MA,
                                pp.X, pp.Y, pp.Z, p2RX, 0f, player.Heading,
                                8.0f, -4.0f, -1, 2, 1.0f, 2, 0);
                            Function.Call(Hash.TASK_PLAY_ANIM, girl, p2FD, p2FA,
                                8.0f, -4.0f, -1, 2, 1.0f, false, false, false);
                            // Per-tick enforcement in RunStreetSitAnimation keeps speed at -1f every frame
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, p2MD, p2MA, -1.0f);
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, p2FD, p2FA, -1.0f);
                        }
                        _sitCowgirlPulloutReverse = true;
                        animPhase = 3;
                        _telemetry.SendStop();
                        if (_usingController) { SetVibration(65535, 65535); _lastRumbleTime = Game.GameTime; _rumblePulseActive = true; _finishBurstFired = true; }
                        float p2Dur = Function.Call<float>(Hash.GET_ANIM_DURATION, p2MD, p2MA);
                        animWaitTime = Game.GameTime + (int)(p2Dur * 1000f);
                        ShowHudStatus("~g~FINISHING...", 2000);
                        return;
                    }
                    // Reverse Cowgirl pull-out: P2 reversed → P1 reversed → finish (black screen same timing as finish-inside)
                    if (!_finishInside && streetActionIdx == 7)
                    {
                        string p2MD, p2MA, p2FD, p2FA;
                        Vector3 p2Off; float p2Hd; bool p2Sit; float p2ZO, p2RX, p2RH;
                        GetStreetPropIntroExitAnimData(1, streetActionIdx, out p2MD, out p2MA, out p2FD, out p2FA,
                            out p2Off, out p2Hd, out p2Sit, out p2ZO, out p2RX, out p2RH);
                        // Reposition girl to reverse-cowgirl offset (same as finish-inside P1)
                        if (girl != null && girl.Exists())
                        {
                            Vector3 girlRevPos = player.GetOffsetPosition(new Vector3(-0.83f, 0.4f, 0.02f));
                            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                                girlRevPos.X, girlRevPos.Y, girlRevPos.Z, false, false, false);
                            girl.Heading = player.Heading + 180f;
                        }
                        if (RequestAnimDict(p2MD) && (p2MD == p2FD || RequestAnimDict(p2FD)))
                        {
                            Vector3 pp = player.Position;
                            Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, player, p2MD, p2MA,
                                pp.X, pp.Y, pp.Z, p2RX, 0f, player.Heading,
                                8.0f, -4.0f, -1, 2, 1.0f, 2, 0);
                            Function.Call(Hash.TASK_PLAY_ANIM, girl, p2FD, p2FA,
                                8.0f, -4.0f, -1, 2, 1.0f, false, false, false);
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, p2MD, p2MA, -1.0f);
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, p2FD, p2FA, -1.0f);
                        }
                        _sitCowgirlPulloutReverse = true;
                        animPhase = 3;
                        _telemetry.SendStop();
                        if (_usingController) { SetVibration(65535, 65535); _lastRumbleTime = Game.GameTime; _rumblePulseActive = true; _finishBurstFired = true; }
                        float p2Dur = Function.Call<float>(Hash.GET_ANIM_DURATION, p2MD, p2MA);
                        animWaitTime = Game.GameTime + (int)(p2Dur * 1000f);
                        // Fade to black 500ms before P2rev ends (same timing as finish-inside)
                        _streetOutroFadeAt = Game.GameTime + (int)(p2Dur * 1000f) - 500;
                        ShowHudStatus("~g~FINISHING...", 2000);
                        return;
                    }
                    // Laying BJ pull-out: female plays proposition_to_bj_p2 reversed; male stays in loop (sequential, like finish-inside P1)
                    if (!_finishInside && streetActionIdx == 4)
                    {
                        string p2MD, p2MA, p2FD, p2FA;
                        Vector3 p2Off; float p2Hd; bool p2Sit; float p2ZO, p2RX, p2RH;
                        GetStreetPropIntroExitAnimData(1, streetActionIdx, out p2MD, out p2MA, out p2FD, out p2FA,
                            out p2Off, out p2Hd, out p2Sit, out p2ZO, out p2RX, out p2RH);
                        if (girl != null && girl.Exists() && RequestAnimDict(p2FD))
                        {
                            Function.Call(Hash.TASK_PLAY_ANIM, girl, p2FD, p2FA,
                                8.0f, -4.0f, -1, 2, 1.0f, false, false, false);
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, p2FD, p2FA, -1.0f);
                        }
                        _sitCowgirlPulloutReverse = true;
                        animPhase = 3;
                        _telemetry.SendStop();
                        if (_usingController) { SetVibration(65535, 65535); _lastRumbleTime = Game.GameTime; _rumblePulseActive = true; _finishBurstFired = true; }
                        float p2Dur4 = Function.Call<float>(Hash.GET_ANIM_DURATION, p2FD, p2FA);
                        animWaitTime = Game.GameTime + (int)(p2Dur4 * 1000f);
                        ShowHudStatus("~g~FINISHING...", 2000);
                        return;
                    }
                    // Vanilla Laying Cowgirl pull-out: play cardrunksex_intro reversed (P2) → fade to black (3.5s) → phase 6
                    if (!_finishInside && streetActionIdx == 8 && !(_sinkraCowgirlAvailable && sinkraReplaceLaying))
                    {
                        string p2MD, p2MA, p2FD, p2FA;
                        Vector3 p2Off; float p2Hd; bool p2Sit; float p2ZO, p2RX, p2RH;
                        GetStreetPropIntroExitAnimData(1, streetActionIdx, out p2MD, out p2MA, out p2FD, out p2FA,
                            out p2Off, out p2Hd, out p2Sit, out p2ZO, out p2RX, out p2RH);
                        if (RequestAnimDict(p2MD))
                        {
                            Vector3 pp = player.Position;
                            Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, player, p2MD, p2MA,
                                pp.X, pp.Y, pp.Z, p2RX, 0f, player.Heading,
                                8.0f, -4.0f, -1, 2, 1.0f, 2, 0);
                            Function.Call(Hash.TASK_PLAY_ANIM, girl, p2FD, p2FA,
                                8.0f, -4.0f, -1, 2, 1.0f, false, false, false);
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, p2MD, p2MA, -1.0f);
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, p2FD, p2FA, -1.0f);
                        }
                        _sitCowgirlPulloutReverse = true;
                        animPhase = 3;
                        _telemetry.SendStop();
                        if (_usingController) { SetVibration(65535, 65535); _lastRumbleTime = Game.GameTime; _rumblePulseActive = true; _finishBurstFired = true; }
                        float p2Dur8 = Function.Call<float>(Hash.GET_ANIM_DURATION, p2MD, p2MA);
                        animWaitTime = Game.GameTime + (int)(p2Dur8 * 1000f);
                        _streetOutroFadeAt = Game.GameTime + 1500;
                        ShowHudStatus("~g~FINISHING...", 2000);
                        return;
                    }
                    // Vanilla Laying Rev Cowgirl pull-out: play cardrunksex_intro reversed → fade to black (3.5s) → phase 6
                    if (!_finishInside && streetActionIdx == 9 && !(_sinkraCowgirlAvailable && sinkraReplaceRevLaying))
                    {
                        string p2MD, p2MA, p2FD, p2FA;
                        Vector3 p2Off; float p2Hd; bool p2Sit; float p2ZO, p2RX, p2RH;
                        GetStreetPropIntroExitAnimData(1, streetActionIdx, out p2MD, out p2MA, out p2FD, out p2FA,
                            out p2Off, out p2Hd, out p2Sit, out p2ZO, out p2RX, out p2RH);
                        if (RequestAnimDict(p2MD))
                        {
                            Vector3 pp = player.Position;
                            Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, player, p2MD, p2MA,
                                pp.X, pp.Y, pp.Z, p2RX, 0f, player.Heading,
                                8.0f, -4.0f, -1, 2, 1.0f, 2, 0);
                            Function.Call(Hash.TASK_PLAY_ANIM, girl, p2FD, p2FA,
                                8.0f, -4.0f, -1, 2, 1.0f, false, false, false);
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, p2MD, p2MA, -1.0f);
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, p2FD, p2FA, -1.0f);
                        }
                        _sitCowgirlPulloutReverse = true;
                        animPhase = 3;
                        _telemetry.SendStop();
                        if (_usingController) { SetVibration(65535, 65535); _lastRumbleTime = Game.GameTime; _rumblePulseActive = true; _finishBurstFired = true; }
                        float p2Dur9 = Function.Call<float>(Hash.GET_ANIM_DURATION, p2MD, p2MA);
                        animWaitTime = Game.GameTime + (int)(p2Dur9 * 1000f);
                        _streetOutroFadeAt = Game.GameTime + 1000;
                        ShowHudStatus("~g~FINISHING...", 2000);
                        return;
                    }
                    // Sinkra Laying Cowgirl / Sinkra Laying Rev Cowgirl pull-out: female plays P2 reversed; male stays in loop
                    if (!_finishInside && ((streetActionIdx == 8 && _sinkraCowgirlAvailable && sinkraReplaceLaying) || (streetActionIdx == 9 && _sinkraCowgirlAvailable && sinkraReplaceRevLaying)))
                    {
                        string p2MD, p2MA, p2FD, p2FA;
                        Vector3 p2Off; float p2Hd; bool p2Sit; float p2ZO, p2RX, p2RH;
                        GetStreetPropIntroExitAnimData(1, streetActionIdx, out p2MD, out p2MA, out p2FD, out p2FA,
                            out p2Off, out p2Hd, out p2Sit, out p2ZO, out p2RX, out p2RH);
                        if (girl != null && girl.Exists() && RequestAnimDict(p2FD))
                        {
                            // Reposition girl to anim origin before reversing, matching finish-inside repositioning
                            if (streetActionIdx == 8)
                            {
                                Vector3 gPos8 = player.GetOffsetPosition(p2Off);
                                float gZ8 = _streetSitOriginPos.Z + (-0.855f);
                                Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl, gPos8.X, gPos8.Y, gZ8, false, false, false);
                                girl.Heading = player.Heading + p2Hd;
                            }
                            else if (streetActionIdx == 9)
                            {
                                Vector3 gPos9 = player.GetOffsetPosition(new Vector3(-0.83f, 0.0f, 0.02f));
                                float gZ9 = _streetSitOriginPos.Z + (-0.855f);
                                Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl, gPos9.X, gPos9.Y, gZ9, false, false, false);
                                girl.Heading = player.Heading + 180f;
                            }
                            Function.Call(Hash.TASK_PLAY_ANIM, girl, p2FD, p2FA,
                                8.0f, -4.0f, -1, 2, 1.0f, false, false, false);
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, p2FD, p2FA, -1.0f);
                        }
                        _sitCowgirlPulloutReverse = true;
                        animPhase = 3;
                        _telemetry.SendStop();
                        if (_usingController) { SetVibration(65535, 65535); _lastRumbleTime = Game.GameTime; _rumblePulseActive = true; _finishBurstFired = true; }
                        float p2Dur89s = Function.Call<float>(Hash.GET_ANIM_DURATION, p2FD, p2FA);
                        animWaitTime = Game.GameTime + (int)(p2Dur89s * 1000f);
                        ShowHudStatus("~g~FINISHING...", 2000);
                        return;
                    }
                    // Street sitting action: start action→prop outro transition
                    string exMD, exMA, exFD, exFA;
                    Vector3 exOff; float exHd; bool exSit; float exZO, exRX, exRH;
                    GetStreetPropIntroExitAnimData(2, streetActionIdx, out exMD, out exMA, out exFD, out exFA,
                        out exOff, out exHd, out exSit, out exZO, out exRX, out exRH);
                    if (RequestAnimDict(exMD) && (exMD == exFD || RequestAnimDict(exFD)))
                    {
                        // Reverse Cowgirl: reposition girl to reverse offset + flip heading for outro transition
                        if (streetActionIdx == 7 && girl != null && girl.Exists())
                        {
                            Vector3 girlOutroPos = player.GetOffsetPosition(new Vector3(-0.83f, 0.4f, 0.02f));
                            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                                girlOutroPos.X, girlOutroPos.Y, girlOutroPos.Z, false, false, false);
                            girl.Heading = player.Heading + 180f;
                        }
                        bool sinkraLaying8 = (streetActionIdx == 8 && _sinkraCowgirlAvailable && sinkraReplaceLaying);
                        bool sinkraLaying9 = (streetActionIdx == 9 && _sinkraCowgirlAvailable && sinkraReplaceRevLaying);
                        // Sinkra Laying Cowgirl: reposition girl to sitting cowgirl offset for outro transition
                        if (sinkraLaying8 && girl != null && girl.Exists())
                        {
                            Vector3 girlOutroPos = player.GetOffsetPosition(exOff);
                            float propZ = _streetSitOriginPos.Z + (-0.855f);
                            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                                girlOutroPos.X, girlOutroPos.Y, propZ, false, false, false);
                            girl.Heading = player.Heading + exHd;
                        }
                        // Sinkra Laying Rev Cowgirl: reposition girl to reverse cowgirl offset for outro transition
                        if (sinkraLaying9 && girl != null && girl.Exists())
                        {
                            Vector3 girlOutroPos = player.GetOffsetPosition(new Vector3(-0.83f, 0.0f, 0.02f));
                            float propZ = _streetSitOriginPos.Z + (-0.855f);
                            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                                girlOutroPos.X, girlOutroPos.Y, propZ, false, false, false);
                            girl.Heading = player.Heading + 180f;
                        }
                        if (streetActionIdx == 4 || sinkraLaying8 || sinkraLaying9)
                        {
                            // Laying BJ / Sinkra Laying Cowgirl: sequential outro — only play female P1, male stays in loop
                            Function.Call(Hash.TASK_PLAY_ANIM, girl, exFD, exFA,
                                8.0f, -4.0f, -1, 2, 0f, false, false, false);
                        }
                        else
                        {
                            Vector3 pp = player.Position;
                            Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, player, exMD, exMA,
                                pp.X, pp.Y, pp.Z, exRX, 0f, player.Heading,
                                8.0f, -4.0f, -1, 2, 0f, 2, 0);
                            Function.Call(Hash.TASK_PLAY_ANIM, girl, exFD, exFA,
                                8.0f, -4.0f, -1, 2, 0f, false, false, false);
                        }
                    }
                    animPhase = 3;
                    _telemetry.SendStop();
                    if (_usingController) { SetVibration(65535, 65535); _lastRumbleTime = Game.GameTime; _rumblePulseActive = true; _finishBurstFired = true; }
                    float exDur = Function.Call<float>(Hash.GET_ANIM_DURATION, exMD, exMA);
                    {
                        bool sinkraLaying8 = (streetActionIdx == 8 && _sinkraCowgirlAvailable && sinkraReplaceLaying);
                        bool sinkraLaying9 = (streetActionIdx == 9 && _sinkraCowgirlAvailable && sinkraReplaceRevLaying);
                        if (streetActionIdx == 4 || sinkraLaying8 || sinkraLaying9)
                        {
                            // Laying BJ / Sinkra Laying Cowgirl/Rev sequential outro: timer = female P1 only
                            exDur = Function.Call<float>(Hash.GET_ANIM_DURATION, exFD, exFA);
                        }
                        // Sinkra Laying Rev Cowgirl: override to 6s — let female outro play naturally before blackscreen
                        if (sinkraLaying9) exDur = 6.0f;
                        else if (exMD != exFD) { float exFDur = Function.Call<float>(Hash.GET_ANIM_DURATION, exFD, exFA); exDur = Math.Max(exDur, exFDur); }
                    }
                    animWaitTime = Game.GameTime + (int)(exDur * 1000f);
                    // Reverse Cowgirl: fade to black 500ms before P1 outro ends (so screen is black when P2 starts)
                    if (streetActionIdx == 7) _streetOutroFadeAt = Game.GameTime + (int)(exDur * 1000f) - 500;
                    // Vanilla Laying Cowgirl: fade to black 3s into the outro
                    if (streetActionIdx == 8 && !(_sinkraCowgirlAvailable && sinkraReplaceLaying)) _streetOutroFadeAt = Game.GameTime + 3000;
                    // Vanilla Laying Reversed Cowgirl: fade to black 2.7s into the outro
                    if (streetActionIdx == 9 && !(_sinkraCowgirlAvailable && sinkraReplaceRevLaying)) _streetOutroFadeAt = Game.GameTime + 2700;
                    ShowHudStatus(_finishInside ? "~p~FINISHING INSIDE...~s~" : "~g~FINISHING...", 2000);
                }
                else if (mode == Mode.Street && streetActionIdx == 2 && animPhase == 2)
                {
                    // Standing BJ: start finish delay
                    if (_usingController) { SetVibration(65535, 65535); _lastRumbleTime = Game.GameTime; _rumblePulseActive = true; _finishBurstFired = true; }
                    _telemetry.SendStop();
                    // Pull-out: stop loop anims immediately and play pull-out upper-body anims during the 3s delay
                    if (!_finishInside)
                    {
                        // Stop only the player's loop — girl's loop keeps running so her lower body
                        // stays kneeling. idle_b is layered on top as an upper-body overlay only.
                        Function.Call(Hash.STOP_ANIM_TASK, player, currentMaleDict, currentMaleAnim, -4.0f);
                        if (RequestAnimDict("switch@trevor@jerking_off"))
                            Function.Call(Hash.TASK_PLAY_ANIM, player, "switch@trevor@jerking_off", "trev_jerking_off_exit",
                                8.0f, -4.0f, -1, 50, 0f, false, false, false); // upper-body(16) + secondary(32) + hold-last-frame(2)
                        if (RequestAnimDict("amb@code_human_cross_road@female@idle_a"))
                            Function.Call(Hash.TASK_PLAY_ANIM, girl, "amb@code_human_cross_road@female@idle_a", "idle_b",
                                8.0f, -4.0f, -1, 50, 0f, false, false, false);
                    }
                    animWaitTime = Game.GameTime + 3000;
                    animPhase = 3;
                    ShowHudStatus(_finishInside ? "~p~FINISHING INSIDE...~s~" : "~g~FINISHING...", 3000);
                }
                else if (mode == Mode.Street && streetActionIdx == 5 && animPhase == 2)
                {
                    // Wall Sex: start finish delay
                    if (_usingController) { SetVibration(65535, 65535); _lastRumbleTime = Game.GameTime; _rumblePulseActive = true; _finishBurstFired = true; }
                    _telemetry.SendStop();
                    // Pull-out: stop player loop, step back 5 units, turn 90° right, play full-body pull-out anim
                    if (!_finishInside)
                    {
                        Function.Call(Hash.STOP_ANIM_TASK, player, currentMaleDict, currentMaleAnim, -4.0f);
                        // Freeze the female's wall-sex loop in place, then layer idle_c on top (unfreezed)
                        Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, currentFemaleDict, currentFemaleAnim, 0f);
                        if (RequestAnimDict("amb@code_human_cross_road@female@idle_a"))
                            Function.Call(Hash.TASK_PLAY_ANIM, girl, "amb@code_human_cross_road@female@idle_a", "idle_c",
                                8.0f, -4.0f, -1, 50, 0f, false, false, false); // upper-body(16) + secondary(32) + hold-last-frame(2)
                        // Move player 1 unit back (away from the wall) and face 90° to the right
                        Vector3 pulloutPos = player.GetOffsetPosition(new Vector3(0f, -0.70f, 0f));
                        Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, player,
                            pulloutPos.X, pulloutPos.Y, pulloutPos.Z, false, false, false);
                        player.Heading = player.Heading - 90f;
                        if (RequestAnimDict("switch@trevor@jerking_off"))
                            Function.Call(Hash.TASK_PLAY_ANIM, player, "switch@trevor@jerking_off", "trev_jerking_off_exit",
                                8.0f, -4.0f, -1, 2, 0f, false, false, false); // flag 2 = hold-last-frame, full body
                    }
                    animWaitTime = Game.GameTime + 3000;
                    animPhase = 3;
                    ShowHudStatus(_finishInside ? "~p~FINISHING INSIDE...~s~" : "~g~FINISHING...", 3000);
                }
                else if (mode == Mode.Street && (streetActionIdx == 10 || streetActionIdx == 11) && animPhase == 2)
                {
                    // Doggy / Rough Doggy: start finish delay
                    if (_usingController) { SetVibration(65535, 65535); _lastRumbleTime = Game.GameTime; _rumblePulseActive = true; _finishBurstFired = true; }
                    _telemetry.SendStop();
                    // Pull-out: stop player loop, freeze female, step back 0.5, rotate 90° right, play full-body pull-out anim
                    if (!_finishInside)
                    {
                        Function.Call(Hash.STOP_ANIM_TASK, player, currentMaleDict, currentMaleAnim, -4.0f);
                        Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, currentFemaleDict, currentFemaleAnim, 0f);
                        Vector3 pulloutPos = player.GetOffsetPosition(new Vector3(0f, -0.70f, 0f));
                        Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, player,
                            pulloutPos.X, pulloutPos.Y, pulloutPos.Z, false, false, false);
                        player.Heading = player.Heading - 90f;
                        if (RequestAnimDict("switch@trevor@jerking_off"))
                            Function.Call(Hash.TASK_PLAY_ANIM, player, "switch@trevor@jerking_off", "trev_jerking_off_exit",
                                8.0f, -4.0f, -1, 2, 0f, false, false, false); // flag 2 = hold-last-frame, full body
                    }
                    animWaitTime = Game.GameTime + 3000;
                    animPhase = 3;
                    ShowHudStatus(_finishInside ? "~p~FINISHING INSIDE...~s~" : "~g~FINISHING...", 3000);
                }
                else // street or fallback
                {
                    if (_usingController) { SetVibration(65535, 65535); _lastRumbleTime = Game.GameTime; _rumblePulseActive = true; _finishBurstFired = true; }
                    PlayFinishSounds();
                    EndAnimation();
                }
            }
        }

        // ════════════════════════════════════════════
        // PICKUP (unified — works from car or on foot)
        // ════════════════════════════════════════════
        /// <summary>
        /// A-Life: find the nearest eligible ped, compute her fingerprint, and open the
        /// Approach submenu so the player can inspect her before committing.
        /// </summary>
        private void OpenApproachMenu(Ped player)
        {
            Ped found = FindNearestFemale(player.Position, 7f);
            if (found == null)
            {
                ShowHudStatus(sandboxMode ? "~r~NO WOMEN NEARBY!" : "~r~NO HOOKERS NEARBY!", 2000);
                return;
            }

            // A-Life: if she's already in active combat against the player, don't interrupt it
            if (aLifeMode && Function.Call<bool>(Hash.IS_PED_IN_COMBAT, found, Game.Player.Character))
            {
                ShowHudStatus(sandboxMode ? "~r~NO WOMEN NEARBY!" : "~r~NO HOOKERS NEARBY!", 2000);
                return;
            }

            // Vehicle validation (same as TryPickup)
            if (player.IsInVehicle())
            {
                Vehicle car = player.CurrentVehicle;
                if (car == null || !IsSuitableVehicle(car))
                { ShowHudStatus("~r~請先尋找合適的載具！", 2000); return; }
                if (car.GetPedOnSeat(VehicleSeat.Passenger) != null)
                { ShowHudStatus("~r~副駕駛座已被佔用！", 2000); return; }
            }

            // Pre-load A-Life data; hostility is handled inside the approach flow, not here.
            string fp = GetPedFingerprint(found);
            if (fp != null)
            {
                // First encounter: rolls name + personality; every session: re-rolls mood.
                if (!sandboxMode && aLifeMode)
                    GetOrCreateProstALifeData(found);
                else
                    GetOrCreateALifeData(found);

                // Persist a random hooker voice immediately at first approach (before hire),
                // so the INI entry is populated even if they are never hired.
                {
                    ALifePedData aDataVoice;
                    if (_aLifePeds.TryGetValue(ALifeKey(fp), out aDataVoice) && (aDataVoice.Voice == null || aDataVoice.Voice.Length == 0))
                    {
                        aDataVoice.Voice = HOOKER_VOICES[rng.Next(HOOKER_VOICES.Length)];
                        SaveALife();
                    }
                }
            }

            // Apply mood facial expression immediately when the approach menu opens
            if (aLifeMode && fp != null)
            {
                ALifePedData faceApply;
                if (_aLifePeds.TryGetValue(ALifeKey(fp), out faceApply))
                    ApplyMoodFacialExpression(found, faceApply.Mood);
            }
            _faceReapplyAt = 0; // ensure OnTick retries immediately next frame

            _approachCandidate = found;
            menuLevel = MenuLevel.Approach;
            menuIndex = 0;
        }

        private void TryPickup(Ped player, Ped preFound = null)
        {
            Ped found;
            if (preFound != null && preFound.Exists())
            {
                found = preFound; // Already validated in OpenApproachMenu
            }
            else
            {
                found = FindNearestFemale(player.Position, 7f);
                if (found == null)
                {
                    ShowHudStatus(sandboxMode ? "~r~附近沒有女性！" : "~r~附近沒有特殊服務目標！", 2000);
                    return;
                }

                // If player is in a vehicle, validate it first
                if (player.IsInVehicle())
                {
                    Vehicle car = player.CurrentVehicle;
                    if (car == null || !IsSuitableVehicle(car))
                    {
                        ShowHudStatus("~r~請先尋找合適的載具！", 2000);
                        return;
                    }
                    if (car.GetPedOnSeat(VehicleSeat.Passenger) != null)
                    {
                        ShowHudStatus("~r~副駕駛座已被佔用！", 2000);
                        return;
                    }
                }
            }

            girl = found;

            // Prost A-Life player-initiated approach: apply the same 20 s cooldown so
            // another hooker won't immediately auto-approach after this session ends.
            // (The auto-approach path sets this itself before calling TryPickup.)
            if (!sandboxMode && aLifeMode && !_prostAutoApproachActive)
                _prostAutoApproachCooldown = Game.GameTime + 20000;

            // A-Life: evict ALL stale handle-keyed cache entries before use.
            // GTA recycles ped handles — a cached entry for this handle may belong to a
            // completely different ped that was deleted earlier in the session.
            // In A-Life mode the fingerprint (INI) is authoritative, not the handle cache.
            if (aLifeMode)
            {
                int staleHandle = girl.Handle;
                priceCache.Remove(staleHandle);
                priceLocked.Remove(staleHandle);
                availabilityCache.Remove(staleHandle);
                _outdoorWillingCache.Remove(staleHandle);
                _bjThroatChanceCache.Remove(staleHandle);
                approachDictCache.Remove(staleHandle);
                voiceCache.Remove(staleHandle);
            }

            GeneratePedPrices(girl.Handle);
            AssignHookerVoice(girl);
            girl.IsPersistent = true;
            girl.BlockPermanentEvents = true;

            // A-Life: build/load identity record for this ped
            _currentGirlFp  = GetPedFingerprint(girl);
            _currentGirlKey = ALifeKey(_currentGirlFp);
            if (aLifeMode && _currentGirlFp != null)
            {
                ALifePedData girlAd = (!sandboxMode && aLifeMode)
                    ? GetOrCreateProstALifeData(girl)
                    : GetOrCreateALifeData(girl);
                if (girlAd != null) ApplyMoodFacialExpression(girl, girlAd.Mood);
            }

            if (!sandboxMode)
            {
                // ── Prostitution mode: hooker approaches player first ──
                menuLevel = MenuLevel.None; // Close menu during approach
                state = ScriptState.Approaching;
                approachPhase = 0;
                jHoldStart = 0;

                // Tell her to walk to the player
                if (player.IsInVehicle())
                {
                    // Walk to the driver-side window area
                    Vector3 windowPos = player.GetOffsetPosition(new Vector3(-1.5f, 0.5f, 0f));
                    approachTargetPos = windowPos;
                    Function.Call(Hash.TASK_GO_STRAIGHT_TO_COORD, girl,
                        windowPos.X, windowPos.Y, windowPos.Z,
                        1.0f, -1, player.Heading + 90f, 0.5f);
                }
                else
                {
                    // Walk toward the player on foot
                    approachTargetPos = player.Position;
                    Function.Call(Hash.TASK_GO_TO_ENTITY, girl, player, -1, 1.5f, 1.0f, 0f, 0);
                }

                ShowHudStatus("~y~目標正在接近你……~s~(~b~HOLD " + MasterDisplay + "~s~ TO ABORT!)", 5000);
            }
            else
            {
                // ── Sandbox mode: immediate pickup (existing behavior) ──
                hasGirl = true;
                AttachGirlBlip();
                Function.Call(Hash.SET_EVERYONE_IGNORE_PLAYER, Game.Player, true);

                if (player.IsInVehicle())
                {
                    playerWasInVehicle = true;
                    VehicleSeat pickupSeat = (player.CurrentVehicle != null && IsPlayerInBackSeat(player.CurrentVehicle))
                        ? VehicleSeat.LeftRear : VehicleSeat.Passenger;
                    if (!IsALifeVehicleEntryGated())
                        TaskGirlEnterVehicle(player.CurrentVehicle, pickupSeat);
                    else
                    {
                        // She's been recruited but not yet invited in — stop her task and wait.
                        girl.Task.ClearAll();
                        girl.BlockPermanentEvents = true;
                    }
                }
                else
                {
                    float dist = girl.Position.DistanceTo(player.Position);
                    float speed = (dist > 7f) ? 3f : 1f;
                    Function.Call(Hash.TASK_FOLLOW_TO_OFFSET_OF_ENTITY,
                        girl, player, 0f, -1f, 0f, speed, -1, 1.5f, true);
                    playerWasInVehicle = false;
                }

                if (!aLifeMode)
                {
                    // Non-A-Life sandbox: no approach menu, show brief HUD
                    ShowHudStatus("~g~WOMAN FOUND!", 3000);
                }
                // A-Life: accept subtitle already shown by ShowApproachAcceptSubtitle before TryPickup
            }
        }

        // ═════════════════
        // APPROACHING STATE
        // ═════════════════
        private void HandleApproachingState(Ped player)
        {
            if (girl == null || !girl.Exists() || !girl.IsAlive)
            {
                FullReset();
                return;
            }

            // Distance to target (for Phase 0 arrival check — uses window pos when in car)
            float distToTarget = player.IsInVehicle()
                ? girl.Position.DistanceTo(approachTargetPos)
                : girl.Position.DistanceTo(player.Position);

            // Distance to player (for all bailout checks — always measures to actual player)
            float dist = girl.Position.DistanceTo(player.Position);

            // ── Hold master key/combo to abort (only while she's still walking / playing intro) ──
            if (approachPhase <= 1)
            {
                bool holdingAbort = _usingController
                    ? (Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 0, masterControlModifier) && Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 0, masterControl))
                    : Game.IsKeyPressed(masterKey);
                if (holdingAbort)
                {
                    if (jHoldStart == 0) jHoldStart = Game.GameTime;
                    int held = Game.GameTime - jHoldStart;
                    float pct = Math.Min(held / 600f, 1f);
                    ShowHudStatus("~r~ABORTING... [" + (int)(pct * 100) + "%]", 100);
                    if (held >= 600)
                    {
                        ShowHudStatus("~r~ABORTED!", 2000);
                        FullReset();
                        return;
                    }
                }
                else
                {
                    jHoldStart = 0;
                }
            }

            // Phase 0: Walking to player
            if (approachPhase == 0)
            {
                // Player walked/drove away — silent reset (no anims, she's just walking)
                if (dist > 10f)
                {
                    FullReset();
                    return;
                }

                float arrivalDist = player.IsInVehicle() ? 0.8f : 1.7f;
                if (distToTarget < arrivalDist)
                {
                    // Stop walking and face the player
                    girl.Task.ClearAll();
                    // Compute heading toward the player/vehicle — used for ALL approach anims
                    approachHeading = (player.Position - girl.Position).ToHeading();
                    girl.Heading = approachHeading;

                    // Pick dict (cached per hooker so she keeps her style across sessions in Prost A-Life)
                    int girlHandle = girl.Handle;
                    if (!approachDictCache.TryGetValue(girlHandle, out approachDict))
                    {
                        // Prost A-Life: load from INI record, or assign once and persist
                        bool isProstALifeDict = !sandboxMode && aLifeMode;
                        ALifePedData dDict = null;
                        if (isProstALifeDict && _currentGirlFp != null)
                            _aLifePeds.TryGetValue(_currentGirlKey, out dDict);

                        if (dDict != null && !string.IsNullOrEmpty(dDict.HookerAnim)
                            && System.Array.IndexOf(HOOKER_APPROACH_DICTS, dDict.HookerAnim) >= 0)
                        {
                            approachDict = dDict.HookerAnim;
                        }
                        else
                        {
                            approachDict = HOOKER_APPROACH_DICTS[rng.Next(HOOKER_APPROACH_DICTS.Length)];
                            if (dDict != null && isProstALifeDict)
                            {
                                dDict.HookerAnim = approachDict;
                                SaveALife();
                            }
                        }
                        approachDictCache[girlHandle] = approachDict;
                    }
                    if (!RequestAnimDict(approachDict)) { FullReset(); return; }

                    // Play solicitation voice (voice already assigned in TryPickup)
                    // In Prostitution A-Life mode, only play if she initiated the approach
                    bool isProstALife = !sandboxMode && aLifeMode;
                    if (!isProstALife || _prostAutoApproachActive)
                        PlayPedSpeech(girl, "提出邀請");

                    // Show accept subtitle only when player initiated the approach (no voice in that case)
                    if (isProstALife && !_prostAutoApproachActive)
                    {
                        ALifePedData dArr = null;
                        if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out dArr);
                        ShowApproachAcceptSubtitle(dArr);
                    }

                    // Play idle_intro — use ANIM_ADVANCED to lock facing toward car
                    Vector3 gp = girl.Position;
                    Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, girl,
                        approachDict, "idle_intro",
                        gp.X, gp.Y, gp.Z, 0.0f, 0.0f, approachHeading,
                        8.0f, -4.0f, -1, 2, 0.0f, 2, 0);

                    float introDur = Function.Call<float>(Hash.GET_ANIM_DURATION, approachDict, "idle_intro");
                    approachAnimWait = Game.GameTime + (int)(introDur * 1000f);
                    approachPhase = 1;
                }
            }
            // Phase 1: idle_intro playing → then go to idle_wait
            else if (approachPhase == 1)
            {
                if (Game.GameTime >= approachAnimWait)
                {
                    // Intro done — start idle_wait loop
                    { Vector3 gp = girl.Position;
                    Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, girl,
                        approachDict, "idle_wait",
                        gp.X, gp.Y, gp.Z, 0.0f, 0.0f, approachHeading,
                        8.0f, -4.0f, -1, 1, 0.0f, 2, 0); } // flag 1 = loop

                    // Schedule first sexy pose interrupt
                    approachSexySwapTime = Game.GameTime + rng.Next(4000, 8000);
                    approachDecisionTimer = Game.GameTime + 45000; // 45 seconds patience
                    approachPhase = 2;
                }
            }
            // Phase 2: idle_wait (loop) — accept/decline, occasional sexy pose interrupts
            else if (approachPhase == 2)
            {
                // Player walked/drove away — play idle_outro then reset
                if (dist > 10f)
                {
                    { Vector3 gp = girl.Position;
                    Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, girl,
                        approachDict, "idle_outro",
                        gp.X, gp.Y, gp.Z, 0.0f, 0.0f, approachHeading,
                        8.0f, -4.0f, -1, 0, 0.0f, 2, 0); }
                    float outroDur = Function.Call<float>(Hash.GET_ANIM_DURATION, approachDict, "idle_outro");
                    approachAnimWait = Game.GameTime + (int)(outroDur * 1000f);
                    approachPhase = 7; // outro-then-reset
                    return;
                }

                // 45s patience expired — she rejects on her own
                if (Game.GameTime >= approachDecisionTimer)
                {
                    Function.Call(Hash.STOP_CURRENT_PLAYING_AMBIENT_SPEECH, girl);
                    { Vector3 gp = girl.Position;
                    Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, girl,
                        approachDict, "idle_reject",
                        gp.X, gp.Y, gp.Z, 0.0f, 0.0f, approachHeading,
                        8.0f, -4.0f, -1, 2, 0.0f, 2, 0); }
                    float rejectDur = Function.Call<float>(Hash.GET_ANIM_DURATION, approachDict, "idle_reject");
                    approachAnimWait = Game.GameTime + (int)(rejectDur * 1000f);
                    approachPhase = 4;
                    if (!sandboxMode && aLifeMode && _currentGirlKey != null)
                    {
                        ALifePedData dTimeout;
                        if (_aLifePeds.TryGetValue(_currentGirlKey, out dTimeout))
                        {
                            dTimeout.Reputation = Math.Max(-1, dTimeout.Reputation - 4);
                            UpdateProstRelationshipState(dTimeout);
                            SaveALife();
                        }
                    }
                    ShowHudStatus("~r~SHE GOT TIRED OF WAITING!", 2000);
                    return;
                }

                GTA.UI.Screen.ShowHelpTextThisFrame(
                    "~g~[" + MenuSelectDisplay + "]~w~ Accept   ~r~[" + MenuBackDisplay + "]~w~ Decline");
                if (Game.GameTime >= approachSexySwapTime)
                {
                    string sexyAnim = HOOKER_SEXY_ANIMS[rng.Next(HOOKER_SEXY_ANIMS.Length)];
                    { Vector3 gp = girl.Position;
                    Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, girl,
                        approachDict, sexyAnim,
                        gp.X, gp.Y, gp.Z, 0.0f, 0.0f, approachHeading,
                        8.0f, -4.0f, -1, 2, 0.0f, 2, 0); } // flag 2 = hold last frame

                    float sexyDur = Function.Call<float>(Hash.GET_ANIM_DURATION, approachDict, sexyAnim);
                    approachAnimWait = Game.GameTime + (int)(sexyDur * 1000f);
                    approachPhase = 6; // sexy pose playing
                }

                if (_usingController ? Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 0, menuSelectControl) : Game.IsKeyPressed(menuSelectKey))
                {
                    // ── Accept: play idle_outro then transition ──
                    { Vector3 gp = girl.Position;
                    Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, girl,
                        approachDict, "idle_outro",
                        gp.X, gp.Y, gp.Z, 0.0f, 0.0f, approachHeading,
                        8.0f, -4.0f, -1, 0, 0.0f, 2, 0); }

                    float outroDur = Function.Call<float>(Hash.GET_ANIM_DURATION, approachDict, "idle_outro");
                    approachAnimWait = Game.GameTime + (int)(outroDur * 1000f);
                    approachPhase = 3;
                }
                else if (_usingController ? Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 0, menuBackControl) : Game.IsKeyPressed(menuBackKey))
                {
                    // ── Decline: roll casual (idle_outro) vs pissed (idle_reject) ──
                    // Sandbox: 50/50, cached per ped handle for the whole game session — once pissed, stays pissed.
                    // Prost A-Life: driven by Aggressiveness + Patience, re-rolled each encounter.
                    bool casualRejection;
                    if (!sandboxMode && aLifeMode && _currentGirlKey != null)
                    {
                        // A-Life: personality-driven, always re-roll
                        ALifePedData dRej;
                        _aLifePeds.TryGetValue(_currentGirlKey, out dRej);
                        PersonalityProfile profRej = (dRej != null) ? GetProfile(dRej.Personality) : null;
                        double casualChance = (profRej != null)
                            ? Math.Max(0.05, Math.Min(0.75, (1.0 - profRej.Aggressiveness) * 0.55 + profRej.Patience * 0.20))
                            : 0.40;
                        casualRejection = rng.NextDouble() < casualChance;
                    }
                    else
                    {
                        // Sandbox: check cache first; roll and store if not seen before
                        int girlHandleRej = girl.Handle;
                        if (!_approachRejectionCasual.TryGetValue(girlHandleRej, out casualRejection))
                        {
                            casualRejection = rng.NextDouble() < 0.50;
                            _approachRejectionCasual[girlHandleRej] = casualRejection;
                        }
                    }

                    Function.Call(Hash.STOP_CURRENT_PLAYING_AMBIENT_SPEECH, girl);
                    Vector3 gpR = girl.Position;
                    if (casualRejection)
                    {
                        // Casual: she doesn't care — plays idle_outro and walks off normally
                        Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, girl,
                            approachDict, "idle_outro",
                            gpR.X, gpR.Y, gpR.Z, 0.0f, 0.0f, approachHeading,
                            8.0f, -4.0f, -1, 0, 0.0f, 2, 0);
                        float outroDurR = Function.Call<float>(Hash.GET_ANIM_DURATION, approachDict, "idle_outro");
                        approachAnimWait = Game.GameTime + (int)(outroDurR * 1000f);
                        approachPhase = 7; // outro-then-reset
                    }
                    else
                    {
                        // Pissed: plays idle_reject → reject_outro
                        Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, girl,
                            approachDict, "idle_reject",
                            gpR.X, gpR.Y, gpR.Z, 0.0f, 0.0f, approachHeading,
                            8.0f, -4.0f, -1, 2, 0.0f, 2, 0);
                        float rejectDur = Function.Call<float>(Hash.GET_ANIM_DURATION, approachDict, "idle_reject");
                        approachAnimWait = Game.GameTime + (int)(rejectDur * 1000f);
                        approachPhase = 4;

                        // Prost A-Life: rep penalty only when she actually cares
                        if (!sandboxMode && aLifeMode && _currentGirlKey != null)
                        {
                            ALifePedData dDecline;
                            if (_aLifePeds.TryGetValue(_currentGirlKey, out dDecline))
                            {
                                dDecline.Reputation = Math.Max(-1, dDecline.Reputation - 4);
                                UpdateProstRelationshipState(dDecline);
                                SaveALife();
                            }
                        }
                    }

                    ShowHudStatus(casualRejection ? "~y~OFFER DECLINED!" : "~r~OFFER DECLINED!", 2000);
                }
            }
            // Phase 3: idle_outro (accepted) — wait for anim then transition to hasGirl
            else if (approachPhase == 3)
            {
                // Player left before she finished the outro
                if (dist > 15f)
                {
                    if (!sandboxMode && aLifeMode && _currentGirlKey != null)
                    {
                        ALifePedData dWalkRec;
                        if (_aLifePeds.TryGetValue(_currentGirlKey, out dWalkRec))
                        {
                            dWalkRec.Reputation = Math.Max(-1, dWalkRec.Reputation - 6);
                            UpdateProstRelationshipState(dWalkRec);
                            SaveALife();
                        }
                    }
                    FullReset();
                    return;
                }

                if (Game.GameTime >= approachAnimWait)
                {
                    girl.Task.ClearAll();
                    hasGirl = true;
                    AttachGirlBlip();
                    Function.Call(Hash.SET_EVERYONE_IGNORE_PLAYER, Game.Player, true);

                    if (player.IsInVehicle())
                    {
                        playerWasInVehicle = true;
                        VehicleSeat approachSeat = (player.CurrentVehicle != null && IsPlayerInBackSeat(player.CurrentVehicle))
                            ? VehicleSeat.LeftRear : VehicleSeat.Passenger;
                        if (!IsALifeVehicleEntryGated())
                            TaskGirlEnterVehicle(player.CurrentVehicle, approachSeat);
                        else
                        {
                            // She accepted the approach but hasn't been invited into the car yet.
                            girl.Task.ClearAll();
                            girl.BlockPermanentEvents = true;
                        }
                    }
                    else
                    {
                        Function.Call(Hash.TASK_FOLLOW_TO_OFFSET_OF_ENTITY,
                            girl, player, 0f, -1f, 0f, 1f, -1, 1.5f, true);
                        playerWasInVehicle = false;
                    }

                    state = ScriptState.Idle;
                    menuLevel = MenuLevel.Main;
                    menuIndex = 0;
                    keyCooldown = KEY_COOLDOWN_FRAMES;
                    ShowHudStatus("~g~交易接受！請選擇服務！", 3000);
                }
            }
            // Phase 4: idle_reject — wait then play reject_outro
            else if (approachPhase == 4)
            {
                if (Game.GameTime >= approachAnimWait)
                {
                    // Use regular TASK_PLAY_ANIM here — idle_reject may have turned her away,
                    // so reject_outro should continue from her current orientation (no snap-back)
                    Function.Call(Hash.TASK_PLAY_ANIM, girl,
                        approachDict, "reject_outro",
                        8.0f, -4.0f, -1, 0, 0.0f, false, false, false);

                    float roaDur = Function.Call<float>(Hash.GET_ANIM_DURATION, approachDict, "reject_outro");
                    approachAnimWait = Game.GameTime + (int)(roaDur * 1000f);
                    approachPhase = 5;
                }
            }
            // Phase 5: reject_outro — wait then full reset
            else if (approachPhase == 5)
            {
                if (Game.GameTime >= approachAnimWait)
                {
                    FullReset();
                }
            }
            // Phase 7: idle_outro (player left) — wait then full reset
            else if (approachPhase == 7)
            {
                if (Game.GameTime >= approachAnimWait)
                {
                    FullReset();
                }
            }
            // Phase 6: sexy pose (idle_a/b/c/d) playing — when done, return to idle_wait
            else if (approachPhase == 6)
            {
                // Player walked/drove away — play idle_outro then reset
                if (dist > 10f)
                {
                    { Vector3 gp = girl.Position;
                    Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, girl,
                        approachDict, "idle_outro",
                        gp.X, gp.Y, gp.Z, 0.0f, 0.0f, approachHeading,
                        8.0f, -4.0f, -1, 0, 0.0f, 2, 0); }
                    float outroDur = Function.Call<float>(Hash.GET_ANIM_DURATION, approachDict, "idle_outro");
                    approachAnimWait = Game.GameTime + (int)(outroDur * 1000f);
                    approachPhase = 7; // outro-then-reset
                    return;
                }

                // 45s patience expired — she rejects on her own
                if (Game.GameTime >= approachDecisionTimer)
                {
                    Function.Call(Hash.STOP_CURRENT_PLAYING_AMBIENT_SPEECH, girl);
                    { Vector3 gp = girl.Position;
                    Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, girl,
                        approachDict, "idle_reject",
                        gp.X, gp.Y, gp.Z, 0.0f, 0.0f, approachHeading,
                        8.0f, -4.0f, -1, 2, 0.0f, 2, 0); }
                    float rejectDur = Function.Call<float>(Hash.GET_ANIM_DURATION, approachDict, "idle_reject");
                    approachAnimWait = Game.GameTime + (int)(rejectDur * 1000f);
                    approachPhase = 4;
                    ShowHudStatus("~r~SHE GOT TIRED OF WAITING!", 2000);
                    return;
                }

                GTA.UI.Screen.ShowHelpTextThisFrame(
                    "~g~[" + MenuSelectDisplay + "]~w~ Accept   ~r~[" + MenuBackDisplay + "]~w~ Decline");

                if (Game.GameTime >= approachAnimWait)
                {
                    // Back to idle_wait loop
                    { Vector3 gp = girl.Position;
                    Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, girl,
                        approachDict, "idle_wait",
                        gp.X, gp.Y, gp.Z, 0.0f, 0.0f, approachHeading,
                        8.0f, -4.0f, -1, 1, 0.0f, 2, 0); } // flag 1 = loop

                    // Schedule next sexy pose interrupt
                    approachSexySwapTime = Game.GameTime + rng.Next(4000, 8000);
                    approachPhase = 2;
                }

                // Still allow accept/decline during sexy pose
                if (_usingController ? Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 0, menuSelectControl) : Game.IsKeyPressed(menuSelectKey))
                {
                    { Vector3 gp = girl.Position;
                    Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, girl,
                        approachDict, "idle_outro",
                        gp.X, gp.Y, gp.Z, 0.0f, 0.0f, approachHeading,
                        8.0f, -4.0f, -1, 0, 0.0f, 2, 0); }

                    float outroDur = Function.Call<float>(Hash.GET_ANIM_DURATION, approachDict, "idle_outro");
                    approachAnimWait = Game.GameTime + (int)(outroDur * 1000f);
                    approachPhase = 3;
                }
                else if (_usingController ? Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 0, menuBackControl) : Game.IsKeyPressed(menuBackKey))
                {
                    Function.Call(Hash.STOP_CURRENT_PLAYING_AMBIENT_SPEECH, girl);
                    { Vector3 gp = girl.Position;
                    Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, girl,
                        approachDict, "idle_reject",
                        gp.X, gp.Y, gp.Z, 0.0f, 0.0f, approachHeading,
                        8.0f, -4.0f, -1, 2, 0.0f, 2, 0); }

                    float rejectDur = Function.Call<float>(Hash.GET_ANIM_DURATION, approachDict, "idle_reject");
                    approachAnimWait = Game.GameTime + (int)(rejectDur * 1000f);
                    approachPhase = 4;
                    ShowHudStatus("~r~OFFER DECLINED!", 2000);
                }
            }
        }

        /// <summary>檢查女性 NPC 是否可進入該載具（有乘客座與空位，支援所有摩托車、單車、直升機、飛機、船隻與汽車）。</summary>
        private bool CanGirlEnterVehicle(Vehicle car)
        {
            if (car == null || !car.Exists() || car.IsDead) return false;
            if (Function.Call<int>(Hash.GET_VEHICLE_MAX_NUMBER_OF_PASSENGERS, car) <= 0) return false;
            VehicleSeat targetSeat = GetBestSeatForGirl(car);
            Ped occupant = car.GetPedOnSeat(targetSeat);
            return occupant == null || occupant == girl;
        }

        /// <summary>檢查載具是否支援性愛服務動畫（僅限封閉式四輪轎車/SUV，不支援摩托車、自行車、船隻、飛行器等）。</summary>
        private bool IsSexSuitableVehicle(Vehicle car)
        {
            if (car == null || !car.Exists()) return false;
            var cls = car.ClassType;
            return cls != VehicleClass.Motorcycles &&
                   cls != VehicleClass.Cycles &&
                   cls != VehicleClass.Boats &&
                   cls != VehicleClass.Helicopters &&
                   cls != VehicleClass.Planes;
        }

        /// <summary>相容舊呼叫：檢查載具是否可供 NPC 進入與跟隨乘坐。</summary>
        private bool IsSuitableVehicle(Vehicle car)
        {
            return CanGirlEnterVehicle(car);
        }

        // ═══════════
        // CAR ACTIONS
        // ═══════════
        private void StartCarAction(int idx)
        {
            if (girl == null || !girl.Exists()) { FullReset(); return; }
            Ped player = Game.Player.Character;
            if (!player.IsInVehicle()) return;

            Vehicle car = player.CurrentVehicle;
            if (car == null) return;
            if (!girl.IsInVehicle() || girl.CurrentVehicle != car) return;

            if (!IsSexSuitableVehicle(car))
            {
                ShowHudStatus("~r~該載具不支援性愛服務！（僅限普通轎車/SUV）", 3000);
                menuLevel = aLifeMode ? MenuLevel.Actions : MenuLevel.Main;
                return;
            }

            // ── Backseat car sex: uses random@drunk_driver_2 animations ──
            if (IsPlayerInBackSeat(car))
            {
                const string DRUNK_DICT = "random@drunk_driver_2";
                if (!RequestAnimDict(DRUNK_DICT))
                {
                    ShowHudStatus("~r~動畫載入失敗！", 2000);
                    return;
                }
                Function.Call(Hash.SET_EVERYONE_IGNORE_PLAYER, Game.Player, true);

                currentFemaleAnim = "cardrunksex_loop_f";
                currentMaleAnim   = "cardrunksex_loop_m";
                currentFemaleDict = DRUNK_DICT;
                currentMaleDict   = DRUNK_DICT;
                currentAction     = "sex";
                _backseatCarSex   = true;
                defaultDeviceIntensity = layingCowgirlDeviceSpeed;
                _deviceExcluded = false;
                healPlayer = true; healGirl = true; healRate = 0.5f;

                string backseatFIntro = "cardrunksex_intro_f";
                string backseatMIntro = "cardrunksex_intro_m";
                Function.Call(Hash.TASK_PLAY_ANIM, girl, DRUNK_DICT, backseatFIntro,
                    8.0f, -4.0f, -1, 2, 0.0f, false, false, false);
                Function.Call(Hash.TASK_PLAY_ANIM, player, DRUNK_DICT, backseatMIntro,
                    8.0f, -4.0f, -1, driveableCar ? 34 : 2, 0.0f, false, false, false);
                Function.Call(Hash.SET_PED_KEEP_TASK, girl, true);

                AttachDildoProp(player);
                state = ScriptState.Animating;
                animPhase = 0;
                float backseatIntroDur = Function.Call<float>(Hash.GET_ANIM_DURATION, DRUNK_DICT, backseatFIntro);
                animWaitTime = Game.GameTime + (int)(backseatIntroDur * 1000f);
                PlayBjCustom(_bjPantsZipFile);
                lastVoiceTime = Game.GameTime;
                ShowHudStatus("~g~正在啟動……~s~(~y~按下 " + MasterDisplay + " TO ABORT~s~)", 2000);
                return;
            }

            // Determine vehicle animation type
            int modelHash = car.Model.Hash;
            
            // Check manual overrides first (from .cfg)
            if (overwriteLowVehicle.Contains(modelHash))
            {
                vehicleAnimType = VehicleAnimType.Low;
            }
            else if (overwriteNormalVehicle.Contains(modelHash))
            {
                vehicleAnimType = VehicleAnimType.Normal;
            }
            else
            {
                // Fallback: ClassType detection
                int classType = Function.Call<int>(Hash.GET_VEHICLE_CLASS, car);
                // Low vehicles: Coupes (3), Sports Classics (5), Sports (6), Super (7)
                if (classType == 3 || classType == 5 || classType == 6 || classType == 7)
                    vehicleAnimType = VehicleAnimType.Low;
                else
                    vehicleAnimType = VehicleAnimType.Normal;
            }
            
            string dict   = (vehicleAnimType == VehicleAnimType.Low) ? VEH_LOW_DICT    : VEH_NORM_DICT;
            string fpDict = (vehicleAnimType == VehicleAnimType.Low) ? VEH_LOW_FP_DICT : VEH_NORM_FP_DICT;

            // Check camera mode so phase-0 intro uses the correct (FP or non-FP) dict
            int camMode = Function.Call<int>(Hash.GET_FOLLOW_PED_CAM_VIEW_MODE);
            bool isFirstPerson = (camMode == 4);
            string introDict = isFirstPerson ? fpDict : dict;

            // Preload both variants; the one we'll actually use must succeed
            RequestAnimDict(isFirstPerson ? dict : fpDict); // preload the other one
            if (!RequestAnimDict(introDict))
            {
                // FP dict not ready yet — fall back to non-FP
                introDict = dict;
                if (!RequestAnimDict(introDict))
                {
                    ShowHudStatus("~r~動畫載入失敗！", 2000);
                    return;
                }
            }

            // Setup
            Function.Call(Hash.SET_EVERYONE_IGNORE_PLAYER, Game.Player, true);

            // Store loop anim names (for phase 2) based on action
            string fIntro = "", mIntro = "";
            
            if (idx == 0) // Blowjob
            {
                if (vehicleAnimType == VehicleAnimType.Low)
                {
                    currentFemaleAnim = "low_car_bj_loop_female";
                    currentMaleAnim = "low_car_bj_loop_player";
                    fIntro = "low_car_prop_to_bj_p1_female";
                    mIntro = "low_car_prop_to_bj_p1_player";
                }
                else
                {
                    currentFemaleAnim = "bj_loop_prostitute";
                    currentMaleAnim = "bj_loop_male";
                    fIntro = "proposition_to_bj_p1_prostitute";
                    mIntro = "proposition_to_bj_p1_male";
                }
                currentAction = "blowjob";
                defaultDeviceIntensity = (vehicleAnimType == VehicleAnimType.Low) ? carLowBjDeviceSpeed : carBjDeviceSpeed;
                _deviceExcluded = false;
                healPlayer = true; healGirl = false; healRate = 0.7f;
            }
            else if (idx == 1) // Sex
            {
                if (vehicleAnimType == VehicleAnimType.Low)
                {
                    currentFemaleAnim = "low_car_sex_loop_female";
                    currentMaleAnim = "low_car_sex_loop_player";
                    fIntro = "low_car_prop_to_sex_p1_female";
                    mIntro = "low_car_prop_to_sex_p1_player";
                }
                else
                {
                    currentFemaleAnim = "sex_loop_prostitute";
                    currentMaleAnim = "sex_loop_male";
                    fIntro = "proposition_to_sex_p1_prostitute";
                    mIntro = "proposition_to_sex_p1_male";
                }
                currentAction = "sex";
                defaultDeviceIntensity = (vehicleAnimType == VehicleAnimType.Low) ? carLowSexDeviceSpeed : carSexDeviceSpeed;
                _deviceExcluded = false;
                healPlayer = true; healGirl = true; healRate = 0.5f;
            }
            
            currentFemaleDict = introDict;
            currentMaleDict   = introDict;

            // Start INTRO animations (flag 2 = hold last frame, no reset)
            
            Function.Call(Hash.TASK_PLAY_ANIM, girl, introDict, fIntro,
                8.0f, -4.0f, -1, 2, 0.0f, false, false, false);
            Function.Call(Hash.TASK_PLAY_ANIM, player, introDict, mIntro,
                8.0f, -4.0f, -1, driveableCar ? 34 : 2, 0.0f, false, false, false);

            // Lock the animation task NOW (after it's been given)
            Function.Call(Hash.SET_PED_KEEP_TASK, girl, true);

            // BJ: defer prop attachment to prop_to_bj_p2 (phase 0→1); sex: attach now
            if (currentAction != "blowjob") AttachDildoProp(player);

            state = ScriptState.Animating;
            animPhase = 0; // Start with intro
            // Set wait time = now + animation duration (ms) using GET_ANIM_DURATION
            float introDuration = Function.Call<float>(Hash.GET_ANIM_DURATION, introDict, fIntro);
            animWaitTime = Game.GameTime + (int)(introDuration * 1000f);
            // Car sex intro: unzip as proposition_to_sex_p1 begins
            if (currentAction == "sex") PlayBjCustom(_bjPantsZipFile);
            lastVoiceTime = Game.GameTime;

            ShowHudStatus("~g~正在啟動……~s~(~y~按下 " + MasterDisplay + " TO ABORT~s~)", 2000);
        }

        // ══════════════════════════════
        // STREET CATEGORY INTRO / EXIT
        // ══════════════════════════════

        /// <summary>Start the sit-down → proposition sequence when opening Sitting/Laying Services on street.</summary>
        private void EnterStreetSitProposition(Ped player, MenuLevel category)
        {
            if (player.IsInVehicle()) return;
            if (girl == null || !girl.Exists()) return;
            if (category != MenuLevel.SittingServices && category != MenuLevel.LayingServices) return;

            _streetSitCategory = category;
            _streetSitOriginPos = player.Position;
            _streetSitOriginHeading = player.Heading;
            girl.BlockPermanentEvents = true;

            _streetSitPropActive = true;
            // When sit anim is disabled, skip the picnic sit-down (phase -1) and go straight to prop enter
            _streetSitPropPhase = streetSitAnimEnabled ? -1 : 0;
            _streetSitPropWaitTime = 0;
        }

        /// <summary>Begin the proposition-exit → stand-up sequence when leaving Sitting/Laying Services on street.</summary>
        private void ExitStreetSitProposition(Ped player)
        {
            if (!_streetSitPropActive) return;

            // If still in sit-down phase (not yet sunk), cancel immediately
            if (_streetSitPropPhase <= 0)
            {
                player.Task.ClearAll();
                if (girl != null && girl.Exists()) girl.Task.ClearAll();
                _streetSitPropActive = false;
                _streetSitPropPhase = 0;
                return;
            }

            string mDict, mAnim, fDict, fAnim;
            Vector3 offset; float heading; bool isSitting; float zOffset; float rotX; float rotHorizontal;
            GetStreetPropAnimData(3, out mDict, out mAnim, out fDict, out fAnim, out offset, out heading, out isSitting, out zOffset, out rotX, out rotHorizontal);

            if (!RequestAnimDict(mDict)) return;
            if (mDict != fDict && !RequestAnimDict(fDict)) return;

            // Both peds already sunk & frozen — just play exit anims
            if (rotX != 0f || rotHorizontal != 0f)
            {
                Vector3 pp = player.Position;
                Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, player, mDict, mAnim,
                    pp.X, pp.Y, pp.Z, rotX, 0f, player.Heading + rotHorizontal,
                    8.0f, -4.0f, -1, 2, 0f, 2, 0);
            }
            else
            {
                Function.Call(Hash.TASK_PLAY_ANIM, player, mDict, mAnim,
                    8.0f, -4.0f, -1, 2, 0f, false, false, false);
            }
            Function.Call(Hash.TASK_PLAY_ANIM, girl, fDict, fAnim,
                8.0f, -4.0f, -1, 2, 0f, false, false, false);

            float exitDur = Function.Call<float>(Hash.GET_ANIM_DURATION, fDict, fAnim);
            _streetSitPropPhase = 3;
            _streetSitPropWaitTime = Game.GameTime + (int)(exitDur * 1000f);
        }

        // ══════════════
        // STREET ACTIONS
        // ══════════════
        private void StartStreetAction(int idx)
        {
            if (girl == null || !girl.Exists()) { FullReset(); return; }
            Ped player = Game.Player.Character;
            if (player.IsInVehicle()) return;

            // Get animation data for this action
            string mDict, mAnim, fDict, fAnim;
            Vector3 offset;
            float headingOff;
            bool isSitting;
            float zOffset;
            float rotX;
            GetStreetAnimData(idx, out mDict, out mAnim, out fDict, out fAnim, out offset, out headingOff, out isSitting, out zOffset, out rotX);

            // Request anim dictionaries
            if (!RequestAnimDict(mDict)) { ShowHudStatus("~r~動畫載入失敗！", 2000); return; }
            if (mDict != fDict && !RequestAnimDict(fDict)) { ShowHudStatus("~r~動畫載入失敗！", 2000); return; }

            currentMaleDict   = mDict;
            currentMaleAnim   = mAnim;
            currentFemaleDict = fDict;
            currentFemaleAnim = fAnim;
            currentAction = (idx == 0) ? "kissing" : (idx >= 2 && idx <= 4) ? "blowjob" : "sex";
            switch (idx)
            {
                case 0:  _deviceExcluded = true;  defaultDeviceIntensity = 0f; _telemetry.SendStop(); break; // Kissing   — device always off
                case 1:  _deviceExcluded = true;  defaultDeviceIntensity = 0f; _telemetry.SendStop(); break; // Facesitting — device always off
                case 2:  _deviceExcluded = false; defaultDeviceIntensity = standingBjDeviceSpeed;       break; // Standing BJ
                case 3:  _deviceExcluded = false; defaultDeviceIntensity = sittingBjDeviceSpeed;        break; // Sitting BJ
                case 4:  _deviceExcluded = false; defaultDeviceIntensity = layingBjDeviceSpeed;         break; // Laying BJ
                case 5:  _deviceExcluded = false; defaultDeviceIntensity = wallSexDeviceSpeed;          break; // Wall Sex
                case 6:  _deviceExcluded = false; defaultDeviceIntensity = sittingCowgirlDeviceSpeed;   break; // Sitting Cowgirl
                case 7:  _deviceExcluded = false; defaultDeviceIntensity = revCowgirlDeviceSpeed;       break; // Reverse Cowgirl
                case 8:  _deviceExcluded = false; defaultDeviceIntensity = (_sinkraCowgirlAvailable && sinkraReplaceLaying)    ? layingSinkraDeviceSpeed : layingCowgirlDeviceSpeed;    break; // Laying Cowgirl
                case 9:  _deviceExcluded = false; defaultDeviceIntensity = (_sinkraCowgirlAvailable && sinkraReplaceRevLaying) ? layingSinkraDeviceSpeed : layingRevCowgirlDeviceSpeed; break; // Laying Reversed Cowgirl
                case 10: _deviceExcluded = false; defaultDeviceIntensity = doggyDeviceSpeed;            break; // Doggy Style
                default: _deviceExcluded = false; defaultDeviceIntensity = roughDoggyDeviceSpeed;       break; // Rough Doggy Style
            }

            // Healing rules per action type
            if (idx == 0) // Kissing: no healing
            { healPlayer = false; healGirl = false; healRate = 0.0f; }
            else if (idx == 1) // Facesitting: NPC only
            { healPlayer = false; healGirl = true; healRate = 0.7f; }
            else if (idx >= 2 && idx <= 4) // BJs: player only
            { healPlayer = true; healGirl = false; healRate = 0.7f; }
            else // All other sex: both, slowest
            { healPlayer = true; healGirl = true; healRate = 0.5f; }

            // Street sitting action: play prop→action transition instead of jumping straight to loop
            if (_streetSitPropActive && (idx == 6 || idx == 3 || idx == 1 || idx == 7 || idx == 4 || idx == 8 || idx == 9))
            {
                string iMDict, iMAnim, iFDict, iFAnim;
                Vector3 iOffset; float iHeading; bool iIsSitting; float iZOff, iRotX, iRotH;
                GetStreetPropIntroExitAnimData(0, idx, out iMDict, out iMAnim, out iFDict, out iFAnim,
                    out iOffset, out iHeading, out iIsSitting, out iZOff, out iRotX, out iRotH);

                if (!RequestAnimDict(iMDict)) { ShowHudStatus("~r~動畫載入失敗！", 2000); return; }
                if (iMDict != iFDict && !RequestAnimDict(iFDict)) { ShowHudStatus("~r~動畫載入失敗！", 2000); return; }

                streetActionIdx = idx;
                if (idx != 1) AttachDildoProp(player);   // Skip dildo for facesitting only
                if (idx != 1) PlayBjCustom(_bjPantsZipFile); // Skip zip for facesitting only

                // Facesitting / Laying BJ / Laying Cowgirl / Laying Rev Cowgirl: fade to black before repositioning
                if (idx == 1 || idx == 4 || idx == 8 || idx == 9)
                {
                    Function.Call(Hash.DO_SCREEN_FADE_OUT, 500);
                    animWaitTime = Game.GameTime + 600;
                    animPhase = -1;
                    state = ScriptState.Animating;
                    lastVoiceTime = Game.GameTime;
                    ShowHudStatus("~g~正在啟動……~s~(~y~按下 " + MasterDisplay + " TO ABORT~s~)", 2000);
                    return;
                }
                if (idx == 7)
                {
                    // Reverse Cowgirl: schedule delayed fade+reposition 1s into P1
                    // (the P1 anim will start playing below like normal — no early return)
                    _streetOutroFadeAt = Game.GameTime + 1000;
                }

                // Facesitting/Laying BJ: start t_getin_veniceb at ~2.5s into the animation
                float mStartPhase = 0f;
                if (idx == 1 || idx == 4)
                {
                    float totalGetin = Function.Call<float>(Hash.GET_ANIM_DURATION, iMDict, iMAnim);
                    if (totalGetin > 0f) mStartPhase = 2.5f / totalGetin;
                }

                // For actions needing different Z (e.g. facesitting), reposition both peds
                Vector3 pp = player.Position;
                float actionZ = _streetSitOriginPos.Z + iZOff;
                float deltaZ = actionZ - pp.Z;
                if (Math.Abs(deltaZ) > 0.01f)
                {
                    Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, player,
                        pp.X, pp.Y, actionZ, false, false, false);
                    if (girl != null && girl.Exists())
                    {
                        Vector3 gp = girl.Position;
                        Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                            gp.X, gp.Y, gp.Z + deltaZ, false, false, false);
                    }
                    pp = player.Position;
                }

                // Facesitting: reposition girl to vanilla action offset for transitions
                // (Sinkra loop offset is different — that gets applied in phase 1→2 for the loop)
                if (idx == 1 && girl != null && girl.Exists())
                {
                    Vector3 girlActionPos = player.GetOffsetPosition(new Vector3(0.73f, -0.77f, -0.3f));
                    Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                        girlActionPos.X, girlActionPos.Y, girlActionPos.Z, false, false, false);
                    girl.Heading = player.Heading;
                }

                // Laying BJ: girl stays at proposition offset for BJ transitions
                // (laying BJ loop offset gets applied in phase 1→2)

                // Reverse Cowgirl: use sitting cowgirl offset for P1 intro only
                // (delayed fade+reposition will move her to reverse offset mid-P1)
                if (idx == 7 && girl != null && girl.Exists())
                {
                    Vector3 girlCowPos = player.GetOffsetPosition(new Vector3(0.85f, -0.01f, -0.02f));
                    Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                        girlCowPos.X, girlCowPos.Y, girlCowPos.Z, false, false, false);
                    girl.Heading = player.Heading;
                }

                // Play prop→action P1 transition with flag 2 (hold last frame)
                Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, player, iMDict, iMAnim,
                    pp.X, pp.Y, pp.Z, iRotX, 0f, player.Heading,
                    8.0f, -4.0f, -1, 2, mStartPhase, 2, 0);
                Function.Call(Hash.TASK_PLAY_ANIM, girl, iFDict, iFAnim,
                    8.0f, -4.0f, -1, 2, 0f, false, false, false);

                float p1Dur = Function.Call<float>(Hash.GET_ANIM_DURATION, iMDict, iMAnim);
                if (mStartPhase > 0f) p1Dur *= (1f - mStartPhase); // Effective remaining duration
                if (idx == 1 || idx == 4)
                {
                    // Facesitting/Laying BJ: time phase on female P1 only — male t_getin plays independently
                    float fP1Dur = Function.Call<float>(Hash.GET_ANIM_DURATION, iFDict, iFAnim);
                    p1Dur = fP1Dur;
                }
                else if (iMDict != iFDict) { float fP1Dur = Function.Call<float>(Hash.GET_ANIM_DURATION, iFDict, iFAnim); p1Dur = Math.Max(p1Dur, fP1Dur); }
                animWaitTime = Game.GameTime + (int)(p1Dur * 1000f);
                animPhase = 0;
                state = ScriptState.Animating;
                lastVoiceTime = Game.GameTime;
                ShowHudStatus("~g~正在啟動……~s~(~y~按下 " + MasterDisplay + " TO ABORT~s~)", 2000);
                return;
            }

            // Setup
            // 1. Disable collision between the two peds
            Function.Call(Hash.SET_ENTITY_NO_COLLISION_ENTITY, girl, player, false);

            Vector3 girlPos = player.GetOffsetPosition(offset);

            // 2. For sitting anims: disable collision and sink peds underground.
            //    The car-seat animation lifts them back up to ground level visually.
            //    Same trick used in old GTA IV HotCoffee (zOffset = -2.12f etc.)
            if (isSitting)
            {
                Function.Call(Hash.SET_ENTITY_COLLISION, player, false, false);
                Function.Call(Hash.SET_ENTITY_COLLISION, girl, false, false);
            }

            Vector3 playerPos = new Vector3(player.Position.X, player.Position.Y, player.Position.Z + zOffset);
            Vector3 girlFinal = new Vector3(girlPos.X, girlPos.Y, girlPos.Z + zOffset);

            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, player,
                playerPos.X, playerPos.Y, playerPos.Z, false, false, false);
            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                girlFinal.X, girlFinal.Y, girlFinal.Z, false, false, false);
            girl.Heading = player.Heading + headingOff;

            // 3. Freeze both peds at their (possibly sunken) positions
            player.IsPositionFrozen = true;
            girl.IsPositionFrozen = true;

            // 4. Make everyone ignore
            Function.Call(Hash.SET_EVERYONE_IGNORE_PLAYER, Game.Player, true);

            Wait(100);

            // Standing BJ: play intro anims (zip/kneel) then transition to loop via blackscreen
            if (idx == 2)
            {
                string introMDict = "trevor_1_int-15";
                string introMAnim = "player_two_dual-15";
                string introFDict = "missheist_jewel";
                string introFAnim = "manageress_kneel_intro";
                if (!RequestAnimDict(introMDict) || !RequestAnimDict(introFDict))
                { ShowHudStatus("~r~動畫載入失敗！", 2000); EndAnimation(); return; }

                Function.Call(Hash.TASK_PLAY_ANIM, player, introMDict, introMAnim,
                    8.0f, -4.0f, -1, 50, 0f, false, false, false);  // flag 50 = upper-body(16) + secondary(32) + hold-last-frame(2)
                Function.Call(Hash.TASK_PLAY_ANIM, girl, introFDict, introFAnim,
                    8.0f, -4.0f, -1, 10, 0f, false, false, false);

                // Overlay upper-body idle so only the lower body shows the kneeling motion
                string ubIdleDict = "amb@code_human_cross_road@female@idle_a";
                if (RequestAnimDict(ubIdleDict))
                    Function.Call(Hash.TASK_PLAY_ANIM, girl, ubIdleDict, "idle_a",
                        8.0f, -4.0f, -1, 50, 0f, false, false, false); // flag 50 = hold-last-frame(2) + upper-body(16) + secondary(32)

                Function.Call(Hash.SET_PED_KEEP_TASK, girl, true);
                streetActionIdx = idx;
                AttachDildoProp(player);
                PlayBjCustom(_bjPantsZipFile);

                state = ScriptState.Animating;
                animPhase = 0;
                animWaitTime = Game.GameTime + 1500; // trigger fade at 1.5s
                lastVoiceTime = Game.GameTime;
                ShowHudStatus("~g~正在啟動……~s~(~y~按下 " + MasterDisplay + " TO ABORT~s~)", 2000);
                return;
            }

            // Wall Sex: play intro anims (zip + female approach) then transition to loop via blackscreen
            if (idx == 5)
            {
                string introMDict = "trevor_1_int-15";
                string introMAnim = "player_two_dual-15";
                string introFDict = "switch@michael@prostitute";
                string introFAnim = "exit_hooker";
                if (!RequestAnimDict(introMDict) || !RequestAnimDict(introFDict))
                { ShowHudStatus("~r~動畫載入失敗！", 2000); EndAnimation(); return; }

                // Player: upper-body zip only
                Function.Call(Hash.TASK_PLAY_ANIM, player, introMDict, introMAnim,
                    8.0f, -4.0f, -1, 50, 0f, false, false, false);  // flag 50 = upper-body + secondary + hold-last-frame

                // Reposition girl to kissing offset for intro (loop will reposition to wall sex offset)
                // Compensate for the -0.15f zOffset that sank the player — girl plays a standing anim
                Vector3 girlIntroPos = player.GetOffsetPosition(new Vector3(0.0f, 1.29f, 0.07f));
                Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                    girlIntroPos.X, girlIntroPos.Y, girlIntroPos.Z - zOffset, false, false, false);
                girl.Heading = player.Heading + 180f + 90f; // kissing heading + 90° right

                // Girl: play exit_hooker starting at ~2.50s
                float introFDur = Function.Call<float>(Hash.GET_ANIM_DURATION, introFDict, introFAnim);
                float introFStartPhase = (introFDur > 0f) ? Math.Min(2.50f / introFDur, 0.99f) : 0.5f;
                Function.Call(Hash.TASK_PLAY_ANIM, girl, introFDict, introFAnim,
                    8.0f, -4.0f, -1, 10, introFStartPhase, false, false, false); // flag 10 = hold-last-frame + non-interruptable

                Function.Call(Hash.SET_PED_KEEP_TASK, girl, true);
                streetActionIdx = idx;
                AttachDildoProp(player);
                PlayBjCustom(_bjPantsZipFile);

                state = ScriptState.Animating;
                animPhase = 0;
                animWaitTime = Game.GameTime + 1500; // trigger fade at 1.5s
                lastVoiceTime = Game.GameTime;
                ShowHudStatus("~g~正在啟動……~s~(~y~按下 " + MasterDisplay + " TO ABORT~s~)", 2000);
                return;
            }

            // Doggy / Rough Doggy: same intro/outro system as Wall Sex
            if (idx == 10 || idx == 11)
            {
                string introMDict = "trevor_1_int-15";
                string introMAnim = "player_two_dual-15";
                string introFDict = "switch@michael@prostitute";
                string introFAnim = "exit_hooker";
                if (!RequestAnimDict(introMDict) || !RequestAnimDict(introFDict))
                { ShowHudStatus("~r~動畫載入失敗！", 2000); EndAnimation(); return; }

                // Player: upper-body zip only
                Function.Call(Hash.TASK_PLAY_ANIM, player, introMDict, introMAnim,
                    8.0f, -4.0f, -1, 50, 0f, false, false, false);

                // Reposition girl to kissing offset for intro (loop will reposition to correct offset)
                Vector3 girlIntroPos = player.GetOffsetPosition(new Vector3(0.0f, 1.29f, 0.07f));
                Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                    girlIntroPos.X, girlIntroPos.Y, girlIntroPos.Z, false, false, false);
                girl.Heading = player.Heading + 180f + 90f; // kissing heading + 90° right

                // Girl: play exit_hooker starting at ~2.50s
                float introFDur = Function.Call<float>(Hash.GET_ANIM_DURATION, introFDict, introFAnim);
                float introFStartPhase = (introFDur > 0f) ? Math.Min(2.50f / introFDur, 0.99f) : 0.5f;
                Function.Call(Hash.TASK_PLAY_ANIM, girl, introFDict, introFAnim,
                    8.0f, -4.0f, -1, 10, introFStartPhase, false, false, false);

                Function.Call(Hash.SET_PED_KEEP_TASK, girl, true);
                streetActionIdx = idx;
                AttachDildoProp(player);
                PlayBjCustom(_bjPantsZipFile);

                state = ScriptState.Animating;
                animPhase = 0;
                animWaitTime = Game.GameTime + 1500;
                lastVoiceTime = Game.GameTime;
                ShowHudStatus("~g~正在啟動……~s~(~y~按下 " + MasterDisplay + " TO ABORT~s~)", 2000);
                return;
            }

            // 5. Play animations - flag 9 = AF_LOOPING(1) + AF_NOT_INTERRUPTABLE(8)
            //    Kissing uses flag 10 = AF_HOLD_LAST_FRAME(2) + AF_NOT_INTERRUPTABLE(8)
            //    so we can manage intro/loop/outro phases manually.
            // cowgirl_clip runs at half the intended speed — double it invisibly so the HUD still shows 1.00x
            _femaleAnimSpeedScale = (fAnim == "cowgirl_clip") ? 2.0f : 1.0f;
            float startPhase = (mDict == "trevor_1_int-13") ? 0.205f : 0.0f;
            int animFlag = (idx == 0) ? 10 : 9; // Kissing: hold-last-frame + non-interruptable; others: looping + non-interruptable
            if (rotX != 0.0f)
            {
                // Use TASK_PLAY_ANIM_ADVANCED for player to apply pitch rotation
                Vector3 pp = player.Position;
                Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, player, mDict, mAnim,
                    pp.X, pp.Y, pp.Z,
                    rotX, 0.0f, player.Heading,
                    8.0f, -4.0f, -1, animFlag, startPhase, 2, 0);
            }
            else
            {
                Function.Call(Hash.TASK_PLAY_ANIM, player, mDict, mAnim,
                    8.0f, -4.0f, -1, animFlag, startPhase, false, false, false);
            }
            Function.Call(Hash.TASK_PLAY_ANIM, girl, fDict, fAnim,
                8.0f, -4.0f, -1, animFlag, startPhase, false, false, false);

            // Lock the animation task NOW (after it's been given, not before)
            Function.Call(Hash.SET_PED_KEEP_TASK, girl, true);

            streetActionIdx = idx;
            if (currentAction != "kissing" && idx != 1) AttachDildoProp(player);

            Wait(200);

            // Verify at least one started
            bool mPlaying = Function.Call<bool>(Hash.IS_ENTITY_PLAYING_ANIM, player, mDict, mAnim, 3);
            bool fPlaying = Function.Call<bool>(Hash.IS_ENTITY_PLAYING_ANIM, girl, fDict, fAnim, 3);
            if (!mPlaying && !fPlaying)
            {
                ShowHudStatus("~r~動畫執行失敗，請重試！", 2000);
                EndAnimation();
                return;
            }

            // Apply base female speed scale immediately (e.g. 2x for cowgirl_clip)
            if (_femaleAnimSpeedScale != 1.0f && fPlaying)
                Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, fDict, fAnim, animSpeed * _femaleAnimSpeedScale);

            state = ScriptState.Animating;
            animPhase = (idx == 0) ? 0 : 2; // Kissing: start at intro phase; others: jump straight to loop
            _kissingReverse = false;
            _standingBjReverse = false;
            _rdoggyReverse = false;
            _sitCowgirlPulloutReverse = false;
            lastVoiceTime = Game.GameTime;
            if (idx == 0)
                ShowHudStatus("~g~正在啟動……~s~(~y~按下 " + MasterDisplay + " TO ABORT~s~)", 2000);
        }

        /// <summary>
        /// Returns animation data for each street proposition phase.
        /// idx: 0 = sit-down, 1 = prop-enter, 2 = prop-loop, 3 = prop-exit, 4 = stand-up.
        /// </summary>
        private void GetStreetPropAnimData(int idx, out string mDict, out string mAnim,
            out string fDict, out string fAnim, out Vector3 offset, out float heading,
            out bool isSitting, out float zOffset, out float rotX, out float rotHorizontal)
        {
            rotHorizontal = 0f; // default — overridden per-case where needed
            switch (idx)
            {
                case 0: // Sit-down (picnic enter)
                default:
                    switch (_streetSitCategory)
                    {
                        case MenuLevel.SittingServices:
                            mDict = "amb@world_human_picnic@male@enter";   mAnim = "enter";
                            fDict = "amb@world_human_picnic@female@enter"; fAnim = "enter";
                            offset = new Vector3(-0.7f, 0.4f, 0.0f);
                            heading = 0f; 
                            isSitting = false; 
                            zOffset = 0f; 
                            rotX = 0f;
                            break;
                        case MenuLevel.LayingServices:
                            mDict = "amb@world_human_picnic@male@enter";   mAnim = "enter";
                            fDict = "amb@world_human_picnic@female@enter"; fAnim = "enter";
                            offset = new Vector3(-0.7f, 0.4f, 0.0f);
                            heading = 0f; 
                            isSitting = false; 
                            zOffset = 0f; 
                            rotX = 0f;
                            break;
                        default:
                            mDict = "amb@world_human_picnic@male@enter";   mAnim = "enter";
                            fDict = "amb@world_human_picnic@female@enter"; fAnim = "enter";
                            offset = new Vector3(-0.7f, 0.4f, 0.0f);
                            heading = 0f; 
                            isSitting = false; 
                            zOffset = 0f; 
                            rotX = 0f;
                            break;
                    }
                    break;
                case 1: // Proposition enter (into_proposition)
                    switch (_streetSitCategory)
                    {
                        case MenuLevel.SittingServices:
                            mDict = VEH_NORM_DICT; mAnim = "into_proposition_male";
                            fDict = VEH_NORM_DICT; fAnim = "into_proposition_prostitute";
                            offset = new Vector3(0.85f, -0.01f, -0.02f);
                            heading = 0f;
                            isSitting = true;
                            zOffset = -0.855f;
                            rotX = 5.0f;
                            break;
                        case MenuLevel.LayingServices:
                            mDict = VEH_NORM_DICT; mAnim = "into_proposition_male";
                            fDict = VEH_NORM_DICT; fAnim = "into_proposition_prostitute";
                            offset = new Vector3(0.85f, -0.01f, -0.02f);
                            heading = 0f;
                            isSitting = true;
                            zOffset = -0.855f;
                            rotX = 5.0f;
                            break;
                        default:
                            mDict = VEH_NORM_DICT; mAnim = "into_proposition_male";
                            fDict = VEH_NORM_DICT; fAnim = "into_proposition_prostitute";
                            offset = new Vector3(0.85f, -0.01f, -0.02f);
                            heading = 0f;
                            isSitting = true;
                            zOffset = -0.855f;
                            rotX = 5.0f;
                            break;
                    }
                    break;
                case 2: // Proposition loop
                    switch (_streetSitCategory)
                    {
                        case MenuLevel.SittingServices:
                            mDict = VEH_NORM_DICT; mAnim = "proposition_loop_male";
                            fDict = VEH_NORM_DICT; fAnim = "proposition_loop_prostitute";
                            offset = new Vector3(0.85f, -0.01f, -0.02f);
                            heading = 0f;
                            isSitting = true;
                            zOffset = -0.855f;
                            rotX = 5.0f;
                            break;
                        case MenuLevel.LayingServices:
                            mDict = VEH_NORM_DICT; mAnim = "proposition_loop_male";
                            fDict = VEH_NORM_DICT; fAnim = "proposition_loop_prostitute";
                            offset = new Vector3(0.85f, -0.01f, -0.02f);
                            heading = 0f;
                            isSitting = true;
                            zOffset = -0.855f;
                            rotX = 5.0f;
                            break;
                        default:
                            mDict = VEH_NORM_DICT; mAnim = "proposition_loop_male";
                            fDict = VEH_NORM_DICT; fAnim = "proposition_loop_prostitute";
                            offset = new Vector3(0.85f, -0.01f, -0.02f);
                            heading = 0f;
                            isSitting = true;
                            zOffset = -0.855f;
                            rotX = 5.0f;
                            break;
                    }
                    break;
                case 3: // Proposition exit (prop_to_sit)
                    switch (_streetSitCategory)
                    {
                        case MenuLevel.SittingServices:
                            mDict = VEH_NORM_DICT; mAnim = "prop_to_sit_male";
                            fDict = VEH_NORM_DICT; fAnim = "prop_to_sit_prostitute";
                            offset = new Vector3(0.85f, -0.01f, -0.02f);
                            heading = 0f;
                            isSitting = true;
                            zOffset = -0.855f;
                            rotX = 5.0f;
                            break;
                        case MenuLevel.LayingServices:
                            mDict = VEH_NORM_DICT; mAnim = "prop_to_sit_male";
                            fDict = VEH_NORM_DICT; fAnim = "prop_to_sit_prostitute";
                            offset = new Vector3(0.85f, -0.01f, -0.02f);
                            heading = 0f;
                            isSitting = true;
                            zOffset = -0.855f;
                            rotX = 5.0f;
                            break;
                        default:
                            mDict = VEH_NORM_DICT; mAnim = "prop_to_sit_male";
                            fDict = VEH_NORM_DICT; fAnim = "prop_to_sit_prostitute";
                            offset = new Vector3(0.85f, -0.01f, -0.02f);
                            heading = 0f;
                            isSitting = true;
                            zOffset = -0.855f;
                            rotX = 5.0f;
                            break;
                    }
                    break;
                case 4: // Stand-up (picnic exit)
                    switch (_streetSitCategory)
                    {
                        case MenuLevel.SittingServices:
                            mDict = "amb@world_human_picnic@male@exit";   mAnim = "exit";
                            fDict = "amb@world_human_picnic@female@exit"; fAnim = "exit";
                            offset = new Vector3(-0.7f, 0.4f, 0.0f);
                            heading = 185f; 
                            isSitting = false; 
                            zOffset = 0f; 
                            rotX = 0f;
                            rotHorizontal = 180f;
                            break;
                        case MenuLevel.LayingServices:
                            mDict = "amb@world_human_picnic@male@exit";   mAnim = "exit";
                            fDict = "amb@world_human_picnic@female@exit"; fAnim = "exit";
                            offset = new Vector3(-0.7f, 0.4f, 0.0f);
                            heading = 185f; 
                            isSitting = false; 
                            zOffset = 0f; 
                            rotX = 0f;
                            rotHorizontal = 180f;
                            break;
                        default:
                            mDict = "amb@world_human_picnic@male@exit";   mAnim = "exit";
                            fDict = "amb@world_human_picnic@female@exit"; fAnim = "exit";
                            offset = new Vector3(-0.7f, 0.4f, 0.0f);
                            heading = 185f; 
                            isSitting = false; 
                            zOffset = 0f; 
                            rotX = 0f;
                            rotHorizontal = 180f;
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// Returns animation data for street action intro/exit transitions.
        /// idx: 0 = prop→action p1, 1 = prop→action p2, 2 = action→prop p1, 3 = action→prop p2.
        /// actionIdx: the street action index (determines BJ vs sex transition anims).
        /// </summary>
        private void GetStreetPropIntroExitAnimData(int idx, int actionIdx, out string mDict, out string mAnim,
            out string fDict, out string fAnim, out Vector3 offset, out float heading,
            out bool isSitting, out float zOffset, out float rotX, out float rotHorizontal)
        {
            rotHorizontal = 0f;
            bool isBj = (actionIdx >= 2 && actionIdx <= 4);
            bool isFacesitting = (actionIdx == 1) || (actionIdx == 8 && _sinkraCowgirlAvailable && sinkraReplaceLaying);
            bool isLayingBj = (actionIdx == 4);
            bool isLayingCowgirl = (actionIdx == 8);
            bool isLayingRevCowgirl = (actionIdx == 9);
            bool isSinkraLayingCow = (actionIdx == 8 && _sinkraCowgirlAvailable && sinkraReplaceLaying);
            bool isSinkraLayingRevCow = (actionIdx == 9 && _sinkraCowgirlAvailable && sinkraReplaceRevLaying);

            // Vanilla Laying Cowgirl: uses random@drunk_driver_2 anims (completely different from VEH_NORM_DICT transitions)
            if (isLayingCowgirl && !(_sinkraCowgirlAvailable && sinkraReplaceLaying))
            {
                string DRUNK_DICT = "random@drunk_driver_2";
                switch (idx)
                {
                    case 0: default: mDict = DRUNK_DICT; mAnim = "cardrunkflirt_intro_m"; fDict = DRUNK_DICT; fAnim = "cardrunkflirt_intro_f"; break;
                    case 1: mDict = DRUNK_DICT; mAnim = "cardrunksex_intro_m"; fDict = DRUNK_DICT; fAnim = "cardrunksex_intro_f"; break;
                    case 2: mDict = DRUNK_DICT; mAnim = "cardrunksex_outro_m"; fDict = DRUNK_DICT; fAnim = "cardrunksex_outro_f"; break;
                    case 3: mDict = DRUNK_DICT; mAnim = "cardrunksex_outro_m"; fDict = DRUNK_DICT; fAnim = "cardrunksex_outro_f"; break;
                }
                offset = new Vector3(-0.65f, 0.0f, -0.02f);
                heading = 0f;
                isSitting = true;
                zOffset = -0.95f;
                rotX = 0f;
                rotHorizontal = 0f;
                return;
            }

            // Vanilla Laying Reversed Cowgirl: same dict, reversed offset/heading, skips P1 (cardrunkflirt) — only cardrunksex_intro
            if (isLayingRevCowgirl && !(_sinkraCowgirlAvailable && sinkraReplaceRevLaying))
            {
                string DRUNK_DICT = "random@drunk_driver_2";
                switch (idx)
                {
                    case 0: default: mDict = DRUNK_DICT; mAnim = "cardrunksex_intro_m"; fDict = DRUNK_DICT; fAnim = "cardrunksex_intro_f"; break;
                    case 1: mDict = DRUNK_DICT; mAnim = "cardrunksex_intro_m"; fDict = DRUNK_DICT; fAnim = "cardrunksex_intro_f"; break;
                    case 2: mDict = DRUNK_DICT; mAnim = "cardrunksex_outro_m"; fDict = DRUNK_DICT; fAnim = "cardrunksex_outro_f"; break;
                    case 3: mDict = DRUNK_DICT; mAnim = "cardrunksex_outro_m"; fDict = DRUNK_DICT; fAnim = "cardrunksex_outro_f"; break;
                }
                offset = new Vector3(-0.35f, 0.15f, -0.02f);
                heading = 180f;
                isSitting = true;
                zOffset = -0.95f;
                rotX = 0f;
                rotHorizontal = 0f;
                return;
            }

            switch (idx)
            {
                case 0: // Proposition to action part 1
                default:
                    switch (_streetSitCategory)
                    {
                        case MenuLevel.SittingServices:
                            if (isFacesitting || isLayingBj) { mDict = "saveveniceb@"; mAnim = "t_getin_veniceb"; }
                            else { mDict = VEH_NORM_DICT; mAnim = isBj ? "proposition_to_bj_p1_male" : "proposition_to_sex_p1_male"; }
                            fDict = VEH_NORM_DICT; fAnim = isBj ? "proposition_to_bj_p1_prostitute" : "proposition_to_sex_p1_prostitute";
                            offset = new Vector3(0.85f, -0.01f, -0.02f);
                            heading = 0f;
                            isSitting = true;
                            zOffset = 0.0f;
                            rotX = (isFacesitting || isLayingBj) ? 0f : 5.0f;
                            break;
                        case MenuLevel.LayingServices:
                            if (isFacesitting || isLayingBj) { mDict = "saveveniceb@"; mAnim = "t_getin_veniceb"; }
                            else { mDict = VEH_NORM_DICT; mAnim = isBj ? "proposition_to_bj_p1_male" : "proposition_to_sex_p1_male"; }
                            fDict = VEH_NORM_DICT; fAnim = isBj ? "proposition_to_bj_p1_prostitute" : "proposition_to_sex_p1_prostitute";
                            offset = new Vector3(0.85f, -0.01f, -0.02f);
                            heading = 0f;
                            isSitting = true;
                            zOffset = 0.0f;
                            rotX = (isFacesitting || isLayingBj) ? 0f : 5.0f;
                            break;
                        default:
                            if (isFacesitting || isLayingBj) { mDict = "saveveniceb@"; mAnim = "t_getin_veniceb"; }
                            else { mDict = VEH_NORM_DICT; mAnim = isBj ? "proposition_to_bj_p1_male" : "proposition_to_sex_p1_male"; }
                            fDict = VEH_NORM_DICT; fAnim = isBj ? "proposition_to_bj_p1_prostitute" : "proposition_to_sex_p1_prostitute";
                            offset = new Vector3(0.85f, -0.01f, -0.02f);
                            heading = 0f;
                            isSitting = true;
                            zOffset = 0.0f;
                            rotX = (isFacesitting || isLayingBj) ? 0f : 5.0f;
                            break;
                    }
                    break;
                case 1: // Proposition to action part 2
                    switch (_streetSitCategory)
                    {
                        case MenuLevel.SittingServices:
                            if (isFacesitting || isLayingBj) { mDict = "saveveniceb@"; mAnim = "t_sleep_loop_veniceb"; }
                            else { mDict = VEH_NORM_DICT; mAnim = isBj ? "proposition_to_bj_p2_male" : "proposition_to_sex_p2_male"; }
                            fDict = VEH_NORM_DICT; fAnim = isBj ? "proposition_to_bj_p2_prostitute" : "proposition_to_sex_p2_prostitute";
                            offset = new Vector3(0.85f, -0.01f, -0.02f);
                            heading = 0f;
                            isSitting = true;
                            zOffset = 0.0f;
                            rotX = (isFacesitting || isLayingBj) ? 0f : 5.0f;
                            break;
                        case MenuLevel.LayingServices:
                            if (isFacesitting || isLayingBj) { mDict = "saveveniceb@"; mAnim = "t_sleep_loop_veniceb"; }
                            else { mDict = VEH_NORM_DICT; mAnim = isBj ? "proposition_to_bj_p2_male" : "proposition_to_sex_p2_male"; }
                            fDict = VEH_NORM_DICT; fAnim = isBj ? "proposition_to_bj_p2_prostitute" : "proposition_to_sex_p2_prostitute";
                            offset = new Vector3(0.85f, -0.01f, -0.02f);
                            heading = 0f;
                            isSitting = true;
                            zOffset = 0.0f;
                            rotX = (isFacesitting || isLayingBj) ? 0f : 5.0f;
                            break;
                        default:
                            if (isFacesitting || isLayingBj) { mDict = "saveveniceb@"; mAnim = "t_sleep_loop_veniceb"; }
                            else { mDict = VEH_NORM_DICT; mAnim = isBj ? "proposition_to_bj_p2_male" : "proposition_to_sex_p2_male"; }
                            fDict = VEH_NORM_DICT; fAnim = isBj ? "proposition_to_bj_p2_prostitute" : "proposition_to_sex_p2_prostitute";
                            offset = new Vector3(0.85f, -0.01f, -0.02f);
                            heading = 0f;
                            isSitting = true;
                            zOffset = 0.0f;
                            rotX = (isFacesitting || isLayingBj) ? 0f : 5.0f;
                            break;
                    }
                    break;
                case 2: // Action to Proposition part 1
                    switch (_streetSitCategory)
                    {
                        case MenuLevel.SittingServices:
                            if (isFacesitting || isLayingBj) { mDict = "saveveniceb@"; mAnim = "t_getout_veniceb"; }
                            else { mDict = VEH_NORM_DICT; mAnim = isBj ? "bj_to_proposition_p1_male" : "sex_to_proposition_p1_male"; }
                            fDict = VEH_NORM_DICT; fAnim = isBj ? "bj_to_proposition_p1_prostitute" : "sex_to_proposition_p1_prostitute";
                            offset = new Vector3(0.85f, -0.01f, -0.02f);
                            heading = 0f;
                            isSitting = true;
                            zOffset = 0.0f;
                            rotX = (isFacesitting || isLayingBj) ? 0f : 5.0f;
                            break;
                        case MenuLevel.LayingServices:
                            if (isFacesitting || isLayingBj) { mDict = "saveveniceb@"; mAnim = "t_getout_veniceb"; }
                            else { mDict = VEH_NORM_DICT; mAnim = isBj ? "bj_to_proposition_p1_male" : "sex_to_proposition_p1_male"; }
                            fDict = VEH_NORM_DICT; fAnim = isBj ? "bj_to_proposition_p1_prostitute" : "sex_to_proposition_p1_prostitute";
                            offset = new Vector3(0.85f, -0.01f, -0.02f);
                            heading = 0f;
                            isSitting = true;
                            zOffset = 0.0f;
                            rotX = (isFacesitting || isLayingBj) ? 0f : 5.0f;
                            break;
                        default:
                            if (isFacesitting || isLayingBj) { mDict = "saveveniceb@"; mAnim = "t_getout_veniceb"; }
                            else { mDict = VEH_NORM_DICT; mAnim = isBj ? "bj_to_proposition_p1_male" : "sex_to_proposition_p1_male"; }
                            fDict = VEH_NORM_DICT; fAnim = isBj ? "bj_to_proposition_p1_prostitute" : "sex_to_proposition_p1_prostitute";
                            offset = new Vector3(0.85f, -0.01f, -0.02f);
                            heading = 0f;
                            isSitting = true;
                            zOffset = 0.0f;
                            rotX = (isFacesitting || isLayingBj) ? 0f : 5.0f;
                            break;
                    }
                    break;
                case 3: // Action to Proposition part 2
                    switch (_streetSitCategory)
                    {
                        case MenuLevel.SittingServices:
                            mDict = VEH_NORM_DICT; mAnim = isBj ? "bj_to_proposition_p2_male" : "sex_to_proposition_p2_male";
                            fDict = VEH_NORM_DICT; fAnim = isBj ? "bj_to_proposition_p2_prostitute" : "sex_to_proposition_p2_prostitute";
                            offset = new Vector3(0.85f, -0.01f, -0.02f);
                            heading = 0f;
                            isSitting = true;
                            zOffset = 0.0f;
                            rotX = 5.0f;
                            break;
                        case MenuLevel.LayingServices:
                            mDict = VEH_NORM_DICT; mAnim = isBj ? "bj_to_proposition_p2_male" : "sex_to_proposition_p2_male";
                            fDict = VEH_NORM_DICT; fAnim = isBj ? "bj_to_proposition_p2_prostitute" : "sex_to_proposition_p2_prostitute";
                            offset = new Vector3(0.85f, -0.01f, -0.02f);
                            heading = 0f;
                            isSitting = true;
                            zOffset = 0.0f;
                            rotX = 5.0f;
                            break;
                        default:
                            mDict = VEH_NORM_DICT; mAnim = isBj ? "bj_to_proposition_p2_male" : "sex_to_proposition_p2_male";
                            fDict = VEH_NORM_DICT; fAnim = isBj ? "bj_to_proposition_p2_prostitute" : "sex_to_proposition_p2_prostitute";
                            offset = new Vector3(0.85f, -0.01f, -0.02f);
                            heading = 0f;
                            isSitting = true;
                            zOffset = 0.0f;
                            rotX = 5.0f;
                            break;
                    }
                    break;
            }

            // Sinkra Laying Rev Cowgirl: use laying male anims (like facesitting/sinkra cowgirl)
            if (isSinkraLayingRevCow)
            {
                if (idx == 0) { mDict = "saveveniceb@"; mAnim = "t_getin_veniceb"; rotX = 0f; }
                else if (idx == 1) { mDict = "saveveniceb@"; mAnim = "t_sleep_loop_veniceb"; rotX = 0f; }
                else if (idx == 2) { mDict = "saveveniceb@"; mAnim = "t_getout_veniceb"; rotX = 0f; }
            }

            // Override zOffset for Z repositioning:
            // Facesitting/Laying BJ/Sinkra Laying Rev Cowgirl transitions need the action Z (-0.59f);
            // all others stay at proposition Z (-0.855f).
            // Case 3 (P2 outro) always uses proposition Z since we reposition back before playing it.
            zOffset = (idx <= 2 && (isFacesitting || isLayingBj || isSinkraLayingRevCow)) ? -0.59f : -0.855f;

        }

        /// <summary>
        /// Returns animation data for each street action.
        /// </summary>
        private void GetStreetAnimData(int idx, out string mDict, out string mAnim,
            out string fDict, out string fAnim, out Vector3 offset, out float heading,
            out bool isSitting, out float zOffset, out float rotX)
        {
            switch (idx)
            {
                case 0: // Kissing (mp_ped_interaction)
                default:
                    mDict = "mp_ped_interaction";
                    mAnim = "kisses_guy_a";
                    fDict = "mp_ped_interaction";
                    fAnim = "kisses_guy_a";
                    offset = new Vector3(0.0f, 1.29f, 0.07f);
                    heading = 180f;
                    isSitting = false;
                    zOffset = 0.0f;
                    rotX = 0.0f;
                    break;
                case 1: // Facesitting
                    if (_sinkraCowgirlAvailable && sinkraReplaceFacesitting)
                    {
                        // Modded anim
                        mDict = "saveveniceb@";
                        mAnim = "t_sleep_loop_veniceb";
                        fDict = "cowgirl@sinkra";
                        fAnim = "cowgirl_clip";
                        offset   = IsSinkraLowerModel() ? _sinkraLowerOffsetFacesit : new Vector3(-0.05f, -0.50f, 0.48f);
                        heading  = 169f;
                        isSitting = true;
                        zOffset = -0.59f;
                        rotX     = 0.0f;
                    }
                    else
                    {
                        // Vanilla
                        mDict = "saveveniceb@";
                        mAnim = "t_sleep_loop_veniceb";
                        fDict = "mini@prostitutes@sexnorm_veh";
                        fAnim = "sex_loop_prostitute";
                        offset   = new Vector3(0.73f, -0.77f, -0.3f);
                        heading  = 0f;
                        isSitting = true;
                        zOffset  = -0.59f;
                        rotX     = 0.0f;
                    }
                    break;
                case 2: // Standing Blowjob (misscarsteal2pimpsex)
                    mDict = "misscarsteal2pimpsex";
                    mAnim = "pimpsex_punter";
                    fDict = "misscarsteal2pimpsex";
                    fAnim = "pimpsex_hooker";
                    offset = new Vector3(0.0f, 0.6f, 0.0f);
                    heading = 180f;
                    isSitting = false;
                    zOffset = 0.0f;
                    rotX = 0.0f;
                    break;
                case 3: // Sitting Blowjob (mini@prostitutes@sexnorm_veh)
                    mDict = "mini@prostitutes@sexnorm_veh";
                    mAnim = "bj_loop_male";
                    fDict = "mini@prostitutes@sexnorm_veh";
                    fAnim = "bj_loop_prostitute";
                    offset = new Vector3(0.78f, 0.0f, -0.02f);
                    heading = 0f;
                    isSitting = true;
                    zOffset = -0.87f;
                    rotX = 5.0f;
                    break;
                case 4: // Laying Blowjob (saveveniceb@ + sexnorm_veh)
                    mDict = "saveveniceb@";
                    mAnim = "t_sleep_loop_veniceb";
                    fDict = "mini@prostitutes@sexnorm_veh";
                    fAnim = "bj_loop_prostitute";
                    offset = new Vector3(0.17f, 0.8f, -0.37f);
                    heading = 90f;
                    isSitting = true;
                    zOffset = -0.59f;
                    rotX = 0.0f;
                    break;
                case 5: // Wall Banger (misscarsteal2pimpsex)
                    mDict = "misscarsteal2pimpsex";
                    mAnim = "shagloop_pimp";
                    fDict = "misscarsteal2pimpsex";
                    fAnim = "shagloop_hooker";
                    offset = IsSinkraLowerModel() ? _wallBangLowerOffset : new Vector3(0.02f, 0.36f, 0.048f);
                    heading = 190f;
                    isSitting = true;
                    zOffset = -0.15f;
                    rotX = 0.0f;
                    break;
                case 6: // Sitting Cowgirl (mini@prostitutes@sexnorm_veh)
                    mDict = "mini@prostitutes@sexnorm_veh";
                    mAnim = "sex_loop_male";
                    fDict = "mini@prostitutes@sexnorm_veh";
                    fAnim = "sex_loop_prostitute";
                    offset = new Vector3(0.85f, -0.01f, -0.02f);
                    heading = 0f;
                    isSitting = true;
                    zOffset = 0.0f;
                    rotX = 5.0f;
                    break;
                case 7: // Reverse Cowgirl (mini@prostitutes@sexnorm_veh)
                    mDict = "mini@prostitutes@sexnorm_veh";
                    mAnim = "sex_loop_male";
                    fDict = "mini@prostitutes@sexnorm_veh";
                    fAnim = "sex_loop_prostitute";
                    offset = new Vector3(-0.83f, 0.4f, 0.02f);
                    heading = 180f;
                    isSitting = true;
                    zOffset = -0.87f;
                    rotX = 5.0f;
                    break;
                case 8: // Laying Cowgirl
                    if (_sinkraCowgirlAvailable && sinkraReplaceLaying)
                    {
                        // Modded anim
                        mDict = "saveveniceb@";
                        mAnim = "t_sleep_loop_veniceb";
                        fDict = "cowgirl@sinkra";
                        fAnim = "cowgirl_clip";
                        offset   = IsSinkraLowerModel() ? _sinkraLowerOffsetCowgirl : new Vector3(0.02f, 0.08f, 0.44f);
                        heading  = 169f;
                        isSitting = true;
                        zOffset = -0.59f;
                        rotX     = 0.0f;
                    }
                    else
                    {
                        // Vanilla fallback
                        mDict = "random@drunk_driver_2";
                        mAnim = "cardrunksex_loop_m";
                        fDict = "random@drunk_driver_2";
                        fAnim = "cardrunksex_loop_f";
                        offset   = new Vector3(-0.65f, 0.0f, -0.02f);
                        heading  = 0f;
                        isSitting = true;
                        zOffset  = -0.95f;
                        rotX     = 0.0f;
                    }
                    break;
                case 9: // Laying Reversed Cowgirl
                    if (_sinkraCowgirlAvailable && sinkraReplaceRevLaying)
                    {
                        // Modded anim
                        mDict = "saveveniceb@";
                        mAnim = "t_sleep_loop_veniceb";
                        fDict = "cowgirl@sinkra";
                        fAnim = "cowgirl_clip";
                        offset   = IsSinkraLowerModel() ? _sinkraLowerOffsetRevCowgirl : new Vector3(0.02f, -0.075f, 0.44f);
                        heading  = 0f;
                        isSitting = true;
                        zOffset = -0.59f;
                        rotX     = 0.0f;
                    }
                    else
                    {
                        // Vanilla fallback
                        mDict = "random@drunk_driver_2";
                        mAnim = "cardrunksex_loop_m";
                        fDict = "random@drunk_driver_2";
                        fAnim = "cardrunksex_loop_f";
                        offset   = new Vector3(-0.35f, 0.15f, -0.02f);
                        heading  = 180f;
                        isSitting = true;
                        zOffset  = -0.95f;
                        rotX     = 0.0f;
                    }
                    break;
                case 10: // Doggy Style (rcmpaparazzo_2)
                    mDict = "rcmpaparazzo_2";
                    mAnim = "shag_loop_a";
                    fDict = "rcmpaparazzo_2";
                    fAnim = "shag_loop_poppy";
                    offset = new Vector3(-0.03f, 0.3f, -0.1f);
                    heading = 0f;
                    isSitting = false;
                    zOffset = 0.0f;
                    rotX = 0.0f;
                    break;
                case 11: // Rough Doggy Style (trevor_1_int-13)
                    mDict = "trevor_1_int-13";
                    mAnim = "player_two_dual-13";
                    fDict = "trevor_1_int-13";
                    fAnim = "cs_ashley_dual-13";
                    offset = new Vector3(-0.11f, 0.33f, -0.11f);
                    heading = 13f;
                    isSitting = false;
                    zOffset = 0.0f;
                    rotX = 0.0f;
                    break;
            }
        }

        // ═════════════════════════════════════
        // CAR ANIMATION TICK (runs every frame)
        // ═════════════════════════════════════
        private void RunCarAnimation(Ped player)
        {
            // ── Backseat car sex: simplified 3-phase system (intro → loop → outro) ──
            if (_backseatCarSex)
            {
                const string DRUNK_DICT = "random@drunk_driver_2";
                // Backseat pull-out: enforce -1f speed every frame while reversed intro is playing
                if (_carPulloutReverse && animPhase == 3)
                {
                    Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, DRUNK_DICT, "cardrunksex_intro_m", -1.0f);
                    if (girl != null && girl.Exists())
                        Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, DRUNK_DICT, "cardrunksex_intro_f", -1.0f);
                }
                if (animPhase == 0) // INTRO: cardrunksex_intro → directly to loop
                {
                    if (animWaitTime <= Game.GameTime)
                    {
                        RequestAnimDict(DRUNK_DICT);
                        Function.Call(Hash.TASK_PLAY_ANIM, girl, DRUNK_DICT, currentFemaleAnim,
                            8.0f, -4.0f, -1, 1, 0.0f, false, false, false);
                        Function.Call(Hash.TASK_PLAY_ANIM, player, DRUNK_DICT, currentMaleAnim,
                            8.0f, -4.0f, -1, driveableCar ? 33 : 1, 0.0f, false, false, false);
                        animPhase = 2;
                        lastShakeTime = Game.GameTime + 500;
                    }
                }
                else if (animPhase == 2) // LOOP: car shake
                {
                    if (animSpeed >= 0.75f && player.IsInVehicle())
                    {
                        Vehicle car = player.CurrentVehicle;
                        if (car != null)
                        {
                            int now = Game.GameTime;
                            int shakeInterval = (int)(670f / animSpeed);
                            if (now - lastShakeTime >= shakeInterval)
                            {
                                float force = 0.65f * Math.Min(animSpeed, 2.0f);
                                Vector3 down = -force * car.UpVector;
                                car.ApplyForce(down, Vector3.Zero, (ForceType)3);
                                lastShakeTime = now;
                            }
                        }
                    }
                }
                else if (animPhase == 3) // OUTRO: cardrunksex_outro — single phase, finish directly
                {
                    if (animWaitTime <= Game.GameTime)
                    {
                        // Backseat pull-out: reversed intro done → zip + finish
                        if (_carPulloutReverse)
                        {
                            PlayBjCustom(_bjPantsZipFile, true);
                            PlayFinishSounds();
                            FinishCarServiceAndReopen();
                            return;
                        }
                        PlayBjCustom(_bjPantsZipFile, true);
                        PlayFinishSounds();
                        FinishCarServiceAndReopen();
                    }
                }
                return;
            }

            // Check if player is in first person
            int camMode = Function.Call<int>(Hash.GET_FOLLOW_PED_CAM_VIEW_MODE);
            bool isFirstPerson = (camMode == 4);
            
            string dict;
            if (vehicleAnimType == VehicleAnimType.Low)
                dict = isFirstPerson ? VEH_LOW_FP_DICT : VEH_LOW_DICT;
            else
                dict = isFirstPerson ? VEH_NORM_FP_DICT : VEH_NORM_DICT;

            // Car BJ/Sex pull-out: TASK_PLAY_ANIM speed is silently ignored on the same tick it's issued,
            // so enforce -1f every frame while the reversed anims are playing.
            if (_carPulloutReverse && (animPhase == 3 || animPhase == 4))
            {
                bool isLow = vehicleAnimType == VehicleAnimType.Low;
                bool isBjE = currentAction == "blowjob";
                string revFA = (animPhase == 3)
                    ? (isBjE ? (isLow ? "low_car_prop_to_bj_p2_female" : "proposition_to_bj_p2_prostitute")
                              : (isLow ? "low_car_prop_to_sex_p2_female" : "proposition_to_sex_p2_prostitute"))
                    : (isBjE ? (isLow ? "low_car_prop_to_bj_p1_female" : "proposition_to_bj_p1_prostitute")
                              : (isLow ? "low_car_prop_to_sex_p1_female" : "proposition_to_sex_p1_prostitute"));
                string revMA = (animPhase == 3)
                    ? (isBjE ? (isLow ? "low_car_prop_to_bj_p2_player" : "proposition_to_bj_p2_male")
                              : (isLow ? "low_car_prop_to_sex_p2_player" : "proposition_to_sex_p2_male"))
                    : (isBjE ? (isLow ? "low_car_prop_to_bj_p1_player" : "proposition_to_bj_p1_male")
                              : (isLow ? "low_car_prop_to_sex_p1_player" : "proposition_to_sex_p1_male"));
                Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, dict, revMA, -1.0f);
                if (girl != null && girl.Exists())
                    Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, dict, revFA, -1.0f);
            }

            if (animPhase == 0) // INTRO: proposition_to_[action]_p1
            {
                if (animWaitTime <= Game.GameTime)
                {
                    // Move to p2 (flag 2 = hold last frame)
                    string fP2, mP2;
                    
                    if (currentAction == "blowjob")
                    {
                        if (vehicleAnimType == VehicleAnimType.Low)
                        {
                            fP2 = "low_car_prop_to_bj_p2_female";
                            mP2 = "low_car_prop_to_bj_p2_player";
                        }
                        else
                        {
                            fP2 = "proposition_to_bj_p2_prostitute";
                            mP2 = "proposition_to_bj_p2_male";
                        }
                    }
                    else // sex
                    {
                        if (vehicleAnimType == VehicleAnimType.Low)
                        {
                            fP2 = "low_car_prop_to_sex_p2_female";
                            mP2 = "low_car_prop_to_sex_p2_player";
                        }
                        else
                        {
                            fP2 = "proposition_to_sex_p2_prostitute";
                            mP2 = "proposition_to_sex_p2_male";
                        }
                    }
                    
                    Function.Call(Hash.TASK_PLAY_ANIM, girl, dict, fP2,
                        8.0f, -4.0f, -1, 2, 0.0f, false, false, false);
                    Function.Call(Hash.TASK_PLAY_ANIM, player, dict, mP2,
                        8.0f, -4.0f, -1, driveableCar ? 34 : 2, 0.0f, false, false, false);
                    float p2Dur = Function.Call<float>(Hash.GET_ANIM_DURATION, dict, fP2);
                    animWaitTime = Game.GameTime + (int)(p2Dur * 1000f);
                    // Car intro p2: play pants-zip and attach dildo as she moves into position
                    if (currentAction == "blowjob")
                    {
                        PlayBjCustom(_bjPantsZipFile);
                        AttachDildoProp(Game.Player.Character);
                    }
                    animPhase = 1;
                }
            }
            else if (animPhase == 1) // INTRO: proposition_to_[action]_p2
            {
                if (animWaitTime <= Game.GameTime)
                {
                    // Move to LOOP
                    Function.Call(Hash.TASK_PLAY_ANIM, girl, dict, currentFemaleAnim,
                        8.0f, -4.0f, -1, 1, 0.0f, false, false, false);
                    Function.Call(Hash.TASK_PLAY_ANIM, player, dict, currentMaleAnim,
                        8.0f, -4.0f, -1, driveableCar ? 33 : 1, 0.0f, false, false, false);
                    animPhase = 2;
                    lastShakeTime = Game.GameTime + 500;
                }
            }
            else if (animPhase == 2) // LOOP - shake car during sex
            {
                if (currentAction == "sex" && animSpeed >= 0.75f && player.IsInVehicle())
                {
                    Vehicle car = player.CurrentVehicle;
                    if (car != null)
                    {
                        int now = Game.GameTime;
                        int shakeInterval = (int)(670f / animSpeed); // Faster anim = faster bounce
                        if (now - lastShakeTime >= shakeInterval)
                        {
                            float force = 0.65f * Math.Min(animSpeed, 2.0f); // Stronger bounce, capped at 2x
                            Vector3 down = -force * car.UpVector;
                            car.ApplyForce(down, Vector3.Zero, (ForceType)3);
                            lastShakeTime = now;
                        }
                    }
                }
            }
            else if (animPhase == 3) // OUTRO: [action]_to_proposition_p1
            {
                if (animWaitTime <= Game.GameTime)
                {
                    // Car BJ/Sex pull-out: P2rev done → play P1 reversed
                    if (_carPulloutReverse)
                    {
                        bool isLow3 = vehicleAnimType == VehicleAnimType.Low;
                        bool isBjP3 = currentAction == "blowjob";
                        string fP1r = isBjP3
                            ? (isLow3 ? "low_car_prop_to_bj_p1_female" : "proposition_to_bj_p1_prostitute")
                            : (isLow3 ? "low_car_prop_to_sex_p1_female" : "proposition_to_sex_p1_prostitute");
                        string mP1r = isBjP3
                            ? (isLow3 ? "low_car_prop_to_bj_p1_player" : "proposition_to_bj_p1_male")
                            : (isLow3 ? "low_car_prop_to_sex_p1_player" : "proposition_to_sex_p1_male");
                        Function.Call(Hash.TASK_PLAY_ANIM, girl, dict, fP1r, 8.0f, -4.0f, -1, 2, 1.0f, false, false, false);
                        Function.Call(Hash.TASK_PLAY_ANIM, player, dict, mP1r, 8.0f, -4.0f, -1, driveableCar ? 34 : 2, 1.0f, false, false, false);
                        Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, dict, mP1r, -1.0f);
                        Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, dict, fP1r, -1.0f);
                        float p1rDur = Function.Call<float>(Hash.GET_ANIM_DURATION, dict, fP1r);
                        animWaitTime = Game.GameTime + (int)(p1rDur * 1000f);
                        animPhase = 4;
                        return;
                    }
                    // Move to p2 exit stage (flag 0 = play once, no hold)
                    string fP2Exit, mP2Exit;
                    
                    if (currentAction == "blowjob")
                    {
                        if (vehicleAnimType == VehicleAnimType.Low)
                        {
                            fP2Exit = "low_car_bj_to_prop_p2_female";
                            mP2Exit = "low_car_bj_to_prop_p2_player";
                        }
                        else
                        {
                            fP2Exit = "bj_to_proposition_p2_prostitute";
                            mP2Exit = "bj_to_proposition_p2_male";
                        }
                    }
                    else // sex
                    {
                        if (vehicleAnimType == VehicleAnimType.Low)
                        {
                            fP2Exit = "low_car_sex_to_prop_p2_female";
                            mP2Exit = "low_car_sex_to_prop_p2_player";
                        }
                        else
                        {
                            fP2Exit = "sex_to_proposition_p2_prostitute";
                            mP2Exit = "sex_to_proposition_p2_male";
                        }
                    }
                    
                    Function.Call(Hash.TASK_PLAY_ANIM, girl, dict, fP2Exit,
                        8.0f, -4.0f, -1, 0, 0.0f, false, false, false);
                    Function.Call(Hash.TASK_PLAY_ANIM, player, dict, mP2Exit,
                        8.0f, -4.0f, -1, driveableCar ? 32 : 0, 0.0f, false, false, false);

                    float p2ExitDur = Function.Call<float>(Hash.GET_ANIM_DURATION, dict, fP2Exit);
                    animWaitTime = Game.GameTime + (int)(p2ExitDur * 1000f);
                    // Car BJ outro p2: swallow sound as she finishes.
                    if (bjSoundsEnabled && currentAction == "blowjob")
                    {
                        bool skipSwallowCar = !_finishInside || _finishForced || _finishBroke;
                        if (!skipSwallowCar) PlayBjCustom(_bjSwallowFile, true);
                        else if (aLifeMode && (_finishForced || _finishBroke)) PlayBjSpit();
                    }
                    animPhase = 4;
                }
            }
            else if (animPhase == 4) // OUTRO: [action]_to_proposition_p2
            {
                if (animWaitTime <= Game.GameTime)
                {
                    // Car BJ pull-out: P1rev done → zip + finish
                    if (_carPulloutReverse)
                    {
                        _carPulloutReverse = false;
                        PlayBjCustom(_bjPantsZipFile, true);
                        PlayFinishSounds();
                        FinishCarServiceAndReopen();
                        return;
                    }
                    // Car sex/bj: zip-up as p2 ends and player returns to proposition pose
                    if (currentAction == "sex" || currentAction == "blowjob") PlayBjCustom(_bjPantsZipFile, true);
                    PlayFinishSounds();
                    FinishCarServiceAndReopen();
                }
            }
        }

        /// <summary>Street sitting action phase system (mirrors RunCarAnimation for prop→action→prop transitions).</summary>
        private void RunStreetSitAnimation(Ped player)
        {
            bool isSinkraLayingCow = (streetActionIdx == 8 && _sinkraCowgirlAvailable && sinkraReplaceLaying);
            bool isSinkraLayingRevCow = (streetActionIdx == 9 && _sinkraCowgirlAvailable && sinkraReplaceRevLaying);

            // Sitting Cowgirl / Sitting BJ pull-out: TASK_PLAY_ANIM speed is silently ignored on the same tick it's issued,
            // so enforce -1f every frame while the reversed anims are playing.
            // animPhase 3 = P2 reversed is playing; animPhase 4 = P1 reversed is playing.
            if (_sitCowgirlPulloutReverse && (streetActionIdx == 6 || streetActionIdx == 3 || streetActionIdx == 7 || streetActionIdx == 4 || streetActionIdx == 8 || streetActionIdx == 9) && (animPhase == 3 || animPhase == 4))
            {
                int revIdx = (animPhase == 3) ? 1 : 0;
                string revMD, revMA, revFD, revFA;
                Vector3 revOff; float revHd; bool revSit; float revZO, revRX, revRH;
                GetStreetPropIntroExitAnimData(revIdx, streetActionIdx, out revMD, out revMA, out revFD, out revFA,
                    out revOff, out revHd, out revSit, out revZO, out revRX, out revRH);
                // Laying BJ / Sinkra Laying Cowgirl / Sinkra Laying Rev Cowgirl: male stays in loop — only enforce speed on female
                bool isFemaleOnlyPullout = streetActionIdx == 4
                    || (streetActionIdx == 8 && _sinkraCowgirlAvailable && sinkraReplaceLaying)
                    || (streetActionIdx == 9 && _sinkraCowgirlAvailable && sinkraReplaceRevLaying);
                if (!isFemaleOnlyPullout)
                    Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, revMD, revMA, -1.0f);
                if (girl != null && girl.Exists())
                    Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, revFD, revFA, -1.0f);
            }

            // Reverse Cowgirl intro: delayed fade to black mid-P1 for girl reposition
            if (streetActionIdx == 7 && (animPhase == 0 || animPhase == 1) && _streetOutroFadeAt > 0 && Game.GameTime >= _streetOutroFadeAt)
            {
                Function.Call(Hash.DO_SCREEN_FADE_OUT, 500);
                _streetOutroFadeAt = 0;
            }

            // Reverse Cowgirl / Laying Cowgirl / Laying Rev Cowgirl outro: delayed fade to black during outro
            if ((streetActionIdx == 7 || streetActionIdx == 8 || streetActionIdx == 9) && animPhase == 3 && _streetOutroFadeAt > 0 && Game.GameTime >= _streetOutroFadeAt)
            {
                Function.Call(Hash.DO_SCREEN_FADE_OUT, 500);
                _streetOutroFadeAt = 0;
            }

            if (animPhase == -1) // PRE-INTRO: screen faded out → reposition, play P1, fade back in
            {
                if (animWaitTime <= Game.GameTime)
                {
                    string iMDict, iMAnim, iFDict, iFAnim;
                    Vector3 iOffset; float iHeading; bool iIsSitting; float iZOff, iRotX, iRotH;
                    GetStreetPropIntroExitAnimData(0, streetActionIdx, out iMDict, out iMAnim, out iFDict, out iFAnim,
                        out iOffset, out iHeading, out iIsSitting, out iZOff, out iRotX, out iRotH);
                    if (!RequestAnimDict(iMDict)) return;
                    if (iMDict != iFDict && !RequestAnimDict(iFDict)) return;

                    float mStartPhase = 0f;
                    if (streetActionIdx == 1 || streetActionIdx == 4 || isSinkraLayingCow || isSinkraLayingRevCow)
                    {
                        float totalGetin = Function.Call<float>(Hash.GET_ANIM_DURATION, iMDict, iMAnim);
                        if (totalGetin > 0f) mStartPhase = 2.5f / totalGetin;
                    }

                    // Z repositioning
                    Vector3 pp = player.Position;
                    float actionZ = _streetSitOriginPos.Z + iZOff;
                    float deltaZ = actionZ - pp.Z;
                    if (Math.Abs(deltaZ) > 0.01f)
                    {
                        Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, player,
                            pp.X, pp.Y, actionZ, false, false, false);
                        // Sinkra Laying Cowgirl/Rev: keep girl at proposition Z — transition anims have
                        // root motion baked for -0.855 Z sink. Moving her to -0.59 causes floating.
                        if (girl != null && girl.Exists() && !isSinkraLayingCow && !isSinkraLayingRevCow)
                        {
                            Vector3 gp = girl.Position;
                            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                                gp.X, gp.Y, gp.Z + deltaZ, false, false, false);
                        }
                        pp = player.Position;
                    }

                    // Girl repositioning
                    if (streetActionIdx == 1 && girl != null && girl.Exists())
                    {
                        Vector3 girlActionPos = player.GetOffsetPosition(new Vector3(0.73f, -0.77f, -0.3f));
                        Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                            girlActionPos.X, girlActionPos.Y, girlActionPos.Z, false, false, false);
                        girl.Heading = player.Heading;
                    }
                    if (streetActionIdx == 4 && girl != null && girl.Exists())
                    {
                        Vector3 girlActionPos = player.GetOffsetPosition(new Vector3(0.17f, 0.8f, -0.37f));
                        Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                            girlActionPos.X, girlActionPos.Y, girlActionPos.Z, false, false, false);
                        girl.Heading = player.Heading + 90f;
                    }
                    if (streetActionIdx == 7 && girl != null && girl.Exists())
                    {
                        Vector3 girlRevPos = player.GetOffsetPosition(new Vector3(-0.83f, 0.4f, 0.02f));
                        Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                            girlRevPos.X, girlRevPos.Y, girlRevPos.Z, false, false, false);
                        girl.Heading = player.Heading + 180f;
                    }
                    if (streetActionIdx == 8 && !isSinkraLayingCow && girl != null && girl.Exists())
                    {
                        Vector3 girlActionPos = player.GetOffsetPosition(new Vector3(-0.65f, 0.0f, -0.02f));
                        Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                            girlActionPos.X, girlActionPos.Y, girlActionPos.Z, false, false, false);
                        girl.Heading = player.Heading;
                    }
                    if (streetActionIdx == 9 && !isSinkraLayingRevCow && girl != null && girl.Exists())
                    {
                        Vector3 girlActionPos = player.GetOffsetPosition(new Vector3(-0.35f, 0.15f, -0.02f));
                        Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                            girlActionPos.X, girlActionPos.Y, girlActionPos.Z, false, false, false);
                        girl.Heading = player.Heading + 180f;
                    }
                    // Sinkra Laying Rev Cowgirl: girl at reverse cowgirl offset, forced to proposition Z
                    // Play female P2 simultaneously with male t_getin_veniceb
                    if (isSinkraLayingRevCow && girl != null && girl.Exists())
                    {
                        Vector3 girlRevPos = player.GetOffsetPosition(new Vector3(-0.83f, 0.0f, 0.02f));
                        float propZ = _streetSitOriginPos.Z + (-0.855f);
                        Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                            girlRevPos.X, girlRevPos.Y, propZ, false, false, false);
                        girl.Heading = player.Heading + 180f;
                    }

                    // Play P1 transition anims
                    Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, player, iMDict, iMAnim,
                        pp.X, pp.Y, pp.Z, iRotX, 0f, player.Heading,
                        8.0f, -4.0f, -1, 2, mStartPhase, 2, 0);
                    // Sinkra Laying Rev Cowgirl: play female P2 simultaneously with male laying down
                    if (isSinkraLayingRevCow)
                    {
                        string p2MD, p2MA, p2FD, p2FA;
                        Vector3 p2Off; float p2Hd; bool p2Sit; float p2ZO, p2RX, p2RH;
                        GetStreetPropIntroExitAnimData(1, streetActionIdx, out p2MD, out p2MA, out p2FD, out p2FA,
                            out p2Off, out p2Hd, out p2Sit, out p2ZO, out p2RX, out p2RH);
                        if (RequestAnimDict(p2FD))
                            Function.Call(Hash.TASK_PLAY_ANIM, girl, p2FD, p2FA,
                                8.0f, -4.0f, -1, 2, 0f, false, false, false);
                    }
                    else
                        Function.Call(Hash.TASK_PLAY_ANIM, girl, iFDict, iFAnim,
                            8.0f, -4.0f, -1, 2, 0f, false, false, false);

                    // Fade back in
                    Function.Call(Hash.DO_SCREEN_FADE_IN, 500);

                    // Set P1 timer
                    float p1Dur = Function.Call<float>(Hash.GET_ANIM_DURATION, iMDict, iMAnim);
                    if (mStartPhase > 0f) p1Dur *= (1f - mStartPhase);
                    if (streetActionIdx == 1 || streetActionIdx == 4 || isSinkraLayingCow)
                    {
                        float fP1Dur = Function.Call<float>(Hash.GET_ANIM_DURATION, iFDict, iFAnim);
                        p1Dur = fP1Dur;
                    }
                    else if (isSinkraLayingRevCow)
                    {
                        // Sinkra Laying Rev Cowgirl: timer = female P2 duration (played simultaneously with male)
                        string p2MD, p2MA, p2FD, p2FA;
                        Vector3 p2Off; float p2Hd; bool p2Sit; float p2ZO, p2RX, p2RH;
                        GetStreetPropIntroExitAnimData(1, streetActionIdx, out p2MD, out p2MA, out p2FD, out p2FA,
                            out p2Off, out p2Hd, out p2Sit, out p2ZO, out p2RX, out p2RH);
                        float fP2Dur = Function.Call<float>(Hash.GET_ANIM_DURATION, p2FD, p2FA);
                        p1Dur = fP2Dur;
                    }
                    else if (iMDict != iFDict) { float fP1Dur = Function.Call<float>(Hash.GET_ANIM_DURATION, iFDict, iFAnim); p1Dur = Math.Max(p1Dur, fP1Dur); }
                    animWaitTime = Game.GameTime + (int)(p1Dur * 1000f);
                    animPhase = 0;
                }
            }
            else if (animPhase == 0) // INTRO: prop→action P1 done → play P2
            {
                if (animWaitTime <= Game.GameTime)
                {
                    // Vanilla Laying Rev Cowgirl: only one intro anim — skip P2, go straight to loop
                    // Sinkra Laying Rev Cowgirl: P2 already played in phase -1 simultaneously with male
                    if (streetActionIdx == 9)
                    {
                        animPhase = 1;
                        return;
                    }

                    string mD, mA, fD, fA;
                    Vector3 off; float hd; bool sit; float zO, rX, rH;
                    GetStreetPropIntroExitAnimData(1, streetActionIdx, out mD, out mA, out fD, out fA,
                        out off, out hd, out sit, out zO, out rX, out rH);
                    if (!RequestAnimDict(mD)) return;
                    if (mD != fD && !RequestAnimDict(fD)) return;

                    Vector3 pp = player.Position;
                    if (streetActionIdx == 1 || streetActionIdx == 4 || isSinkraLayingCow || isSinkraLayingRevCow)
                    {
                        // Facesitting/Laying BJ/Sinkra Laying Cowgirl/Rev: don't interrupt male t_getin_veniceb, only play female P2
                        // Sinkra Laying Rev Cowgirl: reposition girl to reverse offset at proposition Z for P2
                        if (isSinkraLayingRevCow && girl != null && girl.Exists())
                        {
                            Vector3 girlRevPos = player.GetOffsetPosition(new Vector3(-0.83f, 0.4f, 0.02f));
                            float propZ = _streetSitOriginPos.Z + (-0.855f);
                            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                                girlRevPos.X, girlRevPos.Y, propZ, false, false, false);
                            girl.Heading = player.Heading + 180f;
                        }
                        Function.Call(Hash.TASK_PLAY_ANIM, girl, fD, fA,
                            8.0f, -4.0f, -1, 2, 0f, false, false, false);
                        float fDur = Function.Call<float>(Hash.GET_ANIM_DURATION, fD, fA);
                        animWaitTime = Game.GameTime + (int)(fDur * 1000f);
                    }
                    else
                    {
                        // Reverse Cowgirl: reposition girl to reverse offset + fade in before P2
                        if (streetActionIdx == 7 && girl != null && girl.Exists())
                        {
                            Vector3 girlRevPos = player.GetOffsetPosition(new Vector3(-0.83f, 0.4f, 0.02f));
                            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                                girlRevPos.X, girlRevPos.Y, girlRevPos.Z, false, false, false);
                            girl.Heading = player.Heading + 180f;
                            if (Function.Call<bool>(Hash.IS_SCREEN_FADED_OUT) || Function.Call<bool>(Hash.IS_SCREEN_FADING_OUT))
                                Function.Call(Hash.DO_SCREEN_FADE_IN, 500);
                        }

                        Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, player, mD, mA,
                            pp.X, pp.Y, pp.Z, rX, 0f, player.Heading,
                            8.0f, -4.0f, -1, 2, 0f, 2, 0);
                        Function.Call(Hash.TASK_PLAY_ANIM, girl, fD, fA,
                            8.0f, -4.0f, -1, 2, 0f, false, false, false);
                        float dur = Function.Call<float>(Hash.GET_ANIM_DURATION, mD, mA);
                        if (mD != fD) { float fDur = Function.Call<float>(Hash.GET_ANIM_DURATION, fD, fA); dur = Math.Max(dur, fDur); }
                        animWaitTime = Game.GameTime + (int)(dur * 1000f);
                    }
                    animPhase = 1;
                }
            }
            else if (animPhase == 1) // INTRO: prop→action P2 done → play action loop
            {
                if (animWaitTime <= Game.GameTime)
                {
                    // currentMaleDict/Anim already set to the sex loop by StartStreetAction
                    string sMD, sMA, sFD, sFA;
                    Vector3 sOff; float sHd; bool sSit; float sZO, sRX;
                    GetStreetAnimData(streetActionIdx, out sMD, out sMA, out sFD, out sFA,
                        out sOff, out sHd, out sSit, out sZO, out sRX);

                    // Sinkra Laying Rev Cowgirl: transition player from proposition Z (-0.855) to action Z (-0.59) for the loop
                    if (isSinkraLayingRevCow)
                    {
                        Vector3 pp2 = player.Position;
                        float actionZ = _streetSitOriginPos.Z + sZO;
                        if (Math.Abs(actionZ - pp2.Z) > 0.01f)
                        {
                            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, player,
                                pp2.X, pp2.Y, actionZ, false, false, false);
                        }
                    }

                    // Facesitting / Reverse Cowgirl / Laying BJ / Laying Cowgirl / Laying Rev Cowgirl: ensure girl is at action offset for the loop
                    if ((streetActionIdx == 1 || streetActionIdx == 7 || streetActionIdx == 4 || streetActionIdx == 8 || streetActionIdx == 9) && girl != null && girl.Exists())
                    {
                        Vector3 girlActionPos = player.GetOffsetPosition(sOff);
                        Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                            girlActionPos.X, girlActionPos.Y, girlActionPos.Z, false, false, false);
                        girl.Heading = player.Heading + sHd;
                    }

                    Vector3 pp = player.Position;
                    Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, player, currentMaleDict, currentMaleAnim,
                        pp.X, pp.Y, pp.Z, sRX, 0f, player.Heading,
                        8.0f, -4.0f, -1, 9, 0f, 2, 0);
                    Function.Call(Hash.TASK_PLAY_ANIM, girl, currentFemaleDict, currentFemaleAnim,
                        8.0f, -4.0f, -1, 9, 0f, false, false, false);

                    // Sinkra cowgirl_clip runs at half speed — apply 2x scale so it looks normal
                    _femaleAnimSpeedScale = (currentFemaleAnim == "cowgirl_clip") ? 2.0f : 1.0f;
                    if (_femaleAnimSpeedScale != 1.0f)
                        Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, currentFemaleDict, currentFemaleAnim, animSpeed * _femaleAnimSpeedScale);

                    animPhase = 2;
                }
            }
            else if (animPhase == 3) // OUTRO: action→prop P1 done → play P2
            {
                if (animWaitTime <= Game.GameTime)
                {
                    // Sitting Cowgirl / Sitting BJ pull-out: P2 reversed done → play P1 in reverse → then finish
                    if (_sitCowgirlPulloutReverse && (streetActionIdx == 6 || streetActionIdx == 3))
                    {
                        string p1MD, p1MA, p1FD, p1FA;
                        Vector3 p1Off; float p1Hd; bool p1Sit; float p1ZO, p1RX, p1RH;
                        GetStreetPropIntroExitAnimData(0, streetActionIdx, out p1MD, out p1MA, out p1FD, out p1FA,
                            out p1Off, out p1Hd, out p1Sit, out p1ZO, out p1RX, out p1RH);
                        if (RequestAnimDict(p1MD) && (p1MD == p1FD || RequestAnimDict(p1FD)))
                        {
                            Vector3 pp = player.Position;
                            Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, player, p1MD, p1MA,
                                pp.X, pp.Y, pp.Z, p1RX, 0f, player.Heading,
                                8.0f, -4.0f, -1, 2, 1.0f, 2, 0);
                            Function.Call(Hash.TASK_PLAY_ANIM, girl, p1FD, p1FA,
                                8.0f, -4.0f, -1, 2, 1.0f, false, false, false);
                            // Per-tick enforcement in RunStreetSitAnimation keeps speed at -1f every frame
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, p1MD, p1MA, -1.0f);
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, p1FD, p1FA, -1.0f);
                        }
                        float p1Dur = Function.Call<float>(Hash.GET_ANIM_DURATION, p1MD, p1MA);
                        animWaitTime = Game.GameTime + (int)(p1Dur * 1000f);
                        animPhase = 4;
                        return;
                    }
                    // Reverse Cowgirl pull-out: P2rev done, screen faded → reposition under black, play P1 reversed, fade in
                    if (_sitCowgirlPulloutReverse && streetActionIdx == 7)
                    {
                        if (!Function.Call<bool>(Hash.IS_SCREEN_FADED_OUT))
                        {
                            animWaitTime = Game.GameTime + 100;
                            return;
                        }
                        // Fetch P1 data (proposition_to_sex_p1) for reversed playback
                        string p1MD, p1MA, p1FD, p1FA;
                        Vector3 p1Off; float p1Hd; bool p1Sit; float p1ZO, p1RX, p1RH;
                        GetStreetPropIntroExitAnimData(0, streetActionIdx, out p1MD, out p1MA, out p1FD, out p1FA,
                            out p1Off, out p1Hd, out p1Sit, out p1ZO, out p1RX, out p1RH);
                        // Fetch P2-outro data for girl reposition offset (same as finish-inside P2)
                        string xMD, xMA, xFD, xFA;
                        Vector3 xOff; float xHd; bool xSit; float xZO, xRX, xRH;
                        GetStreetPropIntroExitAnimData(3, streetActionIdx, out xMD, out xMA, out xFD, out xFA,
                            out xOff, out xHd, out xSit, out xZO, out xRX, out xRH);
                        // Reposition player Z to proposition level
                        Vector3 pp = player.Position;
                        float propZ = _streetSitOriginPos.Z + xZO;
                        float deltaZ = propZ - pp.Z;
                        if (Math.Abs(deltaZ) > 0.01f)
                        {
                            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, player,
                                pp.X, pp.Y, propZ, false, false, false);
                            pp = player.Position;
                        }
                        // Reposition girl to proposition offset (same as finish-inside P2) and fade in
                        if (girl != null && girl.Exists())
                        {
                            Vector3 girlPropPos = player.GetOffsetPosition(xOff);
                            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                                girlPropPos.X, girlPropPos.Y, girlPropPos.Z, false, false, false);
                            girl.Heading = player.Heading + xHd;
                            Function.Call(Hash.DO_SCREEN_FADE_IN, 500);
                        }
                        if (RequestAnimDict(p1MD) && (p1MD == p1FD || RequestAnimDict(p1FD)))
                        {
                            Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, player, p1MD, p1MA,
                                pp.X, pp.Y, pp.Z, p1RX, 0f, player.Heading,
                                8.0f, -4.0f, -1, 2, 1.0f, 2, 0);
                            Function.Call(Hash.TASK_PLAY_ANIM, girl, p1FD, p1FA,
                                8.0f, -4.0f, -1, 2, 1.0f, false, false, false);
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, p1MD, p1MA, -1.0f);
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, p1FD, p1FA, -1.0f);
                        }
                        float p1RevDur = Function.Call<float>(Hash.GET_ANIM_DURATION, p1MD, p1MA);
                        animWaitTime = Game.GameTime + (int)(p1RevDur * 1000f);
                        animPhase = 4;
                        return;
                    }
                    // Laying BJ pull-out: P2rev done → reposition girl + play proposition_to_bj_p1 reversed (female only, male stays in loop)
                    if (_sitCowgirlPulloutReverse && streetActionIdx == 4)
                    {
                        string p1MD, p1MA, p1FD, p1FA;
                        Vector3 p1Off; float p1Hd; bool p1Sit; float p1ZO, p1RX, p1RH;
                        GetStreetPropIntroExitAnimData(0, streetActionIdx, out p1MD, out p1MA, out p1FD, out p1FA,
                            out p1Off, out p1Hd, out p1Sit, out p1ZO, out p1RX, out p1RH);
                        // Reposition girl to proposition offset (same as finish-inside phase 3)
                        if (girl != null && girl.Exists())
                        {
                            string rpMD, rpMA, rpFD, rpFA;
                            Vector3 rpOff; float rpHd; bool rpSit; float rpZO, rpRX, rpRH;
                            GetStreetPropAnimData(2, out rpMD, out rpMA, out rpFD, out rpFA,
                                out rpOff, out rpHd, out rpSit, out rpZO, out rpRX, out rpRH);
                            float propZ = _streetSitOriginPos.Z + rpZO;
                            Vector3 girlPropPos = player.GetOffsetPosition(rpOff);
                            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                                girlPropPos.X, girlPropPos.Y, propZ, false, false, false);
                            girl.Heading = player.Heading + rpHd;
                        }
                        // Female only — male stays in loop
                        if (RequestAnimDict(p1FD))
                        {
                            Function.Call(Hash.TASK_PLAY_ANIM, girl, p1FD, p1FA,
                                8.0f, -4.0f, -1, 2, 1.0f, false, false, false);
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, p1FD, p1FA, -1.0f);
                        }
                        float p1Dur4 = Function.Call<float>(Hash.GET_ANIM_DURATION, p1FD, p1FA);
                        animWaitTime = Game.GameTime + (int)(p1Dur4 * 1000f);
                        animPhase = 4;
                        return;
                    }
                    // Sinkra Laying Cowgirl / Sinkra Laying Rev Cowgirl pull-out: P2rev done → play P1 reversed (female only)
                    if (_sitCowgirlPulloutReverse && ((streetActionIdx == 8 && _sinkraCowgirlAvailable && sinkraReplaceLaying) || (streetActionIdx == 9 && _sinkraCowgirlAvailable && sinkraReplaceRevLaying)))
                    {
                        string p1MD, p1MA, p1FD, p1FA;
                        Vector3 p1Off; float p1Hd; bool p1Sit; float p1ZO, p1RX, p1RH;
                        GetStreetPropIntroExitAnimData(0, streetActionIdx, out p1MD, out p1MA, out p1FD, out p1FA,
                            out p1Off, out p1Hd, out p1Sit, out p1ZO, out p1RX, out p1RH);
                        // Female only — male stays in loop
                        if (RequestAnimDict(p1FD))
                        {
                            Function.Call(Hash.TASK_PLAY_ANIM, girl, p1FD, p1FA,
                                8.0f, -4.0f, -1, 2, 1.0f, false, false, false);
                            Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, p1FD, p1FA, -1.0f);
                        }
                        float p1Dur89s = Function.Call<float>(Hash.GET_ANIM_DURATION, p1FD, p1FA);
                        animWaitTime = Game.GameTime + (int)(p1Dur89s * 1000f);
                        animPhase = 4;
                        return;
                    }
                    // Vanilla Laying Cowgirl / Laying Rev Cowgirl: single outro anim — skip P2, finish behind blackscreen
                    if ((streetActionIdx == 8 && !isSinkraLayingCow) || (streetActionIdx == 9 && !isSinkraLayingRevCow))
                    {
                        if (!Function.Call<bool>(Hash.IS_SCREEN_FADED_OUT))
                        {
                            animWaitTime = Game.GameTime + 100; // wait for fade to complete
                            return;
                        }
                        animPhase = 6;
                        return;
                    }

                    // Sinkra Laying Rev Cowgirl: skip female P2, go straight to male getout + blackscreen
                    if (isSinkraLayingRevCow)
                    {
                        animPhase = 4;
                        return;
                    }

                    string mD, mA, fD, fA;
                    Vector3 off; float hd; bool sit; float zO, rX, rH;
                    GetStreetPropIntroExitAnimData(3, streetActionIdx, out mD, out mA, out fD, out fA,
                        out off, out hd, out sit, out zO, out rX, out rH);
                    if (!RequestAnimDict(mD)) return;
                    if (mD != fD && !RequestAnimDict(fD)) return;

                    if (streetActionIdx == 1 || streetActionIdx == 4 || isSinkraLayingCow || isSinkraLayingRevCow)
                    {
                        // Facesitting/Laying BJ/Sinkra Laying Cowgirl/Rev: reposition girl back to proposition offset, play only female P2
                        if (girl != null && girl.Exists())
                        {
                            string rpMD, rpMA, rpFD, rpFA;
                            Vector3 rpOff; float rpHd; bool rpSit; float rpZO, rpRX, rpRH;
                            GetStreetPropAnimData(2, out rpMD, out rpMA, out rpFD, out rpFA,
                                out rpOff, out rpHd, out rpSit, out rpZO, out rpRX, out rpRH);
                            // Reposition girl Z back to proposition level
                            float propZ = _streetSitOriginPos.Z + rpZO;
                            Vector3 girlPropPos = player.GetOffsetPosition(rpOff);
                            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                                girlPropPos.X, girlPropPos.Y, propZ, false, false, false);
                            girl.Heading = player.Heading + rpHd;

                            Function.Call(Hash.TASK_PLAY_ANIM, girl, fD, fA,
                                8.0f, -4.0f, -1, 0, 0f, false, false, false);
                        }
                        // Male stays in loop — don't play male anim yet
                        float fDur = Function.Call<float>(Hash.GET_ANIM_DURATION, fD, fA);
                        animWaitTime = Game.GameTime + (int)(fDur * 1000f);
                        // Laying BJ: swallow sound during sequential outro
                        if (streetActionIdx == 4 && bjSoundsEnabled && currentAction == "blowjob")
                        {
                            bool skipSwallow = !_finishInside || _finishForced || _finishBroke;
                            if (!skipSwallow) PlayBjCustom(_bjSwallowFile, true);
                            else if (aLifeMode && (_finishForced || _finishBroke)) PlayBjSpit();
                        }
                        animPhase = 4;
                    }
                    else
                    {
                        // Non-facesitting: reposition both peds back to proposition Z
                        Vector3 pp = player.Position;
                        float propZ = _streetSitOriginPos.Z + zO;
                        float deltaZ = propZ - pp.Z;
                        if (Math.Abs(deltaZ) > 0.01f)
                        {
                            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, player,
                                pp.X, pp.Y, propZ, false, false, false);
                            if (girl != null && girl.Exists())
                            {
                                Vector3 gp = girl.Position;
                                Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                                    gp.X, gp.Y, gp.Z + deltaZ, false, false, false);
                            }
                            pp = player.Position;
                        }

                        // Reverse Cowgirl: reposition girl back to proposition offset + heading for P2 outro
                        if (streetActionIdx == 7 && girl != null && girl.Exists())
                        {
                            Vector3 girlPropPos = player.GetOffsetPosition(off);
                            Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                                girlPropPos.X, girlPropPos.Y, girlPropPos.Z, false, false, false);
                            girl.Heading = player.Heading + hd;
                            // Fade back in after reposition
                            Function.Call(Hash.DO_SCREEN_FADE_IN, 500);
                        }

                        Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, player, mD, mA,
                            pp.X, pp.Y, pp.Z, rX, 0f, player.Heading,
                            8.0f, -4.0f, -1, 0, 0f, 2, 0);
                        Function.Call(Hash.TASK_PLAY_ANIM, girl, fD, fA,
                            8.0f, -4.0f, -1, 0, 0f, false, false, false);

                        float dur = Function.Call<float>(Hash.GET_ANIM_DURATION, mD, mA);
                        if (mD != fD) { float fDur2 = Function.Call<float>(Hash.GET_ANIM_DURATION, fD, fA); dur = Math.Max(dur, fDur2); }
                        animWaitTime = Game.GameTime + (int)(dur * 1000f);
                        // Street sitting BJ outro p2: swallow sound as she finishes (matches car timing)
                        if (bjSoundsEnabled && currentAction == "blowjob")
                        {
                            bool skipSwallow = !_finishInside || _finishForced || _finishBroke;
                            if (!skipSwallow) PlayBjCustom(_bjSwallowFile, true);
                            else if (aLifeMode && (_finishForced || _finishBroke)) PlayBjSpit();
                        }
                        animPhase = 4;
                    }
                }
            }
            else if (animPhase == 4) // OUTRO: action→prop P2 done → back to proposition
            {
                if (animWaitTime <= Game.GameTime)
                {
                    // Laying BJ / Sinkra Laying Cowgirl / Sinkra Laying Rev Cowgirl pull-out: P1rev done → reset flag and fall through
                    // (plays t_getout_veniceb forward + girl proposition loop + fades to black → phase 5→6)
                    if (_sitCowgirlPulloutReverse && (streetActionIdx == 4
                        || (streetActionIdx == 8 && _sinkraCowgirlAvailable && sinkraReplaceLaying)
                        || (streetActionIdx == 9 && _sinkraCowgirlAvailable && sinkraReplaceRevLaying)))
                        _sitCowgirlPulloutReverse = false;

                    if (streetActionIdx == 1 || streetActionIdx == 4 || isSinkraLayingCow || isSinkraLayingRevCow)
                    {
                        // Facesitting/Laying BJ/Sinkra Laying Cowgirl/Rev: female is back at prop, now play male getout at action Z
                        // Male is still at action Z from the loop — DON'T reposition yet
                        Vector3 pp = player.Position;

                        // Play male t_getout_veniceb at current (action) position
                        string gMD, gMA, gFD, gFA;
                        Vector3 gOff; float gHd; bool gSit; float gZO, gRX, gRH;
                        GetStreetPropIntroExitAnimData(2, streetActionIdx, out gMD, out gMA, out gFD, out gFA,
                            out gOff, out gHd, out gSit, out gZO, out gRX, out gRH);
                        if (RequestAnimDict(gMD))
                        {
                            Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, player, gMD, gMA,
                                pp.X, pp.Y, pp.Z, gRX, 0f, player.Heading,
                                8.0f, -4.0f, -1, 2, 0f, 2, 0);
                        }

                        // Play female proposition loop while male gets up
                        // Sinkra Laying Rev Cowgirl: let her outro anim keep playing naturally (screen is already black)
                        if (!isSinkraLayingRevCow)
                        {
                            string plMD, plMA, plFD, plFA;
                            Vector3 plOff; float plHd; bool plSit; float plZO, plRX, plRH;
                            GetStreetPropAnimData(2, out plMD, out plMA, out plFD, out plFA,
                                out plOff, out plHd, out plSit, out plZO, out plRX, out plRH);
                            if (RequestAnimDict(plFD))
                            {
                                Function.Call(Hash.TASK_PLAY_ANIM, girl, plFD, plFA,
                                    8.0f, -4.0f, -1, 1, 0f, false, false, false);
                            }
                        }

                        animWaitTime = Game.GameTime + 1000; // t_getout_veniceb ~1 second
                        Function.Call(Hash.DO_SCREEN_FADE_OUT, 500); // fade to black in 500ms
                        animPhase = 5;
                    }
                    else
                    {
                        PlayBjCustom(_bjPantsZipFile, true);
                        PlayFinishSounds();
                        _sitCowgirlPulloutReverse = false;
                        FinishStreetSitServiceAndReopen();
                    }
                }
            }
            else if (animPhase == 5) // FACESITTING/LAYING BJ: male getout done (screen already black) → finish
            {
                if (animWaitTime <= Game.GameTime)
                {
                    animPhase = 6;
                }
            }
            else if (animPhase == 6) // FACESITTING/LAYING BJ/LAYING COWGIRL: faded out → reposition, finish, fade back in
            {
                if (animWaitTime <= Game.GameTime)
                {
                    // Reposition player Z back to proposition level
                    Vector3 pp = player.Position;
                    float propZ = _streetSitOriginPos.Z + (-0.855f);
                    if (Math.Abs(propZ - pp.Z) > 0.01f)
                    {
                        Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, player,
                            pp.X, pp.Y, propZ, false, false, false);
                    }

                    // Reposition girl back to proposition offset behind blackscreen
                    if ((streetActionIdx == 8 || streetActionIdx == 9) && girl != null && girl.Exists())
                    {
                        string rpMD, rpMA, rpFD, rpFA;
                        Vector3 rpOff; float rpHd; bool rpSit; float rpZO, rpRX, rpRH;
                        GetStreetPropAnimData(2, out rpMD, out rpMA, out rpFD, out rpFA,
                            out rpOff, out rpHd, out rpSit, out rpZO, out rpRX, out rpRH);
                        Vector3 girlPropPos = player.GetOffsetPosition(rpOff);
                        Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, girl,
                            girlPropPos.X, girlPropPos.Y, propZ + (rpZO - (-0.855f)), false, false, false);
                        girl.Heading = player.Heading + rpHd;
                    }

                    if (streetActionIdx == 4 || streetActionIdx == 8 || streetActionIdx == 9) PlayBjCustom(_bjPantsZipFile, true); // Laying BJ / Laying Cowgirl / Rev: zip behind blackscreen
                    PlayFinishSounds();
                    FinishStreetSitServiceAndReopen();
                    Function.Call(Hash.DO_SCREEN_FADE_IN, 500);
                }
            }
        }

        // ════════════════════
        // CAR PROPOSITION POSE
        // ════════════════════

        /// <summary>Start the into_proposition anim when opening Services menu in a car.</summary>
        private void EnterCarProposition(Ped player)
        {
            if (!player.IsInVehicle()) return;
            Vehicle car = player.CurrentVehicle;
            if (car == null) return;

            // Determine vehicle animation type
            int modelHash = car.Model.Hash;
            if (overwriteLowVehicle.Contains(modelHash))
                vehicleAnimType = VehicleAnimType.Low;
            else if (overwriteNormalVehicle.Contains(modelHash))
                vehicleAnimType = VehicleAnimType.Normal;
            else
            {
                int classType = Function.Call<int>(Hash.GET_VEHICLE_CLASS, car);
                if (classType == 3 || classType == 5 || classType == 6 || classType == 7)
                    vehicleAnimType = VehicleAnimType.Low;
                else
                    vehicleAnimType = VehicleAnimType.Normal;
            }

            // Get dict (check first person)
            int camMode = Function.Call<int>(Hash.GET_FOLLOW_PED_CAM_VIEW_MODE);
            bool isFirstPerson = (camMode == 4);
            string dict;
            if (vehicleAnimType == VehicleAnimType.Low)
                dict = isFirstPerson ? VEH_LOW_FP_DICT : VEH_LOW_DICT;
            else
                dict = isFirstPerson ? VEH_NORM_FP_DICT : VEH_NORM_DICT;

            // ── Backseat: use drunk_driver_2 flirt anims ──
            bool inBack = IsPlayerInBackSeat(car);
            if (inBack)
            {
                const string DRUNK_DICT = "random@drunk_driver_2";
                if (!RequestAnimDict(DRUNK_DICT)) return;

                Function.Call(Hash.TASK_PLAY_ANIM, girl, DRUNK_DICT, "cardrunkflirt_intro_f",
                    8.0f, -4.0f, -1, 2, 0.0f, false, false, false);
                Function.Call(Hash.TASK_PLAY_ANIM, player, DRUNK_DICT, "cardrunkflirt_intro_m",
                    8.0f, -4.0f, -1, driveableCar ? 34 : 2, 0.0f, false, false, false);

                float backseatEnterDur = Function.Call<float>(Hash.GET_ANIM_DURATION, DRUNK_DICT, "cardrunkflirt_intro_m");
                carPropActive = true;
                carPropPhase = 0;
                carPropWaitTime = Game.GameTime + Math.Max((int)(backseatEnterDur * 1000f), 1500);
                if (interiorLightEnabled) car.IsInteriorLightOn = true;
                if (!carEngineEnabled) car.IsEngineRunning = false;
                return;
            }

            if (!RequestAnimDict(dict)) return;
            // Preload all dict variants
            RequestAnimDict((vehicleAnimType == VehicleAnimType.Low) ? VEH_LOW_FP_DICT : VEH_NORM_FP_DICT);
            RequestAnimDict((vehicleAnimType == VehicleAnimType.Low) ? VEH_LOW_DICT : VEH_NORM_DICT);

            // Get enter anim names
            string fEnter, mEnter;
            if (vehicleAnimType == VehicleAnimType.Low)
            { fEnter = "low_car_sit_to_prop_female"; mEnter = "low_car_sit_to_prop_player"; }
            else
            { fEnter = "into_proposition_prostitute"; mEnter = "into_proposition_male"; }

            // Play enter anims (flag 2 = hold last frame)
            Function.Call(Hash.TASK_PLAY_ANIM, girl, dict, fEnter,
                8.0f, -4.0f, -1, 2, 0.0f, false, false, false);
            Function.Call(Hash.TASK_PLAY_ANIM, player, dict, mEnter,
                8.0f, -4.0f, -1, driveableCar ? 34 : 2, 0.0f, false, false, false);

            float enterDur = Function.Call<float>(Hash.GET_ANIM_DURATION, dict, fEnter);
            carPropActive = true;
            carPropPhase = 0;
            carPropWaitTime = Game.GameTime + Math.Max((int)(enterDur * 1000f), 1500);
            if (interiorLightEnabled) car.IsInteriorLightOn = true;
            if (!carEngineEnabled) car.IsEngineRunning = false;
        }

        /// <summary>Play the exit animation from proposition pose back to sitting.</summary>
        private void ExitCarProposition(Ped player)
        {
            if (!carPropActive) return;

            // ── Backseat: reverse cardrunkflirt_intro ──
            bool inBackExit = player.IsInVehicle() && player.CurrentVehicle != null
                && IsPlayerInBackSeat(player.CurrentVehicle);
            if (inBackExit)
            {
                const string DRUNK_DICT = "random@drunk_driver_2";
                RequestAnimDict(DRUNK_DICT);

                // Stop current loop anims so we can restart the intro for reverse playback
                Function.Call(Hash.STOP_ANIM_TASK, player, DRUNK_DICT, "cardrunkflirt_loop_m", -4.0f);
                Function.Call(Hash.STOP_ANIM_TASK, girl,   DRUNK_DICT, "cardrunkflirt_loop_f", -4.0f);

                // Play intro from the end (startPhase ≈ 0.99) then set speed to -1
                Function.Call(Hash.TASK_PLAY_ANIM, player, DRUNK_DICT, "cardrunkflirt_intro_m",
                    8.0f, -4.0f, -1, driveableCar ? 32 : 0, 0.99f, false, false, false);
                Function.Call(Hash.SET_ENTITY_ANIM_SPEED, player, DRUNK_DICT, "cardrunkflirt_intro_m", -1.0f);

                Function.Call(Hash.TASK_PLAY_ANIM, girl, DRUNK_DICT, "cardrunkflirt_intro_f",
                    8.0f, -4.0f, -1, 0, 0.99f, false, false, false);
                Function.Call(Hash.SET_ENTITY_ANIM_SPEED, girl, DRUNK_DICT, "cardrunkflirt_intro_f", -1.0f);

                float backseatExitDur = Function.Call<float>(Hash.GET_ANIM_DURATION, DRUNK_DICT, "cardrunkflirt_intro_m");
                carPropPhase = 2;
                carPropWaitTime = Game.GameTime + (int)(backseatExitDur * 1000f);
                return;
            }

            int camMode = Function.Call<int>(Hash.GET_FOLLOW_PED_CAM_VIEW_MODE);
            bool isFirstPerson = (camMode == 4);
            string dict;
            if (vehicleAnimType == VehicleAnimType.Low)
                dict = isFirstPerson ? VEH_LOW_FP_DICT : VEH_LOW_DICT;
            else
                dict = isFirstPerson ? VEH_NORM_FP_DICT : VEH_NORM_DICT;

            string fExit, mExit;
            if (vehicleAnimType == VehicleAnimType.Low)
            { fExit = "low_car_prop_to_sit_female"; mExit = "low_car_prop_to_sit_player"; }
            else
            { fExit = "prop_to_sit_prostitute"; mExit = "prop_to_sit_male"; }

            Function.Call(Hash.TASK_PLAY_ANIM, girl, dict, fExit,
                8.0f, -4.0f, -1, 0, 0.0f, false, false, false);
            Function.Call(Hash.TASK_PLAY_ANIM, player, dict, mExit,
                8.0f, -4.0f, -1, driveableCar ? 32 : 0, 0.0f, false, false, false);

            float exitDur = Function.Call<float>(Hash.GET_ANIM_DURATION, dict, fExit);
            carPropPhase = 2;
            carPropWaitTime = Game.GameTime + (int)(exitDur * 1000f);
        }

        /// <summary>After a car service outro finishes, go back to proposition loop and reopen services.</summary>
        private void FinishCarServiceAndReopen()
        {
            Ped player = Game.Player.Character;

            // Partial cleanup (don't clear anims — we're transitioning back to proposition loop)
            DetachDildoProp();

            // Finish heal boost
            if (healPlayer)
                player.Health = Math.Min(player.Health + FINISH_HEAL, player.MaxHealth);
            if (healGirl && girl != null && girl.Exists())
                girl.Health = Math.Min(girl.Health + FINISH_HEAL, girl.MaxHealth);

            // Re-enable controls that were disabled during animation
            Function.Call(Hash.SET_PLAYER_CAN_DO_DRIVE_BY, Game.Player, true);

            // Reset animation state
            animPhase = 0;
            animSpeed = 1.0f;
            _femaleAnimSpeedScale = 1.0f;
            speedCooldown = 0;
            _deviceExcluded = false;
            _kissingReverse = false;
            _standingBjReverse = false;
            _rdoggyReverse = false;
            _sitCowgirlPulloutReverse = false;
            bool wasCarPullout = _carPulloutReverse; // capture before reset
            _carPulloutReverse = false;
            _standingBjSwallowAt = 0;
            _backseatCarSex = false;
            _throatSpeedLocked   = false;
            _throatLockAt        = 0;
            _throatSequenceEndAt = 0;
            _finishExecutePending = false;
            _finishExecuteAt      = 0;
            _finishInside         = false;
            bool wasFinishForced  = _finishForced; // capture before reset
            _finishForced         = false;
            _finishBroke          = false;
            _finishAsked          = false;

            // Stop device movement while proposition loop / services menu is shown
            _telemetry.SendStop();

            // Switch back to idle and resolve reputation BEFORE starting the proposition loop.
            // If the rep change triggers a force-release (avoidant), the session is torn down here
            // and we never put the player into the looping proposition anim — avoiding a stuck last-frame.
            state = ScriptState.Idle;
            negotiateMode = false;
            keyCooldown = KEY_COOLDOWN_FRAMES;
            // Don't award +1 rep for a forced-inside finish — keeps rep at -2 so the release check fires.
            if (!wasFinishForced && aLifeMode && _currentGirlKey != null)
            {
                ALifePedData dSession;
                if (_aLifePeds.TryGetValue(_currentGirlKey, out dSession) && dSession != null && dSession.Reputation >= 0)
                {
                    dSession.Reputation = Math.Min(REP_MAX, dSession.Reputation + 1);
                    UpdateRelationshipState(dSession);
                    SaveALife();
                }
            }
            ShowHudStatus("~g~DONE!", 3000);

            // Forced finish + hostile rep: full release right here, before the proposition loop starts.
            if (wasFinishForced && aLifeMode && _currentGirlFp != null)
            {
                ALifePedData dForced;
                if (_aLifePeds.TryGetValue(_currentGirlKey, out dForced) && dForced != null && dForced.Reputation <= REP_MIN)
                {
                    _aLifeHookerServices = false;
                    carPropActive = false;
                    if (girl != null && girl.Exists())
                    {
                        Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, girl, false);
                        girl.Task.WanderAround();
                        Function.Call((Hash)0x726256CC1EEB182F, girl); // CLEAR_FACIAL_IDLE_ANIM_OVERRIDE
                        girl.IsPositionFrozen = false;
                        girl.BlockPermanentEvents = false;
                        girl.IsPersistent = false;
                        girl.MarkAsNoLongerNeeded();
                    }
                    girl            = null;
                    hasGirl         = false;
                    _currentGirlFp  = null;
                    _currentGirlKey = null;
                    mode            = Mode.None;
                    menuLevel       = MenuLevel.None;
                    return;
                }
            }

            // If a rep change triggered a force-release (avoidant), session is already over — don't reopen menu
            if (!hasGirl || girl == null)
                return;

            // Start proposition loop (we're already in proposition pose from the outro)
            Ped plReopen = Game.Player.Character;
            bool inBackReopen = plReopen.IsInVehicle() && plReopen.CurrentVehicle != null
                && IsPlayerInBackSeat(plReopen.CurrentVehicle);
            if (inBackReopen)
            {
                const string DRUNK_DICT = "random@drunk_driver_2";
                RequestAnimDict(DRUNK_DICT);
                if (wasCarPullout)
                {
                    // Pull-out: skip flirt intro, go straight to proposition loop
                    Function.Call(Hash.TASK_PLAY_ANIM, girl, DRUNK_DICT, "cardrunkflirt_loop_f",
                        8.0f, -4.0f, -1, 1, 0.0f, false, false, false);
                    Function.Call(Hash.TASK_PLAY_ANIM, plReopen, DRUNK_DICT, "cardrunkflirt_loop_m",
                        8.0f, -4.0f, -1, driveableCar ? 33 : 1, 0.0f, false, false, false);
                    carPropWaitTime = 0;
                }
                else
                {
                    // Finish-inside: play flirt intro as normal
                    Function.Call(Hash.TASK_PLAY_ANIM, girl, DRUNK_DICT, "cardrunkflirt_intro_f",
                        8.0f, -4.0f, -1, 2, 0.0f, false, false, false);
                    Function.Call(Hash.TASK_PLAY_ANIM, plReopen, DRUNK_DICT, "cardrunkflirt_intro_m",
                        8.0f, -4.0f, -1, driveableCar ? 34 : 2, 0.0f, false, false, false);
                    float reopenIntroDur = Function.Call<float>(Hash.GET_ANIM_DURATION, DRUNK_DICT, "cardrunkflirt_intro_m");
                    carPropWaitTime = Game.GameTime + (int)(reopenIntroDur * 1000f);
                }
            }
            else
            {
                int camMode = Function.Call<int>(Hash.GET_FOLLOW_PED_CAM_VIEW_MODE);
                bool isFirstPerson = (camMode == 4);
                string dict;
                if (vehicleAnimType == VehicleAnimType.Low)
                    dict = isFirstPerson ? VEH_LOW_FP_DICT : VEH_LOW_DICT;
                else
                    dict = isFirstPerson ? VEH_NORM_FP_DICT : VEH_NORM_DICT;

                string fLoop, mLoop;
                if (vehicleAnimType == VehicleAnimType.Low)
                { fLoop = "low_car_prop_loop_female"; mLoop = "low_car_prop_loop_player"; }
                else
                { fLoop = "proposition_loop_prostitute"; mLoop = "proposition_loop_male"; }

                Function.Call(Hash.TASK_PLAY_ANIM, girl, dict, fLoop,
                    8.0f, -4.0f, -1, 1, 0.0f, false, false, false);
                Function.Call(Hash.TASK_PLAY_ANIM, plReopen, dict, mLoop,
                    8.0f, -4.0f, -1, driveableCar ? 33 : 1, 0.0f, false, false, false);
            }

            // Set proposition state
            carPropActive = true;
            carPropPhase = inBackReopen ? (wasCarPullout ? 1 : 0) : 1; // Backseat pull-out: already looping; finish-inside: intro playing; front: already looping

            // A-Life: route to the correct menu based on post-session state
            if (aLifeMode && sandboxMode && _currentGirlFp != null)
            {
                ALifePedData dFin;
                if (_aLifePeds.TryGetValue(_currentGirlKey, out dFin) && dFin != null)
                {
                    if (dFin.Reputation <= REP_MIN)
                    {
                        // Hostile (-2) — she leaves, close all menus
                        _aLifeHookerServices = false;
                        carPropActive = false;
                        if (girl != null && girl.Exists())
                        {
                            Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, girl, false);
                            girl.Task.WanderAround();
                        }
                        menuLevel = MenuLevel.None;
                        return;
                    }
                    if (!dFin.IsHooker)
                    {
                        // Casual A-Life: came from Escalate → Services, return there
                        _aLifeHookerServices = false;
                        menuIndex = 0;
                        if (_postSexPhase == 1) { _postSexPendingMenu = MenuLevel.Escalate; return; }
                        menuLevel = MenuLevel.Escalate;
                        return;
                    }
                }
            }
            // Arrangement still active (or non-A-Life hooker) — reopen Services as normal
            menuIndex = 0;
            if (_postSexPhase == 1) { _postSexPendingMenu = MenuLevel.Services; return; }
            menuLevel = MenuLevel.Services;
        }

        /// <summary>After standing service outro: full cleanup, resume girl follow, reopen Standing Services menu.</summary>
        private void FinishStandingServiceAndReopen()
        {
            Ped player = Game.Player.Character;

            // Stop rumble
            int burstDuration = 2000;
            if (_finishBurstFired && Game.GameTime - _lastRumbleTime <= burstDuration)
                Script.Wait(burstDuration - (Game.GameTime - _lastRumbleTime));
            SetVibration(0, 0);
            _rumblePulseActive = false;
            _finishBurstFired  = false;

            // Release player and girl (standing — no sit prop to restore)
            player.IsPositionFrozen = false;
            Function.Call(Hash.SET_ENTITY_COLLISION, player, true, true);
            player.Task.ClearAll();

            if (girl != null && girl.Exists())
            {
                girl.IsPositionFrozen = false;
                Function.Call(Hash.SET_ENTITY_COLLISION, girl, true, true);
                Function.Call(Hash.SET_ENTITY_NO_COLLISION_ENTITY, girl, player, true);
                Function.Call(Hash.SET_PED_KEEP_TASK, girl, false);
                girl.Task.ClearAll();
                Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, girl, true);
                // Resume following the player
                float dist = girl.Position.DistanceTo(player.Position);
                float speed = (dist > 7f) ? 3f : 1f;
                Function.Call(Hash.TASK_FOLLOW_TO_OFFSET_OF_ENTITY,
                    girl, player, 0f, -1f, 0f, speed, -1, 1.5f, true);
            }

            // Finish heal boost
            if (healPlayer)
                player.Health = Math.Min(player.Health + FINISH_HEAL, player.MaxHealth);
            if (healGirl && girl != null && girl.Exists())
                girl.Health = Math.Min(girl.Health + FINISH_HEAL, girl.MaxHealth);

            DetachDildoProp();

            // Re-enable everything
            Function.Call(Hash.SET_PLAYER_CAN_DO_DRIVE_BY, Game.Player, true);
            Function.Call(Hash.SET_EVERYONE_IGNORE_PLAYER, Game.Player, false);

            bool wasDeviceExcluded = _deviceExcluded;

            // Reset animation state
            state = ScriptState.Idle;
            animPhase = 0;
            animSpeed = 1.0f;
            _femaleAnimSpeedScale = 1.0f;
            speedCooldown = 0;
            _deviceExcluded = false;
            _kissingReverse = false;
            _standingBjSwallowAt = 0;
            healPlayer = false;
            healGirl = false;
            healRate = 1.0f;
            streetActionIdx = -1;
            currentAction = "";
            lastPaidAmount = 0;
            _throatSpeedLocked   = false;
            _throatLockAt        = 0;
            _throatSequenceEndAt = 0;
            _finishExecutePending = false;
            _finishExecuteAt      = 0;
            _finishInside         = false;
            bool wasFinishForced  = _finishForced;
            _finishForced         = false;
            _finishBroke          = false;
            _finishAsked          = false;

            if (!wasDeviceExcluded) _telemetry.SendFinish();
            _telemetry.SendStop();

            negotiateMode = false;
            keyCooldown = KEY_COOLDOWN_FRAMES * 3;

            // Award +1 rep (unless forced finish)
            if (!wasFinishForced && aLifeMode && _currentGirlKey != null)
            {
                ALifePedData dSession;
                if (_aLifePeds.TryGetValue(_currentGirlKey, out dSession) && dSession != null && dSession.Reputation >= 0)
                {
                    dSession.Reputation = Math.Min(REP_MAX, dSession.Reputation + 1);
                    UpdateRelationshipState(dSession);
                    SaveALife();
                }
            }
            ShowHudStatus("~g~DONE!", 3000);

            // A-Life: hostile rep → fully release her, don't reopen
            if (aLifeMode && girl != null && girl.Exists() && _currentGirlFp != null)
            {
                ALifePedData dWander;
                if (_aLifePeds.TryGetValue(_currentGirlKey, out dWander) && dWander.Reputation <= REP_MIN)
                {
                    _aLifeHookerServices = false;
                    Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, girl, false);
                    girl.Task.WanderAround();
                    Function.Call((Hash)0x726256CC1EEB182F, girl);
                    girl.IsPositionFrozen = false;
                    girl.BlockPermanentEvents = false;
                    girl.IsPersistent = false;
                    girl.MarkAsNoLongerNeeded();
                    girl  = null;
                    hasGirl = false;
                    _currentGirlFp  = null;
                    _currentGirlKey = null;
                    mode = Mode.None;
                    menuLevel = MenuLevel.None;
                    return;
                }
            }

            if (!hasGirl || girl == null)
                return;

            // A-Life sandbox non-hooker: return to Escalate (came from Escalate → Services)
            if (aLifeMode && sandboxMode && _currentGirlFp != null)
            {
                ALifePedData dFin;
                if (_aLifePeds.TryGetValue(_currentGirlKey, out dFin) && dFin != null)
                {
                    if (!dFin.IsHooker)
                    {
                        _aLifeHookerServices = false;
                        menuIndex = 0;
                        if (_postSexPhase == 1) { _postSexPendingMenu = MenuLevel.Escalate; return; }
                        menuLevel = MenuLevel.Escalate;
                        return;
                    }
                }
            }

            // Reopen Standing Services menu
            menuIndex = 0;
            if (_postSexPhase == 1) { _postSexPendingMenu = MenuLevel.StandingServices; return; }
            menuLevel = MenuLevel.StandingServices;
        }

        /// <summary>After street sitting sex outro: partial cleanup, return to proposition loop, reopen services menu.</summary>
        private void FinishStreetSitServiceAndReopen()
        {
            Ped player = Game.Player.Character;

            // Partial cleanup (don't clear collision/freeze — proposition still active)
            DetachDildoProp();

            // Finish heal boost
            if (healPlayer)
                player.Health = Math.Min(player.Health + FINISH_HEAL, player.MaxHealth);
            if (healGirl && girl != null && girl.Exists())
                girl.Health = Math.Min(girl.Health + FINISH_HEAL, girl.MaxHealth);

            // Reset animation state
            animPhase = 0;
            animSpeed = 1.0f;
            _femaleAnimSpeedScale = 1.0f;
            speedCooldown = 0;
            _deviceExcluded = false;
            _kissingReverse = false;
            _standingBjReverse = false;
            _rdoggyReverse = false;
            _sitCowgirlPulloutReverse = false;
            _standingBjSwallowAt = 0;
            healPlayer = false;
            healGirl = false;
            healRate = 1.0f;
            streetActionIdx = -1;
            currentAction = "";
            lastPaidAmount = 0;
            _throatSpeedLocked   = false;
            _throatLockAt        = 0;
            _throatSequenceEndAt = 0;
            _finishExecutePending = false;
            _finishExecuteAt      = 0;
            _finishInside         = false;
            bool wasFinishForced  = _finishForced;
            _finishForced         = false;
            _finishBroke          = false;
            _finishAsked          = false;

            _telemetry.SendStop();

            state = ScriptState.Idle;
            negotiateMode = false;
            keyCooldown = KEY_COOLDOWN_FRAMES;

            // Award +1 rep (unless forced finish)
            if (!wasFinishForced && aLifeMode && _currentGirlKey != null)
            {
                ALifePedData dSession;
                if (_aLifePeds.TryGetValue(_currentGirlKey, out dSession) && dSession != null && dSession.Reputation >= 0)
                {
                    dSession.Reputation = Math.Min(REP_MAX, dSession.Reputation + 1);
                    UpdateRelationshipState(dSession);
                    SaveALife();
                }
            }
            ShowHudStatus("~g~DONE!", 3000);

            // Forced finish + hostile rep: full release
            if (wasFinishForced && aLifeMode && _currentGirlFp != null)
            {
                ALifePedData dForced;
                if (_aLifePeds.TryGetValue(_currentGirlKey, out dForced) && dForced != null && dForced.Reputation <= REP_MIN)
                {
                    _aLifeHookerServices = false;
                    _streetSitPropActive = false;
                    _streetSitPropPhase = 0;
                    if (girl != null && girl.Exists())
                    {
                        girl.IsPositionFrozen = false;
                        Function.Call(Hash.SET_ENTITY_COLLISION, girl, true, true);
                        Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, girl, false);
                        girl.Task.WanderAround();
                        Function.Call((Hash)0x726256CC1EEB182F, girl);
                        girl.IsPersistent = false;
                        girl.MarkAsNoLongerNeeded();
                    }
                    player.IsPositionFrozen = false;
                    Function.Call(Hash.SET_ENTITY_COLLISION, player, true, true);
                    girl            = null;
                    hasGirl         = false;
                    _currentGirlFp  = null;
                    _currentGirlKey = null;
                    mode            = Mode.None;
                    menuLevel       = MenuLevel.None;
                    return;
                }
            }

            if (!hasGirl || girl == null)
                return;

            // Play proposition loop anims (already in position from outro)
            string mD, mA, fD, fA;
            Vector3 off; float hd; bool sit; float zO, rX, rH;
            GetStreetPropAnimData(2, out mD, out mA, out fD, out fA,
                out off, out hd, out sit, out zO, out rX, out rH);
            if (RequestAnimDict(mD))
            {
                Vector3 pp = player.Position;
                Function.Call(Hash.TASK_PLAY_ANIM_ADVANCED, player, mD, mA,
                    pp.X, pp.Y, pp.Z, rX, 0f, player.Heading,
                    8.0f, -4.0f, -1, 9, 0f, 2, 0);
                Function.Call(Hash.TASK_PLAY_ANIM, girl, fD, fA,
                    8.0f, -4.0f, -1, 9, 0f, false, false, false);
            }

            _streetSitPropPhase = 2;

            // Reopen the correct services menu
            if (aLifeMode && sandboxMode && _currentGirlFp != null)
            {
                ALifePedData dFin;
                if (_aLifePeds.TryGetValue(_currentGirlKey, out dFin) && dFin != null)
                {
                    if (dFin.Reputation <= REP_MIN)
                    {
                        _aLifeHookerServices = false;
                        _streetSitPropActive = false;
                        _streetSitPropPhase = 0;
                        menuLevel = MenuLevel.None;
                        return;
                    }
                    if (!dFin.IsHooker)
                    {
                        // Casual A-Life: came from Escalate → Services, return there
                        _aLifeHookerServices = false;
                        menuIndex = 0;
                        if (_postSexPhase == 1) { _postSexPendingMenu = MenuLevel.Escalate; return; }
                        menuLevel = MenuLevel.Escalate;
                        return;
                    }
                }
            }
            menuIndex = 0;
            if (_postSexPhase == 1) { _postSexPendingMenu = _streetSitCategory; return; }
            menuLevel = _streetSitCategory;
        }

        // ═════════════
        // END / CLEANUP
        // ── XInput controller vibration ───────────────────────────────────
        [StructLayout(LayoutKind.Sequential)]
        private struct XINPUT_VIBRATION { public ushort wLeftMotorSpeed; public ushort wRightMotorSpeed; }
        [DllImport("xinput1_4.dll", EntryPoint = "XInputSetState")]
        private static extern int XInputSetState(int dwUserIndex, ref XINPUT_VIBRATION pVibration);
        private static void SetVibration(ushort left, ushort right)
        {
            try { XINPUT_VIBRATION v = new XINPUT_VIBRATION { wLeftMotorSpeed = left, wRightMotorSpeed = right }; XInputSetState(0, ref v); }
            catch { } // silently ignore if XInput unavailable
        }

        // ═════════════
        /// <summary>Disable player movement controls for this frame (legs stay still during upper-body zip anim).
        /// Camera remains fully controllable. Must be called every tick while movement should be blocked.</summary>
        private void DisablePlayerMovement()
        {
            // 0 = player control group
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, (int)GTA.Control.MoveUpDown, true);     // W/S / Left stick Y
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, (int)GTA.Control.MoveLeftRight, true);  // A/D / Left stick X
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, (int)GTA.Control.Sprint, true);         // Shift / A button
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, (int)GTA.Control.Jump, true);           // Space / X button
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, (int)GTA.Control.MoveUpOnly, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, (int)GTA.Control.MoveDownOnly, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, (int)GTA.Control.MoveLeftOnly, true);
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, (int)GTA.Control.MoveRightOnly, true);
        }

        // ═════════════
        private void EndAnimation()
        {
            Ped player = Game.Player.Character;

            // Stop rumble — only wait on actual finish bursts, not regular loop pulses (abort case).
            // Street: 2000ms burst. Car: already long (outro anim plays for seconds), so 600ms cap.
            int burstDuration = (mode == Mode.Street) ? 2000 : 600;
            if (_finishBurstFired && Game.GameTime - _lastRumbleTime <= burstDuration)
                Script.Wait(burstDuration - (Game.GameTime - _lastRumbleTime));
            SetVibration(0, 0);
            _rumblePulseActive = false;
            _finishBurstFired  = false;
            if (mode == Mode.Street)
            {
                // Matches the old HotCoffee End() exactly:
                //   Player.FreezePosition = false
                //   hotCoffeePed.FreezePosition = false
                //   hotCoffeePed.SetNoCollision(Player, false)
                //   hotCoffeePed.Task.ClearAll()
                //   Player.Task.ClearAll()
                player.IsPositionFrozen = false;
                Function.Call(Hash.SET_ENTITY_COLLISION, player, true, true);
                player.Task.ClearAll();

                if (girl != null && girl.Exists())
                {
                    girl.IsPositionFrozen = false;
                    Function.Call(Hash.SET_ENTITY_COLLISION, girl, true, true);
                    Function.Call(Hash.SET_ENTITY_NO_COLLISION_ENTITY, girl, player, true);
                    // Release task lock so ClearAll works, then re-lock for idle
                    Function.Call(Hash.SET_PED_KEEP_TASK, girl, false);
                    girl.Task.ClearAll();
                    Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, girl, true);
                    // Resume following the player
                    float dist = girl.Position.DistanceTo(player.Position);
                    float speed = (dist > 7f) ? 3f : 1f;
                    Function.Call(Hash.TASK_FOLLOW_TO_OFFSET_OF_ENTITY,
                        girl, player, 0f, -1f, 0f, speed, -1, 1.5f, true);
                }
            }
            else
            {
                Function.Call(Hash.CLEAR_PED_SECONDARY_TASK, player);
                if (player.IsInVehicle() && player.CurrentVehicle != null)
                {
                    player.CurrentVehicle.IsInteriorLightOn = false;
                }
                if (girl != null && girl.Exists())
                {
                    Function.Call(Hash.SET_PED_KEEP_TASK, girl, false);
                    Function.Call(Hash.CLEAR_PED_SECONDARY_TASK, girl);
                    // Re-lock after clearing task
                    Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, girl, true);
                }
            }

            // Finish heal boost (BJ→player, facesitting→girl, sex→both)
            if (healPlayer)
                player.Health = Math.Min(player.Health + FINISH_HEAL, player.MaxHealth);
            if (healGirl && girl != null && girl.Exists())
                girl.Health = Math.Min(girl.Health + FINISH_HEAL, girl.MaxHealth);

            DetachDildoProp();

            // Re-enable everything
            Function.Call(Hash.SET_PLAYER_CAN_DO_DRIVE_BY, Game.Player, true);
            Function.Call(Hash.SET_EVERYONE_IGNORE_PLAYER, Game.Player, false);

            bool wasDeviceExcluded = _deviceExcluded; // capture before reset

            state = ScriptState.Idle;
            animPhase = 0;
            animSpeed = 1.0f;
            _femaleAnimSpeedScale = 1.0f;
            speedCooldown = 0;
            _deviceExcluded = false;
            _kissingReverse = false;
            _standingBjReverse = false;
            _rdoggyReverse = false;
            _sitCowgirlPulloutReverse = false;
            _standingBjSwallowAt = 0;
            _backseatCarSex = false;
            _streetSitPropPhase = 0;
            currentAction = ""; // Clear action after all animations complete
            lastPaidAmount = 0; // Service completed — no refund
            _throatSpeedLocked   = false;
            _throatLockAt        = 0;
            _throatSequenceEndAt = 0;
            _finishExecutePending = false;
            _finishExecuteAt      = 0;
            _finishInside         = false;
            bool wasFinishForced  = _finishForced; // capture before reset
            _finishForced         = false;
            _finishBroke          = false;
            _finishAsked          = false;
            // Don't award +1 rep for a forced-inside finish — keeps rep at -2 so the release block fires.
            if (!wasFinishForced && aLifeMode && _currentGirlKey != null)
            {
                ALifePedData dSession;
                if (_aLifePeds.TryGetValue(_currentGirlKey, out dSession) && dSession != null && dSession.Reputation >= 0)
                {
                    dSession.Reputation = Math.Min(REP_MAX, dSession.Reputation + 1);
                    UpdateRelationshipState(dSession);
                    SaveALife();
                }
            }
            // A-Life: if rep dropped to hostile (-2) during this session, force-release her immediately.
            // At rep -1 (avoiding) she completed the session on her own terms — let normal idle resume.
            if (aLifeMode && girl != null && girl.Exists() && _currentGirlFp != null)
            {
                ALifePedData dWander;
                if (_aLifePeds.TryGetValue(_currentGirlKey, out dWander) && dWander.Reputation <= REP_MIN)
                {
                    // Fully release her so the main loop doesn't re-offer services after she walks away
                    _aLifeHookerServices = false;
                    Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, girl, false);
                    girl.Task.WanderAround();
                    Function.Call((Hash)0x726256CC1EEB182F, girl); // CLEAR_FACIAL_IDLE_ANIM_OVERRIDE
                    girl.IsPositionFrozen = false;
                    girl.BlockPermanentEvents = false;
                    girl.IsPersistent = false;
                    girl.MarkAsNoLongerNeeded();
                    girl  = null;
                    hasGirl = false;
                    _currentGirlFp  = null;
                    _currentGirlKey = null;
                    mode = Mode.None;
                    menuLevel = MenuLevel.None;
                }
            }
            keyCooldown = KEY_COOLDOWN_FRAMES * 3; // Extra cooldown so menu can't open immediately

            // Skip finish sequence for excluded acts (kissing/facesitting) — device was never running
            if (!wasDeviceExcluded) _telemetry.SendFinish();
            ShowHudStatus("~g~DONE!", 3000);
        }

        // ═══════
        // HELPERS
        // ═══════

        /// <summary>Attach dildo prop to player's pelvis bone with per-animation offsets.</summary>
        private void AttachDildoProp(Ped player)
        {
            if (!dildoEnabled) return; // Respect toggle setting
            DetachDildoProp(); // Clean up any existing prop first

            Model mdl = new Model("prop_cs_dildo_01");
            mdl.Request(1000);
            if (!mdl.IsLoaded) return;

            dildoProp = World.CreateProp(mdl, player.Position, false, false);
            mdl.MarkAsNoLongerNeeded();
            if (dildoProp == null) return;

            float oX, oY, oZ, rX, rY, rZ;
            GetDildoPropOffsets(out oX, out oY, out oZ, out rX, out rY, out rZ);

            int boneIndex = Function.Call<int>(Hash.GET_PED_BONE_INDEX, player, 11816); // SKEL_Pelvis
            Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, dildoProp, player, boneIndex,
                oX, oY, oZ,       // offset X, Y, Z
                rX, rY, rZ,       // rotation X, Y, Z
                true, true, false, false, 2, true);
        }

        /// <summary>Per-animation dildo prop offset/rotation lookup.</summary>
        private void GetDildoPropOffsets(out float oX, out float oY, out float oZ,
            out float rX, out float rY, out float rZ)
        {
            // Default = standing BJ offsets (confirmed working)
            oX = 0.04f; oY = 0.13f; oZ = 0.0f;
            rX = 250.0f; rY = -35.0f; rZ = 0.0f;

            if (mode == Mode.Car)
            {
                if (_backseatCarSex)
                {
                    // Backseat car sex — cowgirl-like position
                    oX = 0.145f; oY = 0.13f; oZ = -0.01f;
                    rX = 255.0f; rY = 44.0f; rZ = 0.0f;
                    return;
                }
                if (currentAction == "blowjob")
                {
                    if (vehicleAnimType == VehicleAnimType.Low)
                    {
                        // Low car BJ — player reclined, girl leaning over
                        oX = 0.05f; oY = 0.15f; oZ = 0.0f;
                        rX = 260.0f; rY = -15.0f; rZ = 0.0f;
                    }
                    else
                    {
                        // Normal car BJ
                        oX = 0.04f; oY = 0.13f; oZ = 0.0f;
                        rX = 260.0f; rY = -35.0f; rZ = 0.0f;
                    }
                }
                else // sex
                {
                    if (vehicleAnimType == VehicleAnimType.Low)
                    {
                        // Low car sex — player reclined, girl riding
                        oX = 0.07f; oY = 0.14f; oZ = 0.01f;
                        rX = 270.0f; rY = -35.0f; rZ = 0.0f;
                    }
                    else
                    {
                        // Normal car sex
                        oX = 0.07f; oY = 0.14f; oZ = 0.025f;
                        rX = 280.0f; rY = -35.0f; rZ = 0.0f;
                    }
                }
            }
            else // Street
            {
                switch (streetActionIdx)
                {
                    // case 0: kissing — no dildo (already skipped)
                    // case 1: facesitting — no dildo (already skipped)
                    case 2: // Standing BJ
                        oX = 0.07f; oY = 0.15f; oZ = 0.0f;
                        rX = 240.0f; rY = -8.0f; rZ = 0.0f;
                        break;
                    case 3: // Sitting BJ
                        oX = 0.04f; oY = 0.13f; oZ = 0.0f;
                        rX = 260.0f; rY = -35.0f; rZ = 0.0f;
                        break;
                    case 4: // Laying BJ
                        oX = 0.09f; oY = 0.16f; oZ = 0.01f;
                        rX = 260.0f; rY = -5.0f; rZ = 0.0f;
                        break;
                    case 5: // Wall Banger
                        oX = 0.085f; oY = 0.15f; oZ = 0.03f;
                        rX = 283.0f; rY = -22.0f; rZ = 0.0f;
                        break;
                    case 6: // Sitting Cowgirl
                        oX = 0.07f; oY = 0.14f; oZ = 0.01f;
                        rX = 270.0f; rY = -35.0f; rZ = 0.0f;
                        break;
                    case 7: // Reverse Cowgirl
                        oX = 0.07f; oY = 0.14f; oZ = 0.01f;
                        rX = 270.0f; rY = 2.0f; rZ = 0.0f;
                        break;
                    case 8: // Laying Cowgirl
                        oX = 0.07f; oY = 0.14f; oZ = 0.01f;
                        rX = 270.0f; rY = 2.0f; rZ = 0.0f;
                        break;
                    case 9: // Laying Reversed
                        oX = 0.01f; oY = 0.14f; oZ = 0.01f;
                        rX = 270.0f; rY = -16.0f; rZ = 0.0f;
                        break;
                    case 10: // Doggy Style
                        oX = 0.104f; oY = 0.14f; oZ = 0.0f;
                        rX = 265.0f; rY = -32.0f; rZ = 0.0f;
                        break;
                    case 11: // Rough Doggy
                        oX = 0.13f; oY = 0.14f; oZ = -0.0275f;
                        rX = 230.0f; rY = -5.0f; rZ = 0.0f;
                        break;
                }
            }
        }

        /// <summary>Detach and delete the dildo prop if it exists.</summary>
        private void DetachDildoProp()
        {
            if (dildoProp != null && dildoProp.Exists())
            {
                dildoProp.Detach();
                dildoProp.Delete();
            }
            dildoProp = null;
        }

        /// <summary>Find the nearest non-male ped within range.</summary>
        private Ped FindNearestFemale(Vector3 pos, float range)
        {
            Ped best = null;
            float bestDist = range;

            Ped[] nearby = World.GetNearbyPeds(pos, range);
            if (nearby == null) return null;

            foreach (Ped p in nearby)
            {
                if (p == null || !p.Exists()) continue;
                if (!p.IsAlive || p.IsInVehicle()) continue;
                if (p == Game.Player.Character) continue;

                // Check if female (IS_PED_MALE returns false)
                if (Function.Call<bool>(Hash.IS_PED_MALE, p)) continue;

                float d = p.Position.DistanceTo(pos);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = p;
                }
            }
            return best;
        }

        /// <summary>Request and wait for an animation dictionary to load.</summary>
        private bool RequestAnimDict(string dict)
        {
            Function.Call(Hash.REQUEST_ANIM_DICT, dict);

            int timeout = 1000; // ms
            int start = Game.GameTime;
            while (!Function.Call<bool>(Hash.HAS_ANIM_DICT_LOADED, dict))
            {
                Wait(0);
                if (Game.GameTime - start > timeout)
                    return false;
            }
            return true;
        }

        // ── A-Life helpers ─────────────────────────────────────────────────────

        /// <summary>
        /// Build a stable identity string for a ped from her model hash +
        /// the drawable and texture index of all 12 outfit component slots.
        /// Two peds with identical model and full outfit collide to the same key
        /// and are treated as the same person across sessions.
        /// </summary>
        /// <summary>Returns the namespaced _aLifePeds dict key for a raw fingerprint.
        /// Casual A-Life (sandboxMode) uses "C:" prefix; Prost A-Life uses "P:".
        /// Guarantees that the same NPC can have fully separate records in each mode.</summary>
        private string ALifeKey(string rawFp)
        {
            return rawFp == null ? null : (sandboxMode ? "C:" : "P:") + rawFp;
        }

        private string GetPedFingerprint(Ped ped)
        {
            if (ped == null || !ped.Exists()) return null;
            var sb = new System.Text.StringBuilder();
            sb.Append(ped.Model.Hash.ToString("X8"));
            for (int comp = 0; comp < 12; comp++)
            {
                int d = Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION,  ped, comp);
                int t = Function.Call<int>(Hash.GET_PED_TEXTURE_VARIATION,   ped, comp);
                sb.Append('_').Append(comp).Append(':').Append(d).Append('/').Append(t);
            }
            return sb.ToString();
        }

        /// <summary>Get or create the A-Life record for a ped, rolling name and personality on first encounter.</summary>
        private ALifePedData GetOrCreateALifeData(Ped ped)
        {
            string fp = GetPedFingerprint(ped);
            if (fp == null) return null;

            ALifePedData data;
            string casualKey = "C:" + fp;
            if (!_aLifePeds.TryGetValue(casualKey, out data))
            {
                data = new ALifePedData(fp);

                // Roll a random female first name
                data.Name = FEMALE_NAMES[rng.Next(FEMALE_NAMES.Length)];

                // Roll personality from the full 20-personality table
                data.Personality = PERSONALITIES[rng.Next(PERSONALITIES.Length)].Name;

                // Assign a permanent life aspiration driven by personality traits
                AssignLongTermGoal(data);

                // First-time Cash/Stored initialisation — Casual women are net spenders
                data.Cash   = rng.Next(0, 81);   // $0–$80 pocket money
                data.Stored = rng.Next(0, 301);  // $0–$300 savings

                // Roll cum-in-mouth preference once at creation — personality-weighted, never changes
                data.CimPreference = RollCimPreference(data.Personality, "普通");

                _aLifePeds[casualKey] = data;
                SaveALife();
            }

            // Randomize mood on first encounter each session (mood is transient — not saved to disk).
            // Default "放鬆" appears when a ped is freshly loaded or has never had a conversation.
            // Weights are personality-influenced: warm types lean Playful/Relaxed, cold types lean Guarded/Annoyed.
            if (data.Mood == null || data.Mood.Length == 0 || data.Mood == "Relaxed")
            {
                PersonalityProfile mProf = GetProfile(data.Personality);
                double mFri = (mProf != null) ? mProf.Friendliness : 0.50;
                int mRoll = rng.Next(100);
                if (mFri >= 0.70) // Warm personalities — Relaxed + Happy dominate
                {
                    if      (mRoll < 30) data.Mood = "Relaxed";
                    else if (mRoll < 52) data.Mood = "Happy";
                    else if (mRoll < 70) data.Mood = "玩鬧";
                    else if (mRoll < 82) data.Mood = "Alert";
                    else if (mRoll < 92) data.Mood = "Needy";
                    else                 data.Mood = "Annoyed";
                }
                else if (mFri >= 0.40) // Neutral personalities — Happy possible but rare
                {
                    if      (mRoll < 25) data.Mood = "Relaxed";
                    else if (mRoll < 33) data.Mood = "Happy";
                    else if (mRoll < 55) data.Mood = "Alert";
                    else if (mRoll < 75) data.Mood = "Annoyed";
                    else if (mRoll < 88) data.Mood = "玩鬧";
                    else if (mRoll < 96) data.Mood = "Needy";
                    else                 data.Mood = "Jealous";
                }
                else // Cold / hostile personalities — Happy almost never
                {
                    if      (mRoll < 42) data.Mood = "Alert";
                    else if (mRoll < 69) data.Mood = "Annoyed";
                    else if (mRoll < 82) data.Mood = "Relaxed";
                    else if (mRoll < 90) data.Mood = "Jealous";
                    else if (mRoll < 97) data.Mood = "玩鬧";
                    else                 data.Mood = "Happy";
                }

            }

            return data;
        }

        /// <summary>Get or create the Prostitution A-Life record for a hooker ped.
        /// Always sets ALifeMode="Prostitute" and IsHooker=true on creation.</summary>
        private ALifePedData GetOrCreateProstALifeData(Ped ped)
        {
            string fp = GetPedFingerprint(ped);
            if (fp == null) return null;

            ALifePedData data;
            string prostKey = "P:" + fp;
            if (!_aLifePeds.TryGetValue(prostKey, out data))
            {
                data = new ALifePedData(fp);
                data.ALifeMode = "Prostitute";
                data.IsHooker  = true;
                data.Name        = FEMALE_NAMES[rng.Next(FEMALE_NAMES.Length)];
                data.Personality = PERSONALITIES[rng.Next(PERSONALITIES.Length)].Name;

                // Assign a permanent life aspiration driven by personality traits
                AssignLongTermGoal(data);

                // First-time Cash/Stored initialisation — hookers carry little on-body; most is stashed
                data.Cash   = rng.Next(5, 41);    // $5–$40 pocket money
                data.Stored = rng.Next(50, 501);  // $50–$500 stash

                // Roll cum-in-mouth preference once at creation — personality-weighted, never changes
                data.CimPreference = RollCimPreference(data.Personality, "交易");

                _aLifePeds[prostKey] = data;
                SaveALife();
            }
            else
            {
                // Upgrade legacy record created before ALifeMode field was added
                if (data.ALifeMode == null || data.ALifeMode == "Normal")
                {
                    data.ALifeMode = "Prostitute";
                    data.IsHooker  = true;
                    SaveALife();
                }
            }

            // Roll session mood (personality-weighted, same logic as casual)
            if (data.Mood == null || data.Mood.Length == 0 || data.Mood == "Relaxed")
            {
                PersonalityProfile mProf = GetProfile(data.Personality);
                double mFri = (mProf != null) ? mProf.Friendliness : 0.50;
                int mRoll = rng.Next(100);
                if (mFri >= 0.70)
                {
                    if      (mRoll < 30) data.Mood = "Relaxed";
                    else if (mRoll < 52) data.Mood = "Happy";
                    else if (mRoll < 70) data.Mood = "玩鬧";
                    else if (mRoll < 82) data.Mood = "Alert";
                    else if (mRoll < 92) data.Mood = "Needy";
                    else                 data.Mood = "Annoyed";
                }
                else if (mFri >= 0.40)
                {
                    if      (mRoll < 25) data.Mood = "Relaxed";
                    else if (mRoll < 33) data.Mood = "Happy";
                    else if (mRoll < 55) data.Mood = "Alert";
                    else if (mRoll < 75) data.Mood = "Annoyed";
                    else if (mRoll < 88) data.Mood = "玩鬧";
                    else if (mRoll < 96) data.Mood = "Needy";
                    else                 data.Mood = "Jealous";
                }
                else
                {
                    if      (mRoll < 42) data.Mood = "Alert";
                    else if (mRoll < 69) data.Mood = "Annoyed";
                    else if (mRoll < 82) data.Mood = "Relaxed";
                    else if (mRoll < 90) data.Mood = "Jealous";
                    else if (mRoll < 97) data.Mood = "玩鬧";
                    else                 data.Mood = "Happy";
                }
            }

            return data;
        }

        /// <summary>Load all known A-Life ped records from disk.</summary>
        private void LoadALife()
        {
            _aLifePeds.Clear();
            if (!System.IO.File.Exists(ALIFE_FILE)) return;
            try
            {
                string currentFp = null;
                string name = ""; int rep = 0; string relState = "Stranger"; string lastPosRel = ""; string pers = ""; bool hasMet = false; bool hasMetSecondTime = false; string savedVoice = ""; string hookerAnim = ""; bool isHooker = false; bool wasHooker = false; bool hookerPermaReject = false; bool nameKnown = false; bool? prefBJ = null; bool? prefRough = null; int[] hookerPrices = null; string savedMode = "Normal";
                int cash = 0; int stored = 0; long lastSimUtc = 0; bool civilianRejected = false; bool cimPreference = false; long knownTopics = 0; string ltGoal = "";
                foreach (string raw in System.IO.File.ReadAllLines(ALIFE_FILE))
                {
                    string line = raw.Trim();
                    if (line.StartsWith("[FP:"))
                    {
                        // flush previous block
                        if (currentFp != null)
                        {
                            // Always null Prost A-Life prices — they regenerate fresh each session.
                            // This guarantees stale prices from any old code version never survive.
                            // Casual hooker prices are fine to regenerate too (minor inconvenience only).
                            if (savedMode == "Prostitute") hookerPrices = null;
                            // Migrate unprefixed legacy keys to namespaced keys
                            string flushKey = (currentFp.StartsWith("C:") || currentFp.StartsWith("P:"))
                                ? currentFp
                                : (savedMode == "Prostitute" ? "P:" : "C:") + currentFp;
                            string rawFpFlush = flushKey.Length > 2 ? flushKey.Substring(2) : currentFp;
                            var d = new ALifePedData(rawFpFlush) { Name = name, Reputation = rep, Relationship = relState, LastPositiveRelationship = lastPosRel, Personality = pers, HasMet = hasMet, HasMetSecondTime = hasMetSecondTime, Voice = savedVoice, HookerAnim = hookerAnim, IsHooker = isHooker, WasHooker = wasHooker, HookerPermaReject = hookerPermaReject, NameKnown = nameKnown, PrefBJ = prefBJ, PrefRough = prefRough, HookerPrices = hookerPrices, ALifeMode = savedMode, Cash = cash, Stored = stored, LastSimUtc = lastSimUtc, CivilianRejected = civilianRejected, CimPreference = cimPreference, KnownTopics = knownTopics, LongTermGoal = ltGoal };
                            _aLifePeds[flushKey] = d;
                        }
                        currentFp = line.Substring(4, line.Length - 5); // strip [FP: and ]
                        name = ""; rep = 0; relState = "Stranger"; lastPosRel = ""; pers = ""; hasMet = false; hasMetSecondTime = false; savedVoice = ""; hookerAnim = ""; isHooker = false; wasHooker = false; hookerPermaReject = false; nameKnown = false; prefBJ = null; prefRough = null; hookerPrices = null; savedMode = "Normal";
                        cash = 0; stored = 0; lastSimUtc = 0; civilianRejected = false; cimPreference = false; knownTopics = 0; ltGoal = "";
                    }
                    else if (line.StartsWith("Name="))         name = line.Substring(5);
                    else if (line.StartsWith("Reputation="))  int.TryParse(line.Substring(11), out rep);
                    else if (line.StartsWith("Relationship=") || line.StartsWith("關係度=") || line.StartsWith("關係度=")) { int old; if (int.TryParse(line.Substring(13), out old)) rep = old; else relState = line.Substring(13); }
                    else if (line.StartsWith("Personality="))  pers = line.Substring(12);
                    else if (line.StartsWith("HasMet="))       bool.TryParse(line.Substring(7), out hasMet);
                    else if (line.StartsWith("HasMetSecondTime=")) bool.TryParse(line.Substring(17), out hasMetSecondTime);
                    else if (line.StartsWith("Voice="))        savedVoice = line.Substring(6);
                    else if (line.StartsWith("HookerAnim="))   hookerAnim = line.Substring(11);
                    else if (line.StartsWith("IsHooker="))     bool.TryParse(line.Substring(9), out isHooker);
                    else if (line.StartsWith("WasHooker="))        bool.TryParse(line.Substring(10), out wasHooker);
                    else if (line.StartsWith("HookerPermaReject=") || line.StartsWith("永久拒絕=") || line.StartsWith("永久拒絕=")) bool.TryParse(line.Substring(18), out hookerPermaReject);
                    else if (line.StartsWith("NameKnown="))    bool.TryParse(line.Substring(10), out nameKnown);
                    else if (line.StartsWith("Mode=") || line.StartsWith("模式=") || line.StartsWith("模式="))           savedMode = line.Substring(5);
                    else if (line.StartsWith("PrefBJ="))    { bool _b; int _i; string _s = line.Substring(7);  if (bool.TryParse(_s, out _b)) prefBJ    = _b; else if (int.TryParse(_s,  out _i)) prefBJ    = _i > 0 ? (bool?)true : _i < 0 ? (bool?)false : null; }
                    else if (line.StartsWith("PrefRough=")) { bool _b; int _i; string _s = line.Substring(10); if (bool.TryParse(_s, out _b)) prefRough = _b; else if (int.TryParse(_s,  out _i)) prefRough = _i > 0 ? (bool?)true : _i < 0 ? (bool?)false : null; }
                    else if (line.StartsWith("Cash="))         int.TryParse(line.Substring(5), out cash);
                    else if (line.StartsWith("Stored="))       int.TryParse(line.Substring(7), out stored);
                    else if (line.StartsWith("LastSimUtc="))   long.TryParse(line.Substring(11), out lastSimUtc);
                    else if (line.StartsWith("CivilianRejected=")) bool.TryParse(line.Substring(17), out civilianRejected);
                    else if (line.StartsWith("cimPreference="))    bool.TryParse(line.Substring(14), out cimPreference);
                    else if (line.StartsWith("KnownTopics="))      long.TryParse(line.Substring(12), out knownTopics);
                    else if (line.StartsWith("LongTermGoal="))     ltGoal = line.Substring(13);
                    else if (line.StartsWith("HookerPrices=")) {
                        string[] parts = line.Substring(13).Split(',');
                        if (parts.Length == 5) {
                            hookerPrices = new int[5];
                            bool ok = true;
                            for (int pi = 0; pi < 5; pi++) if (!int.TryParse(parts[pi].Trim(), out hookerPrices[pi])) { ok = false; break; }
                            if (!ok) hookerPrices = null;
                        }
                        // legacy 7 or 14-entry arrays silently dropped — prices re-roll next session
                    }
                }
                // flush last block
                if (currentFp != null)
                {
                    if (savedMode == "Prostitute") hookerPrices = null;
                    string lastKey = (currentFp.StartsWith("C:") || currentFp.StartsWith("P:"))
                        ? currentFp
                        : (savedMode == "Prostitute" ? "P:" : "C:") + currentFp;
                    string rawFpLast = lastKey.Length > 2 ? lastKey.Substring(2) : currentFp;
                    var d = new ALifePedData(rawFpLast) { Name = name, Reputation = rep, Relationship = relState, LastPositiveRelationship = lastPosRel, Personality = pers, HasMet = hasMet, HasMetSecondTime = hasMetSecondTime, Voice = savedVoice, HookerAnim = hookerAnim, IsHooker = isHooker, WasHooker = wasHooker, HookerPermaReject = hookerPermaReject, NameKnown = nameKnown, PrefBJ = prefBJ, PrefRough = prefRough, HookerPrices = hookerPrices, ALifeMode = savedMode, Cash = cash, Stored = stored, LastSimUtc = lastSimUtc, CivilianRejected = civilianRejected, CimPreference = cimPreference, KnownTopics = knownTopics, LongTermGoal = ltGoal };
                    _aLifePeds[lastKey] = d;
                }
            }
            catch { /* corrupt file — start fresh */ }
            // Migration: repair any Casual peds that were incorrectly assigned "Regular"
            // (a Prostitution-only tier) due to a bug in the approach-accept path.
            bool migrationChanged = false;
            foreach (var kvp in _aLifePeds)
            {
                ALifePedData md = kvp.Value;
                if (md.ALifeMode != "Prostitute" && md.Relationship == "Regular")
                {
                    // Re-derive the correct tier from rep using the casual ladder
                    UpdateCasualRelationshipState(md);
                    migrationChanged = true;
                }
            }
            if (migrationChanged) SaveALife();
            RunOfflineSimulation(); // apply elapsed real-world time to all loaded records
        }

        // ══════════════════════════════════════════════
        // OFFLINE SIMULATION
        // ══════════════════════════════════════════════

        /// <summary>
        /// Process offline time for every A-Life ped record.
        /// Called once on load (applies real-world downtime) and every 30 s in-game.
        /// Only peds more than 150 m away from the player are eligible while the game is running;
        /// on first load LastSimUtc may be zero (new record) — handled gracefully.
        /// </summary>
        private void RunOfflineSimulation()
        {
            long nowUtc = DateTime.UtcNow.Ticks;
            Ped player = Game.Player != null ? Game.Player.Character : null;
            bool anyChanged = false;

            // Build a set of A-Life keys for every ped currently loaded in the world
            // within 150 m. This covers ALL nearby peds — not just the hired girl or
            // approach candidate — so none of them get offline-simulated while visible.
            var nearbyKeys = new HashSet<string>();
            if (player != null && player.Exists())
            {
                foreach (Ped np in World.GetNearbyPeds(player, 150f))
                {
                    if (np == null || !np.Exists()) continue;
                    string nFp = GetPedFingerprint(np);
                    if (nFp == null) continue;
                    // Check both namespaces — same ped may appear in C: and P: records
                    nearbyKeys.Add("C:" + nFp);
                    nearbyKeys.Add("P:" + nFp);
                }
            }

            foreach (var kvp in _aLifePeds)
            {
                ALifePedData d = kvp.Value;

                // Skip peds within render distance (150 m) — they are handled live
                if (nearbyKeys.Contains(kvp.Key))
                {
                    d.LastSimUtc = nowUtc; // keep timestamp current, but don't sim
                    continue;
                }

                // Compute elapsed real-world seconds since last sim
                if (d.LastSimUtc <= 0) { d.LastSimUtc = nowUtc; continue; }
                double elapsed = new TimeSpan(nowUtc - d.LastSimUtc).TotalSeconds;
                if (elapsed < 1.0) continue;

                if (SimulatePed(d, elapsed)) anyChanged = true;
                d.LastSimUtc = nowUtc;
            }

            if (anyChanged) SaveALife();
        }

        /// <summary>
        /// Simulate one ped over <paramref name="elapsedSeconds"/> of real-world offline time.
        /// Runs money drift (income / spend / savings management / pimp cut) then active goals.
        /// Returns true if any persistent field changed (triggers SaveALife).
        /// </summary>
        private bool SimulatePed(ALifePedData d, double elapsedSeconds)
        {
            bool changed = false;
            bool isProst = (d.ALifeMode == "Prostitute");

            // ── Money simulation ──────────────────────────────────────────────────────
            // Scale elapsed time to 30-second ticks; cap at 10 ticks to prevent runaway
            // offline jumps (e.g. player away for hours).
            double rawTicks  = elapsedSeconds / 30.0;
            int    fullTicks = (int)Math.Min(rawTicks, 10.0);
            // Fractional-tick fires probabilistically so long intervals stay smooth
            if (rng.NextDouble() < (rawTicks % 1.0)) fullTicks = Math.Min(fullTicks + 1, 10);

            PersonalityProfile prof = GetProfile(d.Personality);
            double greed = (prof != null) ? prof.Greed : 0.50;
            // "Dumb" depositors: Chaotic / Unstable / Party Girl rarely bank money
            bool isDumbDepositor = prof != null &&
                (prof.Name == "Chaotic" || prof.Name == "Unstable" || prof.Name == "Party Girl");

            if (isProst)
            {
                // ── Prostitution: net earner ─────────────────────────────────────────
                for (int t = 0; t < fullTicks; t++)
                {
                    // ~70 % chance: earn from a client
                    if (rng.NextDouble() < 0.70)
                    {
                        d.Cash = Math.Max(0, d.Cash + rng.Next(15, 61)); // $15–$60
                        changed = true;
                    }
                    // ~35 % chance: spend on upkeep / personal
                    if (rng.NextDouble() < 0.35)
                    {
                        d.Cash = Math.Max(0, d.Cash - rng.Next(5, 26));  // $5–$25
                        changed = true;
                    }
                }

                // ── Deposit logic: pimp cut (hooker keeps 25 % of deposited amount) ──
                if (d.Cash > 0)
                {
                    double depositChance;
                    if      (d.Cash >= 600) depositChance = 0.75 + greed * 0.20;
                    else if (d.Cash >= 400) depositChance = 0.45 + greed * 0.20;
                    else if (d.Cash >= 200) depositChance = 0.18 + greed * 0.15;
                    else                    depositChance = 0.04 + greed * 0.05;

                    if (isDumbDepositor) depositChance *= 0.20; // dumb hookers rarely deposit

                    if (rng.NextDouble() < depositChance)
                    {
                        // Pimp takes 75 %, hooker banks the remaining 25 %
                        int hookerKeeps = (int)Math.Ceiling(d.Cash * 0.25);
                        d.Stored  = Math.Max(0, d.Stored + hookerKeeps);
                        d.Cash    = 0;
                        changed   = true;
                    }
                }
            }
            else
            {
                // ── Casual: net spender with savings management behaviour ─────────────
                for (int t = 0; t < fullTicks; t++)
                {
                    // ~65 % chance: spend — comes from Cash first
                    if (rng.NextDouble() < 0.65)
                    {
                        int spend    = rng.Next(5, 31);              // $5–$30
                        int fromCash = Math.Min(spend, d.Cash);
                        d.Cash = Math.Max(0, d.Cash - fromCash);
                        int leftover = spend - fromCash;
                        // Dip into savings only when nearly broke and not greedy
                        if (leftover > 0 && rng.NextDouble() < (1.0 - greed) * 0.25)
                            d.Stored = Math.Max(0, d.Stored - leftover);
                        changed = true;
                    }

                    // ~20 % chance: income event (paycheck / tip / side gig)
                    if (rng.NextDouble() < 0.20)
                    {
                        d.Cash  = Math.Max(0, d.Cash + rng.Next(30, 101)); // $30–$100
                        changed = true;
                    }

                    // ~30 % chance: deposit to savings when Cash ≥ $150 (smart management)
                    if (d.Cash >= 150 && rng.NextDouble() < 0.30)
                    {
                        int save = rng.Next(20, Math.Min(81, d.Cash + 1)); // $20–$80
                        d.Cash   = Math.Max(0, d.Cash - save);
                        d.Stored = Math.Max(0, d.Stored + save);
                        changed  = true;
                    }

                    // ~15 % chance: withdraw from savings when nearly broke (impulse / necessity)
                    if (d.Cash < 30 && d.Stored > 0 && rng.NextDouble() < 0.15)
                    {
                        int withdraw = Math.Min(rng.Next(20, 51), d.Stored); // $20–$50
                        d.Cash   = Math.Max(0, d.Cash + withdraw);
                        d.Stored = Math.Max(0, d.Stored - withdraw);
                        changed  = true;
                    }
                }
            }

            return changed;
        }

        /// <summary>Persist all A-Life ped records to disk.</summary>
        // ================================================================
        //  LONG-TERM GOAL ASSIGNMENT
        // ================================================================

        /// <summary>
        /// Assign a permanent life aspiration to a ped the first time she is encountered.
        /// Called immediately after Personality is set. Never called again — LongTermGoal never changes.
        ///
        /// Casual goals:       GetRich | FindLove | StayFree | HaveFun | Survive | FindProtection
        /// Prostitution goals: GetRich | GetOut   | Survive  | FindProtection
        ///
        /// Decision weights are driven by the personality profile traits so the goal
        /// feels consistent with how she already behaves.
        /// </summary>
        private void AssignLongTermGoal(ALifePedData d)
        {
            if (d == null) return;
            // Already assigned — never overwrite
            if (d.LongTermGoal != null && d.LongTermGoal.Length > 0) return;

            PersonalityProfile prof = GetProfile(d.Personality);
            double greed      = (prof != null) ? prof.Greed       : 0.50;
            double attachment = (prof != null) ? prof.Attachment  : 0.50;
            double riskiness  = (prof != null) ? prof.Riskiness   : 0.50;
            double confidence = (prof != null) ? prof.Confidence  : 0.50;
            double patience   = (prof != null) ? prof.Patience    : 0.50;
            double friendli   = (prof != null) ? prof.Friendliness: 0.50;
            double aggr       = (prof != null) ? prof.Aggressiveness : 0.50;

            if (d.ALifeMode == "Prostitute")
            {
                // Prostitution A-Life: four goals
                // GetRich  — she does this for money and is committed to it
                // GetOut   — she's exhausted and wants out of the life
                // Survive  — just covering rent, no ambition either way
                // FindProtection — scared and wants a reliable client
                double wGetRich         = greed * 1.8 + confidence * 0.6;
                double wGetOut          = (1.0 - patience) * 1.4 + (1.0 - greed) * 0.5;
                double wSurvive         = 1.0; // baseline always available
                double wFindProtection  = (1.0 - aggr) * 1.2 + attachment * 0.6;
                double total = wGetRich + wGetOut + wSurvive + wFindProtection;
                double roll  = rng.NextDouble() * total;
                if      (roll < wGetRich)                          d.LongTermGoal = "Get Rich";
                else if (roll < wGetRich + wGetOut)                d.LongTermGoal = "GetOut";
                else if (roll < wGetRich + wGetOut + wSurvive)     d.LongTermGoal = "Survive";
                else                                               d.LongTermGoal = "Seek Protection";
            }
            else
            {
                // Casual A-Life: six goals
                // GetRich        — wants money above all else; greed-driven
                // FindLove       — looking for a real relationship; attachment + friendliness
                // StayFree       — independent spirit; doesn't want to be tied down
                // HaveFun        — lives in the moment; riskiness-driven
                // Survive        — just getting by day to day
                // FindProtection — nervous, looks for safety
                double wGetRich        = greed * 1.6 + confidence * 0.4;
                double wFindLove       = attachment * 1.8 + friendli * 0.8;
                double wStayFree       = (1.0 - attachment) * 1.2 + confidence * 0.5 + aggr * 0.3;
                double wHaveFun        = riskiness * 1.6 + friendli * 0.4;
                double wSurvive        = 0.8; // always in the pool
                double wFindProtection = (1.0 - aggr) * 1.0 + (1.0 - confidence) * 0.7;
                double tot = wGetRich + wFindLove + wStayFree + wHaveFun + wSurvive + wFindProtection;
                double r   = rng.NextDouble() * tot;
                if      (r < wGetRich)                                           d.LongTermGoal = "Get Rich";
                else if (r < wGetRich + wFindLove)                               d.LongTermGoal = "Find Love";
                else if (r < wGetRich + wFindLove + wStayFree)                   d.LongTermGoal = "Stay Free";
                else if (r < wGetRich + wFindLove + wStayFree + wHaveFun)        d.LongTermGoal = "Have Fun";
                else if (r < wGetRich + wFindLove + wStayFree + wHaveFun + wSurvive) d.LongTermGoal = "Survive";
                else                                                              d.LongTermGoal = "Seek Protection";
            }
        }

        // ================================================================
        //  LONG-TERM GOAL → MOOD HELPERS
        // ================================================================

        /// <summary>
        /// Intercept any mood assignment and optionally redirect it based on the ped's
        /// permanent life aspiration. Call this instead of assigning d.Mood directly
        /// whenever the event is goal-relevant (punishment, reward, session end, init).
        ///
        /// Rules:
        ///   GetRich       — bad events stay Annoyed (money is personal); good events → Happy briefly
        ///   FindLove      — bad events soften to Needy (she aches, not rages); good events → Playful
        ///   StayFree      — anything constraining → Annoyed; positive contact → Relaxed at most
        ///   HaveFun       — bad events soften to Guarded (nothing bothers her long); good → Playful
        private void SaveALife()
        {
            try
            {
                var lines = new System.Collections.Generic.List<string>();
                foreach (var kvp in _aLifePeds)
                {
                    lines.Add("[FP:" + kvp.Key + "]");
                    lines.Add("Name=" + kvp.Value.Name);
                                        lines.Add("Mode=" + (kvp.Value.ALifeMode != null ? kvp.Value.ALifeMode : "Normal"));
                    lines.Add("Voice=" + (kvp.Value.Voice != null ? kvp.Value.Voice : ""));
                    if (kvp.Value.ALifeMode == "Prostitute")
                    {
                        lines.Add("HookerAnim=" + (kvp.Value.HookerAnim != null ? kvp.Value.HookerAnim : ""));
                        if (kvp.Value.CivilianRejected) lines.Add("CivilianRejected=True");
                    }
                    lines.Add("Personality=" + kvp.Value.Personality);
                    lines.Add("Reputation=" + kvp.Value.Reputation);
                    lines.Add("Relationship=" + kvp.Value.Relationship);
                    lines.Add("LastPositiveRelationship=" + (kvp.Value.LastPositiveRelationship ?? ""));
                    lines.Add("HasMet=" + kvp.Value.HasMet);
                    lines.Add("HasMetSecondTime=" + kvp.Value.HasMetSecondTime);
                    lines.Add("NameKnown=" + kvp.Value.NameKnown);
                    if (kvp.Value.KnownTopics != 0) lines.Add("KnownTopics=" + kvp.Value.KnownTopics);
                    lines.Add("cimPreference=" + kvp.Value.CimPreference);
                    if (kvp.Value.ALifeMode != "Prostitute")
                    {
                        lines.Add("IsHooker=" + kvp.Value.IsHooker);
                        if (kvp.Value.WasHooker) lines.Add("WasHooker=True");
                        if (kvp.Value.HookerPermaReject) lines.Add("HookerPermaReject=True");
                        lines.Add("PrefBJ="    + (kvp.Value.PrefBJ.HasValue    ? kvp.Value.PrefBJ.Value.ToString()    : ""));
                        lines.Add("PrefRough=" + (kvp.Value.PrefRough.HasValue ? kvp.Value.PrefRough.Value.ToString() : ""));
                    }
                    if (kvp.Value.HookerPrices != null && kvp.Value.HookerPrices.Length == 5
                        && kvp.Value.ALifeMode != "Prostitute")
                        lines.Add("HookerPrices=" + string.Join(",", kvp.Value.HookerPrices));
                    lines.Add("Cash="       + kvp.Value.Cash);
                    lines.Add("Stored="     + kvp.Value.Stored);
                    if (kvp.Value.LongTermGoal != null && kvp.Value.LongTermGoal.Length > 0)
                        lines.Add("LongTermGoal=" + kvp.Value.LongTermGoal);
                    lines.Add("LastSimUtc=" + kvp.Value.LastSimUtc);
                    lines.Add("");
                }
                System.IO.File.WriteAllLines(ALIFE_FILE, lines);
            }
            catch { }
        }

        /// <summary>
        /// Checks whether a GF should initiate a passive breakup after a rep-damaging interaction.
        /// Hard breakup (rep &lt; 40, 40% chance) or soft friendzone (rep &lt; 60, 25% chance).
        /// Passes the already-resolved ped ref so no mid-method search is needed.
        /// Returns true if a breakup occurred — caller should return immediately.
        /// </summary>
        private bool TryGfPassiveBreakup(ALifePedData d, Ped pedRef)
        {
            if (d == null || d.Relationship != "Girlfriend") return false;
            string name = (d.NameKnown && d.Name != null && d.Name.Length > 0) ? d.Name : "?";
            string pers = d.Personality ?? "";
            bool broke  = false;

            // ── Hard breakup (rep < 40): 40% chance ───────────────────────────────────
            if (d.Reputation < 40 && rng.NextDouble() < 0.40)
            {
                string[] hL = GetGfHardBreakupLines(name, pers);
                ShowSubtitle(hL[rng.Next(hL.Length)], 5500);
                if (rng.NextDouble() < 0.50)
                {
                    d.Reputation   = -1;
                    d.Relationship = "Avoiding";
                }
                else
                {
                    if      (d.Reputation < 10) d.Relationship = "Stranger";
                    else if (d.Reputation < 30) d.Relationship = "Acquaintance";
                    else                        d.Relationship = "Familiar";
                }
                SaveALife();
                broke = true;
            }
            // ── Soft breakup (rep 40–59): 25% chance — friendzoned ────────────────────
            else if (d.Reputation < 60 && rng.NextDouble() < 0.25)
            {
                string[] sL = GetGfSoftBreakupLines(name, pers);
                ShowSubtitle(sL[rng.Next(sL.Length)], 5000);
                d.Relationship = "Friendzoned";
                SaveALife();
                broke = true;
            }

            if (broke)
            {
                FullReset();
                if (pedRef != null && pedRef.Exists()) pedRef.Task.WanderAround();
                menuLevel = MenuLevel.Main;
                menuIndex = 0;
            }
            return broke;
        }

        /// <summary>Dispatch to the correct relationship ladder based on ALifeMode.</summary>
        private void UpdateRelationshipState(ALifePedData d)
        {
            if (d == null) return;
            if (d.ALifeMode == "Prostitute") { UpdateProstRelationshipState(d); return; }
            UpdateCasualRelationshipState(d);
        }

        /// <summary>
        /// Prostitution A-Life relationship ladder: Hostile / Avoiding / Stranger / Regular / Obsessed.
        /// Obsessed is sticky once earned (rep must remain 70+).
        /// </summary>
        private void UpdateProstRelationshipState(ALifePedData d)
        {
            if (d.Reputation <= -2) { d.Relationship = "Hostile";  return; }
            if (d.Reputation == -1)
            {
                if (d.Relationship != "Avoiding")
                {
                    d.LastPositiveRelationship = d.Relationship;
                    d.Relationship = "Avoiding";
                }
                return;
            }
            // Recover from Avoiding: restore saved positive relationship
            if (d.Relationship == "Avoiding")
            {
                string saved = d.LastPositiveRelationship;
                d.Relationship = (saved != null && saved.Length > 0) ? saved : "Stranger";
            }
            // Obsessed is sticky once set
            if (d.Relationship == "Obsessed") return;
            // Threshold ladder
            if      (d.Reputation >= 95) d.Relationship = "Obsessed";
            else if (d.Reputation >= 50) d.Relationship = "Regular";
            else if (d.Reputation >= 10) d.Relationship = "Acquaintance";
            else                         d.Relationship = "Stranger";
        }

        private void UpdateCasualRelationshipState(ALifePedData d)
        {
            if (d == null) return;
            // Negative reputation forces base hostile states regardless of advanced status
            if (d.Reputation <= -2) { d.Relationship = "Hostile"; return; }
            if (d.Reputation == -1)
            {
                // Save the current positive state so we can restore it when rep recovers
                string cur2 = d.Relationship;
                bool isPositiveState = cur2 != "Avoiding" && cur2 != "Avoidant" && cur2 != "Hostile";
                if (isPositiveState && cur2 != null && cur2.Length > 0)
                    d.LastPositiveRelationship = cur2;
                if (cur2 != "Avoidant") d.Relationship = "Avoiding";
                return;
            }
            // Recover from Avoiding: restore whatever relationship she had before the rep dip
            string cur = d.Relationship;
            if (cur == "Avoiding")
            {
                string saved = d.LastPositiveRelationship;
                cur = (saved != null && saved.Length > 0) ? saved : "Stranger";
                d.Relationship = cur;
            }
            // Advanced states are sticky once set in positive territory
            bool isAdvanced = cur == "Friendzoned" || cur == "Flirty" || cur == "女朋友" || cur == "Obsessed";
            if (isAdvanced) return;
            // Base state threshold ladder
            if      (d.Reputation < 10) d.Relationship = "Stranger";
            else if (d.Reputation < 30) d.Relationship = "Acquaintance";
            else if (d.Reputation < 60) d.Relationship = "Familiar";
            else                        d.Relationship = "Friend";
        }


        /// <summary>
        /// If the current girl's reputation has fallen to -1 or below and no animation
        /// is currently running, force-end the session and release her to wander.
        /// When called mid-animation this is a no-op — EndAnimation() already contains
        /// a built-in wander check (rep &lt;= -1) that fires when the session naturally ends.
        /// Mood and facial expression must be applied by the caller before invoking this.
        /// </summary>
        private void CheckAndReleaseIfAvoiding(ALifePedData d)
        {
            if (d == null || d.Reputation > REP_MIN) return;
            if (girl == null || !girl.Exists()) return;

            // If an animation is in progress, let it finish. EndAnimation() will
            // detect rep <= -1 and wander her away at the natural end of the session.
            // Aborting mid-animation causes car anim to freeze on the proposal stage.
            if (state == ScriptState.Animating || animPhase > 0 || _finishExecutePending)
                return;

            // Not animating — safe to force-release immediately.
            Ped released = girl;
            FullReset(keepGirlTask: true);
            if (released != null && released.Exists() && released.IsAlive)
            {
                Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, released, false);
                released.Task.WanderAround();
            }
        }

        /// <summary>Assign a random hooker voice to non-hooker peds. Hookers keep their default voice.</summary>
        private void AssignHookerVoice(Ped ped)
        {
            if (ped == null || !ped.Exists()) return;

            int model = ped.Model.Hash;
            int h1 = Function.Call<int>(Hash.GET_HASH_KEY, "s_f_y_hooker_01");
            int h2 = Function.Call<int>(Hash.GET_HASH_KEY, "s_f_y_hooker_02");
            int h3 = Function.Call<int>(Hash.GET_HASH_KEY, "s_f_y_hooker_03");

            // Sandbox: native hooker models already have their own GTA voice — skip assignment.
            // A-Life: we still need to assign and save a voice to INI so it persists across sessions.
            if (!aLifeMode && (model == h1 || model == h2 || model == h3)) return;

            // Voice already resolved this session — just reapply it, no re-resolution.
            // Prevents mid-session calls (PlayMoaningSounds, PlayServiceVoice, PlayFinishSounds)
            // from re-running the fingerprint lookup, which could roll a new voice if GTA
            // has changed any component variation on the ped since hire time.
            if (_girlCurrentVoice != null)
            {
                Function.Call(Hash.SET_AMBIENT_VOICE_NAME, ped, _girlCurrentVoice);
                _voiceReapplyAt = Game.GameTime + 3000;
                return;
            }

            int handle = ped.Handle;

            string voice;
            if (aLifeMode)
            {
                // A-Life: fingerprint (INI) is authoritative — bypass the handle cache entirely.
                // GTA recycles ped handles; a stale voiceCache hit would return the WRONG voice
                // from a previously deleted unrelated ped that happened to share this handle number.
                string fp = GetPedFingerprint(ped);
                ALifePedData aData = null;
                if (fp != null) _aLifePeds.TryGetValue(ALifeKey(fp), out aData);

                if (aData != null && aData.Voice != null && aData.Voice.Length > 0)
                {
                    // Persisted voice from INI — always wins
                    voice = aData.Voice;
                }
                else
                {
                    // INI has no voice yet — roll a random one and save it.
                    voice = HOOKER_VOICES[rng.Next(HOOKER_VOICES.Length)];
                    if (fp != null)
                    {
                        if (aData == null)
                        {
                            if (!sandboxMode && aLifeMode && IsProstHookerModel(ped))
                                aData = GetOrCreateProstALifeData(ped);
                            else
                                aData = GetOrCreateALifeData(ped);
                        }
                        if (aData != null) { aData.Voice = voice; SaveALife(); }
                    }
                }

                // Sync the handle cache for consistency within this session
                voiceCache[handle] = voice;
            }
            else
            {
                // Sandbox: handle cache persists within the GTA session (static field survives
                // script reload). On cache miss (new ped or fresh session), roll a random voice.
                if (!voiceCache.TryGetValue(handle, out voice))
                {
                    voice = HOOKER_VOICES[rng.Next(HOOKER_VOICES.Length)];
                    voiceCache[handle] = voice;
                }
            }

            // Apply immediately — also stored for periodic reapplication in OnTick
            // (GTA resets ped ambient voices after animations/AI transitions)
            Function.Call(Hash.SET_AMBIENT_VOICE_NAME, ped, voice);
            _girlCurrentVoice = voice;
            _voiceReapplyAt   = Game.GameTime + 3000;
        }

        /// <summary>
        /// Apply a mood-matched facial idle animation override to a ped.
        /// Mood → GTA facial anim mapping (dict: facials@gen_female@base):
        ///   Relaxed → mood_normal_1 | Happy   → mood_happy_1
        ///   Guarded → mood_aiming_1 | Playful → mood_smug_1
        ///   Annoyed → mood_angry_1  | Needy   → mood_stressed_1
        ///   Jealous → mood_injured_1
        /// </summary>
        /// <summary>
        /// Force a downed ped's reputation to -2 (the hard hostile floor).
        /// Always sets exactly -2 regardless of current value, then saves.
        /// </summary>
        private void SetRepDownedPed(ALifePedData d)
        {
            if (d == null) return;
            d.Reputation = -2;
            UpdateRelationshipState(d);
            SaveALife();
        }

                private void ApplyMoodFacialExpression(Ped ped, string mood)
        {
            if (ped == null || !ped.Exists()) return;
            string animName = "mood_normal_1";
            if      (mood == "Annoyed")  animName = "mood_angry_1";
            else if (mood == "Alert")    animName = "mood_aiming_1";
            else if (mood == "Needy")    animName = "mood_injured_1";
            else if (mood == "Jealous")  animName = "mood_stressed_1";
            else if (mood == "Happy" || mood == "玩鬧") animName = "mood_happy_1";
            else animName = "mood_normal_1";
            Function.Call(Hash.SET_FACIAL_IDLE_ANIM_OVERRIDE, ped, animName, (string)null);
        }

        /// <summary>Play ambient speech on a ped with Director Mode audio flag for non-hooker compatibility.</summary>
        private void PlayPedSpeech(Ped ped, string speechName)
        {
            if (ped == null || !ped.Exists()) return;
            Function.Call(Hash.STOP_CURRENT_PLAYING_AMBIENT_SPEECH, ped);
            Function.Call(Hash.SET_AUDIO_FLAG, "DirectorModeActive", true);
            if (!string.IsNullOrEmpty(_girlCurrentVoice))
                Function.Call((Hash)0x3523634255FC3318L, ped, speechName, _girlCurrentVoice, "SPEECH_PARAMS_FORCE_SHOUTED", 0);
            else
                Function.Call(Hash.PLAY_PED_AMBIENT_SPEECH_NATIVE, ped, speechName, "SPEECH_PARAMS_FORCE_SHOUTED", 0);
            Function.Call(Hash.SET_AUDIO_FLAG, "DirectorModeActive", false);
        }

        /// <summary>Play ambient speech moaning sounds on the girl.</summary>
        private void PlayMoaningSounds()
        {
            if (!femaleMoanVoice) return;
            if (girl == null || !girl.Exists() || currentAction == "" || currentAction == "kissing") return;

            // Hostile/Avoiding (rep <= -1): she barely participates — 1% chance she makes any sound
            if (aLifeMode && _currentGirlFp != null)
            {
                ALifePedData dMoan;
                if (_aLifePeds.TryGetValue(_currentGirlKey, out dMoan) && dMoan.Reputation <= -1
                    && rng.NextDouble() >= 0.01)
                    return;
            }

            AssignHookerVoice(girl);
            string speechName = (currentAction == "blowjob") ? "SEX_ORAL" : "SEX_GENERIC";
            PlayPedSpeech(girl, speechName);
            if (currentAction == "blowjob") _lastGtaSpeechTime = Game.GameTime;
        }

        /// <summary>Play girl's service offer voice when opening services menu.</summary>
        private void PlayServiceVoice()
        {
            if (!femaleServiceVoice) return;
            if (girl == null || !girl.Exists()) return;

            AssignHookerVoice(girl);
            PlayPedSpeech(girl, "HOOKER_OFFER_SERVICE");
        }

        /// <summary>Play spit sound + brief mouth anim when player forces finish during BJ (A-Life only).</summary>
        private void PlayBjSpit()
        {
            if (bjSoundsEnabled && _bjSpitFile != null) PlayBjCustom(_bjSpitFile, true);
            if (girl == null || !girl.Exists()) return;
            Function.Call(Hash.REQUEST_ANIM_DICT, "mp_facial");
            if (Function.Call<bool>(Hash.HAS_ANIM_DICT_LOADED, "mp_facial"))
            {
                Function.Call(Hash.PLAY_FACIAL_ANIM, girl.Handle, "mic_chatter", "mp_facial");
                _bjSpitFacialStopAt = Game.GameTime + 500;
            }
        }

        /// <summary>Play girl's finish voice line (skips kissing).</summary>
        private void PlayFinishSounds()
        {
            if (currentAction == "" || currentAction == "kissing") return;
            if (girl == null || !girl.Exists()) return;

            // BJ finish: swallow logic.
            // Only swallow if _finishInside is true (player chose "Inside Her").
            // Forced or broke = always skip. Car mode plays swallow in outro, skip here.
            if (bjSoundsEnabled && currentAction == "blowjob" && mode != Mode.Car && !_streetSitPropActive)
            {
                bool skipSwallow = !_finishInside || _finishForced || _finishBroke;
                if (!skipSwallow) PlayBjCustom(_bjSwallowFile, true);
                else if (aLifeMode && (_finishForced || _finishBroke)) PlayBjSpit();
            }

            if (!femaleEndVoice) return;
            AssignHookerVoice(girl);
            // If she was violated (forced without consent, or player couldn't pay the quoted price),
            // skip the satisfied voice and instead set her mood to Annoyed.
            if (_finishForced || _finishBroke)
            {
                if (aLifeMode && _currentGirlFp != null)
                {
                    ALifePedData dMood;
                    if (_aLifePeds.TryGetValue(_currentGirlKey, out dMood))
                    {
                        dMood.Mood = "Annoyed";
                        ApplyMoodFacialExpression(girl, dMood.Mood);
                        SaveALife();
                    }
                }
                return; // no SEX_FINISHED speech
            }
            if (currentAction == "blowjob")
            {
                PlayPedSpeech(girl, "再次提供服務");
                return;
            }
            _postSexPhase = 1;
            PlayPedSpeech(girl, "互動完成");
        }

        /// <summary>
        /// Poll the post-sex voice sequence state machine. Called every tick from OnTick.
        /// Phase 1: wait for IS_AMBIENT_SPEECH_PLAYING to return false.
        /// Phase 2: wait 1 second, then play HOOKER_OFFER_AGAIN and open the deferred menu.
        /// </summary>
        private void TickPostSexVoice()
        {
            if (_postSexPhase == 0) return;

            if (_postSexPhase == 1)
            {
                bool speaking = girl != null && girl.Exists()
                    && Function.Call<bool>((Hash)0x9072C8B49907BFAD, girl);
                if (!speaking)
                {
                    _postSexWaitUntil = Game.GameTime + 500;
                    _postSexPhase = 2;
                }
                return;
            }

            if (_postSexPhase == 2)
            {
                if (Game.GameTime < _postSexWaitUntil) return;
                _postSexPhase = 0;
                if (girl != null && girl.Exists() && femaleEndVoice)
                {
                    AssignHookerVoice(girl);
                    PlayPedSpeech(girl, "再次提供服務");
                }
                menuLevel = _postSexPendingMenu;
                _postSexPendingMenu = MenuLevel.None;
            }
        }

        /// <summary>
        /// Play a custom BJ WAV file asynchronously from the pre-loaded volume-adjusted buffer.
        /// Skips if another custom sound is already playing, unless <paramref name="force"/> is true.
        /// </summary>
        private void PlayBjCustom(string path, bool force = false)
        {
            if (path == null) return;
            byte[] data;
            if (!_bjWavData.TryGetValue(path, out data)) return;
            if (!force && Game.GameTime < _bjSoundBusyUntil) return;
            try
            {
                if (_bjPlayer == null) _bjPlayer = new System.Media.SoundPlayer();
                _bjPlayer.Stop();
                _bjPlayer.Stream = new System.IO.MemoryStream(data);
                _bjPlayer.Play(); // async — never blocks the game thread
                int dur = _soundDurations.ContainsKey(path) ? _soundDurations[path] : 3000;
                _bjSoundBusyUntil = Game.GameTime + dur;
            }
            catch { }
        }

        /// <summary>Return the slap interval triplet (slow/mid/fast ms) for the current action.</summary>
        private void GetSlapIntervals(out int slow, out int mid, out int fast)
        {
            if (mode == Mode.Car)
            {
                if (_backseatCarSex)
                { slow = SLAP_LAY_COW_SLOW; mid = SLAP_LAY_COW_MID; fast = SLAP_LAY_COW_FAST; return; }
                if (vehicleAnimType == VehicleAnimType.Low)
                { slow = SLAP_LOW_SEX_SLOW; mid = SLAP_LOW_SEX_MID; fast = SLAP_LOW_SEX_FAST; return; }
                else
                { slow = SLAP_CAR_SEX_SLOW; mid = SLAP_CAR_SEX_MID; fast = SLAP_CAR_SEX_FAST; return; }
            }
            // Street — keyed by streetActionIdx
            switch (streetActionIdx)
            {
                case 5:  slow = SLAP_WALL_SEX_SLOW; mid = SLAP_WALL_SEX_MID; fast = SLAP_WALL_SEX_FAST; return; // Wall Sex — standalone
                case 6:  // Sitting Cowgirl
                case 7:  slow = SLAP_COW_SEX_SLOW;  mid = SLAP_COW_SEX_MID;  fast = SLAP_COW_SEX_FAST;  return; // Reverse Cowgirl
                case 8:  // Laying Cowgirl
                    if (_sinkraCowgirlAvailable && sinkraReplaceLaying)
                    { slow = SLAP_SINKRA_LAY_COW_SLOW; mid = SLAP_SINKRA_LAY_COW_MID; fast = SLAP_SINKRA_LAY_COW_FAST; return; }
                    slow = SLAP_LAY_COW_SLOW; mid = SLAP_LAY_COW_MID; fast = SLAP_LAY_COW_FAST; return;
                case 9:  // Laying Reversed Cowgirl
                    if (_sinkraCowgirlAvailable && sinkraReplaceRevLaying)
                    { slow = SLAP_SINKRA_LAY_COW_SLOW; mid = SLAP_SINKRA_LAY_COW_MID; fast = SLAP_SINKRA_LAY_COW_FAST; return; }
                    slow = SLAP_LAY_COW_SLOW; mid = SLAP_LAY_COW_MID; fast = SLAP_LAY_COW_FAST; return;
                case 10: slow = SLAP_DOGGY_SLOW;     mid = SLAP_DOGGY_MID;     fast = SLAP_DOGGY_FAST;     return; // Doggy Style
                case 11: slow = SLAP_RDOGGY_SLOW;    mid = SLAP_RDOGGY_MID;    fast = SLAP_RDOGGY_FAST;    return; // Rough Doggy Style
                default: slow = SLAP_CAR_SEX_SLOW;   mid = SLAP_CAR_SEX_MID;   fast = SLAP_CAR_SEX_FAST;   return;
            }
        }

        /// <summary>Play a sex slap WAV. No overlap guard — fires and forgets on its own player.</summary>
        private void PlaySlapSound(string path)
        {
            byte[] data;
            if (!_bjWavData.TryGetValue(path, out data)) return;
            try
            {
                if (_slapPlayer == null) _slapPlayer = new System.Media.SoundPlayer();
                _slapPlayer.Stop();
                _slapPlayer.Stream = new System.IO.MemoryStream(data);
                _slapPlayer.Play();
            }
            catch { }
        }

        /// <summary>Play a kissing WAV on its own dedicated player.</summary>
        private void PlayKissSound(string path)
        {
            byte[] data;
            if (!_bjWavData.TryGetValue(path, out data)) return;
            try
            {
                if (_kissPlayer == null) _kissPlayer = new System.Media.SoundPlayer();
                _kissPlayer.Stop();
                _kissPlayer.Stream = new System.IO.MemoryStream(data);
                _kissPlayer.Play();
            }
            catch { }
        }

        /// <summary>Scan the sounds folder, load and volume-adjust all BJ WAV files.</summary>
        private void LoadBjSounds()
        {
            try
            {
                if (!Directory.Exists(SOUNDS_DIR)) return;
                var sucks = new List<string>();
                for (int i = 1; i <= 10; i++)
                {
                    string f = Path.Combine(SOUNDS_DIR, "BJSuck_" + i + ".wav");
                    if (CacheBjSound(f)) sucks.Add(f);
                }
                _bjSuckFiles = sucks.Count > 0 ? sucks.ToArray() : null;

                string p;
                p = Path.Combine(SOUNDS_DIR, "BJThroat_Intro.wav"); if (CacheBjSound(p)) _bjThroatIntroFile = p;
                p = Path.Combine(SOUNDS_DIR, "BJThroat.wav");       if (CacheBjSound(p)) _bjThroatFile      = p;
                p = Path.Combine(SOUNDS_DIR, "BJSwallow.wav");      if (CacheBjSound(p)) _bjSwallowFile     = p;
                p = Path.Combine(SOUNDS_DIR, "BJSpit.wav");         if (CacheBjSound(p)) _bjSpitFile        = p;
                p = Path.Combine(SOUNDS_DIR, "BJPantsZip.wav");     if (CacheBjSound(p)) _bjPantsZipFile    = p;

                var slaps = new List<string>();
                for (int i = 1; i <= 11; i++)
                {
                    string f = Path.Combine(SOUNDS_DIR, "SexSlap_" + i + ".wav");
                    if (CacheBjSound(f)) slaps.Add(f);
                }
                _sexSlapFiles = slaps.Count > 0 ? slaps.ToArray() : null;

                var kisses = new List<string>();
                for (int i = 1; i <= 5; i++)
                {
                    string f = Path.Combine(SOUNDS_DIR, "Kissing_" + i + ".wav");
                    if (CacheBjSound(f)) kisses.Add(f);
                }
                _kissSoundFiles = kisses.Count > 0 ? kisses.ToArray() : null;
            }
            catch { }
        }

        /// <summary>Read a WAV file, apply <see cref="BJ_VOLUME"/> to the PCM samples, cache the
        /// result and its duration. Returns true if the file existed and was loaded.</summary>
        private bool CacheBjSound(string path)
        {
            if (!File.Exists(path)) return false;
            try
            {
                byte[] raw = File.ReadAllBytes(path);
                _bjWavData[path]      = AdjustWavVolume(raw, BJ_VOLUME);
                _soundDurations[path] = ReadWavDurationMs(path);
                return true;
            }
            catch { return false; }
        }

        /// <summary>Return a copy of a PCM WAV with all 16-bit samples scaled by <paramref name="vol"/>.</summary>
        private static byte[] AdjustWavVolume(byte[] wav, float vol)
        {
            byte[] result = (byte[])wav.Clone();
            int pos = 12; // skip RIFF(4) + file-size(4) + WAVE(4)
            while (pos + 8 <= result.Length)
            {
                string chunkId = System.Text.Encoding.ASCII.GetString(result, pos, 4);
                int chunkSize  = BitConverter.ToInt32(result, pos + 4);
                if (chunkId == "data")
                {
                    int dataStart = pos + 8;
                    int dataEnd   = Math.Min(dataStart + chunkSize, result.Length);
                    for (int i = dataStart; i + 1 < dataEnd; i += 2)
                    {
                        short s = BitConverter.ToInt16(result, i);
                        int   v = (int)(s * vol);
                        if (v >  32767) v =  32767;
                        if (v < -32768) v = -32768;
                        result[i]     = (byte)(v & 0xFF);
                        result[i + 1] = (byte)((v >> 8) & 0xFF);
                    }
                    break;
                }
                int aligned = chunkSize + (chunkSize % 2); // RIFF chunks are word-aligned
                pos += 8 + aligned;
            }
            return result;
        }

        /// <summary>Parse a WAV RIFF header to get exact playback duration in milliseconds.</summary>
        private static int ReadWavDurationMs(string path)
        {
            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var br = new BinaryReader(fs))
                {
                    byte[] id = br.ReadBytes(4);
                    if (id[0]!='R'||id[1]!='I'||id[2]!='F'||id[3]!='F') return 3000;
                    br.ReadInt32(); // file size
                    byte[] wv = br.ReadBytes(4);
                    if (wv[0]!='W'||wv[1]!='A'||wv[2]!='V'||wv[3]!='E') return 3000;

                    int sampleRate = 44100, channels = 1, bitsPerSample = 16;
                    long dataSize = 0;
                    while (fs.Position < fs.Length - 8)
                    {
                        string chunkId = new string(br.ReadChars(4));
                        int chunkSize  = br.ReadInt32();
                        if (chunkId == "fmt ")
                        {
                            br.ReadInt16(); // audio format
                            channels      = br.ReadInt16();
                            sampleRate    = br.ReadInt32();
                            br.ReadInt32(); // byte rate
                            br.ReadInt16(); // block align
                            bitsPerSample = br.ReadInt16();
                            if (chunkSize > 16) br.ReadBytes(chunkSize - 16);
                        }
                        else if (chunkId == "data")
                        {
                            dataSize = (uint)chunkSize;
                            break;
                        }
                        else
                        {
                            if (chunkSize > 0 && chunkSize < 10000000) br.ReadBytes(chunkSize);
                            else break;
                        }
                    }
                    if (dataSize <= 0 || sampleRate <= 0) return 3000;
                    double secs = (double)dataSize / (sampleRate * channels * (bitsPerSample / 8));
                    return (int)(secs * 1000) + 150; // +150 ms safety buffer
                }
            }
            catch { return 3000; }
        }

        /// <summary>Release the girl back to the world.</summary>
        private void ReleaseGirl()
        {
            if (girl != null && girl.Exists())
            {
                // Remove facial expression override entirely so GTA's natural death face plays if she later dies
                if (girl.IsAlive)
                    Function.Call((Hash)0x726256CC1EEB182F, girl); // CLEAR_FACIAL_IDLE_ANIM_OVERRIDE
                Function.Call(Hash.SET_PED_KEEP_TASK, girl, false);
                girl.Task.ClearAll();
                girl.IsPositionFrozen = false;
                Function.Call(Hash.SET_ENTITY_COLLISION, girl, true, true);
                girl.BlockPermanentEvents = false;
                // Give her a wander task so AI doesn't re-queue the old vehicle enter
                girl.Task.WanderAround();
                girl.IsPersistent = false;
                girl.MarkAsNoLongerNeeded();
            }
            girl = null;
            hasGirl = false;
        }

        /// <summary>Roll whether the NPC accepts the player's approach, based on personality and relationship.</summary>
        private bool RollApproachAcceptance(ALifePedData d, double clusterBonus = 0.0)
        {
            // Prostitution mode: always accepts unless she is already at the hostile floor.
            if (!sandboxMode)
            {
                if (d != null && d.Reputation <= REP_MIN) return false; // Hostile: guaranteed rejection
                return true;
            }

            // At -1 reputation: 99% fail — she's barely tolerating the player
            if (d != null && d.Reputation == -1)
                return rng.NextDouble() < 0.01;

            // Casual / A-Life: personality + relationship determine chance
            double chance;
            if (d == null)
            {
                chance = 0.50;
            }
            else
            {
                PersonalityProfile prof = GetProfile(d.Personality);
                double friendliness = (prof != null) ? prof.Friendliness : 0.50;
                // Base chance: friendly personality is more receptive (range 0.30–0.80)
                chance = 0.30 + friendliness * 0.50;
                // +5% per 10 reputation points above 0
                if (d.Reputation > 0)
                    chance += (d.Reputation / 10) * 0.05;
                // -15% per point below 0 (she remembers the bad history)
                if (d.Reputation < 0)
                    chance -= Math.Abs(d.Reputation) * 0.15;
            }
            // Long-term goal modifier on approach acceptance
            if (d != null && d.LongTermGoal != null)
            {
                switch (d.LongTermGoal)
                {
                    case "尋找愛情":        chance += 0.15; break; // she's looking — more open to strangers
                    case "享受樂趣":         chance += 0.12; break; // easy-going, usually up for it
                    case "尋找保護":  chance += 0.10; break; // wants someone around
                    case "保持自由":        chance -= 0.18; break; // doesn't want to be tied down
                    case "GetOut":          chance -= 0.10; break; // already done with this life
                    // GetRich, Survive — neutral on first contact, money decides everything else
                }
            }
            // Cluster match bonus/penalty: player guessed her personality right (+0.25) or wrong (-0.20)
            chance += clusterBonus;
            chance = Math.Max(0.05, Math.Min(0.95, chance));
            return rng.NextDouble() < chance;
        }

        /// <summary>Return the display name for an NPC: her real name when NameKnown, otherwise "?".</summary>
        private string DisplayName(ALifePedData d)
        {
            return (d != null && d.NameKnown && d.Name != null && d.Name.Length > 0) ? d.Name : "?";
        }

        /// <summary>
        /// Show a character dialogue subtitle with colour-coded name.
        /// Name is green on a positive response, red on a negative one.
        /// Strips internal colour tags and quotes from the response string automatically.
        /// </summary>
        private void ShowCharacterSubtitle(string name, string response, bool positive, int duration = 4500)
        {
            string display = (response != null) ? response.Replace("~r~", "").Replace("~g~", "").Replace("\"", "") : "";
            ShowSubtitle((positive ? "~g~" : "~r~") + name + ":~w~ " + display, duration);
        }

        /// <summary>True when the NPC's Friendliness is above 0.5 (warm-natured personality).</summary>
        private bool IsWarmPersonality(ALifePedData d)
        {
            if (d == null) return rng.Next(2) == 0;
            PersonalityProfile prof = GetProfile(d.Personality);
            return prof == null || prof.Friendliness > 0.50;
        }

        /// <summary>True between 21:00 and 02:59 (GTA clock). Single source of truth for all night/day checks.</summary>
        private bool IsNight()
        {
            int h = Function.Call<int>(Hash.GET_CLOCK_HOURS);
            return h >= 21 || h < 3;
        }

        /// <summary>Show a reactive NPC acceptance line tied to the specific pickup line the player used.</summary>
        private void ShowApproachLineAcceptResponse(ALifePedData d, int cluster, int lineIndex, bool firstEncounter)
        {
            string name = DisplayName(d);
            bool warm = IsWarmPersonality(d);
            string[] opts = GetApproachLineResponseOptions(cluster, lineIndex, firstEncounter, warm, true);
            ShowSubtitle("~g~" + name + ":~s~ " + opts[rng.Next(opts.Length)], 3500);
        }

        /// <summary>Show a reactive NPC rejection line tied to the specific pickup line the player used.</summary>
        private void ShowApproachLineRejectResponse(ALifePedData d, int cluster, int lineIndex, bool firstEncounter)
        {
            string name = DisplayName(d);
            bool warm = IsWarmPersonality(d);
            string[] opts = GetApproachLineResponseOptions(cluster, lineIndex, firstEncounter, warm, false);
            ShowSubtitle("~r~" + name + ":~s~ " + opts[rng.Next(opts.Length)], 3500);
        }

        /// <summary>
        /// Civilian (non-hooker model) reacts to being approached as if she were a sex worker.
        /// Cluster-aware so the phrasing ties back to the player's chosen style.
        /// </summary>
        private void ShowCivilianNotHookerRejectLine(ALifePedData d, int cluster)
        {
            string name = DisplayName(d);
            string p = (d != null && d.Personality != null) ? d.Personality : "";
            string[] lines = GetCivilianNotHookerRejectLines(name, p, cluster);
            ShowSubtitle(lines[rng.Next(lines.Length)], 4500);
        }

        /// <summary>Show an NPC acceptance subtitle based on personality and game mode.</summary>
        private void ShowApproachAcceptSubtitle(ALifePedData d)
        {
            string name = DisplayName(d);
            string response;
            if (sandboxMode)
            {
                string p = (d != null && d.Personality != null) ? d.Personality : "";
                string[] opts = GetSandboxAcceptLines(name, p);
                response = opts[rng.Next(opts.Length)];
            }
            else
            {
                // Prost A-Life: cluster + line -aware accept response
                // d.HasMet is already true at this point; HasMetSecondTime flags returning encounters
                if (d != null && d.Reputation <= -1)
                {
                    // She doesn't like us — still agrees to work but makes that very clear.
                    // Personality-driven: cluster shapes HOW she's hostile, warm/cold affects the edge.
                    bool warmHo = IsWarmPersonality(d);
                    string persHo = (d != null && d.Personality != null) ? d.Personality : "";
                    string[] hostile = GetProstHostileAcceptResponse(_approachSelectedCluster, warmHo, persHo);
                    response = "~r~" + name + ":~s~ " + hostile[rng.Next(hostile.Length)];
                }
                else
                {
                    bool firstEnc = d == null || !d.HasMetSecondTime;
                    bool warm = IsWarmPersonality(d);
                    string persAcc = (d != null && d.Personality != null) ? d.Personality : "";
                    string[] opts = GetProstApproachLineResponse(_approachSelectedCluster, _lastApproachLineIdx, firstEnc, warm, persAcc);
                    response = "~g~" + name + ":~s~ " + opts[rng.Next(opts.Length)];
                }
                _approachSelectedCluster = -1; // consumed
                _lastApproachLineIdx = -1; // consumed
            }
            ShowSubtitle(response, 3000);
        }

        private bool IsSeatValidForVehicle(Vehicle car, VehicleSeat seat)
        {
            if (car == null || !car.Exists()) return false;
            int maxPass = Function.Call<int>(Hash.GET_VEHICLE_MAX_NUMBER_OF_PASSENGERS, car);
            int sIdx = (int)seat;
            if (sIdx == -1) return true; // Driver seat is always valid
            return sIdx >= 0 && sIdx < maxPass;
        }

        /// <summary>
        /// 為女性 NPC 計算最適當的入座位置。
        /// 玩家固定坐在駕駛座，NPC 優先坐 preferredSeat（一般為副駕）。
        /// 僅在該座位實體存在且無其他人（或已有女性自己）時才選擇。
        /// </summary>
        private VehicleSeat GetBestSeatForGirl(Vehicle car, VehicleSeat preferredSeat = VehicleSeat.Passenger)
        {
            if (car == null || !car.Exists()) return preferredSeat;

            // 依優先順序嘗試：preferredSeat -> 副駕 -> 左後座 -> 右後座
            VehicleSeat[] candidates;
            if (preferredSeat == VehicleSeat.LeftRear)
                candidates = new VehicleSeat[] { VehicleSeat.LeftRear, VehicleSeat.RightRear, VehicleSeat.Passenger };
            else
                candidates = new VehicleSeat[] { VehicleSeat.Passenger, VehicleSeat.LeftRear, VehicleSeat.RightRear };

            foreach (VehicleSeat s in candidates)
            {
                if (!IsSeatValidForVehicle(car, s)) continue; // 跳過該車型不存在的座位
                Ped occ = car.GetPedOnSeat(s);
                if (occ == null || !occ.Exists() || occ == girl)
                    return s;
            }

            return (VehicleSeat)(-2); // 全滿或無可用空位
        }

        private void TaskGirlEnterVehicle(Vehicle car, VehicleSeat preferredSeat = VehicleSeat.Passenger)
        {
            if (!hasGirl || girl == null || !girl.Exists() || girl.IsDead) return;
            if (car == null || !car.Exists() || car.IsDead) return;

            // 自動解鎖車門
            if (Function.Call<int>(Hash.GET_VEHICLE_DOOR_LOCK_STATUS, car) > 1)
            {
                Function.Call(Hash.SET_VEHICLE_DOORS_LOCKED, car, 1);
            }

            // 智慧座位選擇：副駕被佔用時自動轉移至後座（若有）
            VehicleSeat targetSeat = GetBestSeatForGirl(car, preferredSeat);
            if ((int)targetSeat == -2)
            {
                ShowHudStatus("~r~載具已滿！", 2000);
                return;
            }

            Ped occupant = car.GetPedOnSeat(targetSeat);
            if (occupant != null && occupant.Exists() && occupant != girl)
            {
                ShowHudStatus("~r~該座位已被佔用！", 2000);
                return; // 避免強行搶座把人拉下車
            }

            girl.BlockPermanentEvents = true;
            // Flag 16 (0x10 = ECF_DONT_JACK_ANYONE) 禁止拉下座上乘客
            Function.Call(Hash.TASK_ENTER_VEHICLE, girl, car, -1, (int)targetSeat, 2.0f, 16, 0);
            _vehEntryStartTime = Game.GameTime;
        }

        /// <summary>
        /// Make a ped flee or fight the player depending on her Aggressiveness.
        /// Used both from OpenApproachMenu (already -2) and from rejection handlers (just pushed to -2).
        /// fleeChance: 100% at Agg=0.0 → 40% at Agg=1.0.
        /// </summary>
        /// <summary>Show a personality-matched "leave me alone" line when reputation drops to -1 (Avoiding).</summary>
        private void ShowAvoidingReactionSubtitle(ALifePedData d)
        {
            string name = DisplayName(d);
            string p = (d != null && d.Personality != null) ? d.Personality : "";
            string[] lines = GetAvoidingReactionLines(name, p);
            ShowSubtitle(lines[rng.Next(lines.Length)], 4000);
        }

        private void TriggerHostileReaction(Ped ped, ALifePedData d)
        {
            if (ped == null || !ped.Exists()) return;
            string name = DisplayName(d);
            PersonalityProfile prof = (d != null) ? GetProfile(d.Personality) : null;
            double agg = (prof != null) ? prof.Aggressiveness : 0.30;
            bool flee = rng.NextDouble() < (1.0 - agg * 0.60);
            if (flee)
            {
                if (d != null) { d.Relationship = "Avoidant"; SaveALife(); }
                ShowSubtitle("~r~" + name + " ~s~avoids you.", 3000);
                Function.Call(Hash.TASK_SMART_FLEE_PED, ped, Game.Player.Character, 150f, -1, false, false);
            }
            else
            {
                if (d != null) { d.Relationship = "Hostile"; SaveALife(); }
                ShowSubtitle("~r~" + name + " ~s~isn't taking it anymore!", 3000);
                ped.BlockPermanentEvents = false;
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 2,  true);
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 5,  true);
                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 46, true);
                Function.Call(Hash.SET_PED_COMBAT_MOVEMENT,   ped, 2);
                Function.Call(Hash.SET_PED_COMBAT_ABILITY,    ped, 100);
                Function.Call(Hash.GIVE_WEAPON_TO_PED, ped, unchecked((int)0xA2719263), 0, true, true);
                Function.Call(Hash.SET_CURRENT_PED_WEAPON,    ped, unchecked((int)0xA2719263), true);
                Function.Call(Hash.TASK_COMBAT_PED, ped, Game.Player.Character, 0, 16);
            }
        }

        /// <summary>Show a personality-matched goodbye reaction when the player chooses to leave.</summary>
        private void ShowLeaveReactionSubtitle(ALifePedData d, int leaveCluster, int leaveLineIdx = 0)
        {
            if (d == null) return;
            string name = DisplayName(d);
            string p = d.Personality ?? "";

            // ── Prostitution A-Life: relationship-based reactions ─────────────────
            if (d.ALifeMode == "Prostitute")
            {
                string[] prostOpts = GetProstLeaveReactionLines(name, d.Relationship, leaveCluster);
                ShowSubtitle(prostOpts[rng.Next(prostOpts.Length)], 4000);
                return;
            }

            string[] clusterNames = { "溫柔", "自信", "高冷", "奉承", "玩鬧" };
            string picked   = (leaveCluster >= 0 && leaveCluster < clusterNames.Length) ? clusterNames[leaveCluster] : null;
            string expected = GetApproachCluster(d.Personality);
            bool matched    = picked != null && expected != null && picked == expected;

            // ── "I love you" leave (Flattering line 3) ─────────────────────────────────
            if (leaveCluster == 3 && leaveLineIdx == 3)
            {
                string[] ilyOpts = d.Relationship == "Girlfriend"
                    ? GetGfILYResponseLines(name, p)
                    : GetILYRejectionLines(name, p);
                ShowSubtitle(ilyOpts[rng.Next(ilyOpts.Length)], 5000);
                return;
            }

            string[] leaveOpts = GetLeaveReactionLines(name, p, matched);
            ShowSubtitle(leaveOpts[rng.Next(leaveOpts.Length)], 4000);
        }

        /// <summary>Return the item count for a given conversation sub-branch.</summary>
        private int GetConvSubItemCount(int branch)
        {
            // Prostitution A-Life: every branch has exactly 4 items
            ALifePedData dSubCount = null;
            if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out dSubCount);
            if (dSubCount != null && dSubCount.ALifeMode == "Prostitute") return 4;
            switch (branch)
            {
                case 1: return 6; // Small Talk (6 items)
                case 2: return 5; // Check Mood (5 items)
                case 3: return 8; // Flirt (8 items: 7 standard + "I love you")
                case 6: return 1; // Business (1 dynamic item: propose / end / re-propose)
                default: return 7; // all others (7 items)
            }
        }

        /// <summary>
        /// Returns true when A-Life casual mode should block automatic vehicle entry this session.
        /// She needs an explicit invite unless she is in an auto-enter relationship tier
        /// (Friendzoned / Girlfriend / Obsessed) or has already accepted an invite.
        /// </summary>
        private bool IsALifeVehicleEntryGated()
        {
            if (!aLifeMode || !sandboxMode) return false;
            if (_invitedToVehicle) return false;
            ALifePedData dGate;
            if (_currentGirlKey == null || !_aLifePeds.TryGetValue(_currentGirlKey, out dGate)) return false;
            if (dGate.IsHooker) return false; // exclusive hooker always follows
            string relGate = dGate.Relationship;
            if (relGate == "Friendzoned" || relGate == "女朋友" || relGate == "Obsessed") return false;
            return true;
        }

        /// <summary>
        /// Returns true when the "邀請上車" item should appear in the Actions menu.
        /// Conditions: A-Life + sandbox mode, has girl, player is in a vehicle, not yet invited this
        /// session, and her relationship is not one of the auto-enter tiers (Friendzoned/Girlfriend/Obsessed).
        /// </summary>
        private bool ShowInviteVehicleItem()
        {
            if (!hasGirl || girl == null || !girl.Exists()) return false;
            if (_invitedToVehicle) return false;

            Ped plSiv = Game.Player.Character;
            Vehicle targetCar = plSiv.IsInVehicle() ? plSiv.CurrentVehicle : FindBackseatVehicle();
            if (targetCar == null || !targetCar.Exists()) return false;

            if (aLifeMode && sandboxMode)
            {
                ALifePedData dSiv;
                if (_currentGirlKey == null || !_aLifePeds.TryGetValue(_currentGirlKey, out dSiv)) return false;
                if (dSiv.IsHooker) return false; // exclusive hooker auto-follows, no invite needed
                string relSiv = dSiv.Relationship;
                if (relSiv == "Friendzoned" || relSiv == "女朋友" || relSiv == "Obsessed") return false;
            }

            // If she's already in the player's vehicle, treat as invited — no point showing the item
            if (girl.IsInVehicle() && plSiv.IsInVehicle() && girl.CurrentVehicle == plSiv.CurrentVehicle)
            { _invitedToVehicle = true; return false; }
            return true;
        }

        /// <summary>傳回 true when both player and girl are in the same vehicle.</summary>
        private bool BothInSameVehicle()
        {
            if (!hasGirl || girl == null || !girl.Exists()) return false;
            Ped pl = Game.Player.Character;
            return pl.IsInVehicle() && girl.IsInVehicle() && girl.CurrentVehicle == pl.CurrentVehicle;
        }

        /// <summary>傳回 true when the "Move to back/front seat" item should appear in the Actions menu.</summary>
        private bool ShowSeatSwapInActions()
        {
            return FindBackseatVehicle() != null;
        }



        private bool IsPlayerInBackSeat(Vehicle car)
        {
            if (car == null || !car.Exists()) return false;
            Ped pl = Game.Player.Character;
            if (pl == null || !pl.Exists() || !pl.IsInVehicle()) return false;
            int maxPass = Function.Call<int>(Hash.GET_VEHICLE_MAX_NUMBER_OF_PASSENGERS, car);
            if (maxPass < 2) return false; // 2-seater cars have no backseat
            for (int s = 1; s <= maxPass; s++)
            {
                if (car.GetPedOnSeat((VehicleSeat)s) == pl)
                    return true;
            }
            return false;
        }

        private bool IsBackseatCarMode()
        {
            if (mode != Mode.Car) return false;
            Ped pl = Game.Player.Character;
            if (!pl.IsInVehicle() || pl.CurrentVehicle == null) return false;
            return IsPlayerInBackSeat(pl.CurrentVehicle);
        }

        private Vehicle FindBackseatVehicle()
        {
            Ped pl = Game.Player.Character;
            if (pl.IsInVehicle())
            {
                Vehicle v = pl.CurrentVehicle;
                if (v != null && IsSexSuitableVehicle(v) && Function.Call<int>(Hash.GET_VEHICLE_MAX_NUMBER_OF_PASSENGERS, v) >= 1
                    && !backseatBlacklist.Contains(v.Model.Hash))
                    return v;
                return null;
            }
            // Scan for closest suitable vehicle within 10m
            Vector3 pos = pl.Position;
            Vehicle closest = null;
            float closestDist = 10f;
            foreach (Vehicle v in World.GetNearbyVehicles(pl, closestDist))
            {
                if (v == null || !v.Exists() || v.IsDead) continue;
                if (!IsSexSuitableVehicle(v)) continue;
                if (Function.Call<int>(Hash.GET_VEHICLE_MAX_NUMBER_OF_PASSENGERS, v) < 1) continue;
                if (backseatBlacklist.Contains(v.Model.Hash)) continue;
                float d = v.Position.DistanceTo(pos);
                if (d < closestDist)
                {
                    closestDist = d;
                    closest = v;
                }
            }
            return closest;
        }

        /// <summary>Return the correct menu label for the seat entry/swap item based on current state.</summary>
        private string GetSeatItemLabel()
        {
            Ped pl = Game.Player.Character;
            if (pl.IsInVehicle())
            {
                Vehicle car = pl.CurrentVehicle;
                bool inBack = car != null && IsPlayerInBackSeat(car);
                return inBack ? "換到前座" : "換到後座";
            }
            return "從後座上車";
        }

        /// <summary>Swap player and girl between front and back seats (blackscreen),
        /// or task them to walk into a nearby vehicle from outside (animated).</summary>
        private void TrySwapSeats()
        {
            if (!hasGirl || girl == null || !girl.Exists())
            {
                ShowHudStatus("~r~未找到目標！", 2000);
                return;
            }
            Ped pl = Game.Player.Character;
            bool bothInside = BothInSameVehicle();
            Vehicle car;
            if (bothInside)
            {
                car = pl.CurrentVehicle;
            }
            else
            {
                car = FindBackseatVehicle();
                if (car == null)
                {
                    ShowHudStatus("~r~附近沒有合適的載具！", 2000);
                    return;
                }
            }

            if (car == null || !IsSexSuitableVehicle(car))
            {
                ShowHudStatus("~r~該載具不支援此功能！", 2000);
                return;
            }

            int maxPass = Function.Call<int>(Hash.GET_VEHICLE_MAX_NUMBER_OF_PASSENGERS, car);
            if (maxPass < 1)
            {
                ShowHudStatus("~r~該載具無法乘坐！", 2000);
                return;
            }

            if (bothInside)
            {
                // ── Already in the vehicle → blackscreen swap ──
                bool goToBack = !IsPlayerInBackSeat(car);

                menuLevel = MenuLevel.None;
                Function.Call(Hash.DO_SCREEN_FADE_OUT, 500);
                Wait(600);

                if (goToBack && maxPass >= 2)
                {
                    Function.Call(Hash.SET_PED_INTO_VEHICLE, pl, car, 2);   // right rear
                    Function.Call(Hash.SET_PED_INTO_VEHICLE, girl, car, 1); // left rear
                }
                else
                {
                    Function.Call(Hash.SET_PED_INTO_VEHICLE, pl, car, -1);  // driver
                    Function.Call(Hash.SET_PED_INTO_VEHICLE, girl, car, 0); // passenger
                }

                Function.Call(Hash.DO_SCREEN_FADE_IN, 500);
                Wait(600);

                mode = Mode.Car;
                menuLevel = MenuLevel.Services;
                menuIndex = 0;
            }
            else
            {
                // ── Outside the vehicle → animated entry ──
                _backseatEntryPending = true; // suppress auto passenger-seat entry in HandleIdleState

                if (maxPass >= 2)
                {
                    // Task girl to enter left rear (seat 1)
                    TaskGirlEnterVehicle(car, VehicleSeat.LeftRear);
                    // Task player to enter right rear (seat 2)
                    Function.Call(Hash.TASK_ENTER_VEHICLE, pl, car, -1, 2, 2.0f, 16, 0);
                }
                else
                {
                    // Task girl to enter passenger (seat 0)
                    TaskGirlEnterVehicle(car, VehicleSeat.Passenger);
                    // Task player to enter driver (seat -1)
                    Function.Call(Hash.TASK_ENTER_VEHICLE, pl, car, -1, -1, 2.0f, 16, 0);
                }
            }
        }

        /// <summary>傳回 true if the street-mode service at index idx is a blowjob variant.</summary>
        private bool IsEscalateBJItem(bool isCar, int idx)
        {
            if (isCar)
            {
                // Car: index 0 = Blowjob
                return idx == 0;
            }
            else
            {
                // Street: Standing BJ (2), Sitting BJ (3), Laying BJ (4)
                return idx == 2 || idx == 3 || idx == 4;
            }
        }

        private bool IsEscalateRoughItem(bool isCar, int idx)
        {
            if (isCar) return false; // no rough car acts
            // Street: Rough Doggy Style (11)
            return idx == 11;
        }

        /// <summary>Roll whether the NPC accepts the player's request to finish inside.</summary>
        private bool RollFinishInsideAcceptance(ALifePedData d)
        {
            if (!sandboxMode)
            {
                // Prost A-Life: relationship-based. She's more willing the better she knows you.
                double prostChance;
                string rel = (d != null) ? d.Relationship : "Stranger";
                switch (rel)
                {
                    case "Obsessed":     return true;         // 100%
                    case "Regular":      prostChance = 0.85; break;
                    case "Flirty":       prostChance = 0.70; break;
                    case "友好":     prostChance = 0.55; break;
                    case "Familiar":     prostChance = 0.35; break;
                    case "Acquaintance": prostChance = 0.20; break;
                    default:             prostChance = 0.10; break; // Stranger
                }
                return rng.NextDouble() < prostChance;
            }
            // Casual A-Life: Friendliness + Riskiness + relationship
            double chance;
            if (d == null)
            {
                chance = 0.30;
            }
            else
            {
                PersonalityProfile prof = GetProfile(d.Personality);
                double friendliness = (prof != null) ? prof.Friendliness : 0.50;
                double riskiness    = (prof != null) ? prof.Riskiness    : 0.40;
                // Base: willingness driven by how warm and adventurous she is
                chance = friendliness * 0.60 + riskiness * 0.20;
                if (d.Reputation > 0)
                    chance += (d.Reputation / 10) * 0.05;
            }
            chance = Math.Max(0.05, Math.Min(0.95, chance));
            return rng.NextDouble() < chance;
        }

        /// <summary>
        /// Roll whether an NPC has the cimPreference trait at creation time.
        /// Casual and Prost A-Life share the same personality table — higher riskiness/friendliness = higher chance.
        /// Prost hookers generally have a slightly lower base chance (it's a job, not a personal preference).
        /// </summary>
        private bool RollCimPreference(string personality, string mode)
        {
            double chance;
            bool isProst = (mode == "交易");
            switch (personality)
            {
                // High chance — adventurous / expressive types
                case "Chaotic":     chance = isProst ? 0.65 : 0.75; break;
                case "Party Girl":  chance = isProst ? 0.60 : 0.72; break;
                case "玩鬧":     chance = isProst ? 0.55 : 0.68; break;
                case "Flirty":      chance = isProst ? 0.55 : 0.65; break;
                case "Unstable":    chance = isProst ? 0.50 : 0.62; break;

                // Medium-high chance — warm / relationship-driven types
                case "Needy":       chance = isProst ? 0.45 : 0.58; break;
                case "Romantic":    chance = isProst ? 0.40 : 0.55; break;
                case "Sweet":       chance = isProst ? 0.38 : 0.52; break;
                case "Jealous":     chance = isProst ? 0.38 : 0.50; break;

                // Medium chance — neutral / self-interested types
                case "Dominant":    chance = isProst ? 0.35 : 0.45; break;
                case "Manipulative":chance = isProst ? 0.32 : 0.42; break;
                case "Street Smart":chance = isProst ? 0.30 : 0.40; break;
                case "Aggressive":  chance = isProst ? 0.28 : 0.38; break;
                case "Sarcastic":   chance = isProst ? 0.25 : 0.35; break;

                // Low chance — guarded / reserved / greedy types
                case "Gold Digger": chance = isProst ? 0.22 : 0.30; break;
                case "Classy":      chance = isProst ? 0.20 : 0.28; break;
                case "Mysterious":  chance = isProst ? 0.18 : 0.26; break;
                case "Independent": chance = isProst ? 0.15 : 0.22; break;
                case "Familiar":    chance = isProst ? 0.12 : 0.20; break;
                case "Cold":        chance = isProst ? 0.10 : 0.18; break;
                case "Shy":         chance = isProst ? 0.08 : 0.14; break;

                default:            chance = isProst ? 0.20 : 0.30; break;
            }
            return rng.NextDouble() < chance;
        }

        /// <summary>
        /// Show a positive unasked reaction for NPCs with CimPreference=true.
        /// Called when the player finishes inside without asking — she's into it.
        /// Per-personality intimate lines.
        /// </summary>
        private void ShowCimPreferenceUnaskedSubtitle(ALifePedData d)
        {
            string name = DisplayName(d);
            string p = (d != null && d.Personality != null) ? d.Personality : "";
            string[] opts = GetCimPreferenceUnaskedLines(name, p);
            ShowSubtitle(opts[rng.Next(opts.Length)], 3500);
        }

        /// <summary>
        /// Roll whether a Casual exclusive hooker waives her finish-inside fee (free-pass).
        /// If true, no price is set and standard acceptance dialogue is used instead.
        /// </summary>
        private bool RollHookerFreePass(ALifePedData d)
        {
            string rel = (d != null) ? d.Relationship : "Stranger";
            double freeChance;
            switch (rel)
            {
                case "Flirty":       freeChance = 0.50; break;
                case "友好":     freeChance = 0.45; break;
                case "Familiar":     freeChance = 0.25; break;
                case "Acquaintance": freeChance = 0.10; break;
                default:             freeChance = 0.01; break; // Stranger
            }
            return rng.NextDouble() < freeChance;
        }

        /// <summary>
        /// Roll whether a Prost A-Life hooker waives her finish-inside fee (free-pass).
        /// Obsessed is nearly always free; lower relationships are very unlikely.
        /// </summary>
        private bool RollProstFreePass(ALifePedData d)
        {
            string rel = (d != null) ? d.Relationship : "Stranger";
            double freeChance;
            switch (rel)
            {
                case "Obsessed":     freeChance = 0.99; break;
                case "Flirty":       freeChance = 0.15; break;
                case "Regular":      freeChance = 0.10; break;
                case "友好":     freeChance = 0.07; break;
                case "Familiar":     freeChance = 0.05; break;
                case "Acquaintance": freeChance = 0.02; break;
                default:             freeChance = 0.01; break; // Stranger
            }
            return rng.NextDouble() < freeChance;
        }

        /// <summary>Show her acceptance dialogue when asked to finish inside (casual A-Life, non-hooker).</summary>
        private void ShowFinishInsideAcceptSubtitle(ALifePedData d)
        {
            string name = DisplayName(d);
            string p = (d != null && d.Personality != null) ? d.Personality : "";
            string[] opts = GetFinishInsideAcceptLines(name, p);
            ShowSubtitle(opts[rng.Next(opts.Length)], 3000);
        }

        /// <summary>Show her rejection dialogue when asked to finish inside.</summary>
        private void ShowFinishInsideDeclineSubtitle(ALifePedData d)
        {
            string name = DisplayName(d);
            string response;
            if (!sandboxMode)
            {
                string rel  = (d != null) ? d.Relationship : "Stranger";
                string pers = (d != null && d.Personality != null) ? d.Personality : "";
                string[] opts = GetFinishInsideDeclineLines(name, rel, pers);
                response = opts[rng.Next(opts.Length)];
            }
            else
            {
                string[] askNoLines = GetFinishAskNoLines(name, (d != null && d.Personality != null) ? d.Personality : "");
                response = askNoLines[rng.Next(askNoLines.Length)];
            }
            ShowSubtitle(response, 3000);
        }

        /// <summary>Show her angry reaction when player finishes inside without asking.</summary>
        private void ShowFinishForcedSubtitle(ALifePedData d)
        {
            string name = DisplayName(d);
            string response;

            if (!sandboxMode)
            {
                // Prostitution mode: reaction driven by relationship + personality
                string rel  = (d != null) ? d.Relationship : "Stranger";
                string pers = (d != null && d.Personality != null) ? d.Personality : "";
                string[] opts = GetFinishForcedProstLines(name, rel, pers);
                response = opts[rng.Next(opts.Length)];

                // Relationship damage: chance of going Hostile varies by how well she knows you
                double hostileChance = 0.0;
                if      (rel == "Stranger")     hostileChance = 0.99;
                else if (rel == "Acquaintance") hostileChance = 0.70;
                else if (rel == "Regular")      hostileChance = 0.50;
                else if (rel == "Obsessed")     hostileChance = 0.01;

                if (d != null && hostileChance > 0.0 && rng.NextDouble() < hostileChance)
                {
                    d.Reputation = -2;
                    UpdateProstRelationshipState(d);
                    d.Mood = "Annoyed";
                    SaveALife();
                    // Always apply the mood expression immediately as visual feedback.
                    if (girl != null && girl.Exists())
                        ApplyMoodFacialExpression(girl, d.Mood);
                    // If not animating, force-release now. If animating, EndAnimation()
                    // detects rep <= -1 and wanders her when the session naturally ends.
                    CheckAndReleaseIfAvoiding(d);
                }
            }
            else
            {
                // Casual/date mode: reaction driven by personality
                string[] forcedLines = GetFinishForcedLines(name, (d != null && d.Personality != null) ? d.Personality : "");
                response = forcedLines[rng.Next(forcedLines.Length)];
            }
            ShowSubtitle(response, 3500);
        }

        /// <summary>
        /// Calculates the device intensity for the current act and anim speed.
        /// At 1x anim speed returns the cfg value (the sync point).
        /// Above 1x it lerps linearly to 1.0 at deviceSpeedRamp.
        /// Below 1x it uses a power curve (exponent 2.24) so the device slows much more
        /// aggressively than linear — e.g. sittingBJ (cfg=0.15) at 0.30x anim → ~1% device.
        /// The 1x sync point is always preserved: DeviceIntensity(1.0) == defaultDeviceIntensity.
        /// </summary>
        private float DeviceIntensity()
        {
            float maxAnim = deviceSpeedRamp;
            float v;
            if (animSpeed <= 1.0f)
                // Power curve: v = base * animSpeed^2.24
                // Gives ~1% at 0.30x for cfg=0.15, and exactly cfg at 1.0x.
                v = defaultDeviceIntensity * (float)Math.Pow(animSpeed, 2.24);
            else
                v = defaultDeviceIntensity + (1.0f - defaultDeviceIntensity) * (animSpeed - 1.0f) / (maxAnim - 1.0f);
            return Math.Max(0f, Math.Min(1f, v));
        }

        /// <summary>Full state reset.</summary>
        private void AttachGirlBlip()
        {
            RemoveGirlBlip();
            if (girl == null || !girl.Exists()) return;
            _girlBlip = girl.AddBlip();
            bool isHooker = !sandboxMode || _aLifeHookerServices;
            _girlBlip.Sprite = (BlipSprite)57; // 57 = Person silhouette
            _girlBlip.Color  = BlipColor.Pink;
            _girlBlip.Scale  = 0.45f;
            _girlBlip.Name   = isHooker ? "Hooker" : "Woman";
            _girlBlip.IsShortRange = true;
        }

        private void RemoveGirlBlip()
        {
            if (_girlBlip != null && _girlBlip.Exists())
                _girlBlip.Delete();
            _girlBlip = null;
        }

        private void FullReset(bool keepGirlTask = false)
        {
            _telemetry.SendStop();
            DetachDildoProp();
            RemoveGirlBlip();
            if (keepGirlTask)
            {
                // Release girl without cancelling her flee/fight task
                if (girl != null && girl.Exists())
                {
                    if (girl.IsAlive)
                        Function.Call((Hash)0x726256CC1EEB182F, girl); // CLEAR_FACIAL_IDLE_ANIM_OVERRIDE — remove override before release
                    Function.Call(Hash.SET_PED_KEEP_TASK, girl, true);
                    girl.IsPositionFrozen = false;
                    Function.Call(Hash.SET_ENTITY_COLLISION, girl, true, true);
                    girl.BlockPermanentEvents = false;
                    girl.IsPersistent = false;
                    girl.MarkAsNoLongerNeeded();
                }
                girl = null;
                hasGirl = false;
            }
            else
            {
                ReleaseGirl();
            }
            Function.Call(Hash.SET_EVERYONE_IGNORE_PLAYER, Game.Player, false);
            Function.Call(Hash.SET_PLAYER_CAN_DO_DRIVE_BY, Game.Player, true);

            Ped player = Game.Player.Character;
            player.IsPositionFrozen = false;
            Function.Call(Hash.SET_ENTITY_COLLISION, player, true, true);
            if (player.IsInVehicle() && player.CurrentVehicle != null)
            {
                player.CurrentVehicle.IsInteriorLightOn = false;
            }

            menuLevel = MenuLevel.None;
            state = ScriptState.Idle;
            mode = Mode.None;
            animPhase = 0;
            animSpeed = 1.0f;
            _femaleAnimSpeedScale = 1.0f;
            _kissingReverse = false;
            _standingBjReverse = false;
            _rdoggyReverse = false;
            _sitCowgirlPulloutReverse = false;
            _standingBjSwallowAt = 0;
            _backseatEntryPending = false;
            _backseatCarSex = false;
            _vehEntryStartTime = 0;
            _lastVehEnterTaskTime = 0;
            approachPhase = 0;
            approachAnimWait = 0;
            approachSexySwapTime = 0;
            jHoldStart = 0;
            approachDict = "";
            carPropActive = false;
            carPropPhase = 0;
            carPropWaitTime = 0;
            _streetSitPropActive = false;
            _streetSitPropPhase = 0;
            _streetSitPropWaitTime = 0;
            _streetSitCategory = MenuLevel.None;
            _approachSecondChance = false;
            try { if (_bjPlayer   != null) _bjPlayer.Stop();   } catch { }
            try { if (_slapPlayer != null) _slapPlayer.Stop(); } catch { }
            try { if (_kissPlayer != null) _kissPlayer.Stop(); } catch { }
            _bjSoundBusyUntil = 0;
            _bjThroatPending  = false;
            _lastSlapSoundTime = 0;
            _lastKissSoundTime = 0;
            _lastRumbleTime    = 0;
            _rumblePulseActive = false;
            _finishExecutePending = false;
            _finishExecuteAt      = 0;
            _finishInside         = false;
            _finishForced         = false;
            _finishBroke          = false;
            _finishAsked          = false;
            _postSexPhase         = 0;
            _postSexPendingMenu   = MenuLevel.None;

            _girlLastHealth       = -1f;
            _girlCurrentVoice     = null;
            _voiceReapplyAt       = 0;
            _faceReapplyAt        = 0;
            _escalateConsented       = false;
            _escalateRejectedOnce    = false;
            _escalatePermaRejected   = false;
            _invitedToVehicle        = false;
            _aLifeHookerServices  = false;
            _aLifeFinishInsidePrice = 0;
            _aLifeHookerPriceLocked.Clear();
            if (_prostAutoApproachActive)
                _prostAutoApproachCooldown = Game.GameTime + 20000; // 20s before another hooker auto-approaches
            _prostAutoApproachActive = false;
            _currentGirlFp  = null;
            _currentGirlKey = null;
            // Clear approachCandidate so faceReapply doesn't keep reapplying her mood
            // expression after she's been released — which would freeze it on her corpse if she dies.
            if (_approachCandidate != null && _approachCandidate.Exists() && _approachCandidate.IsAlive)
                Function.Call((Hash)0x726256CC1EEB182F, _approachCandidate); // CLEAR_FACIAL_IDLE_ANIM_OVERRIDE
            _approachCandidate = null;
        }

        

        /// <summary>Draw the selection menu on screen.</summary>
        private void DrawMenu()
        {
            // Clamp menuIndex when dynamic items disappear (e.g. vehicle drives away)
            int maxIdx = GetCurrentMenuItemCount() - 1;
            if (maxIdx < 0) maxIdx = 0;
            if (menuIndex > maxIdx) menuIndex = maxIdx;

            const float PX     = 0.025f;
            const float TOP    = 0.045f;
            const float W      = 0.216f;
            const float HDR_H  = 0.047f;
            const float PAD    = 0.007f;
            const float LH     = 0.034f;

            // ── Pass 1: measure actual content height by drawing items ──────────
            // Items are drawn here but will be immediately covered by the background
            // rect in Pass 2, then redrawn correctly on top in Pass 3.
            float yMeasure = TOP + HDR_H + 0.016f;
            DispatchDrawMenu(PX, ref yMeasure, LH);

            bool showSwipe = IsInServiceSubMenu() && !sandboxMode;
            bool showAmt   = menuLevel == MenuLevel.GiveMoney;
            float FOOT_H   = (showSwipe || showAmt) ? 0.042f : 0.029f;

            float footY   = yMeasure;
            float totalH  = (footY + FOOT_H) - TOP;

            // ── Pass 2: draw correctly-sized background and header (covers pass 1) ──
            DrawMenuRect(PX, TOP, W, totalH, 0, 0, 0, 190);
            DrawMenuRect(PX, TOP, W, HDR_H, 79, 38, 142, 230);

            // ── Header title "肉桂情事" — vertically centered in the header bar ──
            float hdrTextY = TOP + (HDR_H * 0.5f) - 0.014f;
            DrawMenuText("肉桂情事", PX + PAD, hdrTextY, W - PAD * 2f, 0.50f, true, 210, 125, 45, 255, 7);

            // ── Pass 3: redraw items on top of background ────────────────────────
            float y = TOP + HDR_H + 0.016f;
            DispatchDrawMenu(PX, ref y, LH);

            // ── Dark footer bar ────────────────────────────────────────────────────
            DrawMenuRect(PX, footY, W, FOOT_H, 30, 30, 30, 210);
            string footLine = "[" + MenuUpDisplay + "/" + MenuDownDisplay + "] 移動  ["
                            + MenuSelectDisplay + "] 選擇 [" + MenuBackDisplay + "] 返回  ["
                            + MasterDisplay + "] 關閉";
            if (showSwipe)
                footLine += "\n[" + SpeedDownDisplay + "/" + SpeedUpDisplay + "] 滑動";
            else if (showAmt)
                footLine += "\n[" + SpeedDownDisplay + "/" + SpeedUpDisplay + "] 金額";
            DrawMenuText(footLine, PX + PAD, footY + 0.006f, W - PAD * 2f, 0.19f, false, 200, 200, 200, 255);
        }

        private void DispatchDrawMenu(float x, ref float y, float lh)
        {
            if      (menuLevel == MenuLevel.Main)               DrawMainMenu(x, ref y, lh);
            else if (menuLevel == MenuLevel.SandboxCar)         DrawSandboxCarMenu(x, ref y, lh);
            else if (menuLevel == MenuLevel.Services)           DrawServicesMenu(x, ref y, lh);
            else if (menuLevel == MenuLevel.SitDownSub)         DrawSitDownSubMenu(x, ref y, lh);
            else if (IsInServiceSubMenu())                      DrawStreetServiceSubMenu(x, ref y, lh);
            else if (menuLevel == MenuLevel.Compatibility)      DrawCompatibilityMenu(x, ref y, lh);
            else if (menuLevel == MenuLevel.Settings)           DrawSettingsMenu(x, ref y, lh);
            else if (menuLevel == MenuLevel.Approach)           DrawApproachMenu(x, ref y, lh);
            else if (menuLevel == MenuLevel.ApproachLine)       DrawApproachLineMenu(x, ref y, lh);
            else if (menuLevel == MenuLevel.Actions)            DrawActionsMenu(x, ref y, lh);
            else if (menuLevel == MenuLevel.GiveMoney)         DrawGiveMoneyMenu(x, ref y, lh);
            else if (menuLevel == MenuLevel.InviteToVehicle)    DrawInviteToVehicleMenu(x, ref y, lh);
            else if (menuLevel == MenuLevel.Conversation)       DrawConversationMenu(x, ref y, lh);
            else if (menuLevel == MenuLevel.ConvSub)            DrawConvSubMenu(x, ref y, lh);
            else if (menuLevel == MenuLevel.Intimacy)           DrawIntimacyMenu(x, ref y, lh);
            else if (menuLevel == MenuLevel.IntimacySub)        DrawIntimacySubMenu(x, ref y, lh);
            else if (menuLevel == MenuLevel.Escalate)           DrawEscalateMenu(x, ref y, lh);
            else if (menuLevel == MenuLevel.Leave)              DrawLeaveMenu(x, ref y, lh);
            else if (menuLevel == MenuLevel.LeaveLine)          DrawLeaveLineMenu(x, ref y, lh);
            else if (menuLevel == MenuLevel.Finish)             DrawFinishMenu(x, ref y, lh);
        }

        /// <summary>Estimate the extra vertical space (beyond selectable items) needed per menu level.</summary>
        private float GetMenuInfoHeight()
        {
            const float LH = 0.038f;
            const float SH = 0.034f; // DrawSectionHeader height

            // Every menu has at least one section header (SH).
            // Extra info lines (name/rep/personality/mood) each add LH.
            // Gaps (y += lh * 0.3f) add ~0.011f each.

            if (menuLevel == MenuLevel.Main)
                // 4 section headers + 3 thin-divider sequences (lh*0.3 + lh*0.8 each) + extra slack
                return SH * 4 + (LH * 1.1f) * 3 + LH * 1.5f;
            if (menuLevel == MenuLevel.Actions)
                // SH + name + rep + personality + mood + gap
                return SH + LH * 4 + LH * 0.3f;
            if (menuLevel == MenuLevel.Conversation)
                return SH + LH * 4 + LH * 0.3f;
            if (menuLevel == MenuLevel.Leave)
                // SH + name + rep + personality + gap
                return SH + LH * 3 + LH * 0.3f;
            if (menuLevel == MenuLevel.Intimacy)
                // SH + name + rep + bj/rough line + gap
                return SH + LH * 3 + LH * 0.3f;
            if (menuLevel == MenuLevel.Escalate)
                // SH + name + (optional pref lines) + gap
                return SH + LH * 2 + LH * 0.3f;
            if (menuLevel == MenuLevel.InviteToVehicle)
                // SH + name + rep + gap
                return SH + LH * 2 + LH * 0.3f;
            if (menuLevel == MenuLevel.Approach)
                // SH + known info block (3 lines + gap) + pre-item gap
                return SH + LH * 3 + LH * 0.6f;
            if (menuLevel == MenuLevel.Finish)
                // SH + name line + gaps
                return SH + LH * 1.5f;
            if (menuLevel == MenuLevel.Services)
                // SH + optional name line + gap
                return SH + LH * 2.0f;
            if (IsInServiceSubMenu())
                return SH + LH * 2.0f;
            // ApproachLine, ConvSub, IntimacySub, LeaveLine, SandboxCar, Settings, Compatibility
            return SH + LH * 0.5f;
        }

        private void DrawFinishMenu(float x, ref float y, float lh)
        {
            DrawSectionHeader("完成", x, ref y);

            // ── Sandbox (non-A-Life): simple 2-item menu ──
            if (!aLifeMode)
            {
                y += lh * 0.3f;
                DrawMenuItem("完成 (外射)", 0, x, ref y, lh);
                y += lh * 0.3f;
                DrawMenuItem("完成 (內射)", 1, x, ref y, lh);
                return;
            }

            // ── A-Life: full info + Ask + price display ──
            ALifePedData dFin = null;
            if (_currentGirlKey != null)
                _aLifePeds.TryGetValue(_currentGirlKey, out dFin);

            if (dFin != null)
            {
                // name display removed
            }

            if (!_finishAsked)
            {
                DrawMenuItem("要求內射", 0, x, ref y, lh);
                y += lh * 0.1f;
            }
            // After Ask the items shift: Finish → 0, Finish inside her → 1
            int drawFinish      = _finishAsked ? 0 : 1;
            int drawFinishInside = _finishAsked ? 1 : 2;
            DrawMenuItem("完成 (外射)", drawFinish, x, ref y, lh);
            y += lh * 0.3f;

            // Show the quoted price after she's agreed — works for both exclusive hooker and Prost A-Life
            if (_aLifeFinishInsidePrice > 0)
                DrawMenuItem("完成 (內射)  ~g~($" + _aLifeFinishInsidePrice + ")~s~", drawFinishInside, x, ref y, lh);
            else
                DrawMenuItem("完成 (內射)", drawFinishInside, x, ref y, lh);
        }

        private void DrawApproachMenu(float x, ref float y, float lh)
        {
            if (_approachCandidate == null || !_approachCandidate.Exists())
            {
                DrawSectionHeader("接近", x, ref y);
                DrawTextOnScreen("~r~目標已遺失。~s~", x, y); y += lh;
                return;
            }

            string fp = GetPedFingerprint(_approachCandidate);
            ALifePedData d = null;
            bool known = fp != null && _aLifePeds.TryGetValue(ALifeKey(fp), out d);
            string headerName = (known && d != null && d.NameKnown && d.Name != null && d.Name.Length > 0) ? " " + d.Name.ToUpper() : "";
            DrawSectionHeader("接近" + headerName, x, ref y);

            y += lh * 0.3f;
            DrawMenuItem("溫柔 ~b~[>]~s~",     0, x, ref y, lh);
            DrawMenuItem("自信 ~b~[>]~s~",  1, x, ref y, lh);
            DrawMenuItem("高冷 ~b~[>]~s~",       2, x, ref y, lh);
            DrawMenuItem("奉承 ~b~[>]~s~", 3, x, ref y, lh);
            DrawMenuItem("玩鬧 ~b~[>]~s~",    4, x, ref y, lh);
        }

        private void DrawApproachLineMenu(float x, ref float y, float lh)
        {
            string[] clusterNames = { "溫柔", "自信", "高冷", "奉承", "玩鬧" };
            string clusterName = (_approachSelectedCluster >= 0 && _approachSelectedCluster < clusterNames.Length)
                ? clusterNames[_approachSelectedCluster] : "?";

            DrawSectionHeader(clusterName.ToUpper(), x, ref y);

            if (_approachCandidate == null || !_approachCandidate.Exists())
            {
                DrawTextOnScreen("~r~Target lost.~s~", x, y); y += lh;
                return;
            }

            string fp = GetPedFingerprint(_approachCandidate);
            ALifePedData d = null;
            bool known = fp != null && _aLifePeds.TryGetValue(ALifeKey(fp), out d);
            bool firstEncounter = d == null || !d.HasMet;
            string name = DisplayName(d);

            y += lh * 0.3f;

            if (!sandboxMode)
            {
                // Prost A-Life: prostitution-specific approach lines
                string[][][] prostLines = new string[][][] {
                    // Gentle
                    new string[][] {
                        new string[] {
                            "\"嘿... 在工作嗎？\"",
                            "\"抱歉—我不知道該怎麼問。妳有空嗎？\"",
                            "\"我不是想套近乎。妳的時間怎麼算？\"",
                            "\"打擾一下。我想找人陪陪。妳做這行嗎？\""
                        },
                        new string[] {
                            "\"嘿，又是我。還記得我嗎？\"",
                            "\"見到妳真好。現在有空嗎？\"",
                            "\"我就希望能遇到妳。\""
                        }
                    },
                    // Confident
                    new string[][] {
                        new string[] {
                            "\"在工作嗎？多少錢？\"",
                            "\"我有現金。妳能提供什麼服務？\"",
                            "\"開個價吧。我不是來討價還價的。\"",
                            "\"別廢話了。妳收費多少？\""
                        },
                        new string[] {
                            "\"我又來了。和上次一樣？\"",
                            "\"妳現在有空嗎？我有錢。\"",
                            "\"別浪費時間了。怎麼算？\""
                        }
                    },
                    // Cool
                    new string[][] {
                        new string[] {
                            "\"有空嗎？\"",
                            "\"怎麼收費？\"",
                            "\"想找個伴。在接客嗎？\"",
                            "\"妳有時間陪我嗎？\""
                        },
                        new string[] {
                            "\"又見面了。很好。\"",
                            "\"還在這裡接客？\"",
                            "\"我想說順便過來看看。\""
                        }
                    },
                    // Flattering
                    new string[][] {
                        new string[] {
                            "\"這附近最漂亮的就妳了。怎麼收費？\"",
                            "\"不管妳收費多少，妳都值得。\"",
                            "\"說真的—像妳這樣的女孩要多少錢？\"",
                            "\"妳看起來棒極了。在工作嗎？\""
                        },
                        new string[] {
                            "\"妳比上次更美了。有空嗎？\"",
                            "\"I've been thinking about coming back. 有空嗎？\"",
                            "\"妳依然是這最棒的。妳覺得呢？\""
                        }
                    },
                    // Playful
                    new string[][] {
                        new string[] {
                            "\"好吧，我就直接問了—多少錢？\"",
                            "\"告訴我妳有空。拜託。\"",
                            "\"我有錢而且我知道我想要什麼。有興趣嗎？\"",
                            "\"妳看起來正是我花錢想找的那種麻煩。\""
                        },
                        new string[] {
                            "\"想我了嗎？\"",
                            "\"又來了。我知道，我知道。有空嗎？\"",
                            "\"準備好第二回合了嗎？\""
                        }
                    }
                };
                int pC = (_approachSelectedCluster >= 0 && _approachSelectedCluster < prostLines.Length)
                    ? _approachSelectedCluster : 0;
                string[] pLines = firstEncounter ? prostLines[pC][0] : prostLines[pC][1];
                int pIdx = 0;
                foreach (string pl in pLines)
                { DrawMenuItemWrapped(pl, pIdx, x, ref y, lh); pIdx++; }
                return;
            }

            string[][][] allLines = new string[][][] {
                // Gentle
                new string[][] {
                    new string[] {
                        "\"打擾一下，希望沒有妨礙到妳。\"",
                        "\"妳的笑容真的很溫暖。\"",
                        "\"我無法就這樣走過去而不打招呼。\"",
                        "\"介意我陪妳一會兒嗎？\""
                    },
                    new string[] {
                        "\"Hey " + name + ", 很高興又見到妳。\"",
                        "\"我一直想著妳。想一起去逛逛嗎？\"",
                        "\"跟我來。\""
                    }
                },
                // Confident
                new string[][] {
                    new string[] {
                        "\"打擾了。妳吸引了我的注意。\"",
                        "\"我不想假裝沒注意到妳。\"",
                        "\"妳看起來知道自己想要什麼。\"",
                        "\"有時間嗎？\""
                    },
                    new string[] {
                        "\"Hey " + name + ". 我們別浪費時間了。\"",
                        "\"有空嗎？\"",
                        "\"跟我來。\""
                    }
                },
                // Cool
                new string[][] {
                    new string[] {
                        "\"妳看起來好像有更好的事要做。我也一樣。\"",
                        "\"有意思。妳很難懂。\"",
                        "\"長話短說：妳美極了。\"",
                        "\"沒多少人有妳這種氣質。\""
                    },
                    new string[] {
                        "\"" + name + ". Good.\"",
                        "\"還是一樣難懂。\"",
                        "\"別讓我問兩次。\""
                    }
                },
                // Flattering
                new string[][] {
                    new string[] {
                        "\"如果我不說點什麼那就是白痴了。\"",
                        "\"妳完全是我的菜。\"",
                        "\"別誤會，但妳真的很美。\"",
                        "\"妳值得別人的關注。\""
                    },
                    new string[] {
                        "\"Hey " + name + ", 我一直想著妳。\"",
                        "\"妳看起來比我記憶中還要美。\"",
                        "\"妳會跟我走，對吧？\""
                    }
                },
                // Playful
                new string[][] {
                    new string[] {
                        "\"嘿，我打賭妳常聽到這句話，但是該死，妳真美。\"",
                        "\"妳看起來像個麻煩。我喜歡。\"",
                        "\"告訴我妳不像外表看起來那麼有趣。\"",
                        "\"能耽誤妳五分鐘嗎？\""
                    },
                    new string[] {
                        "\"Hey " + name + ", 準備好惹點麻煩了嗎？\"",
                        "\"想我了嗎？\"",
                        "\"來吧，我們走。\""
                    }
                }
            };

            int c = (_approachSelectedCluster >= 0 && _approachSelectedCluster < allLines.Length)
                ? _approachSelectedCluster : 0;
            string[] lines = firstEncounter ? allLines[c][0] : allLines[c][1];
            int idx = 0;
            foreach (string line in lines)
            { DrawMenuItemWrapped(line, idx, x, ref y, lh); idx++; }
        }

        private void DrawActionsMenu(float x, ref float y, float lh)
        {
            DrawSectionHeader("互動", x, ref y);

            ALifePedData dAct = null;
            if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out dAct);
            int idx = 0;
            if (ShowInviteVehicleItem())
                DrawMenuItem("邀請上車 ~b~[>]~s~", idx++, x, ref y, lh);
            DrawMenuItem("對話交流 ~b~[>]~s~", idx++, x, ref y, lh);
            bool isHookerAct = (dAct != null && (dAct.IsHooker || dAct.ALifeMode == "Prostitute"));
            DrawMenuItem(isHookerAct ? "服務項目 ~b~[>]~s~" : "親密互動 ~b~[>]~s~", idx++, x, ref y, lh);
            // "Give Her Money" — A-Life only
            if (aLifeMode && _currentGirlKey != null)
                DrawMenuItem("給予金錢 ~b~[>]~s~", idx++, x, ref y, lh);
            // "Move to seat" — prostitution/hooker girls when vehicle available
            if (ShowSeatSwapInActions())
                DrawMenuItem(GetSeatItemLabel(), idx, x, ref y, lh);
        }

        private void DrawGiveMoneyMenu(float x, ref float y, float lh)
        {
            DrawSectionHeader("給予金錢", x, ref y);

            string[] denomLabels = { "$1", "$10", "$100", "$1,000", "$10,000", "$100,000", "$1,000,000", "$10,000,000", "$100,000,000" };
            int[] mults = { 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000 };
            long total = 0;
            for (int i = 0; i < 9; i++)
            {
                long rowVal = (long)_giveMoneyAmounts[i] * mults[i];
                total += rowVal;
                string rowText = denomLabels[i] + "  x" + _giveMoneyAmounts[i];
                DrawMenuItem(rowText, i, x, ref y, lh);
            }
            if (total > 999999999) total = 999999999;
            y += lh * 0.3f;
            DrawMenuItem("確認給予 ~s~(總金額: ~g~$" + total.ToString("N0") + "~s~)", 9, x, ref y, lh);
        }

        private void DrawInviteToVehicleMenu(float x, ref float y, float lh)
        {
            DrawSectionHeader("邀請上車", x, ref y);

            ALifePedData dInvDraw = null;
            if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out dInvDraw);
            DrawMenuItem("\"要搭便車嗎？\"",              0, x, ref y, lh);
            DrawMenuItem("\"上車吧，我載我們去兜風。\"", 1, x, ref y, lh);
            DrawMenuItem("\"來吧，我們走。\"",            2, x, ref y, lh);
        }

        private void DrawIntimacyMenu(float x, ref float y, float lh)
        {
            DrawSectionHeader("親密互動", x, ref y);

            ALifePedData dI = null;
            if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out dI);


            DrawMenuItem("詢問喜好 ~b~[>]~s~", 0, x, ref y, lh);
            DrawMenuItem("試探 ~b~[>]~s~",      1, x, ref y, lh);
            DrawMenuItem("進一步發展 ~b~[>]~s~",      2, x, ref y, lh);
            if (_escalateConsented && FindBackseatVehicle() != null)
                DrawMenuItem(GetSeatItemLabel(), 3, x, ref y, lh);
        }

        private void DrawIntimacySubMenu(float x, ref float y, float lh)
        {
            ALifePedData dI = null;
            if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out dI);
            string[] branchNames = { "Ask Preferences", "Test Waters" };
            string bName = (_intimacySelectedBranch >= 0 && _intimacySelectedBranch < branchNames.Length)
                ? branchNames[_intimacySelectedBranch] : "?";
            DrawSectionHeader(bName.ToUpper(), x, ref y);
            y += lh * 0.3f;

            string[][] branches = new string[][] {
                // 0: Ask Preferences
                new string[] {
                    "\"妳接吻嗎？\"",
                    "\"公共場合還是私密點？\"",
                    "\"妳喜歡什麼？\"",
                    "\"妳喜歡用... 嘴巴做那個嗎？\"",
                    "\"妳喜歡粗暴點嗎？\"",
                    "\"妳想不想主導一次？\"",
                    "\"妳絕對不想要什麼？\"",
                    "\"妳想隨便玩玩嗎？\""
                },
                // 1: Test Waters
                new string[] {
                    "\"妳看起來讓人難以抗拒。\"",
                    "\"我想更靠近妳。\"",
                    "\"妳也想要這個嗎？\""
                }
            };

            int b = (_intimacySelectedBranch >= 0 && _intimacySelectedBranch < branches.Length)
                ? _intimacySelectedBranch : 0;
            string[] items = branches[b];
            for (int i = 0; i < items.Length; i++)
                DrawMenuItem(items[i], i, x, ref y, lh);
        }

        private void DrawEscalateMenu(float x, ref float y, float lh)
        {
            DrawSectionHeader("親密模式", x, ref y);

            ALifePedData dE = null;
            if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out dE);


            if (!_escalateConsented)
            {
                bool isNight = IsNight();
                DrawMenuItem(isNight ? "\"今晚留下來陪我。妳覺得呢？\""  : "\"我們來找點樂子吧。\"", 0, x, ref y, lh);
                DrawMenuItem("\"介意我... 靠近一點嗎？..\"",                                                1, x, ref y, lh);
                DrawMenuItem("\"想做愛嗎？\"",                                         2, x, ref y, lh);
            }
            else
            {
                bool isCar = (mode == Mode.Car);
                if (!isCar)
                {
                    // Street: show Standing Services + Sit Down hub
                    DrawMenuItem("站姿服務 ~b~[>]~s~", 0, x, ref y, lh);
                    DrawMenuItem("Sit Down ~b~[>]~s~",          1, x, ref y, lh);
                }
                else
                {
                    string[] items = carServiceItems;
                    bool backseatEsc = IsBackseatCarMode();
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (backseatEsc && i != 1) continue; // backseat: only Sex
                        string label = items[i];
                        if (dE != null && dE.PrefBJ == false && IsEscalateBJItem(isCar, i))
                            label = "~c~" + label + "  ~r~(N/A)~s~";
                        else if (dE != null && dE.PrefRough == false && IsEscalateRoughItem(isCar, i))
                            label = "~c~" + label + "  ~r~(N/A)~s~";
                        DrawMenuItem(label, backseatEsc ? 0 : i, x, ref y, lh);
                    }
                }
            }
        }

        private void DrawConversationMenu(float x, ref float y, float lh)
        {
            DrawSectionHeader("對話", x, ref y);

            ALifePedData dAct = null;
            if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out dAct);
            // Prostitution A-Life: stripped-down conversation — no romantic/arrangement branches
            if (dAct != null && dAct.ALifeMode == "Prostitute")
            {
                DrawMenuItem("認識她 ~b~[>]~s~", 0, x, ref y, lh);
                DrawMenuItem("閒聊 ~b~[>]~s~",      1, x, ref y, lh);
                DrawMenuItem("查看心情 ~b~[>]~s~",      2, x, ref y, lh);
                DrawMenuItem("調情 ~b~[>]~s~",           3, x, ref y, lh);
                return;
            }

            DrawMenuItem("認識她 ~b~[>]~s~",   0, x, ref y, lh);
            DrawMenuItem("閒聊 ~b~[>]~s~",         1, x, ref y, lh);
            DrawMenuItem("查看心情 ~b~[>]~s~",         2, x, ref y, lh);
            DrawMenuItem("調情 ~b~[>]~s~",              3, x, ref y, lh);
            bool isGf4 = (dAct != null && dAct.Relationship == "Girlfriend");
            DrawMenuItem((isGf4 ? "分手 ~b~[>]~s~" : "讓她成為我的 ~b~[>]~s~"), 4, x, ref y, lh);
            DrawMenuItem("詢問私人問題 ~b~[>]~s~", 5, x, ref y, lh);
            // Business is hidden when she's a girlfriend or obsessed — no arrangement needed
            bool hideBusinessMenu = (dAct != null && (dAct.Relationship == "Girlfriend" || dAct.Relationship == "Obsessed"));
            if (!hideBusinessMenu)
                DrawMenuItem("交易談判 ~b~[>]~s~", 6, x, ref y, lh);
        }

        private void DrawConvSubMenu(float x, ref float y, float lh)
        {
            ALifePedData dAct = null;
            if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out dAct);
            bool isGfBranch = (_convSelectedBranch == 4 && dAct != null && dAct.Relationship == "Girlfriend");
            string branch4Name = isGfBranch ? "Break Up" : "Make Her Mine";
            string[] branchNames = { "Get to Know Her", "Small Talk", "查看心情", "Flirt", branch4Name, "Ask Personal Stuff", "Business" };
            string branchName = (_convSelectedBranch >= 0 && _convSelectedBranch < branchNames.Length)
                ? branchNames[_convSelectedBranch] : "?";
            DrawSectionHeader(branchName.ToUpper(), x, ref y);

            // Prostitution A-Life: completely different items for branches 0-3
            if (dAct != null && dAct.ALifeMode == "Prostitute" && _convSelectedBranch >= 0 && _convSelectedBranch <= 3)
            {
                bool nightDraw = IsNight();
                string[][] prostBranches = new string[][] {
                    // 0: Get to Know Her
                    new string[] {
                        "\"我該怎麼稱呼妳？\"",
                        "\"妳是哪裡人？\"",
                        "\"妳一直在這一帶工作嗎？\"",
                        "\"妳做這行多久了？\""
                    },
                    // 1: Small Talk
                    new string[] {
                        nightDraw ? "\"今晚生意清淡嗎？\"" : "\"今天生意清淡嗎？\"",
                        nightDraw ? "\"妳今晚要待到很晚嗎？\"" : "\"妳出來很久了嗎？\"",
                        "\"有人找妳麻煩嗎？\"",
                        "\"妳是一個人單幹嗎？\""
                    },
                    // 2: Check Mood
                    new string[] {
                        "\"妳最近過得怎麼樣？\"",
                        "\"妳還好嗎？\"",
                        "\"妳看起來壓力很大。\"",
                        nightDraw ? "\"妳今晚看起來很迷人。\"" : "\"妳今天看起來很迷人。\""
                    },
                    // 3: Flirt
                    new string[] {
                        "\"走過妳身邊時，很難不轉頭看妳。\"",
                        "\"我每次來這都會找妳。\"",
                        "\"妳身上有一股獨特的魅力。\"",
                        nightDraw ? "\"因為有妳，讓今晚很值得回來。\"" : "\"因為有妳，讓這一切很值得回來。\""
                    },
                };
                string[] prostItems = prostBranches[_convSelectedBranch];
                for (int pi = 0; pi < prostItems.Length; pi++)
                    DrawMenuItemWrapped(prostItems[pi], pi, x, ref y, lh);
                return;
            }

            string[][] branches = new string[][] {
                // 0: Get to Know Her
                new string[] {
                    "\"妳叫什麼名字？\"",
                    "\"妳平常有什麼休閒娛樂？\"",
                    "\"妳是本地人嗎？\"",
                    "\"妳喜歡哪一類型的男生？\"",
                    "\"妳心目中在尋找什麼樣的關係？\"",
                    "\"妳目前有交往對象嗎？\"",
                    "\"可以聊聊妳的故事嗎？\""
                },
                // 1: Small Talk
                new string[] {
                    "\"今天天氣不錯。\"",
                    "\"妳肚子餓嗎？\"",
                    IsNight() ? "\"今晚過得充實嗎？\"" : "\"今天過得充實嗎？\"",
                    "\"妳喜歡這座城市嗎？\"",
                    "\"妳看起來有點疲倦。\"",
                    "\"妳最近都在忙些什麼？\""
                },
                // 2: Check Mood
                new string[] {
                    "\"妳今天心情怎麼樣？\"",
                    "\"妳還好嗎？\"",
                    "\"妳看起來有點緊繃。\"",
                    "\"妳看起來心情很好。\"",
                    "\"妳看起來有點分心。\""
                },
                // 3: Flirt
                new string[] {
                    "\"妳看起來太迷人了。\"",
                    "\"妳有一種讓我無法忽視的吸引力。\"",
                    "\"我想擁有妳。\"",
                    (dAct != null && dAct.HasMetSecondTime) ? "\"我想妳的次數，已經超過了該有的分寸。\"" : "~c~\"我想妳的次數，已經超過了該有的分寸。\" ~r~(第二次會面+)~s~",
                    "\"妳是個磨人的小妖精，對吧？\"",
                    (dAct != null && dAct.HasMetSecondTime) ? "\"妳有想我嗎？\"" : "~c~\"妳有想我嗎？\" ~r~(第二次會面+)~s~",
                    IsNight() ? "\"妳今晚非常漂亮。\"" : "\"妳今天非常漂亮。\"",
                    "\"我愛妳。\""
                },
                // 4: Make Her Mine / Break Up
                isGfBranch ? new string[] {
                    "\"我需要一些個人空間。\"",
                    "\"我們之間行不通了。\"",
                    "\"我覺得我們應該到此為止。\"",
                    "\"我對妳不再有以前的感覺了。\"",
                    "\"對不起，我沒辦法再繼續下去了。\"",
                    "\"這不是妳的錯，是我自己的問題。\"",
                    "\"我覺得我們當初進展得太快了。\""
                } : new string[] {
                    "\"我不滿足於只是普通朋友。\"",
                    "\"妳願意做我的女朋友嗎？\"",
                    "\"我一直都在思考我們的未來。\"",
                    "\"讓我帶妳去個浪漫的地方。\"",
                    "\"妳跟其他人截然不同。\"",
                    "\"我們要怎樣才能真正走在一起？\"",
                    "\"我想我已經無可救藥地愛上妳了。\""
                },
                // 5: Ask Personal Stuff
                new string[] {
                    "\"真實的妳是什麼樣子的？\"",
                    "\"妳為什麼一個人孤單待在外面？\"",
                    "\"妳的家人是什麼樣的？\"",
                    "\"妳生命中追求的是什麼？\"",
                    "\"妳這輩子最大的遺憾是什麼？\"",
                    "\"妳最害怕的是什麼？\"",
                    "\"妳曾真正愛過一個人嗎？\""
                },
                // 6: Business — dynamic based on IsHooker / WasHooker state
                new string[] {
                    (dAct != null && dAct.IsHooker)
                        ? "\"我想我們不應該再以這種金錢關係見面了。\""
                        : (dAct != null && dAct.WasHooker)
                            ? "\"妳還有興趣陪陪我嗎？我一樣會給妳豐厚的報酬。\""
                            : "\"我想付費買下妳的相處時間。\""
                }
            };

            int b = (_convSelectedBranch >= 0 && _convSelectedBranch < branches.Length) ? _convSelectedBranch : 0;
            string[] items = branches[b];
            for (int i = 0; i < items.Length; i++)
                DrawMenuItemWrapped(items[i], i, x, ref y, lh);
        }

        private void DrawLeaveMenu(float x, ref float y, float lh)
        {
            DrawSectionHeader("離開", x, ref y);

            ALifePedData dLeaveH = null;
            if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out dLeaveH);
            // ── Prostitution A-Life: street-tone goodbye clusters ─────────────────
            if (dLeaveH != null && dLeaveH.ALifeMode == "Prostitute")
            {
                DrawMenuItem("簡短甜美 ~b~[>]~s~", 0, x, ref y, lh);
                DrawMenuItem("圓滑風趣 ~b~[>]~s~", 1, x, ref y, lh);
                DrawMenuItem("真誠率性 ~b~[>]~s~", 2, x, ref y, lh);
                DrawMenuItem("溫暖體貼 ~b~[>]~s~", 3, x, ref y, lh);
                DrawMenuItem("俏皮打情 ~b~[>]~s~", 4, x, ref y, lh);
                return;
            }

            DrawMenuItem("溫柔 ~b~[>]~s~",     0, x, ref y, lh);
            DrawMenuItem("自信 ~b~[>]~s~",  1, x, ref y, lh);
            DrawMenuItem("高冷 ~b~[>]~s~",       2, x, ref y, lh);
            DrawMenuItem("奉承 ~b~[>]~s~", 3, x, ref y, lh);
            DrawMenuItem("玩鬧 ~b~[>]~s~",    4, x, ref y, lh);
        }

        private void DrawLeaveLineMenu(float x, ref float y, float lh)
        {
            ALifePedData dLeaveD = null;
            if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out dLeaveD);

            // ── Prostitution A-Life: street-tone leave lines ──────────────────────
            if (dLeaveD != null && dLeaveD.ALifeMode == "Prostitute")
            {
                string[] prostClusterNames = { "簡短甜美", "圓滑風趣", "真誠率性", "溫慢體貼", "俏皮打情" };
                string prostClusterName = (_leaveSelectedCluster >= 0 && _leaveSelectedCluster < prostClusterNames.Length)
                    ? prostClusterNames[_leaveSelectedCluster] : "?";
                DrawSectionHeader(prostClusterName.ToUpper(), x, ref y);
                y += lh * 0.3f;
                string[][] prostLeaveLines = {
                    // Short & Sweet
                    new string[] {
                        "\"行了，我先走了。\"",
                        "\"待會見。\"",
                        "\"打起精神來。\""
                    },
                    // Smooth
                    new string[] {
                        "\"跟妳在一起總是這麼棒。\"",
                        "\"妳讓我總是忍不住想再回來找妳。\"",
                        "\"我們還會再見面的。\""
                    },
                    // Real
                    new string[] {
                        "\"妳真的很實在，我是說真的。\"",
                        "\"跟妳在一起我總是能感到最真實的放鬆。\"",
                        "\"不玩虛招，我欣賞這點。\""
                    },
                    // Warm
                    new string[] {
                        "\"一個人在這附近要小心安全。\"",
                        "\"我是認真的，多保重自己。\"",
                        "\"妳不應該只待在這種街角。\""
                    },
                    // Flirty
                    new string[] {
                        "\"現在我不得不離開溫柔鄉了，這真不公平。\"",
                        "\"每次離開妳都變得更困難了。\"",
                        "\"妳 know 妳是個迷人的小麻煩，對吧？\""
                    }
                };
                int pc = (_leaveSelectedCluster >= 0 && _leaveSelectedCluster < prostLeaveLines.Length)
                    ? _leaveSelectedCluster : 0;
                string[] plines = prostLeaveLines[pc];
                for (int i = 0; i < plines.Length; i++)
                    DrawMenuItemWrapped(plines[i], i, x, ref y, lh);
                return;
            }

            // ── Casual A-Life ─────────────────────────────────────────────────────
            string[] clusterNames = { "溫柔", "自信", "高冷", "奉承", "玩鬧" };
            string clusterName = (_leaveSelectedCluster >= 0 && _leaveSelectedCluster < clusterNames.Length)
                ? clusterNames[_leaveSelectedCluster] : "?";

            DrawSectionHeader(clusterName.ToUpper(), x, ref y);
            y += lh * 0.3f;

            string[][] leaveLines = {
                // Gentle
                new string[] {
                    "\"這對我來說很有意義。\"",
                    "\"我得先走了，但很高興能遇見妳。\"",
                    "\"出門在外要注意安全喔。\""
                },
                // Confident
                new string[] {
                    "\"我們有緣再見。\"",
                    "\"妳知道去哪裡能找到我。\"",
                    "\"這次體驗很棒，下次別讓我等太久。\""
                },
                // Cool
                new string[] {
                    "\"我自己走就行。\"",
                    "\"沒什麼大不了的，回頭見。\"",
                    "\"別搞得太煽情了，再見。\""
                },
                // Flattering
                new string[] {
                    "\"要轉身離開妳真的太難了。\"",
                    "\"我現在就已經在期待下一次相處了。\"",
                    "\"這絕對是我今天最美好的時刻。\"",
                    "\"我愛妳。\""
                },
                // Playful
                new string[] {
                    "\"好吧，在我做出什麼不會後悔的傻事之前先走。\"",
                    "\"妳 know 妳很危險，對吧？\"",
                    "\"可別太想我。啊，不對，還是多想我一點吧。\""
                }
            };

            int c = (_leaveSelectedCluster >= 0 && _leaveSelectedCluster < leaveLines.Length)
                ? _leaveSelectedCluster : 0;
            string[] lines = leaveLines[c];
            for (int i = 0; i < lines.Length; i++)
            { DrawMenuItemWrapped(lines[i], i, x, ref y, lh); }
        }


        private void DrawSandboxCarMenu(float x, ref float y, float lh)
        {
            DrawSectionHeader("互動", x, ref y);
            DrawMenuItem("服務項目 ~b~[>]~s~", 0, x, ref y, lh);
            
            bool hasWep = girl != null && (girl.Weapons.HasWeapon(WeaponHash.MicroSMG) || girl.Weapons.HasWeapon(WeaponHash.CombatPistol));
            DrawMenuItem(hasWep ? "收回防衛武器" : "給予防衛武器 (Micro SMG)", 1, x, ref y, lh);
            
            if (FindBackseatVehicle() != null)
                DrawMenuItem(GetSeatItemLabel(), 2, x, ref y, lh);
        }

        private void DrawMainMenu(float x, ref float y, float lh)
        {
            int idx = 0;

            // ── ACTIONS ─────────────────────────────────────────────
            DrawSectionHeader("動作", x, ref y);
            if (hasGirl)
            {
                ALifePedData _teleDat = null;
                if (_currentGirlKey != null) _aLifePeds.TryGetValue(_currentGirlKey, out _teleDat);
                bool _teleIsHooker = !sandboxMode || _aLifeHookerServices || (_teleDat != null && _teleDat.IsHooker);
                string _teleLabel  = _teleIsHooker ? "傳送目標（特殊服務）至玩家身邊" : "傳送目標（女性）至玩家身邊";
                if (aLifeMode)
                {
                    DrawMenuItem("互動 ~b~[>]~s~", idx, x, ref y, lh); idx++;
                    DrawMenuItem("離開 ~b~[>]~s~",   idx, x, ref y, lh); idx++;
                    DrawMenuItem(_teleLabel,           idx, x, ref y, lh); idx++;
                }
                else
                {
                    DrawMenuItem("互動 ~b~[>]~s~", idx, x, ref y, lh); idx++;
                    DrawMenuItem("解散目標",        idx, x, ref y, lh); idx++;
                    DrawMenuItem(_teleLabel,           idx, x, ref y, lh); idx++;
                }
            }
            else
            {
                string searchLabel = aLifeMode
                    ? (sandboxMode ? "搭訕女性 ~b~[>]~s~" : "招攬特殊服務 ~b~[>]~s~")
                    : (sandboxMode ? "催眠最近的女性" : "尋找最近的特殊服務");
                DrawMenuItem(searchLabel, idx, x, ref y, lh); idx++;
            }

            // ── OPTIONS ──────────────────────────────────────────
            y += lh * 0.3f;
            DrawMenuRect(x, y + lh * 0.4f, 0.216f, 0.002f, 79, 38, 142, 160);
            y += lh * 0.8f;
            DrawSectionHeader("選項", x, ref y);
            string aiModeLabel = hasGirl
                ? "~c~AI 模式：" + (aLifeMode ? "A-Life" : "催眠模式") + "~s~"
                : "AI 模式：" + (aLifeMode ? "~p~A-Life~s~" : "~b~催眠模式~s~");
            DrawMenuItem(aiModeLabel, idx, x, ref y, lh); idx++;
            string gameModeLabel = (hasGirl && aLifeMode)
                ? "~c~遊戲模式：" + (sandboxMode ? "普通" : "交易") + "~s~"
                : "遊戲模式：" + (sandboxMode ? "~b~Casual~s~" : "~o~Prostitution~s~");
            DrawMenuItem(gameModeLabel, idx, x, ref y, lh); idx++;
            DrawMenuItem("設定 ~b~[>]~s~", idx, x, ref y, lh); idx++;

            // ── ADDONS ─────────────────────────────────────────────
            y += lh * 0.3f;
            DrawMenuRect(x, y + lh * 0.4f, 0.216f, 0.002f, 79, 38, 142, 160);
            y += lh * 0.8f;
            DrawSectionHeader("附加功能", x, ref y);
            string sinkraLabel;
            if (_sinkraCowgirlAvailable)
                sinkraLabel = "Cowgirl Anim by Sinkra: ~g~INSTALLED! ~b~[CLICK FOR MORE]~s~";
            else
                sinkraLabel = "Cowgirl Anim by Sinkra: ~r~未安裝！~s~";
            DrawMenuItemCentered(sinkraLabel, idx, x, ref y, lh, 0.24f); idx++;

            // ── AUTOBLOW DEVICE ────────────────────────────────────
            y += lh * 0.3f;
            DrawMenuRect(x, y + lh * 0.4f, 0.216f, 0.002f, 79, 38, 142, 160);
            y += lh * 0.8f;
            DrawSectionHeader("Autoblow 裝置", x, ref y);
            string tokenDisplay;
            if (tokenKeyboardOpen)
                tokenDisplay = "Token：~y~（鍵盤輸入中……）~s~";
            else if (deviceToken.Length > 0)
                tokenDisplay = "Token：~w~" + deviceToken + "~s~";
            else
                tokenDisplay = "Token：~r~（未設定 — 按下 " + MenuSelectDisplay + " 輸入)~s~";
            DrawMenuItem(tokenDisplay, idx, x, ref y, lh); idx++;

            string bridgeStatus = _telemetry.IsHardDisconnected ? "~r~未連接！"
                : !_telemetry.IsConnected && pingPendingTime <= 0 ? "~r~連線程式未啟動！"
                : !_telemetry.IsConnected ? "~y~正在連線……"
                : _telemetry.IsDeviceOnline ? "~g~已連線！"
                : "~r~未連接！";
            DrawMenuItem("裝置狀態：" + bridgeStatus, idx, x, ref y, lh); idx++;
        }

        private void DrawSettingsMenu(float x, ref float y, float lh)
        {
            DrawSectionHeader("設定", x, ref y);
            int idx = 0;

            string tutorialLabel = "顯示教學提示訊息：" + (showTutorialMessages ? "~g~開啟" : "~r~關閉") + "~s~";
            DrawMenuItem(tutorialLabel, idx, x, ref y, lh); idx++;
            string dickLabel = "男性身體裸露 (假陽具)：" + (dildoEnabled ? "~g~開啟" : "~r~關閉") + "~s~";
            DrawMenuItem(dickLabel, idx, x, ref y, lh); idx++;

            string svcVoiceLabel = "服務進行中女性語音：" + (femaleServiceVoice ? "~g~開啟" : "~r~關閉") + "~s~";
            DrawMenuItem(svcVoiceLabel, idx, x, ref y, lh); idx++;

            string moanVoiceLabel = "服務進行中女性呻吟聲：" + (femaleMoanVoice ? "~g~開啟" : "~r~關閉") + "~s~";
            DrawMenuItem(moanVoiceLabel, idx, x, ref y, lh); idx++;

            string voiceLabel = "結束時女性高潮語音：" + (femaleEndVoice ? "~g~開啟" : "~r~關閉") + "~s~";
            DrawMenuItem(voiceLabel, idx, x, ref y, lh); idx++;

            string bjSoundsLabel = "真實口交音效功能：" + (bjSoundsEnabled ? "~g~開啟" : "~r~關閉") + "~s~";
            DrawMenuItem(bjSoundsLabel, idx, x, ref y, lh); idx++;

            string slapSoundsLabel = "真實性愛撞擊音效：" + (slapSoundsEnabled ? "~g~開啟" : "~r~關閉") + "~s~";
            DrawMenuItem(slapSoundsLabel, idx, x, ref y, lh); idx++;

            string sitAnimLabel = "街頭互動前戲與結束動畫：" + (streetSitAnimEnabled ? "~g~開啟" : "~r~關閉") + "~s~";
            DrawMenuItem(sitAnimLabel, idx, x, ref y, lh); idx++;

            string lightLabel = "互動中開啟車內照明燈：" + (interiorLightEnabled ? "~g~開啟" : "~r~關閉") + "~s~";
            DrawMenuItem(lightLabel, idx, x, ref y, lh); idx++;

            string engineLabel = "互動期間保持引擎發動：" + (carEngineEnabled ? "~g~開啟" : "~r~關閉") + "~s~";
            DrawMenuItem(engineLabel, idx, x, ref y, lh); idx++;

            string driveLabel = !carEngineEnabled
                ? "~c~動畫播放中允許控制車輛：" + (driveableCar ? "開啟" : "關閉") + "~s~"
                : "動畫播放中允許控制車輛：" + (driveableCar ? "~g~開啟" : "~r~關閉") + "~s~";
            DrawMenuItem(driveLabel, idx, x, ref y, lh); idx++;
        }

        private void DrawServicesMenu(float x, ref float y, float lh)
        {
            bool isCar = (mode == Mode.Car);

            DrawSectionHeader("服務", x, ref y);

            // Street mode: show 2 top-level items
            if (!isCar)
            {
                // A-Life exclusive hooker name header
                if (_aLifeHookerServices && aLifeMode && sandboxMode && _currentGirlFp != null)
                {
                    ALifePedData dSvc = null;
                    _aLifePeds.TryGetValue(_currentGirlKey, out dSvc);
                    if (dSvc != null)
                    {
                        // name display removed
                    }
                }
                DrawMenuItem("站姿服務 ~b~[>]~s~", 0, x, ref y, lh);
                DrawMenuItem("坐姿服務 ~b~[>]~s~", 1, x, ref y, lh);
                return;
            }

            // Car mode: check if vehicle supports sex
            Ped player = Game.Player.Character;
            Vehicle currentVeh = player != null && player.IsInVehicle() ? player.CurrentVehicle : null;
            if (isCar && currentVeh != null && !IsSexSuitableVehicle(currentVeh))
            {
                DrawMenuItemCentered("~r~（此載具不支援性愛服務，僅供乘坐與聊天）~s~", 0, x, ref y, lh, 0.24f);
                return;
            }

            // Car mode: flat list
            string[] items = carServiceItems;
            bool backseatSvc = IsBackseatCarMode();

            // A-Life exclusive hooker (Casual/sandbox only): show her personal prices
            if (_aLifeHookerServices && aLifeMode && sandboxMode && _currentGirlFp != null)
            {
                ALifePedData dSvc = null;
                _aLifePeds.TryGetValue(_currentGirlKey, out dSvc);
                if (dSvc != null)
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (backseatSvc && i != 1) continue; // backseat: only Sex
                        int drawIdx = backseatSvc ? 0 : i;
                        string label = items[i];
                        int price = GetALifeHookerPrice(dSvc, isCar, i);
                        if (drawIdx == menuIndex && negotiateMode)
                            label += "  ~o~(Negotiate $" + price + ")~s~";
                        else
                            label += "  ~g~($" + price + ")~s~";
                        DrawMenuItem(label, drawIdx, x, ref y, lh);
                    }
                    return;
                }
            }

            for (int i = 0; i < items.Length; i++)
            {
                if (backseatSvc && i != 1) continue; // backseat: only Sex
                int drawIdx = backseatSvc ? 0 : i;
                string label = items[i];
                if (!sandboxMode)
                {
                    int price;
                    ALifePedData dDraw = null;
                    if (aLifeMode && _currentGirlKey != null && _aLifePeds.TryGetValue(_currentGirlKey, out dDraw) && dDraw != null)
                        price = GetProstHookerPrice(dDraw, isCar, i);
                    else
                        price = GetServicePrice(isCar, i);

                    if (drawIdx == menuIndex && negotiateMode)
                        label += "  ~o~(Negotiate $" + price + ")~s~";
                    else
                        label += "  ~g~($" + price + ")~s~";
                }
                else if (aLifeMode)
                {
                    // Sandbox A-Life: grey out services blocked by personality traits
                    if (!IsServiceAvailable(isCar, i))
                        label = "~c~" + label + "  ~r~(N/A)~s~";
                }
                DrawMenuItem(label, drawIdx, x, ref y, lh);
            }
        }

        private void DrawSitDownSubMenu(float x, ref float y, float lh)
        {
            DrawSectionHeader("服務", x, ref y);
            for (int i = 0; i < sitDownSubItems.Length; i++)
                DrawMenuItem(sitDownSubItems[i] + " ~b~[>]~s~", i, x, ref y, lh);
        }

        private void DrawStreetServiceSubMenu(float x, ref float y, float lh)
        {
            string[] labels;
            int[] map;
            GetSubMenuArrays(out labels, out map);

            string catName;
            if (menuLevel == MenuLevel.StandingServices) catName = "站姿服務";
            else if (menuLevel == MenuLevel.SittingServices) catName = "坐姿服務";
            else catName = "躺姿服務";
            DrawSectionHeader(catName.ToUpper(), x, ref y);

            // A-Life exclusive hooker name header
            ALifePedData dSub = null;
            if (_aLifeHookerServices && aLifeMode && sandboxMode && _currentGirlFp != null)
            {
                _aLifePeds.TryGetValue(_currentGirlKey, out dSub);
                if (dSub != null)
                {
                    // name display removed
                }
            }

            bool isEscalateParent = (_serviceSubParent == MenuLevel.Escalate);

            for (int i = 0; i < labels.Length; i++)
            {
                string label = labels[i];
                int flatIdx = map[i];

                if (isEscalateParent)
                {
                    // Escalate: grey out BJ/Rough prefs
                    ALifePedData dE = null;
                    if (aLifeMode && _currentGirlKey != null)
                        _aLifePeds.TryGetValue(_currentGirlKey, out dE);
                    if (dE != null && dE.PrefBJ == false && IsEscalateBJItem(false, flatIdx))
                        label = "~c~" + label + "  ~r~(N/A)~s~";
                    else if (dE != null && dE.PrefRough == false && IsEscalateRoughItem(false, flatIdx))
                        label = "~c~" + label + "  ~r~(N/A)~s~";
                }
                else
                {
                    // Services parent: show prices or availability
                    if (!sandboxMode)
                    {
                        int price;
                        ALifePedData dDraw = null;
                        if (aLifeMode && _currentGirlKey != null && _aLifePeds.TryGetValue(_currentGirlKey, out dDraw) && dDraw != null)
                            price = GetProstHookerPrice(dDraw, false, flatIdx);
                        else
                            price = GetServicePrice(false, flatIdx);

                        if (i == menuIndex && negotiateMode)
                            label += "  ~o~(Negotiate $" + price + ")~s~";
                        else
                            label += "  ~g~($" + price + ")~s~";
                    }
                    else if (_aLifeHookerServices && aLifeMode && dSub != null)
                    {
                        int price = GetALifeHookerPrice(dSub, false, flatIdx);
                        if (i == menuIndex && negotiateMode)
                            label += "  ~o~(Negotiate $" + price + ")~s~";
                        else
                            label += "  ~g~($" + price + ")~s~";
                    }
                    else if (aLifeMode)
                    {
                        if (!IsServiceAvailable(false, flatIdx))
                            label = "~c~" + label + "  ~r~(N/A)~s~";
                    }
                }
                DrawMenuItem(label, i, x, ref y, lh);
            }
        }

        private void DrawCompatibilityMenu(float x, ref float y, float lh)
        {
            DrawSectionHeader("COWGIRL ANIM BY SINKRA", x, ref y);

            string faceLabel, layLabel, revLabel;
            if (!_sinkraCowgirlAvailable)
            {
                // Addon not installed — force all shown as OFF
                faceLabel = "Replace Facesitting: ~r~關閉（附加模組未安裝）~s~";
                layLabel  = "Replace Laying Cowgirl: ~r~關閉（附加模組未安裝）~s~";
                revLabel  = "Replace Laying Reversed Cowgirl: ~r~關閉（附加模組未安裝）~s~";
            }
            else
            {
                faceLabel = "替換坐姿動畫："             + (sinkraReplaceFacesitting ? "~g~ON" : "~r~OFF") + "~s~";
                layLabel  = "Replace Laying Cowgirl: "          + (sinkraReplaceLaying      ? "~g~ON" : "~r~OFF") + "~s~";
                revLabel  = "Replace Laying Reversed Cowgirl: " + (sinkraReplaceRevLaying   ? "~g~ON" : "~r~OFF") + "~s~";
            }

            DrawMenuItem(faceLabel, 0, x, ref y, lh);
            DrawMenuItem(layLabel,  1, x, ref y, lh);
            DrawMenuItem(revLabel,  2, x, ref y, lh);
        }

        private void DrawSectionHeader(string title, float x, ref float y)
        {
            const float W = 0.216f;
            const float H = 0.027f;
            DrawMenuText(title, x, y + 0.005f, W, 0.27f, true, 255, 210, 0, 255);
            y += H + 0.016f;
        }

        private void DrawMenuRect(float x, float y, float w, float h, int r, int g, int b, int a)
        {
            Function.Call(Hash.DRAW_RECT, x + w * 0.5f, y + h * 0.5f, w, h, r, g, b, a, false);
        }

        private void DrawMenuText(string text, float x, float y, float w, float scale, bool centered, int r, int g, int b, int a)
        {
            DrawMenuText(text, x, y, w, scale, centered, r, g, b, a, 0);
        }

        private void DrawMenuText(string text, float x, float y, float w, float scale, bool centered, int r, int g, int b, int a, int font)
        {
            Function.Call(Hash.SET_TEXT_FONT, font);
            Function.Call(Hash.SET_TEXT_SCALE, scale, scale);
            Function.Call(Hash.SET_TEXT_COLOUR, r, g, b, a);
            Function.Call(Hash.SET_TEXT_WRAP, x, x + w);
            Function.Call(Hash.SET_TEXT_CENTRE, centered);
            Function.Call(Hash.SET_TEXT_DROPSHADOW, 2, 2, 0, 0, 0);
            Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_TEXT, "STRING");
            for (int i = 0; i < text.Length; i += 99)
                Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, text.Substring(i, Math.Min(99, text.Length - i)));
            // When centered, GTA centers around the x passed here — use the mid-point of the wrap range
            float drawX = centered ? x + w * 0.5f : x;
            Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_TEXT, drawX, y, 0);
        }

        private void DrawMenuItem(string text, int itemIdx, float x, ref float y, float lh)
        {
            DrawMenuItem(text, itemIdx, x, ref y, lh, 0.31f);
        }

        private void DrawMenuItem(string text, int itemIdx, float x, ref float y, float lh, float scale)
        {
            // constants matching JewsStrike panel layout
            const float W    = 0.216f;
            const float PAD  = 0.007f;
            const float ITEM_H = 0.034f;

            if (itemIdx == menuIndex)
                DrawMenuRect(x, y, W, ITEM_H, 79, 38, 142, 210);   // purple highlight

            DrawMenuText(text, x + PAD, y + 0.005f, W - PAD * 2f, scale, false, 255, 255, 255, 255);
            y += lh;
        }

        private void DrawMenuItemCentered(string text, int itemIdx, float x, ref float y, float lh, float scale = 0.31f)
        {
            const float W          = 0.216f;
            const float PAD        = 0.007f;
            const float ITEM_H     = 0.034f;
            const float BASE_SCALE = 0.31f;
            const float BASE_YOFF  = 0.005f;
            // Vertically center smaller text: as scale shrinks, push text down proportionally
            float textH  = scale / BASE_SCALE * (ITEM_H - BASE_YOFF * 2f);
            float yOff   = (ITEM_H - textH) / 2f;

            if (itemIdx == menuIndex)
                DrawMenuRect(x, y, W, ITEM_H, 79, 38, 142, 210);

            DrawMenuText(text, x + PAD, y + yOff, W - PAD * 2f, scale, false, 255, 255, 255, 255);
            y += lh;
        }

        /// <summary>Like DrawMenuItem but scales the row height to fit wrapped text.</summary>
        private void DrawMenuItemWrapped(string text, int itemIdx, float x, ref float y, float lh, float scale = 0.31f)
        {
            const float W   = 0.216f;
            const float PAD = 0.007f;
            // Strip GTA color codes (~x~) for length estimation
            string plain = System.Text.RegularExpressions.Regex.Replace(text, "~[^~]+~", "");
            // At scale 0.31 inside W=0.202, roughly 36 chars fit per line (scale * 0.018 per char)
            int charsPerLine = (int)(0.202f / (scale * 0.016f));
            if (charsPerLine < 1) charsPerLine = 1;
            int lines = (plain.Length + charsPerLine - 1) / charsPerLine;
            if (lines < 1) lines = 1;
            float rowH = lh * lines;

            if (itemIdx == menuIndex)
                DrawMenuRect(x, y, W, rowH, 79, 38, 142, 210);

            DrawMenuText(text, x + PAD, y + 0.005f, W - PAD * 2f, scale, false, 255, 255, 255, 255);
            y += rowH;
        }

        /// <summary>Draw text on screen using GTA V natives.</summary>
        private void DrawTextOnScreen(string text, float x, float y)
        {
            Function.Call(Hash.SET_TEXT_FONT, 0);
            Function.Call(Hash.SET_TEXT_SCALE, 0.4f, 0.4f);
            Function.Call(Hash.SET_TEXT_COLOUR, 255, 255, 255, 255);
            Function.Call(Hash.SET_TEXT_WRAP, 0.0f, 1.0f);
            Function.Call(Hash.SET_TEXT_CENTRE, false);
            Function.Call(Hash.SET_TEXT_DROPSHADOW, 2, 2, 0, 0, 0);
            Function.Call(Hash.SET_TEXT_EDGE, 1, 0, 0, 0, 205);
            Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_TEXT, "STRING");
            // Split into 99-char chunks to avoid native string limit
            for (int i = 0; i < text.Length; i += 99)
            {
                string chunk = text.Substring(i, Math.Min(99, text.Length - i));
                Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, chunk);
            }
            Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_TEXT, x, y, 0);
        }

        private void DrawTextOnScreen(string text, float x, float y, float scale, bool centered)
        {
            Function.Call(Hash.SET_TEXT_FONT, 0);
            Function.Call(Hash.SET_TEXT_SCALE, scale, scale);
            Function.Call(Hash.SET_TEXT_COLOUR, 255, 255, 255, 255);
            Function.Call(Hash.SET_TEXT_WRAP, 0.0f, 1.0f);
            Function.Call(Hash.SET_TEXT_CENTRE, centered);
            Function.Call(Hash.SET_TEXT_DROPSHADOW, 2, 2, 0, 0, 0);
            Function.Call(Hash.SET_TEXT_EDGE, 1, 0, 0, 0, 205);
            Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_TEXT, "STRING");
            for (int i = 0; i < text.Length; i += 99)
            {
                string chunk = text.Substring(i, Math.Min(99, text.Length - i));
                Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, chunk);
            }
            Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_TEXT, x, y, 0);
        }

        /// <summary>Show a timed HUD status message (non-dialogue system notifications).</summary>
        private void ShowHudStatus(string text, int durationMs)
        {
            if (!showTutorialMessages) return;
            _hudStatusText    = text;
            _hudStatusExpireAt = Game.GameTime + durationMs;
        }

        /// <summary>Show a timed subtitle (NPC/player dialogue only).</summary>
        private void ShowSubtitle(string text, int duration)
        {
            GTA.UI.Screen.ShowSubtitle(text != null ? text.Replace("\"", "") : text, duration);
        }
    }

    /// <summary>
    /// Fire-and-forget UDP telemetry sender for external device intensity control.
    /// Sends UTF-8 JSON messages (including device token) to 127.0.0.1:28777.
    /// Rate-limited to 8 Hz; dead-zone of 0.03 suppresses redundant updates.
    ///
    /// Pong replies from the bridge on port 28778 are read in Poll(), which is
    /// called from OnTick — everything stays on the SHVDN script thread, no
    /// thread-pool callbacks, no cross-thread access.
    /// </summary>
    /// 
    internal sealed class TelemetrySender : IDisposable
    {
        private const int   SendPort    = 28777;
        private const int   RecvPort    = 28778;
        private const float DeadZone    = 0.03f;
        private const int   RateLimitMs = 125;   // 8 Hz max
        private const int   PongTimeout          = 60000; // ms — no pong within this window = bridge gone
        private const int   DeviceOnlineTimeout  = 12000; // ms — device online status expires without refresh (just above ping interval)

        private readonly UdpClient  _udp;   // unbound send socket
        private readonly UdpClient  _recv;  // non-blocking, bound to RecvPort
        private readonly IPEndPoint _ep;    // 127.0.0.1:28777

        private string _token = "";
        private int _strokeMin = 5;
        private int _strokeMax = 65;
        private int _parkDelay = 2000;

        private float _lastV      = -99f;          // sentinel — first send always goes through
        private int   _lastSentMs = int.MinValue;

        private bool _everPonged = false;
        private int  _lastPongMs = 0;
        private bool _deviceOnline = false;
        private int  _deviceOnlineMs = 0;         // TickCount of last pong with deviceOnline:true
        private bool _hardDisconnected = false; // set by user "disconnect" action; gates all sends

        /// <summary>True if a pong was received within the last 8 seconds.</summary>
        public bool IsConnected
        {
            get { return _everPonged && (Environment.TickCount - _lastPongMs < PongTimeout); }
        }

        /// <summary>True if the last pong indicated device is online AND that confirmation is fresh (< 35s).</summary>
        public bool IsDeviceOnline
        {
            get { return _deviceOnline && (Environment.TickCount - _deviceOnlineMs < DeviceOnlineTimeout); }
        }

        /// <summary>True if the user manually disconnected. All sends except SendPing are blocked.</summary>
        public bool IsHardDisconnected
        {
            get { return _hardDisconnected; }
        }

        public TelemetrySender()
        {
            try
            {
                _ep  = new IPEndPoint(IPAddress.Loopback, SendPort);
                _udp = new UdpClient();
            }
            catch { }

            try
            {
                _recv = new UdpClient(new IPEndPoint(IPAddress.Loopback, RecvPort));
                _recv.Client.Blocking = false; // Poll() will never block the game thread
            }
            catch { /* port 28778 already in use — pong status unavailable */ }
        }

        /// <summary>Set the device token included in all outgoing UDP messages.</summary>
        public void SetToken(string token) { _token = token ?? ""; }

        /// <summary>Set stroke position bounds and finish delay sent in every UDP motion message for the bridge to use.</summary>
        public void SetStrokeRange(int min, int max, int parkDelay) { _strokeMin = min; _strokeMax = max; _parkDelay = parkDelay; }

        /// <summary>Discard any datagrams already sitting in the receive socket buffer so stale pongs
        /// cannot immediately satisfy the next <see cref="IsConnected"/> / <see cref="IsDeviceOnline"/> check.</summary>
        private void FlushRecvBuffer()
        {
            if (_recv == null) return;
            try
            {
                while (_recv.Available > 0)
                {
                    IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
                    _recv.Receive(ref ep); // discard
                }
            }
            catch { }
        }

        /// <summary>Clear pong state so <see cref="IsConnected"/> only turns true when a new pong arrives.</summary>
        public void ResetPong() { _everPonged = false; _lastPongMs = 0; _deviceOnline = false; FlushRecvBuffer(); }

        /// <summary>Manually disconnect: block all sends and clear pong state.</summary>
        public void HardDisconnect() { _hardDisconnected = true; _everPonged = false; _lastPongMs = 0; _deviceOnline = false; FlushRecvBuffer(); }

        /// <summary>Re-enable sends and ping the bridge to reconnect.</summary>
        public void Reconnect() { _hardDisconnected = false; _everPonged = false; _lastPongMs = 0; _deviceOnline = false; FlushRecvBuffer(); SendPing(); }

        // Appends ,"min":...,"max":...,"finishDelay":...,"token":"..." (token only when set).
        private string TokenJson()
        {
            string extra = ",\"min\":" + _strokeMin + ",\"max\":" + _strokeMax + ",\"parkDelay\":" + _parkDelay;
            return extra + (_token.Length > 0 ? ",\"token\":\"" + _token + "\"" : "");
        }

        /// <summary>
        /// Call from OnTick. Drains all pending pong datagrams without ever blocking.
        /// Safe to call every frame — returns immediately when no data is available.
        /// </summary>
        public void Poll()
        {
            if (_recv == null) return;
            try
            {
                while (_recv.Available > 0)
                {
                    IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = _recv.Receive(ref remote);
                    if (Encoding.UTF8.GetString(data).Contains("\"pong\""))
                    {
                        _lastPongMs = Environment.TickCount;
                        _everPonged = true;
                        bool devOnline = Encoding.UTF8.GetString(data).Contains("\"deviceOnline\":true");
                        _deviceOnline = devOnline;
                        if (devOnline) _deviceOnlineMs = Environment.TickCount; // stamp freshness
                    }
                }
            }
            catch { }
        }

        /// <summary>Send a ping; <see cref="IsConnected"/> updates on the next Poll() that receives a pong.</summary>
        public void SendPing()
        {
            if (_udp == null) return;
            string json = "{\"type\":\"ping\"" + TokenJson() + "}";
            byte[] buf = Encoding.UTF8.GetBytes(json);
            try { _udp.Send(buf, buf.Length, _ep); } catch { }
        }

        /// <summary>
        /// Send an intensity update. Silently dropped when BOTH the rate-limit AND
        /// the dead-zone conditions are met. <paramref name="v"/> is clamped to [0, 1].
        /// </summary>
        public void SendIntensity(float v)
        {
            if (_udp == null || _hardDisconnected) return;
            v = Math.Max(0f, Math.Min(1f, v));

            int  now         = Environment.TickCount;
            bool rateLimited = (now - _lastSentMs) < RateLimitMs;
            bool inDeadZone  = Math.Abs(v - _lastV) < DeadZone;
            if (rateLimited && inDeadZone) return;

            _lastV      = v;
            _lastSentMs = now;

            string json = "{\"type\":\"intensity\",\"v\":"
                        + v.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture)
                        + TokenJson() + "}";
            byte[] buf = Encoding.UTF8.GetBytes(json);
            try { _udp.Send(buf, buf.Length, _ep); } catch { }
        }

        /// <summary>
        /// Same as <see cref="SendIntensity"/> but bypasses the dead-zone check.
        /// Use when the player explicitly changes speed so the device responds immediately.
        /// </summary>
        public void ForceSendIntensity(float v)
        {
            if (_hardDisconnected) return;
            _lastV      = -99f;           // reset deadzone so the change always registers
            _lastSentMs = int.MinValue;   // reset rate-limit too
            SendIntensity(v);
        }

        /// <summary>
        /// Send a finish message (natural completion). Bridge will park the device
        /// at the 0 position before halting. Also resets rate-limit state.
        /// </summary>
        public void SendFinish()
        {
            if (_udp == null || _hardDisconnected) return;
            _lastV      = -99f;
            _lastSentMs = int.MinValue;
            string json = "{\"type\":\"finish\"" + TokenJson() + "}";
            byte[] buf = Encoding.UTF8.GetBytes(json);
            try { _udp.Send(buf, buf.Length, _ep); } catch { }
        }

        /// <summary>
        /// Send a stop message and reset rate-limit / dead-zone state so that
        /// the next <see cref="SendIntensity"/> call always fires unconditionally.
        /// </summary>
        public void SendStop()
        {
            if (_udp == null) return;
            _lastV      = -99f;
            _lastSentMs = int.MinValue;
            string json = "{\"type\":\"stop\"" + TokenJson() + "}";
            byte[] buf = Encoding.UTF8.GetBytes(json);
            try { _udp.Send(buf, buf.Length, _ep); } catch { }
        }

        /// <summary>
        /// Send a ready message — bridge will goToPosition(strokeMax) so the device parks at the top/retracted position.
        /// </summary>
        public void SendReady()
        {
            if (_udp == null || _hardDisconnected) return;
            string json = "{\"type\":\"ready\"" + TokenJson() + "}";
            byte[] buf = Encoding.UTF8.GetBytes(json);
            try { _udp.Send(buf, buf.Length, _ep); } catch { }
        }

        /// <summary>
        /// Send a goto message — bridge will call PUT /autoblow/goto to move device to <paramref name="position"/> at <paramref name="speed"/>.
        /// position: 0–100 (0 = fully extended/deep, 100 = fully retracted/top).
        /// speed: 1–100.
        /// </summary>
        public void SendGoto(int position, int speed)
        {
            if (_udp == null || _hardDisconnected) return;
            string json = "{\"type\":\"goto\",\"pos\":" + position + ",\"spd\":" + speed + TokenJson() + "}";
            byte[] buf = Encoding.UTF8.GetBytes(json);
            try { _udp.Send(buf, buf.Length, _ep); } catch { }
        }

        public void Dispose()
        {
            try { if (_udp  != null) _udp.Close();  } catch { }
            try { if (_recv != null) _recv.Close(); } catch { }
        }
    }
}

