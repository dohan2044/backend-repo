namespace Journey_of_faith.Application.common.untils;


public static class NumberExtension
{
    extension(double num)
    {
        public string FormatNumbeDoubleToString(string format)
        {
            return num.ToString(format);
        }
    }
}