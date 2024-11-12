using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiFiScanner.Models
{
    public class Network
    {
        private string? _name;
        private string? _signalLevel;
        private uint _localId;

        private static uint _id = 0;

        public string? Name 
        {
            get { return _name; } 
            set { _name = value; }
        }
        public string? SignalLevel 
        {
            get { return _signalLevel; }
            set { _signalLevel = value; }
        }

        public uint LocalId
        {
            get => _localId;
        }

        public Network() 
        {
            _id++;
            _localId = _id;
        }
    }
}
