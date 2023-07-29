using MuseumAPI.Models;

namespace MuseumAPI.Validation
{
    public class ExhibitionValidator
    {
        public ExhibitionValidator() { }

        public string Validate(ExhibitionDTO exhibition)
        {
            List<string> errors = new List<string>();

            if (!IsDateValid(exhibition.StartDate))
            {
                errors.Add("Start date of Exhibition must be between 1000 and 3000.\n");
            }

            if (!IsDateValid(exhibition.EndDate))
            {
                errors.Add("End date of Exhibition must be between 1000 and 3000.\n");
            }

            if (!IsStartDateBeforeEndDate(exhibition.StartDate, exhibition.EndDate))
            {
                errors.Add("Start date of Exhibition must be before end date.\n");
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

        private static bool IsStartDateBeforeEndDate(DateTime startDate, DateTime endDate)
        {
            return startDate < endDate;
        }
    }
}
