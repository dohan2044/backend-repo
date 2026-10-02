using System;
using Journey_of_faith.Domain.entities.masslive;

namespace Journey_of_faith.Domain.entities.location
{
    public class Liturgy
    {
        // --- THUỘC TÍNH (PROPERTIES) ---
        // Đóng gói bằng private set để bảo vệ tính toàn vẹn dữ liệu
        public int Id { get; private set; }
        public int MassScheduleId { get; private set; }
        public string ReadingOne { get; private set; } = string.Empty;
        public string ResponsorialPsalm { get; private set; } = string.Empty;
        public string GoodNew { get; private set; } = string.Empty;
        public string EndWord { get; private set; } = string.Empty;
        public DateTime DateActive { get; private set; }

        // Navigation Property trỏ ngược về cha nếu cần thiết cho việc truy vấn
        public MassSchedule MassSchedule { get; private set; } = null!;

        // --- CONSTRUCTORS ---
        // Constructor rỗng bắt buộc phục vụ cơ chế nạp dữ liệu ngầm của EF Core
        private Liturgy() { }

        // Constructor nghiệp vụ được gọi duy nhất từ bên trong Aggregate Root (MassSchedule)
        public Liturgy(
            string readingOne,
            string responsorialPsalm,
            string goodNew,
            string? endWord,
            DateTime? dateActive = null)
        {
            if (string.IsNullOrWhiteSpace(readingOne)) throw new ArgumentException("Bài đọc 1 không được để trống.");
            if (string.IsNullOrWhiteSpace(responsorialPsalm)) throw new ArgumentException("Đáp ca không được để trống.");
            if (string.IsNullOrWhiteSpace(goodNew)) throw new ArgumentException("Tin Mừng không được để trống.");

            ReadingOne = readingOne;
            ResponsorialPsalm = responsorialPsalm;
            GoodNew = goodNew;
            EndWord = endWord ?? string.Empty;
            DateActive = dateActive ?? DateTime.UtcNow;
        }

        // --- HÀNH VI NGHIỆP VỤ (BEHAVIORS) ---
        // Hàm cập nhật chi tiết các bài đọc, được điều phối bởi MassSchedule
        public void UpdateDetails(string readingOne, string responsorialPsalm, string goodNew, string? endWord)
        {
            if (string.IsNullOrWhiteSpace(readingOne)) throw new ArgumentException("Bài đọc 1 không được để trống.");
            if (string.IsNullOrWhiteSpace(responsorialPsalm)) throw new ArgumentException("Đáp ca không được để trống.");
            if (string.IsNullOrWhiteSpace(goodNew)) throw new ArgumentException("Tin Mừng không được để trống.");

            ReadingOne = readingOne;
            ResponsorialPsalm = responsorialPsalm;
            GoodNew = goodNew;
            EndWord = endWord ?? string.Empty;
            DateActive = DateTime.UtcNow; // Cập nhật lại thời điểm thay đổi nội dung phụng vụ
        }
    }
}
