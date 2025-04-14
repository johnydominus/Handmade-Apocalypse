# Handmade Apocalypse

**Try Not To Waste the Planet**  
_A cooperative turn-based strategy card game where players must manage resources and coordinate efforts to stop global catastrophes._

---

## 🧩 Game Overview

- **Genre**: Cooperative Turn-Based Strategy (2–4 players, currently 2)
- **Engine**: Unity (2022.3, tested with Unity 6)
- **Style**: Pixel art or stylized low-poly (TBD)
- **Status**: In development — prototype phase

Each player controls a region and contributes to global survival by using cards, tokens, and investments to fight off rising threats such as:
- 🌡 Global Warming
- ☣️ Pandemic
- ☄️ Asteroid
- 🔒 (Others planned: Hunger, Nuclear War, Dark Ages)

Victory = All threats eliminated  
Loss = Any threat reaches 100%

---

## 📂 Project Structure

Assets/ 
│ ├── Scripts/ # Gameplay systems 
│ ├── Core/ # GameManager, TurnManager, TokenManager, etc. 
│ ├── Cards/ # CardSpawner, CardDisplay, CardData 
│ ├── Threats/ # ThreatManager, ThreatData 
│ ├── UI/ # RegionUI, TokenUI, InvestmentForecastDisplay 
│ └── Data/ # PlayerController, CardLibrary, Enums 
│ ├── Prefabs/ # Cards, UI panels, token displays 
├── Resources/ # ScriptableObjects (Events, Configs) 
├── Scenes/ # Main prototype scene 
└── ... (Art, Fonts, etc.)


---

## ⚙️ Architecture Overview

This project uses a **modular Event-Driven architecture**:

- 🔧 **Loose coupling via ScriptableObject event channels**
- 📡 **Broadcast-driven systems** (e.g., `OnTurnEnded`, `OnThreatChanged`)
- 🧠 **Service Locator pattern** (ScriptableObject-based) for global managers
- 🔁 Modular, testable, designer-friendly structure

See [`ARCHITECTURE.md`](./ARCHITECTURE.md) and [`IMPLEMENTATION_GUIDE.md`](./IMPLEMENTATION_GUIDE.md) for details.

---

## 🚧 Current Milestone: Prototype Scope

See [`HA-Production Phases-Prototype 1 scope.pdf`](./HA-Production%20Phases-Prototype%201%20scope.pdf)

- ✅ Player turns (sequential)
- ✅ Card drawing and playing system
- ✅ Token economy and investing
- ✅ Threat system (3 threats in prototype)
- ✅ State of Emergency UI (partially functional)
- 🔄 Work in progress: dividends system, forecasting UI

---

## ▶️ Getting Started

### Requirements:
- Unity 2022.3 or Unity 6 (LTS)
- Git (for version control)

### Setup:
1. Clone or download the repository
2. Open the project in Unity
3. Open `Scenes/Prototype.unity`
4. Hit Play and run a 2-player game loop

---

## 💡 Dev Notes

- Cards are currently driven by `CardData` from a central `CardLibrary` (editable)
- UI is being transitioned to modular prefabs
- Threat logic and SoEs are managed independently per region
- Investments and dividends are under active refinement

---

## 📣 Communication & Management

- Trello board planned for tracking sprints and bugs
- DevLog: `HA-DevLog.md` (not public yet)
- Core documents in `/Docs/` folder:
  - GDDs, System breakdowns, Cards, Resources, SoEs, Ideas

---

## 📬 Contributors

| Role              | Name               |
|-------------------|--------------------|
| Game Designer     | Yevhen Liakhovych  |
| Assistant GPT     | Handmade Apocalypse Co-Developer (Custom GPT) |

---

## 🗺 Roadmap (Mid-Term)

- [ ] Finalize 3-threat prototype
- [ ] Polish UI for SoEs and investments
- [ ] Add bots or hotseat multiplayer
- [ ] Create vertical slice with all core systems
- [ ] Build Steam page + trailer
- [ ] Begin publisher or funding outreach

---

## 🧠 Contact / Support

For collaborators or inquiries, please contact:  
📧 `johnydsound@gmail.com`  
🧠 See the full assistant context via GPT project: *Handmade Apocalypse Assistant*

---