namespace RFactor2VREcranSwitcher
{
    public class Fichier
    {
        public Fichier(string path, Extension extension)
        {
            this.Extension = extension;
            this.Path = path;
        }
        public string Path { get; set; }

        public Extension Extension { get; set; }
    }

    public enum Extension
    {
        ini,
        json,
        JSON,
        ecran,
        vr
    }
}
