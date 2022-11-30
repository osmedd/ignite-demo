using Apache.Ignite.Core.Compute;

namespace Demo.Common
{
    public class AssignTopicFunc : IComputeFunc<int>
    {
        public const string FunctionName = nameof(AssignTopicFunc);

        private readonly string _topicId;

        public AssignTopicFunc(string topicId)
        {
            _topicId = topicId;
        }

        public int Invoke()
        {
            Console.WriteLine($"Assign to topic: {_topicId}");
            return 0;
        }
    }
}
