using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    [TestClass]
    public sealed class TreeUtilTest
    {
        private TreeUtil<int, string> CreateSampleTree()
        {
            // 构建测试树结构:
            //          1(Root, weight:10)
            //         / \
            //        2   3(weight:20, 30)
            //       /|   |\
            //      4 5   6 7(weight:40, 50, 60, 70)
            //        |
            //        8(weight:80)

            var nodes = new List<TreeNode<int, string>>
            {
                new TreeNode<int, string>(1, 0, "Root", 10, "Root Data"),
                new TreeNode<int, string>(2, 1, "Node2", 20, "Data2"),
                new TreeNode<int, string>(3, 1, "Node3", 30, "Data3"),
                new TreeNode<int, string>(4, 2, "Node4", 40, "Data4"),
                new TreeNode<int, string>(5, 2, "Node5", 50, "Data5"),
                new TreeNode<int, string>(6, 3, "Node6", 60, "Data6"),
                new TreeNode<int, string>(7, 3, "Node7", 70, "Data7"),
                new TreeNode<int, string>(8, 5, "Node8", 80, "Data8")
            };

            return new TreeUtil<int, string>(nodes);
        }

        [TestMethod]
        public void BuildTree_ShouldCreateCorrectStructure()
        {
            var treeUtil = CreateSampleTree();
            var root = treeUtil.BuildTree();
            var node = treeUtil.GetNodeById(3);
            var allChildNodes = treeUtil.GetDescendantsById(2);
            Assert.IsNotNull(root);
            Assert.AreEqual(1, root.Id);
            Assert.AreEqual("Root", root.Name);
            Assert.AreEqual(2, root.Children.Count);
            Assert.AreEqual(3, node.Id);
        }
    }
}
