using IpFinder.Application.CommandModels;
using IpFinder.ViewModels;
using MediatR;

namespace IpFinder.Domain
{
    public interface IFindIpCommandHandler : IRequestHandler<FindIpCommand, IpViewModel>;
}
