namespace CZJ.Extension.Extensions
{
    public static class ArrayExtension
    {
        public static void Clear(this Array array, Int32 index, Int32 length)
        {
            Array.Clear(array, index, length);
        }

        public static void Reverse(this Array array)
        {
            Array.Reverse(array);
        }
        public static void Sort(this Array array, IComparer comparer)
        {
            Array.Sort(array, comparer);
        }

        public static void Sort(this Array array)
        {
            Array.Sort(array);
        }
    }
}
