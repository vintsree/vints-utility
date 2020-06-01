namespace Vints.Utility.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ContentTypeMetaData : Attribute
    {
        public ContentTypeMetaData()
        {
            this.Value = "text/plain";
            this.IsText = true;
        }

        public string Value { get; set; }

        public bool IsText { get; set; }

        public bool IsBinary
        {
            get
            {
                return !this.IsText;
            }
            set
            {
                this.IsText = !value;
            }
        }
    }
}
