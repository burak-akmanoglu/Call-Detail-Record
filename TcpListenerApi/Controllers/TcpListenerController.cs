using EntityLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataAccesLayer;
using Microsoft.AspNetCore.Authorization;

namespace TcpListenerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TcpListenerController : ControllerBase
    {
       // [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult ClientList()
        {
            using var c = new Context();
            return Ok(c.TblClientData.ToList());
        }
        [HttpPost]
        public IActionResult ClientAdd(ClientData p)
        {
            using var c = new Context();
            c.Add(p);
            c.SaveChanges();
            return Created("", p);
        }
        [HttpGet("{id}")]
        public IActionResult ClientGet(int id)
        {
            using var c = new Context();
            var value = c.TblClientData.Find(id);

            if (value == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(value);
            }

        }
        [HttpDelete]
        public IActionResult ClientDelete(int id)
        {
            using var c = new Context();
            var value = c.TblClientData.Find(id);
            if (value == null)
            {
                return NotFound();
            }
            else
            {
                c.Remove(value);
                c.SaveChanges();
                return NoContent();
            }
        }
        [HttpPut]
        public IActionResult UpdateClient(ClientData parametre)
        {
            using var c = new Context();
            var value = c.Find<ClientData>(parametre.ClientId);

            if (value == null)
            {
                return NotFound();

            }
            else
            {
                value.TelephoneNumber = parametre.TelephoneNumber;
                value.TargetTelephoneNumber = parametre.TargetTelephoneNumber;
                value.RingTime = parametre.RingTime;
                value.StartTime = parametre.StartTime;
                value.FinishTime = parametre.FinishTime;
                value.Date = parametre.Date;
                value.CallTime = parametre.Date;

                c.Update(value);
                c.SaveChanges();
                return NoContent();
            }
        }
    }
}
