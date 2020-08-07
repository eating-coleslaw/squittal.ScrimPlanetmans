using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatchReports
{
    public class PaginatedList<T>
    {
        public int PageIndex { get; set; }
        public int PageCount { get; set; }
        public List<T> Contents { get; set; }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < PageCount;

        public PaginatedList(List<T> contents, int count, int pageIndex = 1, int pageSize = 10)
        {
            //var count = contents.Count();

            PageCount = (int)Math.Ceiling(count / (double)pageSize);

            if (pageIndex < 0)
            {
                PageIndex = 1;
            }
            else if (pageIndex > PageCount)
            {
                PageIndex = PageCount;
            }
            else
            {
                PageIndex = pageIndex;
            }

            Contents = new List<T>();
            Contents.AddRange(contents);
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            
            var items = await source.Skip((pageIndex - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
