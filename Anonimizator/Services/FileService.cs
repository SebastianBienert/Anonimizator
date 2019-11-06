using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anonimizator.Models;
using Microsoft.Win32;

namespace Anonimizator.Services
{
    public class FileService
    {
        public List<Person> GetPeopleData(string path)
        {
            var people = new List<Person>();
            using (var reader = new StreamReader(Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, path)))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var person = Utils.PersonFromCsv(line);
                    people.Add(person);
                }
            }
            return people;
        }

        public void SavePeopleData(IEnumerable<Person> people, string defaultFileName, bool withDialog = true)
        {
            string selectedFileName;
            if (withDialog)
                selectedFileName = GetSelectedFileName(defaultFileName);
            else
                selectedFileName = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, defaultFileName);

            using (var textWriter = File.CreateText(selectedFileName))
            {
                foreach (var line in Utils.ToCsv(people))
                {
                    textWriter.WriteLine(line);
                }
            }
        }

        private string GetSelectedFileName(string defaultFileName)
        {
            var sfd = new SaveFileDialog
            {
                Filter = "Text Files (*.csv)|*.csv|All files (*.*)|*.*",
            };
            if (sfd.ShowDialog() == true)
            {
                return sfd.FileName;
            }

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, defaultFileName);
        }

        public List<List<string>> GetDictionaryData(string path)
        {
            var dictionary = new List<List<string>>();
            using (var reader = new StreamReader(Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, path)))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if(!string.IsNullOrEmpty(line))
                        dictionary.Add(line.Split(';').ToList());
                }
            }
            return dictionary;
        }

        public List<string> GetColumnData(string path, int columnNumber)
        {
            if (columnNumber < 0 || string.IsNullOrEmpty(path))
                return null;

            var column = new List<string>();
            using (var reader = new StreamReader(Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, path)))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                        column.Add(line.Split(';').ToList()[columnNumber]);
                }
            }
            return column;
        }
    }
}
