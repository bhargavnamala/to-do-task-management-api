using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoTaskManagement.Application.DTOs.Auth
{
    public class RefreshRequestDto
    {
        [Required] public string RefreshToken { get; set; } = string.Empty;
        [Required] public string AccessToken { get; set; } = string.Empty;
    }
}
