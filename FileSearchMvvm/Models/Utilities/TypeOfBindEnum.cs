using System.ComponentModel;

namespace FileSearchMvvm.Models.Utilities
{
    public enum TypeOfBindEnum
    {
        [Description("Saddle Stitch")]
        SaddleStitch,
        [Description("Perfect Bind")]
        PerfectBind,
        ProgramDecidesByPageCount,
        [Description("Circuit Court")]
        CircuitCourt,
        [Description("Other")]
        Other
    }
}