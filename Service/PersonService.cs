using Model;
using Model.DomainModels;
using Service.DTOs;
using Microsoft.EntityFrameworkCore;


namespace Service
{
    public class PersonService : IPersonService
    {
        private readonly FinalProjectDbContext _context;

        public PersonService(FinalProjectDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ServiceResult> AddPersonAsync(CreatePersonDto personDto)
        {
            if (personDto == null)
                return ServiceResult.Fail("اطلاعات شخص برای افزودن ارسال نشده است.");
            if (string.IsNullOrWhiteSpace(personDto.FirstName))
                return ServiceResult.Fail("نام شخص الزامی است.");
            if (string.IsNullOrWhiteSpace(personDto.LastName))
                return ServiceResult.Fail("نام خانوادگی شخص الزامی است.");

            try
            {
                var personEntity = new Person
                {
                    FirstName = personDto.FirstName.Trim(),
                    LastName = personDto.LastName.Trim()
                };
                _context.Person.Add(personEntity);
                await _context.SaveChangesAsync();
                return ServiceResult.Success("شخص با موفقیت اضافه شد.");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException in AddPersonAsync (Service): {ex.InnerException?.Message ?? ex.Message}");
                return ServiceResult.Fail($"خطا در ذخیره سازی اطلاعات شخص در دیتابیس: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddPersonAsync (Service): {ex.Message}");
                return ServiceResult.Fail($"خطای پیش بینی نشده در هنگام افزودن شخص: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<PersonDto>>> GetAllPersonsAsync()
        {
            try
            {
                var personEntities = await _context.Person
                                                   .AsNoTracking()
                                                   .ToListAsync();
                var personDtos = personEntities.Select(p => new PersonDto
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    FullName = p.FullName
                }).ToList();
                return ServiceResult<List<PersonDto>>.Success(personDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllPersonsAsync (Service): {ex.Message}");
                return ServiceResult<List<PersonDto>>.Fail($"خطا در بازیابی لیست اشخاص: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PersonDto>> GetPersonByIdAsync(int id)
        {
            if (id <= 0)
                return ServiceResult<PersonDto>.Fail("شناسه شخص نامعتبر است.");

            try
            {
                var personEntity = await _context.Person.FindAsync(id);

                if (personEntity == null)
                    return ServiceResult<PersonDto>.Fail("شخص با شناسه مورد نظر یافت نشد.");

                var personDto = new PersonDto
                {
                    Id = personEntity.Id,
                    FirstName = personEntity.FirstName,
                    LastName = personEntity.LastName,
                    FullName = personEntity.FullName
                };
                return ServiceResult<PersonDto>.Success(personDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPersonByIdAsync (Service): {ex.Message}");
                return ServiceResult<PersonDto>.Fail($"خطا در بازیابی اطلاعات شخص: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UpdatePersonAsync(int id, UpdatePersonDto personDto)
        {
            if (id <= 0)
                return ServiceResult.Fail("شناسه شخص برای ویرایش نامعتبر است.");
            if (personDto == null)
                return ServiceResult.Fail("اطلاعات شخص برای ویرایش ارسال نشده است.");
            if (string.IsNullOrWhiteSpace(personDto.FirstName))
                return ServiceResult.Fail("نام شخص الزامی است.");
            if (string.IsNullOrWhiteSpace(personDto.LastName))
                return ServiceResult.Fail("نام خانوادگی شخص الزامی است.");

            try
            {
                var existingPerson = await _context.Person.FindAsync(id);
                if (existingPerson == null)
                    return ServiceResult.Fail("شخص با شناسه مورد نظر برای ویرایش یافت نشد.");

                existingPerson.FirstName = personDto.FirstName.Trim();
                existingPerson.LastName = personDto.LastName.Trim();

                await _context.SaveChangesAsync();
                return ServiceResult.Success("اطلاعات شخص با موفقیت ویرایش شد.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine($"Concurrency Error in UpdatePersonAsync (Service): {ex.Message}");
                return ServiceResult.Fail("اطلاعات شخص توسط کاربر دیگری تغییر کرده است. لطفا صفحه را رفرش کنید و دوباره تلاش کنید.");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException in UpdatePersonAsync (Service): {ex.InnerException?.Message ?? ex.Message}");
                return ServiceResult.Fail($"خطا در ذخیره سازی ویرایش شخص: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdatePersonAsync (Service): {ex.Message}");
                return ServiceResult.Fail($"خطای پیش بینی نشده در هنگام ویرایش شخص: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeletePersonAsync(int id)
        {
            if (id <= 0)
                return ServiceResult.Fail("شناسه شخص برای حذف نامعتبر است.");

            try
            {
                var person = await _context.Person.FindAsync(id);
                if (person == null)
                    return ServiceResult.Fail("شخص با شناسه مورد نظر برای حذف یافت نشد.");

                _context.Person.Remove(person);
                await _context.SaveChangesAsync();
                return ServiceResult.Success("شخص با موفقیت حذف شد.");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database Error in DeletePersonAsync (Service): {ex.InnerException?.Message ?? ex.Message}");
                return ServiceResult.Fail("خطا در حذف شخص. ممکن است این شخص به سایر اطلاعات سیستم وابسته باشد.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeletePersonAsync (Service): {ex.Message}");
                return ServiceResult.Fail($"خطای پیش بینی نشده در هنگام حذف شخص: {ex.Message}");
            }
        }
    }
}