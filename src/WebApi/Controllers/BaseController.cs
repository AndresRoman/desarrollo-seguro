using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
[ApiController]
public class BaseController : ControllerBase;