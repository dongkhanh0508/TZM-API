namespace TradeMap.Service.Helpers
{
    public static class StatusSurveyRequestEnum
    {
        public enum StatusRequest
        {
            All = 0,
            Surveyed = 1,
            NeedSurvey = 2,
            WaitingApproval = 3,
            Rejectd = 4,
        }
    }
}