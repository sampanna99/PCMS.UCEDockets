namespace PCMS.UCEDockets.Controllers;

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PCMS.UCEDockets.Entities;

[ApiController]
[Route("docket")]
public class DocketsController : ControllerBase
{
    private readonly UCEDocketsContext _context;

    public DocketsController(UCEDocketsContext context)
    {
        _context = context;
    }

    [HttpGet("{docketID}", Name = "GetDocket")]
    [ProducesResponseType(typeof(Xml.Ucms.Criminaldockets.CriminalDocketsDistrictCountyCourtDocket), 200)]    
    public async Task<IActionResult> Get([Required]string docketID)
    {
        if (!Regex.IsMatch(docketID ?? string.Empty, @"Docket-\d*"))
            return BadRequest();

        var docket = await _context.Dockets.FindAsync(docketID);

        if (docket == null)
            return NotFound();

        var uceDocket = Common.UCEDocketsSerializer.DeserializeDocket(docket.XMLDocket);

        return Ok(uceDocket);
    }
}
