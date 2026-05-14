namespace webStudioBlazor.EntityModels
{
    public class TreatmentPhoto
    {
        public const string PhotoTypeBefore = "Before";
        public const string PhotoTypeAfter = "After";

        public int Id { get; set; }

        public int TreatmentHistoryId { get; set; }

        public TreatmentHistory TreatmentHistory { get; set; } = default!;

        /// <summary>Відносний URL, напр. /uploads/treatments/...</summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>Before або After</summary>
        public string PhotoType { get; set; } = PhotoTypeBefore;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
