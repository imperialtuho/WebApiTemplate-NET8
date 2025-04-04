namespace WebApiTemplate.Domain.Enums
{
    /// <summary>
    /// Enum representing the various data interchange formats supported by the application.
    /// </summary>
    public enum DataInterchangeFormat
    {
        /// <summary>
        /// Represents JSON format for data interchange.
        /// </summary>
        Json = 0,

        /// <summary>
        /// Represents XML format for data interchange.
        /// </summary>
        Xml = 1,

        /// <summary>
        /// Represents SOAP XML format for data interchange.
        /// </summary>
        SoapXml = 2
    }
}