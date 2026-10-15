using Microsoft.AspNetCore.Mvc;
using MonApi.Models;
using Microsoft.EntityFrameworkCore;
using MonApi.Data;
using Microsoft.AspNetCore.Authorization;
namespace MonApi.Controllers

{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TasksController(AppDbContext context)
        {
            _context = context;
        }
        // GET /tasks
        [HttpGet]
        public async Task<ActionResult<List<TaskItem>>> GetAll()
        {
            return Ok(await _context.Tasks.ToListAsync());
        }

        // GET /tasks/1
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetById(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        // POST /tasks
        [HttpPost]
        public async Task<ActionResult<TaskItem>> Create(TaskItem task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }

        // PUT /tasks/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TaskItem task)
        {
            if (id != task.Id) return BadRequest();
            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /tasks/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }


    }
}