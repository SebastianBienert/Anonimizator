using System;
using System.Collections.Generic;
using Anonimizator.Core.Models;
using Anonimizator.Core.Services;

namespace Anonimizator.Core.Helpers
{
    public abstract class DataGenerator
    {
        public static List<Person> GenerateNewDataset(FileService fileService, int size)
        {
            var jobs = fileService.GetColumnData(ConstantStrings.FILE_WITH_JOB_GENERALIZATION_DICTIONARY, 0);
            var cities = fileService.GetColumnData(ConstantStrings.FILE_WITH_CITY_GENERALIZATION_DICTIONARY, 0);
            var maleNames = fileService.GetColumnData(ConstantStrings.FILE_WITH_NAME_DICTIONARY, 0);
            var femaleNames = fileService.GetColumnData(ConstantStrings.FILE_WITH_NAME_DICTIONARY, 1);
            var surnames = fileService.GetColumnData(ConstantStrings.FILE_WITH_SURNAME_DICTIONARY, 0);
            List<Person> newData = new List<Person>();
            Random rand = new Random(Guid.NewGuid().GetHashCode());

            for (int i = 0; i < size; i++)
            {
                var gender = rand.Next(0, 2) == 0 ? "M" : "K";
                var age = rand.Next(15, 65).ToString();

                newData.Add(new Person(gender, jobs[rand.Next(0, jobs.Count)], cities[rand.Next(0, cities.Count)],
                                       gender.Equals("M") ? maleNames[rand.Next(0, maleNames.Count)] : femaleNames[rand.Next(0, femaleNames.Count)],
                                       surnames[rand.Next(0, surnames.Count)], age));
            }
            fileService.SavePeopleDataInTemporaryFile(newData);
            return newData;
        }
    }
}
