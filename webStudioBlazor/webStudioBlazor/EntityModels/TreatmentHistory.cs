namespace webStudioBlazor.EntityModels
{
    /// <summary>
    /// Медично-косметологічна картка одного відвідування (процедура).
    /// ClientId — UserId зареєстрованого клієнта або ключ guest:{appointmentId}.
    /// </summary>
    public class TreatmentHistory
    {
        public int Id { get; set; }

        /// <summary>UserId або guest:{appointmentId}</summary>
        public string ClientId { get; set; } = string.Empty;

        public DateTime VisitDate { get; set; }

        public string ProcedureName { get; set; } = string.Empty;

        public string ProcedureDescription { get; set; } = string.Empty;

        public string SkinBeforeDescription { get; set; } = string.Empty;

        public string SkinAfterDescription { get; set; } = string.Empty;

        public string Recommendations { get; set; } = string.Empty;

        public string MasterComment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public List<TreatmentPhoto> Photos { get; set; } = new();
    }
}
