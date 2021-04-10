using NUnit.Framework;
using CoolJobAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CoolJobAPI.UnitTests
{

    [TestFixture]
    public class FavoriteRepositoryTest
    {

        IFavoriteRepository favoriteRepository;
        JobContext context;

        public FavoriteRepositoryTest()
        {
            CreateContext();
            favoriteRepository = new FavoriteRepository(context);
        }

        private void CreateContext()
        {
            User mockUser = new User { Id = 1, Password = "mock", PasswordSalt = "mock", UserName = "mock" };
            User mockUser2 = new User { Id = 2, Password = "mock", PasswordSalt = "mock", UserName = "mock" };
            User mockUser3 = new User { Id = 3, Password = "mock", PasswordSalt = "mock", UserName = "mock" };

            Job mockJob = new Job
            {
                Id = "mock",
                Company = "mock",
                Company_Logo = "mock",
                Company_Url = "mock",
                Created_At = "mock",
                Description = "mock",
                How_To_Apply = "mock",
                Location = "mock",
                Title = "mock",
                Type = "mock",
                Url = "mock",
                User = null
            };

            Favorite fav = new Favorite { Id = 1, Job = mockJob, User = mockUser2 };
            Favorite fav2 = new Favorite { Id = 2, Job = mockJob, User = mockUser3 };
            Favorite fav3 = new Favorite { Id = 3, Job = mockJob, User = mockUser3 };

            var DummyOptions = new DbContextOptionsBuilder<JobContext>().UseInMemoryDatabase(databaseName: "JobDataBase").Options;

            context = new JobContext(DummyOptions);
            context.Favorites.Add(fav);
            context.Favorites.Add(fav2);
            context.Favorites.Add(fav3);

            context.Users.Add(mockUser);
            context.Users.Add(mockUser2);
            context.Users.Add(mockUser3);
            
            context.Jobs.Add(mockJob);
            
            context.SaveChanges();
        }

        [Test] //GetFavorites() should return empty collection if the provided user ID is not exists
        public void TestGetFavoritesForNonExistentUserId()
        {

            int nonExistentUserId = 11;
            int expectedCollectionSize = 0;

            var favorite = favoriteRepository.GetFavorites(nonExistentUserId).ToList();

            Assert.AreEqual(expectedCollectionSize, favorite.Count);
        }

        [Test] //GetFavorites() should return empty collection if the user is don't have any favorite
        public void TestGetFavoritesIfTheUserNotHaveFavorites()
        {
            int userId = 1;
            int expectedCollectionSize = 0;

            var favorite = favoriteRepository.GetFavorites(userId).ToList();

            Assert.AreEqual(expectedCollectionSize, favorite.Count);
        }

        [Test] //GetFavorites() should return a collection with one favorite if the user have only one favorite
        public void TestGetFavoritesIfTheUserHaveOneFavorite()
        {
            int userId = 2;
            int expectedCollectionSize = 1;

            var favorite = favoriteRepository.GetFavorites(userId).ToList();

            Assert.AreEqual(expectedCollectionSize, favorite.Count);
        }

        [Test] //GetFavorites() should return a collection of favorites if the user have more than one favorite
        public void TestGetFavoritesIfTheUserHaveMoreFavorites()
        {
            int userId = 3;
            int expectedCollectionSize = 2;

            var favorite = favoriteRepository.GetFavorites(userId).ToList();

            Assert.AreEqual(expectedCollectionSize, favorite.Count);
        }
    }
}