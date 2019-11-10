using System.Diagnostics;
using MioMatrix.Extensions;

namespace MioMatrix.Messages
{
    public class RetDevice : BaseMessage
    {
        private const int COMMAND_ID = 0x02;

        public RetDevice()
            : base(true, 0x02, 4, null)
        {
        }

        public static RetDevice Parse(byte[] bytes)
        {
            var baseMessage = BaseMessage.Parse(bytes, COMMAND_ID);

            Debug.WriteLine("Raw output from RetDevice: " + bytes.ToDebugString());

            return (RetDevice) baseMessage;
        }
    }
}