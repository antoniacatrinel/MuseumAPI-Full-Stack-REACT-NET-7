using MuseumAPI.Models;

namespace MuseumAPI.Validation
{
    public class PaintingValidator
    {
        public PaintingValidator() { }  

        public string Validate(PaintingDTO painting)
        {
            List<string> errors = new List<string>();

            if (!IsStringNonEmpty(painting.Title))
            {
                errors.Add("Title of Painting must be non-empty.\n");
            }

            if (!IsYearValid(painting.CreationYear))
            {
                errors.Add("Creation year of Painting must be between 1000 and 3000.\n");
            }

            if (!IsNumberPositive(painting.Height))
            {
                errors.Add("Height of Painting must be positive.\n");
            }

            if (!IsStringNonEmpty(painting.Subject))
            {
                errors.Add("Subject of Painting must be non-empty.\n");
            }

            if (!IsStringNonEmpty(painting.Medium))
            {
                errors.Add("Medium of Painting must be non-empty.\n");
            }

            return string.Join("\n", errors);
        }

        private static bool IsDateValid(DateTime? date)
        {
            if (date == null)
            {
                return false;
            }

            return date.Value.Year >= 1000 && date.Value.Year <= 3000;
        }

        private static bool IsStringNonEmpty(string? value)
        {
            if (value == null)
            {
                return false;
            }

            return value.Length != 0;
        }

        private static bool IsNumberPositive(double? height)
        {
            if (height == null)
            {
                return false;
            }

            return height >= 0;
        }

        private static bool IsYearValid(int? year)
        {
            if (year == null)
            {
                return false;
            }

            return year >= 1000 && year <= 3000;
        }
    }
}
