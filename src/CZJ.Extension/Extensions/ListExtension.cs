namespace CZJ.Extension
{
    public static class ListExtension
    {
        /// <summary>
        /// 判断两个列表是否相等。
        /// </summary>
        /// <typeparam name="T">列表元素类型</typeparam>
        /// <param name="list1">要比较的第一个列表</param>
        /// <param name="list2">要比较的第二个列表</param>
        /// <returns>如果两个列表相等，则返回 true；否则返回 false</returns>
        public static bool Equals<T>(this List<T> list1, List<T> list2)
        {
            if (list1 == null && list2 == null)
            {
                return true;
            }
            else if (list1 == null || list2 == null)
            {
                return false;
            }
            else if (list1.Count != list2.Count)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < list1.Count; i++)
                {
                    if (!list1[i].Equals(list2[i]))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// 将列表中的元素分页显示。
        /// </summary>
        /// <typeparam name="T">列表元素类型</typeparam>
        /// <param name="list">要分页的列表</param>
        /// <param name="pageSize">每页显示的元素数量</param>
        /// <param name="pageIndex">要显示的页码，从 0 开始</param>
        /// <returns>指定页的元素列表</returns>
        public static List<T> Page<T>(List<T> list, int pageSize, int pageIndex)
        {
            return list.Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();
        }

        /// <summary>
        /// 将列表中的元素排序。
        /// </summary>
        /// <typeparam name="T">列表元素类型</typeparam>
        /// <param name="list">要排序的列表</param>
        public static void Sort<T>(List<T> list)
        {
            list.Sort();
        }

        /// <summary>
        /// 将列表中的元素按指定的比较器排序。
        /// </summary>
        /// <typeparam name="T">列表元素类型</typeparam>
        /// <param name="list">要排序的列表</param>
        /// <param name="comparer">比较器</param>
        public static void Sort<T>(List<T> list, IComparer<T> comparer)
        {
            list.Sort(comparer);
        }
    }
}
