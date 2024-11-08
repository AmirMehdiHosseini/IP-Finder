using IpFinder.ViewModels;
using MediatR;

namespace IpFinder.Application.CommandModels
{
    public record FindIpCommand(string? Ip,int MachineNumber) : IRequest<IpViewModel>;
}
