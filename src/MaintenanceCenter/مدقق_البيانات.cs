using System.Text.RegularExpressions;

namespace مركز_صيانة_الأجهزة;

/// <summary>
/// دوال تحقق مشتركة. لا تحتاج هذه الفئة إلى إنشاء كائن منها.
/// </summary>
public static class مدقق_البيانات
{
    private static readonly Regex نمط_الرقم_التسلسلي =
        new(@"^[\p{L}\p{N}_-]{4,30}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex نمط_الهاتف =
        new(@"^[0-9\u0660-\u0669+()\-\s]{6,20}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static string تحقق_من_النص(string? القيمة, string اسم_الحقل)
    {
        if (string.IsNullOrWhiteSpace(القيمة))
            throw new ArgumentException($"الحقل «{اسم_الحقل}» مطلوب.");

        return القيمة.Trim();
    }

    public static string تحقق_من_الرقم_التسلسلي(string? الرقم)
    {
        string قيمة = تحقق_من_النص(الرقم, "الرقم التسلسلي");
        if (!نمط_الرقم_التسلسلي.IsMatch(قيمة))
            throw new ArgumentException("الرقم التسلسلي يجب أن يتكون من 4 إلى 30 حرفاً أو رقماً، ويمكن استخدام الشرطة والشرطة السفلية.");

        return قيمة;
    }

    public static string تحقق_من_الهاتف(string? الهاتف)
    {
        string قيمة = تحقق_من_النص(الهاتف, "رقم الهاتف");
        if (!نمط_الهاتف.IsMatch(قيمة))
            throw new ArgumentException("صيغة رقم الهاتف غير صحيحة. استخدم الأرقام والمسافة أو + أو الأقواس أو الشرطة فقط.");

        return قيمة;
    }

    public static decimal تحقق_من_التكلفة(decimal القيمة, string اسم_التكلفة)
    {
        if (القيمة < 0)
            throw new ArgumentOutOfRangeException(اسم_التكلفة, "لا يمكن أن تكون التكلفة سالبة.");

        return decimal.Round(القيمة, 2, MidpointRounding.AwayFromZero);
    }

    public static void تحقق_من_التواريخ(DateTime تاريخ_الاستلام, DateTime تاريخ_التسليم_المتوقع)
    {
        if (تاريخ_التسليم_المتوقع.Date < تاريخ_الاستلام.Date)
            throw new ArgumentException("تاريخ التسليم المتوقع لا يمكن أن يسبق تاريخ الاستلام.");
    }

    public static void تحقق_من_الحالة(حالة_الصيانة الحالة)
    {
        if (!Enum.IsDefined(typeof(حالة_الصيانة), الحالة))
            throw new ArgumentOutOfRangeException(nameof(الحالة), "حالة الصيانة المحددة غير معروفة.");
    }
}
