﻿using Microsoft.EntityFrameworkCore;
using Bogus;
using MuseumAPI.Controllers;
using MuseumAPI.Models;
using MuseumAPI.Context;

namespace MuseumAPI.Utils
{
    public static class SeedData
    {
        // Nested class to use as a type argument for ILogger
        private class SeedDataLogger { }

        private const long STACK_OVERFLOW_LOOPS = 1_000_000;

        private const int ARTISTS_COUNT = 1_000;
        private const int PAINTINGS_COUNT = 1_000;
        private const int MUSEUMS_COUNT = 1_000;
        private const int EXHIBITIONS_COUNT = 10_000;

        private const int USERS_COUNT = 100;
        private static readonly string PASSWORD = UsersController.HashPassword("a");

        private const AccessLevel ACCESS_LEVEL = AccessLevel.Regular;
        private const long PAGE_PREFERENCE = 5;

        public static async Task SeedUsersAndProfilesAsync(MuseumContext context, int n)
        {
            var existingUserIds = await context.Users.Select(u => u.Id).ToListAsync();
            var userNames = await context.Users.Select(u => u.Name).ToListAsync();

            // Generate users
            var users = new List<User>();
            var fakerUser = new Faker<User>()
                .RuleFor(u => u.Name, f => f.Internet.UserName())
                .RuleFor(u => u.Password, PASSWORD)
                .RuleFor(u => u.AccessLevel, ACCESS_LEVEL);

            // Loop n times and only add users with names that are not in the database
            for (int i = 0; i < n; i++)
            {
                var user = fakerUser.Generate();

                long current = 0;
                while (userNames.Contains(user.Name))
                {
                    if (current++ > STACK_OVERFLOW_LOOPS)
                        throw new Exception("Could not find a unique user name");

                    user = fakerUser.Generate();
                }

                users.Add(user);
                userNames.Add(user.Name);
            }

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            // Generate user profiles
            var newUserIds = await context.Users
                .Where(u => !existingUserIds.Contains(u.Id))
                .Select(u => u.Id).ToListAsync();

            var userProfiles = new List<UserProfile>();
            var fakerProfile = new Faker<UserProfile>()
                .RuleFor(up => up.Bio, f => string.Join("\n", f.Lorem.Paragraphs(3)))
                .RuleFor(up => up.Location, f => f.Address.City())
                .RuleFor(up => up.Birthday, f => f.Date.Between(DateTime.Now.AddYears(-60), DateTime.Now.AddYears(-18)))
                .RuleFor(up => up.Gender, f => f.PickRandom<Gender>())
                .RuleFor(up => up.MaritalStatus, f => f.PickRandom<MaritalStatus>())
                .RuleFor(up => up.PagePreference, PAGE_PREFERENCE);

            foreach (var userId in newUserIds)
            {
                var userProfile = fakerProfile.Generate();
                userProfile.UserId = userId;
                userProfiles.Add(userProfile);
            }

            await context.UserProfiles.AddRangeAsync(userProfiles);
            await context.SaveChangesAsync();
        }

        public static async Task SeedArtistsAsync(MuseumContext context, int n, long? userId = null)
        {
            var random = new Random();

            var userIds = await context.Users.Select(u => u.Id).ToListAsync();
            long RandomUserId() => userIds[random.Next(userIds.Count)];

            var faker = new Faker<Artist>()
                .RuleFor(e => e.FirstName, f => f.Name.FirstName())
                .RuleFor(e => e.LastName, f => f.Name.LastName())
                .RuleFor(e => e.BirthDate, f => f.Date.Between(DateTime.Now.AddYears(-50), DateTime.Now))
                .RuleFor(e => e.BirthPlace, f => f.Address.City())
                .RuleFor(e => e.Education, f => f.PickRandom(UNIVERSITIES))
                .RuleFor(e => e.Movement, f => f.PickRandom(MOVEMENTS))
                .RuleFor(er => er.UserId, userId ?? RandomUserId());

            var artists = faker.Generate(n);

            foreach (var artist in artists)
            {
                artist.UserId =  RandomUserId();
            }

            await context.Artists.AddRangeAsync(artists);
            await context.SaveChangesAsync();
        }

