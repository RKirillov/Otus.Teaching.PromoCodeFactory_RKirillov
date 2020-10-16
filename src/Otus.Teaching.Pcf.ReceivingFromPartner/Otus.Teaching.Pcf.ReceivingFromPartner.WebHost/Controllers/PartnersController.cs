﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
 using Otus.Teaching.Pcf.ReceivingFromPartner.WebHost.Models;
 using Otus.Teaching.Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
 using Otus.Teaching.Pcf.ReceivingFromPartner.Core.Abstractions.Repositories;
 using Otus.Teaching.Pcf.ReceivingFromPartner.Core.Domain;
 using Otus.Teaching.Pcf.ReceivingFromPartner.WebHost.Mappers;

 namespace Otus.Teaching.Pcf.ReceivingFromPartner.WebHost.Controllers
{
    /// <summary>
    /// Партнеры
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PartnersController
        : ControllerBase
    {
        private readonly IRepository<Partner> _partnersRepository;
        private readonly IRepository<Preference> _preferencesRepository;
        private readonly INotificationGateway _notificationGateway;
        private readonly IGivingPromoCodeToCustomerGateway _givingPromoCodeToCustomerGateway;

        public PartnersController(IRepository<Partner> partnersRepository,
            IRepository<Preference> preferencesRepository, 
            INotificationGateway notificationGateway,
            IGivingPromoCodeToCustomerGateway givingPromoCodeToCustomerGateway)
        {
            _partnersRepository = partnersRepository;
            _preferencesRepository = preferencesRepository;
            _notificationGateway = notificationGateway;
            _givingPromoCodeToCustomerGateway = givingPromoCodeToCustomerGateway;
        }

        [HttpGet]
        public async Task<ActionResult<List<PartnerResponse>>> GetPartnersAsync()
        {
            var partners = await _partnersRepository.GetAllAsync();

            var response = partners.Select(x => new PartnerResponse()
            {
                Id = x.Id,
                Name = x.Name,
                NumberIssuedPromoCodes = x.NumberIssuedPromoCodes,
                IsActive = true,
                PartnerLimits = x.PartnerLimits
                    .Select(y => new PartnerPromoCodeLimitResponse()
                    {
                        Id = y.Id,
                        PartnerId = y.PartnerId,
                        Limit = y.Limit,
                        CreateDate = y.CreateDate.ToString("dd.MM.yyyy hh:mm:ss"),
                        EndDate = y.EndDate.ToString("dd.MM.yyyy hh:mm:ss"),
                        CancelDate = y.CancelDate?.ToString("dd.MM.yyyy hh:mm:ss"),
                    }).ToList()
            });

            return Ok(response);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<List<PartnerResponse>>> GetPartnersAsync(Guid id)
        {
            var partner = await _partnersRepository.GetByIdAsync(id);

            if (partner == null)
            {
                return NotFound();
            }

            var response = new PartnerResponse()
            {
                Id = partner.Id,
                Name = partner.Name,
                NumberIssuedPromoCodes = partner.NumberIssuedPromoCodes,
                IsActive = true,
                PartnerLimits = partner.PartnerLimits
                    .Select(y => new PartnerPromoCodeLimitResponse()
                    {
                        Id = y.Id,
                        PartnerId = y.PartnerId,
                        Limit = y.Limit,
                        CreateDate = y.CreateDate.ToString("dd.MM.yyyy hh:mm:ss"),
                        EndDate = y.EndDate.ToString("dd.MM.yyyy hh:mm:ss"),
                        CancelDate = y.CancelDate?.ToString("dd.MM.yyyy hh:mm:ss"),
                    }).ToList()
            };

            return Ok(response);
        }
        
        [HttpPost("{id}/limits")]
        public async Task<IActionResult> SetPartnerPromoCodeLimitAsync(Guid id, SetPartnerPromoCodeLimitRequest request)
        {
            var partner = await _partnersRepository.GetByIdAsync(id);

            if (partner == null)
                return NotFound();
            
            //Если партнер заблокирован, то нужно выдать исключение
            if (!partner.IsActive)
                return BadRequest("Данный партнер не активен");
            
            //Установка лимита партнеру
            var activeLimit = partner.PartnerLimits.FirstOrDefault(x => 
                !x.CancelDate.HasValue);
            
            if (activeLimit != null)
            {
                //Если партнеру выставляется лимит, то мы 
                //должны обнулить количество промокодов, которые партнер выдал, если лимит закончился, 
                //то количество не обнуляется
                partner.NumberIssuedPromoCodes = 0;
                
                //При установке лимита нужно отключить предыдущий лимит
                activeLimit.CancelDate = DateTime.Now;
            }

            if (request.Limit <= 0)
                return BadRequest("Лимит должен быть больше 0");
            
            var newLimit = new PartnerPromoCodeLimit()
            {
                Limit = request.Limit,
                Partner = partner,
                PartnerId = partner.Id,
                CreateDate = DateTime.Now,
                EndDate = request.EndDate
            };
            
            partner.PartnerLimits.Add(newLimit);

            await _partnersRepository.UpdateAsync(partner);
            
            await _notificationGateway
                .SendNotificationToPartnerAsync(partner.Id, "Вам установлен лимит на отправку промокодов...");
            
            return CreatedAtAction(nameof(GetPartnerLimitAsync), new {id = partner.Id, limitId = newLimit.Id}, null);
        }
        
        [HttpGet("{id}/limits/{limitId}")]
        public async Task<ActionResult<PartnerPromoCodeLimit>> GetPartnerLimitAsync(Guid id, Guid limitId)
        {
            var partner = await _partnersRepository.GetByIdAsync(id);

            if (partner == null)
                return NotFound();
            
            var limit = partner.PartnerLimits
                .FirstOrDefault(x => x.Id == limitId);

            var response = new PartnerPromoCodeLimitResponse()
            {
                Id = limit.Id,
                PartnerId = limit.PartnerId,
                Limit = limit.Limit,
                CreateDate = limit.CreateDate.ToString("dd.MM.yyyy hh:mm:ss"),
                EndDate = limit.EndDate.ToString("dd.MM.yyyy hh:mm:ss"),
                CancelDate = limit.CancelDate?.ToString("dd.MM.yyyy hh:mm:ss"),
            };
            
            return Ok(response);
        }

        [HttpPost("{id}/canceledLimits")]
        public async Task<IActionResult> CancelPartnerPromoCodeLimitAsync(Guid id)
        {
            var partner = await _partnersRepository.GetByIdAsync(id);
            
            if (partner == null)
                return NotFound();
            
            //Если партнер заблокирован, то нужно выдать исключение
            if (!partner.IsActive)
                return BadRequest("Данный партнер не активен");
            
            //Отключение лимита
            var activeLimit = partner.PartnerLimits.FirstOrDefault(x => 
                !x.CancelDate.HasValue);
            
            if (activeLimit != null)
            {
                activeLimit.CancelDate = DateTime.Now;
            }

            await _partnersRepository.UpdateAsync(partner);

            //Отправляем уведомление
            await _notificationGateway
                .SendNotificationToPartnerAsync(partner.Id, "Ваш лимит на отправку промокодов отменен...");
            
            return NoContent();
        }
        
        /// <summary>
        /// Получить промокод партнера по id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}/promocodes")]
        public async Task<IActionResult> GetPartnerPromoCodesAsync(Guid id)
        {
            var partner = await _partnersRepository.GetByIdAsync(id);
            
            if (partner == null)
            {
                return NotFound("Партнер не найден");
            }
            
            var response = partner.PromoCodes
                .Select(x => new PromoCodeShortResponse()
            {
                Id = x.Id,
                Code = x.Code,
                BeginDate = x.BeginDate.ToString("yyyy-MM-dd"),
                EndDate = x.EndDate.ToString("yyyy-MM-dd"),
                PartnerName = x.Partner.Name,
                PartnerId = x.PartnerId,
                ServiceInfo = x.ServiceInfo
            }).ToList();

            return Ok(response);
        }
        
        /// <summary>
        /// Получить промокод партнера по id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}/promocodes/{promoCodeId}")]
        public async Task<IActionResult> GetPartnerPromoCodeAsync(Guid id, Guid promoCodeId)
        {
            var partner = await _partnersRepository.GetByIdAsync(id);
            
            if (partner == null)
            {
                return NotFound("Партнер не найден");
            }

            var promoCode = partner.PromoCodes.FirstOrDefault(x => x.Id == promoCodeId);

            if (promoCode == null)
            {
                return NotFound("Партнер не найден");
            }
            
            var response =  new PromoCodeShortResponse()
                {
                    Id = promoCode.Id,
                    Code = promoCode.Code,
                    BeginDate = promoCode.BeginDate.ToString("yyyy-MM-dd"),
                    EndDate = promoCode.EndDate.ToString("yyyy-MM-dd"),
                    PartnerName = promoCode.Partner.Name,
                    PartnerId = promoCode.PartnerId,
                    ServiceInfo = promoCode.ServiceInfo
                };

            return Ok(response);
        }
        
        /// <summary>
        /// Создать промокод от партнера 
        /// </summary>
        /// <returns></returns>
        [HttpPost("{id}/promocodes")]
        public async Task<IActionResult> ReceivePromoCodeFromPartnerWithPreferenceAsync(Guid id, ReceivingPromoCodeRequest request)
        {
            var partner = await _partnersRepository.GetByIdAsync(id);
            
            if (partner == null)
            {
                return BadRequest("Партнер не найден");
            }
            
            //Получаем предпочтение по имени
            var preference = await _preferencesRepository.GetFirstWhere(x => 
                x.Name == request.Preference);

            if (preference == null)
            {
                return BadRequest("Предпочтение не найдено");
            }

            PromoCode promoCode = PromoCodeMapper.MapFromModel(request, preference, partner);
            partner.PromoCodes.Add(promoCode);

            await _partnersRepository.UpdateAsync(partner);
            
            //То есть фактически обновили партнера, как единый агрегат
            //TODO: Чтобы информация о том, что промокод был выдан парнером была отправлена
            //в микросервис рассылки клиентам нужно либо вызвать его API, либо отправить событие в очередь
            await _givingPromoCodeToCustomerGateway.GivePromoCodeToCustomer(promoCode);
            
            return CreatedAtAction(nameof(GetPartnerPromoCodeAsync), new {id = partner.Id, promoCodeId = promoCode.Id}, null);
        }
    }
}