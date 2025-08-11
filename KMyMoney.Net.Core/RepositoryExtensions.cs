using KMyMoney.Net.Models;

namespace KMyMoney.Net.Core;

public static class RepositoryExtensions
{
    public static T? GetById<T>(this IEnumerable<T> arr, string id)
        where T : IHasId
        => arr.FirstOrDefault(x => x.Id == id);

    public static T? GetById<T>(this ArrayWithCount<T> arr, string id)
        where T : IHasId
        => arr.Values.GetById(id);

    public static T? GetByName<T>(this IEnumerable<T> arr, string name)
        where T : IHasName
        => arr.FirstOrDefault(x => x.Name == name);

    public static T? GetByName<T>(this ArrayWithCount<T> arr, string name)
        where T : IHasName
        => arr.Values.GetByName(name);

    public static T? GetByNameOrId<T>(this IList<T> arr, string nameOrId)
        where T : IHasName, IHasId
        => arr.FirstOrDefault(x => x.Name == nameOrId) ??
           arr.FirstOrDefault(x => x.Id == nameOrId);

    public static T? GetByNameOrId<T>(this ArrayWithCount<T> arr, string nameOrId)
        where T : IHasName, IHasId
        => arr.Values.GetByNameOrId(nameOrId);
}