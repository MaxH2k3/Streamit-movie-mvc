﻿using Movies.Business.globals;
using Movies.Business.seasons;
using Streamit_movie_mvc.Models.Domain;

namespace Movies.Interface
{
    public interface ISeasonService
    {
        IEnumerable<Season> GetSeasons();
        Season? GetSeason(Guid seasonId);
        IEnumerable<Season> GetSeasonsByMovie(Guid movieId);
        IEnumerable<Season> GetSeasonsByMovieAndNumber(Guid movieId, int? seasonNumber);
        Task<ResponseDTO> CreateSeason(NewSeason newSeason);
        Task<ResponseDTO> DeleteSeason(Guid seasonId);
        Task<ResponseDTO> UpdateSeason(string? name, Guid seasonId);
        Task<ResponseDTO> DeleteSeasonByMovie(Guid id);
    }
}
