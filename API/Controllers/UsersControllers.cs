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
        //

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
            return  await dataContext.Users.FindAsync(id);
        }



    }
}