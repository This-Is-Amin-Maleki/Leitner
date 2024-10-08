﻿using Microsoft.AspNetCore.Mvc;
using ModelsLeit.DTOs.Card;
using ModelsLeit.Entities;
using ServicesLeit.Services;
using SharedLeit;
using Microsoft.AspNetCore.Identity;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;

namespace APILeit.Controllers
{
    [Authorize(Roles = nameof(UserType.Admin))]
    [ApiController]
    public class AdminCardController: ControllerBase
    {
        private readonly CardService _cardService;
        private readonly CollectionService _collectionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminCardController(
            CardService cardService,
            CollectionService collectionService,
            UserManager<ApplicationUser> userManager)
        {
            _cardService = cardService;
            _collectionService = collectionService;
            _userManager = userManager;
        }

        // GET: CardController
        [HttpGet]
        [Route("api/[controller]/[action]/{collectionId}")]
        public async Task<ActionResult> GetAll(long collectionId,[FromQuery] [Optional] CardStatus? state)
        {
            List<CardMiniUnlimitedDto> output = await _cardService.ReadCardsUnlimitedAsync(collectionId, state);
            return Ok(output);
        }

        // GET: CardController/Details/5
        [HttpGet]
        [Route("api/[controller]/[action]/{id}")]
        public async Task<ActionResult> Get(long id) =>
            Ok(await _cardService.ReadCardAsync(id));

        // POST: CardController/Edit
        [HttpPut]
        [Route("api/[controller]/[action]")]
        public async Task<ActionResult> Edit([FromBody] CardDto model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("XX", "Not Valid!");
                return BadRequest(ModelState);
            }

#warning catch!!
            try
            {
                await _cardService.UpdateCardAsync(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("xx", ex.Message);
                return BadRequest(ModelState);
            }

            return CreatedAtAction(
                nameof(GetAll),
                new { id = model.Collection.Id },
                new { message = "Card updated successfully!" }
            );
        }

        [HttpPut]
        [Route("api/[controller]/[action]/{id}")]
        public async Task<ActionResult> TickAll(long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _cardService.TickAllCardsAsync(id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("xx", ex.Message);
                return BadRequest(ModelState);
            }

            return CreatedAtAction(
            nameof(GetAll),
                new { id },
                new { message = "Card updated successfully!" }
            );
        }

        [HttpGet]
        [Route("api/[controller]/[action]/{id}")]
        public async Task<ActionResult> CheckList([FromRoute] long id,[FromQuery] CardStatus? status,[FromQuery] int skip = 0)
        {
            CardCheckDto output;
            try
            {
                output = await _cardService.ReadCardCheck(id, status, skip);
                output.Skip = skip;
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
            return Ok(output);
        }

        [HttpPut]
        [Route("api/[controller]/[action]")]
        public async Task<ActionResult> CheckList([FromBody] CardCheckStatusDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string? content = null;
            try
            {
                var output = await _cardService.UpdateStatusAndReadNextCardCheck(model);
                if (output.Id is not 0)
                {
                    return Ok(output);
                }

                return CreatedAtAction(
                nameof(Index),
                new
                {
                    id = model.CollectionId,
                    state = output.Status
                });

            }
            catch (Exception ex)
            {
                content = ex.Message;
            }

            return CreatedAtAction(
                nameof(GetAll),
                new
                {
                    collectionId = model.CollectionId,
                    CardStatus = model.Status
                },
                new { message = content ?? "Profile updated successfully!" }
            );
        }

        [HttpPut]
        [Route("api/[controller]/[action]")]
        public async Task<ActionResult> CheckNext([FromBody] CardCheckStatusDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CardCheckDto output;
            try
            {
                output = await _cardService.ReadCardCheck(model.Id, model.Status, model.Skip);
                output.Skip = model.Skip + 1;
            }
            catch (Exception ex)
            {
                return CreatedAtAction(
                    nameof(GetAll),
                    new
                    {
                        collectionId = model.CollectionId,
                    },
                    new { message = ex.Message }
                );
            }
            return Ok(output);
        }

    }
}
