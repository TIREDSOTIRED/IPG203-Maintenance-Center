using System.Globalization;
using System.Text;

namespace مركز_صيانة_الأجهزة;

internal static class Program
{
    private static readonly CultureInfo الثقافة_العامة = CultureInfo.InvariantCulture;

    private static void Main()
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        string مسار_البيانات = Path.Combine(Directory.GetCurrentDirectory(), "بيانات", "الأجهزة.json");
        var المستودع = new مستودع_الأجهزة_Json(مسار_البيانات);
        var المركز = new مركز_الصيانة();

        المركز.تنبيه_الصيانة += الرسالة =>
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n[تنبيه صيانة] {الرسالة}");
            Console.ResetColor();
        };

        try
        {
            foreach (جهاز_منزلي جهاز in المستودع.تحميل())
                المركز.إضافة_جهاز(جهاز);

            int عدد_المتأخرة = المركز.مراجعة_الأجهزة_المتأخرة(DateTime.Today);
            if (عدد_المتأخرة > 0)
                المستودع.حفظ(المركز.الأجهزة);
        }
        catch (Exception خطأ)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"تعذر تحميل ملف البيانات: {خطأ.Message}");
            Console.ResetColor();
            Console.WriteLine("أُوقف التشغيل لحماية السجلات من الاستبدال ببيانات غير مكتملة. صحح ملف البيانات ثم أعد التشغيل.");
            return;
        }

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("==================================================");
            Console.WriteLine("       نظام إدارة مركز صيانة الأجهزة المنزلية");
            Console.WriteLine("==================================================");
            Console.WriteLine("1. عرض جميع الأجهزة");
            Console.WriteLine("2. تسجيل جهاز جديد");
            Console.WriteLine("3. تحديث حالة جهاز");
            Console.WriteLine("4. البحث عن جهاز بالرقم التسلسلي");
            Console.WriteLine("5. عرض ملخص المركز والتكاليف");
            Console.WriteLine("0. حفظ البيانات والخروج");
            Console.Write("اختر العملية: ");

            string اختيار = Console.ReadLine()?.Trim() ?? string.Empty;
            try
            {
                switch (اختيار)
                {
                    case "1": عرض_الأجهزة(المركز); break;
                    case "2": إضافة_جهاز(المركز, المستودع); break;
                    case "3": تحديث_حالة(المركز, المستودع); break;
                    case "4": البحث_عن_جهاز(المركز); break;
                    case "5": عرض_الملخص(); break;
                    case "0":
                        المستودع.حفظ(المركز.الأجهزة);
                        Console.WriteLine("تم حفظ البيانات. إلى اللقاء.");
                        return;
                    default:
                        Console.WriteLine("الاختيار غير صحيح. حاول مرة أخرى.");
                        break;
                }
            }
            catch (Exception خطأ)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"تعذر تنفيذ العملية: {خطأ.Message}");
                Console.ResetColor();
            }

            Console.WriteLine();
            Console.Write("اضغط Enter للعودة إلى القائمة الرئيسية...");
            Console.ReadLine();
        }
    }

    private static void عرض_الأجهزة(مركز_الصيانة المركز)
    {
        if (المركز.الأجهزة.Count == 0)
        {
            Console.WriteLine("لا توجد أجهزة مسجلة حتى الآن.");
            return;
        }

        foreach (جهاز_منزلي جهاز in المركز.الأجهزة)
            جهاز.عرض_البيانات();
    }

    private static void إضافة_جهاز(مركز_الصيانة المركز, مستودع_الأجهزة_Json المستودع)
    {
        Console.WriteLine("\n--- تسجيل جهاز جديد ---");
        Console.WriteLine("1. ثلاجة    2. غسالة    3. مكيف");
        int النوع = قراءة_عدد_صحيح("نوع الجهاز: ", 1, 3);
        string الرقم = قراءة_نص("الرقم التسلسلي: ");
        string العميل = قراءة_نص("اسم العميل: ");
        string الهاتف = قراءة_نص("رقم الهاتف: ");
        string العطل = قراءة_نص("وصف العطل: ");
        decimal القطع = قراءة_تكلفة("تكلفة قطع الغيار: ");
        decimal الأجور = قراءة_تكلفة("أجور العمل: ");
        string تسمية_الإضافي = النوع switch
        {
            1 => "تكلفة أعمال نظام التبريد",
            2 => "تكلفة أعمال المحرك",
            _ => "تكلفة خدمة غاز التبريد"
        };
        decimal الإضافي = قراءة_تكلفة($"{تسمية_الإضافي}: ");
        DateTime الاستلام = قراءة_تاريخ("تاريخ الاستلام (YYYY-MM-DD، فارغ = اليوم): ", DateTime.Today);
        DateTime التسليم = قراءة_تاريخ("موعد التسليم المتوقع (YYYY-MM-DD، فارغ = بعد 3 أيام): ", الاستلام.AddDays(3));

        جهاز_منزلي الجهاز = النوع switch
        {
            1 => new ثلاجة(الرقم, العميل, الهاتف, العطل, القطع, الأجور, الإضافي, الاستلام, التسليم),
            2 => new غسالة(الرقم, العميل, الهاتف, العطل, القطع, الأجور, الإضافي, الاستلام, التسليم),
            _ => new مكيف(الرقم, العميل, الهاتف, العطل, القطع, الأجور, الإضافي, الاستلام, التسليم)
        };

        المركز.إضافة_جهاز(الجهاز);
        المستودع.حفظ(المركز.الأجهزة);
        Console.WriteLine("تم تسجيل الجهاز وحفظ البيانات بنجاح.");
    }

    private static void تحديث_حالة(مركز_الصيانة المركز, مستودع_الأجهزة_Json المستودع)
    {
        Console.WriteLine("\n--- تحديث حالة الصيانة ---");
        string الرقم = قراءة_نص("الرقم التسلسلي للجهاز: ");
        Console.WriteLine("1. مستلم  2. قيد الفحص  3. قيد الإصلاح");
        Console.WriteLine("4. جاهز للتسليم  5. تم التسليم  6. متأخر");
        int رقم_الحالة = قراءة_عدد_صحيح("الحالة الجديدة: ", 1, 6);
        var الحالة = (حالة_الصيانة)رقم_الحالة;

        المركز.تغيير_حالة_الجهاز(الرقم, الحالة);
        المستودع.حفظ(المركز.الأجهزة);
        Console.WriteLine("تم تحديث الحالة وحفظ التغيير.");
    }

    private static void البحث_عن_جهاز(مركز_الصيانة المركز)
    {
        string الرقم = قراءة_نص("أدخل الرقم التسلسلي: ");
        جهاز_منزلي? الجهاز = المركز.البحث_عن_جهاز(الرقم);
        if (الجهاز is null)
            Console.WriteLine("لم يتم العثور على الجهاز.");
        else
            الجهاز.عرض_البيانات();
    }

    private static void عرض_الملخص()
    {
        Console.WriteLine("\n--- ملخص المركز ---");
        Console.WriteLine($"عدد الأجهزة المسجلة: {مركز_الصيانة.عدد_الأجهزة_المسجلة}");
        Console.WriteLine($"عدد الأجهزة قيد الإصلاح: {مركز_الصيانة.عدد_الأجهزة_قيد_الإصلاح}");
        Console.WriteLine($"إجمالي تكاليف الإصلاح المقدرة: {مركز_الصيانة.إجمالي_تكاليف_الإصلاحات:N2} وحدة نقدية");
    }

    private static string قراءة_نص(string الرسالة)
    {
        Console.Write(الرسالة);
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    private static int قراءة_عدد_صحيح(string الرسالة, int الحد_الأدنى, int الحد_الأعلى)
    {
        while (true)
        {
            Console.Write(الرسالة);
            string قيمة = Console.ReadLine()?.Trim() ?? string.Empty;
            if (int.TryParse(قيمة, out int رقم) && رقم >= الحد_الأدنى && رقم <= الحد_الأعلى)
                return رقم;

            Console.WriteLine($"أدخل رقماً بين {الحد_الأدنى} و{الحد_الأعلى}.");
        }
    }

    private static decimal قراءة_تكلفة(string الرسالة)
    {
        while (true)
        {
            Console.Write(الرسالة);
            string قيمة = Console.ReadLine()?.Trim() ?? string.Empty;
            bool صالح = decimal.TryParse(قيمة, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal مبلغ)
                        || decimal.TryParse(قيمة, NumberStyles.Number, الثقافة_العامة, out مبلغ);

            if (صالح && مبلغ >= 0)
                return مبلغ;

            Console.WriteLine("أدخل تكلفة رقمية صحيحة لا تقل عن صفر.");
        }
    }

    private static DateTime قراءة_تاريخ(string الرسالة, DateTime القيمة_الافتراضية)
    {
        while (true)
        {
            Console.Write(الرسالة);
            string قيمة = Console.ReadLine()?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(قيمة))
                return القيمة_الافتراضية.Date;

            if (DateTime.TryParseExact(قيمة, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out DateTime تاريخ)
                || DateTime.TryParseExact(قيمة, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out تاريخ))
                return تاريخ.Date;

            Console.WriteLine("صيغة التاريخ غير صحيحة. استخدم YYYY-MM-DD أو DD/MM/YYYY.");
        }
    }
}
