using System;

namespace CDMS.Language
{
    public interface ILocalizedStringProvider
    {
        /// <summary>
        /// Get a localized string for a model property
        /// </summary>
        /// <param name="model">Model being localized</param>
        /// <param name="propertyName">Property to get string for</param>
        /// <returns>Translated string if found; otherwise null.</returns>
        string GetModelString(Type model, string propertyName);


        /// <summary>
        /// Get a localized metadata for a model property
        /// </summary>
        /// <param name="model">Model being localized</param>
        /// <param name="propertyName">Property to get string for</param>
        /// <param name="metadataName">Valid names are: Watermark, Description, NullDisplayText, ShortDisplayText.</param>
        /// <returns>Translated string if found; otherwise null.</returns>
        /// <remarks>
        /// Look at <see cref="ModelMetadata"/> to know more about the meta data
        /// </remarks>
        string GetModelString(Type model, string propertyName, string metadataName);

    }
}
