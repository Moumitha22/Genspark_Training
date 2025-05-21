using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AppointmentManagementApplication.Models
{
    public class AppointmentSearchModel
    {
        public string? PatientName { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public Range<int>? AgeRange { get; set; }

        public override string ToString()
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(PatientName))
                parts.Add($"Patient Name: {PatientName}");

            if (AppointmentDate != null )
                parts.Add($"Appointment Date : {AppointmentDate}");
            
            if (AgeRange != null && AgeRange.MinVal != 0 && AgeRange.MaxVal != 0)
                parts.Add($"Age Range: {AgeRange.MinVal} - {AgeRange.MaxVal}");

            return parts.Count > 0 ? string.Join(", ", parts) : "No search criteria provided";
        }
    }

    public class Range<T>
    {
        public T? MinVal { get; set; }
        public T? MaxVal { get; set; }
    }
}
