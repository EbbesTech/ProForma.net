using ProForma.Shared.Plugins;

namespace ProForma.Common.Extensions;

public static class EnumerableExtensions
{
    extension<T>(IEnumerable<T> enumerable)
    {
        public void ForEach(Action<T> action)
        {
            foreach(var enumerableItem in enumerable)
            {
                action(enumerableItem);
            }
        }
    }
}
