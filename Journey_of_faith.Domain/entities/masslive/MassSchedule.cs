using System;
using Journey_of_faith.Domain.entities.location;

namespace Journey_of_faith.Domain.entities.masslive
{
    // Giả định AuditableEntity của bạn chứa các trường Id, CreationTime, IsDeleted,...
    public class MassSchedule : AuditableEntity 
    {
        // --- THUỘC TÍNH (PROPERTIES) ---
        // Sử dụng private set để bảo vệ trạng thái nội bộ của thực thể
        public bool? IsFixed { get; private set; }
        public int ChurchId { get; private set; }
        public DateTime? FromDate { get; private set; }
        public DateTime? ToDate { get; private set; }
        public DateOnly? Date { get; private set; }
        public string Time { get; private set; } = string.Empty;
        public int? MassTypeId { get; private set; }
        public string Name { get; private set; } = string.Empty;

        // Navigation Properties (Đóng gói an toàn)
        public Liturgy? Liturgy { get; private set; }
        public Church Church { get; private set; } = null!;
        public MassType? MassType { get; private set; }

        private MassSchedule() { } 

        // Constructor nghiệp vụ dùng cho tầng Use Case (Application) khi tạo mới lịch lễ
        public MassSchedule(
            int churchId, 
            DateOnly? date, 
            string time, 
            int? massTypeId, 
            string name, 
            bool? isFixed = false, 
            DateTime? fromDate = null, 
            DateTime? toDate = null)
        {
            // Business Rule Validation (Ví dụ cơ bản)
            if (string.IsNullOrWhiteSpace(time)) throw new ArgumentException("Thời gian lịch lễ không được để trống");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Tên lịch lễ không được để trống");

            ChurchId = churchId;
            Date = date;
            Time = time;
            MassTypeId = massTypeId;
            Name = name;
            IsFixed = isFixed;
            FromDate = fromDate;
            ToDate = toDate;
        }

        public MassSchedule(
            int id,
            int churchId,
            DateOnly? date,
            string time,
            int? massTypeId,
            string name,
            bool? isFixed = false,
            DateTime? fromDate = null,
            DateTime? toDate = null)
            : this(churchId, date, time, massTypeId, name, isFixed, fromDate, toDate)
        {
            Id = id;
        }
        public void SetLiturgy(Liturgy liturgy)
        {
            if (Liturgy is null)
            {
                Liturgy = liturgy;
            }
            else
            {
                Liturgy.UpdateDetails(liturgy.ReadingOne, liturgy.ResponsorialPsalm, liturgy.GoodNew, liturgy.EndWord);
            }
        }

        public void UpdateDetails(string name, string time, int? massTypeId, DateOnly? date)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Tên lịch lễ không được để trống");
            if (string.IsNullOrWhiteSpace(time)) throw new ArgumentException("Thời gian lịch lễ không được để trống");

            Name = name;
            Time = time;
            MassTypeId = massTypeId;
            Date = date;
        }
    }
}
