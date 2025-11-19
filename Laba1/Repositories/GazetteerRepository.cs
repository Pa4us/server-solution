using DataLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Repositories
{
    public class GazetteerRepository
    {
        private readonly string _filePath;
        private List<Gazetteer> _gazetteers;

        public GazetteerRepository(string filePath)
        {
            _filePath = filePath;
            LoadData();
        }

        private void LoadData()
        {
            _gazetteers = new List<Gazetteer>();

            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "Id;Country;Area;Population;Continent;Capital");
                return;
            }

            var lines = File.ReadAllLines(_filePath);
            for (int i = 1; i < lines.Length; i++) 
            {
                var parts = lines[i].Split(';');
                if (parts.Length == 6)
                {
                    _gazetteers.Add(new Gazetteer
                    {
                        Id = int.Parse(parts[0]),
                        Country = parts[1],
                        Area = double.Parse(parts[2]),
                        Population = int.Parse(parts[3]),
                        Continent = parts[4],
                        Capital = parts[5]
                    });
                }
            }
        }

        private void SaveData()
        {
            var lines = new List<string> { "Id;Country;Area;Population;Continent;Capital" };
            lines.AddRange(_gazetteers.Select(g =>
                $"{g.Id};{g.Country};{g.Area};{g.Population};{g.Continent};{g.Capital}"));

            File.WriteAllLines(_filePath, lines);
        }

        public List<Gazetteer> GetAll() => _gazetteers;

        public Gazetteer GetById(int id) => _gazetteers.FirstOrDefault(g => g.Id == id);

        public void Add(Gazetteer gazetteer)
        {
            gazetteer.Id = _gazetteers.Count > 0 ? _gazetteers.Max(g => g.Id) + 1 : 1;
            _gazetteers.Add(gazetteer);
            SaveData();
        }

        public void Update(Gazetteer gazetteer)
        {
            var existing = GetById(gazetteer.Id);
            if (existing != null)
            {
                existing.Country = gazetteer.Country;
                existing.Area = gazetteer.Area;
                existing.Population = gazetteer.Population;
                existing.Continent = gazetteer.Continent;
                existing.Capital = gazetteer.Capital;
                SaveData();
            }
        }

        public void Delete(int id)
        {
            var gazetteer = GetById(id);
            if (gazetteer != null)
            {
                _gazetteers.Remove(gazetteer);
                SaveData();
            }
        }

        public List<Gazetteer> GetByContinent(string continent) =>
            _gazetteers.Where(g => g.Continent.Equals(continent, StringComparison.OrdinalIgnoreCase)).ToList();

        public List<Gazetteer> GetByPopulationRange(int min, int max) =>
            _gazetteers.Where(g => g.Population >= min && g.Population <= max).ToList();
    }
}

