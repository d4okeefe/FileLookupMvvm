namespace FileSearchMvvm.Models.Utilities
{
    public class LengthOfCover
    {
        public int Length;
        public override string ToString()
        {
            if (this.Length == 48 || this.Length == 49 || this.Length == 50 || this.Length == 51)
            { return string.Format(", {0} pica cover", this.Length); }
            else { return ""; }
        }
    }
}
