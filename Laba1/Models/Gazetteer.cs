using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Models
{
    [Serializable]
    public class Gazetteer
    {
        public int Id { get; set; } 
        public string Country { get; set; }
        public double Area { get; set; } 
        public int Population { get; set; }
        public string Continent { get; set; }
        public string Capital { get; set; }

        public override string ToString()
        {
            return $"{Id}: {Country} (Столица: {Capital}, Континент: {Continent}, Площадь: {Area} км², Население: {Population})";
        }
    }
}
