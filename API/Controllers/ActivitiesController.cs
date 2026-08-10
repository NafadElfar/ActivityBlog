using System;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers;

public class ActivitiesController(AppDbContext context) : BaseApiController
{
  [HttpGet]
  public async Task<ActionResult<List<Activity>>> GetActivities()
  {
    return await context.Acticities.ToListAsync();
  }
  [HttpGet("{id}")]
  public async Task<ActionResult<Activity>> GetActivityById(string id)
  {
    var Activity =  await context.Acticities.FindAsync(id);
    if(Activity ==null) return NotFound();
    return Activity;
  }
}
