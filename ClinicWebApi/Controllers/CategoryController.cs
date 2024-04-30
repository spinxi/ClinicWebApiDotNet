using ClinicWebApi.Data;
using ClinicWebApi.DTO;
using ClinicWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace ClinicWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //[Authorize(Roles = "Admin")]
        [Authorize]
        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory(CategoryDTO category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newCategory = new Category
                {
                    Name = category.Name
                };

                _dbContext.Categories.Add(newCategory);
                await _dbContext.SaveChangesAsync();
                return Ok(new { message = "Category added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the category.", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("UpdateCategory/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest(new { message = "Category ID mismatch." });
            }

            var existingCategory = await _dbContext.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                return NotFound(new { message = "Category not found." });
            }

            try
            {
                existingCategory.Name = category.Name;
                _dbContext.Categories.Update(existingCategory);
                await _dbContext.SaveChangesAsync();
                return Ok(new { message = "Category updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the category.", error = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { message = "Category not found." });
            }

            // Check if the category is associated with any doctor
            var associatedDoctor = _dbContext.Doctors.FirstOrDefault(d => d.CategoryId == id);
            if (associatedDoctor != null)
            {
                return BadRequest(new { message = "Cannot delete category as it is associated with one or more doctors." });
            }

            try
            {
                _dbContext.Categories.Remove(category);
                await _dbContext.SaveChangesAsync();
                return Ok(new { message = "Category deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the category.", error = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetCategoriesWithDoctorCounts()
        {
            try
            {
                var categoriesWithCounts = await _dbContext.Categories
                    .Select(c => new
                    {
                        c.CategoryId,
                        c.Name,
                        DoctorCount = _dbContext.Doctors.Count(d => d.CategoryId == c.CategoryId)
                    })
                    .ToListAsync();

                return Ok(categoriesWithCounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving categories with doctor counts.", error = ex.Message });
            }
        }
    }
}
