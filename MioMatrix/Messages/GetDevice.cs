namespace MioMatrix.Messages
{
    public class GetDevice : BaseMessage
    {
        public GetDevice()
            : base(true, 0x01, 0, null)
        {
        }
    }
}