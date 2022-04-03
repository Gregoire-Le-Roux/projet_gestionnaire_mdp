namespace back.ImportModels
{
    public class MdpImport
    {
        public string Titre { get; set; } = null!;
        public string Login { get; set; } = null!;
        public string Mdp { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string DateExpiration { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int IdCompteCreateur { get; set; }
    }
}
