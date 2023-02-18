using System.ComponentModel;

namespace NoteWriter.Models
{
    public class NoteModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Text { get; set; }

        [DefaultValue("0")]
        public string UserId { get; set; }
    }
}
