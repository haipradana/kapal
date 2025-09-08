using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapal.Models
{
    internal class Catch
    {
        private int _catchId;
        private int _landingId;
        private string _species;
        private decimal _weightKg;

        public int CatchId
        {
            get => _catchId;
            set => _catchId = value;
        }

        public int LandingId
        {
            get => _landingId;
            set => _landingId = value;
        }

        public string Species
        {
            get => _species;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Nama spesies tidak boleh kosong");
                _species = value;
            }
        }

        public decimal WeightKg
        {
            get => _weightKg;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Berat tidak boleh negatif");
                _weightKg = value;
            }
        }
    }
}
