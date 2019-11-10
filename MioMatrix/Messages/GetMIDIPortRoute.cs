namespace MioMatrix.Messages
{
    public class GetMIDIPortRoute : BaseMessage
    {
        public GetMIDIPortRoute(int sourcePort)
            : base(true, 0x28, 2, null)
        {
            SourcePort = sourcePort;
        }

        public int SourcePort { get; }

        public override byte[] Data
        {
            get { return SplitByte(SourcePort); }
        }
    }
}