namespace BiletCebimde.Interfaces
{
    // T, projenizdeki herhangi bir EF Core Entity'si (Örn: Event, Category) olacaktır.
    public interface IGenericRepository<T> where T : class
    {
        // Okuma İşlemleri (Read)
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();

        // Ekleme İşlemi (Create)
        Task AddAsync(T entity);

        // Güncelleme İşlemi (Update)
        void Update(T entity);

        // Silme İşlemi (Delete)
        void Delete(T entity);

        // Değişiklikleri Veritabanına Kaydetme
        Task<int> SaveChangesAsync();
    }
}