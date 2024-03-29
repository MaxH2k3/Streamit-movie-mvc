﻿using MongoDB.Driver;
using Movies.Business.anothers;
using Movies.Business.movies;
using Movies.Business.users;
using System.Net;

namespace Streamit_movie_mvc.Models.Domain;

public class MovieMongoContext
{
    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;
    private static bool _isIndexCreated = false;

    public IMongoCollection<UserTemporary> Users { get; set; }
    public IMongoCollection<VerifyToken> Tokens { get; set; }
    public IMongoCollection<AnalystMovie> CurrentTopMovies { get; set; }
    public IMongoCollection<AnalystMovie> PreviousTopMovies { get; set; }
    public IMongoCollection<BlackIP> BlackListIP { get; set; }

    public MovieMongoContext()
    {
        // Create a MongoClient with the connection string
        _client = new MongoClient(GetConnectionString());

        // Access a specific database
        _database = _client.GetDatabase("Movie");

        // Access a specific collection
        Users = _database.GetCollection<UserTemporary>("User");
        Tokens = _database.GetCollection<VerifyToken>("Token");
        CurrentTopMovies = _database.GetCollection<AnalystMovie>("CurrentTopMovie");
        PreviousTopMovies = _database.GetCollection<AnalystMovie>("PreviousTopMovie");
        BlackListIP = _database.GetCollection<BlackIP>("BlackListIP");

        //Create Index for collection
        if (!_isIndexCreated)
        {
            CreateIndex();
        }
    }

    private string GetConnectionString()
    {
        IWebHostEnvironment? environment = new HttpContextAccessor().HttpContext?.RequestServices
                                    .GetRequiredService<IWebHostEnvironment>();

        IConfiguration config = new ConfigurationBuilder()

                .SetBasePath(Directory.GetCurrentDirectory())

                .AddJsonFile("appsettings.json", true, true)

                .Build();

        var strConn = "";
        if (environment?.IsProduction() ?? true)
        {
            strConn = config["ConnectionStrings:MongoDB"];
        }
        else
        {
            strConn = config["LocalDB:MongoDB"];
        }

        return strConn;

    }

    private void CreateIndex()
    {
        try
        {
            //token
            var tokensIndexKeyDefinition = Builders<VerifyToken>.IndexKeys.Ascending(token => token.ExpiredDate);
            var tokensIndexOptions = new CreateIndexOptions { ExpireAfter = TimeSpan.Zero };

            Tokens.Indexes.CreateOne(new CreateIndexModel<VerifyToken>(tokensIndexKeyDefinition, tokensIndexOptions));

            //user
            var usersIndexKeyDefinition = Builders<UserTemporary>.IndexKeys.Ascending(user => user.ExpiredDate);
            var usersIndexOptions = new CreateIndexOptions { ExpireAfter = TimeSpan.Zero };

            Users.Indexes.CreateOne(new CreateIndexModel<UserTemporary>(usersIndexKeyDefinition, usersIndexOptions));

            //set complete config index
            _isIndexCreated = true;

            Console.WriteLine("Create Index successfully!");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

    }
}
