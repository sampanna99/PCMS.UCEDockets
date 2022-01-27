namespace PCMS.UCEDockets.Controllers;

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PCMS.UCEDockets.Entities;
using PCMS.UCEDockets.Models;

[ApiController]
[Route("docket")]
public class DocketsController : ControllerBase
{
    private readonly UCEDocketsContext _context;

    public DocketsController(UCEDocketsContext context)
    {
        _context = context;
    }

    [HttpGet("id/{docketID}", Name = "GetDocket")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Xml.Ucms.Criminaldockets.CriminalDocketsDistrictCountyCourtDocket))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get([Required] string docketID)
    {
        if (!Regex.IsMatch(docketID ?? string.Empty, @"Docket-\d*"))
            return BadRequest();

        var docket = await _context.Dockets.FindAsync(docketID);

        if (docket == null)
            return NotFound();

        var uceDocket = Common.UCEDocketsSerializer.DeserializeDocket(docket.XMLDocket);

        return Ok(uceDocket);
    }

    [HttpGet("search", Name = "SearchDockets")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Get(DateTime? from, string district = null, string county = null, int skip = 0, int max = 50000)
    {
        return Ok(_context.Dockets
            .Where(d => d.Updated >= from.Value)
            .Where(d => district == null || d.District == district)
            .Where(d => county == null || d.County == county)
            .OrderBy(d => d.Updated)
            .Skip(skip)
            .Take(max)
            .Select(d => new SearchDocketsResponseModel {
                DocketID = d.DocketID,
                County = d.County,
                District = d.District,
                Filed = d.Filed,

                Created = d.Created,
                Updated = d.Updated
            })
            .ToList());
    }
}
