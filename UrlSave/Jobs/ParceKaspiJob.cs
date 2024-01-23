using Hangfire;

namespace UrlSave.Jobs
{
    public class ParceKaspiJob
    {
        private readonly ILogger<ParceKaspiJob> _logger;

        public ParceKaspiJob(ILogger<ParceKaspiJob> logger) => _logger = logger;

        [JobDisplayName("Send console log")]
        public void Execute()
        {
            _logger.LogInformation("HELLO:" + DateTime.Now);
        }
    }
}
