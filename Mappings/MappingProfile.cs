using AutoMapper;
using AgroindustryManagementAPI.Models;
using AgroManagementAPI.DTOs. V1. Worker;
using AgroManagementAPI.DTOs.V1. Field;
using AgroManagementAPI.DTOs.V1. Machine;
using AgroManagementAPI.DTOs.V1. WorkerTask;
using AgroManagementAPI.DTOs.V1. Resource;
using AgroManagementAPI.DTOs.V1. Warehouse;
using AgroManagementAPI.DTOs.V1. InventoryItem;
using AgroManagementAPI.DTOs.V1. FieldDetails;
using AgroManagementAPI.Models;

namespace AgroManagementAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ============ WORKER MAPPINGS ============
            CreateMap<Worker, WorkerResponseDto>(). ReverseMap();
            CreateMap<WorkerCreateDto, Worker>();
            CreateMap<WorkerUpdateDto, Worker>();

            // ============ FIELD MAPPINGS ============
            CreateMap<Field, FieldResponseDto>()
                .ForMember(dest => dest.Workers, opt => opt.MapFrom(src => src.Workers))
                .ForMember(dest => dest. Machines, opt => opt.MapFrom(src => src.Machines))
                .ForMember(dest => dest.Tasks, opt => opt.MapFrom(src => src.Tasks));
            
            CreateMap<FieldCreateDto, Field>();
            CreateMap<FieldUpdateDto, Field>();

            // ============ MACHINE MAPPINGS ============
            CreateMap<Machine, MachineResponseDto>()
                .ForMember(dest => dest.Resource, opt => opt.MapFrom(src => src.Resource));
            
            CreateMap<MachineCreateDto, Machine>();
            CreateMap<MachineUpdateDto, Machine>();

            // ============ WORKER TASK MAPPINGS ============
            CreateMap<WorkerTask, WorkerTaskResponseDto>();
            
            CreateMap<WorkerTaskCreateDto, WorkerTask>();
            CreateMap<WorkerTaskUpdateDto, WorkerTask>();

            // ============ RESOURCE MAPPINGS ============
            CreateMap<Resource, ResourceResponseDto>()
                .ForMember(dest => dest.RequiredMachines, opt => opt.MapFrom(src => src.RequiredMachines));
            
            CreateMap<ResourceCreateDto, Resource>();
            CreateMap<ResourceUpdateDto, Resource>();

            // ============ WAREHOUSE MAPPINGS ============
            CreateMap<Warehouse, WarehouseResponseDto>()
                . ForMember(dest => dest. InventoryItems, opt => opt.MapFrom(src => src.InventoryItems));
            
            CreateMap<WarehouseCreateDto, Warehouse>();
            CreateMap<WarehouseUpdateDto, Warehouse>();

            // ============ INVENTORY ITEM MAPPINGS ============
            CreateMap<InventoryItem, InventoryItemResponseDto>();
            CreateMap<InventoryItemCreateDto, InventoryItem>();
            CreateMap<InventoryItemUpdateDto, InventoryItem>();

            // ============ FIELD DETAILS MAPPINGS ============
            CreateMap<FieldDetails, FieldDetailsResponseDto>()
                .ForMember(dest => dest.Workers, opt => opt.MapFrom(src => src.Workers))
                .ForMember(dest => dest.Machines, opt => opt. MapFrom(src => src. Machines))
                .ForMember(dest => dest.Tasks, opt => opt.MapFrom(src => src.Tasks));
        }
    }
}