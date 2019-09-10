﻿using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Domain.Paging
{
    public class PaginatedList<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecordCount { get; set; }

        public List<T> Items { get; } = new List<T>();

        public int TotalPages { get; set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalRecordCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            if(items != null) Items.AddRange(items);
        }
 
        public bool HasPreviousPage => (PageIndex > 1);
        public bool HasNextPage => (PageIndex < TotalPages);

        public int MaxPageRange => (PageSize * PageIndex) > TotalRecordCount ? TotalRecordCount : (PageSize * PageIndex);
        public int MinPageRange => (MaxPageRange + 1) - PageSize;
        
        public List<int> PageSizes { get; set; }
    }
}