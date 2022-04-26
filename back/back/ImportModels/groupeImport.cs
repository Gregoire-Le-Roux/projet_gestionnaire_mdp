namespace back.ImportModels
{
    public class groupeImport
    {
        public int IdCreateur { get; set; }
        public string Titre { get; set; } = null!;
        public int[] IdCompteGroupe { get; set; } = null!;
    }
}
