using Lab11_Navigation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab11_Navigation.Interfaces
{
    public interface IContactRepository
    {
        Task<IEnumerable<Contact>> GetAllAsync();
        Task<Contact?> GetByIdAsync(int id);
        Task AddAsync(Contact contact);
        Task UpdateAsync(Contact contact);
        Task DeleteAsync(int id);
        Task<bool> ExistsByPhoneAsync(string phone, int? excludeId = null);
        Task SaveChangesAsync();
    }
}
