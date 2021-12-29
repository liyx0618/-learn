using System.Collections.Generic;

namespace EventBasedDDD
{
    public interface IPagedList<TData> : IList<TData>
    {
        int TotalCount { get; set; }
        int PageIndex { get; set; }
        int PageSize { get; set; }
        int NumberOfPages { get; }
    }

    public class PagedList<TData> : List<TData>, IPagedList<TData>
    {
        public PagedList(IEnumerable<TData> pageData, int totalCount, int pageIndex, int pageSize)
        {
            this.TotalCount = totalCount;
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.AddRange(pageData);
        }

        public int TotalCount
        {
            get;
            set;
        }

        public int PageIndex
        {
            get;
            set;
        }

        public int PageSize
        {
            get;
            set;
        }

        public int NumberOfPages
        {
            get
            {
                int pageCount = TotalCount / PageSize;
                return TotalCount % PageSize == 0 ? pageCount : pageCount + 1;
            }
        }
    }
}
