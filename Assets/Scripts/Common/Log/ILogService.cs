using System.Threading;
using System.Threading.Tasks;

namespace Scripts.Common.Log
{
    public interface ILogService
    {
        void Write(string content);
    }
}
