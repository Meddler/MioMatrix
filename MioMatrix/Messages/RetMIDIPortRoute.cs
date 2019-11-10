using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace MioMatrix.Messages
{
    public class RetMIDIPortRoute : BaseMessage
    {
        private const int NUMBER_OF_PORTS = 56;
        public const int COMMAND_ID = 0x29;

        public RetMIDIPortRoute(bool write, int sourcePort, IEnumerable<int> destinationPorts)
            :base(write, COMMAND_ID, 17, null)
        {
            SourcePort = sourcePort;
            DestinationPorts = destinationPorts;
        }

        public int CommandVersion { get { return 1; } }
        public int SourcePort { get; }
        public IEnumerable<int> DestinationPorts { get; }

        public override byte[] Data
        {
            get
            {
                var bytes = new List<byte>();

                bytes.Add((byte) CommandVersion);
                bytes.AddRange(SplitByte(SourcePort));

                var numberOfBytes = (((NUMBER_OF_PORTS - 1)/8) + 1)*2;
                List<byte> portBytes = new List<byte>();
                int currentByte = 0x00;
                for (int i = 0; i < numberOfBytes * 4; i++)
                {
                    var bitIndex = i%4;

                    if (DestinationPorts.Contains(i + 1))
                    {
                        int mask = 1 << bitIndex;
                        currentByte |= mask;
                    }

                    if (i%4 == 3)
                    {
                        portBytes.Add((byte) currentByte);
                        currentByte = 0x00;
                    }
                }

                bytes.AddRange(portBytes);

                return bytes.ToArray();
            }
        }

        public static RetMIDIPortRoute Parse(byte[] bytes)
        {
            var baseMessage = BaseMessage.Parse(bytes, COMMAND_ID);

            if (baseMessage == null)
            {
                return null;
            }

            var body = new Queue<byte>(baseMessage.Data);

            int commandVersion = body.Dequeue();
            int sourcePort = CombineBytes(new byte[] {body.Dequeue(), body.Dequeue()});

            var destinationPorts = new List<int>();
            var portBytes = body.ToArray();
            for (int i = 0; i < portBytes.Length; i++)
            {
                var b = portBytes[i];

                if ((b & 1) >> 0 == 1)
                {
                    destinationPorts.Add(1 + (i * 4));
                }

                if ((b & 2) >> 1 == 1)
                {
                    destinationPorts.Add(2 + (i * 4));
                }

                if ((b & 4) >> 2 == 1)
                {
                    destinationPorts.Add(3 + (i * 4));
                }

                if ((b & 8) >> 3 == 1)
                {
                    destinationPorts.Add(4 + (i * 4));
                }
            }

            return new RetMIDIPortRoute(baseMessage.Write, sourcePort, destinationPorts);
        }
    }
}