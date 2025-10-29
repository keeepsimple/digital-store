﻿using Keytietkiem.DTOs;
using Keytietkiem.Infrastructure;
using Keytietkiem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Keytietkiem.Controllers;

[ApiController]
[Route("api/badges")]
public class BadgesController : ControllerBase
{
    private readonly IDbContextFactory<KeytietkiemDbContext> _dbFactory;

    public BadgesController(IDbContextFactory<KeytietkiemDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BadgeListItemDto>>> List(
    [FromQuery] string? keyword,
    [FromQuery] bool? active,
    [FromQuery] string? sort = "displayName",
    [FromQuery] string? direction = "asc")
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var q = db.Badges.AsNoTracking().AsQueryable();

        // ==== FILTER (gộp 1 ô tìm kiếm) ====
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.Trim().ToLowerInvariant();
            q = q.Where(b =>
                b.BadgeCode.ToLower().Contains(kw) ||
                b.DisplayName.ToLower().Contains(kw) ||
                (b.ColorHex != null && b.ColorHex.ToLower().Contains(kw)) ||
                (b.Icon != null && b.Icon.ToLower().Contains(kw))
            );
        }

        if (active is not null)
            q = q.Where(b => b.IsActive == active);

        // ==== SORT ====
        sort = sort?.Trim().ToLowerInvariant();
        direction = direction?.Trim().ToLowerInvariant();

        q = (sort, direction) switch
        {
            ("code", "asc") => q.OrderBy(b => b.BadgeCode),
            ("code", "desc") => q.OrderByDescending(b => b.BadgeCode),
            ("name", "asc") => q.OrderBy(b => b.DisplayName),
            ("name", "desc") => q.OrderByDescending(b => b.DisplayName),
            ("color", "asc") => q.OrderBy(b => b.ColorHex),
            ("color", "desc") => q.OrderByDescending(b => b.ColorHex),
            ("active", "asc") => q.OrderBy(b => b.IsActive),
            ("active", "desc") => q.OrderByDescending(b => b.IsActive),
            ("icon", "asc") => q.OrderBy(b => b.Icon),
            ("icon", "desc") => q.OrderByDescending(b => b.Icon),
            _ => q.OrderBy(b => b.DisplayName)
        };

        // ==== RESULT ====
        var items = await q
            .Select(b => new BadgeListItemDto(
                b.BadgeCode,
                b.DisplayName,
                b.ColorHex,
                b.Icon,
                b.IsActive
            ))
            .ToListAsync();

        return Ok(items);
    }




    [HttpGet("{code}")]
    public async Task<ActionResult<BadgeListItemDto>> Get(string code)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var b = await db.Badges.AsNoTracking().FirstOrDefaultAsync(x => x.BadgeCode == code);
        if (b is null) return NotFound();
        return Ok(new BadgeListItemDto(b.BadgeCode, b.DisplayName, b.ColorHex, b.Icon, b.IsActive));
    }

    [HttpPost]
    public async Task<IActionResult> Create(BadgeCreateDto dto)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        if (await db.Badges.AnyAsync(x => x.BadgeCode == dto.BadgeCode))
            return Conflict(new { message = "BadgeCode already exists" });

        db.Badges.Add(new Badge
        {
            BadgeCode = dto.BadgeCode.Trim(),
            DisplayName = dto.DisplayName.Trim(),
            ColorHex = dto.ColorHex,
            Icon = dto.Icon,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { code = dto.BadgeCode }, null);
    }

    [HttpPut("{code}")]
    public async Task<IActionResult> Update(string code, BadgeUpdateDto dto)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var e = await db.Badges.FirstOrDefaultAsync(x => x.BadgeCode == code);
        if (e is null) return NotFound();
        e.DisplayName = dto.DisplayName.Trim();
        e.ColorHex = dto.ColorHex;
        e.Icon = dto.Icon;
        e.IsActive = dto.IsActive;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{code}")]
    public async Task<IActionResult> Delete(string code)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var e = await db.Badges.FirstOrDefaultAsync(x => x.BadgeCode == code);
        if (e is null) return NotFound();
        db.Badges.Remove(e);
        await db.SaveChangesAsync();
        return NoContent();
    }
    [HttpPatch("{code}/toggle")]
    public async Task<IActionResult> Toggle(string code)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var e = await db.Badges.FirstOrDefaultAsync(x => x.BadgeCode == code);
        if (e is null) return NotFound(new { message = "Badge not found" });

        e.IsActive = !e.IsActive;
        await db.SaveChangesAsync();
        return Ok(new { e.BadgeCode, e.IsActive });
    }
    [HttpPatch("{code}/status")]
    public async Task<IActionResult> SetStatus(string code, [FromBody] bool active)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var e = await db.Badges.FirstOrDefaultAsync(x => x.BadgeCode == code);
        if (e is null) return NotFound(new { message = "Badge not found" });

        e.IsActive = active;
        await db.SaveChangesAsync();
        return Ok(new { e.BadgeCode, e.IsActive });
    }

    [HttpPost("products/{productId:guid}")]
    public async Task<IActionResult> SetBadgesForProduct(Guid productId, [FromBody] IEnumerable<string> codes)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var exists = await db.Products.AnyAsync(p => p.ProductId == productId);
        if (!exists) return NotFound(new { message = "Product not found" });

        var set = codes?.Select(c => c.Trim())
                       .Where(c => !string.IsNullOrWhiteSpace(c))
                       .ToHashSet(StringComparer.OrdinalIgnoreCase)
                  ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var valid = await db.Badges.Where(b => b.IsActive && set.Contains(b.BadgeCode))
                                   .Select(b => b.BadgeCode)
                                   .ToListAsync();

        var current = await db.ProductBadges.Where(p => p.ProductId == productId).ToListAsync();
        db.ProductBadges.RemoveRange(current);
        db.ProductBadges.AddRange(valid.Select(code => new ProductBadge
        {
            ProductId = productId,
            Badge = code,
            CreatedAt = DateTime.UtcNow
        }));

        await db.SaveChangesAsync();
        return NoContent();
    }
}
