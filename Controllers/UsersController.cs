using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IMapper mapper;
        private readonly DataContext context;

        public UsersController(IMapper mapper, DataContext context)
        {
            this.mapper = mapper;
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
        {
            var users = await this.context.Users
                            .ProjectTo<MemberDTO>(this.mapper.ConfigurationProvider)
                            .ToListAsync();
            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetUserByUsername(string username)
        {
            var user = await this.context.Users.Where( x => x.UserName == username)
                            .ProjectTo<MemberDTO>(this.mapper.ConfigurationProvider)
                            .SingleOrDefaultAsync();
            return user;
        }
    }
}