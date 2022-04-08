using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UsersControllers : ControllerBase
    {

        public DataContext dataContext { get; }
        public UsersControllers(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        //We make the methods async to do the request multithread
        //
        // GET PETITONS

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            return await dataContext.Users.ToListAsync();
            
        }

        //Get user by id - 
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            //Function to get entity by ID
            var userItem = await dataContext.Users.FindAsync(id);
            if(userItem == null){
                return NotFound();
            }
            return userItem;
        }


        // POST PETITION
        [HttpPost]
        public async Task<ActionResult<AppUser>> addUser(AppUser appUser){

            //Add and save
            await dataContext.Users.AddAsync(appUser);
            await dataContext.SaveChangesAsync();

            //This returns a StatusCodes.Status201Created response
            return CreatedAtAction(nameof(GetUser), new {id = appUser.Id}, appUser);

        }

    }
}