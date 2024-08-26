namespace FirstScent
{
    public class Pagination<T>
    {
        public List<T> Items { get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }

        public static Pagination<T> Paginate(List<T> source, int pageIndex, int pageSize)
        {
            var totalItems = source.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new  Pagination<T>
            {
                Items = items,
                PageIndex = pageIndex,
                TotalPages = totalPages
            };
        }
    }
}
