// EfContactRepository.cs
using Lab11_Navigation.Interfaces;
using Lab11_Navigation.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab11_Navigation.Repositories
{
    public class EfContactRepository : IContactRepository
    {
        private readonly PhoneBookDbBabikov2307a1Context _context;

        public EfContactRepository(PhoneBookDbBabikov2307a1Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Contact>> GetAllAsync()
        {
            // AsNoTracking() улучшает производительность при чтении, если не планируешь менять сущности
            return await _context.Contacts
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Contact?> GetByIdAsync(int id)
        {
            return await _context.Contacts.FindAsync(id);
        }

        public async Task<Contact?> GetByPhoneAsync(string phone)
        {
            return await _context.Contacts
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Phone == phone);
        }

        public async Task AddAsync(Contact contact)
        {
            if (contact == null)
                throw new ArgumentNullException(nameof(contact));

            // Валидация на уровне репозитория (дополнительная защита)
            if (string.IsNullOrWhiteSpace(contact.Name) || string.IsNullOrWhiteSpace(contact.Phone))
                throw new ArgumentException("Name и Phone не могут быть пустыми", nameof(contact));

            await _context.Contacts.AddAsync(contact);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync(Contact contact)
        {
            if (contact == null)
                throw new ArgumentNullException(nameof(contact));

            var existing = await _context.Contacts.FindAsync(contact.Id);
            if (existing == null)
                throw new InvalidOperationException($"Контакт с Id={contact.Id} не найден");

            // Обновляем только разрешённые поля
            existing.Name = contact.Name;
            existing.Phone = contact.Phone;

            // EF Core отследит изменения автоматически
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
                await SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsByPhoneAsync(string phone, int? excludeId = null)
        {
            var query = _context.Contacts.AsNoTracking();

            if (excludeId.HasValue)
            {
                // Исключаем текущий контакт (полезно при редактировании)
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return await query.AnyAsync(c => c.Phone == phone);
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message?.Contains("unique constraint") == true)
            {
                // Можно пробросить более понятное исключение или обработать здесь
                throw new InvalidOperationException("Нарушение уникальности: контакт с таким номером уже существует", ex);
            }
        }
    }
}