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
            Favorite mockFavorite = new Favorite { Id = 1 };
            var DummyOptions = new DbContextOptionsBuilder<JobContext>().UseInMemoryDatabase(databaseName: "FavControllerDataBase").Options;
            context = new JobContext(DummyOptions);
            context.Favorites.Add(mockFavorite);
            context.SaveChanges();
        }

        [Test]
        public void TestGetFavoriteJobsSuccess()
        {
            Job job = new Job { Id = "mocked" };
            context.Add(job);
            context.SaveChanges();
            _favoriteRepository.GetFavorites(1).Returns(context.Jobs);

            var result = _favoritesController.GetFavoriteJobs(1).Value;

            Assert.AreEqual(context.Jobs.ToList(), result);
        }

        [Test]
        public void TestGetFavoriteJobsContextEmpty()
        {
            ClearDB();
            _favoriteRepository.GetFavorites(11).Returns(context.Jobs);

            var result = _favoritesController.GetFavoriteJobs(11).Value;

            Assert.AreEqual(context.Jobs.ToList(), result);
        }

        [Test]
        public void TestGetFavoriteJobIfIdExist()
        {
            Job job = new Job { Id = "favorite" };
            context.Add(job);
            context.SaveChanges();
            _favoriteRepository.GetFavorites(11).Returns(context.Jobs);

            var result = _favoritesController.GetFavoriteJob("favorite",11).Value;

            Assert.AreEqual(job, result);
        }

        [Test]
        public void TestGetFavoriteJobIfIdNotExist()
        {
            ClearDB();
            _favoriteRepository.GetFavorites(11).Returns(context.Jobs);

            var result = _favoritesController.GetFavoriteJob("favorite", 11).Value;

            Assert.AreEqual(null, result);
        }

        [Test]
        public void TestDeleteFavoriteJobIfIdExist()
        {
            Job job = new Job { Id = "exist1" };
            _favoriteRepository.GetFavId("exist1", 11).Returns(11);
            _favoriteRepository.DeleteFavoriteJob(11).Returns(new Favorite());

            var result = _favoritesController.DeleteFavoriteJob("exist1",11);

            Assert.IsInstanceOf(typeof(NoContentResult), result);
        }

        [Test]
        public void TestDeleteFavoriteJobIfIdNotExist()
        {
            Favorite favorite = null;
            _favoriteRepository.GetFavId("notExist", 11).Returns(11);
            _favoriteRepository.DeleteFavoriteJob(11).Returns(favorite);

            var result = _favoritesController.DeleteFavoriteJob("exist1", 11);

            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }




        private void ClearDB()
        {
            foreach (var item in context.Jobs) context.Jobs.Remove(item);
            context.SaveChanges();
        }
    }
}
