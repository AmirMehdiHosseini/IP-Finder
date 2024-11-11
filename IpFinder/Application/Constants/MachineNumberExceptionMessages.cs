namespace IpFinder.Application.Constants
{
    public static class MachineNumberExceptionMessages
    {
        public const string ZeroOrNegativeMachineNumberValue = "شماره ماشین صفر یا منفی جزو بازه آی‌پی دهی نمی باشد.";
        public const string AClassOutOfCapacity = "شماره ماشین وارد شده بیشتر از ظرفیت کلاس 'آ' می باشد و باید کمتر از 16777215 باشد.";
        public const string BClassOutOfCapacity = "شماره ماشین وارد شده بیشتر از ظرفیت کلاس 'بی' می باشد و باید کمتر از 65535 باشد.";
        public const string CClassOutOfCapacity = "شماره ماشین وارد شده بیشتر از ظرفیت کلاس 'سی' می باشد و باید کمتر از 255 باشد.";
    }
}
