using CommunityManager.Models;

namespace CommunityManager.Service
{
    public interface IEmailService
    {
        Task Send(EmailMetaData emailMetaData);
    }
}
