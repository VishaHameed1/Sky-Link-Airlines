using System;
using System.Collections.Generic;

namespace VISHA_HAMEED.Services
{
    public class LocalizationService
    {
        private string currentLanguage = "English";

        private Dictionary<string, Dictionary<string, string>> translations = new Dictionary<string, Dictionary<string, string>>
        {
            ["English"] = new Dictionary<string, string>
            {
                ["Welcome"] = "Welcome to Sky-Link Airlines",
                ["Booking"] = "Booking",
                ["Cancel"] = "Cancel",
                ["Search"] = "Search",
                ["View"] = "View",
                ["Back"] = "Back",
                ["Exit"] = "Exit",
                ["Confirm"] = "Confirm",
                ["Payment"] = "Payment",
                ["Success"] = "Success",
                ["Error"] = "Error",
                ["PleaseWait"] = "Please wait..."
            },
            ["Urdu"] = new Dictionary<string, string>
            {
                ["Welcome"] = "سکائی لنک ایئر لائنز میں خوش آمدید",
                ["Booking"] = "بکنگ",
                ["Cancel"] = "منسوخ کریں",
                ["Search"] = "تلاش کریں",
                ["View"] = "دیکھیں",
                ["Back"] = "واپس",
                ["Exit"] = "باہر جائیں",
                ["Confirm"] = "تصدیق کریں",
                ["Payment"] = "ادائیگی",
                ["Success"] = "کامیاب",
                ["Error"] = "خرابی",
                ["PleaseWait"] = "براہ کرم انتظار کریں..."
            },
            ["Arabic"] = new Dictionary<string, string>
            {
                ["Welcome"] = "مرحباً بكم في خطوط سكاي لينك الجوية",
                ["Booking"] = "حجز",
                ["Cancel"] = "إلغاء",
                ["Search"] = "بحث",
                ["View"] = "عرض",
                ["Back"] = "رجوع",
                ["Exit"] = "خروج",
                ["Confirm"] = "تأكيد",
                ["Payment"] = "دفع",
                ["Success"] = "نجاح",
                ["Error"] = "خطأ",
                ["PleaseWait"] = "يرجى الانتظار..."
            }
        };

        public void SetLanguage(string language)
        {
            if (translations.ContainsKey(language))
                currentLanguage = language;
        }

        public string GetText(string key)
        {
            if (translations[currentLanguage].ContainsKey(key))
                return translations[currentLanguage][key];
            return translations["English"].GetValueOrDefault(key, key);
        }

        public string FormatDate(DateTime date)
        {
            if (currentLanguage == "English")
                return date.ToString("dd/MM/yyyy");
            else if (currentLanguage == "Urdu")
                return $"{date.Day}/{date.Month}/{date.Year}";
            else
                return date.ToString("dd/MM/yyyy");
        }

        public string FormatCurrency(decimal amount)
        {
            if (currentLanguage == "English")
                return $"${amount:F2}";
            else if (currentLanguage == "Urdu")
                return $"${amount:F2}";
            else
                return $"${amount:F2}";
        }
    }
}