using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class AppUser
    {

        public int Id { get; set; }
        
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Adress { get; set; }
        public string PhoneNumber { get; set; }
        public string IdUser { get; set; }
        public string LevelOfStudies { get; set; }
        public string Profession { get; set; }
        public string ProfessionDescription { get; set; }
        public string Username { get; set; }
        


    }
}