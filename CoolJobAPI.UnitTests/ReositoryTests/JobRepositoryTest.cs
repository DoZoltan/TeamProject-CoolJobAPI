using CoolJobAPI.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolJobAPI.UnitTests.RepositoryTests
{
    class JobRepositoryTest
    {
        IJobRepository jobRepository;
        JobContext context;

        public JobRepositoryTest()
        {
            CreateContext();
            jobRepository = new JobRepository(context);
        }

        private void CreateContext()
        {
            var DummyOptions = new DbContextOptionsBuilder<JobContext>().UseInMemoryDatabase(databaseName: "JobRepoDataBase").Options;
            context = new JobContext(DummyOptions);

            Job mockJob = new Job { Location = "Debrecen" };
            Job mockJob2 = new Job { Location = "Debrecen" };
            Job mockJob3 = new Job { Location = "Budapest" };
            Job mockJob4 = new Job { Location = "Budapest" };
            Job mockJob5 = new Job { Location = "Miskolc" };
            Job mockJob6 = new Job { Location = "Budapest" };
            Job mockJob7 = new Job { Location = "Miskolc" };
            Job mockJob8 = new Job { Location = "Pécs" };
            Job mockJob9 = new Job { Location = "Pécs" };
            Job mockJob10 = new Job { Location = "Gödöllő" };
            Job mockJob11 = new Job { Location = "Gödöllő" };
            Job mockJob12 = new Job { Location = "Gödöllő" };

            context.Jobs.Add(mockJob);
            context.Jobs.Add(mockJob2);
            context.Jobs.Add(mockJob3);
            context.Jobs.Add(mockJob4);
            context.Jobs.Add(mockJob5);
            context.Jobs.Add(mockJob6);
            context.Jobs.Add(mockJob7);
            context.Jobs.Add(mockJob8);
            context.Jobs.Add(mockJob9);
            context.Jobs.Add(mockJob10);
            context.Jobs.Add(mockJob11);
            context.Jobs.Add(mockJob12);

            context.SaveChanges();
        }

        [Test] //GetJobs() should return all of the jobs what was added to the database previously
        public void TestGetJobs()
        {
            int expectedCollectionSize = 12;

            int result = jobRepository.GetJobs().Count();

            Assert.AreEqual(expectedCollectionSize, result);
        }

        [Test] //GetJobById() should return a specific job what was added to the database previously
        public void TestGetJobById()
        {
            int expectedJobId = 1;

            var result = jobRepository.GetJobById(1);

            Assert.AreEqual(expectedJobId, result.Id);
        }

        [Test] //GetJobById() should return null if the provided job id is wrong
        public void TestGetJobByIdIfTheJobIdNotExists()
        {
            var result = jobRepository.GetJobById(111111);

            Assert.AreEqual(null, result);
        }

        [Test] //GetJobsByRange() should return jobs in a specific range (1 --> max 10, 2 --> max 20, 3 --> max 30)
        public void TestGetJobsByRangeWithPageNumOne()
        {
            int expectedCollectionSize = 10;
            int pageNum = 1;

            int result = jobRepository.GetJobsByRange(pageNum).Count();

            Assert.AreEqual(expectedCollectionSize, result);
        }

        [Test] //GetJobsByRange() should return jobs in a specific range (1 --> max 10, 2 --> max 20, 3 --> max 30)
        public void TestGetJobsByRangeWithPageNumTwo()
        {
            int expectedCollectionSize = 12;
            int pageNum = 2;

            int result = jobRepository.GetJobsByRange(pageNum).Count();

            Assert.AreEqual(expectedCollectionSize, result);
        }

        [Test] //GetJobsByRange() should return jobs collection what have max ten element
        public void TestGetJobsByRangeWithLessThanOnePageNumber()
        {
            int expectedCollectionSize = 10;
            int pageNum = -2;

            int result = jobRepository.GetJobsByRange(pageNum).Count();

            Assert.AreEqual(expectedCollectionSize, result);
        }

        [Test] //DeleteJobById() should return the job what was deleted
        public void TestDeleteJobById()
        {
            Job mockJob13 = new Job { Location = "mock" }; // the ID will be 13
            context.Jobs.Add(mockJob13);
            context.SaveChanges();

            var result = jobRepository.DeleteJobById(13);

            Assert.AreEqual(mockJob13, result);
        }

        [Test] //DeleteJobById() should return null if the provided id is wrong
        public void TestDeleteJobByIdIfJobIdIsNotExists()
        {
            var result = jobRepository.DeleteJobById(99999);

            Assert.AreEqual(null, result);
        }

        [Test] //GetFilteredJobs() should return only the jobs that contains the provided keyword
        public void TestGetFilteredJobs()
        {
            string filterBy = "lOcAtIoN"; //for the case insensitivity test
            string filterValue = "BuDaPeSt"; //for the case insensitivity test 
            int pageNum = 1;
            int expectedCollectionSize = 3;

            var result = jobRepository.GetFilteredJobs(filterBy, filterValue, pageNum).Count();

            Assert.AreEqual(expectedCollectionSize, result);
        }

        [Test] //GetFilteredJobs() should return empty collection if the provided filter type (property name) not exists
        public void TestGetFilteredJobsIfTheProvidedFilterTypeNotExists()
        {
            string filterBy = "Size";
            string filterValue = "Budapest";
            int pageNum = 1;
            int expectedCollectionSize = 0;

            var result = jobRepository.GetFilteredJobs(filterBy, filterValue, pageNum).Count();

            Assert.AreEqual(expectedCollectionSize, result);
        }

        [Test] //GetFilteredJobs() should return empty collection if the provided keyword not exists
        public void TestGetFilteredJobsIfTheProvidedFilterValueExists()
        {
            string filterBy = "Location";
            string filterValue = "TestLocation";
            int pageNum = 1;
            int expectedCollectionSize = 0;

            var result = jobRepository.GetFilteredJobs(filterBy, filterValue, pageNum).Count();

            Assert.AreEqual(expectedCollectionSize, result);
        }

        [Test] //GetFilteredJobs() should return empty collection if the provided keyword and provided filter type (property name) is empty,
               //and the page number less than 1
        public void TestGetFilteredJobsIfTheProvidedFilterValueAndFilterTypeIsEmptyAndThePageNumberLessThanOne()
        {
            string filterBy = "";
            string filterValue = "";
            int pageNum = -1;
            int expectedCollectionSize = 0;

            var result = jobRepository.GetFilteredJobs(filterBy, filterValue, pageNum).Count();

            Assert.AreEqual(expectedCollectionSize, result);
        }

        [Test] //GetFilteredJobs() should return unique values what the provided filter types (property) have
        public void TestGetSpecificFilterValuesByFilterType()
        {
            string filterBy = "lOcAtIoN"; //for the case insensitivity test
            int expectedCollectionSize = 5;

            var result = jobRepository.GetSpecificFilterValuesByFilterType(filterBy).Count();

            Assert.AreEqual(expectedCollectionSize, result);
        }

        [Test] //GetFilteredJobs() should return null if the provided filter type is wrong
        public void TestGetSpecificFilterValuesByFilterTypeIfTheFilterTypeIsWrong()
        {
            string filterBy = "TypeTest";

            var result = jobRepository.GetSpecificFilterValuesByFilterType(filterBy);

            Assert.AreEqual(null, result.FirstOrDefault());
        }
    }
}
