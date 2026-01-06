using CZJ.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    [TestClass]
    public sealed class TreeExtensionTest
    {
        [TestMethod]
        public void BuildTreeTest()
        {
            List<RecipeModel> list = [
                 new RecipeModel { Id = "1", Name = "根节点1", ParentId = null, Folder = true },
                    new RecipeModel { Id = "2", Name = "根节点2", ParentId = null, Folder = true },
                    new RecipeModel { Id = "1.1", Name = "子节点1.1", ParentId = "1", Folder = true },
                    new RecipeModel { Id = "1.2", Name = "子节点1.2", ParentId = "1", Folder = false },
                    new RecipeModel { Id = "2.1", Name = "子节点2.1", ParentId = "2", Folder = false },
                    new RecipeModel { Id = "1.1.1", Name = "子节点1.1.1", ParentId = "1.1", Folder = false },


                ];
            var tree = list.ToTree();
        }
    }

    public class RecipeModel : ITreeEntity<RecipeModel>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public bool Folder { get; set; } = false;

        public ICollection<RecipeModel> Children { get; set; }

        public string ParentId { get; set; }
    }
}
