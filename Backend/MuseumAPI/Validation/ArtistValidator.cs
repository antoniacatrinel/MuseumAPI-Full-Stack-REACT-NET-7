using MuseumAPI.Models;

namespace MuseumAPI.Validation
{
    public class ArtistValidator
    {
        public ArtistValidator() { }

        public string Validate(ArtistDTO artist)
        {
            List<string> errors = new List<string>();

            if (!IsStringNonEmpty(artist.FirstName))
            {
                errors.Add("First name of Artist must be non-empty.\n");
            }

            if (!IsStringNonEmpty(artist.LastName))
            {
                errors.Add("Last name of Artist must be non-empty.\n");
            }

            if (!IsDateValid(artist.BirthDate))
            {
                errors.Add("Birth date of Artist must be between 1000 and 3000.\n");
            }

            if (!IsStringNonEmpty(artist.BirthPlace))
            {
                errors.Add("Birth place of Artist must be non-empty.\n");
            }

            if (!IsStringNonEmpty(artist.Education))
            {
                errors.Add("Education of Artist must be non-empty.\n");
            }

            if (!IsStringNonEmpty(artist.Movement))
            {
                errors.Add("Movement of Artist must be non-empty.\n");
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
