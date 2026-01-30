# LuckyCharm - Daily Fortune Cookie App Design

## Overview

A 2D mobile app where users receive a daily fortune cookie at 10 AM local time. They crack it open with multi-tap interaction, unroll the paper with a horizontal swipe, and can share the motivational message to social platforms.

**Platform:** iOS and Android (cross-platform)
**Visual Style:** Realistic/skeuomorphic
**Backend:** None (fully offline, local data)

---

## Core Features

1. **Daily Cookie at 10 AM** - Local notification alerts user when cookie is ready
2. **Multi-tap Cracking** - Tap 3-5 times to break open the cookie
3. **Horizontal Unroll** - Drag finger to reveal the message like real fortune paper
4. **Message History** - Browse past messages with dates
5. **Social Sharing** - Share fortune image to Facebook, Instagram, WhatsApp, etc.
6. **Sound & Haptics** - Immersive feedback for all interactions

---

## Architecture

### Project Structure

```
Assets/
├── Scripts/
│   ├── Core/           # GameManager, NotificationManager, SaveManager
│   ├── UI/             # CookieController, MessageController, HistoryPanel
│   ├── Data/           # FortuneMessage, SaveData, MessageDatabase
│   └── Utils/          # AudioManager, HapticsManager, Extensions
├── Prefabs/            # Cookie, Message, UI elements
├── Sprites/            # 2D textures and atlases
├── Audio/              # Sound effects
├── Scenes/
│   └── MainScene.unity # Single scene app
└── Resources/          # Messages JSON, runtime-loaded assets
```

### Core Managers

| Manager | Responsibility |
|---------|----------------|
| GameManager | App state, daily cookie logic, state machine |
| NotificationManager | Schedule/cancel 10 AM local notifications |
| SaveManager | Persist history, last opened date, settings |
| AudioManager | Sound effects playback, pooling |
| HapticsManager | Vibration feedback (platform-specific) |

### State Machine

```
Waiting (before 10 AM or already opened today)
    ↓ (time >= 10 AM && not opened today)
CookieAvailable (cookie on screen, intact)
    ↓ (tap 3-5 times)
Cracking (cookie breaking animation)
    ↓ (animation complete)
MessageRolled (rolled paper visible)
    ↓ (horizontal drag)
MessageRevealed (text visible, share enabled)
```

### Notification Flow

1. On app launch/close → Schedule notification for next 10 AM (if cookie not opened)
2. OS delivers notification at 10 AM → "Your lucky cookie is ready!"
3. User taps notification → App opens to cookie screen
4. After opening cookie → Cancel today's notification, schedule tomorrow's

---

## UI/UX Flow

### Main Screen (Cookie View)
- Full-screen warm background (wood table texture or soft gradient)
- Fortune cookie centered on screen
- Subtle ambient particles (dust motes, soft light rays)
- If unavailable: dimmed cookie with countdown "Next cookie in X hours"

### Cracking Sequence
- Tap counter: visible cracks appear with each tap (1-5)
- Haptic pulse on each tap, increasing intensity
- "Crack" sound intensifying
- Final tap: cookie splits with particle burst, halves animate off-screen

### Message Reveal
- Rolled paper appears from cookie position
- First-time hint: "Drag to unroll" prompt
- Horizontal drag unrolls paper with parallax effect
- Text fades in as paper unrolls
- Paper rustle sound during drag
- Share button fades in when fully revealed

### History Screen
- Accessed via icon in corner of main screen
- Scrollable list of past messages with dates
- Tap to view full message (read-only)

---

## Data Model

### FortuneMessage
```csharp
[System.Serializable]
public class FortuneMessage
{
    public string id;
    public string text;
    public string category; // "motivation", "wisdom", "humor"
}
```

### SaveData
```csharp
public class SaveData
{
    public string lastCookieDate;        // "2026-01-29"
    public string lastMessageId;         // prevent immediate repeat
    public List<HistoryEntry> history;   // past messages with dates
    public List<string> seenMessageIds;  // no-repeat-until-all-seen logic
    public bool hasSeenTutorial;         // first-time hints
    public bool soundEnabled;            // user preference
    public bool hapticsEnabled;          // user preference
}

public class HistoryEntry
{
    public string date;
    public string messageId;
    public string messageText;
}
```

### Message Storage
- ~150 hardcoded messages in JSON/ScriptableObject
- Random selection daily with no-repeat-until-all-seen logic
- Categories: gratitude, courage, growth, kindness, humor

---

## Sharing System

### Share Image
- Render unrolled fortune paper to RenderTexture
- Add subtle "LuckyCharm" watermark in corner
- Save as PNG (1080x1920, portrait, story-optimized)
- Store in device temporary directory

### Share Flow
1. User taps Share button
2. Brief "Creating image..." indicator
3. Native OS share sheet opens (via NativeShare plugin)
4. Available targets: Facebook, Instagram, WhatsApp, Save to Photos, etc.
5. User selects destination

### Platform Notes
- Instagram/Facebook Stories: Handled via NativeShare intent URIs
- WhatsApp Status: User shares image, manually posts to status
- Download: "Save to Photos" option in native share sheet

---

## Audio & Haptics

### Sound Effects
| Event | File | Description |
|-------|------|-------------|
| Cookie tap (1-4) | crack_light_01-04.wav | Subtle cracking, increasing intensity |
| Cookie break | crack_break.wav | Satisfying snap/crunch |
| Paper appear | paper_whoosh.wav | Soft whoosh |
| Paper unroll | paper_rustle.wav | Gentle rustling (loop during drag) |
| Paper settle | paper_settle.wav | Soft landing sound |
| UI tap | ui_tap.wav | Subtle button feedback |

### Haptics Pattern
| Event | Vibration |
|-------|-----------|
| Cookie tap (1-4) | Light pulse, increasing intensity |
| Cookie break | Strong burst (success) |
| Paper unroll | Subtle continuous during drag |
| Share complete | Medium pulse (confirmation) |

### Implementation
- AudioSource with pooling for rapid taps
- Handheld.Vibrate() or native plugins for nuanced haptics
- Respect device silent mode
- User toggle in settings

---

## Technical Requirements

### Unity Packages
- **com.unity.mobile.notifications** - Local push notifications
- **TextMeshPro** (included) - Crisp text rendering
- **2D Sprite** (included) - Sprite rendering

### Third-Party (Free)
- **NativeShare** - Native share sheet (GitHub)
- **DOTween Free** - Smooth animations

### Build Settings
- iOS: 13.0+
- Android: API 24+ (Android 7.0)
- Orientation: Portrait locked
- Canvas Scaler: Scale with Screen Size

### Permissions
- iOS: User Notifications
- Android: POST_NOTIFICATIONS (Android 13+)

---

## Asset Requirements

### Sprites
- Fortune cookie (intact, multiple crack states, broken halves)
- Fortune paper (rolled, unrolling frames or mesh, unrolled)
- Background texture (warm wood or gradient)
- UI elements (share button, history icon, close button)
- Particle effects (cookie crumbs, dust motes, light rays)

### Audio
- 6-8 sound effect files (see Audio section)

### Fonts
- Handwritten or typewriter style for fortune text
- Clean sans-serif for UI elements

---

## Future Considerations (Out of Scope for MVP)

- Themes/skins for cookie and paper
- Widget showing countdown or today's message
- Apple Watch / WearOS companion
- CloudKit/Google Drive sync for history
- Daily streaks and achievements
