using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoolJobAPI.Controllers;
using CoolJobAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;

namespace CoolJobAPI.UnitTests
{
    class JobsControllerTest
    {
        IJobRepository _jobRepository;
        JobContext context;
        JobsController _jobsController;
        public JobsControllerTest()
        {
            _jobRepository = Substitute.For<IJobRepository>();
            _jobsController = new JobsController(_jobRepository);
            CreateContext();
        }

        private void CreateContext()
        {
            Job mockJob = new Job { Id = "mock"};
            var DummyOptions = new DbContextOptionsBuilder<JobContext>().UseInMemoryDatabase(databaseName: "JobDataBase").Options;
            context = new JobContext(DummyOptions);
            context.Jobs.Add(mockJob);
            context.SaveChanges();

        }

        [Test] //I call GetJobs() method after I filled the database I should get back list of jobs. 
        public void TestGetJobsIfJobsNotEmpty()
        {
            _jobRepository.GetJobs().Returns(context.Jobs);
            
            var result = _jobsController.GetJobs().Value;

            Assert.AreEqual(context.Jobs.ToList(), result);
        }

        [Test]
        public void TestGetJobsIfJobsIsEmpty()
        {
            foreach (var item in context.Jobs) context.Jobs.Remove(item);
            context.SaveChanges();
            _jobRepository.GetJobs().Returns(context.Jobs);

            var result = _jobsController.GetJobs().Value;

            Assert.AreEqual(context.Jobs.ToList(), result);
        }

        [Test]
        public void TestGetJobIfIdIsExist()
        {
            Job mockJob = new Job { Id = "mock1" };
            context.Jobs.Add(mockJob);
            context.SaveChanges();
            _jobRepository.GetJobById("mock1").Returns(mockJob);

            var result = _jobsController.GetJob("mock1").Value;

            Assert.AreEqual(mockJob, result);
        }
    }
}
