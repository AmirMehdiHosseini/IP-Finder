namespace IpFinder.Application.Constants
{
    public static class IpExceptionMessages
    {
        public const string ClassAOctetOutOfBounds = "مقدار یکی از اکتت های دوم تا چهارم 0 نیست.";
        public const string ClassBOctetOutOfBounds = "مقدار یکی از اکتت های سوم یا چهارم 0 نیست.";
        public const string ClassCOctetOutOfBounds = "مقدار اکتت چهارم 0 نیست.";
        public const string EmptyOrWhiteSpaceIpValue = "آی پی وارد نشده است.";
        public const string FourOctetsNotFound = "فرمت آی پی وارد شده شامل چهار اکتت نیست.";
        public const string NonNumericalCharacterFound = "آی پی وارد شده شامل کاراکتر غیر عددی است.";
        public const string OctetOutOfBound = "مقدار یکی از اکتت ها بیشتر از 255 می باشد.";
        public const string EmptyOctetFound = ".یکی از اکتت ها خالی است";
    }
}
