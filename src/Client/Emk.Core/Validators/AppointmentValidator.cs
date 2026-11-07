using System.Text.RegularExpressions;
using Emk.Domain.DTOs;

namespace Emk.Core.Validators
{
    /// <summary>Проверка заявки на прием по правилам контракта (pattern, maxLength).</summary>
    public static partial class AppointmentValidator
    {
        [GeneratedRegex(@"^STU-\d{4}-\d{5}$")]
        private static partial Regex PatientIdPattern();

        public static IReadOnlyList<string> Validate(AppointmentRequest request)
        {
            var errors = new List<string>();
            if (request.SlotId == Guid.Empty)
                errors.Add("Не выбрано время приема");
            if (!PatientIdPattern().IsMatch(request.PatientId ?? ""))
                errors.Add("Номер карты должен иметь формат STU-ГГГГ-NNNNN");
            if (request.Complaint is { Length: > 500 })
                errors.Add("Описание жалоб не должно превышать 500 символов");
            return errors;
        }
    }
}
