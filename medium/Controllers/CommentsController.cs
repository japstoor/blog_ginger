﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using medium.Model;
using Microsoft.AspNetCore.Identity;

namespace medium.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private UserManager<Login> _userManager;
        private readonly UserContext _context;

        public CommentsController(UserContext context, UserManager<Login> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComment()
        {

            return await _context.Comment.ToListAsync();
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComment(int? id)
        {
            var comment = await _context.Comment.Where(p => p.BlogId.Equals(id))
                .Select(x => new Comment()
                {
                    Id = x.Id,
                    UserComment = x.UserComment,
                    Created = x.Created,
                    UserId = x.UserId,
                    BlogId = x.BlogId,
                })
                .ToListAsync();

            if (comment == null)
            {
                return NotFound();
            }


                return comment;
 
        }

        // PUT: api/Comments/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Comment>> PutComment(int id, Comment comment)
        {
            if (id != comment.Id)
            {
                return BadRequest("Id Mismatch");
            }

            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Comments
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(Comment comment)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            comment.UserId = userId;
            _context.Comment.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComment", new { id = comment.Id }, comment);
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Comment>> DeleteComment(int? id)
        {
            var comment = await _context.Comment.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();

            return comment;
        }

        private bool CommentExists(int? id)
        {
            return _context.Comment.Any(e => e.Id == id);
        }

    }
}
