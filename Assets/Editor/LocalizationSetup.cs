using UnityEngine;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using System.IO;
using System.Collections.Generic;

namespace LuckyCharm.Editor
{
    public static class LocalizationSetup
    {
        [MenuItem("LuckyCharm/Setup Localization")]
        public static void Setup()
        {
            CreateFolders();
            var settings = CreateLocalizationSettings();
            var enLocale = CreateLocale("en", "English");
            var esLocale = CreateLocale("es", "Spanish");

            var availableLocales = settings.GetAvailableLocales();
            if (!availableLocales.Locales.Contains(enLocale))
                availableLocales.AddLocale(enLocale);
            if (!availableLocales.Locales.Contains(esLocale))
                availableLocales.AddLocale(esLocale);

            CreateStringTables(enLocale, esLocale);

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            UnityEngine.Debug.Log("Localization setup complete!");
        }

        private static void CreateFolders()
        {
            string[] folders = {
                "Assets/Localization",
                "Assets/Localization/Locales",
                "Assets/Localization/Tables"
            };

            foreach (var folder in folders)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    var parent = Path.GetDirectoryName(folder).Replace("\\", "/");
                    var name = Path.GetFileName(folder);
                    AssetDatabase.CreateFolder(parent, name);
                }
            }
        }

        private static LocalizationSettings CreateLocalizationSettings()
        {
            const string path = "Assets/Localization/LocalizationSettings.asset";

            var settings = AssetDatabase.LoadAssetAtPath<LocalizationSettings>(path);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<LocalizationSettings>();
                AssetDatabase.CreateAsset(settings, path);
                LocalizationEditorSettings.ActiveLocalizationSettings = settings;
            }
            return settings;
        }

        private static Locale CreateLocale(string code, string name)
        {
            string path = $"Assets/Localization/Locales/{code}.asset";

            var locale = AssetDatabase.LoadAssetAtPath<Locale>(path);
            if (locale == null)
            {
                locale = Locale.CreateLocale(new UnityEngine.Localization.LocaleIdentifier(code));
                locale.name = name;
                AssetDatabase.CreateAsset(locale, path);
            }
            return locale;
        }

        private static void CreateStringTables(Locale enLocale, Locale esLocale)
        {
            CreateUITable(enLocale, esLocale);
            CreateMessagesTable(enLocale, esLocale);
            CreateFortunesTable(enLocale, esLocale);
        }

        private static void CreateUITable(Locale enLocale, Locale esLocale)
        {
            var entries = new Dictionary<string, (string en, string es)>
            {
                { "settings_title", ("Settings", "Configuración") },
                { "language_label", ("Language", "Idioma") },
                { "history_title", ("History", "Historial") },
                { "close_button", ("Close", "Cerrar") },
                { "share_button", ("Share", "Compartir") },
                { "select_language_title", ("Select Language", "Seleccionar Idioma") },
                { "confirm_button", ("Confirm", "Confirmar") },
                { "english", ("English", "Inglés") },
                { "spanish", ("Spanish", "Español") },
                { "tap_cookie", ("Tap the cookie!", "¡Toca la galleta!") },
                { "swipe_to_unroll", ("Swipe to unroll", "Desliza para desenrollar") }
            };

            CreateTable("UI", entries, enLocale, esLocale);
        }

        private static void CreateMessagesTable(Locale enLocale, Locale esLocale)
        {
            var entries = new Dictionary<string, (string en, string es)>
            {
                { "countdown_hours", ("Next cookie in\n{0}h {1}m", "Siguiente galleta en\n{0}h {1}m") },
                { "countdown_minutes", ("Next cookie in\n{0}m {1}s", "Siguiente galleta en\n{0}m {1}s") },
                { "countdown_seconds", ("Next cookie in\n{0}s", "Siguiente galleta en\n{0}s") },
                { "notification_title", ("Your Lucky Cookie is Ready!", "¡Tu Galleta de la Suerte está Lista!") },
                { "notification_body", ("Come crack open today's fortune and discover your message.", "Ven a abrir la fortuna de hoy y descubre tu mensaje.") },
                { "share_subject", ("My Lucky Cookie Fortune", "Mi Fortuna de la Galleta") },
                { "share_text", ("\"{0}\"\n\nShared from LuckyCharm", "\"{0}\"\n\nCompartido desde LuckyCharm") }
            };

            CreateTable("Messages", entries, enLocale, esLocale);
        }

        private static void CreateFortunesTable(Locale enLocale, Locale esLocale)
        {
            var entries = new Dictionary<string, (string en, string es)>
            {
                { "fortune_1", ("The best time to plant a tree was 20 years ago. The second best time is now.", "El mejor momento para plantar un árbol fue hace 20 años. El segundo mejor momento es ahora.") },
                { "fortune_2", ("Your kindness is a gift that keeps giving.", "Tu amabilidad es un regalo que sigue dando.") },
                { "fortune_3", ("Courage is not the absence of fear, but action in spite of it.", "El coraje no es la ausencia de miedo, sino actuar a pesar de él.") },
                { "fortune_4", ("Today's struggle is tomorrow's strength.", "La lucha de hoy es la fuerza de mañana.") },
                { "fortune_5", ("A smile is a curve that sets everything straight.", "Una sonrisa es una curva que lo endereza todo.") },
                { "fortune_6", ("The journey of a thousand miles begins with a single step.", "El viaje de mil millas comienza con un solo paso.") },
                { "fortune_7", ("Be the reason someone believes in the goodness of people.", "Sé la razón por la que alguien cree en la bondad de las personas.") },
                { "fortune_8", ("What you seek is seeking you.", "Lo que buscas te está buscando.") },
                { "fortune_9", ("Stars can't shine without darkness.", "Las estrellas no pueden brillar sin oscuridad.") },
                { "fortune_10", ("Your potential is endless. Go do what you were created to do.", "Tu potencial es infinito. Ve y haz aquello para lo que fuiste creado.") },
                { "fortune_11", ("The only way to do great work is to love what you do.", "La única forma de hacer un gran trabajo es amar lo que haces.") },
                { "fortune_12", ("Small acts of kindness create ripples of change.", "Pequeños actos de bondad crean ondas de cambio.") },
                { "fortune_13", ("You are braver than you believe, stronger than you seem.", "Eres más valiente de lo que crees, más fuerte de lo que pareces.") },
                { "fortune_14", ("Every expert was once a beginner.", "Todo experto fue una vez un principiante.") },
                { "fortune_15", ("Life is short. Smile while you still have teeth.", "La vida es corta. Sonríe mientras aún tengas dientes.") },
                { "fortune_16", ("Believe you can and you're halfway there.", "Cree que puedes y ya estás a mitad de camino.") },
                { "fortune_17", ("In a world where you can be anything, be kind.", "En un mundo donde puedes ser cualquier cosa, sé amable.") },
                { "fortune_18", ("Fortune favors the bold.", "La fortuna favorece a los audaces.") },
                { "fortune_19", ("Growth is painful. Change is painful. But nothing is as painful as staying stuck.", "Crecer es doloroso. Cambiar es doloroso. Pero nada es tan doloroso como quedarse estancado.") },
                { "fortune_20", ("Life is too important to be taken seriously.", "La vida es demasiado importante para tomársela en serio.") },
                { "fortune_21", ("Your only limit is your mind.", "Tu único límite es tu mente.") },
                { "fortune_22", ("Happiness is found when you stop comparing yourself to others.", "La felicidad se encuentra cuando dejas de compararte con los demás.") },
                { "fortune_23", ("The universe is conspiring in your favor.", "El universo está conspirando a tu favor.") },
                { "fortune_24", ("Be fearless in the pursuit of what sets your soul on fire.", "Sé intrépido en la búsqueda de lo que enciende tu alma.") },
                { "fortune_25", ("You are allowed to be both a masterpiece and a work in progress.", "Tienes permiso de ser tanto una obra maestra como un trabajo en progreso.") },
                { "fortune_26", ("The sun will rise and we will try again.", "El sol saldrá y lo intentaremos de nuevo.") },
                { "fortune_27", ("Your energy introduces you before you even speak.", "Tu energía te presenta antes de que hables.") },
                { "fortune_28", ("Do something today that your future self will thank you for.", "Haz algo hoy por lo que tu yo del futuro te agradecerá.") },
                { "fortune_29", ("It's okay to not be okay, as long as you don't stay that way.", "Está bien no estar bien, mientras no te quedes así.") },
                { "fortune_30", ("Throw kindness around like confetti.", "Esparce amabilidad como confeti.") },
                { "fortune_31", ("The best view comes after the hardest climb.", "La mejor vista viene después de la subida más difícil.") },
                { "fortune_32", ("You don't have to be perfect to be amazing.", "No tienes que ser perfecto para ser increíble.") },
                { "fortune_33", ("Today is a good day to have a good day.", "Hoy es un buen día para tener un buen día.") },
                { "fortune_34", ("Your vibe attracts your tribe.", "Tu vibra atrae a tu tribu.") },
                { "fortune_35", ("Be a voice, not an echo.", "Sé una voz, no un eco.") },
                { "fortune_36", ("Difficult roads often lead to beautiful destinations.", "Los caminos difíciles a menudo llevan a destinos hermosos.") },
                { "fortune_37", ("You are capable of more than you know.", "Eres capaz de más de lo que sabes.") },
                { "fortune_38", ("Life begins at the end of your comfort zone.", "La vida comienza al final de tu zona de confort.") },
                { "fortune_39", ("The comeback is always stronger than the setback.", "El regreso siempre es más fuerte que el retroceso.") },
                { "fortune_40", ("Make today so awesome that yesterday gets jealous.", "Haz que hoy sea tan increíble que el ayer tenga envidia.") },
                { "fortune_41", ("You are never too old to set another goal or dream a new dream.", "Nunca eres demasiado viejo para fijarte otra meta o soñar un nuevo sueño.") },
                { "fortune_42", ("Kindness is free. Sprinkle it everywhere.", "La amabilidad es gratis. Espárcela por todas partes.") },
                { "fortune_43", ("What lies behind us and what lies before us are tiny matters compared to what lies within us.", "Lo que está detrás y lo que está delante de nosotros son pequeñeces comparado con lo que está dentro de nosotros.") },
                { "fortune_44", ("Take the risk or lose the chance.", "Toma el riesgo o pierde la oportunidad.") },
                { "fortune_45", ("Bloom where you are planted.", "Florece donde estés plantado.") },
                { "fortune_46", ("Good things come to those who hustle.", "Las cosas buenas llegan a quienes se esfuerzan.") },
                { "fortune_47", ("Let your faith be bigger than your fear.", "Deja que tu fe sea más grande que tu miedo.") },
                { "fortune_48", ("The only impossible journey is the one you never begin.", "El único viaje imposible es el que nunca comienzas.") },
                { "fortune_49", ("You are the author of your own story.", "Eres el autor de tu propia historia.") },
                { "fortune_50", ("Dream big. Work hard. Stay humble.", "Sueña en grande. Trabaja duro. Mantente humilde.") }
            };

            CreateTable("Fortunes", entries, enLocale, esLocale);
        }

        private static void CreateTable(string tableName, Dictionary<string, (string en, string es)> entries, Locale enLocale, Locale esLocale)
        {
            var collection = LocalizationEditorSettings.GetStringTableCollection(tableName);

            if (collection == null)
            {
                collection = LocalizationEditorSettings.CreateStringTableCollection(tableName, "Assets/Localization/Tables");
            }

            // Ensure tables exist for both locales
            var enTable = collection.GetTable(enLocale.Identifier) as StringTable;
            var esTable = collection.GetTable(esLocale.Identifier) as StringTable;

            if (enTable == null)
            {
                collection.AddNewTable(enLocale.Identifier);
                enTable = collection.GetTable(enLocale.Identifier) as StringTable;
            }

            if (esTable == null)
            {
                collection.AddNewTable(esLocale.Identifier);
                esTable = collection.GetTable(esLocale.Identifier) as StringTable;
            }

            foreach (var entry in entries)
            {
                if (!collection.SharedData.Contains(entry.Key))
                {
                    collection.SharedData.AddKey(entry.Key);
                }

                if (enTable != null && enTable.GetEntry(entry.Key) == null)
                    enTable.AddEntry(entry.Key, entry.Value.en);

                if (esTable != null && esTable.GetEntry(entry.Key) == null)
                    esTable.AddEntry(entry.Key, entry.Value.es);
            }

            EditorUtility.SetDirty(collection);
            EditorUtility.SetDirty(collection.SharedData);
            if (enTable != null) EditorUtility.SetDirty(enTable);
            if (esTable != null) EditorUtility.SetDirty(esTable);
        }
    }
}