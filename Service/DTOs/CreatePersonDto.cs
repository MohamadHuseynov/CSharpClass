using System.ComponentModel.DataAnnotations;

namespace Service.DTOs
{
    public class CreatePersonDto
    {
        [Required(ErrorMessage = "نام شخص الزامی است.")]
        [StringLength(50, ErrorMessage = "طول نام شخص نمی‌تواند بیشتر از 50 کاراکتر باشد.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "نام خانوادگی شخص الزامی است.")]
        [StringLength(50, ErrorMessage = "طول نام خانوادگی شخص نمی‌تواند بیشتر از 50 کاراکتر باشد.")]
        public string LastName { get; set; }
    }
}