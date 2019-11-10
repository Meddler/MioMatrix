using System;
using System.Collections.Generic;
using System.Linq;

namespace MioMatrix.Messages
{
    public class BaseMessage
    {
        

        public BaseMessage(bool write, int commandID, int dataLength, byte[] data)
        {
            Write = write;
            CommandID = commandID;
            DataLength = dataLength;
            Data = data;
        }

        public byte[] Header { get { return Constants.HEADER; } }
        public bool Write { get; }
        public int CommandID { get; }
        public int DataLength { get; }
        public virtual byte[] Data { get; }
        public byte[] Footer { get { return new byte[] {0xf7}; } }

        public static BaseMessage Parse(byte[] bytes, int expectedCommandID)
        {
            var header = bytes.Take(5);
            if (!header.SequenceEqual(Constants.HEADER))
            {
                return null;
            }

            

            var body = bytes.Skip(5).Take(bytes.Length - 6).ToList();

            if (SumBytes(body) != 0x00)
            {
                throw new ArithmeticException("Invalid checksum");
            }

            var queue = new Queue<byte>(body.Take(body.Count - 1));
            var productID = new[] {queue.Dequeue(), queue.Dequeue()};
            var serialNumber = new[] { queue.Dequeue(), queue.Dequeue(), queue.Dequeue(), queue.Dequeue(), queue.Dequeue() };
            var transactionID = new[] { queue.Dequeue(), queue.Dequeue() };
            var command = CombineBytes(new [] {queue.Dequeue(), queue.Dequeue()});
            var commandID = command & 0x3FF;

            if (commandID != expectedCommandID)
            {
                return null;
            }

            var write = ((command & 0x2000) >> 13) == 1;
            var dataLength = CombineBytes(new[] {queue.Dequeue(), queue.Dequeue()});
            var data = queue.ToArray();

            return new BaseMessage(write, commandID, dataLength, data);
        }

        public byte[] ToByteArray()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(Header);

            var body = new List<byte>();

            body.AddRange(Constants.PRODUCT_ID);
            body.AddRange(Constants.SERIAL_NUMBER);
            body.AddRange(GenerateTransactionID());

            body.AddRange(ConstructCommand(Write, CommandID));
            body.AddRange(SplitByte(DataLength));

            if (Data != null)
            {
                body.AddRange(Data);
            }

            var sum = SumBytes(body);
            byte checksum = (byte)(~sum + 1);
            int chk = (int)checksum;
            chk = chk % 128;
            body.Add((byte)chk);

            if (SumBytes(body) != 0x00)
            {
                throw new ArithmeticException("Invalid checksum");
            }

            bytes.AddRange(body);
            bytes.AddRange(Footer);

            return bytes.ToArray();
        }

        private static byte[] ConstructCommand(bool write, int commandID)
        {
            int value = commandID;

            int mask = 1 << 13;
            if (write)
            {
                value |= mask;
            }
            else
            {
                value &= ~mask;
            }

            return SplitByte(value);
        }

        private static byte SumBytes(IEnumerable<byte> bytes)
        {
            int sum = 0;
            foreach (byte b in bytes)
            {
                sum += b;
            }

            sum %= 128;

            return (byte)sum;
        }

        private static byte[] GenerateTransactionID()
        {
            return new byte[] { 0x00, 0x00 };

            var rand = new Random();
            var value = rand.Next(0, 16383);
            return SplitByte(value);
        }

        public static byte[] SplitByte(int x)
        {
            if (x < 128)
                return new byte[] { 0x00, (byte)x };

            List<Byte> Result = new List<byte>();
            do
            {
                int tmp = x & 0x7f;
                x = x >> 7;
                Result.Add((byte)tmp);
            } while (x > 0);
            Result.Reverse();
            return Result.ToArray();
        }

        public static int CombineBytes(byte[] bytes)
        {
            return (bytes[0] << 7 | bytes[1]);
        }
    }
}