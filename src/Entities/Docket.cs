namespace PCMS.UCEDockets.Entities;

using System;
using System.ComponentModel.DataAnnotations;

public class Docket
{
    [MaxLength(32)]
    public string DocketID { get; set; }

    public string XMLDocket { get; set; }

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }

    [MaxLength(32)]
    public string District { get; set; }

    [MaxLength(32)]
    public string County { get; set; }

    public DateTime? Filed { get; set; }
}