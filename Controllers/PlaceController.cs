using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace APIDesafio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaceController : ControllerBase
    {
        private readonly IConfiguration _config;
        public PlaceController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<Place>>> GelAllPlaces()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var places = await connection.QueryAsync<Place>("select * from place");
            return Ok(places);
        }

        [HttpGet("{placeId}")]
        public async Task<ActionResult<Place>> GelPlace(int placeId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var place = await connection.QueryFirstAsync<Place>("select * from place where id = @Id", 
                new {Id = placeId});
            return Ok(place);
        }

        [HttpPost]
        public async Task<ActionResult<List<Place>>> CreatePlace(Place newPlace)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("insert into place (nome, endereco, telefone) values (@Nome, @Endereco, @Telefone)", newPlace); 
            return Ok(await SelectAllPlaces(connection));

        }

        [HttpPut]
        public async Task<ActionResult<List<Place>>> UpdatePlace(Place newPlace)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update place set name = @Name, endereco = @Endereco, telefone = @Telefone where id = @Id", newPlace);
            return Ok(await SelectAllPlaces(connection));

        }

        [HttpDelete("{placeId}")]
        public async Task<ActionResult<List<Place>>> DeletePlace(int placeId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from Place where id = @Id", new {Id = placeId});
            return Ok(await SelectAllPlaces(connection));

        }

        private static async Task<IEnumerable<Place>> SelectAllPlaces(SqlConnection connection)
        {
            return await connection.QueryAsync<Place>("Select * from place");
        }




    }
}
