namespace PCMS.UCEDockets.Controllers;

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Xml.Ucms.Criminaldockets.CriminalDocketsDistrictCountyCourtDocket))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
