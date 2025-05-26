using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/[controller]")]
public class SampleController : ControllerBase
{
    //[HttpGet]
    // public string GetGreet()
    // {
    //     return "Hello World";
    // }

    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public ActionResult GetGreet()
    {
        return Ok("Hello World");
    }

}