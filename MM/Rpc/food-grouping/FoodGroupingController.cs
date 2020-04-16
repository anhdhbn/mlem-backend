using Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MM.Entities;
using MM.Services.MFoodGrouping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MM.Rpc.food_grouping
{
    public class FoodGroupingRoute : Root
    {
        public const string Master = Module + "/food-grouping/food-grouping-master";
        public const string Detail = Module + "/food-grouping/food-grouping-detail";
        private const string Default = Api + Module + "/food-grouping";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
    }

    public class FoodGroupingController : RpcController
    {
        private IFoodGroupingService FoodGroupingService;
        private ICurrentContext CurrentContext;
        public FoodGroupingController(
            IFoodGroupingService FoodGroupingService,
            ICurrentContext CurrentContext
        )
        {
            this.FoodGroupingService = FoodGroupingService;
            this.CurrentContext = CurrentContext;
        }

        [Route(FoodGroupingRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] FoodGrouping_FoodGroupingFilterDTO FoodGrouping_FoodGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            FoodGroupingFilter FoodGroupingFilter = ConvertFilterDTOToFilterEntity(FoodGrouping_FoodGroupingFilterDTO);
            int count = await FoodGroupingService.Count(FoodGroupingFilter);
            return count;
        }

        [Route(FoodGroupingRoute.List), HttpPost]
        public async Task<ActionResult<List<FoodGrouping_FoodGroupingDTO>>> List([FromBody] FoodGrouping_FoodGroupingFilterDTO FoodGrouping_FoodGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            FoodGroupingFilter FoodGroupingFilter = ConvertFilterDTOToFilterEntity(FoodGrouping_FoodGroupingFilterDTO);
            List<FoodGrouping> FoodGroupings = await FoodGroupingService.List(FoodGroupingFilter);
            List<FoodGrouping_FoodGroupingDTO> FoodGrouping_FoodGroupingDTOs = FoodGroupings
                .Select(c => new FoodGrouping_FoodGroupingDTO(c)).ToList();
            return FoodGrouping_FoodGroupingDTOs;
        }

        [Route(FoodGroupingRoute.Get), HttpPost]
        public async Task<ActionResult<FoodGrouping_FoodGroupingDTO>> Get([FromBody]FoodGrouping_FoodGroupingDTO FoodGrouping_FoodGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            FoodGrouping FoodGrouping = await FoodGroupingService.Get(FoodGrouping_FoodGroupingDTO.Id);
            return new FoodGrouping_FoodGroupingDTO(FoodGrouping);
        }

        [Route(FoodGroupingRoute.Create), HttpPost]
        public async Task<ActionResult<FoodGrouping_FoodGroupingDTO>> Create([FromBody] FoodGrouping_FoodGroupingDTO FoodGrouping_FoodGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            FoodGrouping FoodGrouping = ConvertDTOToEntity(FoodGrouping_FoodGroupingDTO);
            FoodGrouping = await FoodGroupingService.Create(FoodGrouping);
            FoodGrouping_FoodGroupingDTO = new FoodGrouping_FoodGroupingDTO(FoodGrouping);
            if (FoodGrouping.IsValidated)
                return FoodGrouping_FoodGroupingDTO;
            else
                return BadRequest(FoodGrouping_FoodGroupingDTO);
        }

        [Route(FoodGroupingRoute.Update), HttpPost]
        public async Task<ActionResult<FoodGrouping_FoodGroupingDTO>> Update([FromBody] FoodGrouping_FoodGroupingDTO FoodGrouping_FoodGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            FoodGrouping FoodGrouping = ConvertDTOToEntity(FoodGrouping_FoodGroupingDTO);
            FoodGrouping = await FoodGroupingService.Update(FoodGrouping);
            FoodGrouping_FoodGroupingDTO = new FoodGrouping_FoodGroupingDTO(FoodGrouping);
            if (FoodGrouping.IsValidated)
                return FoodGrouping_FoodGroupingDTO;
            else
                return BadRequest(FoodGrouping_FoodGroupingDTO);
        }

        [Route(FoodGroupingRoute.Delete), HttpPost]
        public async Task<ActionResult<FoodGrouping_FoodGroupingDTO>> Delete([FromBody] FoodGrouping_FoodGroupingDTO FoodGrouping_FoodGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            FoodGrouping FoodGrouping = ConvertDTOToEntity(FoodGrouping_FoodGroupingDTO);
            FoodGrouping = await FoodGroupingService.Delete(FoodGrouping);
            FoodGrouping_FoodGroupingDTO = new FoodGrouping_FoodGroupingDTO(FoodGrouping);
            if (FoodGrouping.IsValidated)
                return FoodGrouping_FoodGroupingDTO;
            else
                return BadRequest(FoodGrouping_FoodGroupingDTO);
        }
        
        private FoodGrouping ConvertDTOToEntity(FoodGrouping_FoodGroupingDTO FoodGrouping_FoodGroupingDTO)
        {
            FoodGrouping FoodGrouping = new FoodGrouping();
            FoodGrouping.Id = FoodGrouping_FoodGroupingDTO.Id;
            FoodGrouping.Name = FoodGrouping_FoodGroupingDTO.Name;
            FoodGrouping.StatusId = FoodGrouping_FoodGroupingDTO.StatusId;
            FoodGrouping.BaseLanguage = CurrentContext.Language;
            return FoodGrouping;
        }

        private FoodGroupingFilter ConvertFilterDTOToFilterEntity(FoodGrouping_FoodGroupingFilterDTO FoodGrouping_FoodGroupingFilterDTO)
        {
            FoodGroupingFilter FoodGroupingFilter = new FoodGroupingFilter();
            FoodGroupingFilter.Selects = FoodGroupingSelect.ALL;
            FoodGroupingFilter.Skip = FoodGrouping_FoodGroupingFilterDTO.Skip;
            FoodGroupingFilter.Take = FoodGrouping_FoodGroupingFilterDTO.Take;
            FoodGroupingFilter.OrderBy = FoodGrouping_FoodGroupingFilterDTO.OrderBy;
            FoodGroupingFilter.OrderType = FoodGrouping_FoodGroupingFilterDTO.OrderType;

            FoodGroupingFilter.Id = FoodGrouping_FoodGroupingFilterDTO.Id;
            FoodGroupingFilter.Name = FoodGrouping_FoodGroupingFilterDTO.Name;
            FoodGroupingFilter.StatusId = FoodGrouping_FoodGroupingFilterDTO.StatusId;
            return FoodGroupingFilter;
        }


    }
}

