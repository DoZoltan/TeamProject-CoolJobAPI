using CoolJobAPI.Controllers;
using CoolJobAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolJobAPI.UnitTests.ControllersTest
{
    class FavoritesControllerTest
    {
        IFavoriteRepository _favoriteRepository;
        JobContext context;
        FavoritesController _favoritesController;
        public FavoritesControllerTest()
        {
            _favoriteRepository = Substitute.For<IFavoriteRepository>();
            _favoritesController = new FavoritesController(_favoriteRepository);
            CreateContext();
        }

        private void CreateContext()
        {
            Favorite mockFavorite = new Favorite { Id = 11 };
            var DummyOptions = new DbContextOptionsBuilder<JobContext>().UseInMemoryDatabase(databaseName: "FavControllerDataBase").Options;
            context = new JobContext(DummyOptions);
            context.Favorites.Add(mockFavorite);
            context.SaveChanges();
        }

        [Test]
        public async void TestGetFavoriteJobsSuccess()
        {
            Job job = new Job { Title = "mocked" };
            context.Add(job);
            context.SaveChanges();
            _favoriteRepository.GetFavorites(11).Returns(context.Jobs);

            var favJob = await _favoritesController.GetFavoriteJobs(11);

            var result = favJob.Value;

            Assert.AreEqual(context.Jobs.ToList(), result);
        }

        [Test]
        public async void TestGetFavoriteJobsContextEmpty()
        {
            ClearDB();
            _favoriteRepository.GetFavorites(11).Returns(context.Jobs);

            var favJob = await _favoritesController.GetFavoriteJobs(11);

            var result = favJob.Value;

            Assert.AreEqual(context.Jobs.ToList(), result);
        }

        [Test]
        public async void TestGetFavoriteJobIfIdExist()
        {
            Job job = new Job { Type = "favorite" };
            context.Add(job);
            context.SaveChanges();
            _favoriteRepository.GetFavorites(1).Returns(context.Jobs);

            var favJob = await _favoritesController.GetFavoriteJob(1,1);

            var result = favJob.Value;

            Assert.AreEqual(job, result);
        }

        [Test]
        public async void TestGetFavoriteJobIfIdNotExist()
        {
            ClearDB();
            _favoriteRepository.GetFavorites(11).Returns(context.Jobs);

            var favJob = await _favoritesController.GetFavoriteJob(2, 11);

            var result = favJob.Value;

            Assert.AreEqual(null, result);
        }

        [Test]
        public void TestDeleteFavoriteJobIfIdExist()
        {
            Job job = new Job { Title = "exist1" };
            _favoriteRepository.GetFavId(2, 11).Returns(11);
            _favoriteRepository.DeleteFavoriteJob(11).Returns(new Favorite());

            var result = _favoritesController.DeleteFavoriteJob(2,11);

            Assert.IsInstanceOf(typeof(NoContentResult), result);
        }

        [Test]
        public void TestDeleteFavoriteJobIfIdNotExist()
        {
            Favorite favorite = null;
            _favoriteRepository.GetFavId(20, 11).Returns(11);
            _favoriteRepository.DeleteFavoriteJob(11).Returns(favorite);

            var result = _favoritesController.DeleteFavoriteJob(20, 11);

            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }




        private void ClearDB()
        {
            foreach (var item in context.Jobs) context.Jobs.Remove(item);
            context.SaveChanges();
        }
    }
}
