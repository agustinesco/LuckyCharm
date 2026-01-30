# Localization System Design

## Overview

Add multi-language support to LuckyCharm with English and Spanish. Uses Unity's built-in Localization package with auto-detection and first-launch language selection.

## User Flow

1. First launch: detect system language, show confirmation popup with detected language pre-selected
2. User confirms or changes language preference
3. Preference saved to PlayerPrefs, all UI updates to selected language
4. Settings panel (gear icon) allows changing language anytime

## Architecture

### Package Dependency

- `com.unity.localization` (latest stable via Package Manager)

### String Tables

**UI Table:**
| Key | English | Spanish |
|-----|---------|---------|
| `settings_title` | Settings | Configuración |
| `language_label` | Language | Idioma |
| `history_title` | History | Historial |
| `close_button` | Close | Cerrar |
| `share_button` | Share | Compartir |
| `select_language_title` | Select Language | Seleccionar Idioma |
| `confirm_button` | Confirm | Confirmar |

**Messages Table:**
| Key | English | Spanish |
|-----|---------|---------|
| `countdown_hours` | Next cookie in\n{0}h {1}m | Siguiente galleta en\n{0}h {1}m |
| `countdown_minutes` | Next cookie in\n{0}m {1}s | Siguiente galleta en\n{0}m {1}s |
| `countdown_seconds` | Next cookie in\n{0}s | Siguiente galleta en\n{0}s |
| `notification_title` | Your Lucky Cookie is Ready! | ¡Tu Galleta de la Suerte está Lista! |
| `notification_body` | Come crack open today's fortune and discover your message. | Ven a abrir la fortuna de hoy y descubre tu mensaje. |
| `share_subject` | My Lucky Cookie Fortune | Mi Fortuna de la Galleta |
| `share_text` | "{0}"\n\nShared from LuckyCharm | "{0}"\n\nCompartido desde LuckyCharm |

**Fortunes Table:**
- Keys: `fortune_1` through `fortune_50`
- Contains all 50 fortune messages in both languages

### New Scripts

**LocalizationManager.cs** (Core):
- Singleton under Managers GameObject
- Detects system language on first launch
- Persists selection in PlayerPrefs (`selected_locale`)
- Exposes `SetLanguage(string localeCode)` method
- Fires `OnLanguageChanged` event for reactive updates

**LocalizedText.cs** (UI):
- Attach to TextMeshProUGUI components
- Inspector fields: `tableReference`, `entryKey`
- Auto-updates text when language changes

**SettingsPanel.cs** (UI):
- Controls settings panel visibility
- Language dropdown triggers `LocalizationManager.SetLanguage()`

**LanguageSelectionPopup.cs** (UI):
- First-launch modal
- Two language buttons with flags
- Confirm button saves selection and closes

### Modified Scripts

- `WaitingOverlay.cs` - Use localized countdown strings with placeholders
- `NotificationManager.cs` - Fetch localized title/body from Messages table
- `ShareManager.cs` - Fetch localized share text from Messages table
- `MessageController.cs` - Get fortune text from Fortunes table by message ID

### UI Elements

**Settings Button:**
- Gear icon in top-right corner of main Canvas
- Opens settings panel on click

**Settings Panel:**
- Modal overlay with semi-transparent background
- Title, language dropdown, close button

**First-Launch Popup:**
- Centered modal
- Title, two language buttons (flag + name), confirm button
- Pre-selects detected system language

### File Structure

```
Assets/
├── Localization/
│   ├── Locales/
│   │   ├── en.asset
│   │   └── es.asset
│   ├── Tables/
│   │   ├── UI.asset
│   │   ├── Messages.asset
│   │   └── Fortunes.asset
│   └── LocalizationSettings.asset
├── Scripts/
│   ├── Core/
│   │   └── LocalizationManager.cs
│   └── UI/
│       ├── LocalizedText.cs
│       ├── SettingsPanel.cs
│       └── LanguageSelectionPopup.cs
```

## Implementation Notes

- Fortune messages need Spanish translations for all 50 entries
- Use Smart Strings for countdown text with `{0}`, `{1}` placeholders
- LocalizedText component replaces hardcoded strings in scene
- Notification text fetched at schedule time (respects current language)
- History entries store raw message text (not localized keys) since they're historical records
