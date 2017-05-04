namespace RecommendIT.Common.Data.MongoRepository.Repository
{
    using System.Linq;

    public static class RepositoryExtensions
    {
        public static void DeleteHierarchicalEntity<T>(this IRepository<T> repository, string id) where T : HierarchicalEntity
        {
            var model = repository.FirstOrDefault(s => s.Id == id);
            var children = repository.Where(x => x.ParentId == model.Id);

            foreach (var child in children)
            {
                DeleteHierarchicalEntity(repository, child.Id);
            }

            repository.Delete(model);
        }

        public static bool IsLevelExceeded<T>(this IRepository<T> repository, string parentId) where T : HierarchicalEntity
        {
            var parent = repository.Get(parentId);
            int lastLevel = 6;

            return parent != null && parent.Level >= lastLevel;
        }

        public static bool HasChildren<T>(this IRepository<T> repository, string id) where T : HierarchicalEntity
        {
            return repository.Where(x => x.ParentId == id).Any();
        }
    }
}
