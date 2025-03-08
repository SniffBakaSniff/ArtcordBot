using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArtcordBot.Services.Database
{
    public class PaginationService : IPaginationService
    {
        public async Task<PaginatedResult<T>> GetPaginatedResults<T>(IQueryable<T> query, int pageNumber = 1, int pageSize = 5)
        {
            return await ExceptionHandler.HandleAsync(async () =>
            {
                var totalRecords = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

                var records = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PaginatedResult<T>
                {
                    Records = records,
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    CurrentPage = pageNumber,
                    PageSize = pageSize
                };
            });
        }

        public class PaginatedResult<T>
        {
            public required List<T> Records { get; set; }
            public int TotalRecords { get; set; }
            public int TotalPages { get; set; }
            public int CurrentPage { get; set; }
            public int PageSize { get; set; }
        }
    }
}