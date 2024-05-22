using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrcShackApi.Core.Models;
using OrcShackApi.Web.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OrcShackApi.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DishesController : ControllerBase
    {
        private readonly IDishService _dishService;
        private readonly IUserService _userService;
        private readonly ILogger<DishesController> _logger;
        private readonly IMapper _mapper;

        public DishesController(IDishService dishService, IUserService userService, ILogger<DishesController> logger, IMapper mapper)
        {
            _dishService = dishService ?? throw new ArgumentNullException(nameof(dishService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        // Get all dishes
        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dish>>> GetAll()
        {
            _logger.LogInformation("Getting all dishes");
            var dishes = await _dishService.GetAllDishes();
            return Ok(dishes);
        }

        // Get a specific dish by ID
        [Authorize(Roles = "User,Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Dish>> Get(int id)
        {
            _logger.LogInformation($"Getting dish with ID {id}");
            var dish = await _dishService.GetDishById(id);
            return Ok(dish);
        }

        // Get dishes by name
        [Authorize(Roles = "User,Admin")]
        [HttpGet("name/{name}")]
        public async Task<ActionResult<IEnumerable<Dish>>> GetDishesByName(string name)
        {
            _logger.LogInformation("Getting dish by name: {name}", name);
            var dishes = await _dishService.GetDishesByName(name);
            if (dishes == null)
            {
                return NotFound();
            }
            return Ok(dishes);
        }

        // Create a new dish
        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateDish([FromForm] DishDto dishDto)
        {
            _logger.LogInformation("Creating a new dish");
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (dishDto.Image != null)
                {
                    var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(dishDto.Image.FileName);
                    var path = Path.Combine("Images", fileName);

                    await using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await dishDto.Image.CopyToAsync(stream);
                    }
                }

                var dish = _mapper.Map<Dish>(dishDto);
                var createdDish = await _dishService.CreateDish(dish);
                return CreatedAtAction(nameof(Get), new { id = createdDish.Id }, createdDish);
            }
            catch (Exception ex)
            {
                _logger.LogError(500, ex.Message, ex);
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        // Update dish rating
        [Authorize(Roles = "User,Admin")]
        [HttpPut("rating")]
        public async Task<IActionResult> UpdateDishRating([FromBody]UpdateDishRatingDto updateDishRatingDto)
        {
            _logger.LogInformation("Updating rating for dish with id: {Id}", updateDishRatingDto.DishId);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var emailId = User.FindFirstValue(ClaimTypes.Email);
                var user = await _userService.GetByEmail(emailId ?? string.Empty);

                if (user == null)
                {
                    _logger.LogInformation("User Not Found: {emailId}", emailId);
                    return NotFound();
                }

                var dish = await _dishService.GetDishById(updateDishRatingDto.DishId);
                if (dish == null)
                {
                    _logger.LogInformation("Dish Not Found: {id}", updateDishRatingDto.DishId);
                    return NotFound();
                }
                await _dishService.UpdateDishRating( dish.Id,user.Id, updateDishRatingDto.Rate, updateDishRatingDto.Review);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(500, ex.Message, ex);
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }


        // Update an existing dish
        [Authorize(Roles = "User,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] DishDto dishDto)
        {
            _logger.LogInformation($"Updating dish with ID {id}");
            try
            {
                var dish = await _dishService.GetDishById(id);
                if (id != dish.Id)
                    return BadRequest();

                if (dishDto.Image != null)
                {
                    var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(dishDto.Image.FileName);
                    var path = Path.Combine("Images", fileName);

                    await using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await dishDto.Image.CopyToAsync(stream);
                    }
                }

                _mapper.Map(dishDto, dish);
                await _dishService.UpdateDish(dish);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(500, ex.Message, ex);
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        // Delete a dish
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"Deleting dish with ID {id}");
            try
            {
                var dish = await _dishService.GetDishById(id);
                var imagePath = dish.Image;

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                await _dishService.DeleteDish(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(500, ex.Message, ex);
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}
