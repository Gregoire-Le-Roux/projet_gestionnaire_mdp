namespace back.ExportModels
{
    public class CompteExport
    {
        public int Id { get; set; }
        public string Nom { get; set; } = null!;
        public string Prenom { get; set; } = null!;
        public string Mail { get; set; } = null!;
        public string HashCle { get; set; } = null!;
    }
}
