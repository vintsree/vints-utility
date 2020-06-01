namespace Vints.Utility.Enums
{
    using Attributes;

    /// <summary>
    /// Content Type
    /// </summary>
    public enum ContentType
    {
        [ContentTypeMetaData(Value = "application/json", IsText = true)]
        JSON,
        [ContentTypeMetaData(Value = "application/javascript", IsText = true)]
        JS,
        [ContentTypeMetaData(Value = "application/x-www-form-urlencoded", IsText = true)]
        FURLE,
        [ContentTypeMetaData(Value = "text/plain", IsText = true)]
        DEFAULT
    }
}
