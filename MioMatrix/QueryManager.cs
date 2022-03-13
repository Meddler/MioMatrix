using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Midi;
using MioMatrix.Extensions;
using MioMatrix.Messages;

namespace MioMatrix
{
    public class QueryManager : IDisposable
    {
        private InputDevice _inputDevice;
        private OutputDevice _outputDevice;
        private BlockingCollection<RetMIDIPortRoute> _retMidiPortRoutes;
        private static QueryManager _instance;

        public static QueryManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new QueryManager();
                }

                return _instance;
            }
        }

        public QueryManager()
        {
            _retMidiPortRoutes = new BlockingCollection<RetMIDIPortRoute>();

            if (InputDevice.InstalledDevices.Count > 0)
            {
                _inputDevice = InputDevice.InstalledDevices.FirstOrDefault(x => x.Name == "DIN 1");

                if (_inputDevice == null)
                {
                    throw new Exception(string.Format("Could not get input device. Count: {0} Available devices: {1}", Win32API.midiInGetNumDevs(), string.Join(", ", InputDevice.InstalledDevices.Select(x => x.Name))));
                }

                _inputDevice.SysEx += HandleSysex;
                _inputDevice.Open();
                _inputDevice.StartReceiving(null, true);
                _outputDevice = OutputDevice.InstalledDevices.FirstOrDefault(x => x.Name == "DIN 1");

                if (_outputDevice == null)
                {
                    throw new Exception("Could not get output device.");
                }

                _outputDevice.Open();
            }
        }
        
        public void Dispose()
        {
            _retMidiPortRoutes.Dispose();

            if (_inputDevice != null)
            {
                if (_inputDevice.IsReceiving)
                {
                    _inputDevice.StopReceiving();
                }
                if (_inputDevice.IsOpen)
                {
                    _inputDevice.Close();
                }
            }
            if (_outputDevice != null && _outputDevice.IsOpen)
            {
                _outputDevice.Close();
            }
        }

        public RetMIDIPortRoute GetMidiPortRoute(int sourcePort)
        {
            if (_outputDevice != null)
            {
                _outputDevice.SendSysEx(new GetMIDIPortRoute(sourcePort).ToByteArray());
            }
            else
            {
                //Mock code
                //var msg = RetMIDIPortRoute.Parse(new byte[]
                //{
                //    0xF0, 0x00, 0x01, 0x73, 0x7E, 0x00, 0x02, 0x00, 0x00, 0x00, 0x05, 0x29, 0x00, 0x00, 0x00, 0x29, 0x00,
                //    0x11, 0x01, 0x00, 0x01, 0x0E, 0x0F, 0x07, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x0C, 0x0F, 0x0F,
                //    0x0F, 0x33, 0xF7
                //});
                //_retMidiPortRoutes.Add(msg);
            }

            if (_outputDevice != null)
            {
                RetMIDIPortRoute message;
                if (!_retMidiPortRoutes.TryTake(out message, 5000))
                {
                    throw new TimeoutException("Waiting for RetMIDIPortRoute message timed out.");
                }

                return message;
            }

            return null;
        }

        public void SendMidiPortRoute(int sourcePort, IEnumerable<int> destinationPorts)
        {
            if (_outputDevice != null)
            {
                _outputDevice.SendSysEx(new RetMIDIPortRoute(true, sourcePort, destinationPorts).ToByteArray());
            }
        }

        private void HandleSysex(SysExMessage msg)
        {
            Debug.WriteLine(msg.Data.ToDebugString());
            var retMIDIPortRouteMessage = RetMIDIPortRoute.Parse(msg.Data);

            if (retMIDIPortRouteMessage != null)
            {
                _retMidiPortRoutes.Add(retMIDIPortRouteMessage);
            }
            
        }
    }
}