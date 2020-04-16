using Common;
using Microsoft.AspNetCore.Mvc;
using MM.Entities;
using MM.Services.MFood;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MM.Rpc.food
{
    public class FoodRoute : Root
    {
        public const string Master = Module + "/food/food-master";
        public const string Detail = Module + "/food/food-detail";
        private const string Default = Api + Module + "/food";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
    }

    public class FoodController : RpcController
    {
        private IFoodService FoodService;
        private ICurrentContext CurrentContext;
        public FoodController(
            IFoodService FoodService,
            ICurrentContext CurrentContext
        )
        {
            this.FoodService = FoodService;
            this.CurrentContext = CurrentContext;
        }

        [Route(FoodRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Food_FoodFilterDTO Food_FoodFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            FoodFilter FoodFilter = ConvertFilterDTOToFilterEntity(Food_FoodFilterDTO);
            int count = await FoodService.Count(FoodFilter);
            return count;
        }

        [Route(FoodRoute.List), HttpPost]
        public async Task<ActionResult<List<Food_FoodDTO>>> List([FromBody] Food_FoodFilterDTO Food_FoodFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            FoodFilter FoodFilter = ConvertFilterDTOToFilterEntity(Food_FoodFilterDTO);
            List<Food> Foods = await FoodService.List(FoodFilter);
            List<Food_FoodDTO> Food_FoodDTOs = Foods
                .Select(c => new Food_FoodDTO(c)).ToList();
            return Food_FoodDTOs;
        }

        [Route(FoodRoute.Get), HttpPost]
        public async Task<ActionResult<Food_FoodDTO>> Get([FromBody]Food_FoodDTO Food_FoodDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Food Food = await FoodService.Get(Food_FoodDTO.Id);
            return new Food_FoodDTO(Food);
        }

        [Route(FoodRoute.Create), HttpPost]
        public async Task<ActionResult<Food_FoodDTO>> Create([FromBody] Food_FoodDTO Food_FoodDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            Food Food = ConvertDTOToEntity(Food_FoodDTO);
            Food = await FoodService.Create(Food);
            Food_FoodDTO = new Food_FoodDTO(Food);
            if (Food.IsValidated)
                return Food_FoodDTO;
            else
                return BadRequest(Food_FoodDTO);
        }

        [Route(FoodRoute.Update), HttpPost]
        public async Task<ActionResult<Food_FoodDTO>> Update([FromBody] Food_FoodDTO Food_FoodDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            Food Food = ConvertDTOToEntity(Food_FoodDTO);
            Food = await FoodService.Update(Food);
            Food_FoodDTO = new Food_FoodDTO(Food);
            if (Food.IsValidated)
                return Food_FoodDTO;
            else
                return BadRequest(Food_FoodDTO);
        }

        [Route(FoodRoute.Delete), HttpPost]
        public async Task<ActionResult<Food_FoodDTO>> Delete([FromBody] Food_FoodDTO Food_FoodDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Food Food = ConvertDTOToEntity(Food_FoodDTO);
            Food = await FoodService.Delete(Food);
            Food_FoodDTO = new Food_FoodDTO(Food);
            if (Food.IsValidated)
                return Food_FoodDTO;
            else
                return BadRequest(Food_FoodDTO);
        }
        
        private Food ConvertDTOToEntity(Food_FoodDTO Food_FoodDTO)
        {
            Food Food = new Food();
            Food.Id = Food_FoodDTO.Id;
            Food.Name = Food_FoodDTO.Name;
            Food.PriceEach = Food_FoodDTO.PriceEach;
            Food.DiscountRate = Food_FoodDTO.DiscountRate;
            Food.Image = Food_FoodDTO.Image;
            Food.StatusId = Food_FoodDTO.StatusId;
            Food.Descreption = Food_FoodDTO.Descreption;
            Food.BaseLanguage = CurrentContext.Language;
            return Food;
        }

        private FoodFilter ConvertFilterDTOToFilterEntity(Food_FoodFilterDTO Food_FoodFilterDTO)
        {
            FoodFilter FoodFilter = new FoodFilter();
            FoodFilter.Selects = FoodSelect.ALL;
            FoodFilter.Skip = Food_FoodFilterDTO.Skip;
            FoodFilter.Take = Food_FoodFilterDTO.Take;
            FoodFilter.OrderBy = Food_FoodFilterDTO.OrderBy;
            FoodFilter.OrderType = Food_FoodFilterDTO.OrderType;

            FoodFilter.Id = Food_FoodFilterDTO.Id;
            FoodFilter.Name = Food_FoodFilterDTO.Name;
            FoodFilter.PriceEach = Food_FoodFilterDTO.PriceEach;
            FoodFilter.DiscountRate = Food_FoodFilterDTO.DiscountRate;
            FoodFilter.Image = Food_FoodFilterDTO.Image;
            FoodFilter.StatusId = Food_FoodFilterDTO.StatusId;
            FoodFilter.Descreption = Food_FoodFilterDTO.Descreption;
            return FoodFilter;
        }


    }
}

