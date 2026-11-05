namespace مركز_صيانة_الأجهزة;

/// <summary>
/// نموذج بيانات بسيط يستخدم لحفظ الأجهزة واستعادتها من ملف JSON.
/// </summary>
public sealed class بيانات_جهاز
{
    public string نوع_الجهاز { get; set; } = string.Empty;
    public string الرقم_التسلسلي { get; set; } = string.Empty;
    public string اسم_العميل { get; set; } = string.Empty;
    public string رقم_الهاتف { get; set; } = string.Empty;
    public string وصف_العطل { get; set; } = string.Empty;
    public decimal تكلفة_قطع_الغيار { get; set; }
    public decimal أجور_العمل { get; set; }
    public decimal التكلفة_الإضافية { get; set; }
    public DateTime تاريخ_الاستلام { get; set; }
    public DateTime تاريخ_التسليم_المتوقع { get; set; }
    public string الحالة { get; set; } = nameof(حالة_الصيانة.مستلم);
}
