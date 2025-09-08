using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Kapal.Models
{
    internal class Vessel
    {
        private int _vesselId;
        private string _name;
        private string _regNumber;
        private string _ownerName;
        private string _gear;

        public int VesselId
        {
            get => _vesselId;
            set => _vesselId = value;
        }

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Nama kapal tidak boleh kosong");
                _name = value;
            }
        }

        public string RegNumber
        {
            get => _regNumber;
            set => _regNumber = value;
        }

        public string OwnerName
        {
            get => _ownerName;
            set => _ownerName = value;
        }

        public string Gear
        {
            get => _gear;
            set => _gear = value;
        }
    }
}
