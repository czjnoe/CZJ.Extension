namespace CZJ.Extension
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MapConvertAttribute : Attribute
    {
        public Type ConverterType { get; }

        public MapConvertAttribute(Type converterType)
        {
            ConverterType = converterType;
        }
    }
}
