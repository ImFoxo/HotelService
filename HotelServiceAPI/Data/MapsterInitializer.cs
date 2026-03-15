using HotelServiceAPI.DTOs;
using HotelServiceAPI.Models;
using Mapster;

namespace To_Do_app_server.Data
{
    public class MapsterInitializer
    {
        public static void SetMapsterConfig()
        {
            TypeAdapterConfig<Resource, ResourceGetDTO>.NewConfig()
                .Map(dest => dest.SeatIds, src => src.Seats.Select(s => s.Id));

        }
    }
}
