

using System.Security.Cryptography;
using System.Text;
namespace Journey_of_faith.Infrastructure.services;

public static class PasswordGenerator
{
    public static string GenerateRandomPassword(int length = 12)
    {
        if (length < 6) throw new ArgumentException("Mật khẩu phải dài từ 6 ký tự trở lên để đảm bảo bảo mật.");

        // Các nhóm ký tự để lựa chọn
        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string specials = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        // Gộp chung tất cả các ký tự lại
        string allChars = uppercase + lowercase + digits + specials;
        StringBuilder password = new StringBuilder();

        // Bước 1: Đảm bảo có ít nhất 1 ký tự thuộc mỗi nhóm (đạt chuẩn policy của hầu hết các hệ thống)
        password.Append(GetRandomChar(uppercase));
        password.Append(GetRandomChar(lowercase));
        password.Append(GetRandomChar(digits));
        password.Append(GetRandomChar(specials));

        // Bước 2: Điền nốt các ký tự còn lại ngẫu nhiên từ kho allChars
        for (int i = 4; i < length; i++)
        {
            password.Append(GetRandomChar(allChars));
        }

        // Bước 3: Trộn đều (shuffle) các ký tự lên để không bị cố định thứ tự ban đầu
        return ShuffleString(password.ToString());
    }

    // Hàm lấy 1 ký tự ngẫu nhiên an toàn (Dùng Cryptography tránh bị đoán trước)
    private static char GetRandomChar(string charSet)
    {
        byte[] randomByte = new byte[1];
        RandomNumberGenerator.Fill(randomByte);
        int index = randomByte[0] % charSet.Length;
        return charSet[index];
    }

    // Hàm trộn ngẫu nhiên các ký tự trong chuỗi
    private static string ShuffleString(string str)
    {
        char[] array = str.ToCharArray();
        int n = array.Length;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do { RandomNumberGenerator.Fill(box); } while (!(box[0] < n * (byte.MaxValue / n)));
            int k = box[0] % n;
            n--;
            char value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
        return new string(array);
    }
}