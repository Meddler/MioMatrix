namespace MioMatrix.Messages
{
    public static class Constants
    {
        public static readonly byte[] HEADER = new byte[] { 0xF0, 0x00, 0x01, 0x73, 0x7e };
        public static readonly byte[] PRODUCT_ID = new byte[] { 0x00, 0x02 };
        public static readonly byte[] SERIAL_NUMBER = new byte[] { 0x00, 0x00, 0x00, 0x05, 0x29 };
    }
}