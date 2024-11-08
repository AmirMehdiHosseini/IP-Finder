using IpFinder.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using IpFinder.DTOs;
using IpFinder.Application.CommandModels;
namespace IpFinder.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IpFinderController(IMediator mediator) : ControllerBase
    {

        [HttpPost()]
        public async Task<IpViewModel> FindIp([FromQuery] FindIpCommandModel requestModel)
        {
            var commandModel = new FindIpCommand(requestModel.Ip, requestModel.MachineNumber);
            return await mediator.Send(commandModel);
        }
    }
}
