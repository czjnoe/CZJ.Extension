namespace CZJ.Extension
{
    public static class ObjectExtensions
    {
        public static T? Clone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);

            return JsonConvert.DeserializeObject<T>(serialized);
        }

      
        public static Dictionary<string, object> NonNullPropertiesToDictionary(this object @object)
        {
            Dictionary<string, object> dictionary = new();

            foreach (var propertyInfo in @object.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(@object);

                if (value is not null)
                {
                    dictionary[propertyInfo.Name] = value;
                }
            }

            return dictionary;
        }

    
        public static Dictionary<string, object?> PropertiesToDictionary(this object @object)
        {
            Dictionary<string, object?> dictionary = new();

            foreach (var propertyInfo in @object.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(@object);

                dictionary[propertyInfo.Name] = value;
            }

            return dictionary;
        }
    }
}
