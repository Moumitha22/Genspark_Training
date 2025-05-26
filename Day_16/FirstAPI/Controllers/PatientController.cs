using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/[controller]")]
public class PatientController : ControllerBase
{
    static List<Patient> patients = new List<Patient>
    {
        new Patient {Id = 1001, Name = "Kalki", Age = 28, BloodGroup  = "A+"},
        new Patient{Id = 1002, Name = "Dhiya", Age = 24, BloodGroup = "B+"}
    };

    [HttpGet]
    public ActionResult<IEnumerable<Patient>> GetPatients()
    {
        return Ok(patients);
    }

    [HttpGet("{id}")]
    public ActionResult<Patient> GetPatient(int id)
    {
        var patient = patients.FirstOrDefault(p => p.Id == id);
        if (patient == null)
        {
            return NotFound($"Patient with id {id} not found");
        }
        return patient;
    }

    [HttpPost]
    public ActionResult<Patient> PostPatient([FromBody] Patient patient)
    {
        patients.Add(patient);
        return Created("", patient);

    }

    [HttpPut("{id}")]
    public ActionResult<Patient> UpdatePatient(int id, [FromBody] Patient patient)
    {
        var existingPatient = patients.FirstOrDefault(p => p.Id == id);
        if (existingPatient == null)
        {
            return NotFound($"Patient with id {id} not found");
        }
        existingPatient.Name = patient.Name;
        existingPatient.Age = patient.Age;
        existingPatient.BloodGroup = patient.BloodGroup;
        return Ok(patient);
    }

    [HttpDelete("{id}")]
    public ActionResult DeletePatient(int id)
    {
        var patient = patients.FirstOrDefault(p => p.Id == id);
        if (patient == null)
        {
            return NotFound($"Patient with id {id} not found");
        }
        patients.Remove(patient);
        return Ok(new { message = "Patient deleted successfully",
            id = id });

    }
}