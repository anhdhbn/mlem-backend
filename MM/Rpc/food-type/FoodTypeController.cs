using Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MM.Entities;
using MM.Services.MFoodType;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MM.Rpc.food_type
{
    public class FoodTypeRoute : Root
    {
        public const string Master = Module + "/food-type/food-type-master";
        public const string Detail = Module + "/food-type/food-type-detail";
        private const string Default = Api + Module + "/food-type";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
    }

    public class FoodTypeController : RpcController
    {
        private IFoodTypeService FoodTypeService;
        private ICurrentContext CurrentContext;
        public FoodTypeController(
            IFoodTypeService FoodTypeService,
            ICurrentContext CurrentContext
        )
        {
            this.FoodTypeService = FoodTypeService;
            this.CurrentContext = CurrentContext;
        }

        [Route(FoodTypeRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] FoodType_FoodTypeFilterDTO FoodType_FoodTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            FoodTypeFilter FoodTypeFilter = ConvertFilterDTOToFilterEntity(FoodType_FoodTypeFilterDTO);
            int count = await FoodTypeService.Count(FoodTypeFilter);
            return count;
        }

        [Route(FoodTypeRoute.List), HttpPost]
        public async Task<ActionResult<List<FoodType_FoodTypeDTO>>> List([FromBody] FoodType_FoodTypeFilterDTO FoodType_FoodTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            FoodTypeFilter FoodTypeFilter = ConvertFilterDTOToFilterEntity(FoodType_FoodTypeFilterDTO);
            List<FoodType> FoodTypes = await FoodTypeService.List(FoodTypeFilter);
            List<FoodType_FoodTypeDTO> FoodType_FoodTypeDTOs = FoodTypes
                .Select(c => new FoodType_FoodTypeDTO(c)).ToList();
            return FoodType_FoodTypeDTOs;
        }

        [Route(FoodTypeRoute.Get), HttpPost]
        public async Task<ActionResult<FoodType_FoodTypeDTO>> Get([FromBody]FoodType_FoodTypeDTO FoodType_FoodTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            FoodType FoodType = await FoodTypeService.Get(FoodType_FoodTypeDTO.Id);
            return new FoodType_FoodTypeDTO(FoodType);
        }

        [Route(FoodTypeRoute.Create), HttpPost]
        public async Task<ActionResult<FoodType_FoodTypeDTO>> Create([FromBody] FoodType_FoodTypeDTO FoodType_FoodTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            FoodType FoodType = ConvertDTOToEntity(FoodType_FoodTypeDTO);
            FoodType = await FoodTypeService.Create(FoodType);
            FoodType_FoodTypeDTO = new FoodType_FoodTypeDTO(FoodType);
            if (FoodType.IsValidated)
                return FoodType_FoodTypeDTO;
            else
                return BadRequest(FoodType_FoodTypeDTO);
        }

        [Route(FoodTypeRoute.Update), HttpPost]
        public async Task<ActionResult<FoodType_FoodTypeDTO>> Update([FromBody] FoodType_FoodTypeDTO FoodType_FoodTypeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            FoodType FoodType = ConvertDTOToEntity(FoodType_FoodTypeDTO);
            FoodType = await FoodTypeService.Update(FoodType);
            FoodType_FoodTypeDTO = new FoodType_FoodTypeDTO(FoodType);
            if (FoodType.IsValidated)
                return FoodType_FoodTypeDTO;
            else
                return BadRequest(FoodType_FoodTypeDTO);
        }

        private FoodType ConvertDTOToEntity(FoodType_FoodTypeDTO FoodType_FoodTypeDTO)
        {
            FoodType FoodType = new FoodType();
            FoodType.Id = FoodType_FoodTypeDTO.Id;
            FoodType.Name = FoodType_FoodTypeDTO.Name;
            FoodType.StatusId = FoodType_FoodTypeDTO.StatusId;
            FoodType.BaseLanguage = CurrentContext.Language;
            return FoodType;
        }

        private FoodTypeFilter ConvertFilterDTOToFilterEntity(FoodType_FoodTypeFilterDTO FoodType_FoodTypeFilterDTO)
        {
            FoodTypeFilter FoodTypeFilter = new FoodTypeFilter();
            FoodTypeFilter.Selects = FoodTypeSelect.ALL;
            FoodTypeFilter.Skip = FoodType_FoodTypeFilterDTO.Skip;
            FoodTypeFilter.Take = FoodType_FoodTypeFilterDTO.Take;
            FoodTypeFilter.OrderBy = FoodType_FoodTypeFilterDTO.OrderBy;
            FoodTypeFilter.OrderType = FoodType_FoodTypeFilterDTO.OrderType;

            FoodTypeFilter.Id = FoodType_FoodTypeFilterDTO.Id;
            FoodTypeFilter.Name = FoodType_FoodTypeFilterDTO.Name;
            FoodTypeFilter.StatusId = FoodType_FoodTypeFilterDTO.StatusId;
            return FoodTypeFilter;
        }


    }
}

