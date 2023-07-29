using MuseumAPI.Models;

namespace MuseumAPI.Validation
{
    public class MuseumValidator
    {
        public MuseumValidator() { }

        public string Validate(MuseumDTO museum)
        {
            List<string> errors = new List<string>();

            if (!IsStringNonEmpty(museum.Name))
            {
                errors.Add("Title of Museum must be non-empty.\n");
            }

            if (!IsStringNonEmpty(museum.Address))
            {
                errors.Add("Address of Museum must be non-empty.\n");
            }

            if (!IsDateValid(museum.FoundationDate))
            {
                errors.Add("Foundation Date of Museum must be between 1000 and 3000.\n");
            }

            if (!IsStringNonEmpty(museum.Architect))
            {
                errors.Add("Architect of Museum must be non-empty.\n");
            }

            if (!IsStringNonEmpty(museum.Website))
            {
                errors.Add("Website of Museum must be non-empty.\n");
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
    }
}
