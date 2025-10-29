﻿/**
 * File: ModulesController.cs
 * Author: HieuNDHE173169
 * Created: 16/10/2025
 * Last Updated: 20/10/2025
 * Version: 1.0.0
 * Purpose: Manage application modules (CRUD). Also cascades delete to related
 *          role-permissions to maintain integrity.
 * Endpoints:
 *   - GET    /api/modules              : List modules
 *   - GET    /api/modules/{id}         : Get a module by id
 *   - POST   /api/modules              : Create a module
 *   - PUT    /api/modules/{id}         : Update a module
 *   - DELETE /api/modules/{id}         : Delete a module and its role-permissions
 */
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Keytietkiem.Models;
using Keytietkiem.DTOs;
using Microsoft.EntityFrameworkCore;
namespace Keytietkiem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly KeytietkiemDbContext _context;
        public ModulesController(KeytietkiemDbContext context)
        {
            _context = context;
        }
        // GET: api/<ModulesController>
        [HttpGet]
        /**
        * Summary: Retrieve all modules.
        * Route: GET /api/modules
        * Params: none
        * Returns: 200 OK with list of modules
        */
        public async Task<IActionResult> GetModules()
        {
            var modules = await _context.Modules
                .Select(m => new ModuleDTO
                {
                    ModuleId = m.ModuleId,
                    ModuleName = m.ModuleName,
                    Description = m.Description,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                })
                .ToListAsync();
            return Ok(modules);
        }
        // GET api/<ModulesController>/5
        /**
         * Summary: Retrieve a module by id.
         * @Route: GET /api/modules/{id}
         * @Params: id (long) - module identifier
         * @Returns: 200 OK with module, 404 if not found
         */
        [HttpGet("{id}")]
        public async Task<IActionResult> GetModuleById(long id)
        {
            var module = await _context.Modules
                .FirstOrDefaultAsync(m => m.ModuleId == id);
            if (module == null)
            {
                return NotFound();
            }

            var moduleDto = new ModuleDTO
            {
                ModuleId = module.ModuleId,
                ModuleName = module.ModuleName,
                Description = module.Description,
                CreatedAt = module.CreatedAt,
                UpdatedAt = module.UpdatedAt
            };

            return Ok(moduleDto);
        }
        // POST api/<ModulesController>
        [HttpPost]
        /**
         * Summary: Create a new module.
         * Route: POST /api/modules
         * Body: Module newModule
         * Returns: 201 Created with created module, 400/409 on validation errors
         */
        public async Task<IActionResult> CreateModule([FromBody] CreateModuleDTO createModuleDto)
        {
            if (createModuleDto == null || string.IsNullOrWhiteSpace(createModuleDto.ModuleName))
            {
                return BadRequest("Module name is required.");
            }
            var existing = await _context.Modules
                .FirstOrDefaultAsync(m => m.ModuleName == createModuleDto.ModuleName);
            if (existing != null)
            {
                return Conflict(new { message = "Module name already exists." });
            }

            var newModule = new Module
            {
                ModuleName = createModuleDto.ModuleName,
                Description = createModuleDto.Description,
                CreatedAt = DateTime.UtcNow
            };

            _context.Modules.Add(newModule);
            await _context.SaveChangesAsync();

            // Add RolePermissions for all existing roles and permissions with this new module
            var roles = await _context.Roles.ToListAsync();
            var permissions = await _context.Permissions.ToListAsync();

            var rolePermissions = new List<RolePermission>();
            foreach (var role in roles)
            {
                foreach (var permission in permissions)
                {
                    rolePermissions.Add(new RolePermission
                    {
                        RoleId = role.RoleId,
                        ModuleId = newModule.ModuleId,
                        PermissionId = permission.PermissionId,
                        IsActive = true
                    });
                }
            }

            _context.RolePermissions.AddRange(rolePermissions);
            await _context.SaveChangesAsync();

            var moduleDto = new ModuleDTO
            {
                ModuleId = newModule.ModuleId,
                ModuleName = newModule.ModuleName,
                Description = newModule.Description,
                CreatedAt = newModule.CreatedAt,
                UpdatedAt = newModule.UpdatedAt
            };

            return CreatedAtAction(nameof(GetModuleById), new { id = newModule.ModuleId }, moduleDto);
        }
        // PUT api/<ModulesController>/5
        [HttpPut("{id}")]
        /**
         * Summary: Update an existing module by id.
         * Route: PUT /api/modules/{id}
         * Params: id (long)
         * Body: Module updatedModule
         * Returns: 204 No Content, 400/404 on errors
         */
        public async Task<IActionResult> UpdateModule(long id, [FromBody] UpdateModuleDTO updateModuleDto)
        {
            if (updateModuleDto == null)
            {
                return BadRequest("Invalid module data.");
            }
            var existing = await _context.Modules
                .FirstOrDefaultAsync(m => m.ModuleId == id);
            if (existing == null)
            {
                return NotFound();
            }
            existing.ModuleName = updateModuleDto.ModuleName;
            existing.Description = updateModuleDto.Description;
            existing.UpdatedAt = DateTime.UtcNow;
            _context.Modules.Update(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        // DELETE api/<ModulesController>/5
        [HttpDelete("{id}")]
        /**
         * Summary: Delete a module by id and cascade remove related role-permissions.
         * Route: DELETE /api/modules/{id}
         * Params: id (long)
         * Returns: 204 No Content, 404 if not found
         */
        public async Task<IActionResult> DeleteModule(long id)
        {
            var existingModule = await _context.Modules
                .FirstOrDefaultAsync(m => m.ModuleId == id);
            if (existingModule == null)
            {
                return NotFound();
            }
            _context.RolePermissions.RemoveRange(existingModule.RolePermissions);
            _context.Modules.Remove(existingModule);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
