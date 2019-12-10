using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Anonimizator.Core.Helpers;
using Anonimizator.Core.Models;

namespace Anonimizator.Core.Services
{
    public class FileService
    {
        public List<Person> GetPeopleData()
        {
            return GetPeopleData(Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, ConstantStrings.FILE_WITH_DATA));
        }

        public List<Person> GetPeopleData(string path)
        {
            var people = new List<Person>();
            using (var reader = new StreamReader(path))
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

        public void SavePeopleData(IEnumerable<Person> people, string fileName)
        {
            using (var textWriter = File.CreateText(fileName))
            {
                foreach (var line in Utils.ToCsv(people))
                {
                    textWriter.WriteLine(line);
                }
            }
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

        public List<Person> GetPeopleDataFromTemporaryFile()
        {
            return GetPeopleData(Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, ConstantStrings.TEMPORARY_DATA));
        }

        public void SavePeopleDataInTemporaryFile(IEnumerable<Person> people)
        {
            using (var textWriter = File.CreateText(Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, ConstantStrings.TEMPORARY_DATA)))
            {
                foreach (var line in Utils.ToCsv(people))
                {
                    textWriter.WriteLine(line);
                }
            }
        }
    }
}
