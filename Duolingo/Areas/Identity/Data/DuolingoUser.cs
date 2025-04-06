using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Duolingo.Models;
using Microsoft.AspNetCore.Identity;

namespace Duolingo.Areas.Identity.Data;


public class DuolingoUser : IdentityUser
{
    public ICollection<Course> Courses { get; set; }


    public int DoneTasks { get; set; } = 0;
}

