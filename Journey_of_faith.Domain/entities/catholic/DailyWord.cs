using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.catholic
{
    public class DailyWord : AuditableEntity
    {

        public DailyWord()
        {
            
        }

        public DailyWord(DateTime dateTime, string title, string bibleContent, string gospel)
        {
            Date = dateTime;
            Title = title;
            BibleContent = bibleContent;
            Gospel = gospel;
        }
        public DateTime Date { get; set; }
        public string? Title { get; set; }
        public string BibleContent { get; set; } = string.Empty;
        public string? Gospel { get; set; }
        public bool? IsShortWord { get; set; }
    }
}
