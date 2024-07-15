using Cysharp.Threading.Tasks;

namespace Utility
{
    static public class TaskUtility
    {
        static public async UniTask WaitTasks(params UniTask[] taskList)
        {
            foreach( var task in taskList )
                await task;
        }
    }

}
