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
            Job mockJob = new Job { Id = "mockk" };
            var DummyOptions = new DbContextOptionsBuilder<JobContext>().UseInMemoryDatabase(databaseName: "JobControllerDataBase").Options;
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
            ClearDB();
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

        [Test]
        public void TestGetJobIfIdIsNotExist()
        {
            Job job = null;
            ClearDB();
            _jobRepository.GetJobById("mock").Returns(job);

            var result = _jobsController.GetJob("mock");

           // Assert.AreEqual(NotFound(), result);
            Assert.IsInstanceOf(typeof(NotFoundResult), result.Result);
        }

        [Test]
        public void TestDeleteJobNotExistID()
        {
            Job job = null;
            _jobRepository.DeleteJobById("notExist").Returns(job);

            var result = _jobsController.DeleteJob("notExist");

            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }

        [Test]
        public void TestDeleteJobExistID()
        {
            Job job = new Job { Id = "exist" };
            _jobRepository.DeleteJobById("exist").Returns(job);

            var result = _jobsController.DeleteJob("exist");

            Assert.IsInstanceOf(typeof(NoContentResult), result);
        }

        //[Test]
        //public void TestPostJobIsSuccess()
        //{
        //    Job mockJob = new Job { Id = "mock2" };
        //    _jobRepository.GetJobById("mock2").Returns(mockJob);
            

        //    var result = _jobsController.PostJob(mockJob);

        //    Assert.AreEqual(mockJob, result.Value);

        //}

        private void ClearDB()
        {
            foreach (var item in context.Jobs) context.Jobs.Remove(item);
            context.SaveChanges();
        }
    }
}