        public static async Task SeedPaintingsAsync(MuseumContext context, int n, long? userId = null)
        {
            var random = new Random();

            var artistIds = await context.Artists.Select(a => a.Id).ToListAsync();
            long RandomArtistId() => artistIds[random.Next(artistIds.Count)];

            var userIds = await context.Users.Select(u => u.Id).ToListAsync();
            long RandomUserId() => userIds[random.Next(userIds.Count)];

            var faker = new Faker<Painting>()
                .RuleFor(e => e.Title, f => $"{f.PickRandom(DESCRIPTIVE_WORDS)} {f.Random.Word()} {f.Random.Word()}")
                .RuleFor(e => e.CreationYear, f => f.Random.Int(1800, 2023))
                .RuleFor(e => e.Height, f => Math.Round(f.Random.Double(0.5, 5.0), 2))
                .RuleFor(e => e.Subject, f => f.PickRandom(SUBJECTS))
                .RuleFor(e => e.Medium, f => f.PickRandom(MEDIUMS))
                .RuleFor(e => e.Description, f => string.Join("\n\n", f.Lorem.Paragraphs(2)))
                .RuleFor(e => e.ArtistId, RandomArtistId())
                .RuleFor(e => e.UserId, userId ?? RandomUserId());

            var paintings = faker.Generate(n);

            foreach (var painting in paintings)
            {
                painting.UserId = RandomUserId();
            }

            await context.Paintings.AddRangeAsync(paintings);
            await context.SaveChangesAsync();
        }

        public static async Task SeedMuseumsAsync(MuseumContext context, int n, long? userId = null)
        {
            var random = new Random();

            var userIds = await context.Users.Select(u => u.Id).ToListAsync();
            long RandomUserId() => userIds[random.Next(userIds.Count)];

            var faker = new Faker<Museum>()
                .RuleFor(s => s.Name, f => $"{f.Company.CompanyName()} Museum")
                .RuleFor(s => s.Address, f => f.Address.StreetAddress())
                .RuleFor(s => s.FoundationDate, f => f.Date.Between(DateTime.Now.AddYears(-50), DateTime.Now))
                .RuleFor(s => s.Architect, f => $"{f.Name.FirstName()} {f.Name.LastName()}")
                .RuleFor(s => s.Website, f => f.Internet.Url())
                .RuleFor(s => s.UserId, userId ?? RandomUserId());

            var museums = faker.Generate(n);

            foreach (var museum in museums)
            {
                museum.UserId = RandomUserId();
            }

            await context.Museums.AddRangeAsync(museums);
            await context.SaveChangesAsync();
        }

        private class ArtistMuseumPair
        {
            public long ArtistId { get; set; }
            public long MuseumId { get; set; }
        }

        private class ArtistMuseumPairComparer : IEqualityComparer<ArtistMuseumPair>
        {
            public bool Equals(ArtistMuseumPair? x, ArtistMuseumPair? y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x is null || y is null) return false;

                return x.ArtistId == y.ArtistId && x.MuseumId == y.MuseumId;
            }

            public int GetHashCode(ArtistMuseumPair obj)
            {
                return HashCode.Combine(obj.ArtistId, obj.MuseumId);
            }
        }

