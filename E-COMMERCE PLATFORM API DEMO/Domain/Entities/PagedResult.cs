using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class PagedResult<T>
    {
        public IReadOnlyList<T> items { get; set; }
        public int page { get; set; }
        public int size { get; set; }
        public int totalCount { get; set; }
    }
}
