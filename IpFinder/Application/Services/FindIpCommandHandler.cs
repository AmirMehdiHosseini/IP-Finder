using IpFinder.Application.CommandModels;
using IpFinder.Domain;
using IpFinder.Exceptions;
using IpFinder.ViewModels;
using System.Text.RegularExpressions;

namespace IpFinder.Application.Services
{
    public class FindIpCommandHandler : IFindIpCommandHandler
    {
        public async Task<IpViewModel> Handle(FindIpCommand request, CancellationToken cancellationToken)
        {
            var (ip, @class) = IpFinder(request.Ip!, request.MachineNumber);

            return new IpViewModel(ip, @class);
        }

        private (string, string) IpFinder(string ip, int machineNumber)
        {
            var (firstOctet, secondOctet, thirdOctet, forthOctet) = IpSpliter(ip);

            var classIp = FindClassIp(firstOctet);

            MachineNumberValidation(classIp, machineNumber);

            NetIdBoundValidation(classIp, secondOctet, thirdOctet, forthOctet);

            switch (classIp)
            {
                case "A":
                    ClassA(machineNumber, ref secondOctet, ref thirdOctet, ref forthOctet);
                    break;

                case "B":
                    ClassB(machineNumber, ref thirdOctet, ref forthOctet);
                    break;

                case "C":
                    ClassC(machineNumber, ref forthOctet);
                    break;
            }

            string requestedMachineIp = IpGenarator(firstOctet, secondOctet, thirdOctet, forthOctet);
            return (requestedMachineIp, classIp);
        }

        private void NetIdBoundValidation(string classIp, int secondOctet, int thirdOctet, int forthOctet)
        {
            switch(classIp)
            {
                case "A":
                    if (secondOctet is not 0 || thirdOctet is not 0 || forthOctet is not 0)
                        throw new InValidIpFormatException("مقدار یکی از اکتت های دوم تا چهارم از 255 بیشتر است.");
                    break;

                case "B":
                    if (thirdOctet is not 0 || forthOctet is not 0)
                        throw new InValidIpFormatException("مقدار یکی از اکتت های سوم یا چهارم از 255 بیشتر است.");
                    break;
                
                case "C":
                    if (forthOctet is not 0)
                        throw new InValidIpFormatException("مقدار اکتت چهارم از 255 بیشتر است.");
                    break;
            }
        }

        private void MachineNumberValidation(string classIp, int machineNumber)
        {
            if (machineNumber is 0)
                throw new InvalidOperationException("شماره ماشین صفر جزو بازه آی‌پی دهی نمی باشد.");

            switch (classIp)
            {
                case "A":
                    if (machineNumber > 16777214)
                        throw new InValidIpFormatException("شماره ماشین وارد شده بیشتر از ظرفیت کلاس 'آ' می باشد و باید کمتر از 16777215 باشد.");
                    break;


                case "B":
                    if (machineNumber > 65534)
                        throw new InValidIpFormatException("شماره ماشین وارد شده بیشتر از ظرفیت کلاس 'بی' می باشد و باید کمتر از 65535 باشد.");
                    break;

                case "C":
                    if (machineNumber > 254)
                        throw new InValidIpFormatException("شماره ماشین وارد شده بیشتر از ظرفیت کلاس 'سی' می باشد و باید کمتر از 255 باشد.");
                    break;

                default:
                    throw new InValidIpFormatException("آی پی وارد شده از نوع سه کلاس اول نمی باشد.");

            }
        }

        private (int firstOctet, int secondOctet, int thirdOctet, int forthOctet) IpSpliter(string ip)
        {

            IpValidation(ip);

            var octets = ip.Split(".");

            OctetValidation(octets);




            var firstOctet = int.Parse(octets[0]);
            var secondOctet = int.Parse(octets[1]);
            var thirdOctet = int.Parse(octets[2]);
            var forthOctet = int.Parse(octets[3]);

            return (firstOctet, secondOctet, thirdOctet, forthOctet);

        }

        private void OctetValidation(string[] octets)
        {
            if (octets.Length is not 4)
                throw new InValidIpFormatException("آی پی وارد شده صحیح نمی باشد.");

            foreach (var oct in octets)
                if (Regex.IsMatch(oct, @".*[^0-9]+.*"))
                    throw new InValidIpFormatException("آی پی وارد شده شامل کاراکتر غیر عددی می باشد.");

            foreach (var oct in octets)
                if (int.Parse(oct) > 255)
                    throw new InValidIpFormatException($"مقدار اکتت {oct} بیشتر از 255 می باشد.");
        }

        private void IpValidation(string ip)
        {
            if (String.IsNullOrWhiteSpace(ip))
                throw new InValidIpFormatException("آی پی وارد نشده است.");
        }

        private string FindClassIp(int firstOctet)
        {
            if (firstOctet <= 127)
                return "A";

            else if (firstOctet >= 128 && firstOctet <= 191)
                return "B";

            else if (firstOctet >= 192 && firstOctet <= 223)
                return "C";

            else if (firstOctet >= 224 && firstOctet <= 239)
                return "D";

            else
                return "E";
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
            => forthOctet += machineNumber ; 


        private string IpGenarator(int firstOctet, int secondOctet, int thirdOctet, int forthOctet)
            => firstOctet + "." + secondOctet + "." + thirdOctet + "." + forthOctet;


    }
}
