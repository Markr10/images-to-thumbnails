
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
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Class that contains extension method(s) for the FitMode enum.
    /// </summary>
    /// <remarks>The extension method(s) can only be accessed when the Extension namespace is imported.</remarks>
    public static class FitModeExtensionMethods
    {
        /// <summary>
        /// Returns a string with the enum name in sentence case style.
        /// </summary>
        /// <returns>A string with the enum name in sentence case style</returns>
        public static string ToSentenceCase(this FitMode fitMode)
        {
            Regex rgx = new Regex(@"(?<!^)[A-Z]", RegexOptions.ExplicitCapture);

            // It is possible in this situation to use Enum.GetName. It should be faster as ToString.
            return rgx.Replace(Enum.GetName(typeof(FitMode), fitMode), s => " " + s.Value.ToLower());
        }
    }
}