        public static async Task SeedExhibitionsAsync(MuseumContext context, int n, long? userId = null)
        {
            var random = new Random();

            var userIds = await context.Users.Select(u => u.Id).ToListAsync();
            long RandomUserId() => userIds[random.Next(userIds.Count)];

            var artistIds = await context.Artists.Select(s => s.Id).ToListAsync();
            var museumIds = await context.Museums.Select(e => e.Id).ToListAsync();

            var exhibitions = new List<Exhibition>();
            var faker = new Faker<Exhibition>()
                .RuleFor(ss => ss.StartDate, f => f.Date.Between(DateTime.Now.AddYears(-10), DateTime.Now))
                .RuleFor(ss => ss.EndDate, (f, ss) => f.Date.Between(ss.StartDate, DateTime.Now))
                .RuleFor(ss => ss.UserId, userId ?? RandomUserId());

            var artistMuseumPairs = new HashSet<ArtistMuseumPair>(new ArtistMuseumPairComparer());
            artistMuseumPairs.UnionWith(await context.Exhibitions.Select(am => new ArtistMuseumPair { ArtistId = am.ArtistId, MuseumId = am.MuseumId }).ToListAsync());

            ArtistMuseumPair RandomArtistMuseumPair()
            {
                long artistId;
                long museumId;
                var pair = new ArtistMuseumPair();

                do
                {
                    artistId = artistIds[random.Next(artistIds.Count)];
                    museumId = museumIds[random.Next(museumIds.Count)];

                    pair.ArtistId = artistId;
                    pair.MuseumId = museumId;
                } while (artistMuseumPairs.Contains(pair));

                artistMuseumPairs.Add(pair);
                return pair;
            }

            for (int i = 0; i < n; i++)
            {
                var exhibition = faker.Generate();
                var pair = RandomArtistMuseumPair();

                exhibition.ArtistId = pair.ArtistId;
                exhibition.MuseumId = pair.MuseumId;

                exhibitions.Add(exhibition);
            }

            await context.Exhibitions.AddRangeAsync(exhibitions);
            await context.SaveChangesAsync();
        }

        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using (var context = new MuseumContext(serviceProvider.GetRequiredService<DbContextOptions<MuseumContext>>()))
            {
                var logger = serviceProvider.GetRequiredService<ILogger<SeedDataLogger>>();
                logger.LogInformation("Seeding process started at {time}", DateTimeOffset.UtcNow);

                if (!await context.Users.AnyAsync())
                    await SeedUsersAndProfilesAsync(context, USERS_COUNT);

                if (!await context.Artists.AnyAsync())
                    await SeedArtistsAsync(context, ARTISTS_COUNT);

                if (!await context.Paintings.AnyAsync())
                    await SeedPaintingsAsync(context, PAINTINGS_COUNT);

                if (!await context.Museums.AnyAsync())
                    await SeedMuseumsAsync(context, MUSEUMS_COUNT);

                if (!await context.Exhibitions.AnyAsync())
                    await SeedExhibitionsAsync(context, EXHIBITIONS_COUNT);

                logger.LogInformation("Seeding process finished at {time}", DateTimeOffset.UtcNow);
            }
        }

        private static readonly List<string> UNIVERSITIES = new()
        {
            "University of Cambridge",
            "University of Oxford",
            "Stanford University",
            "Massachusetts Institute of Technology",
            "University of Chicago",
            "University of California",
            "University of Pennsylvania",
            "Yale University",
            "Columbia University",
            "Princeton University"
        };

        private static readonly List<string> MOVEMENTS = new()
        {
            "Abstract Expressionism",
            "Baroque",
            "Cubism",
            "Dada",
            "Fauvism",
            "Impressionism",
            "Minimalism",
            "Pop Art",
            "Renaissance",
            "Romanticism",
            "Surrealism"
        };

        private static readonly List<string> DESCRIPTIVE_WORDS = new()
        {
            "Majestic",
            "Ephemeral",
            "Whimsical",
            "Serene",
            "Mystical",
            "Vibrant",
            "Elegant",
            "Bold",
            "Surreal",
            "Dreamy"
        };

        private static readonly List<string> SUBJECTS = new()
        {
            "Landscape",
            "Portrait",
            "Still Life",
            "Abstract",
            "Cityscape",
            "Wildlife",
            "Floral",
            "Historical",
            "Mythological",
            "Religious",
            "Marine"
        };

        private static readonly List<string> MEDIUMS = new()
        {
            "Oil", 
            "Acrylic", 
            "Watercolor", 
            "Pastel", 
            "Charcoal", 
            "Digital"
        };
    }
}
