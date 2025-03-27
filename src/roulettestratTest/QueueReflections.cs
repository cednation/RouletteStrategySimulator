namespace roulettestratTest;

using System.Reflection;
using roulettestrat;

internal static class QueueReflections
{
    public static int GetCapacity<T>(this Queue<T> queue)
    {
        var arrayField = typeof(Queue<T>).GetField("_array", BindingFlags.NonPublic | BindingFlags.Instance);
        var array = (T[]?)arrayField!.GetValue(queue);
        return array?.Length ?? 0;
    }

    public static int GetHistoryQueueCapacity(this Wheel wheel)
    {
        var historyField = typeof(Wheel).GetField("history", BindingFlags.NonPublic | BindingFlags.Instance);
        var historyQueue = (Queue<SpinResult>?)historyField!.GetValue(wheel);
        return historyQueue?.GetCapacity() ?? 0;
    }
}