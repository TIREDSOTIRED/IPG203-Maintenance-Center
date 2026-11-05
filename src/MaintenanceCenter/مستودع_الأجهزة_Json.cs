using System.Text.Json;

namespace مركز_صيانة_الأجهزة;

/// <summary>
/// مسؤول عن القراءة والكتابة في ملف JSON، مع إبقاء منطق التخزين خارج أصناف الأجهزة.
/// </summary>
public sealed class مستودع_الأجهزة_Json
{
    private readonly string _مسار_الملف;
    private readonly JsonSerializerOptions _خيارات = new() { WriteIndented = true };

    public مستودع_الأجهزة_Json(string مسار_الملف)
    {
        _مسار_الملف = Path.GetFullPath(مدقق_البيانات.تحقق_من_النص(مسار_الملف, "مسار ملف البيانات"));
    }

    public List<جهاز_منزلي> تحميل()
    {
        if (!File.Exists(_مسار_الملف))
            return new List<جهاز_منزلي>();

        string محتوى = File.ReadAllText(_مسار_الملف);
        List<بيانات_جهاز> البيانات = JsonSerializer.Deserialize<List<بيانات_جهاز>>(محتوى, _خيارات)
            ?? new List<بيانات_جهاز>();

        return البيانات.Select(مصنع_الأجهزة.إنشاء).ToList();
    }

    public void حفظ(IEnumerable<جهاز_منزلي> الأجهزة)
    {
        string? المجلد = Path.GetDirectoryName(_مسار_الملف);
        if (!string.IsNullOrWhiteSpace(المجلد))
            Directory.CreateDirectory(المجلد);

        List<بيانات_جهاز> البيانات = الأجهزة
            .Select(مصنع_الأجهزة.تحويل_إلى_بيانات)
            .ToList();

        string محتوى = JsonSerializer.Serialize(البيانات, _خيارات);
        File.WriteAllText(_مسار_الملف, محتوى);
    }
}
