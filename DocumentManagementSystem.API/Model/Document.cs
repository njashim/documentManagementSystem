namespace DocumentManagementSystem.API.Model
{
    public class Document
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? Title { get; set; }

        //public string Content { get; set; }

        //public string? FileName { get; set; }

        //public string FileType { get; set; }

        //public string FilePath { get; set; }

        //public DateTime CreatedAt { get; set; } = DateTime.Now;

        //public DateTime UpdatedAt { get; set; }

        //public string Author { get; set; }

        //public string OCRStatus { get; set; }

        //public string SearchIndexId { get; set; }

        //public void Update()
        //{
        //    UpdatedAt = DateTime.Now;
        //}
    }
}
