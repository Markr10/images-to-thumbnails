
namespace ImagesToThumbnails
{
    public enum FitMode
    {
        Fit,
        FitWidth,
        FitHeight,
        Stretch,
    }
}

// Self defined namespace for classes with extension method(s).
namespace Extension
{
    using ImagesToThumbnails;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Class that contains extension method(s) for the FitMode enum.
    /// </summary>
    /// <remarks>The extension method(s) can only be accessed when the Extension namespace is imported.</remarks>
    public static class FitModeExtensionMethods
    {
        public static string ToSentenceCase(this FitMode fitMode)
        {
            Regex rgx = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z])(.) |
                 (?<=[^A-Z])(?=[A-Z][A-Z]) |
                 (?<=[^A-Z])(?=[A-Z])(.)", RegexOptions.IgnorePatternWhitespace);

            return rgx.Replace(fitMode.ToString(), s => " " + s.Value.ToLower());
        }
    }
}