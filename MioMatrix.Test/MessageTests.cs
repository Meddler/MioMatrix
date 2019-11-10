using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MioMatrix.Extensions;
using MioMatrix.Messages;

namespace MioMatrix.Test
{
    [TestClass]
    public class MessageTests
    {
        [TestMethod]
        public void TestGetDevice()
        {
            var message = new GetDevice();
            var bytes = message.ToByteArray();

            var expectedBytes = new List<byte>();
            expectedBytes.AddRange(Constants.HEADER);
            expectedBytes.AddRange(Constants.PRODUCT_ID);
            expectedBytes.AddRange(Constants.SERIAL_NUMBER);
            expectedBytes.AddRange(new byte[] {0x00, 0x00, 0x40, 0x01, 0x00, 0x00, 0x0f, 0xf7});

            Debug.WriteLine(bytes.ToDebugString());
            Debug.WriteLine(expectedBytes.ToArray().ToDebugString());

            Assert.IsTrue(bytes.SequenceEqual(expectedBytes));
        }

        
    }
}
