using System.Globalization;
using System.Text;
using Journey_of_faith.Domain.entities.quiz;

namespace Journey_of_faith.Application.common.services;


public static class MapObject
{
    public static List<QuestionCategory> MapNameToObject(List<string> names)
    {
        List<QuestionCategory> result = new List<QuestionCategory>();
        foreach(string name in names)
        {
            string code = RemoveDiacritics(name).ToUpperInvariant().Replace(" ", "_");
            var category = new QuestionCategory(name, code, string.Empty);
            result.Add(category);
        }

        return result;
    }

    private static string RemoveDiacritics(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        // 1. Tách ký tự nền và dấu thanh (Ví dụ: 'á' -> 'a' và dấu sắc '́')
        string normalizedString = text.Normalize(NormalizationForm.FormD);

        // 2. Dùng LINQ lọc bỏ các ký tự thuộc nhóm dấu (NonSpacingMark)
        var stringBuilder = new StringBuilder();
        foreach (char c in normalizedString)
        {
            UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(c);
            if (uc != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        // 3. Chuẩn hóa ngược lại về Form C và xử lý riêng chữ đ/Đ
        string result = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

        return result.Replace('đ', 'd').Replace('Đ', 'D');
    }
}