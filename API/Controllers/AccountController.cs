using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            //Validation to check if user exists
            if (await UserExists(registerDTO.email) == true)
            {
                return BadRequest("Username is taken.");
            }
            //encrypt the password
            using var hmac = new HMACSHA512();
            //load data to entity
            var user = new AppUser
            {
                Email = registerDTO.email.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.password)),
                PasswordSalt = hmac.Key
            };
            //load to db
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return new UserDTO{

                Email = user.Email,
                Token = _tokenService.CreateToken(user)
            };

        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {

            //Loads the user if email is found
            //NEVER FORGET THE AWAIT IF IS AN ASYNC
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == loginDTO.Email);

            //validation to check if user exists or email is valid
            if (user == null)
            {
                return Unauthorized("Invalid email.");
            }
            else
            {

                //Decoding hashes
                using var hmac = new HMACSHA512(user.PasswordSalt);
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
                //Check if hash is equal to the userHash
                for (int x = 0; x < computedHash.Length; x++)
                {
                    if (computedHash[x] != user.PasswordHash[x])
                    {
                        return Unauthorized("Invalid password.");
                    }
                }

            }
            //If everything ok return user
            return new UserDTO{

                Email = user.Email,
                Token = _tokenService.CreateToken(user)
            };
           

        }

        //PETITION TO DELETE BY ID
        [HttpDelete("DeleteAccount")]
        public async Task<IActionResult> DeleteAccount(int id)
        {

            //find first the user
            var userItem = await _context.Users.FindAsync(id);
            if (userItem == null)
            {
                return NotFound();
            }

            //Remove from DB and save the changes
            _context.Users.Remove(userItem);
            await _context.SaveChangesAsync();

            //Just return a not content after delete
            return NoContent();

        }

        //Search if UserExist by its email
        private async Task<bool> UserExists(string email)
        {

            return await _context.Users.AnyAsync(x => x.Email == email.ToLower());
        }
    }
}