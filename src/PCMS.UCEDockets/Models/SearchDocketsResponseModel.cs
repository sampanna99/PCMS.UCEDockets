using System;
using System.ComponentModel.DataAnnotations;

namespace PCMS.UCEDockets.Models;

public class SearchDocketsResponseModel
{
    [MaxLength(32)]
    public string DocketID { get; set; }

    [MaxLength(32)]
    public string District { get; set; }
    [MaxLength(32)]
    public string County { get; set; }
    public DateTime? Filed { get; set; }

    public DateTime Updated { get; set; }
    public DateTime Created { get; set; }
}