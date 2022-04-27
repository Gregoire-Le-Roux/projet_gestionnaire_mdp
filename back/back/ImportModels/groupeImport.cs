namespace back.ImportModels
{
    public class groupeImport
    {
        public int IdCreateur { get; set; }
        public int IdMdp { get; set; }
        public string Titre { get; set; } = null!;
        public string[] listeMail { get; set; } = null!;
    }
}
