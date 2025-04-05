using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlatformService2.AsynDataService;
using PlatformService2.Data;
using PlatformService2.DTOs;
using PlatformService2.Models;
using PlatformService2.SyncDataService.Http;

namespace PlatformService2.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IMessageBus _messageBus;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IPlatformRepo _context;
        private readonly IMapper _mapper;

        public PlatformsController(IPlatformRepo context,IMapper mapper, ICommandDataClient commandDataClient,IMessageBus messageBus)
        {
            _messageBus = messageBus;
            _commandDataClient = commandDataClient;
            _context = context;
            _mapper = mapper;

        }

        // GET: api/Platforms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlatformReadDTO>>> GetPlatforms()
        {
            var platforms =await _context.GetAllPlatformsAsync();
            var platformReadDTOs = _mapper.Map<IEnumerable<PlatformReadDTO>>(platforms);
            return Ok(platformReadDTOs);
        }

        // GET: api/Platforms/5
        [HttpGet("{id}",Name = "GetPlatformById")]
        public async Task<ActionResult<PlatformReadDTO>> GetPlatformById(int id)
        {
            var platform =await _context.GetPlatformByIdAsync(id);  
            if(platform == null)
            {
                return NotFound();
            }
            var platformReadDTO = _mapper.Map<PlatformReadDTO>(platform);   
            return platformReadDTO;
        }

        // PUT: api/Platforms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlatform(int id,PlatformUpdateDTO platformUpdateDTO)
        {
            var platform = await _context.GetPlatformByIdAsync(id);
            if(platform == null)
            {
                return NotFound();
            }
            platform.Name = platformUpdateDTO.Name;
            platform.Publisher = platformUpdateDTO.Publisher;
            platform.Cost = platformUpdateDTO.Cost;
            await _context.UpdatePlatform(platform);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
               return NotFound();
            }

            return NoContent();
        }

        // POST: api/Platforms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PlatformReadDTO>> PostPlatform(PlatformCreateDTO platform)
        {
            var platForm = _mapper.Map<Platform>(platform);
            await _context.CreatePlatform(platForm);
            await _context.SaveChangesAsync();

            // Send Sync Message
            try
            {
                await _commandDataClient.SendPlatformToCommand(_mapper.Map<PlatformReadDTO>(platForm));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
            }
            // Send Async Message
            try
            {
                var platformPublishedDto = _mapper.Map<PlatformPublishDTO>(platForm);
                platformPublishedDto.Event = "Platform_Published";
                _messageBus.PublishNewPlatform(platformPublishedDto);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
            }
            var result = _mapper.Map<PlatformReadDTO>(platForm);    
            return CreatedAtAction(nameof(GetPlatformById),new { id = result.Id },result);
        }

        // DELETE: api/Platforms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlatform(int id)
        {
            var platform = await _context.GetPlatformByIdAsync(id);
            if (platform == null)
            {
                return NotFound();
            }

            await _context.DeletePlatform(platform);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<bool> PlatformExists(int id)
        {
            return await _context.GetPlatformByIdAsync(id) != null;
        }
    }
}
