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
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var bytes = BaseMessage.SplitByte(15000);
            int i = BaseMessage.CombineBytes(bytes);

            var command2 = new BaseMessage(true, 0x01, 0, null);
            Debug.WriteLine(command2.ToByteArray().ToDebugString());

            var command3 = new RetMIDIPortRoute(true, 1, new [] {2, 3, 7, 11, 12, 13, 14, 19, 20});
            Debug.WriteLine(command3.ToByteArray().ToDebugString());

            var command4 = RetMIDIPortRoute.Parse(command3.ToByteArray());
            Debug.WriteLine(command4.ToByteArray().ToDebugString());

            var command5 = new GetMIDIPortRoute(1);
            Debug.WriteLine(command5.ToByteArray().ToDebugString());
        }

        
    }
}
