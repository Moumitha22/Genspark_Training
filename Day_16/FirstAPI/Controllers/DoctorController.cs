using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/[controller]")]
public class DoctorController : ControllerBase
{
    static List<Doctor> doctors = new List<Doctor>
    {
        new Doctor{ Id = 101,  Name = "Rajesh"},
        new Doctor{ Id = 102,  Name = "Kesh"},
    };

    [HttpGet]
    public ActionResult<IEnumerable<Doctor>> GetDoctors()
    {
        return Ok(doctors);
    }

    [HttpPost]
    public ActionResult<Doctor> PostDoctor([FromBody] Doctor doctor)
    {
        doctors.Add(doctor);
        return Created("", doctor);
    }

    [HttpPut("{id}")]
    public ActionResult<Doctor> UpdatePostDoctor(int id, [FromBody] Doctor doctor)
    {
        var existingDoctor = doctors.FirstOrDefault(d => d.Id == id);
        if (existingDoctor == null)
        {
            return NotFound($"Doctor with id {id} not found");
        }
        existingDoctor.Name = doctor.Name;
        return Ok(doctor);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteDoctor(int id)
    {
        var doctor = doctors.FirstOrDefault(d => d.Id == id);
        if (doctor == null)
        {
            return NotFound($"Doctor with id {id} not found");
        }
        doctors.Remove(doctor);
        return Ok(new { message = "Doctor deleted successfully", id = id });
    }


}