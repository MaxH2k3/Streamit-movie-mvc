﻿using System;
using System.Collections.Generic;

namespace Streamit_movie_mvc.Models.Domain
{
    public partial class MovieCategory
    {
        public int? CategoryId { get; set; }
        public Guid? MovieId { get; set; }

        public virtual Category? Category { get; set; }
        public virtual Movie? Movie { get; set; }

        public MovieCategory()
        {
        }

        public MovieCategory(int? CategoryId, Guid? MovieId)
        {
            this.CategoryId = CategoryId;
            this.MovieId = MovieId;
        }
    }
}
