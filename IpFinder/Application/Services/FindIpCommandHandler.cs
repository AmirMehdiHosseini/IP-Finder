using IpFinder.Application.CommandModels;
using IpFinder.Application.Constants;
using IpFinder.Domain;
using IpFinder.Exceptions;
using IpFinder.ViewModels;
using System.Text.RegularExpressions;

namespace IpFinder.Application.Services
{
    public class FindIpCommandHandler : IFindIpCommandHandler
    {
        private const string DOT = ".";
        private const string IncludeNonNumericalCharacterRegesSample = @".*[^0-9]+.*";
        public async Task<IpViewModel> Handle(FindIpCommand request, CancellationToken cancellationToken)
        {
            var (ip, @class) = await IpFinderAsync(request.Ip!, request.MachineNumber);

            return new IpViewModel(ip, @class.Result.ToString()!);
        }

        private async Task<(string, Task<string>)> IpFinderAsync(string ip, int machineNumber)
        {

            var (firstOctet, secondOctet, thirdOctet, forthOctet) = await IpSpliterAsync(ip);

            var classIp = FindClassIpAsync(firstOctet);

            await Task.WhenAll(
                MachineNumberValidationAsync(classIp, machineNumber),
                NetIdOctetBoundValidationAsync(classIp, secondOctet, thirdOctet, forthOctet)
            );

            switch (classIp.Result.ToString())
            {
                case IpClass.A:
                    ClassA(machineNumber, ref secondOctet, ref thirdOctet, ref forthOctet);
                    break;

                case IpClass.B:
                    ClassB(machineNumber, ref thirdOctet, ref forthOctet);
                    break;

                case IpClass.C:
                    ClassC(machineNumber, ref forthOctet);
                    break;
            }

            var requestedMachineIp = await IpGenaratorAsync(firstOctet, secondOctet, thirdOctet, forthOctet);
            return (requestedMachineIp, classIp);
        }

        private async Task NetIdOctetBoundValidationAsync(Task<string> classIp, int secondOctet, int thirdOctet, int forthOctet)
        {
            switch (classIp.Result.ToString())
            {
                case IpClass.A:
                    if (secondOctet is not 0 || thirdOctet is not 0 || forthOctet is not 0)
                        throw new InValidIpFormatException(IpExceptionMessages.ClassAOctetOutOfBounds);
                    break;

                case IpClass.B:
                    if (thirdOctet is not 0 || forthOctet is not 0)
                        throw new InValidIpFormatException(IpExceptionMessages.ClassBOctetOutOfBounds);
                    break;

                case IpClass.C:
                    if (forthOctet is not 0)
                        throw new InValidIpFormatException(IpExceptionMessages.ClassCOctetOutOfBounds);
                    break;
            }
        }

        private async Task MachineNumberValidationAsync(Task<string> classIp, int machineNumber)
        {
            if (machineNumber <= 0)
                throw new InValidMachineIpException(MachineNumberExceptionMessages.ZeroOrNegativeMachineNumberValue);

            switch (classIp.Result.ToString())
            {
                case IpClass.A:
                    if (machineNumber > 16777214)
                        throw new InValidMachineIpException(MachineNumberExceptionMessages.AClassOutOfCapacity);
                    break;

                case IpClass.B:
                    if (machineNumber > 65534)
                        throw new InValidMachineIpException(MachineNumberExceptionMessages.BClassOutOfCapacity);
                    break;

                case IpClass.C:
                    if (machineNumber > 254)
                        throw new InValidMachineIpException(MachineNumberExceptionMessages.CClassOutOfCapacity);
                    break;
            }
        }

        private async Task<(int firstOctet, int secondOctet, int thirdOctet, int forthOctet)> IpSpliterAsync(string ip)
        {

            await IpValidationAsync(ip);

            var octets = ip.Split(DOT); 

            await OctetValidationAsync(octets);


            var firstOctet = int.Parse(octets[0]);
            var secondOctet = int.Parse(octets[1]);
            var thirdOctet = int.Parse(octets[2]);
            var forthOctet = int.Parse(octets[3]);

            return (firstOctet, secondOctet, thirdOctet, forthOctet);

        }

        private async Task OctetValidationAsync(string[] octets)
        {
            if (octets.Length is not 4)
                throw new InValidIpFormatException(IpExceptionMessages.FourOctetsNotFound);

            foreach (var octet in octets)
            {
                if (Regex.IsMatch(octet, IncludeNonNumericalCharacterRegesSample))
                    throw new InValidIpFormatException(IpExceptionMessages.NonNumericalCharacterFound);

                else if (String.IsNullOrWhiteSpace(octet))
                    throw new InValidIpFormatException(IpExceptionMessages.EmptyOctetFound);
                
                else if (int.Parse(octet) > 255)
                    throw new InValidIpFormatException(IpExceptionMessages.OctetOutOfBound);
            }
        }

        private async Task IpValidationAsync(string ip)
        {
            if (String.IsNullOrWhiteSpace(ip))
                throw new InValidIpFormatException(IpExceptionMessages.EmptyOrWhiteSpaceIpValue);
        }

        private async Task<string> FindClassIpAsync(int firstOctet)
        {
            if (firstOctet <= 127)
                return IpClass.A;

            else if (firstOctet >= 128 && firstOctet <= 191)
                return IpClass.B;

            else if (firstOctet >= 192 && firstOctet <= 223)
                return IpClass.C;

            else if (firstOctet >= 224 && firstOctet <= 239)
                return IpClass.D;

            else
                return IpClass.E;
        }

        private void ClassA(int machineNumber, ref int secondOctet, ref int thirdOctet, ref int forthOctet)
        {
            secondOctet += (int)machineNumber / 65536;
            machineNumber %= 65536;
            ClassB(machineNumber, ref thirdOctet, ref forthOctet);
        }

        private void ClassB(int machineNumber, ref int thirdOctet, ref int forthOctet)
        {
            thirdOctet += (int)machineNumber / 256;
            machineNumber %= 256;
            ClassC(machineNumber, ref forthOctet);
        }

        private void ClassC(int machineNumber, ref int forthOctet)
            => forthOctet += machineNumber;


        private async Task<string> IpGenaratorAsync(int firstOctet, int secondOctet, int thirdOctet, int forthOctet)
            => firstOctet + DOT + secondOctet + DOT + thirdOctet + DOT + forthOctet;


    }
}
