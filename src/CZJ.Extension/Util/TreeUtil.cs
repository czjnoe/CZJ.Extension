namespace CZJ.Extension
{
    /// <summary>
    /// 树工具类，用于构建树结构
    /// </summary>
    public class TreeUtil<TKey, TData>
    {
        private List<TreeNode<TKey, TData>> _nodes;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="nodes">树节点列表</param>
        public TreeUtil(List<TreeNode<TKey, TData>> nodes)
        {
            _nodes = nodes;
        }

        /// <summary>
        /// 构建树结构
        /// </summary>
        /// <returns>根节点</returns>
        public TreeNode<TKey, TData> BuildTree()
        {
            // 获取根节点
            var root = _nodes.FirstOrDefault(n => n.ParentId.Equals(default(TKey)));

            if (root == null)
            {
                return null;
            }

            // 构建树结构
            BuildTree(root);

            return root;
        }

        private void BuildTree(TreeNode<TKey, TData> node)
        {
            node.Children = _nodes.Where(n => n.ParentId.Equals(node.Id)).ToList();

            if (node.Children.Count > 0)
            {
                foreach (var child in node.Children)
                {
                    BuildTree(child);
                }
            }
        }

        /// <summary>
        /// 获取某个节点的所有父节点
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>父节点列表，从根节点到该节点的顺序</returns>
        public List<TreeNode<TKey, TData>> GetParents(TreeNode<TKey, TData> node)
        {
            var parents = new List<TreeNode<TKey, TData>>();
            var parent = GetParent(node.Id);
            while (parent != null)
            {
                parents.Insert(0, parent);
                parent = GetParent(parent.Id);
            }
            return parents;
        }

        /// <summary>
        /// 获取某个节点的深度
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>节点深度，根节点的深度为0</returns>
        public int GetDepth(TreeNode<TKey, TData> node)
        {
            return GetParents(node).Count;
        }

        /// <summary>
        /// 获取某个节点的所有子孙节点
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>子孙节点列表</returns>
        public List<TreeNode<TKey, TData>> GetDescendants(TreeNode<TKey, TData> node)
        {
            var descendants = new List<TreeNode<TKey, TData>>();
            GetDescendants(node, descendants);
            return descendants;
        }

        private void GetDescendants(TreeNode<TKey, TData> node, List<TreeNode<TKey, TData>> descendants)
        {
            descendants.Add(node);
            if (node.Children.Count > 0)
            {
                foreach (var child in node.Children)
                {
                    GetDescendants(child, descendants);
                }
            }
        }

        private TreeNode<TKey, TData> GetParent(TKey id)
        {
            var node = _nodes.FirstOrDefault(n => n.Id.Equals(id));
            if (node == null)
            {
                return null;
            }
            return _nodes.FirstOrDefault(n => n.Id.Equals(node.ParentId));
        }

        /// <summary>
        /// 获取某个节点的所有兄弟节点
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>兄弟节点列表</returns>
        public List<TreeNode<TKey, TData>> GetSiblings(TreeNode<TKey, TData> node)
        {
            var parent = GetParent(node.Id);
            if (parent == null)
            {
                return new List<TreeNode<TKey, TData>>();
            }
            return parent.Children.Where(n => !n.Id.Equals(node.Id)).ToList();
        }

        /// <summary>
        /// 获取某个节点的所有兄弟节点数量
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>兄弟节点数量</returns>
        public int GetSiblingCount(TreeNode<TKey, TData> node)
        {
            return GetSiblings(node).Count;
        }

        /// <summary>
        /// 判断某个节点是否是叶子节点
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>是否是叶子节点</returns>
        public bool IsLeaf(TreeNode<TKey, TData> node)
        {
            return node.Children.Count == 0;
        }

        /// <summary>
        /// 获取树的最大深度
        /// </summary>
        /// <returns>树的最大深度</returns>
        public int GetMaxDepth()
        {
            return _nodes.Max(n => GetDepth(n));
        }

        /// <summary>
        /// 获取树的最小深度
        /// </summary>
        /// <returns>树的最小深度</returns>
        public int GetMinDepth()
        {
            return _nodes.Min(n => GetDepth(n));
        }

        /// <summary>
        /// 获取某个节点的下一个兄弟节点
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>下一个兄弟节点</returns>
        public TreeNode<TKey, TData> GetNextSibling(TreeNode<TKey, TData> node)
        {
            var siblings = GetSiblings(node);
            var index = siblings.FindIndex(n => n.Id.Equals(node.Id));
            return index + 1 < siblings.Count ? siblings[index + 1] : null;
        }

        /// <summary>
        /// 获取某个节点的上一个兄弟节点
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>上一个兄弟节点</returns>
        public TreeNode<TKey, TData> GetPreviousSibling(TreeNode<TKey, TData> node)
        {
            var siblings = GetSiblings(node);
            var index = siblings.FindIndex(n => n.Id.Equals(node.Id));
            return index - 1 >= 0 ? siblings[index - 1] : null;
        }

        /// <summary>
        /// 获取某个节点的首个子节点
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>首个子节点</returns>
        public TreeNode<TKey, TData> GetFirstChild(TreeNode<TKey, TData> node)
        {
            return node.Children.Count > 0 ? node.Children[0] : null;
        }

        /// <summary>
        /// 获取某个节点的最后一个子节点
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>最后一个子节点</returns>
        public TreeNode<TKey, TData> GetLastChild(TreeNode<TKey, TData> node)
        {
            return node.Children.Count > 0 ? node.Children[node.Children.Count - 1] : null;
        }

        /// <summary>
        /// 获取树的所有节点数量
        /// </summary>
        /// <returns>树的所有节点数量</returns>
        public int GetNodeCount()
        {
            return _nodes.Count;
        }

        /// <summary>
        /// 获取树的所有叶子节点数量
        /// </summary>
        /// <returns>树的所有叶子节点数量</returns>
        public int GetLeafCount()
        {
            return _nodes.Count(IsLeaf);
        }

        /// <summary>
        /// 获取树的所有节点的权重和
        /// </summary>
        /// <returns>树的所有节点的权重和</returns>
        public int GetTotalWeight()
        {
            return _nodes.Sum(n => n.Weight);
        }

        /// <summary>
        /// 获取树的所有叶子节点的权重和
        /// </summary>
        /// <returns>树的所有叶子节点的权重和</returns>
        public int GetLeafWeightTotal()
        {
            return _nodes.Where(IsLeaf).Sum(n => n.Weight);
        }

        /// <summary>
        /// 获取树的平均深度
        /// </summary>
        /// <returns>树的平均深度</returns>
        public int GetAverageDepth()
        {
            return (int)_nodes.Average(n => GetDepth(n));
        }

        /// <summary>
        /// 获取树的平均节点权重
        /// </summary>
        /// <returns>树的平均节点权重</returns>
        public int GetAverageWeight()
        {
            return (int)_nodes.Average(n => n.Weight);
        }

        /// <summary>
        /// 获取树的最大节点权重
        /// </summary>
        /// <returns>树的最大节点权重</returns>
        public int GetMaxWeight()
        {
            return _nodes.Max(n => n.Weight);
        }

        /// <summary>
        /// 获取树的最小节点权重
        /// </summary>
        /// <returns>树的最小节点权重</returns>
        public int GetMinWeight()
        {
            return _nodes.Min(n => n.Weight);
        }

        /// <summary>
        /// 根据ID获取节点
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <returns>找到的节点，如果不存在则返回null</returns>
        public TreeNode<TKey, TData> GetNodeById(TKey id)
        {
            return _nodes.FirstOrDefault(n => n.Id.Equals(id));
        }

        /// <summary>
        /// 根据ID获取节点及其所有子节点
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <returns>找到的节点（包含已填充的子孙节点），如果节点不存在则返回null</returns>
        public List<TreeNode<TKey, TData>> GetNodeWithDescendantsById(TKey id)
        {
            var node = GetNodeById(id);
            if (node == null)
            {
                return new List<TreeNode<TKey, TData>>();
            }
            return GetDescendants(node);
        }

        /// <summary>
        /// 根据ID获取节点的所有子节点（不包含当前节点）
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <returns>所有子孙节点的列表，如果节点不存在则返回空列表</returns>
        public List<TreeNode<TKey, TData>> GetDescendantsById(TKey id)
        {
            var node = GetNodeById(id);
            if (node == null)
            {
                return new List<TreeNode<TKey, TData>>();
            }
            var descendants = GetDescendants(node);
            descendants.Remove(node); // 移除当前节点
            return descendants;
        }
    }

    public class TreeNode<TKey, TData>
    {
        public TKey Id { get; set; }
        public TKey ParentId { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
        public TData Data { get; set; }
        public List<TreeNode<TKey, TData>> Children { get; set; }

        public TreeNode(TKey id, TKey parentId, string name, int weight, TData data)
        {
            this.Id = id;
            this.ParentId = parentId;
            this.Name = name;
            this.Weight = weight;
            this.Data = data;
        }
    }
}
