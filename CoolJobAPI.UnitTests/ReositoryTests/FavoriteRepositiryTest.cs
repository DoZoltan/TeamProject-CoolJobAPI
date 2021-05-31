using NUnit.Framework;
using CoolJobAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CoolJobAPI.UnitTests.RepositoryTests
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
            User mockUser = new User { Id = 1, UserName = "mock" };
            User mockUser2 = new User { Id = 2, UserName = "mock" };
            User mockUser3 = new User { Id = 3, UserName = "mock" };
            User mockUser4 = new User { Id = 4, UserName = "mock" };

            Job mockJob = new Job { User = mockUser };
            Job mockJob2 = new Job { User = mockUser };

            Favorite fav = new Favorite { Id = 1, Job = mockJob, User = mockUser2 };
            Favorite fav2 = new Favorite { Id = 2, Job = mockJob, User = mockUser3 };
            Favorite fav3 = new Favorite { Id = 3, Job = mockJob, User = mockUser3 };

            var DummyOptions = new DbContextOptionsBuilder<JobContext>().UseInMemoryDatabase(databaseName: "FavRepoDataBase").Options;

            context = new JobContext(DummyOptions);
            context.Favorites.Add(fav);
            context.Favorites.Add(fav2);
            context.Favorites.Add(fav3);

            context.Users.Add(mockUser);
            context.Users.Add(mockUser2);
            context.Users.Add(mockUser3);
            context.Users.Add(mockUser4);

            context.Jobs.Add(mockJob);
            context.Jobs.Add(mockJob2);

            context.SaveChanges();
        }

        [Test] //GetFavorites() should return empty collection if the provided user ID is not exists
        public async void TestGetFavoritesForNonExistentUserId()
        {

            int nonExistentUserId = 11;
            int expectedCollectionSize = 0;

            var favorite = await favoriteRepository.GetFavorites(nonExistentUserId);

            Assert.AreEqual(expectedCollectionSize, favorite.ToList().Count);
        }

        [Test] //GetFavorites() should return empty collection if the user is don't have any favorite
        public async void TestGetFavoritesIfTheUserNotHaveFavorites()
        {
            int userId = 1;
            int expectedCollectionSize = 0;

            var favorite = await favoriteRepository.GetFavorites(userId);

            Assert.AreEqual(expectedCollectionSize, favorite.ToList().Count);
        }

        [Test] //GetFavorites() should return a collection with one favorite if the user have only one favorite
        public async void TestGetFavoritesIfTheUserHaveOneFavorite()
        {
            int userId = 2;
            int expectedCollectionSize = 1;

            var favorite = await favoriteRepository.GetFavorites(userId);

            Assert.AreEqual(expectedCollectionSize, favorite.ToList().Count);
        }

        [Test] //GetFavorites() should return a collection of favorites if the user have more than one favorite
        public async void TestGetFavoritesIfTheUserHaveMoreFavorites()
        {
            int userId = 3;
            int expectedCollectionSize = 2;

            var favorite = await favoriteRepository.GetFavorites(userId);

            Assert.AreEqual(expectedCollectionSize, favorite.ToList().Count);
        }

        [Test] //After AddToFavorites() method called then the new favorite should appear in the database
        public async void TestAddToFavorites()
        {
            int userId = 4;
            int expectedCollectionSize = 1;
            int theIdOfTheSelectedJob = 1;

            favoriteRepository.AddToFavorites(theIdOfTheSelectedJob, userId);

            var favorite = await favoriteRepository.GetFavorites(userId);

            Assert.AreEqual(expectedCollectionSize, favorite.ToList().Count);
        }

        [Test] //AddToFavorites() method should return false if it get wrong (non existed) user ID 
        public void TestIfAddToFavoritesGetWrongUserId()
        {
            int nonExistedUserId = 40;
            bool expectedResult = false;
            int theIdOfTheSelectedJob = 1;

            var result = favoriteRepository.AddToFavorites(theIdOfTheSelectedJob, nonExistedUserId);

            Assert.AreEqual(expectedResult, result);
        }

        [Test] //DeleteFavoriteJob() method should return the deleted job if it was successful
        public void TestDeleteFavoriteJob()
        {
            Favorite fav4 = new Favorite { Id = 44, Job = null, User = null };
            context.Favorites.Add(fav4);
            context.SaveChanges();

            int theIdOfTheFavorite = 44;

            var deletedFav = favoriteRepository.DeleteFavoriteJob(theIdOfTheFavorite);

            Assert.AreEqual(fav4.Id, deletedFav.Id);
        }

        [Test] //DeleteFavoriteJob() method should return null if the method gets non existed favorite ID
        public void TestIfDeleteFavoriteJobGetNonExistedFavId()
        {
            int theIdOfTheFavorite = 99;

            var deletedFav = favoriteRepository.DeleteFavoriteJob(theIdOfTheFavorite);

            Assert.AreEqual(null, deletedFav);
        }

        [Test] //GetFavId() method should return the ID of the favorite what the specific user have 
        public async void TestGetFavId()
        {
            User mockUser5 = new User { Id = 9, UserName = "mock" };
            Job mockJob5 = new Job { User = mockUser5 }; // the ID will be 3
            Favorite fav4 = new Favorite { Id = 999, Job = mockJob5, User = mockUser5 };
            context.Favorites.Add(fav4);
            context.SaveChanges();

            int userId = 9;
            int expectedResult = 999;
            int idOfThdJob = 3;

            int result = await favoriteRepository.GetFavId(idOfThdJob, userId);

            Assert.AreEqual(expectedResult, result);
        }

        [Test] //GetFavId() method should return -1 if it can't find the favorite
        public async void TestIfGetFavIdGetNonexistedUserId()
        {
            int userId = 77;
            int expectedResult = -1;
            int idOfThdJob = 5;

            int result = await favoriteRepository.GetFavId(idOfThdJob, userId);

            Assert.AreEqual(expectedResult, result);
        }
    }
}