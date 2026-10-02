using System.Globalization;
using System.Text;

namespace Journey_of_faith.Application.common.untils;

public static class StringExtensions
{
    extension(string str)
    {
        public string RemoveVietnameseSigns()
        {
            if(string.IsNullOrWhiteSpace(str)) return str;

            string normalizedString = str.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString()
                     .Normalize(NormalizationForm.FormC)
                     .Replace("đ", "d")
                     .Replace("Đ", "D");
        }
    }
}